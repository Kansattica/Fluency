using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fluency.Interpreter.Parser;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Fluency.Interpreter.Parser.Entities;

namespace Fluency.Tests
{
    [TestClass]
    public class FunctionParserTests
    {
        [TestMethod]
        [DataRow(@"Cool()", "Cool", 0)]
        [DataRow(@"CoolBeans()", "CoolBeans", 0)]
        [DataRow(@"CoolBeans(1)", "CoolBeans", 1)]
        [DataRow(@"Switch(true)", "Switch", 1)]
        [DataRow(@"CoolBeans(1, 2, 3)", "CoolBeans", 3)]
        [DataRow(@"CoolBeans(1, 2, FunctionName, 3)", "CoolBeans", 4)]
        [DataRow(@"Fun(1,2,3,""four"")", "Fun", 4)]
        [DataRow(@"Fun(""a string"")", "Fun", 1)]
        [DataRow(@"Fun(""a string"", 3, ""another string"", true, false)", "Fun", 5)]
        public void FunctionParsingWorks(string test, string name, int argCount)
        {
            foreach (string teststr in new[] { test, @"\." + test, test + "./", @"\." + test + "./" })
            {
                FunctionToken f = new FunctionToken(teststr, 0, teststr.Length);

                Assert.AreEqual(name, f.Name);
                Assert.AreEqual(argCount, f.Arguments.Length);
            }
        }

        private readonly Dictionary<string, IEnumerable<(int min, int max)>> expectedGroups = new Dictionary<string, IEnumerable<(int min, int max)>>()
        {
            {"./Examples/doubledef.fl", new []{(0,1),(2,4)}},
            {"./Examples/fluency.fl", new []{(0, 1), (2, 32), (33,45), (46, 52), (53, 66)}}

        };

        [TestMethod]
        [DataRow("./Examples/doubledef.fl", 2)]
        [DataRow("./Examples/fluency.fl", 5)]
        public void GroupUntilWorks(string path, int groupCount)
        {
            var lines = File.ReadAllLines(Path.Join("../../../../", path));

            var groups = lines.GroupUntil(x => x.StartsWith("Def("));

            Assert.AreEqual(groupCount, groups.Count());

            foreach (var pair in groups.Select(x => x.Indexes).Zip(expectedGroups[path], (actual, expected) => (actual, expected)))
            {
                Assert.AreEqual(pair.expected.min, pair.actual.Min);
                Assert.AreEqual(pair.expected.max, pair.actual.Max);
            }
        }

        [TestMethod]
        public void GroupUntilInclusive()
        {
            var grouped = Enumerable.Range(0, 100).GroupUntil(x => x == 50, inclusive: true);

            Assert.AreEqual(2, grouped.Count());
            Assert.AreEqual(0, grouped.First().First());
            Assert.AreEqual(50, grouped.First().Last());
            Assert.AreEqual(51, grouped.Last().First());
            Assert.AreEqual(99, grouped.Last().Last());
            Assert.IsTrue(Enumerable.Range(0, 100).SequenceEqual(grouped.SelectMany(x => x)));
        }

        [TestMethod]
        public void GroupUntilInclusiveNoEmptyGroups()
        {
            var grouped = Enumerable.Range(0, 100).GroupUntil(x => x == 99, inclusive: true);

            Assert.AreEqual(1, grouped.Count());
            Assert.AreEqual(0, grouped.First().First());
            Assert.AreEqual(99, grouped.First().Last());
            Assert.IsTrue(Enumerable.Range(0, 100).SequenceEqual(grouped.SelectMany(x => x)));
        }
    }
}
