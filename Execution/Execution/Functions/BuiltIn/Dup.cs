using System.Collections.Generic;

namespace Fluency.Execution.Functions.BuiltIn
{
    /// <summary>
    /// Reads a value from the top input and puts it on both top and bottom outputs.
    /// </summary>
    public class Dup : ITopIn, ITopOut, IBottomOut
    {
        public virtual string Name => nameof(Dup);

        public GetNext TopInput { private get; set; }

        private Queue<Value> _topQueue = new Queue<Value>();
        private Queue<Value> _bottomQueue = new Queue<Value>();

        protected Value DoQueueing(Queue<Value> thisQueue, Queue<Value> thatQueue)
        {
            Value toReturn = Value.Finished;

            if (thisQueue.Count == 0)
            {
                toReturn = TopInput();
                thatQueue.Enqueue(toReturn);
            }
            else
            {
                toReturn = thisQueue.Dequeue();
            }

            return toReturn;
        }

        public Value Top() => DoQueueing(_topQueue, _bottomQueue);

        public Value Bottom() => DoQueueing(_bottomQueue, _topQueue);

    }

    /// <summary>
    /// Reads a value from the top input and puts it on both top and bottom outputs. Same as <see cref="Dup"/>
    /// </summary>
    public class Duplicate : Dup
    {
        public override string Name => nameof(Duplicate);
    }
}