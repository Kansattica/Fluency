using System.Collections.Generic;
using System.Linq;
using Fluency.Common;
using Fluency.Execution.Exceptions;
using Fluency.Execution.Extensions;
using Fluency.Execution.Functions;
using Fluency.Execution.Functions.BuiltIn;
using Fluency.Execution.Functions.BuiltIn.Factory;
using Fluency.Execution.Parsing;

namespace Fluency.Execution
{
    /// <summary>
    /// Executes function graphs.
    /// </summary>
    public class Interpreter
    {
        private readonly Parser parser;
        private readonly bool printReady;

        /// <summary>
        /// Create a new interpreter using the specified parser.
        /// </summary>
        public Interpreter(Parser parser, bool printReady = false)
        {
            this.parser = parser;
            this.printReady = printReady;
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
                abraham.Register(functionGraph.Name, (topArgs, bottomArgs) => new UserFunctionStub(functionGraph, topArgs, bottomArgs, abraham));
            }

            abraham.Register("Expand", (topArgs, bottomArgs) => new Expand(topArgs, bottomArgs, abraham));

            IFunction main;
            try
            {
                main = abraham.Resolve("Main", new Value[0], new Value[0]);
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

            if (printReady)
                yield return new Value("Ready!\n", FluencyType.String);

            while (next = mainOut.Top())
            {
                yield return next;
            }

            yield return Value.Finished;

        }

    }
}