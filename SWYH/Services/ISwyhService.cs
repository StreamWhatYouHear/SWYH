/*
 *	 Stream What Your Hear
 *	 Assembly: SWYH
 *	 File: ISwyhService.cs
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

namespace SWYH.Services
{
    using SWYH.Audio;
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Exposes the SWYH streaming engine (UPnP device, audio capture and renderer discovery)
    /// to the UI layer without leaking the underlying UPnP/NAudio types.
    /// </summary>
    internal interface ISwyhService : IDisposable
    {
        /// <summary>
        /// Renderers discovered on the network, observable from the UI thread.
        /// </summary>
        ObservableCollection<RendererViewModel> Renderers { get; }

        /// <summary>
        /// Gets a value indicating whether a "Record What You Hear" session is currently active.
        /// </summary>
        bool IsRecording { get; }

        /// <summary>
        /// Raised when the recording elapsed time changes while recording is active.
        /// </summary>
        event EventHandler<TimeSpan> RecordingTimeChanged;

        /// <summary>
        /// Sets the renderer friendly names that should be streamed to automatically once discovered.
        /// </summary>
        void SetAutoStreamTargets(string[] rendererNames);

        /// <summary>
        /// Starts the UPnP device, the audio capture pipeline and the renderer discovery.
        /// </summary>
        void Start();

        /// <summary>
        /// Toggles streaming to the given renderer (start if stopped, stop if playing).
        /// </summary>
        void ToggleStreamToRenderer(RendererViewModel renderer);

        /// <summary>
        /// Gets the HTTP URLs exposing the live audio stream for the given format.
        /// </summary>
        System.Collections.Generic.List<string> GetStreamUrls(AudioFormats.Format format);

        /// <summary>
        /// Starts recording the captured audio in memory.
        /// </summary>
        void StartRecording();

        /// <summary>
        /// Stops the current recording and returns the recorded MP3 bytes (empty array if nothing was captured).
        /// </summary>
        byte[] StopRecording();

        /// <summary>
        /// Stops every active renderer connection, the audio capture and the UPnP device.
        /// </summary>
        void Stop();
    }
}
