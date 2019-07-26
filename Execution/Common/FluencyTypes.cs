namespace Fluency.Common
{
    /// <summary>
    /// The type a value can have.
    /// </summary>
    public enum FluencyType
    {
        /// <summary>
        /// Represents a boolean (true or false)
        /// </summary>
        Bool,

        /// <summary>
        /// Represents an integer.
        /// </summary>
        Int,

        /// <summary>
        /// Represents a double precision floating point number.
        /// </summary>
        Double,

        /// <summary>
        /// Represents a string.
        /// </summary>
        String,

        /// <summary>
        /// Represents the name of a function.
        /// </summary>
        Function,

        /// <summary>
        /// Any type, or no type at all. Usually found in function definitions.
        /// </summary>
        Any,
    }

}