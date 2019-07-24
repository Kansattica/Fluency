using System;

namespace Fluency.Interpreter.Parser.Entities.ArgumentTypes
{
    /// <summary>
    /// Represents an integer argument to a function.
    /// </summary>
    public class AnyArg : Argument
    {
        /// <summary>
        /// The Fluency type this object is.
        /// </summary>
        /// <value></value>
        public override ParsedType Type { get { return ParsedType.Any; } }

        /// <summary>
        /// The C# type this object is.
        /// </summary>
        /// <returns></returns>
        protected override Type _realtype { get { return typeof(string); } }

        private AnyArg() { }

        /// <summary>
        /// If str represents a string (surrounded by quotes), make a AnyArg.  Otherwise, return null.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static AnyArg TryParseArg(string str)
        {
            if (str == "any")
            {
                return new AnyArg();
            }
            return null;
        }

        /// <summary>
        /// Return "..."
        /// </summary>
        /// <returns></returns>
        public override string ToString() => "any";

        /// <summary>
        /// Get the value this argument represents.
        /// </summary>
        /// <returns></returns>
        protected override object Value() => "any";
    }
}