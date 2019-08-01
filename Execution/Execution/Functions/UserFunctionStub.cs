using System;
using System.Linq;
using Fluency.Common;
using Fluency.Execution.Parsing.Entities;
using Fluency.Execution.Parsing.Entities.FunctionGraph;

namespace Fluency.Execution.Functions
{
    /// <summary>
    /// Implements the behavior where user-defined functions "expand" the first time a value is requested.
    /// </summary>
    public class UserFunctionStub : IFunction, ITopIn, ITopOut, IBottomIn, IBottomOut
    {
        public string Name { get; private set; }

        public GetNext TopInput { private get; set; }
        public GetNext BottomInput { private get; set; }

        private Func<UserDefinedFunction> makeNewFunction;
        private UserDefinedFunction expandedFunction;

        // basically, when we would expand a new function, if the incoming value is a Done, we instead just pass that forward
        // but if it isn't, we have to pass that forward
        private Value buffer = null;

        private int? toAllowThrough = null;
        private bool inputsSet = false;
        public Value Top()
        {
            if (expandedFunction == null)
            {
                Value tmp = TopInput();
                if (tmp.Done)
                {
                    return tmp;
                }
                else
                {
                    buffer = tmp;
                    expandedFunction = makeNewFunction();
                }
            }

            EnsureInputsSet();

            Value v = expandedFunction.Top();
            if (v.Done)
            {
                expandedFunction = makeNewFunction();
                inputsSet = false;
                EnsureInputsSet();
                v = expandedFunction.Top();
            }
            return v;
        }

        private void EnsureInputsSet()
        {
            if (!inputsSet)
            {
                if (TopInput != null)
                    expandedFunction.TopInput = WrapTopInput(TopInput);

                if (BottomInput != null)
                    expandedFunction.BottomInput = BottomInput;
                inputsSet = true;
            }
        }

        public Value Bottom()
        {
            if (expandedFunction == null)
            {
                expandedFunction = makeNewFunction();
            }

            EnsureInputsSet();

            Value v = expandedFunction.Bottom();
            if (v.Done)
            {
                expandedFunction = makeNewFunction();
                inputsSet = false;
                EnsureInputsSet();
                v = expandedFunction.Bottom();
            }
            return v;
        }

        private Value GetAndClearBuffer(GetNext input)
        {
            if (buffer != null)
            {
                Value tmp = buffer;
                buffer = null;
                return tmp;
            }
            else
            {
                return input();
            }
        }

        private GetNext WrapTopInput(GetNext topInput)
        {
            if (!toAllowThrough.HasValue) { return () => GetAndClearBuffer(topInput); }
            return () =>
            {
                if (toAllowThrough == 0)
                    return Value.Finished;
                else
                {
                    toAllowThrough--;
                    return GetAndClearBuffer(topInput);
                }
            };
        }

        private static readonly Value ellipses = new Value("...", FluencyType.Function);
        private int? ArgumentsToTake(Argument[] arguments)
        {
            if (arguments.Length == 0 || arguments.Any(x => x.Equals(ellipses)))
                return null;
            else
                return arguments.Length;
        }

        public UserFunctionStub(FunctionGraph graph, Value[] arguments, IFunctionResolver linker)
        {
            Name = graph.Name;
            int? totake = ArgumentsToTake(graph.Arguments);
            makeNewFunction = (() =>
            {
                //                Console.WriteLine("expanding " + graph.Name);
                toAllowThrough = totake;
                return new UserDefinedFunction(graph, arguments, linker);
            });
        }
    }
}