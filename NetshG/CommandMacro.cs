using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetshG
{
    public class CommandMacro
    {
        public string Name { get; set; } = "?";
        public List<CommandInfo> Commands { get; } = new List<CommandInfo>();
    }
}
