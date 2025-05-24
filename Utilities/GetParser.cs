using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    internal  static class GetParser
    {
        internal static IMacroParse? GetMacroParser(string value)
        {
            IMacroParse? retval = null; 
            switch (value)
            {
                case "DashLine":
                    retval = null; // new ParseDashLineTab() {  };
                    break;
                case "IndentAllowSpaces":
                    retval = new ParseIndent() { AllowSpacesInName = true };
                    break;
                case "Interfaces":
                    retval = new ParseList() { ColumnToReturn = "IfIndex" };
                    break;
                case "List":
                    retval = new ParseList() { ColumnToReturn = "Name" };
                    break;
                case "Profile":
                    retval = new ParseColonLines() { LineMustMatch = "All User Profile", SplitStr = ":" };
                    break;
                case "Provider":
                    retval = new ParseList() { ColumnToReturn = "Name" };
                    break;
                case "Scenario":
                    retval = new ParseColonLines() { LineMustNotMatch = "Available scenarios (" , ParseType=ParseColonLines.ColonLineParseType.ValueIsBeforeColon };
                    break;
            }
            return retval;
        }
        internal static TableParse? GetTableParser(string value)
        {
            TableParse? retval = null;
            switch (value)
            {
                default:
                case "DashLine":
                    retval = new ParseDashLineTab() { };
                    break;
                case "Indent":
                    retval = new ParseIndent();
                    break;
                case "IndentAllowSpaces":
                    retval = new ParseIndent() { AllowSpacesInName = true };
                    break;
                case "List":
                    retval = new ParseList();
                    break;
            }
            return retval;
        }
    }
}
