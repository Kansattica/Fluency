using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Fluency.Interpreter.Parser.Entities;
using Fluency.Interpreter.Parser.Exceptions;

namespace Fluency.Interpreter.Parser
{
    public class Parser
    {
        private bool _verbose = false;

        private readonly string _spaces;

        public Parser(bool verbose = false, int tabWidth = 4)
        {
            _verbose = verbose;
            _spaces = Enumerable.Range(0, tabWidth).Select(_ => ' ').Stringify(); //ahh, fluent
        }

        public void Parse(IEnumerable<string> lines)
        {
            lines.Select(Line.Create)
            .Where(x => !IsBlank(x.Contents)) //yeet blank lines
            .GroupUntil(x => x.Contents.StartsWith("Def(")) //group blank and nonblank lines
            .Select(Tokenize).ToList();
        }

        private string ExpandTabs(string toExpand) => toExpand.Replace("\t", _spaces);

        private IEnumerable<FunctionToken> Tokenize(IEnumerable<Line> lines, int nthfunc)
        {
            if (_verbose)
            {
                Console.WriteLine("Function #" + nthfunc);
                foreach (Line line in lines)
                    Console.WriteLine(line.ToString());
                Console.WriteLine();
            }

            return lines.SelectMany(TokenizeLine);
        }

        private IEnumerable<FunctionToken> TokenizeLine(Line line)
        {
            try
            {
                return line.Contents.GroupUntil(x => x == '.').Select(TokenizeFunction);
            }
            catch (ParseException ex)
            {
                ex.LineNumber = line.Number;
                throw;
            }
        }

        private FunctionToken TokenizeFunction(UntilGroup<char> parsedfunc, int line)
        {
            string func = parsedfunc.Stringify();
            CheckMatchingParens(func);

            return new FunctionToken(func, parsedfunc.Indexes.Min, parsedfunc.Indexes.Max, line);
        }

        private bool CheckMatchingParens(string str)
        {
            //exactly one set of matching parens
            bool left, right; left = right = false;

            int idx = 0;
            foreach (char c in str)
            {
                if (c == '(')
                {
                    if (left)
                        throw new ParseException("Too many left parenthesis in string {0}") { LineNumber = idx, Snippet = str };
                    else
                        left = true;

                }

                if (c == ')')
                {
                    if (left)
                    {
                        throw new ParseException("Right parenthesis before left in string {0}") { LineNumber = idx, Snippet = str };
                    }

                    if (right)
                    {
                        throw new ParseException("Too many right parens in string {0}") { LineNumber = idx, Snippet = str };
                    }

                    right = true;
                }

                if (left && right)
                    return true;

                idx++;
            }

            throw new ParseException("Mismatched left and right parens in string {0}") { LineNumber = idx, Snippet = str };
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
