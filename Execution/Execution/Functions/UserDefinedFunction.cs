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


        public GetNext TopInput { set => _topHead.TypedFunction.TopInput = value; }
        public GetNext BottomInput { set => _bottomHead.TypedFunction.TopInput = value; }

        private ExecutableNode<ITopIn> _topHead;
        private ExecutableNode<ITopIn> _bottomHead;

        //These two should be TopOut, since they are for whoever comes after this to read from
        //However, C# doesn't let you turn a Something<A> into a Something<B>, even if A is a B and vice versa.
        private ExecutableNode<ITopIn> _topTail;
        private ExecutableNode<ITopIn> _bottomTail;

        private ITopOut _topTailOut;
        private ITopOut _bottomTailOut;
        public Value Top()
        {
            return _topTailOut.Top();
        }

        public Value Bottom()
        {
            return _bottomTailOut.Top();
        }

        public UserDefinedFunction(FunctionGraph graph, Value[] arguments, IFunctionResolver linker)
        {
            Name = graph.Name;
            Arguments = graph.Arguments;
            Expand(graph.Head, linker);
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

            FindHeadTail(_topHead);
            // it's possible that the top and bottom routes never meet
            _bottomTail = PickBetterNode(FindBottoms(_topHead), FindBottoms(_bottomHead));

            _topTailOut = _topTail?.Has<ITopOut>();
            _bottomTailOut = _bottomTail?.Has<ITopOut>();
        }

        private ExecutableNode<T> RecursiveExpand<T>(FunctionNode node, Dictionary<FunctionNode, ExecutableNode> seen, IFunctionResolver linker) where T : IFunction
        {
            if (!seen.ContainsKey(node))
            {
                ExecutableNode<T> resolved = new ExecutableNode<T>();
                resolved.Tiebreaker = node.Tiebreaker;
                resolved.Function = linker.Resolve(node.Name, node.Arguments.Select(x => new Value(x)).ToArray()).Is<T>();
                seen.Add(node, resolved);

                if (node.TopOut != null)
                {
                    var nextTop = RecursiveExpand<ITopIn>(node.TopOut, seen, linker);
                    resolved.TopAfter = nextTop;

                    if (node.TopOut.BottomIn == node) //If I'm giving to them from below.
                    {
                        nextTop.Has<IBottomIn>().BottomInput = resolved.Has<ITopOut>().Top;
                    }
                    else
                    {
                        nextTop.TypedFunction.TopInput = resolved.Has<ITopOut>().Top;
                    }
                }

                if (node.BottomOut != null)
                {
                    // I expect the person below me to be able to take from the top
                    // because I'm handing them that from the bottom.
                    var nextBottom = RecursiveExpand<ITopIn>(node.BottomOut, seen, linker);
                    resolved.BottomAfter = nextBottom;

                    nextBottom.TypedFunction.TopInput = resolved.Has<IBottomOut>().Bottom;
                }

                //maybe detect here if you're a tail?
            }


            return (ExecutableNode<T>)seen[node];
        }


        private void FindHeadTail(ExecutableNode<ITopIn> thisNode)
        {
            // Top is easy- follow it until there's no more head.
            var next = thisNode.TopAfter;

            if (next == null)
            {
                //you're the last one
                _topTail = thisNode;
                return;
            }

            FindHeadTail(next);
        }

        private ExecutableNode<ITopIn> FindBottoms(ExecutableNode<ITopIn> thisNode)
        {
            // We want to find all the non-_topTail tail nodes we can from here and return the best one

            if (thisNode == null) { return null; }


            ExecutableNode<ITopIn> nextTop = thisNode.TopAfter, nextBottom = thisNode.BottomAfter;

            if (nextTop == null && nextBottom == null)
            {
                // We found a tail node!
                if (object.ReferenceEquals(thisNode, _topTail))
                {
                    return null; // we found the top tail
                }
                else
                {
                    return thisNode;
                }
            }
            else
            {
                return PickBetterNode(FindBottoms(nextTop), FindBottoms(nextBottom));
            }
        }

        private ExecutableNode<T> PickBetterNode<T>(ExecutableNode<T> a, ExecutableNode<T> b) where T : IFunction
        {
            if (a == null) { return b; }
            if (b == null) { return a; }

            return a.Tiebreaker > b.Tiebreaker ? a : b;
        }

    }
}