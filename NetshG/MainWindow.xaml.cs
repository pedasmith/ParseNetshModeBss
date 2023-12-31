using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using Windows.ApplicationModel.DataTransfer;
using ParseNetshModeBss; // to get the utilities classes!
using Utilities;
using Clipboard = Windows.ApplicationModel.DataTransfer.Clipboard;
using System.Windows.Input;
using System;
using System.Threading.Tasks;

namespace NetshG
{
    public interface CanDoCommand
    {
        void DoCommand(CommandInfo ci);
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, CanDoCommand
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;

            // ^C
            CommandAdd(Key.C, ModifierKeys.Control, OnCopy);

            // Alt-F4 for Exit
            CommandAdd(Key.F1, ModifierKeys.None, OnMenu_Help_Help);
            CommandAdd(Key.F4, ModifierKeys.Alt, OnMenu_File_Exit);
            CommandAdd(Key.F5, ModifierKeys.Alt, OnRepeat);

            // Specialized
            CommandAdd(Key.A, ModifierKeys.Alt, (s, e) => { DoSetMenuWithTag("#all"); });
            CommandAdd(Key.C, ModifierKeys.Alt, (s, e) => { DoSetMenuWithTag("#common"); });
            CommandAdd(Key.O, ModifierKeys.Alt, (s, e) => { ShowOutputOrTable(ShowWhat.Output); });
            CommandAdd(Key.R, ModifierKeys.Alt, OnRepeat);
            CommandAdd(Key.T, ModifierKeys.Alt, (s, e) => { ShowOutputOrTable(ShowWhat.Table); });
            CommandAdd(Key.W, ModifierKeys.Alt, (s, e) => { DoSetMenuWithTag("#wifi"); });

            int nerror = 0;
            nerror += Utilities.StringUtilities.TestCountStrings();
        }


        private void CommandAdd(Key key, ModifierKeys mod, ExecutedRoutedEventHandler handler)
        {
            RoutedCommand cmdOutput = new RoutedCommand();
            cmdOutput.InputGestures.Add(new KeyGesture(key, mod));
            CommandBindings.Add(new CommandBinding(cmdOutput, handler));
        }

        private 

        ArgumentSettings CurrArgumentSettings = new ArgumentSettings();

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DoInitializeCommonArguments();
            DoSetMenuWithTag(UP.CurrUserPrefs.Tags);
            
            uiMenu_Parameters_Common.Items.Clear();
            DoSetupCommonMenu("Level");
            DoSetupCommonMenu("Store");
            DoSetupCommonMenu("Parser");

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
        }

        private void DoSetMenuWithTag(string tags)
        {
            UP.CurrUserPrefs.Tags = tags;
            var cmdlist = AllNetshCommands.GetCommands();

            if (cmdlist.Count == 0)
            {
                uiLog.Text = "ERROR: unable to load commands";
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

        }
        CommandInfo? LastCommand = null;
        private void OnSelectCommand(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return; // only one item selected
            var fe = e.AddedItems[0] as ContentControl;
            if (fe == null) return; // seriously, it's always a framework element.

            var program = "netsh";
            var nsc = fe as NetshCommandControl; // not all command are the command control??
            CommandInfo ci;
            if (nsc != null)
            {
                ci = nsc.CommandInfo;
            }
            else
            {
                ci = new CommandInfo()
                {
                    Cmd = program,
                    Args = "wlan show"
                };
            }
            LastCommand = ci;

            DoCommand(ci);
        }
        TableParse? CurrTableParser = null;

        public async void DoCommand(CommandInfo ci)
        { 
            var program = ci.Cmd;
            var args = ci.Args;
            var argsExtra = ci.Args2 == "" ? "" : " " + ci.Args2;
            var moreArgs = ci.Args5NoUX == "" ? "" : " " + ci.Args5NoUX;
            args = CurrArgumentSettings.Replace(args, ci.Requires);
            var argsWithExtraMore = CurrArgumentSettings.Replace(args + argsExtra + moreArgs, ci.Requires);


            string result = "No results", qresult= "No help results", csv = "";
            ShowWhat showTable = ShowWhat.Output;
            uiProgress.Visibility = Visibility.Visible;
            uiOutput.Text = $"....getting results for {program} {argsWithExtraMore}...";
            ShowOutputOrTable(ShowWhat.Output);
            await Task.Delay(50);
            try
            {

                uiReplaceList.Children.Clear();
                foreach (var item in ci.Requires)
                {
                    var rvec = new ReplaceViewEditControl(this, item, ci, CurrArgumentSettings);
                    uiReplaceList.Children.Add(rvec);
                }

                uiHelpScroll.ScrollToHome();
                uiOutputScroll.ScrollToHome();
                uiTableScroll.ScrollToHome();

                uiCommand.Text = $"{program} {argsWithExtraMore}";
                qresult = await RunCommandLine.RunNetshGAsync(program, args + " " + ci.Help);
                result = await RunCommandLine.RunNetshGAsync(program, argsWithExtraMore);
                if (false && argsWithExtraMore.Contains("mode=bss")) //Note: this is a great place to set the results to a fixed example string!
                {
                    result = ParseIndent.Example1; // Set to fixed Example string for debugging problems.
                }
                var rawResult = result; // for the parser
                if (UP.CurrUserPrefs.ReplaceTabs)
                {
                    if (result.Contains('\t'))
                    {
                        result = "HAS TABS!!\n" + result.Replace("\t", "\\t");
                    }
                }


                // Handle the parsing. Parsing is the act of looking at the data from,
                // e.g., the list of interface and making a list of all interface names.
                // Those name get set into macros.
                if (!string.IsNullOrEmpty(ci.Sets))
                {
                    var macroParser = GetParser.GetMacroParser(ci.SetParser);
                    if (macroParser != null)
                    {
                        var setList = macroParser.ParseForValues(rawResult);
                        CurrArgumentSettings.SetValueList(ci.Sets, setList);
                    }
                }


                //
                // Parse the results
                //
                var tableParserName = ci.TableParser;
                showTable = ShowWhat.Output;
                if (string.IsNullOrEmpty(tableParserName))
                {
                    //tableParserName = "Indent"; 
                    tableParserName = CurrArgumentSettings.GetCurrent("Parser", "Indent").Value;//DBG: TODO: just for debugging
                }
                else
                {
                    showTable = ShowWhat.Table;
                }
                csv = "";
                if (!string.IsNullOrEmpty(tableParserName))
                {
                    CurrTableParser = GetParser.GetTableParser(tableParserName);
                    if (CurrTableParser != null)
                    {
                        CurrTableParser.Parse(rawResult);
                        csv = CurrTableParser.AsCsv();
                    }
                }
            }
            catch (Exception)
            {

            }
            uiProgress.Visibility = Visibility.Collapsed;
            uiHelpGrid.Visibility = UP.CurrUserPrefs.ShowHelp ? Visibility.Visible : Visibility.Collapsed;
            uiHelp.Text = qresult;
            if (result.Trim() == "") result = "\n\n\n\nNo data returned by the command";
            uiOutput.Text = result;
            uiTable.Text = csv;
            if (CurrTableParser != null)
            {
                CurrDataTable = CurrTableParser.GetDataTable();
                uiTableDataGrid.AutoGenerateColumns = true;
                uiTableDataGrid.DataContext = CurrDataTable;

                if (CurrTableParser.Rows.Count ==  0)
                {
                    showTable = ShowWhat.Output;
                }
            }
            else
            {
                CurrDataTable = null;
                uiTableDataGrid.DataContext = null;
            }

            ShowOutputOrTable(showTable);
        }
        DataTable? CurrDataTable = null;
        private void OnAutogeneratedColumn(object sender, System.EventArgs e)
        {
            if (CurrDataTable == null) return;
            if (CurrDataTable.Columns.Count != uiTableDataGrid.Columns.Count)
            {
                return;  // Should not happen -- the visible data grid should match the underlying data table
            }

            for (int i = 0; i < uiTableDataGrid.Columns.Count; i++)
            {
                var caption = CurrDataTable.Columns[i].Caption;
                var col = uiTableDataGrid.Columns[i];
                col.Header = caption;
            }
        }

        enum ShowWhat {  Output, Table };
        private void ShowOutputOrTable(ShowWhat value)
        {
            var allowShowTable = CurrTableParser != null && CurrTableParser.Rows.Count > 0;

            uiOutputScroll.Visibility = value==ShowWhat.Output ? Visibility.Visible: Visibility.Collapsed;
            uiTableDataGrid.Visibility = value == ShowWhat.Table ? Visibility.Visible : Visibility.Collapsed;
            foreach (var item in uiOutputButtons.Children)
            {
                var button = item as Button;
                if (button == null) continue;
                var tag = button.Tag as string;
                if (string.IsNullOrEmpty (tag)) continue;
                var visibility = Visibility.Collapsed;
                switch (value)
                {
                    case ShowWhat.Output:
                        if (tag.Contains("output")) visibility = Visibility.Visible;
                        if (tag.Contains("allowTable") && allowShowTable == true) visibility = Visibility.Visible;
                        button.Visibility = visibility;
                        break;
                    case ShowWhat.Table:
                        if (tag.Contains("table")) visibility = Visibility.Visible;
                        button.Visibility = visibility;
                        break;
                }
            }
        }


        private void DoInitializeCommonArguments()
        {
            CurrArgumentSettings.SetValueList("Level", new List<ArgumentSettingValue>() { new ArgumentSettingValue("normal"), new ArgumentSettingValue("verbose") });
            CurrArgumentSettings.SetValueList("Store", new List<ArgumentSettingValue>() { new ArgumentSettingValue("active"), new ArgumentSettingValue("persistent") });
            CurrArgumentSettings.SetValueList("Protocol", new List<ArgumentSettingValue>() { new ArgumentSettingValue("tcp"), new ArgumentSettingValue("udp") });
            CurrArgumentSettings.SetValueList("Parser", new List<ArgumentSettingValue>() { new ArgumentSettingValue("DashLine"), new ArgumentSettingValue("Indent"), new ArgumentSettingValue("List") });

            CurrArgumentSettings.SetCurrent("Level", CurrArgumentSettings.GetValue("Level", "verbose"));
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
            DoSetMenuWithTag(tag);
        }


        private void OnMenu_Show_Help_Check(object sender, RoutedEventArgs e)
        {
            UP.CurrUserPrefs.ShowHelp = true;
        }

        private void OnMenu_Show_Help_Uncheck(object sender, RoutedEventArgs e)
        {
            UP.CurrUserPrefs.ShowHelp = false;
        }

        private void OnParse(object sender, RoutedEventArgs e)
        {
            var parser = Utilities.ConfigurableParser.Make.Create_MatchSsidEncrypt();
            var lines = uiOutput.Text.Split('\n');
            foreach ( var rawline in lines )
            {
                var line = rawline.TrimEnd();
                if (line.Length == 0 ) continue;
                parser.ParseLine(line);
            }
            var allresults = "";
            foreach (var result in parser.Results )
            {
                allresults += result + "\n";
            }
            var settings = new JsonSerializerSettings() {  Formatting = Formatting.Indented };
            var jsonAll = JsonConvert.SerializeObject(parser, settings);
            var json = JsonConvert.SerializeObject(parser.Commands[0].MatchRule, typeof(Utilities.ConfigurableParser.Rule), settings);
            allresults = allresults + "\nJSON:\n" + json + "\nPARSER:\n" + parser.ToString();
            uiOutput.Text =  allresults + "\n" + uiOutput.Text;
        }

        private void OnRepeat(object sender, RoutedEventArgs e)
        {
            if (LastCommand == null) return;
            DoCommand(LastCommand);
        }

        private void OnCopy(object sender, RoutedEventArgs e)
        {
            if (uiTableDataGrid.Visibility == Visibility.Visible)
            {
                OnCopyForExcel(sender, e);
                return;
            }
            OnCopyText(sender, e);
        }

        private void OnCopyForExcel(object sender, RoutedEventArgs e)
        {
            if (CurrTableParser == null) return;
            var txt = CurrTableParser.AsHtml();
            var htmlFormat = HtmlFormatHelper.CreateHtmlFormat(txt);
            var dp = new DataPackage();
            dp.SetText(txt);
            dp.SetHtmlFormat(htmlFormat);
            dp.Properties.Title = "Netsh Data";
            Clipboard.SetContent(dp);
        }

        private void OnCopyCSV(object sender, RoutedEventArgs e)
        {
            if (CurrTableParser == null) return;
            var txt = CurrTableParser.AsCsv();
            var dp = new DataPackage();
            dp.SetText(txt);
            dp.Properties.Title = "Netsh Data";
            Clipboard.SetContent(dp);
        }

        private void OnCopyText(object sender, RoutedEventArgs e)
        {
            var dp = new DataPackage();
            dp.SetText(uiOutput.Text);
            dp.Properties.Title = "Netsh Data";
            Clipboard.SetContent(dp);
        }

        private void OnShowTable(object sender, RoutedEventArgs e)
        {
            ShowOutputOrTable(ShowWhat.Table);
        }

        private void OnShowOutput(object sender, RoutedEventArgs e)
        {
            ShowOutputOrTable(ShowWhat.Output);
        }

        private void OnMenu_Help_Help(object sender, RoutedEventArgs e)
        {
            var w = new HelpWindow();
            w.Show();
        }
    }
}
