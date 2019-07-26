using System;
using System.Collections.Generic;
using System.Linq;

namespace Fluency.CLI
{

    class ArgumentParser
    {
        public bool Verbose { get; private set; } = false;
        public bool TabWarn { get; private set; } = true;
        public int TabWidth { get; private set; } = 4;
        public bool Unicode { get; private set; } = false;
        public bool Help { get; private set; } = false;
        public bool PrintGraph { get; private set; } = false;
        public bool WriteGraph { get; private set; } = false;
        public bool Autofix { get; private set; } = false;
        public List<string> ReadFile { get; private set; } = null;
        public string Separator { get; private set; } = "\n";
        public List<string> Inspect { get; private set; } = null;
        public List<string> Leftover { get; private set; } = new List<string>();

        public ArgumentParser(IEnumerable<string> args)
        {
            if (!args.Any())
                Help = true;

            bool inspectNext = false;
            bool widthNext = false;
            bool readNext = false;
            bool readSep = false;
            foreach (string arg in args)
            {
                if (inspectNext)
                {
                    if (Inspect == null) Inspect = new List<string>(1);
                    Inspect.Add(arg);
                    continue;
                }

                if (readNext)
                {
                    if (ReadFile == null) ReadFile = new List<string>(1);
                    ReadFile.Add(arg);
                    continue;
                }

                if (readSep)
                {
                    Separator = arg;
                    continue;
                }

                if (widthNext)
                {
                    if (int.TryParse(arg, out int width))
                    {
                        TabWidth = width;
                        continue;
                    }
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
                    case "-f":
                    case "--in-file":
                        readNext = true;
                        break;
                    case "-s":
                    case "--separator":
                        readSep = true;
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
                    case "-t":
                    case "--tab-width":
                        break;
                    case "--help":
                    case "-h":
                        Help = true;
                        break;
                    case "--autofix":
                        Autofix = true;
                        break;
                    default:
                        Leftover.Add(arg);
                        break;

                }
            }

        }
    }
}