namespace Fluency.Interpreter.Common
{
    /// <summary>
    /// The type a value can have.
    /// </summary>
    public enum ValueTypes
    {
        /// <summary>
        /// This argument is an integer.
        /// </summary>
        Int,

        /// <summary>
        /// This argument is a double precision floating point number.
        /// </summary>
        Double,

        /// <summary>
        /// This argument is a string.
        /// </summary>
        String,

        /// <summary>
        /// This argument is the name of a function.
        /// </summary>
        Function,

        /// <summary>
        /// Any type, or no type at all. Usually found in function definitions.
        /// </summary>
        Any,
    }

}