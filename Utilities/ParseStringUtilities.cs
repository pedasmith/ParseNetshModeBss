using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilities
{
    public static class StringUtilities
    {
        public static int CountChar(this string value, char lookFor)
        {
            int retval = 0;
            foreach (var ch in value)
            {
                if (ch == lookFor)
                {
                    retval += 1;
                }
            }
            return retval;
        }

        public static int CountStrings(this string value, string lookFor)
        {
            int retval = 0;
            var index = value.IndexOf(lookFor);
            while (index >= 0)
            {
                retval++;
                index = value.IndexOf(lookFor, index + 1);
            }

            return retval;
        }
        private static void Log(string text)
        {
            Console.WriteLine(text);
        }
        private static int TestCountStrings_One(string value, string lookFor, int expected)
        {
            int nerror = 0;
            var actual = value.CountStrings(lookFor);
            if (actual != expected)
            {
                Log($"ERROR: CountString({value}, {lookFor}) expected={expected} actual={actual}");
                nerror++;
            }
            return nerror;
        }
        public static int TestCountStrings()
        {
            int nerror = 0;
            nerror += TestCountStrings_One("one two three four two three four three four four", "zero", 0);
            nerror += TestCountStrings_One("one two three four two three four three four four", "one", 1);
            nerror += TestCountStrings_One("one two three four two three four three four four", "two", 2);
            nerror += TestCountStrings_One("one two three four two three four three four four", "three", 3);
            nerror += TestCountStrings_One("one two three four two three four three four four", "four", 4);
            return nerror;
        }

        public static List<int> CountIndents(this string[]? lines)
        {
            if (lines == null) return new List<int>();
            var count = new SortedSet<int>();
            foreach (var line in lines)
            {
                var nspace = line.CountIndents();
                if (nspace < 0) continue;
                count.Add(nspace);
            }
            var retval = count.ToList();

            return retval;
        }
        public static int CountIndents(this string line)
        {
            if (string.IsNullOrEmpty(line)) return -1;
            if (line.TrimStart() == "") return -1;

            int retval = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] != ' ') break;
                retval++;
            }
            return retval;
        }

        public static int IndentLevel(this string line, List<int> indents)
        {
            var indent = line.CountIndents();
            for (int i = 0; i < indents.Count; i++)
            {
                if (indents[i] == indent) return i;
            }
            return -1;
        }

        /// <summary>
        /// Returns a string without a trailing \r (CR)
        /// OBSOLETE: replace is:             var lines = file.Replace("\r\n", "\n").Split(new char[] { '\n' });
        /// </summary>
        public static string ZZZRemoveCR(this string line)
        {
            var retval = line;
            if (retval.EndsWith("\r"))
            {
                retval = retval.Substring(0, retval.Length - 1);
            }
            return retval;
        }

        /// <summary>
        /// Takes e.g. Configuration for interface "Ethernet 2" and returns the Ethernet 2 without quotes
        /// </summary>
        public static string GetQuotedValue(this string line, string defaultValue)
        {
            var q1 = line.IndexOf('"');
            if (q1 < 0) return defaultValue;
            var q2 = line.IndexOf('"', q1 + 1);
            if (q2 < 0) return defaultValue;
            var len = q2 - q1 - 1;
            var retval = line.Substring(q1 + 1, len);
            return retval;
        }

        public static (string, string) SplitColon(this string line)
        {
            var fields = line.Split(new char[] { ':', }, 2);
            var name = fields[0];
            var value = fields.Length >= 2 ? fields[1] : "";
            return (name, value);
        }
        public static (string, string) SplitSpace(this string line)
        {
            var fields = line.Trim().Split(" ", 2); // Unlike SplitSpaces, just one space is enough
            var name = fields[0];
            var value = fields.Length >= 2 ? fields[1] : "";
            return (name, value.TrimStart());
        }
        /// <summary>
        /// Splits the line at a point where there are 4 spaces (or more). String is split into two.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>(left),(right) where right might just be "" if it didn't really exist</returns>
        public static (string, string) SplitSpaces(this string line)
        {
            var fields = line.Split("    ", 2); // Split requires at least 4 space
            var name = fields[0];
            var value = fields.Length >= 2 ? fields[1] : "";
            return (name, value.TrimStart());
        }
    }
}
