using System;
using System.Linq;
using Fluency.Interpreter.Common;
using Fluency.Interpreter.Parsing.Entities.ArgumentTypes;

namespace Fluency.Interpreter.Parsing.Entities
{
    /// <summary>
    /// Represents a parsed function argument.
    /// </summary>
    public abstract class Argument
    {

        /// <summary>
        /// The Fluency type this object is.
        /// </summary>
        /// <value></value>
        public abstract ValueTypes Type { get; }

        /// <summary>
        /// Make an empty argument.
        /// </summary>
        protected Argument() { }

        /// <summary>
        /// Create a new parsed argument from a string representing one.
        /// Valid arguments are "strings", integers, floating point numbers, and function names. 
        /// The special argument ... (three periods) is also valid in a Def only.
        /// </summary>
        /// <param name="arg">The string to be parsed.</param>
        /// <param name="argument">The parsed result, if successful.</param>
        public static bool TryParse(string arg, out Argument argument)
        {
            foreach (var parser in parsers)
            {
                argument = parser(arg);
                if (argument != null)
                    return true;
            }
            argument = null;
            return false;
        }

        private delegate Argument TryParser(string arg);
        private static readonly TryParser[] parsers =
            new TryParser[] { IntArg.TryParseArg, DoubleArg.TryParseArg, StringArg.TryParseArg, FunctionArg.TryParseArg };

        /// <summary>
        /// Attempt to return this function's value as a type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetAs<T>() => (T)Value();

        /// <summary>
        /// Get the value this argument represents.
        /// </summary>
        /// <returns></returns>
        protected abstract object Value();
    }

}