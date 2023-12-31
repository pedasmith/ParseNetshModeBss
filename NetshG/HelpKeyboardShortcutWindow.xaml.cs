using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace NetshG
{
    public class HelpDescription
    {
        public HelpDescription(string key, string description)
        {
            Key = key;
            Description = description;
        }
        public string Key { get; internal set; }
        public string Description { get; internal set; }
    }
    /// <summary>
    /// Interaction logic for HelpKeyboardShortcutWindow.xaml
    /// </summary>
    public partial class HelpKeyboardShortcutWindow : Window
    {
        public HelpKeyboardShortcutWindow()
        {
            InitializeComponent();
            this.Loaded += HelpKeyboardShortcutWindow_Loaded;
        }

        public ObservableCollection<HelpDescription> HelpDescriptions { get; private set; } = new ObservableCollection<HelpDescription>();
        private void HelpKeyboardShortcutWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this;
            HelpDescriptions.Clear();
            foreach (var row in MainWindow.KeyDescriptions)
            {
                HelpDescriptions.Add(row);
            }
        }
    }
}
