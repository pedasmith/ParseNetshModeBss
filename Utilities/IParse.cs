using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    internal interface IParse
    {
        public List<ArgumentSettingValue> ParseForValues(string value);
    }
}
