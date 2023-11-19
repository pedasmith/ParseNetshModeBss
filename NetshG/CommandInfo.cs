using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using Newtonsoft.Json;
using Utilities;

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
        public string ArgsExtra { get; set; } = "";
        /// <summary>
        /// not shown to user and not part of the ? to get help
        /// </summary>
        public string MoreArgs { get; set; } = ""; 
        public string Help { get; set; } = "";
        public List<CommandRequire> Requires { get; set; } = new List<CommandRequire>();
        public string RequireList { get; set; } = "";
        public string Sets { get; set; } = "";
        public string SetParser { get; set; } = "";

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
            if (string.IsNullOrEmpty(tag))
            {
                return true; // no tag == no filter requested == return everything!
            }
            var list = TagList;
            foreach (var item in list)
            {
                if (item == tag) return true;
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
                if (values.Count > 1)
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
