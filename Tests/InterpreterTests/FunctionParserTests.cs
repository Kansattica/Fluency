using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fluency.Execution.Parsing;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Fluency.Execution.Parsing.Entities;
using System.Text;
using Fluency.Execution.Parsing.Entities.ArgumentTypes;
using Fluency.Common;

namespace Fluency.Tests.Execution.Parsing
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
                Assert.AreEqual(argCount, f.TopArguments.Length);
            }
        }

        [TestMethod]
        public void ParseTopAndBottomArguments()
        {
            string teststr = "Def(Main, int n | func y, bool q)";
            FunctionToken f = new FunctionToken(teststr, 0, teststr.Length);

            Assert.AreEqual("Def", f.Name);

            var topargs = f.TopArguments.Cast<FunctionArg>().ToArray();
            var bottomargs = f.BottomArguments.Cast<FunctionArg>().ToArray();
            Assert.AreEqual(2, f.TopArguments.Length);
            Assert.AreEqual(2, f.BottomArguments.Length);
            Assert.AreEqual(FluencyType.Any, topargs[0].DeclaredType);
            Assert.AreEqual("Main", topargs[0].FunctionName);
            Assert.AreEqual(FluencyType.Int, topargs[1].DeclaredType);
            Assert.AreEqual("n", topargs[1].FunctionName);
            Assert.AreEqual(FluencyType.Function, bottomargs[0].DeclaredType);
            Assert.AreEqual("y", bottomargs[0].FunctionName);
            Assert.AreEqual(FluencyType.Bool, bottomargs[1].DeclaredType);
            Assert.AreEqual("q", bottomargs[1].FunctionName);
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
            EqualEnumerables(Enumerable.Range(0, 100), grouped.SelectMany(x => x));
        }

        [TestMethod]
        public void GroupUntilInclusiveNoEmptyGroups()
        {
            var grouped = Enumerable.Range(0, 100).GroupUntil(x => x == 99, inclusive: true);

            Assert.AreEqual(1, grouped.Count());
            Assert.AreEqual(0, grouped.First().First());
            Assert.AreEqual(99, grouped.First().Last());
            EqualEnumerables(Enumerable.Range(0, 100), grouped.SelectMany(x => x));
        }

        [TestMethod]
        public void MergeIf()
        {
            var arr = new[] { 1, 2, 3, 4, 5 };

            var result = arr.MergeLastIf(x => x == 5, (prev, curr) => prev + curr);

            EqualEnumerables(new[] { 1, 2, 3, 9 }, result);
        }

        [TestMethod]
        public void MergeIfPredicateFalse()
        {
            var arr = new[] { 1, 2, 3, 4, 5 };

            var result = arr.MergeLastIf(x => x == 6, (prev, curr) => prev + curr);

            EqualEnumerables(new[] { 1, 2, 3, 4, 5 }, result);
        }

        [TestMethod]
        public void MergeIfMultiple()
        {
            var arr = new[] { 1, 2, 1, 3, 4, 1, 5 };

            var result = arr.MergeLastIf(x => x == 1, (prev, curr) => prev + curr);

            EqualEnumerables(new[] { 1, 3, 3, 5, 5 }, result);
        }

        [TestMethod]
        public void MergeIfEmpty()
        {
            var arr = new int[] { };

            var result = arr.MergeLastIf(x => x == 1, (prev, curr) => prev + curr);

            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void MergeIfOne()
        {
            var arr = new[] { 100 };

            var result = arr.MergeLastIf(x => x == 1, (prev, curr) => prev + curr);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(100, result.Single());
        }

        [TestMethod]
        public void MergeIfTwo()
        {
            var arr = new[] { 100, 5 };

            var result = arr.MergeLastIf(x => x == 5, (prev, curr) => prev - curr);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(95, result.Single());
        }

        [TestMethod]
        public void MergeIfTwoPredicateFalse()
        {
            var arr = new[] { 100, 5 };

            var result = arr.MergeLastIf(x => x == 6, (prev, curr) => prev - curr);

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(100, result.First());
            Assert.AreEqual(5, result.Last());
        }

        [TestMethod]
        public void SkipBetween()
        {
            var arr = "Hey there (buddy), what's up";

            var result = arr.SkipBetween(x => x == '(', x => x == ')').Stringify();

            Assert.AreEqual("Hey there (), what's up", result);
        }

        [TestMethod]
        public void GroupWhile()
        {
            var arr = @"    \.FunctionCall().AnotherFunctionCall(with, ""argum)(ents"")./    \.ThirdCall()./ ";

            int doublequotes = 0;
            var result = arr.GroupWhile((x, infunc) => FunctionParse(x, infunc, ref doublequotes));

            EqualEnumerables(new SourceRange[] { (4, 19), (20, 60), (61, 62), (67, 79), (80, 81) },
                result.Select(x => x.Indexes));
            EqualEnumerables(new[] { @"\.FunctionCall()", ".AnotherFunctionCall(with, \"argum)(ents\")", "./", @"\.ThirdCall()", "./" },
             result.Select(x => x.Stringify()));
            Assert.AreEqual(arr.TrimEnd().Length - 1, result.Last().Indexes.Max); //.Length counts from one, so
        }

        [TestMethod]
        public void GroupWhileStart()
        {
            var arr = "Def(IsPrime, n).Dup().First().Sqrt().Floor().Range().MergeBottom().DivBy().Drain().MergeBottom()";

            int doublequotes = 0;
            var result = arr.GroupWhile((x, infunc) => FunctionParse(x, infunc, ref doublequotes));

            EqualEnumerables(new SourceRange[] { (0, 14), (15, 20), (21, 28), (29, 35), (36, 43), (44, 51), (52, 65), (66, 73), (74, 81), (82, 95) },
                result.Select(x => x.Indexes));
            EqualEnumerables(new[] { "Def(IsPrime, n)", ".Dup()", ".First()", ".Sqrt()", ".Floor()", ".Range()", ".MergeBottom()", ".DivBy()", ".Drain()", ".MergeBottom()" },
             result.Select(x => x.Stringify()));
            Assert.AreEqual(arr.TrimEnd().Length - 1, result.Last().Indexes.Max);
        }

        [TestMethod]
        public void GroupWhileStartSpaces()
        {
            var arr = "     Def(IsPrime, n).Dup().First().Sqrt().Floor().Range().MergeBottom().DivBy().Drain().MergeBottom()";

            int doublequotes = 0;
            var result = arr.GroupWhile((x, infunc) => FunctionParse(x, infunc, ref doublequotes));

            EqualEnumerables(new SourceRange[] { (5, 19), (20, 25), (26, 33), (34, 40), (41, 48), (49, 56), (57, 70), (71, 78), (79, 86), (87, 100) },
                result.Select(x => x.Indexes));
            EqualEnumerables(new[] { "Def(IsPrime, n)", ".Dup()", ".First()", ".Sqrt()", ".Floor()", ".Range()", ".MergeBottom()", ".DivBy()", ".Drain()", ".MergeBottom()" },
             result.Select(x => x.Stringify()));
            Assert.AreEqual(arr.TrimEnd().Length - 1, result.Last().Indexes.Max);
        }

        [TestMethod]
        public void GroupWhileNoBranchUp()
        {
            var arr = @"												\.Switch(false)		\.MergeTop().DupN()./						\.Com(""remainders"")"
            .Replace("\t", "    ");
            int doublequotes = 0;
            var result = arr.GroupWhile((x, infunc) => FunctionParse(x, infunc, ref doublequotes));

            var expectedRanges = new SourceRange[] { (48, 62), (71, 82), (83, 89), (90, 91), (116, 134) };
            var expectedStrings = new[] { @"\.Switch(false)", @"\.MergeTop()", ".DupN()", "./", @"\.Com(""remainders"")" };
            EqualEnumerables(expectedRanges, result.Select(x => x.Indexes));
            EqualEnumerables(expectedStrings, result.Select(x => x.Stringify()));
            foreach (var pair in expectedRanges.Zip(expectedStrings, (ran, str) => (ran, str)))
            {
                Assert.AreEqual(pair.str, IndexedSubstring(arr, pair.ran));
            }
            Assert.AreEqual(arr.TrimEnd().Length - 1, result.Last().Indexes.Max);
        }

        [TestMethod]
        public void GroupWhileSmall()
        {
            var arr = @"\.Func()";

            int doublequotes = 0;
            var result = arr.GroupWhile((x, infunc) => FunctionParse(x, infunc, ref doublequotes));

            EqualEnumerables(new SourceRange[] { (0, 7) },
                result.Select(x => x.Indexes));
            EqualEnumerables(new[] { @"\.Func()" },
             result.Select(x => x.Stringify()));
            Assert.AreEqual(arr.TrimEnd().Length - 1, result.Last().Indexes.Max);
        }

        [TestMethod]
        public void GroupWhileLeftoverIn()
        {
            var arr = new[] { 1, 2, 3, 4, 5 };

            var result = arr.GroupWhile((a, b) => GroupWhileAction.In);

            EqualEnumerables(arr, result.Single());
        }

        [TestMethod]
        public void GroupWhileLeftoverOut()
        {
            var arr = new[] { 1, 2, 3, 4, 5 };

            var result = arr.GroupWhile((a, b) => GroupWhileAction.StillOut);

            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GroupWhileExclude()
        {
            var arr = new[] { 1, 2, 3, 4, 5 };

            var result = arr.GroupWhile((a, state) => a <= 3 ? GroupWhileAction.In : (state ? GroupWhileAction.LeaveExclude : GroupWhileAction.StillOut));

            EqualEnumerables(new[] { 1, 2, 3 }, result.Single());
        }

        private static string IndexedSubstring(string str, SourceRange range)
        {
            return str.Substring(range.Min, 1 + range.Max - range.Min);
        }

        private static GroupWhileAction FunctionParse(char c, bool infunc, ref int doublequotes)
        {
            doublequotes += (c == '"' ? 1 : 0);
            if (infunc)
            {
                if ((c == ')' || c == '/') && (doublequotes % 2 == 0))
                    return GroupWhileAction.LeaveInclude;
                return GroupWhileAction.In;
            }
            else
            {
                //Special case for Def, the only function call that doesn't start with . or \.
                if (c == 'D') { return GroupWhileAction.In; }
                if (c == '\\' || c == '.') { return GroupWhileAction.In; }
                return GroupWhileAction.StillOut;
            }
        }


        private void EqualEnumerables<T>(IEnumerable<IEnumerable<T>> expected, IEnumerable<IEnumerable<T>> actual)
        {
            Assert.AreEqual(expected.Count(), actual.Count());
            foreach (var pair in expected.Zip(actual, (ex, act) => (ex, act)))
            {
                EqualEnumerables(pair.ex, pair.act);
            }
        }
        private void EqualEnumerables<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            if (!expected.SequenceEqual(actual))
            {
                Assert.Fail("(Expected subarray: {0} Got: {1})", Stringify(expected), Stringify(actual));
            }
        }

        private string Stringify<T>(IEnumerable<IEnumerable<T>> toPrint)
        {
            StringBuilder sb = new StringBuilder("{");
            bool first = true;
            foreach (var subenum in toPrint)
            {
                if (!first)
                    sb.Append(", ");
                else
                    first = false;

                sb.Append(Stringify(subenum));
            }
            sb.Append("}");
            return sb.ToString();
        }

        private string Stringify<T>(IEnumerable<T> en)
        {
            return "{" + string.Join(", ", en) + "}";
        }
    }

}