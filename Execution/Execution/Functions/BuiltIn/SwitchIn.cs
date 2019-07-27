using System;
using System.Collections.Generic;
using System.Linq;
using Fluency.Common;
using Fluency.Execution.Exceptions;

namespace Fluency.Execution.Functions.BuiltIn
{
    /// <summary>
    /// If SwitchIn(true), put everything from the top input onto the top pipeline.
    /// If SwitchIn(false), put everything from the bottom input onto the top pipeline.
    /// If no argument given, treat the first value seen as if it was passed as the argument.
    /// </summary>
    public class SwitchIn : ITopIn, IBottomIn, ITopOut
    {
        public string Name => nameof(SwitchIn);

        public GetNext TopInput { private get; set; }
        public GetNext BottomInput { private get; set; }

        bool? everythingFromTop;

        public SwitchIn(Value[] arguments)
        {
            if (arguments.Length == 1)
            {
                everythingFromTop = arguments.Single().Get<bool>(FluencyType.Bool,
                        "SwitchIn takes a boolean. True means everything comes from the top, false means everything comes from the bottom.");
            }
            else if (arguments.Length > 1)
            {
                throw new ExecutionException("SwitchIn takes either zero or one arguments.");
            }

        }

        private bool EnsureDirectionSet()
        {
            if (!everythingFromTop.HasValue)
            {
                Value direction = TopInput();
                everythingFromTop = direction.Get<bool>(FluencyType.Bool, "SwitchOut needs a boolean to set which direction it's going.");
                return false;
            }
            return true;
        }

        public Value Top()
        {
            EnsureDirectionSet();

            return everythingFromTop.Value ? TopInput() : BottomInput();
        }

    }
}