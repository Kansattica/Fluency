using System.Collections.Generic;
using System.Linq;
using Fluency.Interpreter.Parser.Entities.ArgumentTypes;
using Fluency.Interpreter.Parser.Exceptions;

namespace Fluency.Interpreter.Parser.Entities.FunctionGraph
{
    /// <summary>
    /// Represents a single user-defined function and all the functions that implement it.
    /// </summary>
    public class FunctionGraph
    {
        /// <summary>
        /// The root node of the function. This is the Def() function.
        /// </summary>
        /// <value></value>
        public FunctionNode Head { get; private set; }

        /// <summary>
        /// Name of the function this graph represents.
        /// </summary>
        /// <value></value>
        public string Name { get; private set; }

        /// <summary>
        /// The arguments this function has declared.
        /// </summary>
        /// <value></value>
        public Argument[] Arguments { get; private set; }

        /// <summary>
        /// Create a new function graph. This takes an IEnumerable of tokenized lines (which are themselves IEnumerables of tokens).
        /// </summary>
        /// <param name="tokens"></param>
        public FunctionGraph(IEnumerable<IEnumerable<FunctionToken>> tokens)
        {
            // for each line, we want to go through each function
            // for each function, we want to store it in the graph and remember where it is, because 
            // things on the line below might want to connect to it.

            IEnumerable<ProcessedFunction> prevLine = ProcessFirstLine(tokens.First());
            foreach (var line in tokens.Skip(1))
                prevLine = ProcessLine(line, prevLine);
        }

        private IEnumerable<ProcessedFunction> ProcessFirstLine(IEnumerable<FunctionToken> tokens)
        {
            FunctionNode lastAdded = null;
            bool first = true;
            List<ProcessedFunction> processed = new List<ProcessedFunction>();
            foreach (var token in tokens)
            {
                FunctionNode newNode = new FunctionNode(token);
                if (first)
                {
                    CheckDefinition(token);
                    Head = newNode;
                    lastAdded = Head;
                    first = false;
                }
                processed.Add(new ProcessedFunction { Node = newNode, Token = token });
                lastAdded.TopOut = newNode;
                lastAdded = newNode;
            }
            return processed;
        }

        private IEnumerable<ProcessedFunction> ProcessLine(IEnumerable<FunctionToken> tokens, IEnumerable<ProcessedFunction> prevLine)
        {
            List<ProcessedFunction> processed = new List<ProcessedFunction>();
            FunctionNode lastAdded = null;
            foreach (var token in tokens)
            {
                bool atLeastOneConnection = false;
                FunctionNode newNode = new FunctionNode(token);
                processed.Add(new ProcessedFunction { Node = newNode, Token = token });

                if (token.ConnectsUpBefore)
                {
                    //Find the one this connects to
                    var connected = prevLine.FirstOrDefault(x => x.Range.Contains(token.Range.Min));
                    if (connected == null)
                        throw new ParseException("Could not connect to an input function above.") { FunctionToken = token };
                    connected.Node.BottomOut = newNode;
                    newNode.TopIn = connected.Node;
                    atLeastOneConnection = true;
                }

                if (token.ConnectsUpAfter)
                {
                    //Find the one this connects to
                    var connected = prevLine.FirstOrDefault(x => x.Range.Contains(token.Range.Max));
                    if (connected == null)
                        throw new ParseException("Could not connect to an output function above.") { FunctionToken = token };
                    connected.Node.BottomIn = newNode;
                    newNode.TopOut = connected.Node;
                    atLeastOneConnection = true;
                }

                if (token.ConnectsBefore)
                {
                    if (lastAdded == null)
                        throw new ParseException(@"No function before this on the same level to call. Did you mean \.?") { FunctionToken = token };

                    lastAdded.TopOut = newNode;
                    newNode.TopIn = lastAdded;
                    atLeastOneConnection = true;
                }

                if (!atLeastOneConnection)
                    throw new ParseException(@"It doesn't look like this function connects to anything. Did you mean to connect it to a function above it?") { FunctionToken = token };

                lastAdded = newNode;
            }

            return processed;
        }

        private class ProcessedFunction
        {
            public FunctionToken Token;
            public FunctionNode Node;
            public Range Range { get { return Token.Range; } }
        }

        private const string example = "\"Def(FunctionName)\" or \"Def(FunctionName, int some, any arguments)\"";
        private void CheckDefinition(FunctionToken def)
        {
            if (def.Name != "Def")
                throw new ParseException("Function definitions must start with a call named Def, like this: " + example)
                { FunctionToken = def };

            if (def.Arguments.Length == 0)
                throw new ParseException("Function definitions must have at leat one argument- the name: " + example)
                { FunctionToken = def };

            foreach (Argument arg in def.Arguments)
            {
                FunctionArg argument = arg as FunctionArg;

                if (argument == null)
                    throw new ParseException("Incorrect formal parameter declaration. All parameters must have alphabetic names and no quotes.");

            }

        }

    }
}