using System;
using System.Collections.Generic;
using System.Linq;
using Fluency.Common;
using Fluency.Execution.Exceptions;

namespace Fluency.Execution.Functions.BuiltIn
{

    /// <summary>
    /// Returns the first N values, then puts the rest on the bottom. If no arguments given, N is read from the top pipeline.
    /// </summary>
    public class FirstN : ITopIn, ITopOut, IBottomOut
    {
        public virtual string Name => nameof(FirstN);

        public GetNext TopInput { private get; set; }

        protected int? toAllow;
        public FirstN(Value[] args)
        {
            if (args.Length == 1)
            {
                toAllow = args.Single().Get<int>(FluencyType.Int, "FirstN takes one argument- the number of things to allow through, or no arguments- read an integer from the top pipeline and then take that many.");
            }
            else if (args.Length > 1)
            {
                throw new ExecutionException("FirstN takes either zero or one argument.");
            }
        }

        private bool EnsureAllowSet()
        {
            if (!toAllow.HasValue)
            {
                Value direction = TopInput();
                if (direction.Done)
                {
                    return false;
                }
                else
                {
                    toAllow = direction.Get<int>(FluencyType.Int, "FirstN needs to read an integer to know how many to take.");
                }
            }
            return true;
        }

        private Queue<Value> topQueue = new Queue<Value>();
        public Value Bottom()
        {
            if (!EnsureAllowSet())
            {
                return Value.Finished;
            }

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
            if (!EnsureAllowSet())
            {
                return Value.Finished;
            }

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

    /// <summary>
    /// Returns the first N values, then puts the rest on the bottom. If no arguments given, N is 1.
    /// </summary>
    public class First : FirstN
    {
        public override string Name => nameof(First);

        public First(Value[] args) : base(new Value[] { new Value(1, FluencyType.Int) })
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

    }
}