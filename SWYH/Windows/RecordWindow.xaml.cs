/*
 *	 Stream What Your Hear
 *	 Assembly: SWYH
 *	 File: RecordWindow.xaml.cs
 *	 Web site: http://www.streamwhatyouhear.com
 *	 Copyright (C) 2012-2017 - Sebastien Warin <http://sebastien.warin.fr> and others	
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

namespace SWYH
{
    using Microsoft.Win32;
    using SWYH.Audio;
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for RecordWindow.xaml
    /// </summary>
    public partial class RecordWindow : Window
    {
        protected bool isRecording = false;
        private DateTime startDate = DateTime.MinValue;
        private DispatcherTimer timer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 0, 10) };

        public RecordWindow()
        {
            InitializeComponent();
            timer.Tick += new EventHandler(timer_Tick);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (this.isRecording)
            {
                var recordingTime = DateTime.Now.Subtract(this.startDate);
                this.tbTime.Text = string.Format(@"{0}:{1}:{2}.{3}", Math.Truncate(recordingTime.TotalHours), recordingTime.Minutes.ToString("00"), recordingTime.Seconds.ToString("00"), recordingTime.Milliseconds.ToString("000"));
            }
            else if(!this.isRecording && this.tbTime.Text != "0:00:00.000")
            {
                this.tbTime.Text = "0:00:00.000";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.UpdateUi();
        }

        private void btRecord_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
            this.isRecording = true;
            this.startDate = DateTime.Now;
            App.CurrentInstance.swyhDevice.sessionMp3Streams.GetOrAdd(Int32.MinValue, new PipeStream());
            App.CurrentInstance.wasapiProvider.UpdateClientsList();
            this.UpdateUi();
        }

        private void btStop_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            this.isRecording = false;
            PipeStream value = null;
            while (!App.CurrentInstance.swyhDevice.sessionMp3Streams.TryRemove(Int32.MinValue, out value)) ; ;
            App.CurrentInstance.wasapiProvider.UpdateClientsList();
            if (value.Length > 0)
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
                    byte[] content = new byte[value.Length];
                    if (value.Read(content, 0, content.Length) >= 0)
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            if (this.isRecording)
            {
                MessageBoxResult msg = MessageBox.Show("The current recording will be lost !\nAre you sure to exit ?", "Record What You Hear", MessageBoxButton.YesNo, MessageBoxImage.Hand);
                if (msg == MessageBoxResult.No || msg == MessageBoxResult.None)
                {
                    return;
                }
                this.isRecording = false;
                PipeStream value = null;
                while (!App.CurrentInstance.swyhDevice.sessionMp3Streams.TryRemove(Int32.MinValue, out value)) ; ;
                App.CurrentInstance.wasapiProvider.UpdateClientsList();
                this.UpdateUi();
            }
            this.Hide();
            timer.Stop();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                this.Close();
            }
        }
    }
}
