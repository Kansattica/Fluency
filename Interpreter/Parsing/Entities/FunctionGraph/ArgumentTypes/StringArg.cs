using System;

namespace Fluency.Interpreter.Parsing.Entities.ArgumentTypes
{
    /// <summary>
    /// Represents a string argument to a function.
    /// </summary>
    public class StringArg : Argument
    {
        /// <summary>
        /// The Fluency type this object is.
        /// </summary>
        /// <value></value>
        public override ParsedType Type { get { return ParsedType.String; } }

        private string _value;

        private StringArg(string value)
        {
            _value = value;
        }

        /// <summary>
        /// If str represents a string (surrounded by quotes), make a StringArg.  Otherwise, return null.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static StringArg TryParseArg(string str)
        {
            if (str[0] == '"' && str[str.Length - 1] == '"')
            {
                return new StringArg(str.Substring(1, str.Length - 2).Replace("\\\"", "\""));
            }
            return null;
        }

        /// <summary>
        /// Return the stored string.
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