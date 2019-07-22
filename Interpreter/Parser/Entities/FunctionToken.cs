using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Fluency.Interpreter.Parser.Entities
{
    public class FunctionToken
    {
        //public static Regex ExtractNameAndArgs = new Regex(@"(?<name>[a-zA-Z]+)\(((?<arguments>[0-9]+|true|false|[""].?[""]),? *)*\)", RegexOptions.ExplicitCapture);
        public string Name;
        public string[] Arguments = new string[0];
        public int Line;
        public Range Range;

        public FunctionToken(string toparse, int start, int end, int line)
        {
            ParseNameAndArgs(toparse);
            Range = new Range(start, end);
        }

        private static readonly char[] _leftp = new[] { '(' }; //gotta give split arguments as arrays
        public void ParseNameAndArgs(string func)
        {
            var s = func.Split(_leftp, 2);
            Name = s[0];
            string args = s[1].TrimEnd(')');
            if (!string.IsNullOrWhiteSpace(args))
            {
                Arguments = args.Split(',');
            }

        }
    }
}