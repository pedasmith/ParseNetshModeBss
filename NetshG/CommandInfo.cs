using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Automation.Peers;
using Utilities;
using Utilities.ConfigurableParser;

namespace NetshG
{
    public class CommandInfo
    {
        public string ID { get; set; } = "";
        /// <summary>
        /// e.g. netsh
        /// </summary>
        public string Cmd { get; set; } = "";
        /// <summary>
        /// e.g. show interfaces. shown to user and part of ? to get help; 
        /// </summary>
        public string Args { get; set; } = ""; 
        /// <summary>
        /// shown to user but not part of the ? to get help
        /// </summary>
        public string Args2 { get; set; } = "";
        /// <summary>
        /// not shown to user and not part of the ? to get help
        /// </summary>
        public string Args5NoUX { get; set; } = ""; 
        public string Help { get; set; } = "/?";
        public string Issues { get; set; } = "Command has no known issues.";
        public List<CommandRequire> Requires { get; set; } = new List<CommandRequire>();
        public string RequireList { get; set; } = "";
        public string Sets { get; set; } = "";
        public string SetParser { get; set; } = "";
        public string TableParser { get; set; } = "";

        public string Tags { get; set; } = "";
        private List<string>? _taglist = null;
        public List<string> TagList
        {
            get
            {
                if (_taglist == null)
                {
                    var list = Tags.Split(' ').ToList();
                    _taglist = list;
                }
                return _taglist;
            }
        }
        public bool HasTag(string tag)
        {
            if (string.IsNullOrEmpty(tag) || tag == "#allverbose")
            {
                return true; // no tag == no filter requested == return everything!
            }
            var list = TagList;
            if (tag == "#all")
            {
                // #all means all but not the obsolete entries
                foreach (var item in list)
                {
                    if (item == "#obsolete") return false;
                }
                return true;
            }

            foreach (var item in list)
            {
                if (item == tag) return true;
            }

            if (this.TableParser == tag)
            {
                return true;
            }
            return false;
        }

        public void UpdateRequiresFromList()
        {
            if (string.IsNullOrEmpty(RequireList)) return;
            var list = RequireList.Split(",");
            foreach (var item in list)
            {
                Requires.Add(new CommandRequire() { Name = item });
            }
        }


        /// <summary>
        /// Given an arg like InterfaceIndex return a list of allCommands that can set this values. Return an empty list
        /// if the arg is already in the existingArgs structure.
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="allCommands"></param>
        /// <param name="existingArgs"></param>
        /// <returns>null=</returns>
        public static List<CommandInfo> GetMissingSettersFor(string arg, List<CommandInfo> allCommands, ArgumentSettings existingArgs, List<CommandInfo> returnList)
        {
            // arg is e.g. InterfaceIndex or Level
            var existing = existingArgs.GetCurrent(arg, "");
            if (existing.UserString == "")
            {
                foreach (var command in allCommands)
                {
                    if (command.Sets == arg)
                    {
                        returnList.Add(command);
                    }
                }
            }
            return returnList;
        }
        public static List<CommandInfo> GetAllMissingSettersFor(CommandInfo ci, List<CommandInfo> allCommands, ArgumentSettings existingArgs)
        {
            var retval = new List<CommandInfo>();
            foreach (var require in ci.Requires)
            {
                if (require == null) continue;
                GetMissingSettersFor(require.Name, allCommands, existingArgs, retval);
            }
            return retval;
        }
        public static string VerifyAllSetters(List<CommandInfo> commands, ArgumentSettings existingArgs)
        {
            string retval = "";
            foreach (var command in commands)
            {
                foreach (var requires in command.Requires)
                {
                    if (requires == null) continue;
                    var startingList = new List<CommandInfo>();
                    var list = GetMissingSettersFor(requires.Name, commands, existingArgs, startingList);
                    if (list.Count > 1)
                    {
                        retval += $"ERROR: {command.Cmd} {command.Args} {command.Args2} ARG={requires.Name} can be set by {list.Count} allCommands\n";
                        foreach (var item in list)
                        {
                            retval += $"    {item.Cmd} {item.Args} {item.Args2}\n";
                        }
                    }
                    else if (list.Count == 1)
                    {
                        var setby = list[0];
                        retval += $"NOTE: ARG={requires.Name} FROM {command.Cmd} {command.Args} {command.Args2}  can be set by {setby.Cmd} {setby.Args} {setby.Args2}\n";
                    }
                    else if (list.Count == 0 && existingArgs.GetCurrent(requires.Name, "").UserString == "")
                    {
                        retval += $"ERROR: {command.Cmd} {command.Args} {command.Args2} ARG={requires.Name} cannot be set\n";
                    }
                }
            }
            return retval;
        }

    }

    public class CommandRequire
    {
        public string Name { get; set; } = "";
        public string From { get; set; } = "";
        private string _replace = "";
        public string Replace
        {
            get
            {
                if (_replace == "") return Name;
                return _replace;
            }

            set { _replace = value; }
        }
    }

    
    public class ArgumentSettings
    {
        // Example: a command might have a "Sets" of "Profile". When the command is run, the output
        // is parsed and a list of strings is created and added to Values under "Profiles".
        // At the same time, the first element is added to Current as the value for "Profile".
        private Dictionary<string, List<ArgumentSettingValue>> Values = new Dictionary<string, List<ArgumentSettingValue>>();
        private Dictionary<string, ArgumentSettingValue> Current = new Dictionary<string, ArgumentSettingValue>();
        /// <summary>
        /// Smart routine that given a string with e.g. "netsh ... .level=Level" with a requires of "Level", replaces
        /// the Level with the right value (e.g., "normal" or "verbose")
        /// </summary>
        /// <param name="value"></param>
        /// <param name="requires"></param>
        /// <returns></returns>
        public string Replace(string value, List<CommandRequire> requires)
        {
            string retval = value;
            foreach (var item in requires)
            {
                var itemvalue = GetCurrent(item.Name, "");
                if (itemvalue.Value == "")
                {
                    retval = $"ERROR: unable to expand {item.Name}";
                }
                else
                {
                    retval = retval.Replace(item.Replace, itemvalue.Value);
                }
            }
            return retval;
        }
        /// <summary>
        /// Get 'Current' value for e.g. level (return "normal") or defaultValue for error.
        /// </summary>
        public ArgumentSettingValue GetCurrent(string name, string defaultValue)
        {
            ArgumentSettingValue retval = Current.ContainsKey(name) 
                ? Current[name] 
                : new ArgumentSettingValue(defaultValue);
            return retval;
        }
        /// <summary>
        /// Exampele: find the level=normal value, or return null;
        /// </summary>
        public ArgumentSettingValue? GetValue(string valueName, string value)
        {
            var idx = Find(valueName, value);
            ArgumentSettingValue? retval = idx>=0 ? Values[valueName][idx] : null;
            return retval;
        }

        /// <summary>
        /// Return list index for valueName=value (e.g., level=normal) or -1 for any error (can find level or normal)
        /// </summary>
        public int Find(string valueName, string value)
        {
            if (!Values.ContainsKey(valueName)) return -1;
            var list = Values[valueName];
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Value == value) return i;
            }
            return -1;
        }

        /// <summary>
        /// Returns the list of values associated with a parameters (e.g., "store" returns a list of two values, "active" and "persistent"
        /// </summary>
        /// <param name="valueName"></param>
        /// <returns></returns>
        public List<ArgumentSettingValue> GetValueList(string valueName)
        {
            if (!Values.ContainsKey(valueName)) return new List<ArgumentSettingValue>();
            var list = Values[valueName];
            return list;
        }

        public void SetCurrent(string name, ArgumentSettingValue? value)
        {
            if (value == null) return;
            if (Current.ContainsKey(name))
            {
                Current[name] = value;
            }
            else
            {
                Current.Add(name, value);
            }
        }

        public void SetValueList(string name, List<ArgumentSettingValue> values)
        {
            if (Values.ContainsKey(name))
            {
                Values[name] = values;
            }
            else
            {
                Values.Add(name, values);
            }
            var curr = GetCurrent(name, "");
            if (curr.Value == "" || !values.Contains(curr))
            {
                if (values.Count >= 1)
                {
                    curr = values[0];
                }
            }
            if (curr.Value != "")
            {
                SetCurrent(name, curr);
            }
        }
    }
}
