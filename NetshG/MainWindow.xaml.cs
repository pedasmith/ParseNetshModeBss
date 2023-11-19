using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
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


using ParseNetshModeBss; // to get the utilities classes!
using Utilities;

namespace NetshG
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        ArgumentSettings CurrArgumentSettings = new ArgumentSettings();
        UserPreferences CurrUserPrefs = new UserPreferences();

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DoSetMenuWithTag(CurrUserPrefs.Tags);
        }

        private void DoSetMenuWithTag(string tags)
        {
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

            DoCommand(ci);
        }

        private void DoCommand(CommandInfo ci)
        { 
            var program = ci.Cmd;
            var args = ci.Args;

            uiOutput.Text = "....getting command...";
            args = CurrArgumentSettings.Replace(args, ci.Requires);
            if (ci.Requires.Count >= 1)
            {
                var name = ci.Requires[0].Name;
                uiReplaceName.Text = name;
                uiReplaceValue.Text = CurrArgumentSettings.GetCurrent(name, "(not set)");

                uiReplace.Visibility = Visibility.Visible;
            }
            else
            {
                uiReplace.Visibility = Visibility.Collapsed;
            }
            uiOutputScroll.ScrollToHome();
            uiCommand.Text = $"{program} {args}";
            var qresult = RunCommandLine.RunNetshG(program, args + " ?");
            var result = RunCommandLine.RunNetshG(program, args);
            if (CurrUserPrefs.ReplaceTabs)
            {
                if (result.Contains('\t'))
                {
                    result = "HAS TABS!!\n" + result.Replace("\t", "\\t");
                }
            }
            if (CurrUserPrefs.ShowHelp)
            {
                result = qresult + "\n\n\n" + result;
            }
            uiOutput.Text = result;

            // Handle the parsing...
            if (!string.IsNullOrEmpty(ci.Sets))
            {
                var parser = GetParser.Get(ci.SetParser);
                var setList = parser.ParseForValues(result);
                CurrArgumentSettings.SetValueList(ci.Sets, setList);
            }
        }



        private void MoveToNextMacroValue(int delta)
        {
            var name = uiReplaceName.Text;
            var currValue = uiReplaceValue.Text;
            var list = CurrArgumentSettings.GetValueList(name);
            var index = CurrArgumentSettings.Find(name, currValue);
            if (index < 0) return;
            index += delta;
            if (index < 0 || index >= list.Count) return;
            var newValue = list[index];
            CurrArgumentSettings.SetCurrent(name, newValue);

            uiReplaceValue.Text = newValue;

            // And now re-do the command!
            var ncc = uiCommandList.SelectedItem as NetshCommandControl;
            var ci = ncc?.CommandInfo;
            if (ci != null)
            {
                DoCommand(ci);
            }
        }

        private void OnNextMacro(object sender, RoutedEventArgs e)
        {
            MoveToNextMacroValue(1);
        }

        private void OnPrevMacro(object sender, RoutedEventArgs e)
        {
            MoveToNextMacroValue(-1);
        }

        private void OnMenu_File_Exit(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void OnMenu_Show_Tag(object sender, RoutedEventArgs e)
        {
            var tag = (sender as MenuItem)?.Tag as string;
            if (tag == null) tag = "";
            CurrUserPrefs.Tags = tag;
            DoSetMenuWithTag(CurrUserPrefs.Tags);
        }

        private void OnMenu_Show_Help_Check(object sender, RoutedEventArgs e)
        {
            CurrUserPrefs.ShowHelp = true;
        }

        private void OnMenu_Show_Help_Uncheck(object sender, RoutedEventArgs e)
        {
            CurrUserPrefs.ShowHelp = false;
        }
    }
}
