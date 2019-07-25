using System.Collections.Generic;

namespace Fluency.Interpreter.Execution.Functions
{

    /// <summary>
    /// A Fluency function.
    /// </summary>
    public interface IFunction
    {
        /// <summary>
        /// The function's name.
        /// </summary>
        string Name { get; }
    }


    /// <summary>
    /// A Fluency function that writes to the top output.
    /// </summary>
    public interface ITopOut : IFunction
    {

        /// <summary>
        /// Called when the function after this one on top wants a value.
        /// </summary>
        Value Top();
    }

    /// <summary>
    /// A Fluency function that writes to the bottom output.
    /// </summary>
    public interface IBottomOut : IFunction
    {
        /// <summary>
        /// Called when the function after this one on the bottom wants a value.
        /// </summary>
        Value Bottom();
    }

    /// <summary>
    /// A Fluency function that reads from the top input.
    /// </summary>
    public interface ITopIn : IFunction
    {
        /// <summary>
        /// A function to call when you want the next value from the top input.
        /// </summary>
        GetNext TopInput { set; }
    }

    /// <summary>
    /// A Fluency function that reads from the bottom input.
    /// </summary>
    public interface IBottomIn : IFunction
    {
        /// <summary>
        /// A function to call when you want the next value from the bottom input.
        /// </summary>
        GetNext BottomInput { set; }
    }

    /// <summary>
    /// A function that, when called, returns the next element in the corresponding pipeline.
    /// </summary>
    public delegate Value GetNext();
}