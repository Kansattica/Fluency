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
        public int? CountFrom { get; private set; } = null;
        public int? CountTo { get; private set; } = null;
        public List<string> ReadFile { get; private set; } = null;
        public string Separator { get; private set; } = "\n";
        public List<string> Leftover { get; private set; } = new List<string>();

        public ArgumentParser(IEnumerable<string> args)
        {
            if (!args.Any())
                Help = true;

            bool widthNext = false;
            bool readNext = false;
            bool readSep = false;
            bool nextCount = false;
            bool nextCountTo = false;
            foreach (string arg in args)
            {
                if (readNext)
                {
                    if (ReadFile == null) ReadFile = new List<string>(1);
                    ReadFile.Add(arg);
                    readNext = false;
                    continue;
                }

                if (readSep)
                {
                    Separator = arg;
                    readNext = false;
                    continue;
                }

                if (widthNext)
                {
                    if (int.TryParse(arg, out int width))
                    {
                        TabWidth = width;
                        continue;
                    }
                    widthNext = false;
                }

                if (nextCount)
                {
                    if (int.TryParse(arg, out int count))
                    {
                        CountFrom = count;
                        continue;
                    }
                    nextCount = false;
                }


                if (nextCountTo)
                {
                    if (int.TryParse(arg, out int count))
                    {
                        CountTo = count;
                        continue;
                    }
                    nextCountTo = false;
                }

                switch (arg)
                {
                    case "-v":
                    case "--verbose":
                        Verbose = true;
                        break;
                    case "-i":
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
                    case "--count-from":
                        nextCount = true;
                        break;
                    case "--count-to":
                        nextCountTo = true;
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