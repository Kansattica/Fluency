using Fluency.Interpreter.Parsing.Entities;
using Fluency.Interpreter.Parsing.Entities.FunctionGraph;

namespace Fluency.Interpreter.Execution.Functions
{
    class UserDefinedFunction
    {
        public string Name { get { return Graph.Name; } }
        public Argument[] Arguments { get { return Graph.Arguments; } }
        public FunctionGraph Graph { get; private set; }

        public UserDefinedFunction(FunctionGraph graph)
        {
            Graph = graph;
        }
    }
}