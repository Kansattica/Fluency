using Fluency.Common;
using Fluency.Execution.Functions;

namespace Fluency.CLI
{
    class NumberGenerator
    {
        private int next;

        public NumberGenerator(int startAt = 0)
        {
            next = startAt;
        }

        public Value ReadSequential()
        {
            return new Value((next++).ToString(), FluencyType.Int);
        }
    }
}