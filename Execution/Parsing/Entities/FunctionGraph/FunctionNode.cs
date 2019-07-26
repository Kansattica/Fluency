
using System.Linq;

namespace Fluency.Execution.Parsing.Entities.FunctionGraph
{
    /// <summary>
    /// Represents a single function in a pipeline.
    /// </summary>
    public class FunctionNode
    {
        /// <summary>
        /// The function's name.
        /// </summary>
        public string Name;

        /// <summary>
        /// The function's declared arguments.
        /// </summary>
        public Argument[] Arguments;

        /// <summary>
        /// The function that this reads from on top ("top" can be above or immediately before on the same level).
        /// </summary>
        public FunctionNode TopIn;

        /// <summary>
        /// The function that feeds this one from the bottom.
        /// </summary>
        public FunctionNode BottomIn;

        /// <summary>
        /// The function that this outputs to on top ("top" can be above or immediately before on the same level).
        /// </summary>
        public FunctionNode TopOut;

        /// <summary>
        /// The function that this outputs to on the bottom.
        /// </summary>
        public FunctionNode BottomOut;

        /// <summary>
        /// A unique integer ID for this function. Two function calls with the same name have different IDs.
        /// </summary>
        /// <value></value>
        public int Id { get; private set; } = _nextId++;
        private static int _nextId = 1;

        /// <summary>
        /// Make a function node from a parsed function token.
        /// </summary>
        /// <param name="tok"></param>
        public FunctionNode(FunctionToken tok)
        {
            Name = tok.Name;
            Arguments = tok.Arguments;
        }

        /// <summary>
        /// Return a string that looks like Name(arguments, like this)
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{Name}({Arguments.Stringify()})";
    }
}