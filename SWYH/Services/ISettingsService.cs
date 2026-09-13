/*
 *	 Stream What Your Hear
 *	 Assembly: SWYH
 *	 File: ISettingsService.cs
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
    using SWYH.Audio;
    using System.Collections.Generic;

    /// <summary>
    /// A capture device the user can pick as the WASAPI audio source.
    /// </summary>
    internal sealed record AudioDeviceOption(string Id, string Name, string Category);

    /// <summary>
    /// A selectable capture format (sample rate / bit depth / channels).
    /// </summary>
    internal sealed record CaptureFormatOption(string DisplayName, NAudio.Wave.WaveFormat Format);

    /// <summary>
    /// Exposes SWYH's persisted settings to the UI layer without leaking <see cref="SWYH.Properties.Settings"/>
    /// or the Windows registry directly into ViewModels.
    /// </summary>
    internal interface ISettingsService
    {
        IReadOnlyList<CaptureFormatOption> AvailableCaptureFormats { get; }

        IReadOnlyList<int> AvailableBitrates { get; }

        IReadOnlyList<AudioDeviceOption> AvailableAudioDevices { get; }

        AudioFormats.Format StreamFormat { get; set; }

        CaptureFormatOption CaptureFormat { get; set; }

        int Mp3Bitrate { get; set; }

        AudioDeviceOption AudioDevice { get; set; }

        bool Debug { get; set; }

        bool UseSpecificHttpPort { get; set; }

        int HttpPort { get; set; }

        bool RunAtWindowsStartup { get; set; }

        /// <summary>
        /// Gets a value indicating whether any setting was changed since this service was created.
        /// </summary>
        bool HasPendingChanges { get; }

        /// <summary>
        /// Persists the current values. Returns <c>true</c> if a restart is required for the changes to take effect.
        /// </summary>
        bool Save();
    }
}
