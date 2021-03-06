using System;
using System.Linq;
using Fluency.Common;
using Fluency.Execution.Exceptions;

namespace Fluency.Execution.Functions.BuiltIn
{

    /// <summary>
    /// Wrap a C# function that takes one argument and returns one value as a Fluency function.
    /// - This is how Sqrt, Floor, Not, and ParseInt are implemented.
    /// </summary>
    public class WrapUnary<TRealIn, TRealOut> : ITopIn, ITopOut
    {
        private readonly Func<TRealIn, TRealOut> function;
        private readonly FluencyType type;

        public string Name { get; private set; }

        public GetNext TopInput { private get; set; }

        public WrapUnary(Func<TRealIn, TRealOut> function, FluencyType returnType, string name)
        {
            this.function = function;
            this.type = returnType;
            Name = name;
        }

        public Value Top()
        {
            Value next;
            if (next = TopInput())
                return new Value(function(next.Get<TRealIn>(type)), type);

            return Value.Finished;
        }
    }
}