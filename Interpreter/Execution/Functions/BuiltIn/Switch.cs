using System;
using System.Collections.Generic;
using System.Linq;
using Fluency.Interpreter.Common;

namespace Fluency.Interpreter.Execution.Functions.BuiltIn
{
    /// <summary>
    /// If switch(true), put everything from the top input onto the top pipeline.
    /// If switch(false), put everything from the top input onto the bottom pipeline.
    /// If no argument given, treat the first value seen as if it was passed as the argument.
    /// </summary>
    class Switch : ITopIn, ITopOut, IBottomOut
    {
        public string Name => "Switch";

        public GetNext TopInput { private get; set; }

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
            Value next;
            if ((next = TopInput()) && EnsureDirectionSet(next) && everythingToTop.Value)
            {
                return next;
            }
            return Value.Finished;
        }

        public Value Bottom()
        {
            Value next;
            if ((next = TopInput()) && EnsureDirectionSet(next) && everythingToTop.Value == false)
            {
                return next;
            }
            return Value.Finished;
        }
    }
}