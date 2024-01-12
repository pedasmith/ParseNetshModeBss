using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Xml.Linq;
using Utilities;
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

            var valuelist = CurrArgumentSettings.GetValueList(ri.Name);
            var currValue = CurrArgumentSettings.GetCurrent(ri.Name, "(not set)");
            ComboBoxItem? selected = null;
            foreach (var value in valuelist)
            {
                var user =  value.UserString;
                if (user == value.Value) user = ""; // not really set, so don't show it.
                var str = $"{value.Value} {user}";
                var cbi = new ComboBoxItem()
                {
                    Content = str,
                    Tag = value,
                };
                uiValueList.Items.Add(cbi);
                if (value == currValue)
                {
                    selected = cbi;
                }
            }
            uiValueList.SelectedItem = selected;
        }

        private void OnPrevMacro(object sender, RoutedEventArgs e)
        {
            var index = uiValueList.SelectedIndex - 1;
            MoveToMacroValue(index, CommandOptions.KeepRepeatButtons);
        }

        private void OnNextMacro(object sender, RoutedEventArgs e)
        {
            var index = uiValueList.SelectedIndex + 1;
            MoveToMacroValue(index, CommandOptions.KeepRepeatButtons);
        }
        //string CurrValue = "";
        CommandOptions CurrCommandOptions = CommandOptions.None;
        private void MoveToMacroValue(int index, CommandOptions commandOptions)
        {
            int count = uiValueList.Items.Count;
            if (count == 0) return;
            CurrCommandOptions = commandOptions;
            // The loop-around logic. Will actually only ever go once.
            while (index < 0) index += count;
            while (index >= count) index -= count;
            uiValueList.SelectedIndex = index;
        }

        bool stopStepAll = false;
        private async void OnStepAllGo(object sender, RoutedEventArgs e)
        {
            stopStepAll = false;
            uiStepAllGo.Visibility = Visibility.Collapsed;
            uiStepAllStop.Visibility = Visibility.Visible;
            CurrCommandOptions = CommandOptions.None;
            await Task.Delay(10); // let the UX update

            CanDoCommand.DoClearTable();
            var name = uiReplaceName.Text;
            var list = CurrArgumentSettings.GetValueList(name);
            for (var index=0; index<list.Count && stopStepAll==false; index++)
            {
                MoveToMacroValue(index, CommandOptions.SuppressFlash | CommandOptions.AppendToTable | CommandOptions.KeepRepeatButtons);
            }
            ;
            uiStepAllGo.Visibility = Visibility.Visible;
            uiStepAllStop.Visibility = Visibility.Collapsed;
            CurrCommandOptions = CommandOptions.None; // reset to none
        }

        private void OnStepAllStop(object sender, RoutedEventArgs e)
        {
            stopStepAll = true;
        }

        private async void OnChangeValue(object sender, SelectionChangedEventArgs e)
        {
            // Gets called even during initialization, which I'm not keen on.
            if (!this.IsLoaded) return;
            if (e.AddedItems.Count != 1) return;
            var asv = (e.AddedItems[0] as FrameworkElement)?.Tag as ArgumentSettingValue;
            if (asv == null) return;
            var name = uiReplaceName.Text;
            CurrArgumentSettings.SetCurrent(name, asv);

            await CanDoCommand.DoCommandAsync(CurrCommandInfo, CurrCommandOptions | CommandOptions.KeepRepeatButtons);
        }
    }
}
