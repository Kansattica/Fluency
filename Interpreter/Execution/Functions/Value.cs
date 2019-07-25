namespace Fluency.Interpreter.Execution.Functions
{
    class Value
    {
        /// <summary>
        /// Returned when the called function has no more work to do.
        /// </summary>
        /// <value></value>
        public bool Done { get; private set; }

        public ValueTypes Type { get; private set; }
    }
}