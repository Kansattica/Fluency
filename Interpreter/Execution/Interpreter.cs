using System.Collections.Generic;
using System.Linq;
using Fluency.Interpreter.Execution.Functions.BuiltIn;
using Fluency.Interpreter.Parsing;

namespace Fluency.Interpreter.Execution
{
    /// <summary>
    /// Executes function graphs.
    /// </summary>
    public class Interpreter
    {
        private readonly Parser parser;

        /// <summary>
        /// Create a new interpreter using the specified parser.
        /// </summary>
        public Interpreter(Parser parser)
        {
            this.parser = parser;
        }

        /// <summary>
        /// Execute the program represented by a bunch of lines of source code.
        /// </summary>
        /// <param name="program"></param>
        public void Execute(IEnumerable<string> program)
        {
            var functions = parser.Parse(program);

            Linker abraham = new Linker(BuiltInFactory.BuiltInFunctions.Select(kvp => (kvp.Key, kvp.Value)));
            //who kept the country as one big compilation unit


        }
    }
}