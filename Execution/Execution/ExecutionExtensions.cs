using System;
using System.Collections.Generic;
using Fluency.Execution.Exceptions;
using Fluency.Execution.Functions;

namespace Fluency.Execution.Extensions
{
    /// <summary>
    /// Extension methods to help execution.
    /// </summary>
    public static class ExecutionExtensions
    {
        /// <summary>
        /// Asserts that this IFunction also implements interface T, usually one of the I[Top|Bottom][InOut] interfaces.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="extraInfo">Will be added to the exception, if thrown.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ExecutionException">Thrown if function is not a T</exception>
        public static T Is<T>(this IFunction function, string extraInfo = null) where T : IFunction
        {
            try
            {
                return (T)function;
            }
            catch (InvalidCastException ex)
            {
                throw new ExecutionException("Tried to use function {0} as a {1}, which it isn't.{2}", ex, function.Name, typeof(T).Name,
                        extraInfo != null ? "\n" + extraInfo : "");
            }

        }
    }
}