using System;
using System.Collections.Generic;
using Fluency.Common;
using Fluency.Execution.Functions;

namespace Fluency.CLI
{
    public class ConsoleIO
    {
        private readonly string seperator;

        public ConsoleIO(string seperator = "\n")
        {
            this.seperator = seperator;
        }

        private Queue<string> buffer = new Queue<string>();
        public Value Read()
        {
            string line;
            if (buffer.Count == 0)
            {
                if ((line = Console.ReadLine()) != null)
                {
                    foreach (string chunk in line.Split(seperator))
                    {
                        buffer.Enqueue(chunk);
                    }
                }
            }

            if (buffer.TryDequeue(out line))
            {
                return new Value(line, FluencyType.String);
            }
            else
            {
                return Value.Finished;
            }
        }

        public void Write(IEnumerable<Value> values)
        {
            foreach (var value in values)
            {
                Console.Write(value.Type == FluencyType.String ? value.Get<string>() : value.ToString());
                Console.Write(seperator);
            }

        }
    }
}