using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace Fluency.CLI
{
    public class Autofix
    {

        private readonly List<Func<string, string>> strategies = new List<Func<string, string>>();

        /// <summary>
        /// Create and configure an autofixer.
        /// </summary>
        /// <param name="tabsAfterText">Whether to replace tabs after non-whitespace characters with spaces.</param>
        /// <param name="tabWidth">If tabsaftertext is true, how many spaces to replace tabs with.</param>
        public Autofix(bool tabsAfterText = false, int tabWidth = 4)
        {
            if (tabsAfterText) { strategies.Add((s) => TabsAfterText(s, new string(' ', tabWidth))); }
        }

        /// <summary>
        /// Read fileName into memory, try to fix each line, and save it back if it's changed.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>True if the file was changed and written back. False if it wasn't.</returns>
        public bool FixFile(string fileName)
        {
            var fixedLines = File.ReadAllLines(fileName)
                                .Select(line => new { Old = line, Fixed = FixString(line) });

            if (fixedLines.Any(x => x.Old != x.Fixed))
            {
                File.WriteAllLines(fileName, fixedLines.Select(x => x.Fixed).ToArray());
                return true;
            }
            return false;
        }

        public string FixString(string tofix)
        {
            if (IsBlank(tofix)) { return tofix; }
            return strategies.Aggregate(tofix, (str, fun) => fun(str));
        }


        private string TabsAfterText(string tofix, string replaceTabs)
        {
            var firstNonWhitespace = tofix.Select((c, idx) => (c, idx)).First(x => !char.IsWhiteSpace(x.c));
            return tofix.Substring(0, firstNonWhitespace.idx) + tofix.Substring(firstNonWhitespace.idx).Replace("\t", replaceTabs);
        }

        private bool IsBlank(string line)
        {
            //a line is "blank" if it consists of nothing but whitespace or has the comment characters // before anything else
            //haha! an old fashioned for loop, baby!
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (i < line.Length - 1 && c == '/' && line[i + 1] == '/')
                    return true;

                if (!char.IsWhiteSpace(c))
                    return false;
            }

            return true;
        }

    }
}