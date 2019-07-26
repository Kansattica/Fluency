using System;
using Fluency.Execution.Exceptions;

namespace Fluency.Execution.Functions
{

    public class ExecutableNode
    {
        public IFunction Function { get; set; }

        public ExecutableNode<ITopIn> TopAfter { get; set; }
        public ExecutableNode<ITopIn> BottomAfter { get; set; }

        public int Tiebreaker { get; set; }

    }

    public class ExecutableNode<T> : ExecutableNode where T : IFunction
    {
        public T TypedFunction { get { return (T)Function; } set { Function = value; } }

        public OutT Has<OutT>() where OutT : IFunction
        {
            try
            {
                return (OutT)Function;
            }
            catch (InvalidCastException ex)
            {
                throw new ExecutionException("Tried to use function {0} as a {1}, which it isn't.", ex, Function.Name, typeof(OutT).Name);
            }
        }

    }
}