using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    internal class ParseIndent : TableParse
    {
         public override void Parse(string file)
        {
            var lines = file.Replace("\r\n", "\n").Split(new char[] { '\n' });
            var indents = lines.CountIndents();
            var indentstr = string.Join(",", indents);

            int prevIndent = 0;
            var prevLine = "";
            string l0linename = "";
            string l0namevalue = "";
            string l0colname = "";
            string l0index = "";
            List<string> currRow = new List<string>();
            var currRowHasData = false;

            // Saved rows
            List<string>[] savedRows = new List<string>[5]; // 5 is kind of arbitrary...

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var nextLine = (i+1 < lines.Length) ? lines[i + 1] : "";
                var indent = line.IndentLevel(indents);
                var nextIndent = nextLine.IndentLevel(indents);
                switch (indent)
                {
                    case 0:
                        if (indent < prevIndent)
                        {
                            if (currRowHasData)
                            {
                                Rows.Add(currRow);
                                currRowHasData = false;
                                currRow = savedRows[indent] ?? new List<string>();
                            }

                        }
                        if (line.Contains(":")) //  e.g., SSID 1 : MyHouseWiFi
                        {
                            // colname="SSID" index="1" value="MyHouseWiFi"
                            (l0linename, l0namevalue) = line.SplitColon();
                            (l0colname, l0index) = l0linename.SplitSpace();
                            l0colname = l0colname.Trim();
                            l0namevalue = l0namevalue.Trim();

                            var col = ColumnUpsert(l0colname);
                            RowEnsureWidth(currRow, col);
                            currRow[col] = l0namevalue;
                            currRowHasData = true;
                        }
                        else if (line.StartsWith("Configuration for")) //  e.g., Configuration for interface "Ethernet 2"
                        {
                            // This type of value happens with netsh interface ipv4 show config
                            l0namevalue = line.GetQuotedValue("(not set)");

                            var col = ColumnUpsert("Name");
                            RowEnsureWidth(currRow, col);
                            currRow[col] = l0namevalue;
                            currRowHasData = true;
                        }
                        if (nextIndent > indent) // save it for later
                        {
                            savedRows[indent] = new List<string>(currRow);
                        }
                        break;
                    case 1:
                    case 2:
                        // e.g.,     Network type            : Infrastructure
                        // or     BSSID 2                 : 70:3a:cb:19:26:68
                        var (name, value) = line.Trim().SplitColon(); // name is e.g., Network type or BBSID 2
                        name = name.Trim(); // see if it's BSSID 2 style -- with a space where the second is integer
                        value = value.Trim();

                        int index = 0;
                        var (namename, nameindex) = name.SplitSpace();
                        namename = namename.Trim();
                        var isInt = Int32.TryParse(nameindex, out index);
                        if (isInt && nextIndent > indent)
                        {
                            // Is the BSSID 2 case. Save the current row
                            savedRows[indent] = new List<string>(currRow);
                            var col = ColumnUpsert(namename);
                            RowEnsureWidth(currRow, col);
                            currRow[col] = value;
                            currRowHasData = true;
                        }
                        else
                        {
                            var col = ColumnUpsert(name);
                            RowEnsureWidth(currRow, col);
                            currRow[col] = value;
                            currRowHasData = true;
                        }

                        if (indent == 2 && nextIndent < indent)
                        {
                            // TODO: make robust
                            // row is complete; add to table and restore the old row
                            Rows.Add(currRow);
                            currRowHasData = false; // it has an echo of data. But don't save it unless more is added.
                            var indentForRow = nextIndent >= 0 ? nextIndent : 0;
                            currRow = savedRows[indentForRow] ?? new List<string>();
                        }
                        break;
                }
                if (!string.IsNullOrEmpty(line))
                {
                    prevLine = line;
                    prevIndent = indent;
                }
            }
            if (currRowHasData)
            {
                Rows.Add(currRow);
                currRowHasData = false;
                currRow = new List<string>();
            }
        }
    }
}
