using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Utilities
{
    internal class ParseDashLineTab : TableParse
    {
        public const string Example = """
Domain Profile Settings: 
----------------------------------------------------------------------
State                                 ON
Firewall Policy                       BlockInbound,AllowOutbound
LocalFirewallRules                    N/A (GPO-store only)
LocalConSecRules                      N/A (GPO-store only)
InboundUserNotification               Enable
RemoteManagement                      Disable
UnicastResponseToMulticast            Enable

Logging:
LogAllowedConnections                 Disable
LogDroppedConnections                 Disable
FileName                              %systemroot%\system32\LogFiles\Firewall\pfirewall.log
MaxFileSize                           4096

""";

        public bool SuppressLastOk = true;
        public string SectionSeperator = "-----";
        public string SubsectionSeperator = "";

        enum MajorFileType { DashedSection, EqualsSection };
        MajorFileType FileType = MajorFileType.DashedSection;

        private void AutodetectParsing(string file)
        {
            var nequals = file.CountChar('=');
            var ndash = file.CountChar('-');

            if (nequals > 5)
            {
                FileType = MajorFileType.EqualsSection;
                SectionSeperator = "=====";
                SubsectionSeperator = "-----";
            }
            else if (ndash > 5)
            {
                FileType = MajorFileType.DashedSection;
                SectionSeperator = "-----";
                SubsectionSeperator = "";
            }
        }
        public override void Parse(string file)
        {
            AutodetectParsing(file); // Sets up the settings
            var lines = file.Replace("\r\n", "\n").Split(new char[] { '\n' });

            var currSection = "";
            List<string>? currRow = null;
            var justSetSection = false;

            for (int i=0; i<lines.Length; i++) // First line is never anything
            {
                var line = lines[i];
                var nextLine = (i < (lines.Length-1)) ? lines[i + 1] : ""; ;
                if (nextLine.Contains(SectionSeperator))
                {
                    // Section might include a colon and might not.
                    // Example: "Name Resolution Policy Table Options " from netsh dnsclient show state
                    // Example: "Domain Profile Settings: " from netsh advfirewall show allprofiles
                    // The last row must be complete; add it. Make sure we have a fresh row ready for data!
                    if (currRow != null)
                    {
                        Rows.Add(currRow);
                    }
                    currRow = new List<string>();

                    var fields = SplitColon(line);
                    currSection = fields.Item1.Trim(); // Section is previous line e.g. "Domain Profile Settings:" 
                    var col = ColumnUpsert("Section");
                    RowEnsureWidth(currRow, col);
                    currRow[col] = currSection;

                    justSetSection = true;
                }
                else if (SubsectionSeperator != "" && line.Contains(SubsectionSeperator)) // DBG: IP
                {
                    ; // Skip it
                }
                else if (SubsectionSeperator != "" && nextLine.Contains(SubsectionSeperator)) // DBG: IP
                {
                    ; // Skip it
                }
                else if (line.Contains(SectionSeperator) && justSetSection)
                {
                    // No need to add the dashed line to the CSV :-)
                    justSetSection = false;
                }
                else if (string.IsNullOrEmpty(line))
                {
                    ; // do nothing; blank lines are for the user's convenience, not the parser's
                }
                else if (SuppressLastOk && line==("Ok.") && i > (line.Length-5))
                {
                    // Ignore. The Ok isn't always the exact last line.
                    ;
                }
                else if (currRow != null) // Working on a row
                {
                    // Must be data
                    bool startsWithSpaces = false;
                    if (line.StartsWith(" "))
                    {
                        line = line.TrimStart();
                        startsWithSpaces = true;
                    }
                    var fields = SplitSpaces(line);
                    if (fields.Item1.EndsWith(":") && string.IsNullOrEmpty(fields.Item2))
                    {
                        // E.G., the "Logging:" section of netsh advfirewall show allprofiles
                    }
                    else if (string.IsNullOrEmpty(fields.Item2) && fields.Item1.Contains(":"))
                    {
                        // Example: "maxcacheresponsesize (per-uri cache limit): 262144 bytes" from netsh http show cacheparam
                        fields = SplitColon(line);
                        var col = ColumnUpsert(fields.Item1.Trim());
                        RowEnsureWidth(currRow, col);

                        var value = fields.Item2;
                        currRow[col] = value.Trim();
                    }
                    else
                    {
                        var colname = fields.Item1;
                        // Example: "Entry Type:                         Base Service Provider" from netsh winsock show catalog
                        if (colname.EndsWith(":")) colname = colname.Trim(':'); // Example: 
                        var col = ColumnUpsert(colname);
                        RowEnsureWidth(currRow, col);

                        var value = fields.Item2;
                        if (value.StartsWith(": "))
                        {
                            // Example: "UDP-fallback                : no"
                            // from command netsh dnsclient show encryption
                            value = value.Substring(2); // remove the ": "
                        }
                        currRow[col] = value;
                    }
                }
            } // end of FOR loop through all lines

            // The last row must be complete; add it. Make sure we have a fresh row ready for data!
            if (currRow != null)
            {
                Rows.Add(currRow);
            }

        }

        private (string, string) SplitColon(string line)
        {
            var fields = line.Split(new char[] { ':', }, 2);
            var name = fields[0];
            var value = fields.Length >= 2 ? fields[1] : "";
            return (name, value);
        }
        private (string, string) SplitSpaces(string line)
        {
            var fields = line.Split("    ", 2); // Split requires at least 4 space
            var name = fields[0];
            var value = fields.Length >= 2 ? fields[1] : "";
            return (name, value.TrimStart());
        }



    }

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
    }

}
