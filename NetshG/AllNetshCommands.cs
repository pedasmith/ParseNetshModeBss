using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Resources;

namespace NetshG
{
    class AllNetshCommands
    {
        private static List<CommandInfo> Commands = new List<CommandInfo>();
        public static List<CommandInfo> GetCommandsShow()
        {
			if (Commands.Count == 0)
			{
				Uri uri = new Uri("/AllCommands_Show.json", UriKind.Relative);
				StreamResourceInfo commands = Application.GetContentStream(uri);
				var sr = new StreamReader(commands.Stream);
				var alljson = sr.ReadToEnd();

				var newCommands = JsonConvert.DeserializeObject<List<CommandInfo>>(alljson);
				if (newCommands == null)
				{
					newCommands = new List<CommandInfo>() { };
				}
				foreach (var item in newCommands)
				{
					item.UpdateRequiresFromList(); // As appropriate
				}
				Commands = newCommands;
			}
            return Commands;
        }
    }
}

