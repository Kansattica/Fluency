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

            Console.WriteLine("Welcome to Fluency!");
            var p = new Parser(a.Verbose);

            if (a.Verbose)
            {
                Console.WriteLine("My arguments are: {0}", string.Join(' ', args));
            }

            var fileLines = a.Leftover.SelectMany(x => File.ReadAllLines(x));
            var parsed = p.Parse(fileLines);

            Console.Write(string.Join(Environment.NewLine, parsed.SelectMany(x => x).Select(x => x.ToString())));
        }
    }
}
