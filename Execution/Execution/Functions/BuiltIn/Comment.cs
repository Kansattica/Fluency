namespace Fluency.Execution.Functions.BuiltIn
{
    
    /// <summary>
    /// A Fluency source code comment. Passes input to output.
    /// </summary>
    public class Com : ITopIn, ITopOut
    {

        public virtual string Name => nameof(Com);

        public GetNext TopInput { private get; set; }

        public Value Top() => TopInput();
    }

    /// <summary>
    /// A Fluency source code comment. Passes input to output. Same as <see cref="Com"/>
    /// </summary>
    public class Comment : Com
    {
        public override string Name => nameof(Comment);
    }
}