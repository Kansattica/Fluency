using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fluency.Interpreter;
using System;
using System.IO;
using System.Linq;

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

        [TestMethod]
        [DataRow("./Examples/doubledef.fl", 2)]
        [DataRow("./Examples/fluency.fl", 4)]
        public void GroupUntilWorks(string path, int groupCount)
        {
            var lines = File.ReadAllLines(Path.Join("../../../../", path));

            var groups = lines.GroupUntil(x => x.StartsWith("Def("));

            Assert.AreEqual(groupCount, groups.Count());
        }
    }
}
