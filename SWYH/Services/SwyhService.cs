/*
 *	 Stream What Your Hear
 *	 Assembly: SWYH
 *	 File: SwyhService.cs
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
    using OpenSource.UPnP.AV.RENDERER.CP;
    using SWYH.Audio;
    using SWYH.UPnP;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Threading;

    /// <inheritdoc cref="ISwyhService"/>
    internal sealed class SwyhService : ISwyhService
    {
        private const int RecordingSessionKey = int.MinValue;

        private static readonly TimeSpan ConnectionTimeout = TimeSpan.FromSeconds(15);

        private readonly Dispatcher dispatcher;
        private readonly Dictionary<string, RendererViewModel> renderersByUdn = new();

        private AVRendererDiscovery rendererDiscovery;
        private SwyhDevice swyhDevice;
        private WasapiProvider wasapiProvider;
        private string[] autoStreamTo = Array.Empty<string>();
        private DateTime recordingStartedAt = DateTime.MinValue;
        private DispatcherTimer recordingTimer;

        public ObservableCollection<RendererViewModel> Renderers { get; } = new();

        public bool IsRecording { get; private set; }

        public event EventHandler<TimeSpan> RecordingTimeChanged;

        internal SwyhDevice Device => this.swyhDevice;

        public SwyhService()
        {
            this.dispatcher = Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
        }

        public void SetAutoStreamTargets(string[] rendererNames)
        {
            this.autoStreamTo = rendererNames ?? Array.Empty<string>();
        }

        public void Start()
        {
            this.wasapiProvider = new WasapiProvider();
            this.swyhDevice = new SwyhDevice(this.wasapiProvider);
            this.swyhDevice.Start();
            this.rendererDiscovery = new AVRendererDiscovery(this.OnRendererFound);
            this.rendererDiscovery.OnRendererRemoved += this.OnRendererLost;
        }

        public void RescanRenderers()
        {
            this.rendererDiscovery?.ReScan();
        }

        public void ToggleStreamToRenderer(RendererViewModel renderer)
        {
            if (renderer?.Renderer == null || renderer.Renderer.Connections.Count == 0)
            {
                return;
            }

            var connection = (AVConnection)renderer.Renderer.Connections[0];
            if (renderer.IsStreaming)
            {
                connection.Stop();
            }
            else
            {
                var media = this.swyhDevice.ContentDirectory.GetWasapiMediaItem();
                if (media == null)
                {
                    return;
                }
                connection.Stop();
                renderer.IsConnecting = true;
                this.StartConnectionTimeout(renderer);
                renderer.Renderer.CreateConnection(media, DateTime.Now.Ticks);
                connection.Play();
            }
        }

        private void StartConnectionTimeout(RendererViewModel renderer)
        {
            var timeout = new DispatcherTimer(DispatcherPriority.Normal, this.dispatcher) { Interval = ConnectionTimeout };
            timeout.Tick += (s, e) =>
            {
                timeout.Stop();
                renderer.IsConnecting = false;
            };
            timeout.Start();
        }

        public List<string> GetStreamUrls(AudioFormats.Format format)
        {
            return this.swyhDevice?.ContentDirectory.GetWasapiUris(format) ?? new List<string>();
        }

        public void StartRecording()
        {
            if (this.IsRecording)
            {
                return;
            }
            this.IsRecording = true;
            this.recordingStartedAt = DateTime.Now;
            this.swyhDevice.sessionMp3Streams.GetOrAdd(RecordingSessionKey, new PipeStream());
            this.wasapiProvider.UpdateClientsList(this.swyhDevice.sessionMp3Streams.Count, this.swyhDevice.sessionPcmStreams.Count);

            this.recordingTimer = new DispatcherTimer(DispatcherPriority.Normal, this.dispatcher) { Interval = TimeSpan.FromMilliseconds(50) };
            this.recordingTimer.Tick += (s, e) => this.RecordingTimeChanged?.Invoke(this, DateTime.Now - this.recordingStartedAt);
            this.recordingTimer.Start();
        }

        public byte[] StopRecording()
        {
            if (!this.IsRecording)
            {
                return Array.Empty<byte>();
            }

            this.IsRecording = false;
            this.recordingTimer?.Stop();
            this.recordingTimer = null;

            PipeStream recorded = null;
            while (!this.swyhDevice.sessionMp3Streams.TryRemove(RecordingSessionKey, out recorded)) { }
            this.wasapiProvider.UpdateClientsList(this.swyhDevice.sessionMp3Streams.Count, this.swyhDevice.sessionPcmStreams.Count);

            if (recorded == null || recorded.Length <= 0)
            {
                return Array.Empty<byte>();
            }

            byte[] content = new byte[recorded.Length];
            int totalRead = 0;
            while (totalRead < content.Length)
            {
                int read = recorded.Read(content, totalRead, content.Length - totalRead);
                if (read <= 0)
                {
                    break;
                }
                totalRead += read;
            }
            return content;
        }

        public void Stop()
        {
            foreach (var renderer in this.Renderers.Where(r => r.IsStreaming))
            {
                this.ToggleStreamToRenderer(renderer);
            }
            this.swyhDevice?.Stop();
            this.wasapiProvider?.Dispose();
            SystemIntegrationService.SetLocalAudioMuted(false);
        }

        public void Dispose()
        {
            this.Stop();
        }

        private void OnRendererFound(AVRendererDiscovery sender, AVRenderer renderer)
        {
            this.dispatcher.BeginInvoke(new Action(() =>
            {
                if (this.renderersByUdn.ContainsKey(renderer.UniqueDeviceName))
                {
                    return;
                }

                var vm = new RendererViewModel(renderer);
                this.renderersByUdn[renderer.UniqueDeviceName] = vm;
                this.Renderers.Add(vm);

                if (renderer.Connections.Count > 0)
                {
                    var connection = (AVConnection)renderer.Connections[0];
                    connection.OnPlayStateChanged += (s, state) => this.OnPlayStateChanged(vm, state);
                    if (this.autoStreamTo.Any(name => name.Equals(renderer.FriendlyName, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        this.ToggleStreamToRenderer(vm);
                    }
                }
            }));
        }

        private void OnRendererLost(AVRendererDiscovery sender, AVRenderer renderer)
        {
            this.dispatcher.BeginInvoke(new Action(() =>
            {
                if (this.renderersByUdn.Remove(renderer.UniqueDeviceName, out var vm))
                {
                    this.Renderers.Remove(vm);
                }
            }));
        }

        private void OnPlayStateChanged(RendererViewModel renderer, AVConnection.PlayState newState)
        {
            this.dispatcher.BeginInvoke(new Action(() =>
            {
                renderer.IsStreaming = newState == AVConnection.PlayState.PLAYING;
                renderer.IsConnecting = false;
                if (SWYH.Properties.Settings.Default.MuteWhileStreaming)
                {
                    SystemIntegrationService.SetLocalAudioMuted(this.Renderers.Any(r => r.IsStreaming));
                }
            }));
        }
    }
}
