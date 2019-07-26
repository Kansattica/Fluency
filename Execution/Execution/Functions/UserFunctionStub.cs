namespace Fluency.Execution.Functions
{
    public class UserFunctionStub : IFunction, ITopIn, ITopOut, IBottomIn, IBottomOut
    {
        public string Name => throw new System.NotImplementedException();

        public GetNext TopInput { set => throw new System.NotImplementedException(); }
        public GetNext BottomInput { set => throw new System.NotImplementedException(); }

        public Value Bottom()
        {
            throw new System.NotImplementedException();
        }

        public Value Top()
        {
            throw new System.NotImplementedException();
        }
    }
}