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
using System.IO;
using System.Xml;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Collections;
using OpenSource.UPnP.AV;

namespace OpenSource.UPnP.AV.CdsMetadata
{
	/// <summary>
	/// <para>
	/// This class is a class factory for 
	/// <see cref="MediaResource"/>
	/// objects. 
	/// </para>
	/// </summary>
	public class ResourceBuilder
	{
		/// <summary>
		/// Creates a resource, given the contentUri and protocolInfo.
		/// </summary>
		/// <param name="contentUri"></param>
		/// <param name="protocolInfo"></param>
		/// <returns></returns>
		public static MediaResource CreateResource (string contentUri, string protocolInfo)
		{
			ResourceAttributes attribs = new ResourceAttributes();
			attribs.contentUri = contentUri;
			attribs.protocolInfo = CdsMetadataCaches.ProtocolInfoStrings.CacheThis(protocolInfo);

			MediaResource newRes = new MediaResource();
			SetCommonAttributes(newRes, attribs);

			return newRes;
		}

		/// <summary>
		/// Creates a resource, given a metadata block of values.
		/// </summary>
		/// <param name="attribs"></param>
		/// <returns></returns>
		public static MediaResource CreateResource(ResourceAttributes attribs)
		{
			MediaResource newRes = new MediaResource();
			SetCommonAttributes(newRes, attribs);

			return newRes;
		}
		

		/// <summary>
		/// Constructors use this method to set the properties of a resource.
		/// Programmers can use this method also, but the method is not
		/// guaranteed to be thread safe. As a general rule, programmers shouldn't
		/// be changing the values of resources after they've instantiated them.
		/// </summary>
		/// <param name="res"></param>
		/// <param name="attribs"></param>
		public static void SetCommonAttributes(MediaResource res, ResourceAttributes attribs)
		{
			res.ProtocolInfo = attribs.protocolInfo;
			res.ContentUri = attribs.contentUri.Trim();
			
			if ((attribs.importUri != null) && (attribs.importUri != ""))
			{
				res[T[_RESATTRIB.importUri]] = attribs.importUri;
			}

			TransferValue("bitrate", res, attribs);
			TransferValue("bitsPerSample", res, attribs);
			TransferValue("colorDepth", res, attribs);
			TransferValue("duration", res, attribs);
			TransferValue("nrAudioChannels", res, attribs);
			TransferValue("protection", res, attribs);
			TransferValue("resolution", res, attribs);
			TransferValue("sampleFrequency", res, attribs);
			TransferValue("size", res, attribs);
		}
		
		/// <summary>
		/// <para>
		/// This method allows a generalized implementation of transfering
		/// a value from a 
		/// <see cref="ResourceBuilder.ResourceAttributes"/>
		/// to a
		/// <see cref="MediaResource"/>
		/// object. 
		/// </para>
		/// 
		/// <para>
		/// Classes derived from the
		/// <see cref="ResourceBuilder.ResourceAttributes"/>
		/// class have fields, with names that match against fields in a 
		/// <see cref="MediaResource"/>
		/// object. 
		/// </para>
		/// </summary>
		/// <param name="attribName">name of the attribute to transfer</param>
		/// <param name="res">The
		/// <see cref="MediaResource"/>
		/// object to transfer to.
		/// </param>
		/// <param name="attribs">
		/// The
		/// <see cref="ResourceBuilder.ResourceAttributes"/>
		/// object to transfer from.
		/// </param>
		private static void TransferValue(string attribName, MediaResource res, ResourceAttributes attribs)
		{
			object val = null;
			
			System.Type type = attribs.GetType();
			FieldInfo info = type.GetField(attribName);
			if (info != null)
			{
				val = info.GetValue(attribs);

				if (val != null)
				{
					IValueType ivt = val as IValueType;

					bool ok = true;
					if (ivt != null)
					{
						ok = ivt.IsValid;
					}
					if (ok)
					{
						//res.m_Attributes[attribName] = val;
						res[attribName] = val;
					}
				}
			}
		}

		/// <summary>
		/// Allows for easy text-delimited parsing.
		/// </summary>
		private static DText DT = new DText();

		/// <summary>
		/// Gets a static instance that allows easy translation
		/// of CommonPropertyNames enumeration into ContentDirectory element
		/// names and attributes.
		/// </summary>
		private static Tags T = Tags.GetInstance();

		/// <summary>
		/// All metadata fields applicable to a resource
		/// are derived from this empty base class.
		/// </summary>
		public class ResourceAttributes
		{
			public ProtocolInfoString protocolInfo;
			public string contentUri = null;
			public string importUri= null;
			public _ULong size = new _ULong(false);
		}

		/// <summary>
		/// All possible metadata fields applicable to a
		/// resource are in this class. This is useful
		/// for instances of custom metadata.
		/// </summary>
		public class AllResourceAttributes : ResourceAttributes
		{
			/// <summary>
			/// The bitrate of the media. Usually
			/// used for audio and video resources.
			/// </summary>
			public _UInt bitrate = new _UInt(false);
			/// <summary>
			/// The bits for each sample. Usually
			/// used for images.
			/// </summary>
			public _UInt bitsPerSample = new _UInt(false);
			/// <summary>
			/// The number of colors in the media.
			/// Usually used for image.
			/// </summary>
			public _UInt colorDepth = new _UInt(false);
			/// <summary>
			/// Indicates the duration of the resource.
			/// </summary>
			public _TimeSpan duration = new _TimeSpan(false);
			
			/// <summary>
			/// The number of audio channels for the media. Usually for audio or video items.
			/// </summary>
			public _UInt nrAudioChannels = new _UInt(false);

			/// <summary>
			/// Content protection string. Syntax not defined.
			/// </summary>
			public string protection = null;

			/// <summary>
			/// The pixel resolution of the media, in "[width]x[height]" format.
			/// </summary>
			public ImageDimensions resolution = new ImageDimensions(false);

			/// <summary>
			/// The frequency at which samples are taken.
			/// </summary>
			public _UInt sampleFrequency = new _UInt(false);
		}

		/// <summary>
		/// This is the list of metadata fields applicable
		/// for audio resources.
		/// </summary>
		public class AudioItem : ResourceAttributes
		{
			/// <summary>
			/// The bitrate of the media. Usually
			/// used for audio and video resources.
			/// </summary>
			public _UInt bitrate = new _UInt(false);
			
			/// <summary>
			/// The bits for each sample. Usually
			/// used for images.
			/// </summary>
			public _UInt bitsPerSample = new _UInt(false);

			/// <summary>
			/// The number of audio channels for the media. Usually for audio or video items.
			/// </summary>
			public _UInt nrAudioChannels = new _UInt(false);
			
			/// <summary>
			/// Content protection string. Syntax not defined.
			/// </summary>
			public string protection = null;

			/// <summary>
			/// The frequency at which samples are taken.
			/// </summary>
			public _UInt sampleFrequency = new _UInt(false);
		}

		/// <summary>
		/// This is the additional list of metadata fields applicable
		/// for music tracks, in addition to the metadata from
		/// <see cref="ResourceBuilder.AudioItem"/>.
		/// </summary>
		public class MusicTrack : AudioItem
		{
			/// <summary>
			/// The time duration of the media, in the format
			/// "h*:mm:ss:f*" or "h*:mm:ss.f0/f1",
			/// where
			/// <list type="bullet">
			/// <item>
			/// <term>h*</term>
			/// <description>any number of digits (including no digits) to indicated elapsed time in hours</description>
			/// </item>
			/// <item>
			/// <term>mm</term>
			/// <description>0 to 59, to indicate minutes</description>
			/// </item>
			/// <item>
			/// <term>ss</term>
			/// <description>0 to 59, to indicate seconds</description>
			/// </item>
			/// <item>
			/// <term>f*</term>
			/// <description>any number of digits (including no digits) to indicated fractions of seconds</description>
			/// </item>
			/// <item>
			/// <term>f0/f1</term>
			/// <description>a fraction, with f0 and f1 at least one digitl long and f0 is less than f1. This field may be preceded
			/// by an optional + or - sign, and the preceding decimal point may be omitted if no
			/// fractional seconds are listed</description>
			/// </item>
			/// </list>
			/// </summary>
			public _TimeSpan duration = new _TimeSpan(false);

		}

		/// <summary>
		/// This is the list of metadata fields applicable
		/// for video resources.
		/// </summary>
		public class VideoItem : ResourceAttributes
		{
			/// <summary>
			/// The bitrate of the media. Usually
			/// used for audio and video resources.
			/// </summary>
			public _UInt bitrate = new _UInt(false);
			/// <summary>
			/// The number of audio channels for the media. Usually for audio or video items.
			/// </summary>
			public _UInt nrAudioChannels = new _UInt(false);
			/// <summary>
			/// Content protection string. Syntax not defined.
			/// </summary>
			public string protection = null;
			/// <summary>
			/// The pixel resolution of the media, in "[width]x[height]" format.
			/// </summary>
			public ImageDimensions resolution = new ImageDimensions(false);
		}

		/// <summary>
		/// This is the additional list of metadata fields applicable
		/// for movies, in addition to the metadata from
		/// <see cref="ResourceBuilder.VideoItem"/>.
		/// </summary>
		public class Movie : VideoItem
		{
			/// <summary>
			/// The time duration of the media, in the format
			/// "h*:mm:ss:f*" or "h*:mm:ss.f0/f1",
			/// where
			/// <list type="bullet">
			/// <item>
			/// <term>h*</term>
			/// <description>any number of digits (including no digits) to indicated elapsed time in hours</description>
			/// </item>
			/// <item>
			/// <term>mm</term>
			/// <description>0 to 59, to indicate minutes</description>
			/// </item>
			/// <item>
			/// <term>ss</term>
			/// <description>0 to 59, to indicate seconds</description>
			/// </item>
			/// <item>
			/// <term>f*</term>
			/// <description>any number of digits (including no digits) to indicated fractions of seconds</description>
			/// </item>
			/// <item>
			/// <term>f0/f1</term>
			/// <description>a fraction, with f0 and f1 at least one digitl long and f0 is less than f1. This field may be preceded
			/// by an optional + or - sign, and the preceding decimal point may be omitted if no
			/// fractional seconds are listed</description>
			/// </item>
			/// </list>
			/// </summary>
			public _TimeSpan duration = new _TimeSpan(false);
		}

		/// <summary>
		/// This is the list of metadata fields applicable
		/// for image and photo resources. 
		/// </summary>
		public class ImageItem : ResourceAttributes
		{
			/// <summary>
			/// The number of colors in the media.
			/// Usually used for image.
			/// </summary>
			public _UInt colorDepth = new _UInt(false);
			/// <summary>
			/// Content protection string. Syntax not defined.
			/// </summary>
			public string protection = null;
			/// <summary>
			/// The pixel resolution of the media, in "[width]x[height]" format.
			/// </summary>
			public ImageDimensions resolution = new ImageDimensions(false);
		}

		/// <summary>
		/// This is the list of metadata fields applicable
		/// for text items.
		/// </summary>
		public class TextItem : ResourceAttributes
		{
			/// <summary>
			/// Content protection string. Syntax not defined.
			/// </summary>
			public string protection = null;
		}
	}
}
