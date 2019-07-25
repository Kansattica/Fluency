using System;
using System.Collections.Generic;

namespace Fluency.Interpreter.Execution.Functions.BuiltIn
{
    class Switch : IFunction
    {
        private GetNext _beforeTop;
        private GetNext _afterTop;

        public string Name => "Switch";

        public GetNext BeforeTop { set => _beforeTop = value; }
        public GetNext BeforeBottom { set => _afterTop = value; }

        public Switch(Value[] arguments)
        {
            if (arguments.Length > 1) { throw new ArgumentException("Switch takes either zero or one argument."); }

        }

        public Value Top()
        {
            throw new NotImplementedException();
        }

        public Value Bottom()
        {
            throw new NotImplementedException();
        }
    }
}