using System;
using System.Linq;
using Fluency.Common;
using Fluency.Execution.Exceptions;

namespace Fluency.Execution.Functions.BuiltIn
{

    /// <summary>
    /// Wrap a C# function that takes two arguments and returns one value as a Fluency function.
    /// </summary>
    public class WrapBinaryFold<TReal> : ITopIn, ITopOut
    {
        private readonly Func<TReal, TReal, TReal> function;
        private readonly FluencyType type;
        private Value stored;

        public string Name { get; private set; }

        public GetNext TopInput { private get; set; }
        public GetNext BottomInput { private get; set; }

        public WrapBinaryFold(Func<TReal, TReal, TReal> function, FluencyType returnType, string name, Value[] arguments)
        {
            this.function = function;
            this.type = returnType;
            Name = name;

            if (arguments.Length == 1)
            {
                stored = arguments[0];
                if (returnType != FluencyType.Any && stored.Type != returnType)
                    throw new ExecutionException("Function {0} takes a {1}. You supplied a {2}: {3}", name, returnType, stored.Type, stored.ToString());

            }
            else if (arguments.Length > 1)
            {
                throw new ExecutionException("Function {0} takes either zero or one parameters. You supplied {1}: {2}", name, arguments.Length, string.Join(", ", arguments.Select(x => x.ToString())));
            }
        }

        private bool done = false;
        public Value Top()
        {
            if (done)
                return Value.Finished;

            if (stored == null)
                stored = TopInput();

            Value value;

            while (!stored.Done && (value = TopInput()))
                stored = new Value(function(stored.Get<TReal>(type), value.Get<TReal>(type)), type);

            done = true;
            return stored;
        }
    }
}