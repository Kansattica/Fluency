using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Fluency.Interpreter.Exceptions;
using System.Text.RegularExpressions;

namespace Fluency.Interpreter
{
    public class Parser
    {
        private bool verbose = false;

        public Parser(bool verbose = false)
        {
            this.verbose = verbose;
        }

        public void Parse(IEnumerable<string> lines) 
        {   
            lines.Select(Line.Create)
            .Where(x => !IsBlank(x.Contents)) //yeet blank lines
            .GroupUntil(x => x.Contents.StartsWith("Def(")) //group blank and nonblank lines
            .Select(Tokenize).ToList();
        }

        private IEnumerable<FunctionToken> Tokenize(IEnumerable<Line> lines, int nthfunc)
        {
            if (verbose)
            {
                Console.WriteLine("Function #" + nthfunc);
                foreach (Line line in lines)
                    Console.WriteLine(line.ToString());
                Console.WriteLine();
            }

           return lines.SelectMany(TokenizeLine);
        }

        private IEnumerable<FunctionToken> TokenizeLine(Line line, int inFunc)
        {
            return line.Contents.Split('.').Select(TokenizeFunction);
        }

        private FunctionToken TokenizeFunction(string func, int nthfunc)
        {
            CheckMatchingParens(func, nthfunc);

            return new FunctionToken(func);
        }

        private bool CheckMatchingParens(string str, int line)
        {
            //exactly one set of matching parens
            bool left, right; left = right = false;

            int idx = 0;
            foreach (char c in str)
            {
                if (c == '(')
                {
                    if (left)
                        throw new ParseException("Too many left parenthesis in string {0}", line, idx, str);
                    else
                        left = true;

                }

                if (c == ')')
                {
                    if (left)
                    {
                        throw new ParseException("Right parenthesis before left in string {0}", line, idx, str);
                    }

                    if (right)
                    {
                        throw new ParseException("Too many right parens in string {0}", line, idx, str);
                    }

                    right = true;
                }

                if (left && right)
                    return true;

                idx++;
            }

            throw new ParseException("Mismatched left and right parens in string {0}", line, idx, str);
        }

        private bool IsBlank(string line)
        {
            //a line is "blank" if it consists of nothing but whitespace or has the comment characters // before anything else
            //haha! an old fashioned for loop, baby!
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                
                if (i < line.Length-1 && c == '/' && line[i+1] == '/')
                    return true;

                if (!char.IsWhiteSpace(c))
                    return false;
            }

            return true;
        }
    }
}
