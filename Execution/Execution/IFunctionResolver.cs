using Fluency.Execution.Functions;

namespace Fluency.Execution
{
    /// <summary>
    /// Something that knows how to make a Fluency function, given a name and arguments.
    /// </summary>
    public interface IFunctionResolver
    {
        IFunction Resolve(string name, Value[] arguments);
    }
}