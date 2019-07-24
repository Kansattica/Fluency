using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fluency.Interpreter.Parsing.Entities.FunctionGraph;

namespace Fluency.CLI
{
    class GraphPrinter
    {
        private readonly FunctionNode head;
        private string next = " → ";
        private const string below = @"\ ";
        private const string above = "/";

        public GraphPrinter(FunctionNode head, bool unicode = true)
        {
            this.head = head;
            if (!unicode)
                next = " -> ";
        }

        public string Print()
        {
            List<PrintedFunction> lastPrinted = new List<PrintedFunction>();
            List<PrintedFunction> printed = new List<PrintedFunction>();
            Queue<FunctionNode> toVisit = new Queue<FunctionNode>();

            StringBuilder thisLine = new StringBuilder();
            //We want to do a depth-first search, I think.
            //the first row is easy

            FunctionNode curr = head;
            while (curr != null)
            {
                var pnode = new PrintedFunction(curr, thisLine.Length);
                thisLine.Append(pnode.Stringed);
                thisLine.Append(next);
                if (curr.BottomOut != null)
                {
                    toVisit.Enqueue(curr.BottomOut);
                    pnode.Child = curr.BottomOut;
                }
                lastPrinted.Add(pnode);
                curr = curr.TopOut;
            }
            thisLine.Append("Return (top)");
            toVisit.Enqueue(null);
            string toReturn = thisLine.ToString();
            thisLine = new StringBuilder();

            bool secondLine = true;
            while (toVisit.TryDequeue(out curr))
            {
                if (curr == null)
                {
                    //we're done with this line
                    if (secondLine)
                    {
                        secondLine = false;
                        string check = thisLine.ToString();
                        int lastUp = check.LastIndexOf('/');
                        int lastParen = check.LastIndexOf(')');

                        if (lastParen > lastUp)
                        {
                            thisLine.Append(next);
                            thisLine.Append("Return (bottom)");
                        }
                    }

                    toReturn = toReturn + "\n" + thisLine.ToString();
                    thisLine = new StringBuilder();
                    if (toVisit.Count != 0)
                    {
                        // if there's more stuff in the queue, then there's another line after this
                        toVisit.Enqueue(null);
                    }
                    lastPrinted = printed;
                    printed = new List<PrintedFunction>();
                    continue;
                }

                // Find where this node is in the last line to print underneath it
                var parent = lastPrinted.First(x => x.Child == curr);
                int printAt = parent.Midpoint + 2;
                EnsurePadding(thisLine, printAt);
                var pcurr = new PrintedFunction(curr, thisLine.Length);
                thisLine.Append(below);
                thisLine.Append(pcurr.Stringed);
                printed.Add(pcurr);

                if (curr.BottomOut != null)
                {
                    toVisit.Enqueue(curr.BottomOut);
                    pcurr.Child = curr.BottomOut;
                }

                while (curr.TopOut != null)
                {
                    var top = lastPrinted.FirstOrDefault(x => x.Node == curr.TopOut);
                    //A TopOut can either be above you or on the same level.
                    if (top != null)
                    {
                        //It's above
                        EnsurePadding(thisLine, top.Midpoint - 2);
                        thisLine.Append(above);
                        curr.TopOut = null;
                    }
                    else
                    {
                        //It's next
                        curr = curr.TopOut;
                        var nextpcurr = new PrintedFunction(curr, thisLine.Length);
                        EnsurePadding(thisLine, printAt);
                        thisLine.Append(next);
                        thisLine.Append(nextpcurr.Stringed);
                        printed.Add(nextpcurr);
                    }
                }
            }

            return toReturn;
        }

        private void EnsurePadding(StringBuilder sb, int atLeast)
        {
            int toAdd = atLeast - sb.Length;
            if (toAdd > 0)
                sb.Append(' ', toAdd);
        }

        private class PrintedFunction
        {
            public FunctionNode Node;
            public FunctionNode Child;
            public int Midpoint;
            public string Stringed;

            public PrintedFunction(FunctionNode node, int currentLength)
            {
                Node = node;
                Stringed = node.ToString();
                Midpoint = currentLength + (Stringed.Length / 2);
            }
        }

    }
}