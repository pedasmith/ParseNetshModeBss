using System;
using System.Collections.Generic;
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
using Windows.Graphics.Printing.PrintSupport;
using Windows.Media.Core;

namespace NetshG
{
    /// <summary>
    /// Interaction logic for HistoryControl.xaml
    /// </summary>
    public partial class HistoryControl : UserControl
    {
        private List<UserControl> HistoryItems {  get;  }  = new List<UserControl>();
        private int CurrIndex = -1;

        public Panel? HistoryPanel;
        /// <summary>
        /// Adds a UserControl (in practice, always a CommandOutputControl) to the history list. Will also update
        /// the HistoryPanel with the new control, removing the old entry.
        /// </summary>
        /// <param name="item"></param>
        public void AddCurrentControl(UserControl item)
        {
            HistoryItems.Add(item);
            CurrIndex = HistoryItems.Count - 1;
            if (HistoryPanel != null)
            {
                HistoryPanel.Children.Clear();
                HistoryPanel.Children.Add(item);
            }
            // Add in a little tag  
            // ◦ WHITE BULLET U+25E6
            // • BULLET U+2022
            var tag = new Run() { Text = BULLET_SELECTED };
            uiHistoryRuns.Inlines.Add(tag);
            SetBullet(CurrIndex);
        }

        const string BULLET_NOT_SELECTED = " ◦ ";
        const string BULLET_SELECTED = " • ";

        private void SetBullet(int selected = 0)
        {
            int index = 0;
            var dup = uiHistoryRuns.Inlines.ToList();
            foreach (var item in dup)
            {
                var run = item as Run;
                if (run != null)
                {
                    var txt = (index == selected) ? BULLET_SELECTED : BULLET_NOT_SELECTED;
                    run.Text = txt;
                }
                index++;
            }
        }
        public UserControl? GetCurrentControl()
        {
            if (CurrIndex < 0) return null;
            var retval = HistoryItems[CurrIndex];
            return retval;
        }
        public HistoryControl()
        {
            InitializeComponent();
        }

        private void IncIndex(int step)
        {
            CurrIndex += step;
            if (CurrIndex < 0) CurrIndex = 0;
            if (CurrIndex >= HistoryItems.Count)
            {
                CurrIndex = HistoryItems.Count - 1;
            }
        }
        private void ShowCurrIndex()
        {
            if (CurrIndex < 0) return;
            var item = HistoryItems[CurrIndex];
            if (HistoryPanel != null && item != null)
            {
                HistoryPanel.Children.Clear();
                HistoryPanel.Children.Add(item);
            }
            SetBullet(CurrIndex);
        }
        private void OnPrev(object sender, MouseButtonEventArgs e)
        {
            IncIndex(-1);
            ShowCurrIndex();
        }

        private void OnNext(object sender, MouseButtonEventArgs e)
        {
            IncIndex(1);
            ShowCurrIndex();
        }
    }
}
