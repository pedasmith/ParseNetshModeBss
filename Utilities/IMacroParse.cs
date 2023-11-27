using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    internal interface IMacroParse
    {
        public List<ArgumentSettingValue> ParseForValues(string value);
    }

    abstract class TableParse
    {
        public List<string> ColNames { get; } = new List<string>();
        /// <summary>
        /// If a column already exists, return the index. Otherwise, add to the colname and return the new index
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int ColumnUpsert(string name)
        {
            var index = ColNames.IndexOf(name);
            if (index < 0)
            {
                ColNames.Add(name);
                index = ColNames.Count -1;
            }
            return index;
        }
        public List<List<string>> Rows { get; } = new List<List<string>>();
        public static void RowEnsureWidth(List<string> row, int index, string defaultValue = "")
        {
            while (index >= row.Count)
            {
                row.Add(defaultValue);
            }
        }

        public abstract void Parse(string file);
        public string AsCsv()
        {
            var sb = new StringBuilder();
            var isFirst = true;
            foreach (var colname in ColNames)
            {
                if (!isFirst) sb.Append(',');
                isFirst = false;
                sb.Append(EscapeIfNeeded(colname));
            }
            sb.Append('\n');
            foreach (var row in Rows)
            {
                isFirst = true;
                foreach (var data in row)
                {
                    if (!isFirst) sb.Append(',');
                    isFirst = false;
                    sb.Append(EscapeIfNeeded(data));
                }
                sb.Append('\n');
            }
            return sb.ToString();
        }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Comma-separated_values
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string EscapeIfNeeded(string value)
        {
            var retval = value;
            bool mustQuote = false;
            if (retval.Contains("\""))
            {
                mustQuote = true;
                retval = retval.Replace("\"", "\"\"");
            }
            mustQuote = mustQuote || retval.Contains(",") || retval.Contains("\n") || retval.Contains("\r");
            if (mustQuote)
            {
                retval = "\"" + retval + "\"";
            }
            return retval;
        }
    }
}
