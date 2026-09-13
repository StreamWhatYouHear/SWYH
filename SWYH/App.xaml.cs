/*
 *	 Stream What Your Hear
 *	 Assembly: SWYH
 *	 File: App.xaml.cs
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

namespace SWYH
{
    using Semver;
    using SWYH.Audio;
    using SWYH.Services;
    using SWYH.Views;
    using SWYH.Views.Pages;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Media;
    using Wpf.Ui.Appearance;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static App CurrentInstance { get { return Application.Current as App; } }
        public static bool NeedUpdate { get; private set; }

        internal ISwyhService SwyhService { get; private set; }

        private string[] autoStreamTo = new string[0];

        private MainWindow mainWindow = null;
        private System.Windows.Forms.NotifyIcon notifyIcon = null;
        private bool directClose = false;   //Skip the statements in Application_Exit function.


        internal static string ExecutablePath
        {
            get
            {
                var processPath = Environment.ProcessPath;
                if (!string.IsNullOrEmpty(processPath))
                {
                    return processPath;
                }

                processPath = Process.GetCurrentProcess().MainModule.FileName;
                if (!string.IsNullOrEmpty(processPath))
                {
                    return processPath;
                }

                return Path.Combine(AppContext.BaseDirectory, Constants.SWYH_PROCESS_NAME + ".exe");
            }
        }

        internal static FileVersionInfo FileVersion => FileVersionInfo.GetVersionInfo(ExecutablePath);

        internal static SemVersion Version => new SemVersion(
            App.FileVersion.ProductMajorPart,
            Math.Max(App.FileVersion.ProductMinorPart, 0),
            Math.Max(App.FileVersion.ProductBuildPart, 0));

        internal static void OpenUrl(string url)
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }

        internal static void Restart()
        {
            Process.Start(new ProcessStartInfo(ExecutablePath) { Arguments = Constants.RESTART_ARGUMENT_NAME, UseShellExecute = true });
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var startTest = 0;
        start:
            var existingProcess = Process.GetProcessesByName(Constants.SWYH_PROCESS_NAME);
            if (existingProcess != null && existingProcess.Length > 1)
            {
                if (e.Args != null && e.Args.Contains(Constants.RESTART_ARGUMENT_NAME) && (startTest++) < Constants.NUMBER_OF_RESTART_TEST)
                {
                    Thread.Sleep(100);
                    goto start;
                }
                MessageBox.Show("Stream What You Hear is already running !", "Stream What You Hear", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                directClose = true;   //Skip the statements in Application_Exit function, otherwise it will casue null object exception
                this.Shutdown();
            }
            else
            {
                if (SWYH.Properties.Settings.Default.Debug)
                {
                    AppDomain.CurrentDomain.UnhandledException += (ss, ee) =>
                    {
                        var ex = (Exception)ee.ExceptionObject;
                        StringBuilder error = new StringBuilder();
                        error.AppendLine("Date: " + DateTime.Now.ToString());
                        error.AppendLine("Message: " + ex.Message);
                        error.AppendLine("Detail: " + ex.ToString());
                        error.AppendLine("------------------------------");
                        File.AppendAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), Constants.SWYH_CRASHLOG_FILENAME), error.ToString());
                        MessageBox.Show("An unhandled error has occured ! See the '" + Constants.SWYH_CRASHLOG_FILENAME + "' on your desktop for more information", "Stream What You Hear", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    };
                }
                if (new NAudio.CoreAudioApi.MMDeviceEnumerator().EnumerateAudioEndPoints(NAudio.CoreAudioApi.DataFlow.Render, NAudio.CoreAudioApi.DeviceState.Active).Count == 0)    //Check the available interface.
                {
                    System.Windows.Forms.MessageBox.Show("Unable to find your sound interface, please check your sound interface in control panel.", "Cannot find the sound interface");
                    directClose = true;    //Skip the statements in Application_Exit function, otherwise it will casue null object exception
                    this.Shutdown();
                }
                else
                {
                    this.CheckAutomaticDeviceStreamed(e);
                    this.CheckNewVersion();
                    ApplicationAccentColorManager.Apply(Color.FromRgb(0x2F, 0x7C, 0xF6), ApplicationTheme.Dark);
                    this.SwyhService = new SwyhService();
                    this.SwyhService.SetAutoStreamTargets(this.autoStreamTo);
                    this.SwyhService.Start();
                    this.InitializeUI();
                }
            }
        }

        private void InitializeUI()
        {
            FileVersionInfo fileVersion = App.FileVersion;
            this.mainWindow = new MainWindow();
            this.MainWindow = this.mainWindow;
            this.mainWindow.Show();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon()
            {
                ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(),
                Icon = SWYH.Properties.Resource.swyh,
                Text = string.Format("Stream What You Hear {0}.{1}{2}", fileVersion.ProductMajorPart, fileVersion.ProductMinorPart, (fileVersion.ProductPrivatePart % 2) == 0 ? "" : " (BETA)"),
                Visible = true
            };
            this.notifyIcon.DoubleClick += (s, e2) => this.ShowMainWindow();
            this.notifyIcon.ContextMenuStrip.Items.Add("Open", null, (s, e2) => this.ShowMainWindow());
            this.notifyIcon.ContextMenuStrip.Items.Add("Settings", null, (s, e2) => this.ShowMainWindow(typeof(SettingsPage)));
            this.notifyIcon.ContextMenuStrip.Items.Add("Website", null, (s, e2) => App.OpenUrl(Constants.SWYH_WEBSITE_URL));
            this.notifyIcon.ContextMenuStrip.Items.Add("About", null, (s, e2) => this.ShowMainWindow(typeof(AboutPage)));
            this.notifyIcon.ContextMenuStrip.Items.Add("-");
            this.notifyIcon.ContextMenuStrip.Items.Add("Exit", null, (s, e2) =>
            {
                this.mainWindow.AllowClose = true;
                this.Shutdown();
            });
        }

        private void ShowMainWindow(Type navigateTo = null)
        {
            if (navigateTo != null)
            {
                this.mainWindow.NavigateTo(navigateTo);
            }
            this.mainWindow.Show();
            this.mainWindow.WindowState = WindowState.Normal;
            this.mainWindow.Activate();
        }

        internal void NotifyMinimizedToTray()
        {
            this.notifyIcon?.ShowBalloonTip(2000, "Stream What You Hear is still running", "It keeps streaming in the background. Click this icon to reopen it.", System.Windows.Forms.ToolTipIcon.Info);
        }

        private void CheckAutomaticDeviceStreamed(StartupEventArgs startupEvent)
        {
            if (startupEvent.Args.Any(a => a.StartsWith(Constants.STREAM_TO_ARGUMENT_NAME, StringComparison.InvariantCultureIgnoreCase)))
            {
                autoStreamTo = startupEvent.Args.First(a => a.StartsWith(Constants.STREAM_TO_ARGUMENT_NAME, StringComparison.InvariantCultureIgnoreCase)).Substring(Constants.STREAM_TO_ARGUMENT_NAME.Length).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        private async void CheckNewVersion()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    string lastVersionStr = (await httpClient.GetStringAsync(Constants.UPDATE_VERSION_URL)).Trim();
                    if (string.IsNullOrEmpty(lastVersionStr))
                    {
                        return;
                    }

                    SemVersion lastVersion;
                    if (!SemVersion.TryParse(lastVersionStr, out lastVersion))
                    {
                        return;
                    }

                    SemVersion currentVersion = App.Version;
                    App.NeedUpdate = (lastVersion > currentVersion);
                }

                if (App.NeedUpdate)
                {
                    var response = MessageBox.Show("A new version of Stream What You Hear is available !\n\nDo you want to download it now?", "Stream What You Hear", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (response == MessageBoxResult.Yes)
                    {
                        App.OpenUrl(Constants.DOWNLOAD_SWYH_URL);
                    }
                }
            }
            catch
            {
                App.NeedUpdate = false;
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (!directClose)
            {
                this.SwyhService?.Dispose();
                if (this.notifyIcon != null)
                {
                    this.notifyIcon.Dispose();
                }
            }
        }
    }
}
