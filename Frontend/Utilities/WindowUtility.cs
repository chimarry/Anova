using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ANOVA.Frontend.Utilities
{
    public static class WindowUtility
    {
        /// <summary>
        /// Refresh window
        /// </summary>
        /// <param name="window"></param>
        public static void Refresh(IRefreshable window)
        {
            window.Refresh();

        }

        /// <summary>
        /// Sets window border depending on screen properties, and sets grid also
        /// </summary>
        /// <param name="window">Window to set</param>
        /// <param name="grid">Grid to resize</param>
        /// <returns>Number of grid columns or rows, depending what is greater</returns>
        public static int SetBorder(Window window, Grid grid)
        {
            window.Height = Config.Properties.Default.WindowHeightProportion * SystemParameters.FullPrimaryScreenHeight;
            window.Width = Config.Properties.Default.WindowWidthProportion * SystemParameters.FullPrimaryScreenWidth;
            int Count = WindowUtility.CalculateGridMaxCount(grid);
            //   window.Left = SystemParameters.BorderWidth;
            // window.Top = SystemParameters.FixedFrameHorizontalBorderHeight;
            return Count;
        }

        /// <summary>
        /// Sets window as modal
        /// </summary>
        /// <param name="modalWindow">Modal window to adjust</param>
        /// <param name="parentWindowHeight">Parent's window height</param>
        /// <param name="parentWindowWidth">Parent's window widht</param>
        public static void SetModal(Window modalWindow, double parentWindowHeight, double parentWindowWidth)
        {
            modalWindow.Top = 20;
            modalWindow.Left = (parentWindowWidth - modalWindow.Width) / 2;
            modalWindow.Height = parentWindowHeight;
        }

        /// <summary>
        /// Centers window based on parent's window properties
        /// </summary>
        /// <param name="modalWindow">Modal window to adjust</param>
        /// <param name="parentWindowHeight">Parent's window height</param>
        /// <param name="parentWindowWidth">Parent's window widht</param>
        public static void CenterWindow(Window modalWindow, double parentWindowHeigth, double parentWindowWidth)
        {
            modalWindow.Left = (parentWindowWidth - modalWindow.Width) / 2;
            modalWindow.Top = (parentWindowHeigth - modalWindow.Height) / 2;
        }

        public static int CalculateGridMaxCount(Grid grid)
        {
            return grid.RowDefinitions.Count > grid.ColumnDefinitions.Count ? grid.RowDefinitions.Count : grid.ColumnDefinitions.Count;
        }

        /// <summary>
        /// One window closes, and the other shows
        /// </summary>
        /// <param name="toClose"></param>
        /// <param name="toShow"></param>
        public static void ShowWindow(Window toClose, Window toShow)
        {
            toShow.Left = toClose.Left;
            toShow.Top = toClose.Top;
            toClose.Close();
            toShow.Show();
        }

        /// <summary>
        /// Returns to previous window
        /// </summary>
        /// <param name="returnToWindow"></param>
        /// <param name="currentWindow"></param>
        public static void ReturnToPrevious(Window returnToWindow, Window currentWindow)
        {
            returnToWindow = Activator.CreateInstance(returnToWindow.GetType()) as Window;
            ShowWindow(currentWindow, returnToWindow);
        }

        /// <summary>
        /// Writes message on specified label
        /// </summary>
        /// <param name="mesage"></param>
        /// <param name="label"></param>
        /// <param name="success">If operation was successfull, color will be green</param>
        public static void WriteMessage(string mesage, Label label, bool success)
        {
            label.Content = new TextBlock()
            {
                Text = mesage,
                Foreground = success == true ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.Red),
                FontWeight = FontWeights.Bold,
                FontSize = Config.Properties.Default.LabelFontSize,
                TextWrapping = TextWrapping.Wrap
            };
        }
    }
}
