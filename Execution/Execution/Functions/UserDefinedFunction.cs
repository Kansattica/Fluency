using Fluency.Execution.Parsing.Entities;
using Fluency.Execution.Parsing.Entities.FunctionGraph;

namespace Fluency.Execution.Functions
{
    public class UserDefinedFunction : IFunction, ITopIn, ITopOut, IBottomIn, IBottomOut
    {
        public string Name { get { return _graph.Name; } }
        public Argument[] Arguments { get { return _graph.Arguments; } }

        private FunctionGraph _graph;

        public GetNext TopInput { private get; set; }
        public GetNext BottomInput { private get; set; }


        private ExecutableGraph _expandedGraph = null;

        public Value Top()
        {
            throw new System.NotImplementedException();
        }

        public Value Bottom()
        {
            throw new System.NotImplementedException();
        }

        private void Expand()
        {

        }

        public UserDefinedFunction(FunctionGraph graph, Value[] arguments)
        {
            _graph = graph;
            Expand();
        }
    }
}