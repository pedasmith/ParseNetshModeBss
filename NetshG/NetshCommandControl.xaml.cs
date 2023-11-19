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

namespace NetshG
{
    /// <summary>
    /// Interaction logic for NetshCommandControl.xaml
    /// </summary>
    public partial class NetshCommandControl : UserControl
    {
        private string _args = "??";
        public string Args { get { return _args; } set { _args = value; uiCommand.Text = value; } }


        private string _cmd = "netsh";
        public string Cmd { get { return _cmd; } set { _cmd = value; uiCommand.Text = value; } }
        public string Help { get; set; } = "netsh ?";

        public NetshCommandControl()
        {
            InitializeComponent();
            uiCommand.Text = Cmd;
        }
    }
}
