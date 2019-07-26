namespace Fluency.Execution.Functions.BuiltIn
{
    
    /// <summary>
    /// Pass everything from the top input to the top output, then everything from the bottom input to top output.
    /// </summary>
    public class MergeTop : ITopIn, IBottomIn, ITopOut
    {
        public virtual string Name => nameof(MergeTop);

        public GetNext TopInput { protected get; set; }
        public GetNext BottomInput { protected get; set; }

        protected bool topClosed = false;
        protected bool bottomClosed = false;
        public virtual Value Top()
        {
            Value next;
            if (!topClosed && (next = TopInput()))
                return next;
            else
                topClosed = true;

            if (!bottomClosed && (next = BottomInput()))
                return next;
            else
                bottomClosed = true;

            return Value.Finished;
        }
    }

    /// <summary>
    /// Pass everything from the bottom input to the top output, then everything from the top input to top output.
    /// </summary>
    public class MergeBottom : MergeTop
    {
        public override string Name => nameof(MergeBottom);

        public override Value Top()
        {
            Value next;

            if (!bottomClosed && (next = BottomInput()))
                return next;
            else
                bottomClosed = true;

            if (!topClosed && (next = TopInput()))
                return next;
            else
                topClosed = true;

            return Value.Finished;
        }

    }

}