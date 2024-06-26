﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Resources;

namespace NetshG
{
    public class AllNetshCommands
    {
        public enum CmdType {  Show, Reset, Macro };
        private static List<CommandInfo> CommandsShow = new List<CommandInfo>();
        private static List<CommandInfo> CommandsReset = new List<CommandInfo>();
        private static List<CommandMacro> CommandMacros = new List<CommandMacro>();
        public static List<CommandMacro> GetMacros(CmdType menuType)
        {
            switch (menuType)
            {
                default:
                case CmdType.Macro:
                    if (CommandMacros.Count == 0)
                    {
                        CommandMacros = ReadMacroFile("/AllMacros_Reset.json"); ;
                    }
                    return CommandMacros;
            }
        }

        public static List<CommandInfo> GetCommands(CmdType menuType)
        {
            switch (menuType)
            {
                case CmdType.Reset:
                    if (CommandsReset.Count == 0)
                    {
                        CommandsReset = ReadFile("/AllCommands_Reset.json"); ;
                    }
                    return CommandsReset;
                default:
                case CmdType.Show:
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
        private static List<CommandMacro> ReadMacroFile(string name)
        {
            Uri uri = new Uri(name, UriKind.Relative);
            StreamResourceInfo commands = Application.GetContentStream(uri);
            var sr = new StreamReader(commands.Stream);
            var alljson = sr.ReadToEnd();

            var newCommands = JsonConvert.DeserializeObject<List<CommandMacro>>(alljson);
            if (newCommands == null)
            {
                newCommands = new List<CommandMacro>() { };
            }
            return newCommands;
        }
    }
}

