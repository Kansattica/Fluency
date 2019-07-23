using System;
using System.IO;
using System.Linq;
using Fluency.Interpreter.Parser;

namespace Fluency.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            ArgumentParser a = new ArgumentParser(args);

            if (a.Inspect != null)
            {
                foreach (string toInspect in a.Inspect)
                {
                    Console.WriteLine(string.Join("\n", toInspect.Select((c, idx) => idx + ": " + c)));
                    Console.Write(string.Join("\n", toInspect.GroupUntil(x => x == '.').Select(x => x.Stringify() + " " + x.Indexes)));
                }

                return;
            }

            Console.WriteLine("Welcome to Fluency!");
            var p = new Parser(a.Verbose);

            if (a.Verbose)
            {
                Console.WriteLine("My arguments are: {0}", string.Join(' ', args));
            }

            var fileLines = a.Leftover.SelectMany(x => File.ReadAllLines(x));
            var parsed = p.Parse(fileLines);

            if (!Directory.Exists("../Graphs")) { Directory.CreateDirectory("../Graphs"); }
            foreach (var graph in parsed)
            {
                var writer = new GraphWriter();
                writer.WalkFunctionGraph(graph.Head);
                writer.Serialize("../Graphs/" + graph.Name + ".dgml");
            }

            // Console.Write(string.Join(Environment.NewLine, parsed.SelectMany(x => x).Select(x => x.ToString())));
        }
    }
}
