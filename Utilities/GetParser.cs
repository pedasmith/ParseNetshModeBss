using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    internal  static class GetParser
    {
        internal static IParse Get(string value)
        {
            IParse retval = new ParseColonLines(); // TODO: what's the right kind of default parser?
            switch (value)
            {
                case "Profile":
                    retval = new ParseColonLines() { LineMustMatch = "All User Profile", SplitStr = ":" };
                    break;
            }
            return retval;
        }
    }
}
