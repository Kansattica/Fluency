using System.Collections.Generic;
using System.Linq;
using Fluency.Common;
using Fluency.Execution.Exceptions;
using Fluency.Execution.Extensions;
using Fluency.Execution.Parsing.Entities;
using Fluency.Execution.Parsing.Entities.ArgumentTypes;
using Fluency.Execution.Parsing.Entities.FunctionGraph;

namespace Fluency.Execution.Functions
{
    public class UserDefinedFunction : IFunction, ITopIn, ITopOut, IBottomIn, IBottomOut
    {
        public string Name { get; private set; }
        public FunctionArg[] TopArguments { get; private set; }
        public FunctionArg[] BottomArguments { get; private set; }


        public GetNext TopInput { set => _topHead.TypedFunction.TopInput = WrapInput(value, TopArguments, _topRuntimeArgs); }
        public GetNext BottomInput { set { if (_bottomHead != null) { _bottomHead.TypedFunction.TopInput = WrapInput(value, BottomArguments, _bottomRuntimeArgs); } } }

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

        private Value[] _topRuntimeArgs;
        private Value[] _bottomRuntimeArgs;
        private bool shouldValidate = true;

        private GetNext WrapInput(GetNext thisinput, FunctionArg[] arguments, Value[] runtimeArgs)
        {
            if (arguments.Length == 0 || runtimeArgs.Length == 0 || arguments.Any(x => x.Type == FluencyType.Function && x.GetAs<string>() == "..."))
                return thisinput;

            if (runtimeArgs.Length > arguments.Length)
                throw new ExecutionException("Too many arguments passed to {0}. Expected {1}, got {2}.", Name, arguments.Length, runtimeArgs.Length);

            var argumentsValid = arguments.Zip(runtimeArgs, (a, v) => (a, v, valid: ValidateArgument(a, v))).Where(x => !x.valid)
                .Aggregate("",
                (acc, curr) => $"Invalid argument. Function {Name} expects an argument of type {curr.a.Type} and got {curr.v.ToString()}, which is of type {curr.v.Type}.\n" + acc);

            if (!string.IsNullOrWhiteSpace(argumentsValid))
                throw new ExecutionException(argumentsValid);

            int argIndex = 0;
            return () =>
            {
                Value v = thisinput();
                if (shouldValidate && !ValidateArgument(arguments[argIndex], v))
                    throw new ExecutionException("Function {0} tried to take value {1} (type {2}) from the pipeline, when it declares type {3}.",
                        Name, v, v.Type, arguments[argIndex].DeclaredType);
                argIndex++;

                if (argIndex >= arguments.Length)
                {
                    shouldValidate = false;
                }
                return v;
            };

        }

        private bool ValidateArgument(FunctionArg argument, Value value)
        {
            if (argument.DeclaredType == FluencyType.Any)
                return true;

            return argument.DeclaredType == value.Type;
        }

        public UserDefinedFunction(FunctionGraph graph, Value[] topArguments, Value[] bottomArguments, IFunctionResolver linker)
        {
            Name = graph.Name;
            TopArguments = graph.TopArguments;
            BottomArguments = graph.BottomArguments;
            _topRuntimeArgs = topArguments;
            _bottomRuntimeArgs = bottomArguments;
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

            _topHead = RecursiveExpand<ITopIn>(head.TopOut, seen, linker, 0);

            if (head.BottomOut != null)
            {
                _bottomHead = RecursiveExpand<ITopIn>(head.BottomOut, seen, linker, 1);
            }

            _topTailOut = _topTail?.Has<ITopOut>();
            _bottomTailOut = _bottomTail?.Has<ITopOut>();
        }

        private Value[] ArgumentsToValues(IEnumerable<Argument> args)
        {
            return args.Select(x => new Value(x)).ToArray();
        }

        private ExecutableNode<T> RecursiveExpand<T>(FunctionNode node, Dictionary<FunctionNode, ExecutableNode> seen, IFunctionResolver linker, int level) where T : IFunction
        {
            if (!seen.ContainsKey(node))
            {
                ExecutableNode<T> resolved = new ExecutableNode<T>();
                resolved.Tiebreaker = node.Tiebreaker;
                resolved.Function = linker.Resolve(node.Name, ArgumentsToValues(node.TopArguments), ArgumentsToValues(node.BottomArguments)).Is<T>();
                seen.Add(node, resolved);

                if (node.TopOut != null)
                {
                    var nextTop = RecursiveExpand<ITopIn>(node.TopOut, seen, linker, level);
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
                else
                {
                    // if we don't have any top outputs, we might be a tail
                    // if we're on level 0, we might be a top tail
                    if (level == 0)
                        _topTail = PickBetterNode(_topTail, resolved) as ExecutableNode<ITopIn>;

                    // if we're on level 1, we might be a bottom tail
                    else if (level == 1)
                        _bottomTail = PickBetterNode(_bottomTail, resolved) as ExecutableNode<ITopIn>;
                }

                if (node.BottomOut != null)
                {
                    // I expect the person below me to be able to take from the top
                    // because I'm handing them that from the bottom.
                    var nextBottom = RecursiveExpand<ITopIn>(node.BottomOut, seen, linker, level + 1);
                    resolved.BottomAfter = nextBottom;

                    nextBottom.TypedFunction.TopInput = resolved.Has<IBottomOut>().Bottom;
                }

            }

            return (ExecutableNode<T>)seen[node];
        }


        private ExecutableNode PickBetterNode(ExecutableNode a, ExecutableNode b)
        {
            if (a == null) { return b; }
            if (b == null) { return a; }

            return a.Tiebreaker > b.Tiebreaker ? a : b;
        }

    }
}