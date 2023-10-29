// See https://aka.ms/new-console-template for more information

using ParseNetshModeBss;

// Console.WriteLine("# Parse output from netsh wlan show networks mode=bssid");
UserOptions options = new UserOptions();
foreach (var arg in args)
{
    Console.WriteLine($"arg is {arg}");
}

switch (options.CurrMode)
{
    case UserOptions.Mode.ReadStdin:
        string? line;
        var parser = new ParseBssidMode();
        while ((line = Console.ReadLine()) != null)
        {
            parser.ParseLine(line);
        }
        Console.WriteLine(SsidInfo.ToCsv(parser.ParsedData));
        break;

    case UserOptions.Mode.PrintExample:
        Console.WriteLine(ParseBssidMode.Examples[options.ExampleToShow]);
        break;

    case UserOptions.Mode.ReadExample:
        var result = ParseBssidMode.Parse(ParseBssidMode.Examples[options.ExampleToShow]);
        Console.WriteLine(SsidInfo.ToCsv(result));
        break;
}

class UserOptions
{
    public enum Mode { ReadStdin, ReadExample, PrintExample };
    public int ExampleToShow = 0;
    public Mode CurrMode = Mode.ReadStdin;
}
