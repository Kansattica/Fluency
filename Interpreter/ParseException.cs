using System;

namespace Fluency.Interpreter.Exceptions
{
    class ParseException : Exception
    {
        public int LineNumber { get; set; }
        public int? Position { get; set; }
        public string Snippet { get; set; }

        public ParseException(string message) : base(message) { }
        public ParseException(string message, params object[] args) :
            base(String.Format(message, args))
        { }
        public ParseException(string message, Exception inner) : base(message, inner) { }

        public override string Message
        {
            get
            {
                return $"Parse error at line number {LineNumber}." +
                (Position == null ? string.Empty : $"\nAt position {Position}") +
                (Snippet == null ? string.Empty : $"\n Code: {Snippet}") +
                "\n\n" + base.Message;
            }
        }



    }

}