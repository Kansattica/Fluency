using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Fluency.Execution;
using Fluency.Execution.Parsing;

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

            if (a.Help)
            {
                PrintHelp(Environment.GetCommandLineArgs()[0]);
                return;
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

            if (a.Autofix)
            {
                var fixer = new Autofix(a.Autofix, a.TabWidth);
                foreach (var file in a.Leftover)
                {
                    if (fixer.FixFile(file))
                        Console.WriteLine("Autofixed " + file);
                }
                return;
            }

            var p = new Parser(a.Verbose, a.TabWarn, a.TabWidth);

            if (a.WriteGraph)
            {
                if (!Directory.Exists("../Graphs")) { Directory.CreateDirectory("../Graphs"); }
                p.Register(graph =>
                {
                    var writer = new GraphWriter();
                    writer.WalkFunctionGraph(graph.Head);
                    writer.Serialize("../Graphs/" + graph.Name + ".dgml");
                });
            }

            if (a.PrintGraph)
            {
                p.Register(graph =>
                {
                    var printer = new GraphPrinter(graph.Head, a.Unicode);
                    Console.WriteLine(printer.Print());
                    Console.WriteLine();

                });
            }

            var fileLines = a.Leftover.Where(x => x.EndsWith(".fl")).SelectMany(x => File.ReadAllLines(x));

            var console = new ConsoleIO();
            var fileReader = new FileReader("numbers", " ");
            var result = new Interpreter(p).Execute(fileLines, fileReader.Read);

            console.Write(result);


            // Console.Write(string.Join(Environment.NewLine, parsed.SelectMany(x => x).Select(x => x.ToString())));
        }

        private static void PrintHelp(string programName)
        {
            Console.WriteLine("Welcome to Fluency, a functional programming language.");
            Console.WriteLine("Usage: {0} [flags] [paths to files ending in .fl]", programName);
            foreach (var pair in new[] {
                ("-h, --help", "You're lookin' at it."),
                ("-v, --verbose", "Run in verbose mode."),
                ("-i [string], --inspect [string]", "Inspect (print indexes of all characters in, try naive splitting on periods the given string."),
                ("--no-tab-warn", "Supress warning when tabs appear in source after text."),
                ("--tab-warn", "Enable warning when tabs appear in source after text. This is the default."),
                ("--autofix", "Expand tabs after text and write back to input file."),
                ("-t [number], --tab-width [number]", "How many spaces a tab is equal to. Defaults to four."),
                ("-w, --write-graph", "Write each function's parsed graph to ../Graphs/[FunctionName].dgml"),
                ("-p, --print-graph", "Print each function's parsed graph to stdout."),
                ("-p, --print-graph", "Print each function's parsed graph to stdout."),
                ("--unicode-arrows", "Show nice unicode arrows instead of the text ones when printing the graph to the console with -p. You may have to set unicode support in your terminal. In Powershell, try:\n[Console]::OutputEncoding = [Text.UTF8Encoding]::UTF8"),
                 })
            {
                Console.WriteLine("{0}\t{1}", pair.Item1, pair.Item2);
            }
        }
    }
}
