using System.Collections.Generic;
using System.Linq;
using Fluency.Interpreter.Parser.Exceptions;

namespace Fluency.Interpreter.Parser.Entities.FunctionGraph
{
    public class FunctionGraph
    {
        public FunctionNode Head { get; private set; }
        public string Name { get { return Head.Arguments[0]; } }
        public string[] Arguments { get { return Head.Arguments; } }

        public FunctionGraph(IEnumerable<IEnumerable<FunctionToken>> tokens)
        {
            var definition = tokens.First().First();

            CheckDefinition(definition);

            Head = new FunctionNode(definition);

            // for each line, we want to go through each function
            // for each function, we want to store it in the graph and remember where it is, because 
            // things on the line below might want to connect to it.

            IEnumerable<ProcessedFunction> prevLine = null;
            foreach (var line in tokens)
            {
                prevLine = ProcessLine(line, prevLine);
            }
        }

        private IEnumerable<ProcessedFunction> ProcessLine(IEnumerable<FunctionToken> tokens, IEnumerable<ProcessedFunction> prevLine)
        {
            var processed = new List<ProcessedFunction>();

            FunctionNode lastAdded = Head;
            foreach (var token in tokens)
            {
                FunctionNode newNode = new FunctionNode(token);
                processed.Add(new ProcessedFunction { Node = newNode, Token = token });
                if (prevLine == null) //if this is the first row
                {
                    if (newNode.Name == "Def") { continue; } //don't double add the definition
                    lastAdded.TopOut = newNode;
                    lastAdded = newNode;
                }
                else
                {
                    if (token.ConnectsUpBefore)
                    {
                        //Find the one this connects to
                        var connected = prevLine.First(x => x.Range.Contains(token.Range.Min));
                        connected.Node.BottomOut = newNode;
                        newNode.TopIn = connected.Node;
                    }

                    if (token.ConnectsUpAfter)
                    {
                        //Find the one this connects to
                        var connected = prevLine.First(x => x.Range.Contains(token.Range.Max));
                        connected.Node.BottomIn = newNode;
                        newNode.TopOut = connected.Node;
                    }
                }
            }

            return processed;
        }

        private class ProcessedFunction
        {
            public FunctionToken Token;
            public FunctionNode Node;
            public Range Range { get { return Token.Range; } }
        }

        private const string example = "\"Def(FunctionName)\" or \"Def(FunctionName, some, arguments)\"";
        private void CheckDefinition(FunctionToken def)
        {
            if (def.Name != "Def")
                throw new ParseException("Function definitions must start with a call named Def, like this: " + example)
                { LineNumber = def.Line, Snippet = def.Original, Range = def.Range };

            if (def.Arguments.Length == 0)
                throw new ParseException("Function definitions must have at leat one argument- the name: " + example)
                { LineNumber = def.Line, Snippet = def.Original, Range = def.Range };

        }

    }
}