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
        public FileReader(IEnumerable<string> files, string seperator = "\n")
        {
           _read = ReadFile(files, seperator).GetEnumerator();
        }

        private IEnumerable<string> ReadFile(IEnumerable<string> files, string seperator)
        {
            foreach (var chunk in files.SelectMany(f => File.ReadAllText(f).Split(seperator)))
            {
                yield return chunk;
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