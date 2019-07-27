using System;
using Fluency.Execution.Parsing.Entities.FunctionGraph;

namespace Fluency.Execution.Functions
{
    /// <summary>
    /// User-defined functions should "expand" on first use so you can do recursion without overflowing the stack.
    /// </summary>
    public class UserFunctionStub : IFunction, ITopIn, ITopOut, IBottomIn, IBottomOut
    {
        public string Name { get; private set; }


        public GetNext TopInput { private get; set; }
        public GetNext BottomInput { private get; set; }

        private Lazy<UserDefinedFunction> toexpand = null;

        private bool topset = false;
        public Value Top()
        {
            var expanded = toexpand.Value;

            if (!topset)
            {
                expanded.TopInput = TopInput;
                topset = true;
            }

            return expanded.Top();
        }

        private bool bottomset = false;
        public Value Bottom()
        {
            var expanded = toexpand.Value;

            if (!bottomset)
            {
                expanded.BottomInput = BottomInput;
                bottomset = true;
            }

            return expanded.Bottom();
        }


        public UserFunctionStub(FunctionGraph graph, Value[] arguments, IFunctionResolver linker)
        {
            Name = graph.Name;
            toexpand = new Lazy<UserDefinedFunction>(() => new UserDefinedFunction(graph, arguments, linker));
        }
    }
}