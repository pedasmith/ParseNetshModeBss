using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Windows.Documents;

namespace NetshG
{
    public class UserPreferences
    {
        public bool ReplaceTabs = false;
        private bool _showHelp = false;
        public bool ShowHelp { get { return _showHelp; } set { _showHelp = value; } }
        public string Tags = "#common"; // CHECK: reset to common: "#common" when shipping;
        public AllNetshCommands.CmdType CmdType { get; set; } = AllNetshCommands.CmdType.Show;

        public List<string> Favorites = new List<string>() {
        };
        public bool IsFavorite(CommandInfo ci)
        {
            var test = ci.FavoriteMatch;
            var retval = Favorites.Contains(test);
            return retval;
        }
        public DisplayOptions CurrDisplayOptions { get; } = new DisplayOptions();
        public override string ToString()
        {
            return $"ShowHelp={ShowHelp} CmdType={CmdType} Tags={Tags} Display={CurrDisplayOptions.CurrShowWhat}";
        }

    }

    public class AutomationCommand
    {
        public enum AutomationType {  RunCommand, Search };
        public AutomationType Automation { get; set;  } = AutomationType.RunCommand;
        // AllNetshCommands.CmdType CmdType = AllNetshCommands.CmdType.Show; // Will always be show, nothing else is allowed.

        // RunCommand fields
        public string CommandToRun = "netshinterfaceipv4showaddressesInterfaceIndex";
    }

    /// <summary>
    /// Global for saving/restoring user preferences + command line arguments for MainWindow
    /// </summary>
    public static class UP
    {
        public static UserPreferences CurrUserPrefs = new UserPreferences();
        public static void Save()
        {
            try
            { 
                var json = JsonSerializer.Serialize(CurrUserPrefs);
                Properties.Settings.Default.JSON = json;
            }
            catch (Exception ex)
            {
                Log($"ERROR: exception when save user preferences. Message={ex.Message}");
            }
        }

        public static void Restore()
        {
            try
            {
                var json = Properties.Settings.Default.JSON;
                var prefs = JsonSerializer.Deserialize<UserPreferences>(json);
                Log($"DBG: restore from {prefs?.ToString()}");
            }
            catch (Exception ex)
            {
                Log($"ERROR: exception when restoring user preferences. Message={ex.Message}");
            }
        }

        private static void Log(string str)
        {
            Console.WriteLine(str);
        }

        /// <summary>
        /// Startup commands parsed out from 
        /// </summary>
        public static List<AutomationCommand> StartupCommands = new List<AutomationCommand>();
    }
}
