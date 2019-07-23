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
    public class Parser
    {
        private bool _verbose = false;

        private readonly string _spaces;

        public Parser(bool verbose = false, int tabWidth = 4)
        {
            _verbose = verbose;
            _spaces = Enumerable.Range(0, tabWidth).Select(_ => ' ').Stringify(); //ahh, fluent
        }

        public IEnumerable<FunctionGraph> Parse(IEnumerable<string> lines)
        {
            return lines.Select(x => x.TrimEnd())
            .Select(ExpandTabs)
            .Select(Line.Create)
            .Where(x => !IsBlank(x.Contents)) //yeet blank lines
            .GroupUntil(x => x.Contents.StartsWith("Def(")) //group blank and nonblank lines
            .Select(Tokenize)
            .Select(x => new FunctionGraph(x));
        }

        private string ExpandTabs(string toExpand) => toExpand.Replace("\t", _spaces);

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
                int numberOfQuotes = 0;
                return line.Contents.GroupUntil(x =>
                { //don't count close parens inside string literals
                    numberOfQuotes += x == '"' ? 1 : 0;
                    return numberOfQuotes % 2 == 0 ? x == ')' || x == '/' : false;
                }, inclusive: true)
                .MergeLastIf(x => x.Stringify().StartsWith("./"),
                    (previous, current) => new UntilGroup<char>(previous.Concat(current).ToList(), previous.Indexes.Min, current.Indexes.Max))
                .Select(TokenizeFunction)
                .Select(x => { x.Line = line.Number; return x; })
                .ToList(); //if you don't evaluate a little, that catch block never gets called
            }
            catch (ParseException ex)
            {
                ex.LineNumber = line.Number; throw;
            }
        }



        private FunctionToken TokenizeFunction(UntilGroup<char> parsedfunc, int nthfunc)
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
