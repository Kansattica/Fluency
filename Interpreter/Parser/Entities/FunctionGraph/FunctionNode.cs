
namespace Fluency.Interpreter.Parser.Entities.FunctionGraph
{
    public class FunctionNode
    {
        public string Name;
        public string[] Arguments;

        public FunctionNode TopIn;
        public FunctionNode BottomIn;
        public FunctionNode TopOut;
        public FunctionNode BottomOut;

        public int Id { get; private set; } = _nextId++;
        private static int _nextId = 1;
        public FunctionNode(FunctionToken tok)
        {
            Name = tok.Name;
            Arguments = tok.Arguments;
        }

        public override string ToString() => $"{Name}({string.Join(", ", Arguments)})";
    }
}