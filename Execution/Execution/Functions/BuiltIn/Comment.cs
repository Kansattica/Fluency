namespace Fluency.Execution.Functions.BuiltIn
{
    
    /// <summary>
    /// A Fluency source code comment. Passes input to output.
    /// </summary>
    public class Comment : ITopIn, ITopOut
    {

        public virtual string Name => nameof(Comment);

        public GetNext TopInput { private get; set; }

        public Value Top() => TopInput();
    }

    /// <summary>
    /// A Fluency source code comment. Passes input to output. Same as <see cref="Comment"/>
    /// </summary>
    public class Com : Comment
    {
        public override string Name => nameof(Com);
    }

    /// <summary>
    /// The identity function. Passes input to output. Same as <see cref="Comment"/>
    /// </summary>
    public class I : Comment
    {
        public override string Name => nameof(I);
    }
}