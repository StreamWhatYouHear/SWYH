/*   
Copyright 2006 - 2010 Intel Corporation

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Xml;
using System.Text;
using OpenSource.UPnP;
using System.Reflection;
using System.Collections;
using OpenSource.Utilities;
using System.Runtime.Serialization;

namespace OpenSource.UPnP.AV.CdsMetadata
{

	/// <summary>
	/// This class helps in mapping property names and enumerations to
	/// the actual objects that should be instantiated for them.
	/// The class also helps in determining if the property allows
	/// multiple values.
	/// </summary>
	[Serializable()]
	public class PropertyMappings
	{
		/// <summary>
		/// Takes the property name as string and returns the type of
		/// object that needs to be instantiated as well as an indication
		/// of whether the property allows multiple values.
		/// </summary>
		/// <param name="propertyName">
		/// A property name, that can map to 
		/// <see cref="CommonPropertyNames"/>.
		/// </param>
		/// <param name="isMultiple">true, if the property allows multiple values</param>
		/// <returns></returns>
		public static System.Type PropertyNameToType(string propertyName, out bool isMultiple)
		{
			if (propertyName == T[CommonPropertyNames.actor])
			{
				isMultiple = true;
				return typeof(PropertyPersonWithRole);
			}
			else if (propertyName == T[CommonPropertyNames.album])
			{
				isMultiple = false;
				return typeof(PropertyString);
			}
			else if (propertyName == T[CommonPropertyNames.albumArtURI])
			{
				isMultiple = true;
				return typeof(PropertyUri);
			}
			else if (propertyName == T[CommonPropertyNames.artist])
			{
				isMultiple = true;
				return typeof(PropertyPersonWithRole);
			}
			else if (propertyName == T[CommonPropertyNames.artistDiscographyURI])
			{
				isMultiple = false;
				return typeof(PropertyUri);
			}
			else if (propertyName == T[CommonPropertyNames.author])
			{
				isMultiple = true;
				return typeof(PropertyPersonWithRole);
			}
			else if (propertyName == T[CommonPropertyNames.channelName])
			{
				isMultiple = false;
				return typeof(PropertyString);
			}
			else if (propertyName == T[CommonPropertyNames.channelNr])
			{
				isMultiple = false;
				return typeof(PropertyInt);
			}
			else if (propertyName == T[CommonPropertyNames.Class])
			{
				isMultiple = false;
				return typeof(MediaClass);
			}
			else if (propertyName == T[CommonPropertyNames.contributor])
			{
				isMultiple = true;
				return typeof(PropertyString);
			}
			else if (propertyName == T[CommonPropertyNames.createClass])
			{
				isMultiple = true;
				return typeof(CreateClass);
			}
			else if (propertyName == T[CommonPropertyNames.creator])
			{
				isMultiple = false;
				return typeof(PropertyString);
			}
			else if (propertyName == T[CommonPropertyNames.date])
			{
				isMultiple = false;
				return typeof(PropertyDate);
			}
			else if (propertyName == T[CommonPropertyNames.description])
			{
				isMultiple = false;
				return typeof(PropertyString);
			}
			else if (propertyName == T[CommonPropertyNames.director])
			{
				isMultiple = false;
				return typeof(PropertyString);
			}
			else if (propertyName == T[CommonPropertyNames.DVDRegionCode])
			{
				isMultiple = false;
				return typeof(PropertyInt);
			}
			else if (propertyName == T[CommonPropertyNames.genre])
			{
				isMultiple = true;
				return typeof(PropertyString);
			}
			else if (propertyName == T[CommonPropertyNames.icon])
			{
				isMultiple = false;
				return typeof(PropertyUri);
			}
			else if (propertyName == T[CommonPropertyNames.language])
			{
				isMultiple = false;
				return typeof(PropertyLanguage);
			}
			else if (propertyName == T[CommonPropertyNames.longDescription])
			{
				isMultiple = false;
				return typeof(PropertyString);
			}
			else if (propertyName == T[CommonPropertyNames.lyricsURI])
			{
				isMultiple = false;
				return typeof(PropertyUri);
			}
			else if (propertyName == T[CommonPropertyNames.originalTrackNumber])
			{
				isMultiple = false;
				return typeof(PropertyInt);
			}
			else if (propertyName == T[CommonPropertyNames.playlist])
			{
				isMultiple = true;
				return typeof(PropertyString);
			}
			else if (propertyName == T[CommonPropertyNames.producer])
			{
				isMultiple = false;
				return typeof(PropertyString);
			}
			else if (propertyName == T[CommonPropertyNames.publisher])
			{
				isMultiple = false;
				return typeof(PropertyString);
			}
			else if (propertyName == T[CommonPropertyNames.radioBand])
			{
				isMultiple = false;
				return typeof(PropertyString);
			}
			else if (propertyName == T[CommonPropertyNames.radioCallSign])
			{
				isMultiple = false;
				return typeof(PropertyString);
			}
			else if (propertyName == T[CommonPropertyNames.radioStationID])
			{
				isMultiple = false;
				return typeof(PropertyString);
			}
			else if (propertyName == T[CommonPropertyNames.rating])
			{
				isMultiple = false;
				return typeof(PropertyString);
			}
			else if (propertyName == T[CommonPropertyNames.region])
			{
				isMultiple = false;
				return typeof(PropertyString);
			}
			else if (propertyName == T[CommonPropertyNames.relation])
			{
				isMultiple = true;
				return typeof(PropertyUri);
			}
			else if (propertyName == T[CommonPropertyNames.rights])
			{
				isMultiple = true;
				return typeof(PropertyString);
			}
			else if (propertyName == T[CommonPropertyNames.role])
			{
				isMultiple = true;
				return typeof(PropertyString);
			}
			else if (propertyName == T[CommonPropertyNames.scheduledEndTime])
			{
				isMultiple = true;
				return typeof(PropertyDateTime);
			}
			else if (propertyName == T[CommonPropertyNames.scheduledStartTime])
			{
				isMultiple = true;
				return typeof(PropertyDateTime);
			}
			else if (propertyName == T[CommonPropertyNames.searchClass])
			{
				isMultiple = true;
				return typeof(SearchClass);
			}
			else if (propertyName == T[CommonPropertyNames.storageFree])
			{
				isMultiple = false;
				return typeof(PropertyLong);
			}
			else if (propertyName == T[CommonPropertyNames.storageMaxPartition])
			{
				isMultiple = false;
				return typeof(PropertyLong);
			}
			else if (propertyName == T[CommonPropertyNames.storageMedium])
			{
				isMultiple = false;
				return typeof(PropertyStorageMedium);
			}
			else if (propertyName == T[CommonPropertyNames.storageTotal])
			{
				isMultiple = false;
				return typeof(PropertyLong);
			}
			else if (propertyName == T[CommonPropertyNames.storageUsed])
			{
				isMultiple = true;
				return typeof(PropertyLong);
			}
			else if (propertyName == T[CommonPropertyNames.title])
			{
				isMultiple = false;
				return typeof(PropertyString);
			}
			else if (propertyName == T[CommonPropertyNames.toc])
			{
				isMultiple = false;
				return typeof(PropertyString);
			}
			else if (propertyName == T[CommonPropertyNames.userAnnotation])
			{
				isMultiple = true;
				return typeof(PropertyString);
			}
			else if (propertyName == T[CommonPropertyNames.writeStatus])
			{
				isMultiple = false;
				return typeof(PropertyEnumWriteStatus);
			}
			else 
			{
				isMultiple = false;
				return null;
			}
		}

		private static Tags T = Tags.GetInstance();
	}
}
