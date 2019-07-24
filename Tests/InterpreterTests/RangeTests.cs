using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Threading;
using System.Numerics;
using Fluency.Interpreter.Parser.Entities;

namespace Fluency.Tests.Parser
{
    [TestClass]
    public class RangeTests
    {
        private static Random _initRand = new Random();
        ThreadLocal<Random> rand = new ThreadLocal<Random>(() =>
        {
            lock (_initRand)
            {
                return new Random(_initRand.Next());
            }
        });


        private int NextRand() => rand.Value.Next(int.MinValue, int.MaxValue);

        private Range RandomRange()
        {
            int min, max;
            do
            {
                min = NextRand();
                max = NextRand();
            } while (min == max);

            if (min > max) { int tmp = min; min = max; max = tmp; }

            return new Range(min, max);
        }

        private const int numberoftrials = 9000000; //make bigger for more thorough tests

        [TestMethod]
        public void Equality()
        {
            Parallel.For(0, numberoftrials, (i, _) =>
            {
                Range a = RandomRange();
                Range b = new Range(a.Min, a.Max);
                Assert.IsTrue(a == b);
                Assert.IsTrue(a.Equals(b));
                Assert.AreEqual(a, b);
                Assert.IsFalse(a != b);
                Assert.AreNotSame(a, b);
            });
        }

        [TestMethod]
        public void NullEquality()
        {
            Range a = RandomRange();
            Range b = null;

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
        [DoNotParallelize]
        public void Contains()
        {
            Parallel.For(0, numberoftrials, (i, _) =>
            {
                Range a = RandomRange();

                Assert.IsTrue(a.Contains(Midpoint(a.Min, a.Max)));
                Assert.IsTrue(a.Contains(a.Min));
                Assert.IsTrue(a.Contains(a.Max));
                Assert.IsFalse(a.Contains(a.Min - 1));
                Assert.IsFalse(a.Contains(a.Max + 1));

                int r = NextRand();
                Assert.AreEqual(r >= a.Min && r <= a.Max, a.Contains(r));
            });
        }

        [TestMethod]
        [DoNotParallelize]
        public void Single()
        {
            Parallel.For(0, numberoftrials, (i, _) =>
            {
                Range a = new Range(i, i);
                Assert.IsTrue(a.Contains(i));
                Assert.IsFalse(a.Contains(i + 1));
                Assert.IsFalse(a.Contains(i - 1));

                int r = NextRand();
                Assert.AreEqual(r == i, a.Contains(r));
            });
        }

    }
}