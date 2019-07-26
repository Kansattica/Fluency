using System.Collections.Generic;
using System.Linq;
using Fluency.Execution.Exceptions;
using Fluency.Execution.Extensions;
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
        /// <param name="input">The values to pass to the program.</param>
        public IEnumerable<Value> Execute(IEnumerable<string> program, GetNext input)
        {
            var functions = parser.Parse(program);

            Linker abraham = new Linker(verbose: false);
            //who kept the country as one big compilation unit

            foreach (var kvp in BuiltInFactory.BuiltInFunctions)
            {
                abraham.Register(kvp.Key, kvp.Value);
            }

            foreach (var functionGraph in functions)
            {
                abraham.Register(functionGraph.Name, (args) => new UserDefinedFunction(functionGraph, args));
            }

            IFunction main;
            try
            {
                main = abraham.Create("Main", new Value[0]);
            }
            catch (ExecutionException ex)
            {
                throw new ExecutionException("You must define exactly one function called Main.", ex);
            }

            return RunMain(main, input);
        }

        private IEnumerable<Value> RunMain(IFunction main, GetNext input)
        {
            ITopIn mainIn = main.Is<ITopIn>();
            ITopOut mainOut = main.Is<ITopOut>();

            mainIn.TopInput = input;
            Value next;
            while (next = mainOut.Top())
            {
                yield return next;
            }

            yield return Value.Finished;

        }

    }
}