/*
 *	 Stream What Your Hear
 *	 Assembly: SWYH
 *	 File: AboutPage.xaml.cs
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

namespace SWYH.Views.Pages
{
    using SWYH.ViewModels;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Wpf.Ui.Controls;

    /// <summary>
    /// Interaction logic for AboutPage.xaml
    /// </summary>
    public partial class AboutPage : Page
    {
        internal AboutViewModel ViewModel { get; }

        public AboutPage()
        {
            this.ViewModel = new AboutViewModel();
            this.DataContext = this.ViewModel;
            InitializeComponent();
        }

        private async void VersionBadge_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(this.ViewModel.VersionText);
            this.VersionBadge.Content = new SymbolIcon { Symbol = SymbolRegular.Checkmark24, Foreground = System.Windows.Media.Brushes.White };
            await Task.Delay(1200);
            this.VersionBadge.Content = this.ViewModel.VersionText;
        }
    }
}
