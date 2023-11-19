// See https://aka.ms/new-console-template for more information

using ParseNetshModeBss;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;

namespace ParseNetshModeBss
{
    public enum Mode { BadArgs, Help, Version, ReadStdin, Generate, ReadExample, PrintExample };
    enum ReturnErrors { Ok, FailedSystemTest, BadArgs };
    class Program
    {
        const string Version = "1.1";
        static int Main(string[] args)
        {
            ReturnErrors retval = ReturnErrors.Ok;
            retval = RunSystemTests();
            if (retval != ReturnErrors.Ok) return (int)retval;

            UserOptions options = new UserOptions(args);

            var parser = new ParseBssidMode();
            var currDate = GetDate();
            var dateHeader = "Date,Time,";
            switch (options.CurrMode)
            {
                case Mode.BadArgs:
                    retval = ReturnErrors.BadArgs;
                    PrintHelp();
                    break;
                case Mode.Help:
                    retval = ReturnErrors.Ok;
                    PrintHelp();
                    break;


                case Mode.ReadStdin:
                    string? line;
                    while ((line = Console.ReadLine()) != null)
                    {
                        parser.ParseLine(line);
                    }
                    Console.Write(SsidInfo.ToCsv(parser.ParsedData, options.PrintCsvHeader, dateHeader, currDate + ","));
                    break;

                case Mode.Generate:
                    var lines = RunCommandLine.RunNetsh();
                    parser.Parse(lines);
                    Console.Write(SsidInfo.ToCsv(parser.ParsedData, options.PrintCsvHeader, dateHeader, currDate + ","));
                    break;

                case Mode.PrintExample:
                    Console.WriteLine(ParseBssidMode.Examples[options.ExampleToShow]);
                    break;

                case Mode.ReadExample:
                    var result = parser.Parse(ParseBssidMode.Examples[options.ExampleToShow]);
                    Console.Write(SsidInfo.ToCsv(result, options.PrintCsvHeader, dateHeader, currDate + ","));
                    break;
                case Mode.Version:
                    Console.WriteLine($"ParseNetshModeBss version={Version}");
                    break;
                default: // Huh? I added a mode and didn't handle it?
                    Console.WriteLine($"Error: unknown argument {options.CurrMode.ToString()} (version {Version})");
                    retval = ReturnErrors.BadArgs;
                    break;
            }

            return (int)retval;

        }

        private static string GetDate()
        {
            var date = DateTime.Now.ToString("yyyy-MM-dd,HH:mm:ss");
            return date;
        }



        private static ReturnErrors RunSystemTests()
        {
            var retval = ReturnErrors.Ok;
            int nerror = 0;
            nerror += Rfc4180_CSV.Test();
            if (nerror > 0)
            {
                Log("System test fails; ");
                retval = ReturnErrors.FailedSystemTest;
            }
            return retval;
        }

        private static void PrintHelp()
        {
            var help = $@"
ParseNetshBssMode {Version}: parses the output of netsh wlan show networks mode=bssid
That command often spits out pages of data; this command converts much of that data
into CSV-format (RFC 4180) compatible output that can be read in by Excel.

By default this acts like a filter, readin in from stdin and writing CSV to stdout

Switches:
/? -? /help -help: print out help
-generate   : automatically runs the netsh command and then produces the CSV output
-noheader   : prints CSV without a header
-example    :  prints out a CSV based on a baked-in example with no PII
-printexample:  prints out the baked-in example with no PII
-version    : prints version
";
            Console.WriteLine(help);
        }

        public static void Log(string str)
        {
            Console.WriteLine(str);
        }
    }


    class UserOptions
    {
        public Mode CurrMode = Mode.ReadStdin;
        public int ExampleToShow = 0;
        public bool PrintCsvHeader = true;

        public UserOptions(string[] args)
        {
            int nskip = 0;
            foreach (var arg in args)
            {
                if (nskip > 0)
                {
                    nskip--;
                    continue;
                }
                switch (arg)
                {
                    // How many standard ways are there for asking for help?
                    // Answer: so many ways.
                    case "/?":
                    case "-?":
                    case "/help":
                    case "-help":
                    case "--help":
                        CurrMode = Mode.Help;
                        break;
                    case "-version":
                        CurrMode = Mode.Version; 
                        break;
                    case "-example":
                        CurrMode = Mode.ReadExample;
                        break;
                    case "-generate":
                        CurrMode = Mode.Generate;
                        break;
                    case "-header":
                        PrintCsvHeader = true;
                        break;
                    case "-noheader":
                        PrintCsvHeader = false;
                        break;
                    case "-printexample":
                        CurrMode = Mode.PrintExample;
                        break;
                    default:
                        Program.Log($"ERROR: Unknown arg {arg}");
                        CurrMode = Mode.BadArgs;
                        return; // <=== return on bad argument
                }
            }
        }
    }
}




