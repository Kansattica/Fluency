using System;
using System.Collections.Generic;
using System.Linq;
using Fluency.Common;
using Fluency.Execution.Exceptions;
using Fluency.Execution.Extensions;

namespace Fluency.Execution.Functions.BuiltIn
{

    /// <summary>
    /// Wrap a C# function that takes two arguments and returns many values as a Fluency function.
    /// Works just like <see cref="WrapBinary{TRealTop, TRealBottom, TRealOut}"/>, except it wraps something that returns more than one value.
    /// - If no arguments given, read something from the top and the bottom and do the operation on them.
    /// - If one argument given, read something from the top and do the operation with that and the argument.
    /// This is how Split is implemented.
    /// </summary>
    public class WrapBinaryStreamOutput<TRealTop, TRealBottom, TRealOut> : ITopIn, IBottomIn, ITopOut
    {
        private readonly Func<TRealTop, TRealBottom, IEnumerable<TRealOut>> function;
        private readonly FluencyType returnType;
        private readonly Value stored;

        public string Name { get; private set; }

        public GetNext TopInput { private get; set; }
        public GetNext BottomInput { private get; set; }

        public WrapBinaryStreamOutput(Func<TRealTop, TRealBottom, IEnumerable<TRealOut>> function, FluencyType returnType, FluencyType inputType, string name, Value[] arguments)
        {
            this.function = function;
            this.returnType = returnType;
            Name = name;

            if (arguments.Length == 1)
            {
                stored = arguments[0];
                if (inputType != FluencyType.Any && stored.Type != inputType)
                    throw new ExecutionException("Function {0} takes a {1}. You supplied a {2}: {3}", name, inputType, stored.Type, stored.ToString());

            }
            else if (arguments.Length > 1)
            {
                throw new ExecutionException("Function {0} takes either zero or one parameters. You supplied {1}: {2}", name, arguments.Length, string.Join(", ", arguments.Select(x => x.ToString())));
            }
        }

        Queue<Value> unsent = new Queue<Value>();

        public Value Top()
        {
            if (unsent.Count == 0)
            {
                Value top = TopInput();
                Value bottom = stored ?? BottomInput();
                if (top && bottom)
                    unsent.EnqueueRange(function(top.Get<TRealTop>(returnType), bottom.Get<TRealBottom>(returnType)).Select(x => new Value(x, returnType)));
                else
                    return Value.Finished;
            }
            
            return unsent.Dequeue();

        }
    }
}