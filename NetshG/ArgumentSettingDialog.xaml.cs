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
using System.Windows.Shapes;
using Utilities;

namespace NetshG
{
    /// <summary>
    /// Interaction logic for ArgumentSettingDialog.xaml
    /// </summary>
    public partial class ArgumentSettingDialog : Window
    {
        public bool InitOk = true;
        public ArgumentSettingDialog(ArgumentSettings settings, string name)
        {
            InitializeComponent();
            uiSetting.SetValues(settings, name);
            InitOk = uiSetting.InitOk;
        }

        private void OnOk(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
