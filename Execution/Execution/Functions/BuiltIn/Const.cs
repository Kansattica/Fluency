using Fluency.Execution.Exceptions;

namespace Fluency.Execution.Functions.BuiltIn
{
    /// <summary>
    /// Returns an infinite stream of its arguments.
    /// </summary>
    public class Const: ITopIn, ITopOut
    {

        public virtual string Name => nameof(Const);

        public GetNext TopInput { set {} }
            
        public Const(Value[] arguments)
        {
            if (arguments.Length == 0)
            {
                throw new ExecutionException("Const takes one or more values.");
            }

            this.arguments = arguments;
        }

        private readonly Value[] arguments;
        private int nextArg = 0;
        public Value Top()
        {
            return arguments[(nextArg++ % arguments.Length)];
        }
    }

}
