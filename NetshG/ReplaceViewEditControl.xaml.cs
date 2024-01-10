using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using static NetshG.CanDoCommand;

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

        private async void OnPrevMacro(object sender, RoutedEventArgs e)
        {
            await MoveToNextMacroValueAsync(-1);
        }

        private async void OnNextMacro(object sender, RoutedEventArgs e)
        {
            await MoveToNextMacroValueAsync(1);
        }
        private async Task MoveToNextMacroValueAsync(int delta)
        {
            if (CanDoCommand == null) return;
            if (CurrArgumentSettings == null) return;
            if (CurrCommandInfo == null) return;

            var name = uiReplaceName.Text;
            var currValue = uiReplaceValue.Text;
            var index = CurrArgumentSettings.Find(name, currValue);
            if (index < 0) return;
            index += delta;
            await MoveToMacroValueAsync(index, CommandOptions.None);
        }

        private async Task MoveToMacroValueAsync(int index, CommandOptions commandOptions)
        {
            var name = uiReplaceName.Text;
            var list = CurrArgumentSettings.GetValueList(name);
            if (index < 0 || index >= list.Count) return;

            var newValue = list[index];
            CurrArgumentSettings.SetCurrent(name, newValue);

            uiReplaceValue.Text = newValue.Value;

            // And now re-do the command!
            await CanDoCommand.DoCommandAsync(CurrCommandInfo, commandOptions);

        }

        bool stopStepAll = false;
        private async void OnStepAllGo(object sender, RoutedEventArgs e)
        {
            stopStepAll = false;
            uiStepAllGo.Visibility = Visibility.Collapsed;
            uiStepAllStop.Visibility = Visibility.Visible;
            await Task.Delay(10); // let the UX update

            CanDoCommand.DoClearTable();
            var name = uiReplaceName.Text;
            var list = CurrArgumentSettings.GetValueList(name);
            for (var index=0; index<list.Count && stopStepAll==false; index++)
            {
                await MoveToMacroValueAsync(index, CommandOptions.SuppressFlash | CommandOptions.AppendToTable);
            }

            //uiStepAllGo.Visibility = Visibility.Visible;
            //uiStepAllStop.Visibility = Visibility.Collapsed;

        }

        private void OnStepAllStop(object sender, RoutedEventArgs e)
        {
            stopStepAll = true;
        }
    }
}
