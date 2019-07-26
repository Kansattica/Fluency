using System;
using System.Collections.Generic;
using System.IO;
using Fluency.Common;
using Fluency.Execution.Functions;

namespace Fluency.CLI
{
    public class FileReader
    {
        public FileReader(string file, string seperator = "\n")
        {
            foreach (var chunk in File.ReadAllText(file).Split(seperator))
            {
                buffer.Enqueue(chunk);
            }

        }

        private Queue<string> buffer = new Queue<string>();
        public Value Read()
        {
            string line;
            if (buffer.TryDequeue(out line))
            {
                return new Value(line, FluencyType.String);
            }
            else
            {
                return Value.Finished;
            }
        }

    }
}