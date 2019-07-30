using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fluency.Common;
using Fluency.Execution.Functions;

namespace Fluency.CLI
{
    public class FileReader
    {
        public FileReader(IEnumerable<string> files, string separator = "\n")
        {
            _read = ReadFile(files, separator).GetEnumerator();
        }

        private IEnumerable<string> ReadFile(IEnumerable<string> files, string separator)
        {
            foreach (var stream in files.Select(f => new StreamReader(File.OpenRead(f))))
            {
                while (!stream.EndOfStream)
                {
                    foreach (string chunk in stream.ReadLine().Split(separator))
                    {
                        yield return chunk.Trim('\r', '\n', ' ', '\t');
                    }
                }
            }
        }

        private IEnumerator<string> _read;


        public Value Read()
        {
            if (_read.MoveNext())
            {
                return new Value(_read.Current, FluencyType.String);
            }

            return Value.Finished;
        }

    }
}