using System;
using System.Collections.Generic;
using System.Linq;
using Fluency.Common;
using Fluency.Execution.Exceptions;

namespace Fluency.Execution.Functions.BuiltIn
{
    /// <summary>
    /// Reads one element from the top pipeline. If true, put everything from the bottom pipeline onto the top. Otherwise, return nothing.
    /// </summary>
    public class MergeIf : ITopIn, IBottomIn, ITopOut
    {
        public string Name => nameof(MergeIf);

        public GetNext TopInput { private get; set; }
        public GetNext BottomInput { private get; set; }

        bool? takeFromBottom;

        private void EnsureDirectionSet()
        {
            if (!takeFromBottom.HasValue)
            {
                Value direction = TopInput();
                takeFromBottom = direction.Get<bool>(FluencyType.Bool, "MergeIf needs a boolean to set whether it's taking.");
            }
        }

        public Value Top()
        {
            EnsureDirectionSet();

            return takeFromBottom.Value ?  BottomInput() : Value.Finished;
        }

    }
}