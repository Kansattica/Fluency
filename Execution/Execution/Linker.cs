
using System;
using System.Collections.Generic;
using System.Linq;
using Fluency.Execution.Exceptions;
using Fluency.Execution.Functions;

namespace Fluency.Execution
{
    /// <summary>
    /// Ensures all the function names in the program correspond to something.
    /// </summary>
    class Linker
    {
        private Dictionary<string, FunctionMaker> _namesToFunctions = new Dictionary<string, FunctionMaker>();

        private readonly bool verbose;

        public Linker(bool verbose = false)
        {
            this.verbose = verbose;
        }

        /// <summary>
        /// Tell the linker about a function.
        /// </summary>
        public void Register(string name, FunctionMaker function)
        {
            if (verbose) { Console.WriteLine("Registering {0} with the linker.", name); }
            if (_namesToFunctions.ContainsKey(name))
                throw new ExecutionException("Function {0} defined twice.", name);

            _namesToFunctions[name] = function;
        }

        /// <summary>
        /// Create a new instance of the function with the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public IFunction Create(string name, Value[] arguments)
        {
            if (_namesToFunctions.TryGetValue(name, out var func))
            {
                return func(arguments);
            }

            throw new ExecutionException("Could not find a definition for function {0}.{1}", name,
                    verbose ? "\nI know about these functions:\n" + string.Join("\n", _namesToFunctions.Keys) : "");
        }

    }

    //naming things is hard
    public delegate IFunction FunctionMaker(Value[] arguments);

}