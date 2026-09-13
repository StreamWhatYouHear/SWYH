/*
 *	 Stream What Your Hear
 *	 Assembly: SWYH
 *	 File: HomePage.xaml.cs
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
    using SWYH.ViewModels;
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using Wpf.Ui.Controls;

    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        internal HomeViewModel ViewModel { get; }

        public HomePage()
        {
            this.ViewModel = new HomeViewModel(App.CurrentInstance.SwyhService);
            this.DataContext = this.ViewModel;
            InitializeComponent();
            this.Unloaded += (s, e) => this.ViewModel.Dispose();
        }

        private void ChangeAudioSource_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentInstance.ShowMainWindow(typeof(SettingsPage));
        }

        private async void CopyUrlButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Wpf.Ui.Controls.Button button)
            {
                return;
            }

            var originalIcon = button.Icon;
            var originalToolTip = button.ToolTip;
            button.Icon = new SymbolIcon { Symbol = SymbolRegular.Checkmark24 };
            button.ToolTip = "Copied!";
            await Task.Delay(1200);
            button.Icon = originalIcon;
            button.ToolTip = originalToolTip;
        }
    }
}

