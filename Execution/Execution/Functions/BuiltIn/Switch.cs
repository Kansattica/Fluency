using System;
using System.Collections.Generic;
using System.Linq;
using Fluency.Common;
using Fluency.Execution.Exceptions;

namespace Fluency.Execution.Functions.BuiltIn
{
    /// <summary>
    /// If switch(true), put everything from the top input onto the top pipeline.
    /// If switch(false), put everything from the top input onto the bottom pipeline.
    /// If no argument given, treat the first value seen as if it was passed as the argument.
    /// </summary>
    public class Switch : ITopIn, ITopOut, IBottomOut
    {
        public string Name => nameof(Switch);

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
                throw new ExecutionException("Switch takes either zero or one arguments.");
            }

        }

        private bool EnsureDirectionSet()
        {
            if (!everythingToTop.HasValue)
            {
                Value direction = TopInput();
                everythingToTop = direction.Get<bool>(FluencyType.Bool, "Switch needs a boolean to set which direction it's going.");
                return false;
            }
            return true;
        }

        public Value Top() => SendTo(isTop: true);
        public Value Bottom() => SendTo(isTop: false);

        private Value SendTo(bool isTop)
        {
            EnsureDirectionSet();

            // If we know that we shouldn't be sending stuff this way, return finished
            // before consuming an input.
            if (everythingToTop.Value != isTop) { return Value.Finished; }

            return TopInput();
        }

    }
}