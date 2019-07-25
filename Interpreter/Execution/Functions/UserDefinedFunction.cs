using Fluency.Interpreter.Parsing.Entities;
using Fluency.Interpreter.Parsing.Entities.FunctionGraph;

namespace Fluency.Interpreter.Execution.Functions
{
    class UserDefinedFunction : IFunction, ITopIn, ITopOut, IBottomIn, IBottomOut
    {
        public string Name { get { return Graph.Name; } }
        public Argument[] Arguments { get { return Graph.Arguments; } }
        public FunctionGraph Graph { get; private set; }
        public GetNext TopInput { set => throw new System.NotImplementedException(); }
        public GetNext BottomInput { set => throw new System.NotImplementedException(); }

        public Value Top()
        {
            throw new System.NotImplementedException();
        }

        public Value Bottom()
        {
            throw new System.NotImplementedException();
        }

        public UserDefinedFunction(FunctionGraph graph)
        {
            Graph = graph;
        }
    }
}