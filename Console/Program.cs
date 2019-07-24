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

            if (a.Verbose)
            {
                Console.WriteLine("My arguments are: {0}", string.Join(' ', args));
            }

            if (a.Inspect != null)
            {
                foreach (string toInspect in a.Inspect)
                {
                    Console.WriteLine(string.Join("\n", toInspect.Select((c, idx) => idx + ": " + c)));
                    Console.WriteLine(string.Join(", ", toInspect.GroupUntil(x => x == '.').Select(x => $"\"{x.Stringify()}\"")));
                    Console.WriteLine(string.Join(", ", toInspect.GroupUntil(x => x == '.').Select(x => x.Indexes.ToString())));
                }
                return;
            }

            var p = new Parser(a.Verbose, a.TabWarn);

            if (a.Help)
            {
                PrintHelp(Environment.GetCommandLineArgs()[0]);
                return;
            }

            var fileLines = a.Leftover.Where(x => x.EndsWith(".fl")).SelectMany(x => File.ReadAllLines(x));
            var parsed = p.Parse(fileLines);

            if (!Directory.Exists("../Graphs")) { Directory.CreateDirectory("../Graphs"); }
            foreach (var graph in parsed)
            {
                var writer = new GraphWriter();
                writer.WalkFunctionGraph(graph.Head);
                writer.Serialize("../Graphs/" + graph.Name + ".dgml");
                Console.WriteLine("Wrote graph for " + graph.Name);

                var printer = new GraphPrinter(graph.Head, a.Unicode);
                Console.WriteLine(printer.Print());
            }

            // Console.Write(string.Join(Environment.NewLine, parsed.SelectMany(x => x).Select(x => x.ToString())));
        }

        private static void PrintHelp(string programName)
        {
            Console.WriteLine("Welcome to Fluency, a functional programming language.");
            Console.WriteLine("Usage: {0} [flags] [paths to files ending in .fl]", programName);
            foreach (var pair in new [] {
                ("-v, --verbose", "Run in verbose mode."),
                ("-i [string], --inspect [string]", "Inspect (print indexes of all characters in, try naive splitting on periods the given string."),
                ("--no-tab-warn", "Supress warning when tabs appear in source after text."),
                ("--tab-warn", "Enable warning when tabs appear in source after text. This is the default."),
                ("--unicode-arrows", "Show nice unicode arrows instead of the text ones when printing the graph to the console. You may have to set unicode support in your terminal."),
                ("-h, --help", "You're lookin' at it.") })
                {
                    Console.WriteLine("{0}\t{1}", pair.Item1, pair.Item2);
                }
        }
    }
}
