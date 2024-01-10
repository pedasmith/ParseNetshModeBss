using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetshG
{
    public class UserPreferences
    {
        public bool ReplaceTabs = false;
        private bool _showHelp = false;
        public bool ShowHelp { get { return _showHelp; } set { _showHelp = value; } }
        public string Tags = "#common"; // CHECK: reset to common: "#common" when shipping;

    }

    public static class UP
    {
        public static UserPreferences CurrUserPrefs = new UserPreferences();
    }
}
