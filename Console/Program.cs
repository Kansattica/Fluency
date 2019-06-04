using System;
using System.IO;
using System.Linq;
using Fluency.Interpreter;

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
            p.Parse(fileLines);
        }
    }
}
