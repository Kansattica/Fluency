namespace Fluency.Execution.Functions
{
    public class ExecutableNode<T> where T : IFunction
    {
        public T Function { get; set; }

        public ExecutableNode<ITopOut> TopBefore { get; set; }
        public ExecutableNode<ITopIn> TopAfter { get; set; }
        public ExecutableNode<IBottomOut> BottomBefore { get; set; }
        public ExecutableNode<IBottomIn> BottomAfter { get; set; }
    }
}