/*
 *	 Stream What Your Hear
 *	 Assembly: SWYH
 *	 File: MainWindow.xaml.cs
 *	 Web site: https://www.streamwhatyouhear.com
 *	 Copyright (C) 2012-2026 - Sebastien Warin <https://sebastien.warin.fr> and others
 *
 *   This file is part of Stream What Your Hear.
 *	 
 *	 Stream What Your Hear is free software: you can redistribute it and/or modify
 *	 it under the terms of the GNU General Public License as published by
 *	 the Free Software Foundation, either version 2 of the License, or
 *	 (at your option) any later version.
 *	 
 *	 Stream What Your Hear is distributed in the hope that it will be useful,
 *	 but WITHOUT ANY WARRANTY; without even the implied warranty of
 *	 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *	 GNU General Public License for more details.
 *	 
 *	 You should have received a copy of the GNU General Public License
 *	 along with Stream What Your Hear. If not, see <http://www.gnu.org/licenses/>.
 */

namespace SWYH.Views
{
    using SWYH.Views.Pages;
    using System.ComponentModel;
    using Wpf.Ui.Controls;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FluentWindow
    {
        /// <summary>
        /// Set to true only when the application is really shutting down (tray "Exit"), to allow the window to close.
        /// </summary>
        public bool AllowClose { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += (s, e) => this.RootNavigation.Navigate(typeof(HomePage));
        }

        internal void NavigateTo(System.Type pageType)
        {
            this.RootNavigation.Navigate(pageType);
        }

        private void FluentWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!this.AllowClose)
            {
                e.Cancel = true;
                this.Hide();
                App.CurrentInstance.NotifyMinimizedToTray();
            }
        }

        private void FluentWindow_StateChanged(object sender, System.EventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Minimized)
            {
                this.Hide();
            }
        }
    }
}
