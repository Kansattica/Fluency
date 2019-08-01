using System;
using Fluency.Common;
using Fluency.Execution.Exceptions;
using Fluency.Execution.Extensions;

namespace Fluency.Execution.Functions.BuiltIn
{
    /// <summary>
    /// Reads a function f from the top pipeline and expands in place, replacing itself with f and passing its arguments to f.
    /// </summary>
    public class Expand : IFunction, ITopIn, ITopOut, IBottomIn, IBottomOut
    {
        private readonly Value[] arguments;
        private readonly IFunctionResolver linker;
        private Func<Value> wrapTop;
        private Func<Value> wrapBottom;

        public string Name { get; private set; } = nameof(Expand);

        public GetNext TopInput { private get; set; }
        public GetNext BottomInput { private get; set; }

        public Expand(Value[] arguments, IFunctionResolver linker)
        {
            this.arguments = arguments;
            this.linker = linker;
        }

        public bool EnsureValue()
        {
            if (wrapTop != null || wrapBottom != null) { return true; }

            Value v = TopInput();
            if (v.Done) { return false; }

            if (v.Type == FluencyType.Function)
            {
                IFunction stored = linker.Resolve(v.Get<string>(), arguments);

                if (stored is ITopIn)
                {
                    stored.Is<ITopIn>().TopInput = TopInput;
                }

                if (stored is ITopOut)
                {
                    wrapTop = stored.Is<ITopOut>().Top;
                }

                if (stored is IBottomIn)
                {
                    stored.Is<IBottomIn>().BottomInput = BottomInput;
                }

                if (stored is IBottomOut)
                {
                    wrapBottom = stored.Is<IBottomOut>().Bottom;
                }

                Name = stored.Name + " (Expanded)";

                return true;
            }
            else
            {
                throw new ExecutionException("Expand tried to read value {0} of type {1}, expected a function.", v.ToString(), v.Type);
            }

        }

        public Value Top()
        {
            if (EnsureValue())
            {
                return wrapTop();
            }
            else
            {
                return Value.Finished;
            }
        }

        public Value Bottom()
        {
            if (EnsureValue())
            {
                return wrapBottom();
            }
            else
            {
                return Value.Finished;
            }
        }
    }
}