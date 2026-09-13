/*
 *	 Stream What Your Hear
 *	 Assembly: SWYH
 *	 File: RecordPage.xaml.cs
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
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for RecordPage.xaml
    /// </summary>
    public partial class RecordPage : Page
    {
        private bool isRecording = false;
        private DateTime startDate = DateTime.MinValue;
        private readonly DispatcherTimer timer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 0, 50) };

        public RecordPage()
        {
            InitializeComponent();
            this.timer.Tick += this.Timer_Tick;
            this.UpdateUi();
            this.Unloaded += (s, e) => this.timer.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (this.isRecording)
            {
                var recordingTime = DateTime.Now.Subtract(this.startDate);
                this.tbTime.Text = string.Format(@"{0}:{1}:{2}.{3}", Math.Truncate(recordingTime.TotalHours), recordingTime.Minutes.ToString("00"), recordingTime.Seconds.ToString("00"), recordingTime.Milliseconds.ToString("000"));
            }
        }

        private void btRecord_Click(object sender, RoutedEventArgs e)
        {
            this.timer.Start();
            this.isRecording = true;
            this.startDate = DateTime.Now;
            App.CurrentInstance.SwyhService.StartRecording();
            this.UpdateUi();
        }

        private void btStop_Click(object sender, RoutedEventArgs e)
        {
            this.timer.Stop();
            this.isRecording = false;
            this.tbTime.Text = "0:00:00.000";
            byte[] content = App.CurrentInstance.SwyhService.StopRecording();
            if (content.Length > 0)
            {
                SaveFileDialog sfd = new SaveFileDialog()
                {
                    Title = "Save the stream",
                    DefaultExt = "mp3",
                    AddExtension = true,
                    Filter = "MP3 File|*.mp3",
                    OverwritePrompt = true
                };
                bool? result = sfd.ShowDialog();
                if (result.HasValue && result.Value)
                {
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
            }
            else
            {
                MessageBox.Show("No Recording Data Found", "Record What You Hear", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            this.UpdateUi();
        }

        private void UpdateUi()
        {
            this.btRecord.IsEnabled = !this.isRecording;
            this.btStop.IsEnabled = this.isRecording;
        }
    }
}
