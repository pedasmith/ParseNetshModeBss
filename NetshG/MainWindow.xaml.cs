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
            var result = RunCommandLine.RunNetshG(program, args);
            uiOutput.Text = result;
        }
    }
}
