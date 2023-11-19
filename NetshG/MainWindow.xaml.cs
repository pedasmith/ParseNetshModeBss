using System;
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

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var cmdlist = AllNetshCommands.GetCommands();

            if (cmdlist.Count == 0)
            {
                uiLog.Text = "ERROR: unable to load commands";
                return;
            }
            foreach (var cmd in cmdlist)
            {
                var ctrl = new NetshCommandControl(cmd);
                uiCommandList.Items.Add(ctrl);
            }
        }

        private void OnSelectCommand(object sender, SelectionChangedEventArgs e)
        {
            var program = "netsh";
            if (e.AddedItems.Count != 1) return; // only one item selected
            var fe = e.AddedItems[0] as ContentControl;
            if (fe == null) return; // seriously, it's always a framework element.
            var args = fe.Tag as string;
            if (string.IsNullOrEmpty(args)) 
            {
                args = fe.Content as string;
            }
            var nsc = fe as NetshCommandControl; // not all command are the command control
            if (nsc != null)
            {
                program = nsc.Cmd;
                args = nsc.Args;
            }
            if (string.IsNullOrEmpty(args)) return; // never happens

            uiOutput.Text = "....getting command...";
            if (nsc != null)
            {
                args = CurrArgumentSettings.Replace(args, nsc.CommandInfo.Requires);
            }
            var result = RunCommandLine.RunNetshG(program, args);
            uiOutput.Text = result;

            // Handle the parsing...
            if (nsc != null)
            {
                if (!string.IsNullOrEmpty(nsc.CommandInfo.Sets))
                {
                    var parser = GetParser.Get(nsc.CommandInfo.SetParser);
                    var setList = parser.ParseForValues(result);
                    CurrArgumentSettings.SetValueList(nsc.CommandInfo.Sets, setList);
                }
            }
        }
    }
}
