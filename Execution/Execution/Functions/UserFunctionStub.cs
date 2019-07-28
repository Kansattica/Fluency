using System;
using System.Linq;
using Fluency.Common;
using Fluency.Execution.Parsing.Entities;
using Fluency.Execution.Parsing.Entities.FunctionGraph;

namespace Fluency.Execution.Functions
{
    /// <summary>
    /// User-defined functions should "expand" on first use so you can do recursion without overflowing the stack.
    /// </summary>
    public class UserFunctionStub : IFunction, ITopIn, ITopOut, IBottomIn, IBottomOut
    {
        public string Name { get; private set; }


        // for now, take n arguments from the top
        // let functions specify that they want to take arguments on the bottom, too, later
        public GetNext TopInput { private get; set; }
        public GetNext BottomInput { private get; set; }

        private Func<UserDefinedFunction> makeNewFunction;
        private UserDefinedFunction expandedFunction;

        private int? toAllowThrough = null;
        private bool topset = false;
        public Value Top()
        {
            if (expandedFunction == null)
            {
                expandedFunction = makeNewFunction();
            }

            if (!topset)
            {
                expandedFunction.TopInput = WrapTopInput(TopInput);
                topset = true;
            }

            Value v = expandedFunction.Top();
            if (v.Done)
            {
                bottomset = false;
                expandedFunction = makeNewFunction();
                expandedFunction.TopInput = WrapTopInput(TopInput);
                v = expandedFunction.Top();
            }
            return v;
        }

        private bool bottomset = false;
        public Value Bottom()
        {
            if (expandedFunction == null)
            {
                expandedFunction = makeNewFunction();
            }

            if (!bottomset)
            {
                expandedFunction.BottomInput = BottomInput;
                bottomset = true;
            }

            Value v = expandedFunction.Bottom();
            if (v.Done)
            {
                topset = false;
                expandedFunction = makeNewFunction();
                expandedFunction.BottomInput = BottomInput;
                v = expandedFunction.Bottom();
            }
            return v;
        }

        private GetNext WrapTopInput(GetNext topInput)
        {
            // if we're allowing everything, no need to wrap
            if (!toAllowThrough.HasValue) { return topInput; }
            return () =>
            {
                if (toAllowThrough == 0)
                    return Value.Finished;
                else
                {
                    toAllowThrough--;
                    return topInput();
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
            makeNewFunction = (() => {
//                Console.WriteLine("expanding " + graph.Name);
                toAllowThrough = totake;
                return new UserDefinedFunction(graph, arguments, linker);
            });
        }
    }
}