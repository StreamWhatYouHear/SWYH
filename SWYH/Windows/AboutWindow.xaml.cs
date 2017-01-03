/*
 *	 Stream What Your Hear
 *	 Assembly: SWYH
 *	 File: AboutWindow.xaml.cs
 *	 Web site: http://www.streamwhatyouhear.com
 *	 Copyright (C) 2012-2017 - Sebastien Warin <http://sebastien.warin.fr> and others	
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

namespace SWYH
{
    using System.Diagnostics;
    using System.Reflection;
    using System.Windows;

    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.btUpdate.Visibility = (App.NeedUpdate) ? Visibility.Visible : System.Windows.Visibility.Collapsed;
            var version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            this.version.Text = string.Format("Version{3} {0}.{1} (Build {2})", version.ProductMajorPart, version.ProductMinorPart, version.ProductBuildPart, (version.ProductPrivatePart % 2) == 0 ? "" : " BETA");
        }

        private void btUpdate_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start(Constants.DOWNLOAD_SWYH_URL);
            this.Close();
        }

        private void tbSwarinWebSite_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start(Constants.SWARIN_WEBSITE_URL);
        }

        private void tbSWYHWebSite_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start(Constants.SWYH_WEBSITE_URL);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                this.Close();
            }
        }
    }
}
