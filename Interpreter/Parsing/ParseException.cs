using System;
using Fluency.Interpreter.Parsing.Entities;

namespace Fluency.Interpreter.Parsing.Exceptions
{
    ///<summary>
    /// Represents an error that ocurred while parsing the source code.
    ///</summary>
    public class ParseException : Exception
    {
        public int LineNumber { get; set; }
        public int? Position { get; set; }
        public Range Range { get; set; }
        public string Snippet { get; set; }

        public FunctionToken FunctionToken { set { Range = value.Range; Snippet = value.Original; LineNumber = value.Line ?? LineNumber; } }

        public ParseException(string message) : base(message) { }
        public ParseException(string message, params object[] args) :
            base(String.Format(message, args))
        { }
        public ParseException(string message, Exception inner) : base(message, inner) { }

        private string PrintableMetadata()
        {
            return $"Parse error at line number {LineNumber}." +
                            (Position == null ? string.Empty : $"\nAt position {Position}") +
                            (Range == null ? string.Empty : $"\n At positions {Range}") +
                            (string.IsNullOrWhiteSpace(Snippet) ? string.Empty : $"\n Code: {Snippet}");
        }

        public override string Message
        {
            get { return PrintableMetadata() + "\n\n" + base.Message; }
        }

        public override string ToString() => PrintableMetadata() + "\n\n" + base.ToString();
    }

}