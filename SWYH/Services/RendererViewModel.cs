/*
 *	 Stream What Your Hear
 *	 Assembly: SWYH
 *	 File: RendererViewModel.cs
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
    using CommunityToolkit.Mvvm.ComponentModel;
    using OpenSource.UPnP.AV.RENDERER.CP;

    /// <summary>
    /// UI-friendly representation of an UPnP/DLNA media renderer discovered on the network.
    /// </summary>
    internal sealed partial class RendererViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool isStreaming;

        public string Name { get; }

        public string UniqueDeviceName { get; }

        internal AVRenderer Renderer { get; }

        internal RendererViewModel(AVRenderer renderer)
        {
            this.Renderer = renderer;
            this.Name = renderer.FriendlyName;
            this.UniqueDeviceName = renderer.UniqueDeviceName;
        }
    }
}
