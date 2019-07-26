using System;
using System.Runtime.Serialization;

namespace Fluency.Execution.Exceptions
{
    public class ExecutionException : Exception
    {
        public ExecutionException(string message, params object[] args) :
            base(String.Format(message, args))
        { }

        public ExecutionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ExecutionException(string message, Exception innerException, params object[] args) :
            base(String.Format(message, args), innerException)
        {
        }
    }
}