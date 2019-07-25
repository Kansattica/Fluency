using System;

namespace Fluency.Interpreter.Execution.Exceptions
{
    public class ExecutionException : Exception
    {
        public ExecutionException(string message, params object[] args) :
            base(String.Format(message, args))
        { }
    }
}