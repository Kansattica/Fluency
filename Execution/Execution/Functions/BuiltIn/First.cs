using System;
using System.Collections.Generic;
using System.Linq;
using Fluency.Common;
using Fluency.Execution.Exceptions;

namespace Fluency.Execution.Functions.BuiltIn
{

    /// <summary>
    /// Returns the first N values, then puts the rest on the bottom.
    /// </summary>
    public class First : ITopIn, ITopOut, IBottomOut
    {

        public virtual string Name => nameof(First);

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

        private Queue<Value> topQueue = new Queue<Value>();
        public Value Bottom()
        {
            Value value = null;

            // If we haven't sent enough off the top yet, queue that many up for the top
            while (toAllow > 0 && (value = TopInput()))
            {
                toAllow--;
                topQueue.Enqueue(value);
            }

            //didn't get a value up there or didn't have to
            if (value == null)
            {
                return TopInput();
            }

            return value; //only if the while loop set it to done
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