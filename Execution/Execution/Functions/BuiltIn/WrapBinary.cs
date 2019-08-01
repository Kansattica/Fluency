using System;
using System.Linq;
using Fluency.Common;
using Fluency.Execution.Exceptions;

namespace Fluency.Execution.Functions.BuiltIn
{

    /// <summary>
    /// Wrap a C# function that takes two arguments and returns one value as a Fluency function that:
    /// - If no arguments given, read something from the top and the bottom and do the operation on them.
    /// - If one argument given, read something from the top and do the operation with that and the argument.
    /// This is how Add, Mult, Equals, And, Or, AddDouble, and Concat are implemented.
    /// </summary>
    public class WrapBinary<TRealTop, TRealBottom, TRealOut> : ITopIn, IBottomIn, ITopOut
    {
        private readonly Func<TRealTop, TRealBottom, TRealOut> function;
        private readonly FluencyType type;
        private readonly Value stored;

        public string Name { get; private set; }

        public GetNext TopInput { private get; set; }
        public GetNext BottomInput { private get; set; }

        public WrapBinary(Func<TRealTop, TRealBottom, TRealOut> function, FluencyType argType, FluencyType returnType, string name, Value[] arguments)
        {
            this.function = function;
            this.type = returnType;
            Name = name;

            if (arguments.Length == 1)
            {
                stored = arguments[0];
                if (argType != FluencyType.Any && stored.Type != argType)
                    throw new ExecutionException("Function {0} takes a {1}. You supplied a {2}: {3}", name, returnType, stored.Type, stored.ToString());

            }
            else if (arguments.Length > 1)
            {
                throw new ExecutionException("Function {0} takes either zero or one parameters. You supplied {1}: {2}", name, arguments.Length, string.Join(", ", arguments.Select(x => x.ToString())));
            }
        }

        public Value Top()
        {
            Value top = TopInput();
            Value bottom = stored ?? (BottomInput != null ? BottomInput() : throw new ExecutionException("{0} tried to read from a bottom input that doesn't exist. Are you missing a pipeline?", Name));

            if (top && bottom)
                return new Value(function(top.Get<TRealTop>(type), bottom.Get<TRealBottom>(type)), type);

            return Value.Finished;
        }
    }
}