﻿using ParseNetshModeBss;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    internal class ParseColonLines : IMacroParse, ILog
    {
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
                if (line.Contains(LineMustMatch))
                {
                    var nv = line.Split(SplitStr, 2);
                    if (nv.Length != 2)
                    {
                        Logger.Log($"ERROR: Line {line} cannot be split by {SplitStr}");
                        continue;
                    }
                    var n = nv[0].Trim();
                    var v = nv[1].Trim();
                    retval.Add(new ArgumentSettingValue(v));
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
