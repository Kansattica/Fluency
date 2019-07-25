using System;
using System.Linq;
using Fluency.Interpreter.Common;
using Fluency.Interpreter.Parsing.Exceptions;

namespace Fluency.Interpreter.Parsing.Entities.ArgumentTypes
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
        public override FluencyType Type { get { return FluencyType.Function; } }

        /// <summary>
        /// If this is a declaration of a parameter, what type is it
        /// </summary>
        /// <value></value>
        public FluencyType DeclaredType { get; private set; } = FluencyType.Any;

        /// <summary>
        /// The name of the function or parameter this instance represents.
        /// </summary>
        /// <value></value>
        public string FunctionName { get { return _value; } }

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
            string functionName;
            FluencyType declType = FluencyType.Any;
            string[] halves = str.Split(_spliton, 2, StringSplitOptions.RemoveEmptyEntries);

            if (halves.Length == 1)
                functionName = halves[0];
            else
            {
                functionName = halves[1];
                declType = PickType(halves[0]);
            }

            if (functionName.All(c => char.IsLetter(c)) || functionName == "...")
            {
                return new FunctionArg(functionName) { DeclaredType = declType };
            }

            return null;
        }

        private static FluencyType PickType(string typename)
        {
            switch (typename)
            {
                case "float":
                case "double":
                    return FluencyType.Double;
                case "func":
                case "fun":
                    return FluencyType.Function;
                case "int":
                    return FluencyType.Int;
                case "string":
                case "str":
                    return FluencyType.String;
                case "any":
                    return FluencyType.Any;
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
            if (DeclaredType != FluencyType.Any)
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