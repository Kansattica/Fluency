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

        private Func<UserDefinedFunction> makeNewFunction;
        private UserDefinedFunction expandedFunction;

        private bool topset = false;
        public Value Top()
        {
            if (expandedFunction == null)
            {
                expandedFunction = makeNewFunction();
            }

            if (!topset)
            {
                expandedFunction.TopInput = TopInput;
                topset = true;
            }

            Value v = expandedFunction.Top();
            if (v.Done)
            {
                bottomset = false;
                expandedFunction = makeNewFunction();
                expandedFunction.TopInput = TopInput;
                v = expandedFunction.Top();
            }
            return v;
        }

        private bool bottomset = false;
        public Value Bottom()
        {
            if (expandedFunction == null)
            {
                expandedFunction = makeNewFunction();
            }

            if (!bottomset)
            {
                expandedFunction.BottomInput = BottomInput;
                bottomset = true;
            }

            Value v = expandedFunction.Bottom();
            if (v.Done)
            {
                topset = false;
                expandedFunction = makeNewFunction();
                expandedFunction.BottomInput = BottomInput;
                v = expandedFunction.Bottom();
            }
            return v;
        }


        public UserFunctionStub(FunctionGraph graph, Value[] arguments, IFunctionResolver linker)
        {
            Name = graph.Name;
            makeNewFunction = (() => new UserDefinedFunction(graph, arguments, linker));
        }
    }
}