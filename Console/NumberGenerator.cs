using Fluency.Common;
using Fluency.Execution.Functions;

namespace Fluency.CLI
{
    class NumberGenerator
    {
        private int next;
        private readonly int countTo;

        public NumberGenerator(int startAt = 0, int countTo = int.MaxValue)
        {
            next = startAt;
            this.countTo = countTo;
        }

        public Value ReadSequential()
        {
            if (next <= countTo)
                return new Value((next++).ToString(), FluencyType.String);
            else
                return Value.Finished;
        }
    }
}