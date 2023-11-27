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
                case "Interfaces":
                    retval = new ParseTable() { ColumnToReturn = "Idx" };
                    break;
                case "Profile":
                    retval = new ParseColonLines() { LineMustMatch = "All User Profile", SplitStr = ":" };
                    break;
            }
            return retval;
        }
        internal static ITableParse? GetTableParser(string value)
        {
            ITableParse? retval = null;
            switch (value)
            {
                case "DashLine":
                    retval = new ParseDashLineTab() {  };
                    break;
            }
            return retval;
        }
    }
}
