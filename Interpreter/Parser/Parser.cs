using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Fluency.Interpreter.Parser.Entities;
using Fluency.Interpreter.Parser.Exceptions;
using System.Runtime.CompilerServices;
using Fluency.Interpreter.Parser.Entities.FunctionGraph;

namespace Fluency.Interpreter.Parser
{
    /// <summary>
    /// Represents a Fluency parser.
    /// </summary>
    public class Parser
    {
        private readonly bool _verbose = false;
        private readonly bool _tabWarn;
        private readonly string _spaces;

        /// <summary>
        /// Construct a new parser.
        /// </summary>
        /// <param name="verbose">Be verbose.</param>
        /// <param name="tabWarn">Whether to warn about tabs after text.</param>
        /// <param name="tabWidth">How wide to try to expand tabs to.</param>
        public Parser(bool verbose = false, bool tabWarn = true, int tabWidth = 4)
        {
            _verbose = verbose;
            _spaces = Enumerable.Range(0, tabWidth).Select(_ => ' ').Stringify(); //ahh, fluent
            if (_spaces.Length != tabWidth)
                throw new ArgumentException("Go yell at the programmer for expanding tabs wrong.");
            _tabWarn = tabWarn;
        }

        /// <summary>
        /// Parse an IEnumerable of lines, such as those from File.ReadAllLines.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public IEnumerable<FunctionGraph> Parse(IEnumerable<string> lines)
        {
            return lines.Select(x => x.TrimEnd())
            .Select(ProblematicTabWarn)
            .Select(ExpandTabs)
            .Select(Line.Create)
            .Where(x => !IsBlank(x.Contents)) //yeet blank lines
            .GroupUntil(x => x.Contents.StartsWith("Def(")) //group blank and nonblank lines
            .Select(Tokenize)
            .Select(x => new FunctionGraph(x));
        }

        private string ExpandTabs(string toExpand) => toExpand.Replace("\t", _spaces);

        private string ProblematicTabWarn(string line, int number)
        {
            if (_tabWarn && line.TrimStart().Contains('\t'))
                Console.WriteLine("NOTE: Line {0} contains tabs after text. Many text editors use \"soft tabs\" that don't look four spaces wide all the time. Consider setting your editor to display hard tabs or using spaces to align code. Note that tabs at the beginning of a line, before any text, are fine.", number + 1);
            return line;
        }

        private IEnumerable<IEnumerable<FunctionToken>> Tokenize(IEnumerable<Line> lines, int nthfunc)
        {
            if (_verbose)
            {
                Console.WriteLine("Function #" + nthfunc);
                foreach (Line line in lines)
                    Console.WriteLine(line.ToString());
                Console.WriteLine();
            }

            return lines.Select(TokenizeLine);
        }

        private IEnumerable<FunctionToken> TokenizeLine(Line line)
        {
            try
            {
                int doublequotes = 0;
                return line.Contents.GroupWhile((c, infunc) =>
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
                })
                .MergeLastIf(x => x.Stringify().StartsWith("./"),
                    (previous, current) => new Grouped<char>(previous.Concat(current).ToList(), previous.Indexes.Min, current.Indexes.Max))
                .Select(TokenizeFunction)
                .Select(x => { x.Line = line.Number; return x; });
                // .ToList(); //if you don't evaluate a little, that catch block never gets called?
            }
            catch (ParseException ex)
            {
                ex.LineNumber = line.Number; throw;
            }
        }

        private FunctionToken TokenizeFunction(Grouped<char> parsedfunc, int nthfunc)
        {
            string func = parsedfunc.Stringify();

            try
            {
                CheckMatchingParens(func);
                return new FunctionToken(func, parsedfunc.Indexes.Min, parsedfunc.Indexes.Max);
            }
            catch (Exception e)
            {
                ParseException ex = new ParseException("Error occurred while parsing a function.", e);
                ex.Snippet = func;
                ex.Range = parsedfunc.Indexes;
                throw ex;
            }

        }

        private bool CheckMatchingParens(string str)
        {
            var justparens = str.Select((c, idx) => (c, idx)).SkipBetween(x => x.c == '"', x => x.c == '"')
                .Where(x => x.c == '(' || x.c == ')');
            string parens = justparens.Select(x => x.c).Stringify();

            if (parens == "()")
                return true;

            var lefts = justparens.Where(x => x.c == '(');
            if (lefts.Count() > 1)
                throw new ParseException("Too many left parenthesis.") { Snippet = str, Position = lefts.Last().idx };

            if (lefts.Count() == 0)
                throw new ParseException("Missing left parenthesis.") { Snippet = str };

            var rights = justparens.Where(x => x.c == ')');
            if (rights.Count() > 1)
                throw new ParseException("Too many left parenthesis.") { Snippet = str, Position = rights.Last().idx };

            if (rights.Count() == 0)
                throw new ParseException("Missing left parenthesis.") { Snippet = str };

            throw new ParseException("Mismatched left and right parenthesis.") { Snippet = str };
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
