﻿using ParseNetshModeBss;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utilities;
using Windows.ApplicationModel.DataTransfer;
using static NetshG.CanDoCommand;
using Windows.UI.Popups;
using Clipboard = Windows.ApplicationModel.DataTransfer.Clipboard;

namespace NetshG
{
    /// <summary>
    /// Interaction logic for CommandOutputControl.xaml
    /// </summary>
    public partial class CommandOutputControl : UserControl, CanDoCommand
    {

        //ShowWhat? CurrShowWhat = null; // TODO: use the DisplayOptions instead!
        public DisplayOptions? CurrDisplayOptions { get; set; } = null;
        public UXCommands? UXCommands { get; set; } = null;
        /// <summary>
        /// Used to make the grid actually work -- it's needed for the callback on creating columns
        /// </summary>
        DataTable? CurrDataTable = null;


        /// <summary>
        /// Used by e.g., copy, and to decide if we can show a table
        /// </summary>
        TableParse? CurrTableParser = null;
        string CurrTableParserName = "";
        public CommandOutputControl()
        {
            InitializeComponent();
        }
        public CommandOutputControl(UXCommands commands, DisplayOptions displayOptions)
        {
            InitializeComponent();
            UXCommands = commands;
            CurrDisplayOptions = displayOptions;
        }
        public async void OnRepeat(object sender, RoutedEventArgs e)
        {
            if (UXCommands != null)
            {
                await UXCommands.OnRepeatAsync(sender, e);
            }
        }

        public void DoAddToText(string str)
        {
            uiOutput.Text += str;
        }

        public void DoClearTable()
        {
            CurrTableParserName = ""; // blank it out so that the next command clears the table.
            uiOutput.Text = "";
        }

        public void OnCopy(object sender, RoutedEventArgs e)
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
            ShowOutputOrTable(DisplayOptions.ShowWhat.Table);
        }

        private void OnShowOutput(object sender, RoutedEventArgs e)
        {
            ShowOutputOrTable(DisplayOptions.ShowWhat.Output);
        }
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
        public async Task DoCommandAsync(CommandInfo ci, CommandOptions commandOptions = CommandOptions.None)
        {
            if (UXCommands == null) return;

            // We know we have to use the "Show" commands to get the data. Any other list
            // will potentially reset some part of the system, and we don't want that.
            var cmdlist = AllNetshCommands.GetCommands(AllNetshCommands.CommandType.Show);

            var requireList = CommandInfo.GetAllMissingSettersFor(ci, cmdlist, UXCommands.GetCurrArgumentSettings());
            foreach (var requireci in requireList)
            {
                // Do the commands on the list of missing items. Note that there's a strong assumption that
                // the list is one level deep; there's no place where A depends on B depends on C.
                await DoCommandAsyncRaw(requireci, CommandOptions.SuppressFlash); // always suppress the flash for getting these values
            }

            // Now run the command for real
            await DoCommandAsyncRaw(ci, commandOptions);
        }
        public async Task DoCommandAsyncRaw(CommandInfo ci, CommandOptions commandOptions)
        {
            // This method is all about the UX needed to run the command. The command is finally
            // run with the RunCommandLine.RunNetshGAsync method
            //
            if (UXCommands == null) return; // it's never null.

            UXCommands.SetAmDoCommand(true);
            var program = ci.Cmd;
            var args = ci.Args;
            var args2 = ci.Args2 == "" ? "" : " " + ci.Args2;
            var args5 = ci.Args5NoUX == "" ? "" : " " + ci.Args5NoUX;
            args = UXCommands.GetCurrArgumentSettings().Replace(args, ci.Requires);
            var argsWithExtraMore = UXCommands.GetCurrArgumentSettings().Replace(args + args2 + args5, ci.Requires);

            ci.Title = $"{program} {args}"; // make it short and sweet.
            string result = "No results", result_help = "No help results", csv = "";
            DisplayOptions.ShowWhat showWhat = DisplayOptions.ShowWhat.Output;
            if (CurrDisplayOptions != null && CurrDisplayOptions.CurrShowWhat != null)
            {
                showWhat = (DisplayOptions.ShowWhat)CurrDisplayOptions.CurrShowWhat;
            }
            bool haveNoPreferenceForShow = CurrDisplayOptions == null || CurrDisplayOptions.CurrShowWhat == null;
            uiProgress.Visibility = Visibility.Visible;
            uiCommandOutput.Visibility = Visibility.Visible;
            UXCommands?.Help_Remove(); // make the help go away

            UXCommands?.Log($"DoCommand: {program} {args} {args2} {args5}");
            if (!commandOptions.HasFlag(CommandOptions.SuppressFlash))
            {
                uiOutput.Text = $"\n\n\n\n....getting results for {program} {argsWithExtraMore}...";
                ShowOutputOrTable(DisplayOptions.ShowWhat.Output);
                await Task.Delay(50);
            }


            try
            {
                bool haveCorrectRequiresUX =
                    commandOptions.HasFlag(CommandOptions.KeepRepeatButtons)
                    && uiReplaceList.Children.Count > 0;
                if (!haveCorrectRequiresUX)
                {
                    uiReplaceList.Children.Clear();
                    foreach (var item in ci.Requires)
                    {
                        if (UXCommands != null)
                        {
                            var rvec = new ReplaceViewEditControl(this, item, ci, UXCommands.GetCurrArgumentSettings());
                            uiReplaceList.Children.Add(rvec);
                        }
                    }
                }

                uiHelpScroll.ScrollToHome();
                uiOutputScroll.ScrollToHome();
                uiTableScroll.ScrollToHome();


                // Fill in the help text (if appropriate)
                UXCommands?.SetCommand(ci.Title);
                if (ci.Help.Contains("#nohelp"))
                {
                    // Example: the explorer.exe ms-availablenetworks
                    if (!string.IsNullOrEmpty(ci.HelpText))
                    {
                        result_help = ci.HelpText;
                    }
                    else
                    {
                        result_help = "No help is available for this command";
                    }
                }
                else
                {
                    result_help = await RunCommandLine.RunNetshGAsync(program, args + " " + ci.Help);
                    if (!string.IsNullOrEmpty(ci.HelpText))
                    {
                        result_help = ci.HelpText + "\n\n" + result_help;
                    }
                }
                // Set this early so that for long-running commands the user has something to look
                // at (it also helps with reducing the screen flashing)
                uiHelp.Text = result_help;

                //
                // Actually run the command!
                //
                result = await RunCommandLine.RunNetshGAsync(program, argsWithExtraMore, this as AddToText);
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


                // Handle the "Set" parsing. E.G., Look at the data from, list of interface and making a
                // list of all interface names. Those helpFileName get set into macros.
                if (!string.IsNullOrEmpty(ci.Sets))
                {
                    var macroParser = GetParser.GetMacroParser(ci.SetParser);
                    if (macroParser != null)
                    {
                        var setList = macroParser.ParseForValues(rawResult);
                        UXCommands?.GetCurrArgumentSettings()?.SetValueList(ci.Sets, setList);
                    }
                }


                //
                // Parse the results
                //
                var tableParserName = ci.TableParser;
                if (string.IsNullOrEmpty(tableParserName))
                {
                    tableParserName = UXCommands?.GetCurrArgumentSettings()?.GetCurrent("Parser", "Indent")?.Value;// Used while the parsers are figured out. 
                    // Goal is for all output type to have a parser; that will be a challenge given the nature of the output.
                }
                else
                {
                    if (haveNoPreferenceForShow)
                    {
                        showWhat = DisplayOptions.ShowWhat.Table;
                    }
                }
                csv = "";
                if (!string.IsNullOrEmpty(tableParserName))
                {
                    if (!commandOptions.HasFlag(CommandOptions.AppendToTable)
                        || CurrTableParserName != tableParserName)
                    {
                        CurrTableParser = GetParser.GetTableParser(tableParserName);
                        CurrTableParserName = tableParserName;
                    }

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
            if (result.Trim() == "") result = "\n\n\n\nNo data returned by the command";
            if (commandOptions.HasFlag(CommandOptions.AppendToTable) && !string.IsNullOrEmpty(uiOutput.Text))
            {
                uiOutput.Text = uiOutput.Text + "\n\n\n" + result;
            }
            else
            {
                uiOutput.Text = result;
            }
            uiTable.Text = csv;
            if (CurrTableParser != null)
            {
                CurrDataTable = CurrTableParser.GetDataTable();
                uiTableDataGrid.AutoGenerateColumns = true;
                uiTableDataGrid.DataContext = CurrDataTable;

                if (CurrTableParser.Rows.Count == 0)
                {
                    // It doesn't matter what your preference was, we can't show a table
                    showWhat = DisplayOptions.ShowWhat.Output;
                }
            }
            else
            {
                CurrDataTable = null;
                uiTableDataGrid.DataContext = null;
            }

            ShowOutputOrTable(showWhat);

            // Update the status
            UXCommands?.SetUIIssues(ci.Issues);
            if (CurrDataTable != null && CurrTableParser?.Rows.Count > 0)
            {
                UXCommands?.SetCount($"{CurrDataTable.Rows.Count} rows {CurrDataTable.Columns.Count} cols {result.Length} chars");
            }
            else
            {
                UXCommands?.SetCount($"{result.Length} chars");
            }
            UXCommands?.SetAmDoCommand(false);
        }
        public DisplayOptions.ShowWhat? ToggleOutputOrTable()
        {
            if (CurrDisplayOptions == null || CurrDisplayOptions.CurrShowWhat == null) return null;
            var retval = DisplayOptions.ShowWhat.Output;
            switch (CurrDisplayOptions.CurrShowWhat)
            {
                case DisplayOptions.ShowWhat.Output: retval = DisplayOptions.ShowWhat.Table; break;
                case DisplayOptions.ShowWhat.Table: retval = DisplayOptions.ShowWhat.Output; break;
            }
            ShowOutputOrTable(retval);
            return retval;
        }


        /// <summary>
        /// Sets up the UX to show either the output or the table. But is a little smart; won't
        /// show the table unless it's actually possible to see something
        /// </summary>
        /// <param helpFileName="value"></param>
        public void ShowOutputOrTable(DisplayOptions.ShowWhat value)
        {
            //
            // Smarts: decide if the table can be shown or not.
            //
            var allowShowTable = CurrTableParser != null && CurrTableParser.Rows.Count > 0;
            if (value == DisplayOptions.ShowWhat.Table && !allowShowTable)
            {
                value = DisplayOptions.ShowWhat.Output;
            }
            if (CurrDisplayOptions != null) CurrDisplayOptions.CurrShowWhat = value;

            // 
            // Now display the output or table
            //
            uiOutputScroll.Visibility = value == DisplayOptions.ShowWhat.Output ? Visibility.Visible : Visibility.Collapsed;
            uiTableDataGrid.Visibility = value == DisplayOptions.ShowWhat.Table ? Visibility.Visible : Visibility.Collapsed;
            foreach (var item in uiOutputButtons.Children)
            {
                var button = item as Button;
                if (button == null) continue;
                var tag = button.Tag as string;
                if (string.IsNullOrEmpty(tag)) continue;
                var visibility = Visibility.Collapsed;
                switch (value)
                {
                    case DisplayOptions.ShowWhat.Output:
                        if (tag.Contains("output")) visibility = Visibility.Visible;
                        if (tag.Contains("allowTable") && allowShowTable == true) visibility = Visibility.Visible;
                        button.Visibility = visibility;
                        break;
                    case DisplayOptions.ShowWhat.Table:
                        if (tag.Contains("table")) visibility = Visibility.Visible;
                        button.Visibility = visibility;
                        break;
                }
            }
        }

    }
}
