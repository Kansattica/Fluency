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

        public FunctionToken(string toparse, int start, int end)
        {
            Range = new Range(start, end);
            ParseNameAndArgs(toparse);
        }

        private static readonly char[] _leftp = new[] { '(' }; //gotta give split arguments as arrays
        public void ParseNameAndArgs(string func)
        {
            var s = func.Trim().Split(_leftp, 2);
            Name = s[0].TrimStart('.', '\\');
            string args = s[1].TrimEnd(')', '.', '/');
            if (!string.IsNullOrWhiteSpace(args))
            {
                Arguments = args.Split(',').Select(x => x.Trim()).ToArray();
            }
        }

        public override string ToString()
        {
            return $"Name: {Name}, Args: {string.Join(", ", Arguments)}, LineNumber: {Line}, Range: {Range}";
        }
    }
}