using System.Collections.Generic;
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

        }
    }
}