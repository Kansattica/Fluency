using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fluency.CLI;

namespace Fluency.Tests.Console
{
    [TestClass]
    public class AutofixTests
    {
        [TestMethod]
        [DataRow("\t\t\t\t\t\t\t")]
        [DataRow("\t       \t\t\t\t\t\t")]
        [DataRow("          ")]
        [DataRow("      //comment")]
        [DataRow("\t\t // hi mom")]
        [DataRow("// :)")]
        public void NoFixBlankLines(string toTest)
        {
            var fixer = new Autofix(tabsAfterText: true);

            Assert.AreEqual(toTest, fixer.FixString(toTest));
        }

        [TestMethod]
        [DataRow("\t\t\t .SomeText()\t ", "\t\t\t .SomeText()   ")]
        [DataRow("\t\t\t\\.SomeText()\t ", "\t\t\t\\.SomeText()   ")]
        [DataRow(@"\t\t\t\.SomeText()\t ", @"\t\t\t\.SomeText()\t ")]
        [DataRow("\t \\.SomeText()   \\.SomeOtherText().OverHere()./      ", "\t \\.SomeText()   \\.SomeOtherText().OverHere()./      ")]
        public void FixTabsAfterText(string toTest, string expected)
        {
            var fixer = new Autofix(tabsAfterText: true, tabWidth: 2);

            Assert.AreEqual(expected, fixer.FixString(toTest));
        }
    }

}