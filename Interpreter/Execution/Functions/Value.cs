using Fluency.Interpreter.Common;
using Fluency.Interpreter.Execution.Exceptions;
using Fluency.Interpreter.Parsing.Entities;

namespace Fluency.Interpreter.Execution.Functions
{
    /// <summary>
    /// Represents a value that Fluency functions operate on.
    /// </summary>
    public class Value
    {
        /// <summary>
        /// Returned when the called function has no more work to do.
        /// </summary>
        /// <value></value>
        public bool Done { get; private set; }

        /// <summary>
        /// The type of this value.
        /// </summary>
        /// <value></value>
        public FluencyType Type { get; private set; }

        private object _value;

        /// <summary>
        /// Construct a value from a parsed function argument.
        /// </summary>
        /// <param name="arg"></param>
        public Value(Argument arg)
        {
            Type = arg.Type;
            _value = arg.GetAs<object>();
            Done = false;
        }

        /// <summary>
        /// Get the value inside the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>(FluencyType? expected = null, string failMessage = null)
        {
            if (!(_value is T))
            {
                throw new ExecutionException("{3}Expected a value of type {0}, got {1}{2}", typeof(T).Name, _value.GetType().Name,
                    (expected.HasValue ? ($" (Expected runtime type: {expected.Value}, was {Type}") : ""),
                    (failMessage != null ? failMessage + "\n" : ""));
            }

            return (T)_value;

        }

        /// <summary>
        /// Construct a value from a C# value and a type.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        public Value(object value, FluencyType type)
        {
            _value = value;
            Type = type;
            Done = false;
        }

        private Value() { }

        /// <summary>
        /// The Done value converts to false, all others to true. This is so you can do something like (while (NextValue()) { doSomething(); }
        /// </summary>
        public static implicit operator bool(Value v) => !v.Done;
        private static Value _done = new Value { Done = true, Type = FluencyType.Any };

        /// <summary>
        /// The "done" Value that indicates there's no more work.
        /// </summary>
        public static Value Finished => _done;
    }
}