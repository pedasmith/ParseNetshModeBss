using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using Newtonsoft.Json;

namespace NetshG
{
    public class CommandInfo
    {
		public string ID { get; set; } = "";
        public string Cmd { get; set; } = "";
        public string Args { get; set; } = "";
        public string Help { get; set; } = "";
        public List<CommandRequire> Requires { get; set; } = new List<CommandRequire>();
        public string Sets { get; set; } = "";
        public string SetParser { get; set; } = "";
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
        private Dictionary<string,List<string>> Values = new Dictionary<string,List<string>>();
        private Dictionary<string,string> Current = new Dictionary<string,string>();

        public string Replace(string value, List<CommandRequire> requires)
        {
            string retval = value;
            foreach (var item in requires)
            {
                var itemvalue = GetCurrent(item.Name, "");
                if (itemvalue == "") 
                {
                    retval = $"ERROR: unable to expand {item.Name}";
                }
                else
                {
                    retval = retval.Replace(item.Replace, itemvalue);
                }
            }
            return retval;
        }
        public string GetCurrent(string name, string defaultValue)
        {
            string retval = defaultValue;
            if (Current.ContainsKey(name))
            {
                retval = Current[name];
            }
            return retval;
        }

        public void SetCurrent(string name, string value)
        {
            if (Current.ContainsKey(name))
            {
                Current[name] = value;
            }
            else
            {
                Current.Add(name, value);
            }
        }

        public void SetValueList(string name, List<string> values)
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
            if (curr == "" || !values.Contains(curr))
            {
                if (values.Count > 1)
                {
                    curr = values[0];
                }
            }
            if (curr != "")
            {
                    SetCurrent(name, curr);
            }
        }
    }
}
