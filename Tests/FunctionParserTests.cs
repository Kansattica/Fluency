using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fluency.Interpreter;
using System;
using System.IO;
using System.Linq;
using Fluency.Interpreter.Entities;
using System.Collections.Generic;

namespace Tests
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
        public void FunctionParsingWorks(string teststr, string name, int argCount)
        {
            FunctionToken f = new FunctionToken(teststr);

            Assert.AreEqual(name, f.Name);
            Assert.AreEqual(argCount, f.Arguments.Length);
        }

        private readonly Dictionary<string, IEnumerable<(int min, int max)>> expectedGroups = new Dictionary<string, IEnumerable<(int min, int max)>>()
        {
            {"./Examples/doubledef.fl", new []{(0,1),(2,4)}},
            {"./Examples/fluency.fl", new []{(0, 30), (31,43), (44, 50), (51, 64)}}

        };

        [TestMethod]
        [DataRow("./Examples/doubledef.fl", 2)]
        [DataRow("./Examples/fluency.fl", 4)]
        public void GroupUntilWorks(string path, int groupCount)
        {
            var lines = File.ReadAllLines(Path.Join("../../../../", path));

            var groups = lines.GroupUntil(x => x.StartsWith("Def("));

            Assert.AreEqual(groupCount, groups.Count());

            foreach (var pair in groups.Select(x => x.Between).Zip(expectedGroups[path], (actual, expected) => (actual, expected)))
            {
                Assert.AreEqual(pair.expected.min, pair.actual.Min);
                Assert.AreEqual(pair.expected.max, pair.actual.Max);
            }
        }
    }
}
