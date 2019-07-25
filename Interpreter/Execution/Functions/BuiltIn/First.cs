using System;
using System.Linq;
using Fluency.Interpreter.Common;

namespace Fluency.Interpreter.Execution.Functions.BuiltIn
{

    /// <summary>
    /// Returns the first N values, then stops.
    /// </summary>
    class First : ITopIn, ITopOut
    {

        public virtual string Name => "First";

        public GetNext TopInput { private get; set; }

        private int toAllow = 1;
        public First(Value[] args)
        {
            if (args.Length == 1)
            {
                toAllow = args.Single().Get<int>(FluencyType.Int, "First takes one argument- the number of things to allow through, or allow one thing if no arguments.");
            }
            else if (args.Length > 1)
            {
                throw new ArgumentException("First takes either zero or one argument.");
            }
        }

        public Value Top()
        {
            if (toAllow > 0)
            {
                toAllow--;
                return TopInput();
            }
            else
            {
                return Value.Finished;
            }
        }
    }

}