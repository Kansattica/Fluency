using System;
using Fluency.Execution.Exceptions;

namespace Fluency.Execution.Functions
{

    public class ExecutableNode
    {
        public IFunction Function { get; set; }

        public ExecutableNode<ITopIn> TopAfter { get; set; }
        public ExecutableNode<ITopIn> BottomAfter { get; set; }

    }

    public class ExecutableNode<T> : ExecutableNode where T : IFunction
    {
        public T TypedFunction { get { return (T)Function; } set { Function = value; } }

        public ExecutableNode<OutT> Is<OutT>() where OutT : IFunction
        {

            try
            {
                //hahahaha wow
                return (ExecutableNode<OutT>)(ExecutableNode)this;
            }
            catch (InvalidCastException ex)
            {
                throw new ExecutionException("Tried to use function {0} as a {1}, which it isn't.{2}", ex, Function.Name, typeof(T).Name);
            }
        }
    }

}