using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetshG
{
    public class UserPreferences
    {
        public bool ReplaceTabs = true;
        private bool _showHelp = false;
        public bool ShowHelp { get { return _showHelp; } set { _showHelp = value; } }
        public string Tags = "#common"; // DBG: should be reset to common: "#common";

    }

    public static class UP
    {
        public static UserPreferences CurrUserPrefs = new UserPreferences();
    }
}
