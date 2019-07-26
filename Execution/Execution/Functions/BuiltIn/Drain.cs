namespace Fluency.Execution.Functions.BuiltIn
{
    /// <summary>
    /// Takes all values from both inputs and returns nothing.
    /// </summary>
    public class Drain: ITopIn, IBottomIn, ITopOut, IBottomOut
    {
        public virtual string Name => nameof(Drain);

        public GetNext TopInput { private get; set; }
        public GetNext BottomInput { private get; set; }

        public Value Top() => DrainBoth();
        public Value Bottom() => DrainBoth();

        private Value DrainBoth()
        {
            while (TopInput()) {}
            while (BottomInput()) {}
            return Value.Finished;
        }

    }
}