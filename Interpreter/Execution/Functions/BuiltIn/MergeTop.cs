namespace Fluency.Interpreter.Execution.Functions.BuiltIn
{
    
    /// <summary>
    /// Pass everything from the top input to the top output, then everything from the bottom input to top output.
    /// </summary>
    class MergeTop : ITopIn, IBottomIn, ITopOut
    {
        public virtual string Name => nameof(MergeTop);

        public GetNext TopInput { protected get; set; }
        public GetNext BottomInput { protected get; set; }

        public virtual Value Top()
        {
            Value next;
            if (next = TopInput())
            {
                return next;
            }

            if (next = BottomInput())
            {
                return next;
            }

            return Value.Finished;
        }
    }

    /// <summary>
    /// Pass everything from the bottom input to the top output, then everything from the top input to top output.
    /// </summary>
    class MergeBottom : MergeTop
    {
        public override string Name => nameof(MergeBottom);

        public override Value Top()
        {
            Value next;
            if (next = BottomInput())
            {
                return next;
            }

            if (next = TopInput())
            {
                return next;
            }

            return Value.Finished;
        }

    }

}