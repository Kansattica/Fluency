using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Fluency.Execution.Parsing.Exceptions;

namespace Fluency.Execution.Parsing.Entities
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
        /// The top arguments the function takes.
        /// </summary>
        /// <value></value>
        public Argument[] TopArguments { get; private set; } = new Argument[0];

        /// <summary>
        /// The bottom arguments the function takes.
        /// </summary>
        /// <value></value>
        public Argument[] BottomArguments { get; private set; } = new Argument[0];

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
        private static readonly char[] _pipe = new[] { '|' }; //gotta give split arguments as arrays
        private void ParseNameAndArgs(string func)
        {
            Original = func;
            var s = func.Trim().Split(_leftp, 2);
            ConnectsUpBefore = s[0].StartsWith(@"\.");
            ConnectsBefore = s[0].StartsWith(".");
            ConnectsUpAfter = s[1].EndsWith("./");
            Name = s[0].TrimStart('.', '\\');
            if (!char.IsUpper(Name[0]))
                throw new ParseException("Fluency functions must start with a capital letter.") { FunctionToken = this };
            string args = TrimTrailingFromArguments(s[1]);

            if (!string.IsNullOrWhiteSpace(args))
            {
                var topbottom = args.Split(_pipe, 2);

                if (!string.IsNullOrWhiteSpace(topbottom[0]))
                    TopArguments = ParseArguments(SplitArgument(topbottom[0]).Select(x => x.Trim())).ToArray();

                if (topbottom.Length == 2 && !string.IsNullOrWhiteSpace(topbottom[1])) //if there are bottom arguments
                    BottomArguments = ParseArguments(SplitArgument(topbottom[1]).Select(x => x.Trim())).ToArray();
            }
        }

        private string TrimTrailingFromArguments(string args)
        {
            // remove the patterns:
            // )
            // )./
            // ).
            // without removing the same with a ... in front
            int ellipses = args.IndexOf("...");
            if (ellipses == -1)
                return args.TrimEnd(')', '.', '/');
            
            return args.Remove(ellipses + 3); //3 is the length of the ...
        }


        private IEnumerable<Argument> ParseArguments(IEnumerable<string> args)
        {
            foreach (string arg in args)
            {
                if (!Argument.TryParse(arg, out Argument argument))
                    throw new ParseException("Could not parse argument {0}", arg) { FunctionToken = this };

                yield return argument;
            }
        }

        private IEnumerable<string> SplitArgument(IEnumerable<char> source)
        {
            bool indoublequotes = false;
            return source.GroupWhile((c, inArgument) =>
            {
                indoublequotes = (c == '"' ? !indoublequotes : indoublequotes);
                if (inArgument)
                {
                    if (c == ',' && !indoublequotes) { return GroupWhileAction.LeaveExclude; }
                    return GroupWhileAction.In;
                }
                else
                {
                    if (char.IsWhiteSpace(c)) { return GroupWhileAction.StillOut; }
                    return GroupWhileAction.In;
                }
            }).Select(x => x.Stringify());
        }

        /// <summary>
        /// Prettyprints some metadata about the function token.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Name: {Name}, Args: {TopArguments.Stringify()}, LineNumber: {Line}, Range: {Range}, Connects Before: {ConnectsUpBefore} After: {ConnectsUpAfter}";
        }
    }
}