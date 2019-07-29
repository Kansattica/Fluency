

using System.Collections.Generic;
using System.Linq;
using Fluency.Common;
using Fluency.Execution.Exceptions;
using Fluency.Execution.Extensions;

namespace Fluency.Execution.Functions.BuiltIn
{
    /// <summary>
    /// If Unzip(true), put an element from the top pipeline onto the top pipeline, then top to bottom, then top to top, and so on.
    /// If Unzip(false), same, but put on the bottom pipeline first.
    /// If no argument given, treat the first value seen as if it was passed as the argument.
    /// </summary>
    public class Unzip: ITopIn, ITopOut, IBottomOut
    {
        public string Name => nameof(Unzip);

        public GetNext TopInput { private get; set; }

        bool? putOnTop;

        public Unzip(Value[] arguments)
        {
            if (arguments.Length == 1)
            {
                putOnTop = !arguments.Single().Get<bool>(FluencyType.Bool, //invert here because we'll invert it back before getting the first element
                        "Unzip takes a boolean. True means the first element goes on top, false, means the first element goes on bottom.");
            }
            else if (arguments.Length > 1)
            {
                throw new ExecutionException("Unzip takes either zero or one arguments.");
            }

        }

        private bool EnsureDirectionSet()
        {
            if (!putOnTop.HasValue)
            {
                Value direction = TopInput();
                if (direction.Done)
                    return false;
                putOnTop = direction.Get<bool>(FluencyType.Bool, "Unzip needs a boolean to set which direction it's going.");
            }
            return true;
        }

        private Queue<Value> _top = new Queue<Value>();
        private Queue<Value> _bottom = new Queue<Value>();

        public Value Top() => DoQueue(_top, _bottom, true);

        public Value Bottom() => DoQueue(_bottom, _top, false);

        private Value DoQueue(Queue<Value> mine, Queue<Value> yours, bool top)
        {
            if (!EnsureDirectionSet())
                return Value.Finished;

            putOnTop = !putOnTop;

            if (mine.TryDequeue(out Value val))
            {
                return val;
            }

            if (putOnTop.Value == top)
            {
                if (mine.Count > 0)
                {
                    return mine.Dequeue();
                }

                return TopInput();
            }
            else
            {
                yours.Enqueue(TopInput()); //give the other guy their value
                return TopInput();
            }
        }

    }
}