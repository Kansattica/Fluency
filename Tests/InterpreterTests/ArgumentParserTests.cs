using System;
using Fluency.Interpreter.Parsing.Entities;
using Fluency.Interpreter.Parsing.Entities.ArgumentTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fluency.Tests.Parsing
{
    [TestClass]
    public class ArgumentParserTests
    {
        [TestMethod]
        [DataRow("1", 1)]
        [DataRow("123123123", 123123123)]
        [DataRow("069", 69)]
        [DataRow("0", 0)]
        [DataRow("-1", -1)]
        public void CanParseInts(string toparse, int expected)
        {
            if (Argument.TryParse(toparse, out var argument))
            {
                Assert.IsInstanceOfType(argument, typeof(IntArg));
                Assert.AreEqual(expected, argument.GetAs<int>());
                Assert.AreEqual(ParsedType.Int, argument.Type);
            }
            else
            {
                Assert.Fail("Could not parse {0}.", toparse);
            }
        }

        [TestMethod]
        [DataRow("1.5", 1.5)]
        [DataRow("1.69", 1.69)]
        [DataRow("NaN", double.NaN)]
        [DataRow("1000000000.2", 1000000000.2)]
        public void CanParseDoubles(string toparse, double expected)
        {
            if (Argument.TryParse(toparse, out var argument))
            {
                Assert.IsInstanceOfType(argument, typeof(DoubleArg));
                Assert.AreEqual(expected, argument.GetAs<double>());
                Assert.AreEqual(ParsedType.Double, argument.Type);
            }
            else
            {
                Assert.Fail("Could not parse {0}.", toparse);
            }
        }

        [TestMethod]
        public void RandomIntParse()
        {
            Random r = new Random();
            for (int i = 0; i < 10000000; i++)
            {
                int expected = r.Next(int.MinValue, int.MaxValue);
                string toparse = expected.ToString();
                if (Argument.TryParse(toparse, out var argument))
                {
                    Assert.IsInstanceOfType(argument, typeof(IntArg));
                    Assert.AreEqual(expected, argument.GetAs<int>());
                    Assert.AreEqual(ParsedType.Int, argument.Type);
                }
                else
                {
                    Assert.Fail("Could not parse {0}. (was {1})", toparse, expected);
                }
            }
        }

        [TestMethod]
        public void RandomDoubleParse()
        {
            Random r = new Random();
            for (int i = 0; i < 10000000; i++)
            {
                double expected = r.NextDouble() * r.Next(-100, 100);
                string toparse = expected.ToString();
                if (Argument.TryParse(toparse, out var argument))
                {
                    if (argument.Type == ParsedType.Int && argument.GetAs<int>() == 0)
                        continue;

                    Assert.IsInstanceOfType(argument, typeof(DoubleArg));
                    Assert.AreEqual(expected, argument.GetAs<double>(), .0000000001);
                    Assert.AreEqual(ParsedType.Double, argument.Type);
                }
                else
                {
                    Assert.Fail("Could not parse {0}. (was {1})", toparse, expected);
                }
            }
        }

        [TestMethod]
        [DataRow("\"Howdy\"", "Howdy")]
        [DataRow("\"Howdy  \"", "Howdy  ")]
        [DataRow("\" Howdy  \"", " Howdy  ")]
        [DataRow("\"iuhoih&YH&I*^&*6g78g\"", "iuhoih&YH&I*^&*6g78g")]
        [DataRow("\"check this \\\" guy out\"", "check this \" guy out")]
        public void CanParseStrings(string toparse, string expected)
        {
            if (Argument.TryParse(toparse, out var argument))
            {
                Assert.IsInstanceOfType(argument, typeof(StringArg));
                Assert.AreEqual(expected, argument.GetAs<string>());
                Assert.AreEqual(ParsedType.String, argument.Type);
            }
            else
            {
                Assert.Fail("Could not parse {0}.", toparse);
            }
        }

        [TestMethod]
        [DataRow("Howdy")]
        [DataRow("Switch")]
        [DataRow("CoolFunc")]
        [DataRow("FunctionTime")]
        [DataRow("Any")]
        public void CanParseFunctions(string toparse)
        {
            if (Argument.TryParse(toparse, out var argument))
            {
                Assert.IsInstanceOfType(argument, typeof(FunctionArg));
                Assert.AreEqual(toparse, argument.GetAs<string>());
                Assert.AreEqual(ParsedType.Function, argument.Type);
            }
            else
            {
                Assert.Fail("Could not parse {0}.", toparse);
            }
        }

        [TestMethod]
        [DataRow("int n", ParsedType.Int, "n")]
        [DataRow("float floaty", ParsedType.Double, "floaty")]
        [DataRow("double floaty", ParsedType.Double, "floaty")]
        [DataRow("int    n", ParsedType.Int, "n")]
        [DataRow("float \tfloaty", ParsedType.Double, "floaty")]
        [DataRow("double    floaty", ParsedType.Double, "floaty")]
        [DataRow("string n", ParsedType.String, "n")]
        [DataRow("str string", ParsedType.String, "string")]
        [DataRow("func f", ParsedType.Function, "f")]
        [DataRow("fun predicate", ParsedType.Function, "predicate")]
        [DataRow("any afun", ParsedType.Any, "afun")]
        [DataRow("...", ParsedType.Any, "...")]
        [DataRow("int ...", ParsedType.Int, "...")]
        public void CanParseFunctionsWithTypenames(string toparse, ParsedType expectedType, string expectedName)
        {
            if (Argument.TryParse(toparse, out var argument))
            {
                Assert.IsInstanceOfType(argument, typeof(FunctionArg));
                Assert.AreEqual(ParsedType.Function, argument.Type);
                Assert.AreEqual(expectedName, argument.GetAs<string>());
                FunctionArg arg = (FunctionArg)argument;
                Assert.AreEqual(expectedType, arg.DeclaredType);
            }
            else
            {
                Assert.Fail("Could not parse {0}.", toparse);
            }
        }

        [TestMethod]
        [DataRow("\"H\"owdy")]
        [DataRow("Wor34234ds")]
        [DataRow("&*&^(^(&*^$")]
        [DataRow("There's spaces here")]
        [DataRow("Func22")]
        public void WontParse(string toparse)
        {
            try
            {
                if (Argument.TryParse(toparse, out var argument))
                    Assert.Fail("Should not have parsed string {0}. Thought it was an {1}.", toparse, argument.Type);
            }
            catch (Exception ex) //This is fine, too.
            {
                Assert.AreEqual("ParseException", ex.GetType().Name);
            }
        }
    }

}
