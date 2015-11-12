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
using System.Collections;

namespace OpenSource.UPnP.AV.CdsMetadata
{
	/// <summary>
	/// IMediaResource provides a standard interface for
	/// reading and writing metadata on an object that
	/// represents a "res" (resource) element in a 
	/// DIDL-Lite document.
	/// <para>
	/// Regarding attributes with values effectively represented by
	/// value types, this resource expects such attributes
	/// to use <see cref="IValueType"/> objects to represent
	/// those values. The reason is that value types do not have
	/// a null value, and attributes on resources are not always
	/// required. The <see cref="IValueType"/> provides a means
	/// of representing value types so that they can also represent
	/// an unassigned/null value.
	/// </para>
	/// <para>
	/// The interface declares a number of "bool Hasxxx" properties.
	/// If an attribute is unassigned/null, then those properties
	/// will return false.
	/// </para>
	/// </summary>
	public interface IMediaResource : ICdsElement, ICloneable
	{
		/// <summary>
		/// Returns true if the bitrate attribute exists for this resources.
		/// </summary>
		bool HasBitrate { get; }
		/// <summary>
		/// Returns true if the bitsPerSample attribute exists for this resources.
		/// </summary>
		bool HasBitsPerSample { get; }
		/// <summary>
		/// Returns true if the colorDepth attribute exists for this resources.
		/// </summary>
		bool HasColorDepth { get; }
		/// <summary>
		/// Returns true if the duration attribute exists for this resources.
		/// </summary>
		bool HasDuration { get; }
		/// <summary>
		/// Returns true if the nrAudioChannels attribute exists for this resources.
		/// </summary>
		bool HasNrAudioChannels { get; }
		/// <summary>
		/// Returns true if the bitrate attribute exists for this resources.
		/// </summary>
		bool HasProtection { get; }
		/// <summary>
		/// Returns true if the resolution attribute exists for this resources.
		/// </summary>
		bool HasResolution { get; }
		/// <summary>
		/// Returns true if the sampleFrequency attribute exists for this resources.
		/// </summary>
		bool HasSampleFrequency { get; }
		/// <summary>
		/// Returns true if the size attribute exists for this resources.
		/// </summary>
		bool HasSize { get; }

		/// <summary>
		/// Gets/Sets the content uri value for the resource.
		/// </summary>
		string ContentUri { get; set; }

		/// <summary>
		/// Gets/Sets the import uri value for the resource.
		/// </summary>
		string ImportUri { get; set; }

		/// <summary>
		/// Gets/Sets the protocolInfo string for the resource.
		/// </summary>
		ProtocolInfoString ProtocolInfo { get; set;}

		/// <summary>
		/// Gets/sets the audio or video bitrate attribute for the resource.
		/// You can assign the property to null or assign the property
		/// to an <see cref="_UInt"/> object who's <see cref="IValueType.IsValid"/>
		/// property returns false to mark the attribute as unassigned/null.
		/// </summary>
		_UInt Bitrate { get; set; }
		/// <summary>
		/// Gets/sets the bitsPerSample attribute for the resource.
		/// You can assign the property to null or assign the property
		/// to an <see cref="_UInt"/> object who's <see cref="IValueType.IsValid"/>
		/// property returns false to mark the attribute as unassigned/null.
		/// </summary>
		_UInt BitsPerSample { get; set; }
		/// <summary>
		/// Gets/sets the image color depth attribute for the resource.
		/// You can assign the property to null or assign the property
		/// to an <see cref="_UInt"/> object who's <see cref="IValueType.IsValid"/>
		/// property returns false to mark the attribute as unassigned/null.
		/// </summary>
		_UInt ColorDepth { get; set; }
		/// <summary>
		/// Gets/sets the duration attribute for the resource.
		/// You can assign the property to null or assign the property
		/// to an <see cref="_TimeSpan"/> object who's <see cref="IValueType.IsValid"/>
		/// property returns false to mark the attribute as unassigned/null.
		/// </summary>
		_TimeSpan Duration { get; set; }
		/// <summary>
		/// Gets/sets the number of audio channels attribute for the resource.
		/// You can assign the property to null or assign the property
		/// to an <see cref="_UInt"/> object who's <see cref="IValueType.IsValid"/>
		/// property returns false to mark the attribute as unassigned/null.
		/// </summary>
		_UInt nrAudioChannels { get; set; }
		/// <summary>
		/// Gets/sets the content protection attribute for the resource.
		/// You can assign the property to null to mark the attribute as unassigned/null.
		/// </summary>
		string Protection { get; set; }
		/// <summary>
		/// Gets/sets the image resolution attribute for the resource.
		/// You can assign the property to null or assign the property
		/// to an <see cref="ImageDimensions"/> object who's <see cref="IValueType.IsValid"/>
		/// property returns false to mark the attribute as unassigned/null.
		/// </summary>
		ImageDimensions Resolution { get; set; }
		/// <summary>
		/// Gets/sets the sampling frequency attribute for the resource.
		/// You can assign the property to null or assign the property
		/// to an <see cref="_UInt"/> object who's <see cref="IValueType.IsValid"/>
		/// property returns false to mark the attribute as unassigned/null.
		/// </summary>
		_UInt SampleFrequency { get; set; }
		/// <summary>
		/// Gets/sets the binary length attribute for the resource.
		/// You can assign the property to null or assign the property
		/// to an <see cref="_ULong"/> object who's <see cref="IValueType.IsValid"/>
		/// property returns false to mark the attribute as unassigned/null.
		/// </summary>
		_ULong Size { get; set; }
		/// <summary>
		/// Gets/Sets the owner <see cref="IUPnPMedia"/> object for this resource.
		/// </summary>
		IUPnPMedia Owner { get; set; }
		/// <summary>
		/// Indexer for doing get/set operations on attributes (not including contentUri value)
		/// for this resource. This indexer expects an attribute name in the 
		/// <see cref="_RESATTRIB"/> enumeration.
		/// </summary>
		object this[_RESATTRIB attrib] { get; set; }
		/// <summary>
		/// Indexer for doing get/set operations on attributes (not including contentUri value)
		/// for this resource. This indexer expects tha name of the attribute in string form.
		/// </summary>
		object this[string attrib] { get; set; }
		/// <summary>
		/// Method updates the metadata of the media resource object using the metadata
		/// of another resource.
		/// </summary>
		/// <param name="newMetadata"></param>
		void UpdateResource (IMediaResource newMetadata);

		/// <summary>
		/// Miscellaneous property for application-logic.
		/// </summary>
		object Tag { get; set; }
	}
}
