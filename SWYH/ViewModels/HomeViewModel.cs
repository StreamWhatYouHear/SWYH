/*
 *	 Stream What Your Hear
 *	 Assembly: SWYH
 *	 File: HomeViewModel.cs
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
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Backs the dashboard (Home) page: streaming status, audio level, output URLs and renderers.
    /// </summary>
    internal sealed partial class HomeViewModel : ObservableObject, IDisposable
    {
        private const int LevelHistorySize = 48;
        private const double LevelBarMaxHeight = 64;
        private const double LevelBarMinHeight = 3;

        private readonly ISwyhService swyhService;
        private readonly AudioLevelService audioLevelService = new();
        private readonly DispatcherTimer refreshTimer;

        private int slowRefreshCounter;

        public ObservableCollection<RendererViewModel> Renderers => this.swyhService.Renderers;

        public ObservableCollection<double> LevelHistory { get; } = new(Enumerable.Repeat(LevelBarMinHeight, LevelHistorySize));

        public ObservableCollection<string> OutputUrls { get; } = new();

        [ObservableProperty]
        private string captureDeviceName = "Default device";

        [ObservableProperty]
        private string devicesHeaderText = "Devices";

        public IRelayCommand<string> CopyUrlCommand { get; }

        public IRelayCommand<RendererViewModel> ToggleRendererCommand { get; }

        public IRelayCommand RefreshDevicesCommand { get; }

        public HomeViewModel(ISwyhService swyhService)
        {
            this.swyhService = swyhService;
            this.CopyUrlCommand = new RelayCommand<string>(this.CopyUrl);
            this.ToggleRendererCommand = new RelayCommand<RendererViewModel>(this.swyhService.ToggleStreamToRenderer);
            this.RefreshDevicesCommand = new RelayCommand(this.swyhService.RescanRenderers);

            this.RefreshOutputUrls();
            this.captureDeviceName = this.audioLevelService.GetConfiguredDeviceName();

            this.refreshTimer = new DispatcherTimer(DispatcherPriority.Background) { Interval = TimeSpan.FromMilliseconds(120) };
            this.refreshTimer.Tick += this.RefreshTimer_Tick;
            this.refreshTimer.Start();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            this.LevelHistory.RemoveAt(0);
            double peak = this.audioLevelService.GetCurrentPeak();
            this.LevelHistory.Add(Math.Clamp(peak * LevelBarMaxHeight, LevelBarMinHeight, LevelBarMaxHeight));

            this.DevicesHeaderText = this.Renderers.Count == 1 ? "1 device found" : $"{this.Renderers.Count} devices found";

            // Device enumeration is expensive: only refresh the capture device name ~once per second.
            if (++this.slowRefreshCounter >= 8)
            {
                this.slowRefreshCounter = 0;
                this.CaptureDeviceName = this.audioLevelService.GetConfiguredDeviceName();
                this.RefreshOutputUrls();
            }
        }

        private void RefreshOutputUrls()
        {
            var urls = this.swyhService.GetStreamUrls(AudioFormats.Format.Mp3);
            if (this.OutputUrls.SequenceEqual(urls))
            {
                return;
            }

            this.OutputUrls.Clear();
            foreach (var url in urls)
            {
                this.OutputUrls.Add(url);
            }
        }

        private void CopyUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                Clipboard.SetText(url);
            }
        }

        public void Dispose()
        {
            this.refreshTimer.Stop();
            this.refreshTimer.Tick -= this.RefreshTimer_Tick;
            this.audioLevelService.Dispose();
        }
    }
}
