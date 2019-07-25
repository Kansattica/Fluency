using System;
using System.Collections.Generic;
using System.Linq;
using Fluency.Interpreter.Common;

namespace Fluency.Interpreter.Execution.Functions.BuiltIn
{
    class Switch : IFunction, ITopIn, ITopOut, IBottomOut
    {
        private GetNext _next;

        public string Name => "Switch";

        public GetNext TopInput { set => _next = value; }

        bool? everythingToTop;

        public Switch(Value[] arguments)
        {
            if (arguments.Length == 1)
            {
                everythingToTop = arguments.Single().Get<bool>(FluencyType.Bool,
                        "Switch takes a boolean. True means everything goes up, false means everything goes down.");
            }
            else if (arguments.Length > 1)
            {
                throw new ArgumentException("Switch takes either zero or one argument.");
            }

        }

        private bool EnsureDirectionSet(Value direction)
        {
            if (!everythingToTop.HasValue)
            {
                everythingToTop = direction.Get<bool>(FluencyType.Bool, "Switch needs a boolean to set which direction it's going.");
                return false;
            }
            return true;
        }

        public Value Top()
        {
            Value next = _next();
            if (EnsureDirectionSet(next) && everythingToTop.Value)
            {
                return next;
            }
            return Value.Finished;
        }

        public Value Bottom()
        {
            Value next = _next();
            if (EnsureDirectionSet(next) && everythingToTop.Value == false)
            {
                return next;
            }
            return Value.Finished;
        }
    }
}