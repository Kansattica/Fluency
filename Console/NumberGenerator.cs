using Fluency.Common;
using Fluency.Execution.Functions;

namespace Fluency.CLI
{
    class NumberGenerator
    {
        private int next;
        private readonly int? countTo;

        public NumberGenerator(int startAt = 0, int? countTo = null)
        {
            next = startAt;
            this.countTo = countTo;
        }

        public Value ReadSequential()
        {
            if (next < countTo.GetValueOrDefault(int.MaxValue))
                return new Value((next++).ToString(), FluencyType.Int);
            else
                return Value.Finished;
        }
    }
}