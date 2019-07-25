
using System.Collections.Generic;
using Fluency.Interpreter.Execution.Exceptions;
using Fluency.Interpreter.Execution.Functions;

namespace Fluency.Interpreter.Execution
{
    /// <summary>
    /// Ensures all the function names in the program correspond to something.
    /// </summary>
    class Linker
    {
        private Dictionary<string, IFunction> _namesToFunctions = new Dictionary<string, IFunction>();

        /// <summary>
        /// Tell the linker about a function.
        /// </summary>
        public void Register(IFunction function)
        {
            if (_namesToFunctions.ContainsKey(function.Name))
                throw new ExecutionException("Function {0} defined twice.", function.Name);
            
            _namesToFunctions[function.Name] = function;
        }


    }
}