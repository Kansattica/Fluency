using System;
using System.Collections.Generic;
using System.Linq;
using Fluency.Common;
using Fluency.Execution.Extensions;
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
        private Queue<Value> topArgumentBuffer = new Queue<Value>();
        private Queue<Value> bottomArgumentBuffer = new Queue<Value>();

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
                    topArgumentBuffer.Enqueue(tmp);
                    expandedFunction = makeNewFunction();
                    if (expandedFunction == null)
                        return Value.Finished;
                    inputsSet = false;
                }
            }

            EnsureInputsSet();

            Value v = expandedFunction.Top();
            if (v.Done)
            {
                topArgumentBuffer.Clear();
                expandedFunction = makeNewFunction();
                if (expandedFunction == null)
                    return Value.Finished;
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
                expandedFunction.TopInput = WrapInput(TopInput, topArgumentBuffer);

                expandedFunction.BottomInput = WrapInput(BottomInput, bottomArgumentBuffer);

                inputsSet = true;
            }
        }

        public Value Bottom()
        {
            if (expandedFunction == null)
            {
                expandedFunction = makeNewFunction();
                if (expandedFunction == null)
                    return Value.Finished;
            }

            EnsureInputsSet();

            Value v = expandedFunction.Bottom();
            if (v.Done)
            {
                expandedFunction = makeNewFunction();
                if (expandedFunction == null)
                    return Value.Finished;
                inputsSet = false;
                EnsureInputsSet();
                v = expandedFunction.Bottom();
            }
            return v;
        }

        private Value TryTakeBuffer(GetNext input, Queue<Value> buffer)
        {
            if (buffer.TryDequeue(out Value tmp))
            {
                return tmp;
            }
            else
            {
                return input();
            }
        }

        private GetNext WrapInput(GetNext input, Queue<Value> buffer)
        {
            if (!toAllowThrough.HasValue) { return () => TryTakeBuffer(input, buffer); }
            return () =>
            {
                if (toAllowThrough == 0)
                    return Value.Finished;
                else
                {
                    toAllowThrough--;
                    return TryTakeBuffer(input, buffer);
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

        private bool BufferArguments(int ensureBufferHas, Queue<Value> bufferInto, Value[] arguments, GetNext next)
        {
            bufferInto.EnqueueRange(arguments);

            while (bufferInto.Count < ensureBufferHas)
            {
                Value v = next();
                if (v.Done)
                {
                    bufferInto.Clear();
                    return false;
                }
                else
                {
                    bufferInto.Enqueue(v);
                }
            }

            return true;
        }

        public UserFunctionStub(FunctionGraph graph, Value[] topArguments, Value[] bottomArguments, IFunctionResolver linker)
        {
            Name = graph.Name;
            int? takeTop = ArgumentsToTake(graph.TopArguments);
            int? takeBottom = ArgumentsToTake(graph.BottomArguments);
            makeNewFunction = () =>
            {
                //                Console.WriteLine("expanding " + graph.Name);
                toAllowThrough = takeTop;
                if (takeTop.HasValue)
                {
                    if (!BufferArguments(takeTop.Value, topArgumentBuffer, topArguments, TopInput))
                        return null;
                }
                if (takeBottom.HasValue)
                {
                    if (!BufferArguments(takeBottom.Value, bottomArgumentBuffer, bottomArguments, BottomInput))
                        return null;
                }
                return new UserDefinedFunction(graph, topArguments, bottomArguments, linker);
            };
        }
    }
}