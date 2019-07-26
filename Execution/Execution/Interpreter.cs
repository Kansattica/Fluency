using System.Collections.Generic;
using System.Linq;
using Fluency.Execution.Functions;
using Fluency.Execution.Functions.BuiltIn;
using Fluency.Execution.Parsing;

namespace Fluency.Execution
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

            Linker abraham = new Linker(verbose: true);
            //who kept the country as one big compilation unit

            foreach (var kvp in BuiltInFactory.BuiltInFunctions)
            {
                abraham.Register(kvp.Key, kvp.Value);
            }

            foreach (var functionGraph in functions)
            {
                abraham.Register(functionGraph.Name, (args) => new UserDefinedFunction(functionGraph, args));
            }


        }
    }
}