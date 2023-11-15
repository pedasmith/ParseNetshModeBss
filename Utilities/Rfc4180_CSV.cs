using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace ParseNetshModeBss
{
    /// <summary>
    /// Escapes data in accordance with RFC 4180, the CSV RFC
    /// Exception: the RFC text doesn't match the BNF; the text says that only CRLF needs
    /// to be escaped, but the BNF says that lone CR and LF needs to be escaped. Also, the
    /// text implies that UTF-8 is all good, but it's not actually allowed by the BNF.
    /// https://www.rfc-editor.org/rfc/rfc4180
    /// https://www.rfc-editor.org/rfc/rfc7111
    /// </summary>
    public static class Rfc4180_CSV
    {
        public static string Escape_Field(string value)
        {
            if (!value.ContainsAny(NeedsQuotes)) return value;
            value = "\"" + value.Replace("\"", "\\\"") + "\"";
            return value;
        }

        public static int Test()
        {
            int nerror = 0;
            nerror += TestOne("", "");
            nerror += TestOne("abc", "abc");
            nerror += TestOne("a'b'c", "a'b'c"); // not quoted

            // These all need quotes
            nerror += TestOne(",abc", "\",abc\"");
            nerror += TestOne("\"abc", "\"\\\"abc\"");
            nerror += TestOne("ab\"c", "\"ab\\\"c\"");
            nerror += TestOne("ab\nc", "\"ab\nc\"");
            return nerror;
        }
        private static int TestOne(string value, string expected)
        {
            int nerror = 0;
            var actual = Escape_Field(value);
            if (actual != expected)
            {
                nerror ++;
                Console.WriteLine($"ERROR: RFC4180: escape({value}) expected={expected} actual={actual}");
            }
            return nerror;
        }

        private static bool ContainsAny(this string value, char[] lookFor)
        {
            foreach (var ch in lookFor) { 
                if (value.Contains(ch)) return true;
            }
            return false;
        }
        private static bool NeedsQuote(char value)
        {
            if (value >= 0x00 && value < 0x20) return true; // include CR and LF
            if (value == 0x22) return true; // DQUOTE
            if (value == 0x2C) return true; // COMMA
            if (value >= 0x7F) return true; // DEL and up
            return false;
        }
        private static char[] NeedsQuotes = { '\r', '\n', '"', ',' };
    }
}
