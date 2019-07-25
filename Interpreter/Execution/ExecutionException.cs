using System;

namespace Fluency.Interpreter.Execution.Exceptions
{
    class ExecutionException : Exception
    {

        public ExecutionException(string message, params object[] args) :
            base(String.Format(message, args))
        { }
    }
}