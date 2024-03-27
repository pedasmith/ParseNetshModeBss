using System.Collections.ObjectModel;
using System.Windows;

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
