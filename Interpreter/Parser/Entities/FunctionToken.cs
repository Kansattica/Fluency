using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Fluency.Interpreter.Parser.Entities
{
    public class FunctionToken
    {
        //public static Regex ExtractNameAndArgs = new Regex(@"(?<name>[a-zA-Z]+)\(((?<arguments>[0-9]+|true|false|[""].?[""]),? *)*\)", RegexOptions.ExplicitCapture);
        public string Name { get; private set; }
        public string[] Arguments { get; private set; } = new string[0];
        public int? Line { get; set; } //this has to be set from outside
        public Range Range { get; private set; }
        public string Original { get; private set; }
        public bool ConnectsUpBefore { get; private set; }
        public bool ConnectsBefore { get; private set; }
        public bool ConnectsUpAfter { get; private set; }

        public FunctionToken(string toparse, int start, int end)
        {
            Range = new Range(start, end);
            ParseNameAndArgs(toparse);
        }

        private static readonly char[] _leftp = new[] { '(' }; //gotta give split arguments as arrays
        public void ParseNameAndArgs(string func)
        {
            Original = func;
            var s = func.Trim().Split(_leftp, 2);
            ConnectsUpBefore = s[0].StartsWith(@"\.");
            ConnectsBefore = s[0].StartsWith(".");
            ConnectsUpAfter = s[1].EndsWith("./");
            Name = s[0].TrimStart('.', '\\');
            string args = s[1].TrimEnd(')', '.', '/');
            if (!string.IsNullOrWhiteSpace(args))
            {
                Arguments = args.Split(',').Select(x => x.Trim()).ToArray();
            }
        }

        public override string ToString()
        {
            return $"Name: {Name}, Args: {string.Join(", ", Arguments)}, LineNumber: {Line}, Range: {Range}, Connects Before: {ConnectsUpBefore} After: {ConnectsUpAfter}";
        }
    }
}