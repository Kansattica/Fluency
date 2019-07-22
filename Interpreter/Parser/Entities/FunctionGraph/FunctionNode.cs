
namespace Fluency.Interpreter.Parser.Entities.FunctionGraph
{
    internal class FunctionNode
    {
        public string Name;
        public string[] Arguments;

        FunctionNode TopIn;
        FunctionNode BottomIn;
        FunctionNode TopOut;
        FunctionNode BottomOut;


    }
}