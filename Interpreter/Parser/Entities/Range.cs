using System;

namespace Fluency.Interpreter.Parser.Entities
{
    public class Range : IEquatable<Range>
    {
        public int Min { get; set; }
        public int Max { get; set; }

        public Range(int min, int max)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException("min", min, $"Beginning of range must be less than or equal to the end. Attempted to create ({min},{max}).");
            Min = min;
            Max = max;
        }

        public bool Contains(int n) => n >= Min && n <= Max;

        public bool Equals(Range that) => !(that is null) && Min == that.Min && Max == that.Max;

        public override bool Equals(object obj) => !(obj is null) && (obj is Range) && Equals((Range)obj);
        public override int GetHashCode() => Min.GetHashCode() ^ Max.GetHashCode();

        public static bool operator ==(Range a, Range b)
        {
            if (a is null && b is null) { return true; }
            return !(a is null) && a.Equals(b);
        }
        public static bool operator !=(Range a, Range b) => !(a == b);

        public static implicit operator Range(ValueTuple<int, int> pair) => new Range(pair.Item1, pair.Item2);

        public override string ToString() => $"({Min},{Max})";
    }
}