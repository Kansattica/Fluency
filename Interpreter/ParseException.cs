using System;

namespace Fluency.Interpreter.Exceptions
{
    class ParseException : Exception
    {
        public ParseException(string message) : base(message) { }
        public ParseException(string message, int line, params object[] args) : 
            base(String.Format(FormatLine(line) + message, args)) { }
        public ParseException(string message, int line, int pos, params object[] args) : 
            base(string.Format(FormatLinePos(line, pos) + message, args)) { }
        public ParseException(string message, Exception inner) : base(message, inner) { }


        private static string FormatLine(int line) => line + ": ";
        private static string FormatLinePos(int line, int pos) => line + ":" + pos + ": ";

    }

}