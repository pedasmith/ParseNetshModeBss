using System.Windows;

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
