using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

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

        public enum MajorFileType { Unknown, DashedSection, EqualsSection, IndentParser };
        public MajorFileType FileType = MajorFileType.DashedSection;

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
            if (FileType == MajorFileType.Unknown)
            {
                AutodetectParsing(file); // Sets up the settings
            }

            switch (FileType)
            {
                default:
                case MajorFileType.EqualsSection:
                case MajorFileType.DashedSection:
                    ParseSection(file);
                    break;
            }
        }
        private void Log(string str)
        {
            Console.WriteLine(str);
        }

        private void ParseSection(string file)
        {
            // TODO: this line is 100% not needed (it's done already)
            AutodetectParsing(file); // Sets up the settings

            var lines = file.Replace("\r\n", "\n").Split(new char[] { '\n' });

            var currSection = "";
            List<string>? currRow = null;
            var justSetSection = false;
            var justSetSectionCount = 999; // fake value
            var nsubsectionCount = 0;
            for (int i=0; i<lines.Length; i++) // First line is never anything
            {
                var line = lines[i];
                var nextLine = (i < (lines.Length-1)) ? lines[i + 1] : ""; ;
                justSetSectionCount++; // bump this; will be reset to zero as needed
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

                    var fields = line.SplitColon();
                    currSection = fields.Item1.Trim(); // Section is previous line e.g. "Domain Profile Settings:" 
                    RowUpsert(currRow, "Section", currSection);
                    //var col = ColumnUpsert("Section");
                    //RowEnsureWidth(currRow, col);
                    //currRow[col] = currSection;

                    justSetSection = true;
                    justSetSectionCount = 0;
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
                    //bool startsWithSpaces = false;
                    if (line.StartsWith(" "))
                    {
                        line = line.TrimStart();
                        //startsWithSpaces = true;
                    }
                    var fields = line.SplitSpaces();
                    if (string.IsNullOrEmpty(fields.Item2))
                    {
                        if (fields.Item1.EndsWith(":"))
                        {
                            // E.G., the "Logging:" section of netsh advfirewall show allprofiles
                        }
                        else if (fields.Item1.Contains(":"))
                        {
                            // Example: "maxcacheresponsesize (per-uri cache limit): 262144 bytes" from netsh http show cacheparam
                            fields = line.SplitColon();
                            RowUpsert(currRow, fields.Item1.Trim(), fields.Item2);
                            //var col = ColumnUpsert(fields.Item1.Trim());
                            //RowEnsureWidth(currRow, col);

                            //var value = fields.Item2;
                            //currRow[col] = value.Trim();
                        }
                        else if (fields.Item1 == "INPUT" || fields.Item1 == "OUTPUT")
                        {
                            // Example: netsh interface ipv4 show icmpstats divides the overall section (MIB-II ICMP Statistics)
                            // into two sections: one is input, and the other is output.
                            // TODO: consider making this more generic
                            if (currRow != null && nsubsectionCount > 0)
                            {
                                Rows.Add(currRow);
                            }
                            currRow = new List<string>();

                            RowUpsert(currRow, "Subsection", fields.Item1);
                            //var col = ColumnUpsert("Subsection");
                            //RowEnsureWidth(currRow, col);
                            //currRow[col] = fields.Item1;
                            nsubsectionCount++;
                        }
                        else
                        {
                            Log($"ERROR: unable to parse dash line={line}");
                        }
                    }
                    else
                    {
                        var colname = fields.Item1;
                        // Example: "Entry Type:                         Base Service Provider" from netsh winsock show catalog
                        if (colname.EndsWith(":")) colname = colname.Trim(':'); // Example: 
                        //var col = ColumnUpsert(colname);
                        //RowEnsureWidth(currRow, col);

                        var value = fields.Item2;
                        if (value.StartsWith(": "))
                        {
                            // Example: "UDP-fallback                : no"
                            // from command netsh dnsclient show encryption
                            value = value.Substring(2); // remove the ": "
                        }
                        //currRow[col] = value;
                        RowUpsert(currRow, colname, value);
                    }
                }
            } // end of FOR loop through all lines

            // The last row must be complete; add it. Make sure we have a fresh row ready for data!
            if (currRow != null)
            {
                Rows.Add(currRow);
            }
            MakeRectangular();
        }
    }
}
