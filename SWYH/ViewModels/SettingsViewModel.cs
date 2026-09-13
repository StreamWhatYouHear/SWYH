/*
 *	 Stream What Your Hear
 *	 Assembly: SWYH
 *	 File: SettingsViewModel.cs
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

namespace SWYH.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using SWYH.Audio;
    using SWYH.Services;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Backs the Settings page.
    /// </summary>
    internal sealed partial class SettingsViewModel : ObservableObject
    {
        private readonly ISettingsService settingsService;

        public ICollectionView AudioDevices { get; }

        public System.Collections.Generic.IReadOnlyList<CaptureFormatOption> CaptureFormats => this.settingsService.AvailableCaptureFormats;

        public System.Collections.Generic.IReadOnlyList<int> Bitrates => this.settingsService.AvailableBitrates;

        [ObservableProperty]
        private bool isUpdateAvailable;

        [ObservableProperty]
        private bool isMp3;

        [ObservableProperty]
        private CaptureFormatOption selectedCaptureFormat;

        [ObservableProperty]
        private int selectedBitrate;

        [ObservableProperty]
        private AudioDeviceOption selectedAudioDevice;

        [ObservableProperty]
        private bool debugLogging;

        [ObservableProperty]
        private bool useSpecificPort;

        [ObservableProperty]
        private string httpPortText;

        [ObservableProperty]
        private bool runAtWindowsStartup;

        public IRelayCommand SaveCommand { get; }

        public IRelayCommand UpdateCommand { get; }

        public SettingsViewModel(ISettingsService settingsService)
        {
            this.settingsService = settingsService;

            var devicesView = new ListCollectionView(new System.Collections.Generic.List<AudioDeviceOption>(settingsService.AvailableAudioDevices));
            devicesView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(AudioDeviceOption.Category)));
            this.AudioDevices = devicesView;

            this.IsUpdateAvailable = App.NeedUpdate;
            this.IsMp3 = settingsService.StreamFormat == AudioFormats.Format.Mp3;
            this.SelectedCaptureFormat = settingsService.CaptureFormat;
            this.SelectedBitrate = settingsService.Mp3Bitrate;
            this.SelectedAudioDevice = settingsService.AudioDevice;
            this.DebugLogging = settingsService.Debug;
            this.UseSpecificPort = settingsService.UseSpecificHttpPort;
            this.HttpPortText = settingsService.HttpPort.ToString();
            this.RunAtWindowsStartup = settingsService.RunAtWindowsStartup;

            this.SaveCommand = new RelayCommand(this.Save);
            this.UpdateCommand = new RelayCommand(() => App.OpenUrl(Constants.DOWNLOAD_SWYH_URL));
        }

        private void Save()
        {
            if (this.UseSpecificPort && (!int.TryParse(this.HttpPortText, out int port) || port < 80 || port > 65534))
            {
                MessageBox.Show("Invalid port value !", "Stream What You Hear", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            this.settingsService.StreamFormat = this.IsMp3 ? AudioFormats.Format.Mp3 : AudioFormats.Format.Pcm;
            this.settingsService.CaptureFormat = this.SelectedCaptureFormat;
            this.settingsService.Mp3Bitrate = this.SelectedBitrate;
            this.settingsService.AudioDevice = this.SelectedAudioDevice;
            this.settingsService.Debug = this.DebugLogging;
            this.settingsService.UseSpecificHttpPort = this.UseSpecificPort;
            this.settingsService.HttpPort = this.UseSpecificPort ? int.Parse(this.HttpPortText) : 0;
            this.settingsService.RunAtWindowsStartup = this.RunAtWindowsStartup;

            if (!this.settingsService.HasPendingChanges)
            {
                this.settingsService.Save();
                return;
            }

            var response = MessageBox.Show("You need to restart Stream What You Hear to save new settings.\nDo you want to continue ?", "Stream What You Hear", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (response != MessageBoxResult.Yes)
            {
                return;
            }

            this.settingsService.Save();
            App.Restart();
            Application.Current.Shutdown();
        }
    }
}
