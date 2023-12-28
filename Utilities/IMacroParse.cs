using System;
using System.Collections.Generic;
using System.Data;
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

        public int ColumnIndex(string name)
        {
            var index = ColNames.IndexOf(name);
            return index;

        }
        public List<ArgumentSettingValue> GetColumn(string name, string userName)
        {
            var retval = new List<ArgumentSettingValue>();
            var index = ColNames.IndexOf(name);
            var userIndex = ColNames.IndexOf(userName);
            foreach (var row in Rows)
            {
                bool hasCol = (index >= 0) && (index < row.Count);
                bool hasUserCol = (userIndex >= 0) && (userIndex < row.Count);
                var value = hasCol ? row[index] : "";
                var valueUser = hasUserCol ? row[userIndex] : "";
                retval.Add(new ArgumentSettingValue(value, valueUser));
            }
            return retval;
        }

        public DataTable GetDataTable()
        {
            var table = new DataTable();
            var strType = System.Type.GetType("System.String");
            foreach (var item in ColNames)
            {
                var column = new DataColumn()
                {
                    DataType = strType,
                    ColumnName = item,
                };
                table.Columns.Add(column);
            }

            foreach (var row in Rows)
            {
                var r = table.NewRow();
                for (int i=0; i<row.Count; i++)
                {
                    var name = ColNames[i];
                    r[name] = row[i];
                }
                table.Rows.Add(r);
            }
            return table;
        }
        public List<List<string>> Rows { get; } = new List<List<string>>();
        public static void RowEnsureWidth(List<string> row, int index, string defaultValue = "")
        {
            while (index >= row.Count)
            {
                row.Add(defaultValue);
            }
        }

        public void RowUpsert(List<string> row, string colName, string value)
        {
            var col = ColumnUpsert(colName);
            RowEnsureWidth(row, col);
            row[col] = value;
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
                sb.Append(EscapeIfNeededCsv(colname));
            }
            sb.Append('\n');
            foreach (var row in Rows)
            {
                isFirst = true;
                foreach (var data in row)
                {
                    if (!isFirst) sb.Append(',');
                    isFirst = false;
                    sb.Append(EscapeIfNeededCsv(data));
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
        private string EscapeIfNeededCsv(string value)
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

        public string AsHtml()
        {
            var sb = new StringBuilder();
            var str = "";
            foreach (var colname in ColNames)
            {
                str += colname.th();
            }
            sb.Append(str.tr());
            foreach (var row in Rows)
            {
                var rowstr = "";
                foreach (var data in row)
                {
                    rowstr += data.td();
                }
                sb.Append(rowstr.tr());
            }
            var retval = sb.ToString().html();
            return retval;
        }
    }
}
