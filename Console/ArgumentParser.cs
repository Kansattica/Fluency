using System.Collections.Generic;

class ArgumentParser
{
    public bool Verbose { get; private set; }
    public List<string> Leftover { get; private set; } = new List<string>();

    public ArgumentParser(IEnumerable<string> args)
    {
        foreach (string arg in args)
        {
            switch (arg)
            {
                case "-v":
                case "--verbose":
                    Verbose = true;
                    break;
                default:
                    Leftover.Add(arg);
                    break;

            }
        }

    }

}