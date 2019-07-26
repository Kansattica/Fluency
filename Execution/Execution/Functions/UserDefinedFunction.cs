using System.Collections.Generic;
using System.Linq;
using Fluency.Execution.Extensions;
using Fluency.Execution.Parsing.Entities;
using Fluency.Execution.Parsing.Entities.FunctionGraph;

namespace Fluency.Execution.Functions
{
    public class UserDefinedFunction : IFunction, ITopIn, ITopOut, IBottomIn, IBottomOut
    {
        public string Name { get; private set; }
        public Argument[] Arguments { get; private set; }


        public GetNext TopInput { set => _topHead.Function.TopInput = value; }
        public GetNext BottomInput { set => _bottomHead.Function.TopInput = value; }

        private ExecutableNode<ITopIn> _topHead;
        private ExecutableNode<ITopIn> _bottomHead;
        private ExecutableNode<ITopOut> _topTail;
        private ExecutableNode<ITopOut> _bottomTail;

        public Value Top()
        {
            return _topTail.TypedFunction.Top();
        }

        public Value Bottom()
        {
            return _bottomTail.TypedFunction.Top();
        }

        private ExecutableNode<T> RecursiveExpand<T>(FunctionNode node, Dictionary<FunctionNode, ExecutableNode> seen, IFunctionResolver linker) where T : IFunction
        {
            if (!seen.ContainsKey(node))
            {
                ExecutableNode<T> resolved = new ExecutableNode<T>();
                resolved.Function = linker.Resolve(node.Name, node.Arguments.Select(x => new Value(x)).ToArray()).Is<T>();
                seen.Add(node, resolved);

                if (node.TopOut != null)
                {
                    var nextTop = RecursiveExpand<ITopIn>(node.TopOut, seen, linker);
                    resolved.TopAfter = nextTop;

                    ExecutableNode<ITopOut> topOut = resolved.Is<ITopOut>();
                    nextTop.TypedFunction.TopInput = topOut.TypedFunction.Top;
                    nextTop.TopBefore = topOut;
                }

                if (node.BottomOut != null)
                {
                    // I expect the person below me to be able to take from the top
                    // because I'm handing them that from the bottom.
                    var nextBottom = RecursiveExpand<ITopIn>(node.BottomOut, seen, linker);
                    resolved.BottomAfter = nextBottom;

                    ExecutableNode<IBottomOut> topOut = resolved.Is<IBottomOut>();
                    nextBottom.TypedFunction.TopInput = topOut.TypedFunction.Bottom;
                    nextBottom.TopBefore = topOut;
                }
            }

            return (ExecutableNode<T>)seen[node];
        }


        private void Expand(FunctionNode head, IFunctionResolver linker)
        {
            //Basically, we want to walk the FunctionGraph and turn it into an ExecutableGraph of IFunctions.

            var seen = new Dictionary<FunctionNode, ExecutableNode>();

            if (head.TopOut != null)
            {
                _topHead = RecursiveExpand<ITopIn>(head.TopOut, seen, linker);
            }

            if (head.BottomOut != null)
            {
                _bottomHead = RecursiveExpand<ITopIn>(head.BottomOut, seen, linker);
            }

            // Find the tail functions. Top is easy- follow it until there's no more head.

            if (_topHead != null)
            {
                ExecutableNode<ITopIn> next = _topHead;
                if (next.TopAfter != null)
                {
                    next = next.Is<ITopOut>().Top
                }
            }

        }

        public UserDefinedFunction(FunctionGraph graph, Value[] arguments, IFunctionResolver linker)
        {
            Name = graph.Name;
            Arguments = graph.Arguments;
            Expand(graph.Head, linker);
        }
    }
}