using System.Collections.Generic;

namespace Fluency.Interpreter.Execution.Functions
{
    interface IFunction
    {
        string Name { get; }
    }

    interface ITopOut
    {
        Value Top();
    }

    interface IBottomOut
    {

        Value Bottom();
    }

    interface ITopIn
    {
        GetNext TopInput { set; }
    }

    interface IBottomIn
    {
        GetNext TopInput { set; }
    }

    delegate Value GetNext();
}