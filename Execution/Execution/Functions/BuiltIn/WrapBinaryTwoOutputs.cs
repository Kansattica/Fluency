using System;
using System.Collections.Generic;
using System.Linq;
using Fluency.Common;
using Fluency.Execution.Exceptions;

namespace Fluency.Execution.Functions.BuiltIn
{

    /// <summary>
    /// Wrap a C# function that takes two arguments and returns two values as a Fluency function.
    /// Works just like <see cref="WrapBinary{TRealTop, TRealBottom, TRealOut}"/>, except it wraps something that returns exactly two values.
    /// - If no arguments given, read something from the top and the bottom and do the operation on them, returning something on top and something on the bottom.
    /// - If one argument given, read something from the top and do the operation with that and the argument.
    /// This is how DivMod is implemented.
    /// </summary>
    public class WrapBinaryTwoOutputs<TRealTop, TRealBottom, TRealTopOut, TRealBottomOut> : ITopIn, IBottomIn, ITopOut, IBottomOut
    {
        private readonly Func<TRealTop, TRealBottom, (TRealTopOut, TRealBottomOut)> function;
        private readonly FluencyType topReturn;
        private readonly FluencyType bottomReturn;
        private readonly Value stored;

        public string Name { get; private set; }

        public GetNext TopInput { private get; set; }
        public GetNext BottomInput { private get; set; }

        public WrapBinaryTwoOutputs(Func<TRealTop, TRealBottom, (TRealTopOut, TRealBottomOut)> function, FluencyType argType,
                                                FluencyType topReturnType, FluencyType bottomReturnType, string name, Value[] arguments)
        {
            this.function = function;
            this.topReturn = topReturnType;
            this.bottomReturn = bottomReturnType;
            Name = name;

            if (arguments.Length == 1)
            {
                stored = arguments[0];
                if (argType != FluencyType.Any && stored.Type != argType)
                    throw new ExecutionException("Function {0} takes a {1}. You supplied a {2}: {3}", name, topReturnType, stored.Type, stored.ToString());

            }
            else if (arguments.Length > 1)
            {
                throw new ExecutionException("Function {0} takes either zero or one parameters. You supplied {1}: {2}", name, arguments.Length, string.Join(", ", arguments.Select(x => x.ToString())));
            }
        }


        private Queue<Value> _top = new Queue<Value>();
        private Queue<Value> _bottom = new Queue<Value>();

        public Value TryGetFromQueue(Queue<Value> queue)
        {
            if (queue.Count == 0)
            {
                Value nextTop = TopInput();
                Value nextBottom = (stored ?? BottomInput());

                if (nextTop.Done || nextBottom.Done)
                {
                    _top.Enqueue(Value.Finished);
                    _bottom.Enqueue(Value.Finished);
                    return Value.Finished;
                }

                (TRealTopOut top, TRealBottomOut bottom) = function(nextTop.Get<TRealTop>(), nextBottom.Get<TRealBottom>());


                _top.Enqueue(new Value(top, topReturn));
                _bottom.Enqueue(new Value(bottom, bottomReturn));
            }

            return queue.Dequeue();
        }

        public Value Top()
        {
            return TryGetFromQueue(_top);
        }

        public Value Bottom()
        {
            return TryGetFromQueue(_bottom);
        }
    }
}