using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    internal class ParseList : TableParse, IMacroParse
    {
        private static string ExtractName(string value, string defaultValue="(no name)")
        {
            var retval = defaultValue;
            // The line is like Interface Ethernet 2 Parameters
            // Example: Compartment 1 Parameters (from netsh interface ipv4 show compartments level=verbose store=active)
            // Strip off the first and last words to return "Compartment 1"
            int firstSpace = value.IndexOf(' ');
            int lastSpace = value.LastIndexOf(' ');
            if (firstSpace != -1 && lastSpace != -1 && lastSpace > firstSpace)
            {
                var len = lastSpace - firstSpace - 1;
                retval = value.Substring(firstSpace + 1, len);
            }
            return retval;
        }

        enum ParseType {  NamesWithDashes, NoNames };
        ParseType CurrParseType = ParseType.NamesWithDashes;

        // Standard values
        string nameStd = ""; // goes into column "Name"
        private List<string> MakeNewRow()
        {
            var row = new List<string>();

            // Add standard values
            switch (CurrParseType)
            {
                case ParseType.NamesWithDashes:
                    RowUpsert(row, "Name", nameStd);
                    break;
                default: // no standard columns
                    break;
            }

            return row;
        }

        private void AutodetectParseType(string value)
        {
            var ndash = value.CountStrings(Dashes);
            CurrParseType = ParseType.NamesWithDashes; // Default
            if (ndash < 1)
            {
                CurrParseType = ParseType.NoNames;
            }
        }

        private string Dashes = "---------------------------------";
        public override void Parse(string file)
        {
            AutodetectParseType(file);
            var lines = file.Replace("\r\n", "\n").Split(new char[] { '\n' });
            var indents = lines.CountIndents();
            if (indents.Count == 2)
            {
                // e.g., the netsh wlan show interfaces where the non-indented lines are 
            }


            bool expectDashes = false;
            int ncol = 0;
            char splitChar = ':';

            var rowHasData = false;
            var row = new List<string>(ncol);

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var nextLine = (i + 1 < lines.Length) ? lines[i + 1] : "";
                var indentLevel = line.IndentLevel(indents);

                // Do stuff based on parse type
                switch (CurrParseType)
                {
                    case ParseType.NamesWithDashes:
                        if (line == "")
                        {
                            continue;
                        }
                        break; ;
                    case ParseType.NoNames:
                        // Empty line and parse type is NoNames -- the empty line indicates new data.
                        var newrowOnBlank = line == "" && indents.Count != 2;
                        var newrowOnName = line.Trim().StartsWith("Name ") && indents.Count == 2;
                        if (newrowOnBlank || newrowOnName)
                        {
                            if (rowHasData)
                            {
                                Rows.Add(row);
                                rowHasData = false;
                            }
                            row = MakeNewRow();
                        }
                        break;
                }

                if (nextLine.Contains(Dashes))
                {
                    // The line is like Interface Ethernet 2 Parameters
                    // Strip off the first and last words

                    if (rowHasData)
                    {
                        Rows.Add(row);
                        rowHasData = false;
                    }

                    nameStd = ExtractName(line, "(no name)");

                    row = MakeNewRow();
                    rowHasData = true;
                    expectDashes = true;
                }
                else if (expectDashes)
                {
                    expectDashes = false;
                }
                else
                {
                    if (indents.Count == 2 && indentLevel == 0)
                    {
                        continue;
                    }
                    // Must be a row. Get the exact number of entries.
                    if (line.Contains("DHCP/Static"))
                    {
                        ; // TODO: handy place for debugging
                    }
                    if (line.Trim() == "") continue; // Ignore blank lines here
                    var values = line.Trim().Split(splitChar, 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    var name = values[0].Trim();
                    var text = (values.Length > 1) ? values[1].Trim() : "";
                    RowUpsert(row, name, text);
                    rowHasData = true;
                }
            }
            if (rowHasData)
            {
                Rows.Add(row);
                rowHasData = false;
            }
        }



        public string ColumnToReturn = "";
        public List<ArgumentSettingValue> ParseForValues(string value)
        {
            Parse(value);
            var retval = GetColumn(ColumnToReturn, "Name");
            return retval;
        }

        public string Example1 = """
Interface Loopback Pseudo-Interface 1 Parameters
----------------------------------------------
IfLuid                             : loopback_0
IfIndex                            : 1
State                              : connected
Metric                             : 75
Link MTU                           : 4294967295 bytes

Interface Wi-Fi Parameters
----------------------------------------------
IfLuid                             : wireless_32768
IfIndex                            : 6
State                              : connected
Metric                             : 35
Link MTU                           : 1500 bytes
Reachable Time                     : 25500 ms

""";

        public string Example2 = """

There is 1 interface on the system: 

    Name                   : Wi-Fi
    Description            : Intel(R) Wi-Fi 6 AX201 160MHz
    GUID                   : 6947e285-cbb3-4da9-a1f8-1e37ec485c3e
    Physical address       : 2c:0d:a7:c8:53:2f
    Interface type         : Primary
    State                  : connected
    SSID                   : 7-Deco-B71C-MLO
    BSSID                  : 66:62:8b:42:b7:1f
    Network type           : Infrastructure
    Radio type             : 802.11ax
    Authentication         : WPA3-Personal  (H2E)
    Cipher                 : CCMP
    Connection mode        : Profile
    Band                   : 5 GHz  
    Channel                : 44
    Receive rate (Mbps)    : 2402
    Transmit rate (Mbps)   : 2402
    Signal                 : 93% 
    Profile                : 7-Deco-B71C-MLO 

    Hosted network status  : Not available


""";
    } // end 
}
