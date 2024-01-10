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
        public enum CommandType {  Show, Reset };
        private static List<CommandInfo> CommandsShow = new List<CommandInfo>();
        private static List<CommandInfo> CommandsReset = new List<CommandInfo>();
        public static List<CommandInfo> GetCommands(CommandType menuType)
        {
            switch (menuType)
            {
                case CommandType.Reset:
                    if (CommandsReset.Count == 0)
                    {
                        CommandsReset = ReadFile("/AllCommands_Reset.json"); ;
                    }
                    return CommandsReset;
                default:
                case CommandType.Show:
                    if (CommandsShow.Count == 0)
                    {
                        CommandsShow = ReadFile("/AllCommands_Show.json"); ;
                    }
                    return CommandsShow;
            }
        }

        private static List<CommandInfo> ReadFile(string name)
		{
            Uri uri = new Uri(name, UriKind.Relative);
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
            return newCommands;
        }
    }
}

