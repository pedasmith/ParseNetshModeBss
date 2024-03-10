﻿using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using Windows.ApplicationModel.DataTransfer;
using ParseNetshModeBss; // to get the utilities classes!
using Utilities;
using System.Windows.Input;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;
using static NetshG.CanDoCommand;
using System.IO;
using System.Windows.Resources;
using System.Xml.Linq;
using Markdig;
using Markdig.Wpf;
using System.Text;
using System.Windows.Documents;
using System.Xaml;
using System.Reflection;



using XamlReader = System.Windows.Markup.XamlReader;
using Windows.ApplicationModel.VoiceCommands;


namespace NetshG
{
    public interface CanDoCommand
    {
        [Flags]
        enum CommandOptions {  None, SuppressFlash=0x01, AppendToTable=0x02, KeepRepeatButtons=0x04 }
        Task DoCommandAsync(CommandInfo ci, CommandOptions commandOptions = CommandOptions.None);
        void DoClearTable();
    }

    public interface UXCommands
    {
        Task OnRepeatAsync(object sender, RoutedEventArgs e);
        // void OnMenuRepeatStop(object sender, RoutedEventArgs e)
        void Log(string str);
        void SetAmDoCommand(bool value);
        void SetCommand(string str);
        void SetUIIssues(string str);
        void SetCount(string str);
        ArgumentSettings GetCurrArgumentSettings();
        void Help_Remove();

    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, CanDoCommand, AddToText, UXCommands
    {
        public MainWindow()
        {
            InitializeComponent();
            uiCommandControl.UXCommands = this;
            this.Loaded += MainWindow_Loaded;

            // ^C
            KeyDescriptions.Clear();
            CommandAdd(Key.C, ModifierKeys.Control, OnCopy, "^C", "Copy data from output or table");

            // Alt-F4 for Exit
            CommandAdd(Key.F1, ModifierKeys.None, OnMenu_Help_Help, "F1", "Show help file");
            CommandAdd(Key.F4, ModifierKeys.Alt, OnMenu_File_Exit, "ALT+F4", "Exit program");
            CommandAdd(Key.F5, ModifierKeys.None, OnRepeat, "F5", "Repeat command");

            // Specialized
            CommandAdd(Key.A, ModifierKeys.Alt, (s, e) => { DoSetMenuWithTag("#all"); }, "ALT+A", "Show all commands in command list");
            CommandAdd(Key.C, ModifierKeys.Alt, (s, e) => { DoSetMenuWithTag("#common"); }, "ALT+C", "Show common command in command list");
            CommandAdd(Key.O, ModifierKeys.Alt, (s, e) => { ShowOutputOrTable(CommandOutputControl.ShowWhat.Output); }, "ALT-O", "Show output as text, not as table");
            //CommandAdd(Key.R, ModifierKeys.Alt, OnRepeat, "ALT+R", "Repeat command"); Removed; it's part of the menu now. But still good to tell user
            KeyDescriptions.Add(new HelpDescription("ALT+R R", "Repeat command"));
            KeyDescriptions.Add(new HelpDescription("ALT+R S", "Repeat command every second"));
            KeyDescriptions.Add(new HelpDescription("ALT+R M", "Repeat command every minute"));
            KeyDescriptions.Add(new HelpDescription("ALT+R H", "Repeat command every hour"));

            CommandAdd(Key.T, ModifierKeys.Alt, (s, e) => { ShowOutputOrTable(CommandOutputControl.ShowWhat.Table); }, "ALT+T", "Show output as table, not as text");
            CommandAdd(Key.V, ModifierKeys.Alt, (s, e) => { ToggleOutputOrTable(); }, "ALT+V", "Toggle between text and table for output");
            CommandAdd(Key.W, ModifierKeys.Alt, (s, e) => { DoSetMenuWithTag("#wifi"); }, "ALT+W", "Show common Wi-Fi commands");

            int nerror = 0;
            nerror += Utilities.StringUtilities.TestCountStrings();

        }
        /// <summary>
        /// Set here, but used by the HelpKeyboardShortcutWindow to show shortcuts. That way the help is always
        /// in sync with the actual shortcuts.
        /// </summary>
        public static List<HelpDescription> KeyDescriptions = new List<HelpDescription>();
        /// <summary>
        /// Internal stuff used to expand out commands -- e.g., "level" might be "verbose"
        /// </summary>
        private ArgumentSettings CurrArgumentSettings = new ArgumentSettings();
        public ArgumentSettings GetCurrArgumentSettings() {  return CurrArgumentSettings; }

        /// <summary>
        /// LastCommand is used by the Repeat command.
        /// </summary>
        CommandInfo? LastCommand = null;





        int CurrRepeatTimeInSeconds = 0; // not repeating


        private void CommandAdd(Key key, ModifierKeys mod, ExecutedRoutedEventHandler handler, string shortcut, string description)
        {
            RoutedCommand cmdOutput = new RoutedCommand();
            cmdOutput.InputGestures.Add(new KeyGesture(key, mod));
            CommandBindings.Add(new CommandBinding(cmdOutput, handler));

            KeyDescriptions.Add(new HelpDescription(shortcut, description));
        }


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DoInitializeCommonArguments();
            DoSetMenuWithTag(UP.CurrUserPrefs.Tags);
            
            uiMenu_Parameters_Common.Items.Clear();
            DoSetupCommonMenu("Level");
            DoSetupCommonMenu("Store");
            DoSetupCommonMenu("Parser");
            DoSetupCommonMenu("TestHost");

            uiMenu_Show_Help.IsChecked = UP.CurrUserPrefs.ShowHelp;

            var tags = UP.CurrUserPrefs.Tags;
            foreach (var item in uiMenuShow.Items)
            {
                var menu = item as MenuItem;
                if (menu == null) continue;
                if (menu.Tag as string == tags)
                {
                    menu.IsChecked = true;
                }
            }

            // Just for testing
            var cmdlist = AllNetshCommands.GetCommands(AllNetshCommands.CommandType.Show);
            CommandInfo.VerifyAllSetters(cmdlist, CurrArgumentSettings);
        }

        AllNetshCommands.CommandType CurrCommandType = AllNetshCommands.CommandType.Show;
        private void DoSetMenuWithTag(string tags, AllNetshCommands.CommandType menuType = AllNetshCommands.CommandType.Show)
        {
            UP.CurrUserPrefs.Tags = tags;
            CurrCommandType = menuType;
            var cmdlist = AllNetshCommands.GetCommands(CurrCommandType);

            if (cmdlist.Count == 0)
            {
                uiIssues.Text = "ERROR: unable to load commands";
                return;
            }
            uiCommandList.Items.Clear();
            foreach (var cmd in cmdlist)
            {
                if (cmd.HasTag(tags))
                {
                    var ctrl = new NetshCommandControl(cmd);
                    uiCommandList.Items.Add(ctrl);
                }
            }
            uiIssues.Text = $"{tags}: {uiCommandList.Items.Count} commands out of {cmdlist.Count}";
        }

        private void DoSetMenuWithSearch (string search)
        {
            var cmdlist = AllNetshCommands.GetCommands(CurrCommandType);

            if (cmdlist.Count == 0)
            {
                uiIssues.Text = "ERROR: unable to load commands";
                return;
            }
            uiCommandList.Items.Clear();
            search = search.ToLower();
            foreach (var cmd in cmdlist)
            {
                if (cmd.AllUserText.ToLower().Contains(search))
                {
                    var ctrl = new NetshCommandControl(cmd);
                    uiCommandList.Items.Add(ctrl);
                }
            }
            uiIssues.Text = $"{search}: {uiCommandList.Items.Count} commands out of {cmdlist.Count}";

        }
        public void OnCopy(object sender, RoutedEventArgs e)
        {
            uiCommandControl.OnCopy(sender, e);
        }

        public async Task OnRepeatAsync(object sender, RoutedEventArgs e)
        {
            // ALT-R will either do a single repeat OR will stop a current repeat loop
            if (AmRepeating())
            {
                OnMenuRepeatStop(sender, e);
                return;
            }
            if (LastCommand == null) return;
            await DoCommandAsync(LastCommand);
        }
        public async void OnRepeat(object sender, RoutedEventArgs e)
        {
            await OnRepeatAsync(sender, e);
        }
        private async void OnSelectCommand(object sender, SelectionChangedEventArgs e)
        {
            OnMenuRepeatStop(sender, e); // If I was repeating, stop it.

            if (e.AddedItems.Count != 1) return; // only one item selected
            var fe = e.AddedItems[0] as ContentControl;
            if (fe == null) return; // seriously, it's always a framework element.

            var nsc = fe as NetshCommandControl; // not all command are the command control??
            CommandInfo ci;
            if (nsc != null)
            {
                ci = nsc.CommandInfo;
            }
            else // fallback in case of weirdness. just do a wlan show
            {
                ci = new CommandInfo()
                {
                    Cmd = "netsh",
                    Args = "wlan show"
                };
            }
            LastCommand = ci;

            await DoCommandAsync(ci);
        }

        public void DoClearTable()
        {
            uiCommandControl.DoClearTable();
        }
        bool AmDoCommand = false;
        public void SetAmDoCommand(bool value)
        {
            AmDoCommand = value;
        }
        public async Task DoCommandAsync(CommandInfo ci, CommandOptions commandOptions = CommandOptions.None)
        {
            // We know we have to use the "Show" commands to get the data. Any other list
            // will potentially reset some part of the system, and we don't want that.
            var cmdlist = AllNetshCommands.GetCommands(AllNetshCommands.CommandType.Show);

            var requireList = CommandInfo.GetAllMissingSettersFor(ci, cmdlist, CurrArgumentSettings);
            foreach (var requireci in requireList)
            {
                // Do the commands on the list of missing items. Note that there's a strong assumption that
                // the list is one level deep; there's no place where A depends on B depends on C.
                await DoCommandAsyncRaw(requireci, CommandOptions.SuppressFlash); // always suppress the flash for getting these values
            }

            // Now run the command for real
            await DoCommandAsyncRaw(ci, commandOptions);
        }
        public void Log(string str)
        {
            Console.WriteLine(str);
        }

        public void SetCommand(string str)
        {
            uiCommand.Text = str;
        }
        public void SetCount(string str)
        {
            uiCount.Text = str;
        }
        public void SetUIIssues(string str)
        {
            uiIssues.Text = str;
        }

        public void DoAddToText(string str)
        {
            uiCommandControl.DoAddToText(str);
        }
        public async Task DoCommandAsyncRaw(CommandInfo ci, CommandOptions commandOptions)
        {
            uiCommandControl.Visibility = Visibility.Visible;
            await uiCommandControl.DoCommandAsyncRaw(ci, commandOptions);
        }

        private void ToggleOutputOrTable()
        {
            uiCommandControl?.ToggleOutputOrTable();
        }

        /// <summary>
        /// Sets up the UX to show either the output or the table. But is a little smart; won't
        /// show the table unless it's actually possible to see something
        /// </summary>
        /// <param helpFileName="value"></param>
        private void ShowOutputOrTable(CommandOutputControl.ShowWhat value)
        {
            uiCommandControl.ShowOutputOrTable(value);
        }


        private void DoInitializeCommonArguments()
        {
            CurrArgumentSettings.SetValueList("Level", new List<ArgumentSettingValue>() { new ArgumentSettingValue("normal"), new ArgumentSettingValue("verbose") });
            CurrArgumentSettings.SetValueList("Store", new List<ArgumentSettingValue>() { new ArgumentSettingValue("active"), new ArgumentSettingValue("persistent") });
            CurrArgumentSettings.SetValueList("TestHost", new List<ArgumentSettingValue>() { new ArgumentSettingValue("connectivity.office.com"), new ArgumentSettingValue("microsoft.com"), new ArgumentSettingValue("testconnectivity.microsoft.com"), new ArgumentSettingValue("www.msftconnecttest.com"), new ArgumentSettingValue("msftncsi.com") });
            CurrArgumentSettings.SetValueList("Protocol", new List<ArgumentSettingValue>() { new ArgumentSettingValue("tcp"), new ArgumentSettingValue("udp") });
            CurrArgumentSettings.SetValueList("Parser", new List<ArgumentSettingValue>() { new ArgumentSettingValue("DashLine"), new ArgumentSettingValue("Indent"), new ArgumentSettingValue("List") });
            CurrArgumentSettings.SetValueList("ITSSTemplate", new List<ArgumentSettingValue>() {
                new ArgumentSettingValue("automatic"),
                new ArgumentSettingValue("datacenter"),
                new ArgumentSettingValue("compat"),
                new ArgumentSettingValue("internet"),
                new ArgumentSettingValue("internetcustom"),
                new ArgumentSettingValue("datacentercustom"),
                new ArgumentSettingValue("custom"),
            });

            CurrArgumentSettings.SetCurrent("Level", CurrArgumentSettings.GetValue("Level", "verbose"));
            CurrArgumentSettings.SetCurrent("ITSSTemplate", CurrArgumentSettings.GetValue("ITSSTemplate", "internet"));
        }

        private void DoSetupCommonMenu(string name)
        {
            var list = CurrArgumentSettings.GetValueList(name);
            var mi = new MenuItem() { Header = name, Tag=name };
            mi.Click += Mi_Click;
            uiMenu_Parameters_Common.Items.Add(mi);
        }

        private void Mi_Click(object sender, RoutedEventArgs e)
        {
            var name = ((sender as MenuItem)?.Tag as string) ?? "";
            var dlg = new ArgumentSettingDialog(CurrArgumentSettings, name);
            if (!dlg.InitOk) return; // NOTE: show something to user?
            var result = dlg.ShowDialog();
        }




        private void OnMenu_File_Exit(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void OnMenu_Show_Tag(object sender, RoutedEventArgs e)
        {
            var tag = (sender as MenuItem)?.Tag as string;
            if (tag == null) tag = "";
            DoSetMenuWithTag(tag, AllNetshCommands.CommandType.Show);
        }

        private void OnMenu_Reset_Tag(object sender, RoutedEventArgs e)
        {
            var tag = (sender as MenuItem)?.Tag as string;
            if (tag == null) tag = "";
            DoSetMenuWithTag(tag, AllNetshCommands.CommandType.Reset);
        }

        private void OnMenu_Show_Help_Check(object sender, RoutedEventArgs e)
        {
            UP.CurrUserPrefs.ShowHelp = true;
        }

        private void OnMenu_Show_Help_Uncheck(object sender, RoutedEventArgs e)
        {
            UP.CurrUserPrefs.ShowHelp = false;
        }




        private void OnMenu_Help_Help(object sender, RoutedEventArgs e)
        {
            ShowHelp("/Netshg_help.md");
        }

        MarkdownPipeline? mdpipe = null;
        string lastHelpFile = "";
        public void Help_Remove()
        {
            uiHelpMD.Visibility = Visibility.Collapsed;
        }
        private void ShowHelp(string helpFileName)
        {
            var isdifferent = helpFileName != lastHelpFile;
            lastHelpFile = helpFileName;
            if (uiHelpMD.Visibility == Visibility.Visible && !isdifferent)
            {
                Help_Remove();
                return;
            }

            uiHelpMD.Visibility = Visibility.Visible;
            uiCommandControl.Visibility = Visibility.Collapsed;
            Uri uri = new Uri(helpFileName, UriKind.Relative);
            StreamResourceInfo commands = Application.GetContentStream(uri);
            var sr = new StreamReader(commands.Stream);
            var markdown = sr.ReadToEnd();
            var version = Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString();
            markdown = markdown.Replace("{VERSION}", version);

            if (mdpipe == null)
            {
                mdpipe = new MarkdownPipelineBuilder()
                    .UseSupportedExtensions()
                    .Build();
            }
            if (mdpipe == null) return;

            var xaml = Markdig.Wpf.Markdown.ToXaml(markdown, mdpipe);
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xaml)))
            {
                using (var reader = new XamlXmlReader(stream, new MyXamlSchemaContext()))
                {
                    if (XamlReader.Load(reader) is FlowDocument document)
                    {
                        uiHelpMD.Document = document;
                    }
                }
            }

        }
        private void OnMenu_Help_Versions(object sender, RoutedEventArgs e)
        {
            ShowHelp("/Netshg_help_versions.md");
        }
        private void OnMenu_Help_Shortcuts(object sender, RoutedEventArgs e)
        {
            var w = new HelpKeyboardShortcutWindow();
            w.Show();
        }

        System.Timers.Timer? CurrRepeatTimer = null; // new Timer(OnTimerCallback);
        private void OnMenuRepeatStart(object sender, RoutedEventArgs e)
        {
            OnMenuRepeatStop(sender, e); // always stop first.

            var tagString = (sender as MenuItem)?.Tag as string ?? "";
            CurrRepeatTimeInSeconds = Int32.Parse(tagString);

            if (CurrRepeatTimer == null)
            {
                CurrRepeatTimer = new System.Timers.Timer();
                CurrRepeatTimer.Elapsed += OnTimerCallback;
            }
            CurrRepeatTimer.Interval = CurrRepeatTimeInSeconds * 1000; // convert from seconds to milliseconds
            CurrRepeatTimer.AutoReset = true;
            uiMenuRepeatStop.IsEnabled = true;
            uiRepeat.Text = "REPEATING";
            CurrRepeatTimer.Start();
        }

        private bool AmRepeating()
        {
            var retval = (CurrRepeatTimer != null && CurrRepeatTimer.Enabled == true);
            return retval;
        }
        public void OnMenuRepeatStop(object sender, RoutedEventArgs e)
        {
            if (CurrRepeatTimer == null) return; // can't be more stopped than this!
            CurrRepeatTimer.Stop(); // Sets enabled to false
            uiMenuRepeatStop.IsEnabled = false;
            uiRepeat.Text = "";
        }

        private void OnTimerCallback(object? state, ElapsedEventArgs e)
        {
            // Need to run on the UI thread
            if (AmDoCommand) return; // e.g., netsh wlan show all is very slow and doesn't work in 1 second
            this.Dispatcher.BeginInvoke(new Action(async () => 
            {
                if (LastCommand == null) return;
                await DoCommandAsync(LastCommand, CommandOptions.SuppressFlash);
            }));
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = (sender as TextBox)?.Text ?? "";
            DoSetMenuWithSearch(searchText);
        }
    }

    // See https://github.com/Kryptos-FR/markdig.wpf/blob/develop/src/Markdig.Xaml.SampleApp/MainWindow.xaml.cs
    class MyXamlSchemaContext : XamlSchemaContext
    {
        public override bool TryGetCompatibleXamlNamespace(string xamlNamespace, out string compatibleNamespace)
        {
            if (xamlNamespace.Equals("clr-namespace:Markdig.Wpf", StringComparison.Ordinal))
            {
                compatibleNamespace = $"clr-namespace:Markdig.Wpf;assembly={Assembly.GetAssembly(typeof(Markdig.Wpf.Styles))?.FullName}";
                return true;
            }
            return base.TryGetCompatibleXamlNamespace(xamlNamespace, out compatibleNamespace);
        }
    }
}
