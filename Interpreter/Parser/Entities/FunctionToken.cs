using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Fluency.Interpreter.Parser.Entities
{
    /// <summary>
    /// Represents a parsed function from source control.
    /// </summary>
    public class FunctionToken
    {
        /// <summary>
        /// The function's name.
        /// </summary>
        /// <value></value>
        public string Name { get; private set; }

        /// <summary>
        /// The arguments the function takes.
        /// </summary>
        /// <value></value>
        public string[] Arguments { get; private set; } = new string[0];

        /// <summary>
        /// The line number this function was on. Not available when constructed, has to be set from the outside for better metadata and error location.
        /// </summary>
        /// <value></value>
        public int? Line { get; set; } 

        /// <summary>
        /// The inclusive range of indexes this was in its original line.
        /// </summary>
        /// <value></value>
        public Range Range { get; private set; }

        /// <summary>
        /// What the function looked like before it got spaces, periods, and slashes trimmed.
        /// </summary>
        /// <value></value>
        public string Original { get; private set; }

        /// <summary>
        /// Whether the function connects to the one above it for input (starts with \.)
        /// </summary>
        /// <value></value>
        public bool ConnectsUpBefore { get; private set; }

        /// <summary>
        /// Whether the function connects to the one before it (starts with .)
        /// </summary>
        /// <value></value>
        public bool ConnectsBefore { get; private set; }

        /// <summary>
        /// Whether the function connects to the one above it for output (ends with ./)
        /// </summary>
        /// <value></value>
        public bool ConnectsUpAfter { get; private set; }

        /// <summary>
        /// Create a new function token.
        /// </summary>
        /// <param name="toparse"></param>
        /// <param name="start">The first index into the line containing this function.</param>
        /// <param name="end">The last index into the line containing this function.</param>
        public FunctionToken(string toparse, int start, int end)
        {
            Range = new Range(start, end);
            ParseNameAndArgs(toparse);
        }

        private static readonly char[] _leftp = new[] { '(' }; //gotta give split arguments as arrays
        private void ParseNameAndArgs(string func)
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

        /// <summary>
        /// Prettyprints some metadata about the function token.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Name: {Name}, Args: {string.Join(", ", Arguments)}, LineNumber: {Line}, Range: {Range}, Connects Before: {ConnectsUpBefore} After: {ConnectsUpAfter}";
        }
    }
}