using System;
using System.Linq;
using Fluency.Common;
using Fluency.Execution.Exceptions;

namespace Fluency.Execution.Functions.BuiltIn
{

    /// <summary>
    /// Wrap a C# function that takes two arguments and returns one value as a Fluency function that:
    /// - If no arguments given, read something from the top and the bottom and do the operation on them.
    /// - If one argument given, read something from the top and do the operation with that and the argument.
    /// This is how Add, Mult, Equals, And, Or, AddDouble, and Concat are implemented.
    /// </summary>
    public class WrapBinary<TRealTop, TRealBottom, TRealOut> : ITopIn, IBottomIn, ITopOut
    {
        private readonly Func<TRealTop, TRealBottom, TRealOut> function;
        private readonly FluencyType type;
        private readonly Value stored;
        private readonly bool topStored;

        public string Name { get; private set; }

        public GetNext TopInput { private get; set; }
        public GetNext BottomInput { private get; set; }

        public WrapBinary(Func<TRealTop, TRealBottom, TRealOut> function, FluencyType argType, FluencyType returnType, string name, Value[] topArguments, Value[] bottomArguments = null)
        {
            this.function = function;
            this.type = returnType;
            Name = name;

            bottomArguments = bottomArguments ?? new Value[0];
            if (topArguments.Length > 0 && bottomArguments.Length > 0)
                throw new ExecutionException("You must pass either one top argument, one bottom argument, or neither to {0}, not both.", Name);

            if (ValidateArgs(topArguments, argType, name))
            {
                stored = topArguments[0];
                topStored = true;
            }

            if (ValidateArgs(bottomArguments, argType, name))
            {
                stored = bottomArguments[0];
                topStored = false;
            }
        }

        private bool ValidateArgs(Value[] arguments, FluencyType argType, string name)
        {
            if (arguments.Length == 1)
            {
                if (argType != FluencyType.Any && arguments[0].Type != argType)
                    throw new ExecutionException("Function {0} takes a {1}. You supplied a {2}: {3}", name, argType, stored.Type, stored.ToString());
                else
                    return true;

            }
            else if (arguments.Length > 1)
            {
                throw new ExecutionException("Function {0} takes either zero or one top parameters. You supplied {1}: {2}", name, arguments.Length, string.Join(", ", arguments.Select(x => x.ToString())));
            }
            // if no arguments, don't store this one
            return false;
        }

        private Value ReadOrGetStored(GetNext input, bool isTop)
        {
            if (isTop == topStored && stored != null)
                return stored;
            else if (input != null)
                return input();
            else
                throw new ExecutionException("{0} tried to read from a {1} input that doesn't exist. Are you missing a pipeline?", Name, isTop ? "top" : "bottom");
        }

        public Value Top()
        {
            Value top = ReadOrGetStored(TopInput, true);
            Value bottom = ReadOrGetStored(BottomInput, false);

            if (top && bottom)
                return new Value(function(top.Get<TRealTop>(type), bottom.Get<TRealBottom>(type)), type);

            return Value.Finished;
        }
    }
}