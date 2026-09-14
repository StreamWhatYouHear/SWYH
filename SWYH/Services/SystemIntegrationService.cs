/*
 *	 Stream What Your Hear
 *	 Assembly: SWYH
 *	 File: SystemIntegrationService.cs
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
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Applies the system-level options (process priority, sleep inhibition, muting the local audio endpoints).
    /// </summary>
    internal static class SystemIntegrationService
    {
        private static bool localAudioMuted;

        private static class NativeMethods
        {
            [DllImport("kernel32.dll")]
            public static extern uint SetThreadExecutionState(uint esFlags);

            public const uint ES_CONTINUOUS = 0x80000000;
            public const uint ES_SYSTEM_REQUIRED = 0x00000001;
        }

        /// <summary>
        /// Raises (or restores) the priority of the SWYH process to avoid stuttering when the CPU is busy.
        /// </summary>
        public static void ApplyProcessPriority(bool high)
        {
            try
            {
                using var process = Process.GetCurrentProcess();
                process.PriorityClass = high ? ProcessPriorityClass.AboveNormal : ProcessPriorityClass.Normal;
            }
            catch { }
        }

        /// <summary>
        /// Prevents (or allows again) the system from going to sleep. The screen saver is still allowed.
        /// </summary>
        public static void ApplyPreventSleep(bool prevent)
        {
            try
            {
                NativeMethods.SetThreadExecutionState(prevent
                    ? NativeMethods.ES_CONTINUOUS | NativeMethods.ES_SYSTEM_REQUIRED
                    : NativeMethods.ES_CONTINUOUS);
            }
            catch { }
        }

        /// <summary>
        /// Mutes or unmutes every active local playback device. Only unmutes what SWYH has muted itself.
        /// </summary>
        public static void SetLocalAudioMuted(bool muted)
        {
            if (muted == localAudioMuted)
            {
                return;
            }

            try
            {
                using var enumerator = new MMDeviceEnumerator();
                foreach (var device in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
                {
                    try
                    {
                        device.AudioEndpointVolume.Mute = muted;
                    }
                    catch { }
                }
                localAudioMuted = muted;
            }
            catch (Exception)
            {
                // Nothing we can do if the endpoints cannot be enumerated.
            }
        }
    }
}
