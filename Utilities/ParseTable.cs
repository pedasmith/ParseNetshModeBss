﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    // NOTE: update to build off of IMacroParse.cs "TableParse" helper class
    // Note that right now (2023-12-30), nothing actually uses this class
    internal class ParseTable : IMacroParse
    {
        public string Example = """
Idx     Met         MTU          State                Name
---  ----------  ----------  ------------  ---------------------------
  1          75  4294967295  connected     Loopback Pseudo-Interface 1
  6          30        1500  disconnected  Wi-Fi
  4          25        1500  disconnected  Local Area Connection* 1
  9          65        1500  disconnected  Bluetooth Network Connection
  5          25        1500  connected     Ethernet 2

""";
        public List<string> ColumnNames = new List<string>();
        public List<List<string>> Rows = new List<List<string>>();

        public ParseTable()
        {
        }

        private void DoParse(string file) 
        {
            var lines = file.Replace("\r\n", "\n").Split(new char[] { '\n' });
            bool expectDashes = false;
            int ncol = 0;
            char splitChar = ' ';
            foreach (var line in lines)
            {
                if (line == "\r" || line == "")
                {
                    continue;
                }

                if (ColumnNames.Count == 0)
                {
                    var names = line.Trim().Split(splitChar, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    foreach (var name in names)
                    {
                        ColumnNames.Add(name);
                    }
                    expectDashes = true;
                    ncol = ColumnNames.Count;
                }
                else if (expectDashes)
                {
                    expectDashes = false;
                }
                else
                {
                    // Must be a row. Get the exact number of entries.
                    var row = new List<string>(ncol);
                    var cells = line.Trim().Split(splitChar, ncol, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    foreach (var cell in cells)
                    {
                        row.Add(cell);
                    }
                    Rows.Add(row);
                }
            }
        }

        private List<ArgumentSettingValue> GetColumn(string name)
        {
            var retval = new List<ArgumentSettingValue>();
            var index = ColumnNames.IndexOf(name);
            foreach (var row in Rows)
            {
                bool hasCol = (index >= 0) && (index < row.Count);
                retval.Add (new ArgumentSettingValue(hasCol ? row[index] : ""));
            }
            return retval;
        }

        public string ColumnToReturn = "";
        public List<ArgumentSettingValue> ParseForValues(string value)
        {
            DoParse(value);
            var retval = GetColumn(ColumnToReturn);
            return retval;
        }
    }
}
