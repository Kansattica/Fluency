using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Fluency.Interpreter.Parser.Entities;
using Fluency.Interpreter.Parser.Exceptions;
using System.Runtime.CompilerServices;

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

        public IEnumerable<IEnumerable<FunctionToken>> Parse(IEnumerable<string> lines)
        {
            return lines.Select(Line.Create)
            .Where(x => !IsBlank(x.Contents)) //yeet blank lines
            .GroupUntil(x => x.Contents.StartsWith("Def(")) //group blank and nonblank lines
            .Select(Tokenize);
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
                return line.Contents.GroupUntil(x => x == ')', inclusive: true).Select(TokenizeFunction);
            }
            catch (ParseException ex)
            {
                ex.LineNumber = line.Number; throw;
            }
        }

        private FunctionToken TokenizeFunction(UntilGroup<char> parsedfunc, int line)
        {
            string func = parsedfunc.Stringify();

            try
            {
                CheckMatchingParens(func);
            }
            catch (ParseException ex)
            {
                ex.Range = parsedfunc.Indexes; throw;
            }

            return new FunctionToken(func, parsedfunc.Indexes.Min, parsedfunc.Indexes.Max, line);
        }

        private bool CheckMatchingParens(string str)
        {
            var justparens = str.Select((c, idx) => (c, idx)).Where(x => x.c == '(' || x.c == ')');
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
