using System.Windows;
using System.Windows.Controls;

namespace NetshG
{
    /// <summary>
    /// Interaction logic for ReplaceViewEditControl.xaml
    /// </summary>
    public partial class ReplaceViewEditControl : UserControl
    {
        private CanDoCommand CanDoCommand;
        private ArgumentSettings CurrArgumentSettings;
        private CommandInfo CurrCommandInfo;

        public ReplaceViewEditControl(CanDoCommand cdc, CommandRequire ri, CommandInfo info, ArgumentSettings settings)
        {
            CanDoCommand = cdc;
            CurrArgumentSettings = settings;
            CurrCommandInfo = info;
            InitializeComponent();
            uiReplaceName.Text = ri.Name;
            var value = CurrArgumentSettings.GetCurrent(ri.Name, "(not set)");
            uiReplaceValue.Text = value.Value;
            var user = value.UserString;
            if (user == value.Value) user = ""; // not really set, so don't show it.
            uiReplaceUserValue.Text = user;
        }

        private void OnPrevMacro(object sender, RoutedEventArgs e)
        {
            MoveToNextMacroValue(-1);

        }

        private void OnNextMacro(object sender, RoutedEventArgs e)
        {
            MoveToNextMacroValue(1);
        }
        private void MoveToNextMacroValue(int delta)
        {
            if (CanDoCommand == null) return;
            if (CurrArgumentSettings == null) return;
            if (CurrCommandInfo == null) return;

            var name = uiReplaceName.Text;
            var currValue = uiReplaceValue.Text;
            var list = CurrArgumentSettings.GetValueList(name);
            var index = CurrArgumentSettings.Find(name, currValue);
            if (index < 0) return;
            index += delta;
            if (index < 0 || index >= list.Count) return;
            var newValue = list[index];
            CurrArgumentSettings.SetCurrent(name, newValue);

            uiReplaceValue.Text = newValue.Value;

            // And now re-do the command!
            CanDoCommand.DoCommand(CurrCommandInfo);
        }

    }
}
