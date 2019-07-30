using System;
using System.Collections.Generic;
using System.IO;
using Fluency.Common;
using Fluency.Execution.Functions;

namespace Fluency.CLI
{
    public class StreamingIO
    {
        private readonly StreamReader input;
        private readonly StreamWriter output;
        private readonly char separator;

        public StreamingIO(Stream input, Stream output, char separator = '\n', bool flushAfterWrite = false)
        {
            this.input = new StreamReader(input);
            this.output = new StreamWriter(output);
            if (!output.CanWrite) throw new ArgumentException("Output stream must be writable.");
            this.separator = separator;
            this.output.AutoFlush = flushAfterWrite;
        }

        private Queue<string> unread = new Queue<string>();


        public Value Read()
        {
            while (true)
            {
                if (unread.TryDequeue(out string chunk))
                {
                    return new Value(chunk, FluencyType.String);
                }

                if (!input.EndOfStream)
                {
                    foreach (string read in input.ReadLine().Split(separator))
                    {
                        unread.Enqueue(read);
                    }
                }
                else
                {
                    return Value.Finished;
                }
            }

        }

        public void Write(IEnumerable<Value> values)
        {
            foreach (var value in values)
            {
                output.Write(value.Type == FluencyType.String ? value.Get<string>() : value.ToString());
                output.Write(separator);
            }

        }

    }
}