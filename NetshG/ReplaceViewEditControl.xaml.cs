using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
using System.Xml.Linq;
using Utilities;

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
