using System.Linq;
using Fluency.Common;
using Fluency.Execution.Exceptions;

namespace Fluency.Execution.Functions.BuiltIn
{
    /// <summary>
    /// If Zip(true), put an element from the top pipeline onto the top pipeline, then bottom to top, then top to top, and so on.
    /// If Zip(false), same, but take from the bottom pipeline first.
    /// If no argument given, treat the first value seen as if it was passed as the argument.
    /// </summary>
    public class Zip: ITopIn, IBottomIn, ITopOut
    {
        public string Name => nameof(Zip);

        public GetNext TopInput { private get; set; }
        public GetNext BottomInput { private get; set; }

        bool? takeFromTop;

        public Zip(Value[] arguments)
        {
            if (arguments.Length == 1)
            {
                takeFromTop = !arguments.Single().Get<bool>(FluencyType.Bool, //invert here because we'll invert it back before getting the first element
                        "Zip takes a boolean. True means the first element comes from the top, false, false means the first element comes from the.");
            }
            else if (arguments.Length > 1)
            {
                throw new ExecutionException("Zip takes either zero or one arguments.");
            }

        }

        private bool EnsureDirectionSet()
        {
            if (!takeFromTop.HasValue)
            {
                Value direction = TopInput();
                takeFromTop = direction.Get<bool>(FluencyType.Bool, "Zip needs a boolean to set which direction it's going.");
                return false;
            }
            return true;
        }

        public Value Top()
        {
            EnsureDirectionSet();

            takeFromTop = !takeFromTop;
            return takeFromTop.Value ? TopInput() : BottomInput();
        }


    }
}