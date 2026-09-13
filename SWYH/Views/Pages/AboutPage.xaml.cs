/*
 *	 Stream What Your Hear
 *	 Assembly: SWYH
 *	 File: AboutPage.xaml.cs
 *	 Web site: http://www.streamwhatyouhear.com
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

namespace SWYH.Views.Pages
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for AboutPage.xaml
    /// </summary>
    public partial class AboutPage : Page
    {
        public AboutPage()
        {
            InitializeComponent();
            this.btUpdate.Visibility = App.NeedUpdate ? Visibility.Visible : Visibility.Collapsed;
            var version = App.FileVersion;
            this.tbVersion.Content = string.Format("Version {0}", version.ProductVersion);
        }

        private void btUpdate_Click(object sender, RoutedEventArgs e)
        {
            App.OpenUrl(Constants.DOWNLOAD_SWYH_URL);
        }
    }
}
