using System;
using System.Linq;

namespace Fluency.Interpreter.Parser.Entities.ArgumentTypes
{
    /// <summary>
    /// Represents an integer argument to a function.
    /// </summary>
    public class FunctionArg : Argument
    {
        /// <summary>
        /// The Fluency type this object is.
        /// </summary>
        /// <value></value>
        public override ParsedType Type { get { return ParsedType.Function; } }

        /// <summary>
        /// The C# type this object is.
        /// </summary>
        /// <returns></returns>
        protected override Type _realtype { get { return typeof(string); } }

        private string _value;

        private FunctionArg(string value)
        {
            _value = value;
        }

        /// <summary>
        /// If str represents a function name (no quotes, letters only), make a FuncArg. Otherwise, return null.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static FunctionArg TryParseArg(string str)
        {
            if (str.All(c => char.IsLetter(c)))
            {
                return new FunctionArg(str);
            }
            return null;
        }

        /// <summary>
        /// Return the stored function name.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _value;
        }

        /// <summary>
        /// Get the value this argument represents.
        /// </summary>
        /// <returns></returns>
        protected override object Value()
        {
            return _value;
        }
    }
}