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

        /// <summary>
        /// Puts everything in range into the queue.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="range"></param>
        /// <typeparam name="T"></typeparam>
        public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> range)
        {
            foreach (T val in range)
            {
                queue.Enqueue(val);
            }
        }

        /// <summary>
        /// Try to get a value from queue. Returns true if it found one.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool TryDequeue<T>(this Queue<T> queue, out T value)
        {
            if (queue.Count == 0)
            {
                value = default(T);
                return false;
            }
            else
            {
                value = queue.Dequeue();
                return true;
            }

        }
    }
}