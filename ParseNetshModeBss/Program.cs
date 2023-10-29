// See https://aka.ms/new-console-template for more information

using ParseNetshModeBss;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;

namespace ParseNetshModeBss
{
    public enum Mode { BadArgs, Help, ReadStdin, Generate, ReadExample, PrintExample };
    enum ReturnErrors { Ok, FailedSystemTest, BadArgs };
    class Program
    {
        static int Main(string[] args)
        {
            ReturnErrors retval = ReturnErrors.Ok;
            retval = RunSystemTests();
            if (retval != ReturnErrors.Ok) return (int)retval;

            UserOptions options = new UserOptions(args);

            var parser = new ParseBssidMode();
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
                    Console.WriteLine(SsidInfo.ToCsv(parser.ParsedData));
                    break;

                case Mode.Generate:
                    var lines = RunNetsh();
                    parser.Parse(lines);
                    Console.WriteLine(SsidInfo.ToCsv(parser.ParsedData));
                    break;

                case Mode.PrintExample:
                    Console.WriteLine(ParseBssidMode.Examples[options.ExampleToShow]);
                    break;

                case Mode.ReadExample:
                    var result = parser.Parse(ParseBssidMode.Examples[options.ExampleToShow]);
                    Console.WriteLine(SsidInfo.ToCsv(result));
                    break;
                default: // Huh? I added a mode and didn't handle it?
                    retval = ReturnErrors.BadArgs;
                    break;
            }

            return (int)retval;

        }

        private static string RunNetsh(string program = "netsh", string args = "wlan show networks mode=bssid")
        {
            string retval = "";
            var start = new ProcessStartInfo()
            {
                FileName = program,
                Arguments = args,
                RedirectStandardOutput = true,
                WindowStyle = ProcessWindowStyle.Hidden,
            };
            using (Process? proc = Process.Start(start))
            {
                retval = proc.StandardOutput.ReadToEnd();
                proc!.WaitForExit();
            }
            return retval;
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
            var help = @"
ParseNetshBssMode: parses the output of netsh wlan show networks mode=bssid
That command often spits out pages of data; this command converts much of that data
into CSV-format (RFC 4180) compatible output that can be read in by Excel.

By default this acts like a filter, readin in from stdin and writing CSV to stdout

Switches:
/? -? /help -help: print out help
-generate: automatically runs the netsh command and then produces the CSV output
-example:  prints out a CSV based on a baked-in example with no PII
-printexample:  prints out the baked-in example with no PII
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
                    case "-example":
                        CurrMode = Mode.ReadExample;
                        break;
                    case "-generate":
                        CurrMode = Mode.Generate;
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
        public int ExampleToShow = 0;
        public Mode CurrMode = Mode.ReadStdin;
    }
}




