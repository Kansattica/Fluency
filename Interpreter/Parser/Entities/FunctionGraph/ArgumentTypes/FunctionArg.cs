using System;
using System.Linq;
using Fluency.Interpreter.Parser.Exceptions;

namespace Fluency.Interpreter.Parser.Entities.ArgumentTypes
{
    /// <summary>
    /// Represents a function argument to any function or a parameter a declared function takes.
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

        /// <summary>
        /// If this is a declaration of a parameter, what type is it
        /// </summary>
        /// <value></value>
        public ParsedType DeclaredType { get; private set; } = ParsedType.Any;

        private string _value;

        private FunctionArg(string value)
        {
            _value = value;
        }

        private static readonly char[] _spliton = { ' ', '\t' };
        /// <summary>
        /// If str represents a function name (no quotes, letters only), make a FuncArg. Otherwise, return null.
        /// In a declaration, function names can have a type, such as int n.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static FunctionArg TryParseArg(string str)
        {
            string functionName, paramName = null;
            ParsedType declType = ParsedType.Any;
            string[] halves = str.Split(_spliton, 2, StringSplitOptions.RemoveEmptyEntries);

            if (halves.Length == 1)
                functionName = halves[0];
            else
            {
                functionName = paramName = halves[1];
                declType = PickType(halves[0]);
            }

            if (functionName.All(c => char.IsLetter(c)))
            {
                return new FunctionArg(functionName) { DeclaredType = declType, Name = paramName };
            }
            return null;
        }

        private static ParsedType PickType(string typename)
        {
            switch (typename)
            {
                case "float":
                case "double":
                    return ParsedType.Double;
                case "func":
                case "fun":
                    return ParsedType.Function;
                case "int":
                    return ParsedType.Int;
                case "string":
                case "str":
                    return ParsedType.String;
                case "any":
                    return ParsedType.Any;
                default:
                    throw new ParseException("Could not parse type name {0}. Did you forget a comma?", typename);

            }
        }

        /// <summary>
        /// Return the stored function name.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (DeclaredType != ParsedType.Any)
                return DeclaredType.ToString() + " " + _value;
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