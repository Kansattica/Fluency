using System.Collections.Generic;

namespace Fluency.Interpreter.Execution.Functions.BuiltIn
{
    /// <summary>
    /// Reads a value from the top input and puts it on both top and bottom outputs.
    /// </summary>
    public class Dup : ITopIn, ITopOut, IBottomOut
    {
        public virtual string Name => "Dup";

        public GetNext TopInput { private get; set; }

        private Queue<Value> _topQueue = new Queue<Value>();
        private Queue<Value> _bottomQueue = new Queue<Value>();

        protected Value DoQueueing(Queue<Value> thisQueue, Queue<Value> thatQueue)
        {
            Value toReturn;
            if (thisQueue.Count == 0)
            {
                toReturn = TopInput();
                thisQueue.Enqueue(toReturn);
                thatQueue.Enqueue(toReturn);
            }

            return thisQueue.Dequeue();
        }

        public Value Top() => DoQueueing(_topQueue, _bottomQueue);

        public Value Bottom() => DoQueueing(_bottomQueue, _topQueue);

    }

    public class Duplicate : Dup
    {
        public override string Name => "Duplicate";
    }
}