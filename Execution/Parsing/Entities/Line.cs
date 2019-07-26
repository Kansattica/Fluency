namespace Fluency.Execution.Parsing.Entities
{
    /// <summary>
    /// Represents a raw line from the source file.
    /// </summary>
    public class Line
    {
        /// <summary>
        /// The one-indexed line number.
        /// </summary>
        /// <value></value>
        public int Number { get; private set; }

        /// <summary>
        /// The contents of the string.
        /// </summary>
        /// <value></value>
        public string Contents { get; private set; }

        private Line() { }

        /// <summary>
        /// Create a line. This is a factory function so you can give it to Linq's .Select and have it do the indexing for you.
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="number">The zero-based index. The number one higher than this will be stored.</param>
        /// <returns></returns>
        public static Line Create(string contents, int number)
        {
            return new Line()
            {
                Number = number + 1, //linq starts counting from zero
                Contents = contents
            };
        }

        /// <summary>
        /// Return lineNumber: Contents
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Number + ":" + Contents;
    }
}