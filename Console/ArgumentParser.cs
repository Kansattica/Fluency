using System.Collections.Generic;

class ArgumentParser
{
    public bool Verbose { get; private set; }
    public List<string> Inspect { get; private set; }
    public List<string> Leftover { get; private set; } = new List<string>();

    public ArgumentParser(IEnumerable<string> args)
    {
        bool inspectNext = false;
        foreach (string arg in args)
        {
            if (inspectNext)
            {
                if (Inspect == null) Inspect = new List<string>(1);
                Inspect.Add(arg);
                continue;
            }

            switch (arg)
            {
                case "-v":
                case "--verbose":
                    Verbose = true;
                    break;
                case "-i":
                case "--inspect":
                    inspectNext = true;
                    break;
                default:
                    Leftover.Add(arg);
                    break;

            }
        }

    }

}