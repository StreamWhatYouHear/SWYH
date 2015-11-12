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
using System.Text;
using OpenSource.UPnP.AV;

namespace OpenSource.UPnP.AV.CdsMetadata
{
	/// <summary>
	/// Represent the names of attributes applicable to resources.
	/// The order of these attributes should match as in the ones
	/// in <see cref="_ATTRIB"/>.
	/// </summary>
	public enum _RESATTRIB
	{
		/// <summary>
		/// required for res
		/// </summary>
		protocolInfo = _ATTRIB.protocolInfo,

		/// <summary>
		/// optional for res
		/// </summary>
		importUri,		

		/// <summary>
		/// optional for res
		/// </summary>
		size,
		/// <summary>
		/// optional for res
		/// </summary>
		duration,
		/// <summary>
		/// optional for res
		/// </summary>
		bitrate,
		/// <summary>
		/// optional for res
		/// </summary>
		sampleFrequency,
		/// <summary>
		/// optional for res
		/// </summary>
		bitsPerSample,
		/// <summary>
		/// optional for res
		/// </summary>
		nrAudioChannels,
		/// <summary>
		/// optional for res
		/// </summary>
		resolution,
		/// <summary>
		/// optional for res
		/// </summary>
		colorDepth,
		/// <summary>
		/// optional for res
		/// </summary>
		protection,

	}

	/// <summary>
	/// Various attributes in all 3 (upnp:, dc:, DIDL-Lite) namespaces tags.
	/// </summary>
	public enum _ATTRIB
	{
		/// <summary>
		/// required for item, container
		/// </summary>
		id,
		/// <summary>
		/// required for item, container
		/// </summary>
		restricted,
		/// <summary>
		/// optional for item, container
		/// </summary>
		parentID,
		/// <summary>
		/// optional for item, required by
		/// item if the item is a reference
		/// to another item
		/// </summary>
		refID,
		/// <summary>
		/// optional for container
		/// </summary>
		childCount,
		/// <summary>
		/// optional for container
		/// </summary>
		searchable,

		/// <summary>
		/// required for res
		/// </summary>
		protocolInfo,
		/// <summary>
		/// optional for res
		/// </summary>
		importUri,

		/// <summary>
		/// optional for res
		/// </summary>
		size,
		/// <summary>
		/// optional for res
		/// </summary>
		duration,
		/// <summary>
		/// optional for res
		/// </summary>
		bitrate,
		/// <summary>
		/// optional for res
		/// </summary>
		sampleFrequency,
		/// <summary>
		/// optional for res
		/// </summary>
		bitsPerSample,
		/// <summary>
		/// optional for res
		/// </summary>
		nrAudioChannels,
		/// <summary>
		/// optional for res
		/// </summary>
		resolution,
		/// <summary>
		/// optional for res
		/// </summary>
		colorDepth,
		/// <summary>
		/// optional for res
		/// </summary>
		protection,

		/// <summary>
		/// required for upnp:searchClass and upnp:createClass
		/// </summary>
		includeDerived,
		/// <summary>
		/// optional for upnp:class, upnp:searchClass, and upnp:createClass
		/// </summary>
		name,

		/// <summary>
		/// optional for upnp:person
		/// </summary>
		role,
	}

	/// <summary>
	/// Enumerates the optional resource properties.
	/// The declared order of the resources should match
	/// the _RESATTRIB enumerator.
	/// </summary>
	public enum OptionalResourceProperties
	{
		size = _RESATTRIB.size,
		duration,
		bitrate,
		sampleFrequency,
		bitsPerSample,
		nrAudioChannels,
		resolution,		
		colorDepth,		
		protection,		
	}

	/// <summary>
	/// Enumeration of dublin-core tag names.
	/// The declared order should match the
	/// CommonPropertyNames enumerator.
	/// </summary>
	public enum _DC
	{
		title = CommonPropertyNames.title,
		contributor,
		publisher,
		description,
		date,
		language,
		relation,
		rights,
		creator,
	}

	/// <summary>
	/// Enumeration of UPNP-AV defined tag names.
	/// The declared order should match the
	/// CommonPropertyNames enumerator.
	/// </summary>
	public enum _UPNP
	{
		/// <summary>
		/// upnp namespace items
		/// </summary>
		artist = CommonPropertyNames.artist,
		actor,
		author,
		role,
		producer,
		director,

		genre,
		album,
		playlist,

		albumArtURI,
		artistDiscographyURI,
		lyricsURI,

		storageTotal,
		storageUsed,
		storageFree,
		storageMaxPartition,
		storageMedium,

		longDescription,
		icon,
		region,
		rating,

		radioCallSign,
		radioStationID,
		radioBand,

		channelNr,
		channelName,
		scheduledStartTime,
		scheduledEndTime,

		DVDRegionCode,
		originalTrackNumber,
		toc,
		userAnnotation,

		/// <summary>
		/// This is a special case because it is an enumeration type.
		/// </summary>
		writeStatus,

		/// <summary>
		///  These are special cases of the _UPNP enumeration
		///  because they are represented by classes and not
		///  simple data types. 
		/// </summary>
		Class,
		searchClass,
		createClass,
	}

	/// <summary>
	/// Enumeration of DIDL-Lite tags
	/// The declared order should match the
	/// CommonPropertyNames enumerator.
	/// </summary>
	public enum _DIDL
	{
		DIDL_Lite,
		Container,
		Item,
		Res,
		Desc,
	}

	/// <summary>
	/// Enumeration of standard metadata properties that
	/// can be used with the 
	/// <see cref="Tags"/>
	/// class's indexer. This aggregates the _UPNP and _DC 
	/// enumerations.
	/// </summary>
	public enum CommonPropertyNames
	{
		artist,		// upnp namespace items order must be same as _UPNP
		actor,
		author,
		role,
		producer,
		director,

		genre,
		album,
		playlist,

		albumArtURI,
		artistDiscographyURI,
		lyricsURI,

		storageTotal,
		storageUsed,
		storageFree,
		storageMaxPartition,
		storageMedium,

		longDescription,
		icon,
		region,
		rating,

		radioCallSign,
		radioStationID,
		radioBand,

		channelNr,
		channelName,
		scheduledStartTime,
		scheduledEndTime,

		DVDRegionCode,
		originalTrackNumber,
		toc,
		userAnnotation,

		writeStatus,

		Class,
		searchClass,
		createClass,

		DoNotUse,	//not used, reserved for implementation

		title,		//dublin-core tags must be same order as in _DC
		contributor,
		publisher,
		description,
		date,
		language,
		relation,
		rights,
		creator,
	}

	/// <summary>
	/// Enumeration of the values indicating whether
	/// an object's binary resources can be overwritten.
	/// </summary>
	public enum EnumWriteStatus
	{
		UNKNOWN,
		/// <summary>
		/// But the english language says to use this...
		/// ... and we've seen people use this so 
		/// we'll allow both.
		/// </summary>
		WRITEABLE,
		/// <summary>
		/// The spec says to use this...
		/// </summary>
		WRITABLE,
		PROTECTED,
		NOT_WRITABLE,
		MIXED
	}
}

