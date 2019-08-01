using System.Collections.Generic;
using System.Linq;
using Fluency.Common;
using Fluency.Execution.Exceptions;
using Fluency.Execution.Extensions;
using Fluency.Execution.Parsing.Entities.ArgumentTypes;
using Fluency.Execution.Parsing.Entities.FunctionGraph;

namespace Fluency.Execution.Functions
{
    public class UserDefinedFunction : IFunction, ITopIn, ITopOut, IBottomIn, IBottomOut
    {
        public string Name { get; private set; }
        public FunctionArg[] Arguments { get; private set; }


        public GetNext TopInput { set => _topHead.TypedFunction.TopInput = WrapInput(value); }
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

        //idea for runtime arguments:
        //if you got arguments in the constructor, store them, for the first [number of arguments given], do the function on them instead
        // probably hook into TopInput for this
        // also validate the arguments in the ctor, make sure the types match up
        // we should also validate incoming args too, of course
        private Value[] _runtimeArgs;
        private GetNext WrapInput(GetNext topinput)
        {
            if (Arguments.Length == 0 || _runtimeArgs.Length == 0 || Arguments.Any(x => x.Type == FluencyType.Function && x.GetAs<string>() == "..."))
                return topinput;
            
            if (_runtimeArgs.Length > Arguments.Length)
                throw new ExecutionException("Too many arguments passed to {0}. Expected {1}, got {2}.", Name, Arguments.Length, _runtimeArgs.Length);
            
            var argumentsValid = Arguments.Zip(_runtimeArgs, (a, v) => (a, v, valid: ValidateArgument(a, v))).Where(x => !x.valid)
                .Aggregate("", 
                (acc, curr) => $"Invalid argument. Function {Name} expects an argument of type {curr.a.Type} and got {curr.v.ToString()}, which is of type {curr.v.Type}.\n" + acc);

            if (!string.IsNullOrWhiteSpace(argumentsValid))
                throw new ExecutionException(argumentsValid);

            int argIndex = 0;
            return () =>
            {
                if (argIndex < _runtimeArgs.Length)
                    return _runtimeArgs[argIndex++];
                else if (argIndex == Arguments.Length)
                    return Value.Finished;
                else
                {
                    Value v = topinput();
                    if (!ValidateArgument(Arguments[argIndex++], v))
                        throw new ExecutionException("Function {0} tried to take value {1} (type {2}) from the pipeline, when it declares type {3}.", 
                            Name, v, v.Type, Arguments[argIndex].DeclaredType);

                    return v;
                }
            };

        }

        private bool ValidateArgument(FunctionArg argument, Value value)
        {
            if (argument.DeclaredType == FluencyType.Any)
                return true;
            
            return argument.DeclaredType == value.Type;
        }

        public UserDefinedFunction(FunctionGraph graph, Value[] arguments, IFunctionResolver linker)
        {
            Name = graph.Name;
            Arguments = graph.Arguments;
            _runtimeArgs = arguments;
            Expand(graph.Head, linker);
        }

        private void Expand(FunctionNode head, IFunctionResolver linker)
        {
            //Basically, we want to walk the FunctionGraph and turn it into an ExecutableGraph of IFunctions.

            var seen = new Dictionary<FunctionNode, ExecutableNode>();

            if (head.TopOut == null)
            {
                head.TopOut = new FunctionNode("I", head.Tiebreaker + 4) { TopIn = head };
            }

            _topHead = RecursiveExpand<ITopIn>(head.TopOut, seen, linker);

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