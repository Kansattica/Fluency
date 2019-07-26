using System;
using Fluency.Common;

namespace Fluency.Execution.Parsing.Entities.ArgumentTypes
{
    /// <summary>
    /// Represents a boolean argument to a function.
    /// </summary>
    public class BoolArg : Argument
    {
        /// <summary>
        /// The Fluency type this object is.
        /// </summary>
        /// <value></value>
        public override FluencyType Type { get { return FluencyType.Bool; } }

        private bool _value;

        private BoolArg(bool value)
        {
            _value = value;
        }

        /// <summary>
        /// If str represents a bool, make a BoolArg.  Otherwise, return null.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static BoolArg TryParseArg(string str)
        {
            if (bool.TryParse(str, out var i))
            {
                return new BoolArg(i);
            }
            return null;
        }

        /// <summary>
        /// Return the stored bool as a string.
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