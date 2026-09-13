/*
 *	 Stream What Your Hear
 *	 Assembly: SWYH
 *	 File: SettingsService.cs
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

namespace SWYH.Services
{
    using Microsoft.Win32;
    using NAudio.CoreAudioApi;
    using SWYH.Audio;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <inheritdoc cref="ISettingsService"/>
    internal sealed class SettingsService : ISettingsService
    {
        // Snapshot of the values on disk when this instance was created, to detect changes requiring a restart.
        private readonly AudioFormats.Format originalStreamFormat;
        private readonly NAudio.Wave.WaveFormat originalCaptureFormat;
        private readonly int originalMp3Bitrate;
        private readonly string originalAudioDeviceId;
        private readonly int originalHttpPort;
        private readonly bool originalDebug;

        public IReadOnlyList<CaptureFormatOption> AvailableCaptureFormats { get; } = new[]
        {
            new CaptureFormatOption(AudioFormats.AsString(AudioFormats.Pcm32kHz16bitMono), AudioFormats.Pcm32kHz16bitMono),
            new CaptureFormatOption(AudioFormats.AsString(AudioFormats.Pcm32kHz16bitStereo), AudioFormats.Pcm32kHz16bitStereo),
            new CaptureFormatOption(AudioFormats.AsString(AudioFormats.Pcm44kHz16bitMono), AudioFormats.Pcm44kHz16bitMono),
            new CaptureFormatOption(AudioFormats.AsString(AudioFormats.Pcm44kHz16bitStereo), AudioFormats.Pcm44kHz16bitStereo),
            new CaptureFormatOption(AudioFormats.AsString(AudioFormats.Pcm48kHz16bitMono), AudioFormats.Pcm48kHz16bitMono),
            new CaptureFormatOption(AudioFormats.AsString(AudioFormats.Pcm48kHz16bitStereo), AudioFormats.Pcm48kHz16bitStereo),
        };

        public IReadOnlyList<int> AvailableBitrates { get; } = AudioFormats.Mp3BitRates;

        public IReadOnlyList<AudioDeviceOption> AvailableAudioDevices { get; }

        public AudioFormats.Format StreamFormat { get; set; }

        public CaptureFormatOption CaptureFormat { get; set; }

        public int Mp3Bitrate { get; set; }

        public AudioDeviceOption AudioDevice { get; set; }

        public bool Debug { get; set; }

        public bool UseSpecificHttpPort { get; set; }

        public int HttpPort { get; set; }

        public bool RunAtWindowsStartup { get; set; }

        public SettingsService()
        {
            var devices = new List<AudioDeviceOption> { new AudioDeviceOption("", "Default", "") };
            using (var enumerator = new MMDeviceEnumerator())
            {
                foreach (var wasapi in enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active))
                {
                    devices.Add(new AudioDeviceOption(wasapi.ID, wasapi.FriendlyName, wasapi.DataFlow + " devices"));
                }
            }
            this.AvailableAudioDevices = devices;

            this.originalStreamFormat = AudioSettings.GetStreamFormat();
            this.originalCaptureFormat = AudioSettings.GetAudioFormat();
            this.originalMp3Bitrate = AudioSettings.GetMP3Bitrate();
            this.originalAudioDeviceId = SWYH.Properties.Settings.Default.AudioDevice;
            this.originalHttpPort = SWYH.Properties.Settings.Default.HTTPPort;
            this.originalDebug = SWYH.Properties.Settings.Default.Debug;

            this.StreamFormat = this.originalStreamFormat;
            this.CaptureFormat = this.AvailableCaptureFormats.FirstOrDefault(f => f.Format == this.originalCaptureFormat) ?? this.AvailableCaptureFormats[^1];
            this.Mp3Bitrate = this.originalMp3Bitrate;
            this.AudioDevice = this.AvailableAudioDevices.FirstOrDefault(d => d.Id == this.originalAudioDeviceId) ?? this.AvailableAudioDevices[0];
            this.Debug = this.originalDebug;
            this.UseSpecificHttpPort = this.originalHttpPort > 0;
            this.HttpPort = this.originalHttpPort > 0 ? this.originalHttpPort : Constants.DEFAULT_HTTP_PORT;
            this.RunAtWindowsStartup = this.GetRegisterInStartup();
        }

        public bool HasPendingChanges =>
            this.StreamFormat != this.originalStreamFormat ||
            this.CaptureFormat.Format != this.originalCaptureFormat ||
            this.Mp3Bitrate != this.originalMp3Bitrate ||
            this.AudioDevice.Id != this.originalAudioDeviceId ||
            this.Debug != this.originalDebug ||
            (this.UseSpecificHttpPort ? this.HttpPort : 0) != this.originalHttpPort;

        public bool Save()
        {
            bool restartRequired = this.HasPendingChanges;

            this.SetRegisterInStartup(this.RunAtWindowsStartup);

            AudioSettings.SetAudioFormat(this.CaptureFormat.Format);
            AudioSettings.SetMP3Bitrate(this.Mp3Bitrate);
            AudioSettings.SetStreamFormat(this.StreamFormat);
            SWYH.Properties.Settings.Default.AudioDevice = this.AudioDevice.Id;
            SWYH.Properties.Settings.Default.HTTPPort = this.UseSpecificHttpPort ? this.HttpPort : 0;
            SWYH.Properties.Settings.Default.Debug = this.Debug;
            SWYH.Properties.Settings.Default.Save();

            return restartRequired;
        }

        private void SetRegisterInStartup(bool isChecked)
        {
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(Constants.REGISTRY_START_SUBKEY, true);
                if (isChecked)
                {
                    registryKey.SetValue(Constants.REGISTRY_SWYH_KEY, System.Windows.Forms.Application.ExecutablePath);
                }
                else if (this.GetRegisterInStartup())
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
