using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
using Utilities;

namespace NetshG
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ArgumentSettingControl : UserControl
    {
        public bool InitOk = true;
        public ArgumentSettingControl()
        {
            InitializeComponent();
        }
        ArgumentSettings? CurrArgumentSettings;
        String ItemName = "";

        public void SetValues(ArgumentSettings settings, string name)
        {
            CurrArgumentSettings = settings;
            ItemName = name;
            var values = settings.GetValueList(name);
            var defaultValue = settings.GetCurrent(name, "");
            if (values == null)
            {
                InitOk = false;
                return;
            }

            uiName.Text = name;

            uiValues.Items.Clear();
            ComboBoxItem? defaultCbi = null;
            foreach (var item in values)
            {
                var cbi = new ComboBoxItem();
                cbi.Tag = item;
                cbi.Content = item.HasUserString ? $"{item.UserString} ({item.Value})" : item.Value;
                uiValues.Items.Add(cbi);

                if (item.Value == defaultValue.Value)
                {
                    defaultCbi = cbi;
                }
            }
            uiValues.SelectedItem = defaultCbi;
        }


        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrArgumentSettings == null) return;
            if (e.AddedItems.Count != 1) return;
            var cbi = e.AddedItems[0] as ComboBoxItem;
            if (cbi == null) return;
            var item = cbi.Tag as ArgumentSettingValue;
            if (item == null) return;
            CurrArgumentSettings.SetCurrent(ItemName, item);
        }
    }
}
