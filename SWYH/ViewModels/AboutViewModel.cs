/*
 *	 Stream What Your Hear
 *	 Assembly: SWYH
 *	 File: AboutViewModel.cs
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

    /// <summary>
    /// Backs the About page.
    /// </summary>
    internal sealed partial class AboutViewModel : ObservableObject
    {
        public string VersionText { get; }

        public bool IsUpdateAvailable { get; }

        public IRelayCommand UpdateCommand { get; }

        public AboutViewModel()
        {
            var versionStr = App.Version?.ToString() ?? App.FileVersion.ProductVersion;
            this.VersionText = string.Format("Version {0}", versionStr);
            this.IsUpdateAvailable = App.NeedUpdate;
            this.UpdateCommand = new RelayCommand(() => App.OpenUrl(Constants.DOWNLOAD_SWYH_URL));
        }
    }
}
