using ParseNetshModeBss;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    /// <summary>
    /// Despite the name, will parse any line of the form NAME SEPERATOR VALUE. By default,
    /// will pick up the name from "Value" (seriously, there's reasons for this) and the seperator
    /// is a colon. The line must match the "LineMustMatch" which by default is just a colon, and
    /// must not match the LineMustNotMatch which by default is "" which allows any line.
    /// </summary>
    internal class ParseColonLines : IMacroParse, ILog
    {
        public enum ColonLineParseType { ValueIsAfterColon, ValueIsBeforeColon };
        public ColonLineParseType ParseType = ColonLineParseType.ValueIsAfterColon;
        public string LineMustNotMatch = "";
        public string LineMustMatch = ":";
        public string SplitStr = ":";
        public ILog Logger;
        internal ParseColonLines()
        {
            Logger = this;
        }


        public List<ArgumentSettingValue> ParseForValues(string value)
        {
            var retval = new List<ArgumentSettingValue>();
            var lines = value.Split('\n');
            foreach (var line in lines)
            {
                bool notMatchOk = true;
                if (LineMustNotMatch != "")
                {
                    notMatchOk = !line.Contains(LineMustNotMatch);
                }
                if (notMatchOk && line.Contains(LineMustMatch))
                {
                    var nv = line.Split(SplitStr, 2);
                    if (nv.Length != 2)
                    {
                        Logger.Log($"ERROR: Line {line} cannot be split by {SplitStr}");
                        continue;
                    }
                    var n = nv[0].Trim();
                    var v = nv[1].Trim();
                    switch (ParseType)
                    {
                        case ColonLineParseType.ValueIsBeforeColon:
                            retval.Add(new ArgumentSettingValue(n, v));
                            break;
                        default:
                            retval.Add(new ArgumentSettingValue(v));
                            break;
                    }
                }
            }

            return retval;
        }

        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
