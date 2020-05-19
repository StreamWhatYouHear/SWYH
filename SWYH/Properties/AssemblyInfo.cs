/*
 *	 Stream What Your Hear
 *	 Assembly: SWYH
 *	 File: AssemblyInfo.cs
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

using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

[assembly: Guid("2ecd245f-4268-48ee-b0f1-0706ead1ba42")]
[assembly: AssemblyTitle("Stream What You Hear")]
[assembly: AssemblyDescription("Stream What You Hear (SWYH) is a Windows application to stream the sound from your PC to an UPnP / DLNA device")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Sebastien.warin.fr")]
[assembly: AssemblyProduct("Stream What You Hear (SWYH)")]
[assembly: AssemblyCopyright("Copyright © 2012-2019 Sebastien.warin.fr ans others")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                                     //(used if a resource is not found in the page, 
                                     // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                              //(used if a resource is not found in the page, 
                                              // app, or any theme specific resource dictionaries)
)]

// Version 10.0 for local development
// Automatically update by the build pipeline with GitVersion
[assembly: AssemblyVersion("10.0.0.0")]
[assembly: AssemblyFileVersion("10.0.0.0")]