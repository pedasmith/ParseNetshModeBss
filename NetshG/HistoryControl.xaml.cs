﻿using System;
using System.Collections.Generic;
using System.Globalization; // just needed for measuring a string with the current culture.
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace NetshG
{
    /// <summary>
    /// HistoryControl handles a set of UserControl items to be displayed in a seperate HistoryPanel. The control displays a 
    /// series of "dots". When the user picks a "dot", the correspondng UserControl is displayed in the HistoryPanel.
    /// </summary>
    public partial class HistoryControl : UserControl
    {
        class ControlData
        {
            public ControlData(UserControl control, string title, int index) { Control = control; Time = DateTimeOffset.Now; CDTitle = title; HistoryIndex = index; } 

            // Fields
            public UserControl Control;
            public DateTimeOffset Time;
            public string TimeStr {  get {  return Time.ToString("HH:mm:ss"); } }
            public string CDTitle { get; set; }
            public string Selected { get; set; } = HistoryControl.BULLET_SELECTED;
            public string NotSelected { get; set; } = HistoryControl.BULLET_NOT_SELECTED;
            public int HistoryIndex { get; set; } = 0;
        }

        public const int MAX_HISTORY_ITEMS = 50;

        /// <summary>
        /// List of the items to display. A ControlData has the UserControl plus a "Time" and a CDTitle. The time is time
        /// that the cd was added to the list.
        /// </summary>
        private List<ControlData> HistoryItems {  get;  }  = new List<ControlData>();
        private int CurrIndex = -1;
        private int NItemsAdded = 0;
        public bool IsEmpty {  get { return CurrIndex == -1; } }

        public Panel? HistoryPanel;
        public UXCommands? UXCommands { get; set; } = null;
        public UserControl? LastControlAdded { get; private set; } = null;

        private Random rnd = new Random();
        /// <summary>
        /// Adds a UserControl (in practice, always a CommandOutputControl) to the history list. Will also update
        /// the HistoryPanel with the new control, removing the old entry (but only if the current index is the last
        /// index; if the user asked to show a specific cd, that cd stays up)
        /// </summary>
        /// <param name="item"></param>
        public void AddCurrentControl(UserControl item, string title)
        {
            NItemsAdded++; // total number ever added
            LastControlAdded = item;
            var cd = new ControlData(item, title, NItemsAdded);

            cd.Selected = BULLET_SELECTED; // + NItemsAdded.ToString() + " "; // Handy extra bits for debugging
            cd.NotSelected = BULLET_NOT_SELECTED; // + NItemsAdded.ToString() + " ";

            // Figure out what to do when the history list is "too full"
            // Exponentially weighted moving average (EWMA) for the win (FTW)!
            // Note that unlike a classic EWMA, we always add the cd as the last index;
            // the code will only decide whether to save old last cd as one of the
            // historical items

            var idx = rnd.NextInt64(NItemsAdded);
            if (HistoryItems.Count < MAX_HISTORY_ITEMS)
            {
                // Easy case: This happens at start-up and also every time the user deletes
                // an cd. Note that we aren't deleting old things, so there's no weird swappng
                // of the CurrIndex cd.
            }
            else
            {
                // Default case: get rid of the last entry. We always want to
                // add the cd, possibly just this one time, to the UX
                var lastIndex = HistoryItems.Count - 1;
                var indexToDelete = lastIndex;


                if (idx < MAX_HISTORY_ITEMS-1)
                {
                    // Winner! remove an old cd. By default remove the 'idx' cd. If 
                    // 
                    indexToDelete = (int)idx;
                    if (CurrIndex == idx)
                    {
                        // delete an earlier cd if possible
                        if (indexToDelete > 0) indexToDelete--;
                        else indexToDelete++;
                    }
                }

                // delete correct items + fix up the CurrIndex
                HistoryItems.RemoveAt((int)indexToDelete);
                if (CurrIndex > indexToDelete)
                {
                    CurrIndex--;
                }

                // For whatever reason, can't do RemoveAt on an Inlines?
                var uxdot = uiHistoryRuns.Inlines.ToArray()[CurrIndex];
                uiHistoryRuns.Inlines.Remove(uxdot); 
            }


            HistoryItems.Add(cd);


            var newIndex = HistoryItems.Count - 1;
            bool selectNew = CurrIndex == -1 || CurrIndex >= newIndex -1;
            if (selectNew)
            {
                CurrIndex = newIndex;
            }
            if (HistoryPanel != null && selectNew)
            {
                HistoryPanel.Children.Clear();
                HistoryPanel.Children.Add(item);
            }
            if (selectNew)
            {
                SetTimeAndIndex(cd.TimeStr, cd.HistoryIndex);
            }
            // Add in a little tag  
            // ◦ WHITE BULLET U+25E6
            // • BULLET U+2022
            var tt = cd.CDTitle + " " + cd.TimeStr;
            var bulletText = selectNew ? cd.Selected: cd.NotSelected;
            var tag = new Run() { Text = bulletText, ToolTip = tt,  };
            tag.Tag = cd;
            tag.MouseLeftButtonUp += Tag_MouseLeftButtonUp;
            //tag.MouseRightButtonUp += Tag_MouseRightButtonUp;

            // Context menu
            var cm = new ContextMenu();
            cm.Items.Add(new MenuItem() { Header = cd.CDTitle });
            var dmi = new MenuItem() { Header = "Delete", Tag=cd };
            dmi.Click += OnDelete_Click;
            cm.Items.Add(dmi);
            tag.ContextMenu = cm;

            uiHistoryRuns.Inlines.Add(tag);
            if (selectNew)
            {
                SetBullet(CurrIndex);
            }
            SetHistoryMargin("AD");
        }

        private void OnDelete_Click(object sender, RoutedEventArgs e)
        {
            var cd = (sender as FrameworkElement)?.Tag as ControlData;
            if (cd == null) return;

            var index = GetIndexOf(cd);
            DoDeleteAt(index);
        }
        public void DeleteCurrIndex()
        {
            DoDeleteAt(CurrIndex);
        }

        private void DoDeleteAt(int index)
        {
            if (index < 0) return;
            var cd = HistoryItems[index];
            if (cd == null) return; // can never happen.
            var inline = uiHistoryRuns.Inlines.ElementAt(index);
            if (inline == null) return;

            // Remove Stuff
            uiHistoryRuns.Inlines.Remove(inline);
            HistoryItems.Remove(cd);

            // Update the UX. If the index is >= CurrIndex, must decrement
            if (index >= CurrIndex) CurrIndex--;
            IncIndex(0); // Ensures that the CurrIndex is set correctly.
            ShowCurrIndex();

        }

        private void Tag_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var run = sender as Run;
            if (run == null) return;
            var item = run.Tag as ControlData;
            if (item == null) return;

        }

        private void Tag_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var run = sender as Run;
            if (run == null) return;
            var item = run.Tag as ControlData;
            if (item == null) return;
            var index = GetIndexOf(item);
            if (index < 0) return;
            CurrIndex = index;
            ShowCurrIndex();
        }

        private void SetTimeAndIndex(string time, int index)
        {
            if (_log != "") return; // It's also the logging area :-)
            uiTime.Text = time;
            uiIndex.Text = index.ToString();
        }



        private string _log = "";
        private void Log(string str)
        {
            _log = str;
            uiTime.Text = str;
        }

        const string BULLET_NOT_SELECTED = " ◦ "; // Uses " " THIN SPACE U+2009
        const string BULLET_SELECTED = " • ";

        private void SetBullet(int selected = 0)
        {
            int index = 0;
            var dup = uiHistoryRuns.Inlines.ToList();
            foreach (var item in dup)
            {
                var run = item as Run;
                if (run != null)
                {
                    var txt = (index == selected) ? HistoryItems[index].Selected : HistoryItems[index].NotSelected;
                    run.Text = txt;
                }
                index++;
            }
        }
        public UserControl? GetCurrentControl()
        {
            if (CurrIndex < 0) return null;
            var retval = HistoryItems[CurrIndex];
            return retval.Control;
        }
        public HistoryControl()
        {
            InitializeComponent();
        }

        public void Move(int step)
        {
            IncIndex(step);
            ShowCurrIndex();
        }

        public void MoveTo(int value)
        {
            CurrIndex = value;
            IncIndex(0); // Makes sure CurrIndex is in the proper range
            ShowCurrIndex();
        }
        /// <summary>
        /// Increments (or decrements) the CurrIndex, carefully making sure it stays in range.
        /// If given 0 for the step, just resets to be in range. 
        /// If -1, resets to be in range, unless the range having no values.
        /// Will set to -1 only in the case of the range having no values.
        /// </summary>
        /// <param name="step"></param>
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
            if (CurrIndex < 0)
            {
                if (HistoryPanel != null)
                {
                    HistoryPanel.Children.Clear();
                }
                return;
            }
            var cd = HistoryItems[CurrIndex];
            if (cd == null) return;
            if (HistoryPanel != null && cd.Control != null)
            {
                HistoryPanel.Children.Clear();
                HistoryPanel.Children.Add(cd.Control);
            }
            UXCommands?.SetCommandTitle(cd.CDTitle);
            SetTimeAndIndex(cd.TimeStr, cd.HistoryIndex);
            SetBullet(CurrIndex);
            SetHistoryMargin("curr");
        }

        private int GetIndexOf(ControlData searchFor)
        {
            for (int i=0; i<HistoryItems.Count; i++)
            {
                var item = (ControlData)HistoryItems[i];
                if (item == searchFor) return i;
            }
            return -1;
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

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetHistoryMargin("SC");
        }

        private void SetHistoryMargin(string src = "")
        {
            var candidate = uiHistoryRuns.Text;
            var tb = uiHistoryRuns;

            var areaWidth = uiHistoryRunPanel.ActualWidth; 
            var nitems = tb.Inlines.Count();

            if (areaWidth <= 0) return;
            if (nitems < 1) return;

            var tf = new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch);
            var fmt = new FormattedText(
                candidate,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                tf,
                tb.FontSize,
                Brushes.Black,
                new NumberSubstitution(),
                VisualTreeHelper.GetDpi(tb).PixelsPerDip);
            var strWidth = fmt.Width;
            double ratio = (double)CurrIndex / (double)nitems;
            //double chWidth = strWidth / (double)nitems;

            // Now do our calculations using strWidth and areaWidth plus ratio

            // Goal #1: always center at the current index
            var tp = "M";
            var idealLeft = (areaWidth / 2.0) - (strWidth * ratio);

            // Goal #2: but don't have a gap on the right.
            var strRight = idealLeft + strWidth;
            var gapRight = areaWidth - strRight; // when positive, there's a gap on the right
            if (gapRight > 0)
            {
                idealLeft += gapRight;
                tp += "R";
            }

            // Goal #3: but don't have any gap on the left
            if (idealLeft > 0) { idealLeft = 0; tp = "L"; }

            Canvas.SetLeft(tb, idealLeft);
        }
    }
}
