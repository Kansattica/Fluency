using System;
using Fluency.Common;

namespace Fluency.Execution.Parsing.Entities.ArgumentTypes
{
    /// <summary>
    /// Represents a double argument to a function.
    /// </summary>
    public class DoubleArg : Argument
    {
        /// <summary>
        /// The Fluency type this object is.
        /// </summary>
        /// <value></value>
        public override FluencyType Type { get { return FluencyType.Double; } }

        private double _value;

        private DoubleArg(double value)
        {
            _value = value;
        }

        /// <summary>
        /// If str represents a double, make a DoubleArg.  Otherwise, return null.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DoubleArg TryParseArg(string str)
        {
            if (double.TryParse(str, out var i))
            {
                return new DoubleArg(i);
            }
            return null;
        }

        /// <summary>
        /// Return the stored double as a string.
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