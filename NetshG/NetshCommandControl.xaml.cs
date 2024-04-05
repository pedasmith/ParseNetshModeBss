using System.Windows;
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

        public bool IsFavorite { get; set; } = false;

        public NetshCommandControl()
        {
            InitializeComponent();
            uiCommand.Text = Cmd;
        }

        public NetshCommandControl(CommandInfo info, bool isFavorite)
        {
            InitializeComponent();
            IsFavorite = isFavorite;
            CommandInfo = info;
            Cmd = info.Cmd;
            Args = info.Args + " " + info.Args2;

            SetFavorite(isFavorite);
        }

        public void SetFavorite(bool isFavorite)
        {
            IsFavorite = isFavorite;
            uiCommand.FontWeight = IsFavorite ? FontWeights.Bold : FontWeights.Regular;
        }
    }
}
