using System;

namespace Fluency.Execution.Parsing.Entities
{
    /// <summary>
    /// Represents an inclusive range.
    /// </summary>
    public class Range : IEquatable<Range>
    {
        /// <summary>
        /// The lower bound.
        /// </summary>
        /// <value></value>
        public int Min { get; set; }

        /// <summary>
        /// The upper bound.
        /// </summary>
        /// <value></value>
        public int Max { get; set; }

        /// <summary>
        /// Create a new range from its inclusive bounds.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if min is greater than max.</exception>
        public Range(int min, int max)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException("min", min, $"Beginning of range must be less than or equal to the end. Attempted to create ({min},{max}).");
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Whether n is inside the range.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public bool Contains(int n) => n >= Min && n <= Max;

        /// <summary>
        /// Two ranges are equal if they represent the same range- that is, they have the same minimum and maximum.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public bool Equals(Range that) => !(that is null) && Min == that.Min && Max == that.Max;

        /// <summary>
        /// Two ranges are equal if they represent the same range- that is, they have the same minimum and maximum.
        /// </summary>
        /// <returns></returns>
        public override bool Equals(object obj) => !(obj is null) && (obj is Range) && Equals((Range)obj);

        /// <summary>
        /// Gets a hash code by xoring the min and max's hash codes.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => Min.GetHashCode() ^ Max.GetHashCode();

        /// <summary>
        /// Two ranges are equal if they represent the same range- that is, they have the same minimum and maximum.
        /// </summary>
        /// <returns></returns>
        public static bool operator ==(Range a, Range b)
        {
            if (a is null && b is null) { return true; }
            return !(a is null) && a.Equals(b);
        }

        /// <summary>
        /// Two ranges are equal if and only if they represent the same range- that is, they have the same minimum and maximum.
        /// </summary>
        /// <returns></returns>
        public static bool operator !=(Range a, Range b) => !(a == b);

        /// <summary>
        /// A Range can be implicitly constructed from a tuple of two ints. 
        /// </summary>
        /// <param name="pair"></param>
        public static implicit operator Range(ValueTuple<int, int> pair) => new Range(pair.Item1, pair.Item2);

        /// <summary>
        /// Returns a nice string representation like (Min, Max)
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"({Min}, {Max})";
    }
}