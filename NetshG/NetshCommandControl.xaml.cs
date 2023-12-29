using System.Windows.Controls;

namespace NetshG
{
    /// <summary>
    /// Interaction logic for NetshCommandControl.xaml
    /// </summary>
    public partial class NetshCommandControl : UserControl
    {
        private string _args = "??";
        public string Args { get { return _args; } set { _args = value; uiCommand.Text = Cmd + " " + Args; } }


        private string _cmd = "netsh";
        public string Cmd { get { return _cmd; } set { _cmd = value; uiCommand.Text = Cmd + " " + Args; } }
        public string Help { get; set; } = "netsh ?";

        public CommandInfo CommandInfo { get; set; } = new CommandInfo();

        public NetshCommandControl()
        {
            InitializeComponent();
            uiCommand.Text = Cmd;
        }

        public NetshCommandControl(CommandInfo info)
        {
            InitializeComponent();
            CommandInfo = info;
            Cmd = info.Cmd;
            Args = info.Args + " " + info.Args2;
        }
    }
}
