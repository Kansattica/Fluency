using System.Collections.Generic;
using System.Linq;

class ArgumentParser
{
    public bool Verbose { get; private set; } = false;
    public bool TabWarn { get; private set; } = true;
    public bool Unicode { get; private set; } = false;
    public bool Help { get; private set; } = false;
    public bool PrintGraph { get; private set; } = false;
    public bool WriteGraph { get; private set; } = false;
    public List<string> Inspect { get; private set; } = null;
    public List<string> Leftover { get; private set; } = new List<string>();

    public ArgumentParser(IEnumerable<string> args)
    {
        if (!args.Any())
            Help = true;

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
                case "--no-tab-warn":
                    TabWarn = false;
                    break;
                case "--tab-warn":
                    TabWarn = true;
                    break;
                case "--unicode-arrows":
                    Unicode = true;
                    break;
                case "-p":
                case "--print-graph":
                    PrintGraph = true;
                    break;
                case "-w":
                case "--write-graph":
                    WriteGraph = true;
                    break;
                case "--help":
                case "-h":
                    Help = true;
                    break;
                default:
                    Leftover.Add(arg);
                    break;

            }
        }

    }

}