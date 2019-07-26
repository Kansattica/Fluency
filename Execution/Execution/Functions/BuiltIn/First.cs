using System;
using System.Linq;
using Fluency.Common;
using Fluency.Execution.Exceptions;

namespace Fluency.Execution.Functions.BuiltIn
{

    /// <summary>
    /// Returns the first N values, then stops.
    /// </summary>
    public class First : ITopIn, ITopOut
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
                throw new ExecutionException("First takes either zero or one argument.");
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