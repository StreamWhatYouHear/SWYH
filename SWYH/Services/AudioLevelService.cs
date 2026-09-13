/*
 *	 Stream What Your Hear
 *	 Assembly: SWYH
 *	 File: AudioLevelService.cs
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
    using NAudio.CoreAudioApi;
    using System;

    /// <summary>
    /// Reads the live peak level of the configured capture device, for UI visualization purposes only.
    /// </summary>
    internal sealed class AudioLevelService : IDisposable
    {
        private readonly MMDeviceEnumerator enumerator = new();

        /// <summary>
        /// Gets the current master peak value (0.0 - 1.0) of the configured capture device.
        /// </summary>
        public double GetCurrentPeak()
        {
            try
            {
                using var device = this.GetConfiguredDevice();
                return device?.AudioMeterInformation.MasterPeakValue ?? 0d;
            }
            catch
            {
                return 0d;
            }
        }

        /// <summary>
        /// Gets the friendly name of the configured capture device.
        /// </summary>
        public string GetConfiguredDeviceName()
        {
            try
            {
                using var device = this.GetConfiguredDevice();
                return device?.FriendlyName ?? "Unknown device";
            }
            catch
            {
                return "Unknown device";
            }
        }

        private MMDevice GetConfiguredDevice()
        {
            string deviceId = SWYH.Properties.Settings.Default.AudioDevice;
            if (string.IsNullOrEmpty(deviceId))
            {
                return this.enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            }
            return this.enumerator.GetDevice(deviceId);
        }

        public void Dispose()
        {
            this.enumerator.Dispose();
        }
    }
}
