/*
 *	 Stream What Your Hear
 *	 Assembly: SWYH
 *	 File: VolumeWindow.xaml.cs
 *	 Web site: http://www.streamwhatyouhear.com
 *	 Copyright (C) 2012-2019 - Sebastien Warin <http://sebastien.warin.fr> and others
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
    using System;
    using System.Windows;

    /// <summary>
    /// Interaction logic for VolumeWindow.xaml
    /// </summary>
    public partial class VolumeWindow : Window
    {
        public VolumeWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.slider1.Value = SWYH.Properties.Settings.Default.Volume;
            this.checkBox1.IsChecked = SWYH.Properties.Settings.Default.Mute;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                this.Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            int volume = (int)this.slider1.Value;
            if (SWYH.Properties.Settings.Default.Volume != volume ||
                SWYH.Properties.Settings.Default.Mute != this.checkBox1.IsChecked)
            {
                SWYH.Properties.Settings.Default.Volume = volume;
                SWYH.Properties.Settings.Default.Mute = (bool)this.checkBox1.IsChecked;
                SWYH.Properties.Settings.Default.Save();
            }

            this.Hide();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Close();
        }

        public void ShowWithPosition()
        {
            this.Show();
            System.Drawing.Point point = System.Windows.Forms.Control.MousePosition;
            var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
            var mouse = transform.Transform(new System.Windows.Point(point.X, point.Y));
            Left = mouse.X - ActualWidth;
            Top = mouse.Y - ActualHeight;
            this.Activate();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (App.CurrentInstance.wasapiProvider != null)
            {
                App.CurrentInstance.wasapiProvider.SetVolume((int)e.NewValue, (bool)this.checkBox1.IsChecked);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (App.CurrentInstance.wasapiProvider != null)
            {
                App.CurrentInstance.wasapiProvider.SetVolume((int)this.slider1.Value, true);
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (App.CurrentInstance.wasapiProvider != null)
            {
                App.CurrentInstance.wasapiProvider.SetVolume((int)this.slider1.Value, false);
            }
        }
    }
}
