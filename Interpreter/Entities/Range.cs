using System;

namespace Fluency.Interpreter.Entities
{
    public class Range
    {
        public int Min { get; set; }
        public int Max { get; set; }

        public Range(int min, int max)
        {
            if (min >= max)
                throw new ArgumentOutOfRangeException("min", min, $"Beginning of range must be less than the end. Attempted to create ({min},{max}).");
            Min = min;
            Max = max;
        }

        public bool Contains(int n)
        {
            return n >= Min && n <= Max;
        }
    }
}