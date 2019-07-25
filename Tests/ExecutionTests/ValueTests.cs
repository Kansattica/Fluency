using System;
using System.Collections.Generic;
using System.Linq;
using Fluency.Interpreter.Common;
using Fluency.Interpreter.Execution.Functions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fluency.Tests.Execution
{
    [TestClass]
    public class ValueTests
    {
        private static readonly Dictionary<Type, Func<object, Value>> _makeType = new Dictionary<Type, Func<object, Value>>()
        {
            {typeof(bool), (v) => new Value(v, FluencyType.Bool)},
            {typeof(int), (v) => new Value(v, FluencyType.Int)},
            {typeof(double), (v) => new Value(v, FluencyType.Double)},
            {typeof(string), (v) => new Value(v, FluencyType.String)},
        };

        private static readonly Value[] testValues = new Value[]
        { // 0         1       2            3         4             5         6
            From(1), From(2), From("Hi"), From("1"), From("2"), From(true), From(false),
            // 7         8                 9                               10        
            From(1.2), From(double.NaN), From(double.PositiveInfinity), Value.Finished,
            // 11      12
            From(1), From("Hi")
        };

        [TestMethod]
        public void BoolConvertible()
        {
            Assert.AreEqual(true, new Value(3, FluencyType.Int));
            Assert.AreEqual(true, new Value(null, FluencyType.String));
            Assert.AreEqual(true, new Value("3fasfdas", FluencyType.Any));
            Assert.AreEqual(true, new Value(double.NaN, FluencyType.Double));
            Assert.AreEqual(false, Value.Finished);
            Assert.AreEqual(Value.Finished, Value.Finished);
            Assert.AreNotEqual(Value.Finished, new Value(null, FluencyType.Any));
        }


        [TestMethod]
        [DynamicData(nameof(CartesianProduct), DynamicDataSourceType.Method)]
        public void EqualsWorks(int idxa, int idxb, bool expected)
        {
            if ((idxa, idxb) == (0, 11) || (idxa, idxb) == (2, 12)) { expected = true; }

            Value a = testValues[idxa], b = testValues[idxb];
            Assert.AreEqual(expected, a.Equals(b));
            Assert.AreEqual(expected, b.Equals(a));
        }

        private static IEnumerable<object[]> CartesianProduct() =>
         Enumerable.Range(0, 13).Zip(Enumerable.Range(0, 13), (a, b) => new object[] { a, b, a == b ? true : false }).ToArray();

        [TestMethod]
        [DataRow(0, true)]
        public void EqualsSelf(int idx, bool expected)
        {
            Value a = testValues[idx], b = testValues[idx];
            Assert.AreEqual(expected, a.Equals(b));
            Assert.AreEqual(expected, b.Equals(a));
        }


        private static Value From<T>(T val)
        {
            return _makeType[typeof(T)](val);
        }

    }
}