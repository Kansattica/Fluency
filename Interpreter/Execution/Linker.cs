
using System;
using System.Collections.Generic;
using System.Linq;
using Fluency.Interpreter.Execution.Exceptions;
using Fluency.Interpreter.Execution.Functions;

namespace Fluency.Interpreter.Execution
{
    /// <summary>
    /// Ensures all the function names in the program correspond to something.
    /// </summary>
    class Linker
    {
        private Dictionary<string, FunctionMaker> _namesToFunctions;

        ///Construct a new linker already knowing about some functions.
        public Linker(IEnumerable<(string FunctionName, FunctionMaker Constructor)> startingFunctions = null)
        {
            if (startingFunctions == null)
            {
                _namesToFunctions = new Dictionary<string, FunctionMaker>();
            }
            else
            {
                _namesToFunctions = startingFunctions.ToDictionary(pair => pair.FunctionName, pair => pair.Constructor);
            }

        }

        /// <summary>
        /// Tell the linker about a function.
        /// </summary>
        public void Register(string name, FunctionMaker function)
        {
            if (_namesToFunctions.ContainsKey(name))
                throw new ExecutionException("Function {0} defined twice.", name);

            _namesToFunctions[name] = function;
        }
    }

    //naming things is hard
    public delegate IFunction FunctionMaker(Value[] arguments);

}