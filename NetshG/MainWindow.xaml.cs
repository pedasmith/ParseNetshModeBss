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
        }

        ArgumentSettings CurrArgumentSettings = new ArgumentSettings();
        UserPreferences CurrUserPrefs = new UserPreferences();

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DoInitializeCommonArguments();
            DoSetMenuWithTag(CurrUserPrefs.Tags);

            uiMenu_Parameters_Common.Items.Clear();
            DoSetupCommonMenu("Level");
            DoSetupCommonMenu("Store");
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

        public void DoCommand(CommandInfo ci)
        { 
            var program = ci.Cmd;
            var args = ci.Args;
            var argsExtra = ci.ArgsExtra == "" ? "" : " " + ci.ArgsExtra;
            var moreArgs = ci.MoreArgs == "" ? "" : " " + ci.MoreArgs;

            uiOutput.Text = "....getting command...";

            args = CurrArgumentSettings.Replace(args, ci.Requires);
            var argsWithExtraMore = CurrArgumentSettings.Replace(args + argsExtra + moreArgs, ci.Requires);

            uiReplaceList.Children.Clear();
            foreach (var item in ci.Requires)
            {
                var rvec = new ReplaceViewEditControl(this, item, ci, CurrArgumentSettings);
                uiReplaceList.Children.Add(rvec);
            }

            uiOutputScroll.ScrollToHome();
            uiCommand.Text = $"{program} {argsWithExtraMore}";
            var qresult = RunCommandLine.RunNetshG(program, args + " ?");
            var result = RunCommandLine.RunNetshG(program, argsWithExtraMore);
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


        private void DoInitializeCommonArguments()
        {
            CurrArgumentSettings.SetValueList("Level", new List<ArgumentSettingValue>() { new ArgumentSettingValue("normal"), new ArgumentSettingValue("verbose") });
            CurrArgumentSettings.SetValueList("Store", new List<ArgumentSettingValue>() { new ArgumentSettingValue("active"), new ArgumentSettingValue("persistent") });

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
