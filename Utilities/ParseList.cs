using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    internal class ParseList : TableParse, IMacroParse
    {
        public string Example = """
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

        private static string ExtractName(string value, string defaultValue="(no name)")
        {
            var retval = defaultValue;
            // The line is like Interface Ethernet 2 Parameters
            // Strip off the first and last words
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
            bool expectDashes = false;
            int ncol = 0;
            char splitChar = ':';

            var rowHasData = false;
            var row = new List<string>(ncol);

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var nextLine = (i + 1 < lines.Length) ? lines[i + 1] : "";

                if (line == "" && CurrParseType == ParseType.NamesWithDashes)
                {
                    continue;
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
                else if (line == "")
                {
                    // Empty line and parse type is NoNames -- the empty line indicates new data.
                    if (rowHasData)
                    {
                        Rows.Add(row);
                        rowHasData = false;
                    }
                    row = MakeNewRow();
                }
                else if (expectDashes)
                {
                    expectDashes = false;
                }
                else
                {
                    // Must be a row. Get the exact number of entries.
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

    }
}
