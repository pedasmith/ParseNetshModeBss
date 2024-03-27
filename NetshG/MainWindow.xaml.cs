using Markdig;
//using Markdig.Wpf;
using Neo.Markdig.Xaml;
using ParseNetshModeBss; // to get the utilities classes!
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Resources;
using Utilities;
using static NetshG.CanDoCommand;


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
        void Help_Remove();
        void Log(string str);
        void SetAmDoCommand(bool value);
        /// <summary>
        /// Sets the command string UX as a convenience for the user
        /// </summary>
        void SetCommandTitle(string str);
        void SetCount(string str);
        void SetUIIssues(string str);
        ArgumentSettings GetCurrArgumentSettings();

    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, AddToText, UXCommands
    {
        private CommandOutputControl? CurrCommandControl { get { return uiHistoryControl.GetCurrentControl() as CommandOutputControl; } }
        // DisplayOptions CurrDisplayOptions { get; } = new DisplayOptions();

        public MainWindow()
        {
            InitializeComponent();
            uiHistoryControl.HistoryPanel = uiCommandPanel;
            // DisplayOptions CurrDisplayOptions = new DisplayOptions();

            this.Loaded += MainWindow_Loaded;

            UP.Restore();
            CheckAllMenuCorrectly(UP.CurrUserPrefs.CmdType, UP.CurrUserPrefs.Tags);

            KeyDescriptions.Clear();

            // ^C
            CommandAdd(Key.C, ModifierKeys.Control, OnCopy, "^C", "Copy data from output or table");

            // Alt-F4 for Exit
            CommandAdd(Key.F1, ModifierKeys.None, OnMenu_Help_Help, "F1", "Show help file");
            CommandAdd(Key.F4, ModifierKeys.Alt, OnMenu_File_Exit, "ALT+F4", "Exit program");
            CommandAdd(Key.F5, ModifierKeys.None, OnRepeat, "F5", "Repeat command");

            // Arrows HOME and END for history
            CommandAdd(Key.Left, ModifierKeys.None, OnHistoryLeft, "Left Arrow", "See previous item");
            CommandAdd(Key.Right, ModifierKeys.None, OnHistoryRight, "Right Arrow", "See next item");
            CommandAdd(Key.Home, ModifierKeys.None, OnHistoryStart, "Home", "Go to first item");
            CommandAdd(Key.End, ModifierKeys.None, OnHistoryEnd, "End", "Go to last item");
            CommandAdd(Key.Delete, ModifierKeys.None, OnHistoryDelete, "Delete", "Delete current item");

            // Specialized
            CommandAdd(Key.A, ModifierKeys.Alt, (s, e) => { DoSetMenuWithTag("#all"); }, "ALT+A", "Show all commands in command list");
            CommandAdd(Key.C, ModifierKeys.Alt, (s, e) => { DoSetMenuWithTag("#common"); }, "ALT+C", "Show common command in command list");
            CommandAdd(Key.O, ModifierKeys.Alt, (s, e) => { ShowOutputOrTable(DisplayOptions.ShowWhat.Output); }, "ALT-O", "Show output as text, not as table");
            //CommandAdd(Key.R, ModifierKeys.Alt, OnRepeat, "ALT+R", "Repeat command"); Removed; it's part of the targetMenu now. But still good to tell user
            KeyDescriptions.Add(new HelpDescription("ALT+R R", "Repeat command"));
            KeyDescriptions.Add(new HelpDescription("ALT+R S", "Repeat command every second"));
            KeyDescriptions.Add(new HelpDescription("ALT+R M", "Repeat command every minute"));
            KeyDescriptions.Add(new HelpDescription("ALT+R H", "Repeat command every hour"));

            CommandAdd(Key.T, ModifierKeys.Alt, (s, e) => { ShowOutputOrTable(DisplayOptions.ShowWhat.Table); }, "ALT+T", "Show output as table, not as text");
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

        /// <summary>
        /// LastCommand is used by the Repeat command.
        /// </summary>
        CommandInfo? LastCommand = null;






        int CurrRepeatTimeInSeconds = 0; // not repeating


        #region KEYSTROKE_AND_MENU_METHODS

        private void CommandAdd(Key key, ModifierKeys mod, ExecutedRoutedEventHandler handler, string shortcut, string description)
        {
            RoutedCommand cmdOutput = new RoutedCommand();
            cmdOutput.InputGestures.Add(new KeyGesture(key, mod));
            CommandBindings.Add(new CommandBinding(cmdOutput, handler));

            KeyDescriptions.Add(new HelpDescription(shortcut, description));
        }

        public void OnCopy(object sender, RoutedEventArgs e)
        {
            CurrCommandControl?.OnCopy(sender, e);
        }

        private void OnMenu_File_Exit(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void OnMenu_Show_Tag(object sender, RoutedEventArgs e)
        {
            // Part of the "Show" targetMenu.
            var commandType = AllNetshCommands.CmdType.Show;

            var mi = sender as MenuItem;
            var tag = mi?.Tag as string;
            if (mi == null || tag == null) return;
            CheckAllMenuCorrectly(commandType, tag);
            DoSetMenuWithTag(tag, commandType);
        }


        private void OnMenu_Reset_Tag(object sender, RoutedEventArgs e)
        {
            // Part of the "Show" targetMenu.
            var commandType = AllNetshCommands.CmdType.Reset;

            var mi = sender as MenuItem;
            var tag = mi?.Tag as string;
            if (mi == null || tag == null) return;
            CheckAllMenuCorrectly(commandType, tag);
            DoSetMenuWithTag(tag, commandType);
        }


        private void CheckAllMenuCorrectly(AllNetshCommands.CmdType commandType, string tag)
        {
            var targetMenu = uiMenuShow;
            var menuList = new List<MenuItem>() { uiMenuReset, uiMenuShow };
            switch (commandType)
            {
                case AllNetshCommands.CmdType.Reset: targetMenu = uiMenuReset; break;
                case AllNetshCommands.CmdType.Show: targetMenu = uiMenuShow; break;
            }
            if (tag == null) return;


            foreach (var menu in menuList)
            {
                foreach (var child in menu.Items)
                {
                    if (child is Separator) break; // only uncheck the first items
                    var mm = child as MenuItem;
                    if (mm == null) continue;
                    var ch = (menu == targetMenu) && ((mm.Tag as string) == tag);
                    mm.IsChecked = ch;
                }
            }

        }


        private void OnMenu_Show_Help_Check(object sender, RoutedEventArgs e)
        {
            UP.CurrUserPrefs.ShowHelp = true;
            UP.Save();
        }

        private void OnMenu_Show_Help_Uncheck(object sender, RoutedEventArgs e)
        {
            UP.CurrUserPrefs.ShowHelp = false;
            UP.Save();
        }

        private void OnMenu_Help_Help(object sender, RoutedEventArgs e)
        {
            ShowHelp("/Assets/Help/Netshg_help.md");
        }

        public void OnHistoryLeft(object sender, RoutedEventArgs e)
        {
            uiHistoryControl.Move(-1);
        }

        public void OnHistoryRight(object sender, RoutedEventArgs e)
        {
            uiHistoryControl.Move(1);
        }
        public void OnHistoryEnd(object sender, RoutedEventArgs e)
        {
            uiHistoryControl.MoveTo(int.MaxValue);
        }
        public void OnHistoryDelete(object sender, RoutedEventArgs e)
        {
            uiHistoryControl.DeleteCurrIndex();
        }
        public void OnHistoryStart(object sender, RoutedEventArgs e)
        {
            uiHistoryControl.MoveTo(0);
        }

        public async void OnRepeat(object sender, RoutedEventArgs e)
        {
            await OnRepeatAsync(sender, e);
        }

        private void ToggleOutputOrTable()
        {
            var newvalue = CurrCommandControl?.ToggleOutputOrTable();
            if (newvalue != null)
            {
                UP.CurrUserPrefs.CurrDisplayOptions.CurrShowWhat = newvalue;
            }
        }

        /// <summary>
        /// Sets up the UX to show either the output or the table. But is a little smart; won't
        /// show the table unless it's actually possible to see something
        /// </summary>
        /// <param helpFileName="value"></param>
        private void ShowOutputOrTable(DisplayOptions.ShowWhat value)
        {
            CurrCommandControl?.ShowOutputOrTable(value);
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = (sender as TextBox)?.Text ?? "";
            DoSetMenuWithSearch(searchText);
        }
        #endregion


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

            uiHistoryControl.UXCommands = this; // to set the title

            // Just for testing
            var cmdlist = AllNetshCommands.GetCommands(AllNetshCommands.CmdType.Show);
            CommandInfo.VerifyAllSetters(cmdlist, CurrArgumentSettings);
        }

        AllNetshCommands.CmdType CurrCommandType = AllNetshCommands.CmdType.Show;

        #region UX_SETUP
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

        private void DoSetMenuWithTag(string tags, AllNetshCommands.CmdType menuType = AllNetshCommands.CmdType.Show)
        {
            UP.CurrUserPrefs.Tags = tags;
            UP.Save();
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
        private void DoSetupCommonMenu(string name)
        {
            var list = CurrArgumentSettings.GetValueList(name);
            var mi = new MenuItem() { Header = name, Tag = name };
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

        #endregion UX_SETUP


        #region SELECT_FROM_COMMAND_LIST
        private void OnSelectCommand(object sender, SelectionChangedEventArgs e) // Old way to run commands; now I use MouseUp
        {
        }

        private async void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            OnMenuRepeatStop(sender, e); // If I was repeating, stop it.

            //if (e.AddedItems.Count != 1) return; // only one item selected
            //var fe = e.AddedItems[0] as ContentControl;
            var fe = uiCommandList.SelectedItem as ContentControl;
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
        #endregion



        #region UXCOMMANDS_INTERFACE

        // OnRepeatAsync Help_remove Log SetAmDoCommand SetCount SetUIIssues GetCurrArgumentSettings
        public void Log(string str)
        {
            Console.WriteLine(str);
        }

        bool AmDoCommand = false;
        public void SetAmDoCommand(bool value)
        {
            AmDoCommand = value;
        }

        /// <summary>
        /// Sets the command string UX as a convenience for the user
        /// </summary>
        /// <param name="str"></param>
        public void SetCommandTitle(string str)
        {
            uiCommand.Text = str;
        }

        /// <summary>
        /// Sets the count string UX
        /// </summary>
        /// <param name="str"></param>
        public void SetCount(string str)
        {
            uiCount.Text = str;
        }

        /// <summary>
        /// Sets the list of issue for the command
        /// </summary>
        /// <param name="str"></param>
        public void SetUIIssues(string str)
        {
            uiIssues.Text = str;
        }
        public ArgumentSettings GetCurrArgumentSettings() { return CurrArgumentSettings; }

        #endregion UXCOMMANDS_INTERFACE




        #region ADDTOTEXT_INTERFACE

        public void DoAddToText(string str)
        {
            CurrCommandControl?.DoAddToText(str);
        }

        #endregion ADDTOTEXT_INTERFACE







        #region HELP_UX

        MarkdownPipeline? mdpipe = null;
        string lastHelpFile = "";


        public void Help_Remove()
        {
            uiHelpMD.Visibility = Visibility.Collapsed;
        }
        private void ShowHelp(string helpFileName)
        {
            // Help files are all in Markdown and use the https://github.com/xoofx/markdig toolkit
            // plus the https://github.com/neolithos/NeoMarkdigXaml renderer
            //     via the NUGET package (https://www.nuget.org/packages/Neo.Markdig.Xaml/)
            // It used to use the now-obsolete https://github.com/Kryptos-FR/markdig.wpf renderer

            var ccc = CurrCommandControl;
            var isdifferent = helpFileName != lastHelpFile;
            lastHelpFile = helpFileName;
            if (uiHelpMD.Visibility == Visibility.Visible && !isdifferent)
            {
                Help_Remove();
                return;
            }

            uiHelpMD.Visibility = Visibility.Visible;
            if (ccc != null)
            {
                ccc.Visibility = Visibility.Collapsed;
            }
            uiHistoryControl.Visibility = Visibility.Collapsed;

            //
            // Read in the markdown files
            //
            Uri uri = new Uri(helpFileName, UriKind.Relative);
            StreamResourceInfo commands = Application.GetContentStream(uri);
            var sr = new StreamReader(commands.Stream);
            var markdown = sr.ReadToEnd();
            var version = Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString();
            var content = markdown.Replace("{VERSION}", version);

            //
            // Ensure we have the MarkdownPipeline
            //
            if (mdpipe == null)
            {
                mdpipe = new MarkdownPipelineBuilder()
                    // old renderer used this: .UseSupportedExtensions()
                    .Build();
            }
            if (mdpipe == null) return;

            var doc = MarkdownXaml.ToFlowDocument(content, mdpipe);
            uiHelpMD.Document = doc;

#if OBSOLETE_CODE_2024_03_24
            // This is for the older XAML renderer https://github.com/Kryptos-FR/markdig.wpf
            // That renderer is marked as archived. It's being replaced because it was
            // throwing exceptions when images are rendered.
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

#endif

        }
        private void OnMenu_Help_Versions(object sender, RoutedEventArgs e)
        {
            ShowHelp("/Assets/Help/Netshg_help_versions.md");
        }
        private void OnMenu_Help_Shortcuts(object sender, RoutedEventArgs e)
        {
            var w = new HelpKeyboardShortcutWindow();
            w.Show();
        }
#endregion HELP_UX


        #region REPEAT
        System.Timers.Timer? CurrRepeatTimer = null; // new Timer(OnTimerCallback);


        /// <summary>
        /// Called either from OnRepeat from e.g., key or reflected up from the current CommandOutputControl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
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
        #endregion REPEAT


        #region ACTUALLY_RUN_COMMAND
        /// <summary>
        /// Runs a command, first calling all the required setup commands (e.g., a command to show an adapter 
        /// first needs to have the list of adapters set up)
        /// </summary>
        /// <param name="ci"></param>
        /// <param name="commandOptions"></param>
        /// <returns></returns>
        public async Task DoCommandAsync(CommandInfo ci, CommandOptions commandOptions = CommandOptions.None)
        {


            // We know we have to use the "Show" commands to get the data. Any other list
            // will potentially reset some part of the system, and we don't want that.
            var cmdlist = AllNetshCommands.GetCommands(AllNetshCommands.CmdType.Show);

            var requireList = CommandInfo.GetAllMissingSettersFor(ci, cmdlist, CurrArgumentSettings);
            var ccc = new CommandOutputControl(this, UP.CurrUserPrefs.CurrDisplayOptions);

            // Add the history early; it looks nicer that way.
            uiHistoryControl.AddCurrentControl(ccc, ci.Title);
            ccc.Visibility = Visibility.Visible;
            uiHistoryControl.Visibility = Visibility.Visible;

            // Do the commands on the list of missing items. Note that there's a strong assumption that
            // the list is one level deep; there's no place where A depends on B depends on C.
            foreach (var requireci in requireList)
            {
                // always suppress the flash for getting these values
                await ccc.DoCommandAsyncRaw(requireci, CommandOptions.SuppressFlash);
            }

            // Now run the command for real
            await ccc.DoCommandAsyncRaw(ci, commandOptions);
        }

        #endregion ACTUALLY_RUN_COMMANDS


    }

    // See https://github.com/Kryptos-FR/markdig.wpf/blob/develop/src/Markdig.Xaml.SampleApp/MainWindow.xaml.cs
#if NEVER_EVER_DEFINED
// Removed 2024-03-25 when switching from the old Markdown renderer to the new one.
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
#endif
}
