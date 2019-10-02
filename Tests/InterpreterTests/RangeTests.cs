using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Threading;
using System.Numerics;
using Fluency.Execution.Parsing.Entities;

namespace Fluency.Tests.Execution.Parsing
{
    [TestClass]
    public class SourceRangeTests
    {
        private Random rand = new Random();

        private int NextRand() => rand.Next(int.MinValue, int.MaxValue);

        private SourceRange RandomSourceRange()
        {
            int min, max;
            do
            {
                min = NextRand();
                max = NextRand();
            } while (min == max);

            if (min > max) { int tmp = min; min = max; max = tmp; }

            return new SourceRange(min, max);
        }

        private const int numberoftrials = 10000; //make bigger for more thorough tests

        [TestMethod]
        public void Equality()
        {
            for (int i = 0; i < numberoftrials; i++)
            {
                SourceRange a = RandomSourceRange();
                SourceRange b = new SourceRange(a.Min, a.Max);
                Assert.IsTrue(a == b);
                Assert.IsTrue(a.Equals(b));
                Assert.AreEqual(a, b);
                Assert.IsFalse(a != b);
                Assert.AreNotSame(a, b);
            }
        }

        [TestMethod]
        public void NullEquality()
        {
            SourceRange a = RandomSourceRange();
            SourceRange b = null;

            Assert.AreNotEqual(a, b);
            Assert.IsFalse(a == b);
            Assert.IsFalse(a == null);
            Assert.IsTrue(b == null);
            Assert.IsFalse(a.Equals(b));
            Assert.IsTrue(a != b);
            Assert.IsTrue(a != null);
            Assert.IsFalse(b != null);
        }

        public int Midpoint(long a, long b)
        {
            long midpoint = (a + b) / 2;
            return (int)midpoint;
        }

        [TestMethod]
        public void Contains()
        {
            for (int i = 0; i < numberoftrials; i++)
            {
                SourceRange a = RandomSourceRange();

                Assert.IsTrue(a.Contains(Midpoint(a.Min, a.Max)));
                Assert.IsTrue(a.Contains(a.Min));
                Assert.IsTrue(a.Contains(a.Max));
                Assert.IsFalse(a.Contains(a.Min - 1));
                Assert.IsFalse(a.Contains(a.Max + 1));

                int r = NextRand();
                Assert.AreEqual(r >= a.Min && r <= a.Max, a.Contains(r));
            };
        }

        [TestMethod]
        public void Single()
        {
            for (int i = 0; i < numberoftrials; i++)
            {
                SourceRange a = new SourceRange(i, i);
                Assert.IsTrue(a.Contains(i));
                Assert.IsFalse(a.Contains(i + 1));
                Assert.IsFalse(a.Contains(i - 1));

                int r = NextRand();
                Assert.AreEqual(r == i, a.Contains(r));
            }
        }

    }
}