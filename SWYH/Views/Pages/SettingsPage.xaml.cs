/*
 *	 Stream What Your Hear
 *	 Assembly: SWYH
 *	 File: SettingsPage.xaml.cs
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
    using SWYH.Services;
    using SWYH.ViewModels;
    using System.Text.RegularExpressions;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        internal SettingsViewModel ViewModel { get; }

        public SettingsPage()
        {
            this.ViewModel = new SettingsViewModel(new SettingsService());
            this.DataContext = this.ViewModel;
            InitializeComponent();
        }

        private void textBox1_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
            if (!e.Handled)
            {
                int.TryParse(this.textBox1.Text + e.Text, out int result);
                e.Handled = !(result > 0 && result < 65534);
            }
        }

        private static bool IsTextAllowed(string text)
        {
            return !new Regex("[^0-9.-]+").IsMatch(text);
        }
    }
}
