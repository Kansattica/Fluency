using System;
using System.Linq;

namespace Fluency.Interpreter.Parser.Entities.ArgumentTypes
{
    /// <summary>
    /// Represents an integer argument to a function.
    /// </summary>
    public class IntArg : Argument
    {
        /// <summary>
        /// The Fluency type this object is.
        /// </summary>
        /// <value></value>
        public override ParsedType Type { get { return ParsedType.Int; } }

        private int _value;

        private IntArg(int value)
        {
            _value = value;
        }

        /// <summary>
        /// If str represents an integer, make an IntArg.  Otherwise, return null.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static IntArg TryParseArg(string str)
        {
            if (int.TryParse(str, out var i))
            {
                return new IntArg(i);
            }
            return null;
        }

        /// <summary>
        /// Return the stored integer as a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _value.ToString();
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