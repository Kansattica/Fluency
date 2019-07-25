using System;
using System.Linq;
using Fluency.Interpreter.Common;
using Fluency.Interpreter.Execution.Exceptions;

namespace Fluency.Interpreter.Execution.Functions.BuiltIn
{
    
    /// <summary>
    /// Wrap a math function with one output.
    /// </summary>
    public class WrapMath<TReal> : ITopIn, IBottomIn, ITopOut
    {
        private readonly Func<TReal, TReal, TReal> function;
        private readonly FluencyType type;
        private readonly Value stored;

        public string Name { get; private set; }

        public GetNext TopInput { private get; set; }
        public GetNext BottomInput  { private get; set; }

        public WrapMath(Func<TReal, TReal, TReal> function, FluencyType type, string name, Value[] arguments)
        {
            this.function = function;
            this.type = type;
            Name = name;

            if (arguments.Length == 1)
            {
                stored = arguments[0];
                if (stored.Type != type)
                    throw new ExecutionException("Function {0} takes a {1}. You supplied a {2}: {3}", name, type, stored.Type, stored.ToString());

            }
            else if (arguments.Length > 1)
            {
                throw new ExecutionException("Function {0} takes either zero or one parameters. You supplied {1}: {2}", name, arguments.Length, string.Join(", ", arguments.Select(x => x.ToString())));
            }
        }

        public Value Top()
        {
            Value top = TopInput(), bottom = BottomInput();

            if (top && bottom)
                return new Value(function(top.Get<TReal>(type), bottom.Get<TReal>(type)), type);

            return Value.Finished;
        }
    }
}