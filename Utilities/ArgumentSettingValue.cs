using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    public class ArgumentSettingValue
    {
        public ArgumentSettingValue(string value)
        {
            Value = value;
        }
        public ArgumentSettingValue(string value, string userString)
        {
            Value = value;
            UserString = userString;
        }
        /// <summary>
        /// The value is the actual value that needs to be supplied as a macro expansion for a command.
        /// For example, netsh ... store=active|persistant where there are two values, "active" and 
        /// "persistent".
        /// </summary>
        public string Value = "";
        private string _userString = "";
        /// <summary>
        /// The user string is for things like an adapter index where a interface adapter
        /// has an index value (like "1") and a user-understandable string (like "Ethernet Adapter")
        /// </summary>
        public string UserString
        {
            get { return string.IsNullOrEmpty(_userString) ? Value : _userString; }
            set { _userString = value; }
        }

        public bool HasUserString {  get {  return !string.IsNullOrEmpty(_userString); } }
    }
}
