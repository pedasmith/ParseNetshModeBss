using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// <summary>
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();
            this.Loaded += HelpWindow_Loaded;
        }

        private void HelpWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var version = Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString();
            List<Run> runsToModify = new List<Run>();
            foreach (var child in uiHelpText.Inlines)
            {
                if (child is Run run)
                {
                    if (run.Text.Contains("{VERSION}"))
                    {
                        runsToModify.Add(run);
                    }
                }
            }
            foreach (var run in runsToModify) 
            {
                run.Text = run.Text.Replace("{VERSION}", version);
            }


        }
    }
}
