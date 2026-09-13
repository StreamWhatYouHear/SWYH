/*
 *	 Stream What Your Hear
 *	 Assembly: SWYH
 *	 File: RecordViewModel.cs
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
    using Microsoft.Win32;
    using SWYH.Services;
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Backs the "Record What You Hear" page.
    /// </summary>
    internal sealed partial class RecordViewModel : ObservableObject, IDisposable
    {
        private readonly ISwyhService swyhService;
        private readonly DispatcherTimer timer = new() { Interval = TimeSpan.FromMilliseconds(50) };

        private DateTime startDate = DateTime.MinValue;

        [ObservableProperty]
        private bool isRecording;

        [ObservableProperty]
        private string elapsedText = "0:00:00.000";

        public IRelayCommand RecordCommand { get; }

        public IRelayCommand StopCommand { get; }

        public RecordViewModel(ISwyhService swyhService)
        {
            this.swyhService = swyhService;
            this.timer.Tick += this.Timer_Tick;
            this.RecordCommand = new RelayCommand(this.Record);
            this.StopCommand = new RelayCommand(this.Stop);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var recordingTime = DateTime.Now.Subtract(this.startDate);
            this.ElapsedText = string.Format(@"{0}:{1}:{2}.{3}", Math.Truncate(recordingTime.TotalHours), recordingTime.Minutes.ToString("00"), recordingTime.Seconds.ToString("00"), recordingTime.Milliseconds.ToString("000"));
        }

        private void Record()
        {
            this.timer.Start();
            this.IsRecording = true;
            this.startDate = DateTime.Now;
            this.swyhService.StartRecording();
        }

        private void Stop()
        {
            this.timer.Stop();
            this.IsRecording = false;
            this.ElapsedText = "0:00:00.000";
            byte[] content = this.swyhService.StopRecording();
            if (content.Length == 0)
            {
                MessageBox.Show("No Recording Data Found", "Record What You Hear", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog()
            {
                Title = "Save the stream",
                DefaultExt = "mp3",
                AddExtension = true,
                Filter = "MP3 File|*.mp3",
                OverwritePrompt = true
            };
            if (sfd.ShowDialog() != true)
            {
                return;
            }

        saveFile:
            try
            {
                if (File.Exists(sfd.FileName))
                {
                    File.Delete(sfd.FileName);
                }
                File.WriteAllBytes(sfd.FileName, content);
                MessageBox.Show("Recording saved as " + sfd.FileName, "Record What You Hear", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                if (MessageBox.Show(string.Format("A file error has occurred : '{0}'\nDo you want to retry ?", ex.Message), "Record What You Hear", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    goto saveFile;
                }
            }
        }

        public void Dispose()
        {
            this.timer.Stop();
            this.timer.Tick -= this.Timer_Tick;
        }
    }
}
