using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    internal interface IMacroParse
    {
        public List<ArgumentSettingValue> ParseForValues(string value);
    }

    internal interface ITableParse
    {

    }
}
