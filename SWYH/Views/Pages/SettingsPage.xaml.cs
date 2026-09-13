/*
 *	 Stream What Your Hear
 *	 Assembly: SWYH
 *	 File: SettingsPage.xaml.cs
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
    using Microsoft.Win32;
    using NAudio.CoreAudioApi;
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        private class GroupItem
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public string Category { get; set; }
        }

        public SettingsPage()
        {
            InitializeComponent();
            this.Loaded += this.SettingsPage_Loaded;
        }

        private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.btUpdate.Visibility = App.NeedUpdate ? Visibility.Visible : Visibility.Collapsed;
            this.cbRunAtWindowsStartup.IsChecked = this.GetRegisterInStartup();

            this.comboBox1.Items.Add(Audio.AudioFormats.AsString(Audio.AudioFormats.Pcm32kHz16bitMono));
            this.comboBox1.Items.Add(Audio.AudioFormats.AsString(Audio.AudioFormats.Pcm32kHz16bitStereo));
            this.comboBox1.Items.Add(Audio.AudioFormats.AsString(Audio.AudioFormats.Pcm44kHz16bitMono));
            this.comboBox1.Items.Add(Audio.AudioFormats.AsString(Audio.AudioFormats.Pcm44kHz16bitStereo));
            this.comboBox1.Items.Add(Audio.AudioFormats.AsString(Audio.AudioFormats.Pcm48kHz16bitMono));
            this.comboBox1.Items.Add(Audio.AudioFormats.AsString(Audio.AudioFormats.Pcm48kHz16bitStereo));
            foreach (var bitrate in Audio.AudioFormats.Mp3BitRates)
            {
                this.comboBox2.Items.Add(bitrate);
            }

            List<GroupItem> deviceItems = new List<GroupItem> { new GroupItem() { ID = "", Name = "Default", Category = "" } };
            var enumerator = new MMDeviceEnumerator();
            foreach (var wasapi in enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active))
            {
                deviceItems.Add(new GroupItem() { ID = wasapi.ID, Name = wasapi.FriendlyName, Category = wasapi.DataFlow.ToString() + " devices" });
            }
            ListCollectionView deviceItemsView = new ListCollectionView(deviceItems);
            deviceItemsView.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
            this.comboBoxDevice.ItemsSource = deviceItemsView;

            this.comboBox1.SelectedItem = Audio.AudioFormats.AsString(Audio.AudioSettings.GetAudioFormat());
            this.comboBox2.SelectedItem = Audio.AudioSettings.GetMP3Bitrate();
            this.comboBoxDevice.SelectedValue = SWYH.Properties.Settings.Default.AudioDevice;
            this.radioButton1.IsChecked = Audio.AudioSettings.GetStreamFormat() == Audio.AudioFormats.Format.Mp3;
            this.radioButton2.IsChecked = !this.radioButton1.IsChecked;
            this.cbDebug.IsChecked = SWYH.Properties.Settings.Default.Debug;
            this.cbUseSpecificPort.IsChecked = SWYH.Properties.Settings.Default.HTTPPort > 0;
            this.textBox1.Text = SWYH.Properties.Settings.Default.HTTPPort.ToString();
        }

        private void btValid_Click(object sender, RoutedEventArgs e)
        {
            if (this.cbUseSpecificPort.IsChecked == true)
            {
                if (!int.TryParse(this.textBox1.Text, out int port) || port > 65534 || port < 80)
                {
                    MessageBox.Show("Invalid port value !", "Stream What You Hear", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            this.RegisterInStartup(this.cbRunAtWindowsStartup.IsChecked == true);

            bool unchanged =
                this.comboBox1.SelectedItem.ToString() == Audio.AudioFormats.AsString(Audio.AudioSettings.GetAudioFormat()) &&
                this.comboBox2.SelectedItem.ToString() == Audio.AudioSettings.GetMP3Bitrate().ToString() &&
                this.comboBoxDevice.SelectedValue.ToString() == SWYH.Properties.Settings.Default.AudioDevice &&
                ((this.radioButton1.IsChecked == true && Audio.AudioSettings.GetStreamFormat() == Audio.AudioFormats.Format.Mp3) ||
                 (this.radioButton2.IsChecked == true && Audio.AudioSettings.GetStreamFormat() == Audio.AudioFormats.Format.Pcm)) &&
                this.cbDebug.IsChecked == SWYH.Properties.Settings.Default.Debug &&
                this.cbUseSpecificPort.IsChecked == (SWYH.Properties.Settings.Default.HTTPPort > 0) &&
                (this.cbUseSpecificPort.IsChecked != true || SWYH.Properties.Settings.Default.HTTPPort.ToString() == this.textBox1.Text);

            if (unchanged)
            {
                return;
            }

            var msg = MessageBox.Show("You need to restart Stream What You Hear to save new settings.\nDo you want to continue ?", "Stream What You Hear", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (msg != MessageBoxResult.Yes)
            {
                return;
            }

            if (this.comboBox1.SelectedItem.ToString() == Audio.AudioFormats.AsString(Audio.AudioFormats.Pcm32kHz16bitMono))
            {
                Audio.AudioSettings.SetAudioFormat(Audio.AudioFormats.Pcm32kHz16bitMono);
            }
            else if (this.comboBox1.SelectedItem.ToString() == Audio.AudioFormats.AsString(Audio.AudioFormats.Pcm32kHz16bitStereo))
            {
                Audio.AudioSettings.SetAudioFormat(Audio.AudioFormats.Pcm32kHz16bitStereo);
            }
            else if (this.comboBox1.SelectedItem.ToString() == Audio.AudioFormats.AsString(Audio.AudioFormats.Pcm44kHz16bitMono))
            {
                Audio.AudioSettings.SetAudioFormat(Audio.AudioFormats.Pcm44kHz16bitMono);
            }
            else if (this.comboBox1.SelectedItem.ToString() == Audio.AudioFormats.AsString(Audio.AudioFormats.Pcm44kHz16bitStereo))
            {
                Audio.AudioSettings.SetAudioFormat(Audio.AudioFormats.Pcm44kHz16bitStereo);
            }
            else if (this.comboBox1.SelectedItem.ToString() == Audio.AudioFormats.AsString(Audio.AudioFormats.Pcm48kHz16bitMono))
            {
                Audio.AudioSettings.SetAudioFormat(Audio.AudioFormats.Pcm48kHz16bitMono);
            }
            else
            {
                Audio.AudioSettings.SetAudioFormat(Audio.AudioFormats.Pcm48kHz16bitStereo);
            }
            Audio.AudioSettings.SetMP3Bitrate(int.Parse(this.comboBox2.SelectedItem.ToString()));
            Audio.AudioSettings.SetStreamFormat(this.radioButton1.IsChecked == true ? Audio.AudioFormats.Format.Mp3 : Audio.AudioFormats.Format.Pcm);
            SWYH.Properties.Settings.Default.AudioDevice = this.comboBoxDevice.SelectedValue.ToString();
            SWYH.Properties.Settings.Default.HTTPPort = this.cbUseSpecificPort.IsChecked == true ? int.Parse(this.textBox1.Text) : 0;
            SWYH.Properties.Settings.Default.Debug = this.cbDebug.IsChecked == true;
            SWYH.Properties.Settings.Default.Save();
            App.Restart();
            Application.Current.Shutdown();
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

        private void cbUseSpecificPort_Checked(object sender, RoutedEventArgs e)
        {
            if (this.cbUseSpecificPort.IsChecked == true && (this.textBox1.Text == "0" || string.IsNullOrEmpty(this.textBox1.Text)))
            {
                this.textBox1.Text = Constants.DEFAULT_HTTP_PORT.ToString();
            }
        }

        private void btUpdate_Click(object sender, RoutedEventArgs e)
        {
            App.OpenUrl(Constants.DOWNLOAD_SWYH_URL);
        }

        private void RegisterInStartup(bool isChecked)
        {
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(Constants.REGISTRY_START_SUBKEY, true);
                if (isChecked)
                {
                    registryKey.SetValue(Constants.REGISTRY_SWYH_KEY, System.Windows.Forms.Application.ExecutablePath);
                }
                else if (GetRegisterInStartup())
                {
                    registryKey.DeleteValue(Constants.REGISTRY_SWYH_KEY);
                }
            }
            catch { }
        }

        private bool GetRegisterInStartup()
        {
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(Constants.REGISTRY_START_SUBKEY, true);
                return registryKey.GetValue(Constants.REGISTRY_SWYH_KEY, null) != null;
            }
            catch { }
            return false;
        }
    }
}
