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
using OpenSource.UPnP.AV;

namespace OpenSource.UPnP.AV.CdsMetadata
{
	/// <summary>
	/// Exposes static methods for converting strings from mime type to file extension and vice-versa.
	/// The list is not comprehensive. See the individual methods for the mappings that they return.
	/// </summary>
	public class MimeTypes
	{
		/// <summary>
		/// Converts from a mime type to a file extension. Since mime-types
		/// are often fuzzy, multiple mime-types may map to a single file extension.
		/// Unless mapped below, all are ".bin" extensions.
		/// <list type="table">
		/// <listheader><term>mime type</term><description>file extension</description></listheader>
		/// <item>application/postscript</item><description>ps</description>
		/// <item>application/x-ms-asx</item><description>asx</description>
		/// <item>application/asx</item><description>asx</description>
		/// <item>application/x-troff-msvideo</item><description>avi</description>
		/// <item>audio/mpeg</item><description>mp3</description>
		/// <item>audio/mp3</item><description>mp3</description>
		/// <item>audio/x-ms-wma</item><description>wma</description>
		/// <item>audio/wma</item><description>wma</description>
		/// <item>audio/wav</item><description>wav</description>
		/// <item>audio/lpcm</item><description>wav</description>
		/// <item>audio/asx</item><description>asx</description>
		/// <item>audio/x-mpegurl</item><description>m3u</description>
		/// <item>audio/mpegurl</item><description>m3u</description>
		/// <item>video/MP1S</item><description>mpg</description>
		/// <item>video/mpg</item><description>mpg</description>
		/// <item>video/mpeg2</item><description>mpeg2</description>
		/// <item>video/mpeg</item><description>mpg</description>
		/// <item>video/mpg</item><description>mpg</description>
		/// <item>video/x-ms-wmv</item><description>wmv</description>
		/// <item>video/wmv</item><description>wmv</description>
		/// <item>video/x-ms-asf</item><description>asf</description>
		/// <item>video/asf</item><description>asf</description>
		/// <item>video/x-ms-avi</item><description>avi</description>
		/// <item>video/msvideo</item><description>avi</description>
		/// <item>video/x-msvideo</item><description>avi</description>
		/// <item>video/avi</item><description>avi</description>
		/// <item>video/asx</item><description>asx</description>
		/// <item>image/jpeg</item><description>jpg</description>
		/// <item>image/pjpeg</item><description>jpg</description>
		/// <item>image/jpe</item><description>jpe</description>
		/// <item>image/jpg</item><description>jpg</description>
		/// <item>image/tif</item><description>tif</description>
		/// <item>image/x-windows-bmp</item><description>bmp</description>
		/// <item>image/bmp</item><description>bmp</description>
		/// <item>image/gif</item><description>gif</description>
		/// <item>image/png</item><description>png</description>
		/// <item>text/plain</item><description>txt</description>
		/// <item>text/richtext</item><description>rtf</description>
		/// <item>audio/x-pn/audibleaudio</item><description>aa</description>
		/// <item>application/mspowerpoint</item><description>ppt</description>
		/// <item>video/vod</item><description>vod</description>
		/// <item>video/3gp</item><description>3gp</description>
		/// <item>audio/amr</item><description>amr</description>
		/// </list>
		/// </summary>
		/// <param name="mime">mime type</param>
		/// <returns>returns file extension</returns>
		public static string MimeToExtension(string mime)
		{
			if (
				(String.Compare(mime, "audio/x-pn/audibleaudio", true) == 0)
				)
			{
				return ".aa";
			}
			else if (
				(String.Compare(mime, "audio/mpeg", true) == 0) ||
				(String.Compare(mime, "audio/mp3", true) == 0)
				)
			{
				return ".mp3";
			}
			else if 
				(
				(String.Compare(mime, "audio/x-ms-wma", true) == 0) ||
				(String.Compare(mime, "audio/wma", true) == 0)
				)
			{
				return ".wma";
			}
			else if 
				(
				(String.Compare(mime, "audio/wav", true) == 0)
				)
			{
				return ".wav";
			}
			else if 
				(
				(String.Compare(mime, "audio/lpcm", true) == 0)
				)
			{
				return ".lpcm";
			}
			else if 
				(
				(String.Compare(mime, "video/mpeg2", true) == 0)
				)
			{
				return ".mpeg2";
			}
			else if 
				(
				(String.Compare(mime, "video/MP1S", true) == 0) ||
				(String.Compare(mime, "video/mpeg", true) == 0) ||
				(String.Compare(mime, "video/mpg", true) == 0) 
				)
			{
				return ".mpg";
			}
			else if 
				(
				(String.Compare(mime, "video/x-ms-wmv", true) == 0) ||
				(String.Compare(mime, "video/wmv", true) == 0)
				)
			{
				return ".wmv";
			}
			else if 
				(
				(String.Compare(mime, "video/x-ms-asf", true) == 0) ||
				(String.Compare(mime, "video/asf", true) == 0)
				)
			{
				return ".asf";
			}
			else if 
				(
				(String.Compare(mime, "video/x-ms-avi", true) == 0) ||
				(String.Compare(mime, "application/x-troff-msvideo", true) == 0) ||
				(String.Compare(mime, "video/msvideo", true) == 0) ||
				(String.Compare(mime, "video/x-msvideo", true) == 0) ||
				(String.Compare(mime, "video/avi", true) == 0)
				)
			{
				return ".avi";
			}
			else if 
				(
				(String.Compare(mime, "image/jpeg", true) == 0) ||
				(String.Compare(mime, "image/pjpeg", true) == 0) ||
				(String.Compare(mime, "image/jpg", true) == 0)
				)
			{
				return ".jpg";
			}
			else if 
				(
				(String.Compare(mime, "image/jpe", true) == 0)
				)
			{
				return ".jpe";
			}
			else if 
				(
				(String.Compare(mime, "image/tif", true) == 0)
				)
			{
				return ".tif";
			}
			else if 
				(
				(String.Compare(mime, "image/png", true) == 0)
				)
			{
				return ".png";
			}
			else if 
				(
				(String.Compare(mime, "image/x-windows-bmp", true) == 0) ||
				(String.Compare(mime, "image/bmp", true) == 0)
				)
			{
				return ".bmp";
			}
			else if 
				(
				(String.Compare(mime, "image/gif", true) == 0)
				)
			{
				return ".gif";
			}
			else if 
				(
				(String.Compare(mime, "text/plain", true) == 0)
				)
			{
				return ".txt";
			}
			else if 
				(
				(String.Compare(mime, "application/postscript", true) == 0)
				)
			{
				return ".ps";
			}
			else if 
				(
				(String.Compare(mime, "text/richtext", true) == 0)
				)
			{
				return ".rtf";
			}
			else if 
				(
				(String.Compare(mime, "audio/x-mpegurl", true) == 0) ||
				(String.Compare(mime, "audio/mpegurl", true) == 0)
				)
			{
				return ".m3u";
			}
			else if 
				(
				(String.Compare(mime, "application/x-ms-asx", true) == 0) ||
				(String.Compare(mime, "application/asx", true) == 0) ||
				(String.Compare(mime, "audio/asx", true) == 0) ||
				(String.Compare(mime, "video/asx", true) == 0)
				)
			{
				return ".asx";
			}
			else if 
				(
				(String.Compare(mime, "video/vod", true) == 0)
				)
			{
				return ".vod";
			}
			else if 
				(
				(String.Compare(mime, "video/3gp", true) == 0)
				)
			{
				return ".3gp";
			}
			else if 
				(
				(String.Compare(mime, "audio/amr", true) == 0)
				)
			{
				return ".amr";
			}
			else if 
				(
				(String.Compare(mime, "application/mspowerpoint", true) == 0)
				)
			{
				return ".ppt";
			}
			else
			{
				return ".bin";
			}

		}

		
		/// <summary>
		/// Converts from file extension to mime type and also retrieves a MediaClass. Mappings 
		/// are not standardized, but generally agreed upon in the UPNP-AV Forum. Unless mapped
		/// below, all extensions will map to "application/octet-stream".
		/// 
		/// <list type="table">
		/// <listheader><term>file extension</term><description>mime-type &amp; mediaClass</description></listheader>
		/// <item><term>mp3</term><description>audio/mpeg</description></item>
		/// <item><term>wma</term><description>audio/x-ms-wma</description></item>
		/// <item><term>wav</term><description>audio/wav</description></item>
		/// <item><term>lpcm</term><description>audio/lpcm</description></item>
		/// <item><term>mpeg</term><description>video/mpeg</description></item>
		/// <item><term>mpeg2</term><description>video/mpeg2</description></item>
		/// <item><term>mpg</term><description>video/MP1S</description></item>
		/// <item><term>m1v</term><description>video/mpeg</description></item>
		/// <item><term>m2v</term><description>video/mpeg</description></item>
		/// <item><term>wmv</term><description>video/x-ms-wmv</description></item>
		/// <item><term>asf</term><description>video/x-ms-asf</description></item>
		/// <item><term>avi</term><description>video/x-ms-avi</description></item>
		/// <item><term>jpg</term><description>image/jpeg</description></item>
		/// <item><term>jpeg</term><description>image/jpeg</description></item>
		/// <item><term>pjpeg</term><description>image/jpeg</description></item>
		/// <item><term>jpe</term><description>image/jpe</description></item>
		/// <item><term>bmp</term><description>image/bmp</description></item>
		/// <item><term>tif</term><description>image/tif</description></item>
		/// <item><term>gif</term><description>image/gif</description></item>
		/// <item><term>txt</term><description>text/plain</description></item>
		/// <item><term>text</term><description>text/plain</description></item>
		/// <item><term>png</term><description>image/png</description></item>
		/// <item><term>ps</term><description>application/postscript</description></item>
		/// <item><term>rtf</term><description>text/richtext</description></item>
		/// <item><term>m3u</term><description>audio/x-mpegurl</description></item>
		/// <item><term>asx</term><description>application/x-ms-asx</description></item>
		/// <item><term>aa</term><description>audio/x-pn/audibleaudio</description></item>
		/// <item><term>ppt</term><description>application/mspowerpoint</description></item>
		/// <item><term>vod</term><description>video/vod</description></item>
		/// <item><term>3gp</term><description>video/3gp</description></item>
		/// <item><term>amr</term><description>audio/amr</description></item>
		/// </list>
		/// </summary>
		/// <param name="extension">file extension</param>
		/// <param name="mime">output mime type</param>
		/// <param name="mediaClass">output media type, useful for instantiating
		/// <see cref="MediaClass"/> instances
		/// </param>
		public static void ExtensionToMimeType(string extension, out string mime, out string mediaClass)
		{
			if (extension == "")
			{
				mediaClass = MediaBuilder.StandardMediaClasses.Item.ToString();
				mime = MimeTypes.ApplicationOctetStream;
				return;
			}

			int pos = 0;
			while (extension[pos] == '.')
			{
				pos++;
			}
			extension = extension.Substring(pos);
			
			if (
				(String.Compare(extension, "mp3", true) == 0)
				)
			{
				mime =  "audio/mpeg";
				mediaClass = MediaBuilder.StandardMediaClasses.MusicTrack.ToString();
			}
			else if 
				(
				(String.Compare(extension, "wma", true) == 0)
				)
			{
				mime =  "audio/x-ms-wma";
				mediaClass = MediaBuilder.StandardMediaClasses.MusicTrack.ToString();
			}
			else if 
				(
				(String.Compare(extension, "wav", true) == 0)
				)
			{
				mime =  "audio/wav";
				mediaClass = MediaBuilder.StandardMediaClasses.AudioItem.ToString();
			}
			else if 
				(
				(String.Compare(extension, "lpcm", true) == 0)
				)
			{
				mime =  "audio/lpcm";
				mediaClass = MediaBuilder.StandardMediaClasses.AudioItem.ToString();
			}
			else if
				(
				(String.Compare(extension, "mpg", true) == 0)
				)
			{
				mime =  "video/MP1S";
				mediaClass = MediaBuilder.StandardMediaClasses.VideoItem.ToString();
			}
			else if
				(
				(String.Compare(extension, "mpeg2", true) == 0)
				)
			{
				mime =  "video/mpeg2";
				mediaClass = MediaBuilder.StandardMediaClasses.VideoItem.ToString();
			}
			else if
				(
				(String.Compare(extension, "mpeg", true) == 0) ||
				(String.Compare(extension, "m1v", true) == 0) ||
				(String.Compare(extension, "m2v", true) == 0)
				)
			{
				mime =  "video/mpeg";
				mediaClass = MediaBuilder.StandardMediaClasses.VideoItem.ToString();
			}
			else if
				(
				(String.Compare(extension, "wmv", true) == 0)
				)
			{
				mime =  "video/x-ms-wmv";
				mediaClass = MediaBuilder.StandardMediaClasses.VideoItem.ToString();
			}
			else if
				(
				(String.Compare(extension, "asf", true) == 0)
				)
			{
				mime =  "video/x-ms-asf";
				mediaClass = MediaBuilder.StandardMediaClasses.VideoItem.ToString();
			}
			else if
				(
				(String.Compare(extension, "avi", true) == 0)
				)
			{
				mime =  "video/x-ms-avi";
				mediaClass = MediaBuilder.StandardMediaClasses.VideoItem.ToString();
			}
			else if
				(
				(String.Compare(extension, "jpeg", true) == 0) ||
				(String.Compare(extension, "pjpeg", true) == 0) ||
				(String.Compare(extension, "jpg", true) == 0)
				)
			{
				mime =  "image/jpeg";
				mediaClass = MediaBuilder.StandardMediaClasses.ImageItem.ToString();
			}
			else if
				(
				(String.Compare(extension, "jpe", true) == 0) 
				)
			{
				mime =  "image/jpe";
				mediaClass = MediaBuilder.StandardMediaClasses.ImageItem.ToString();
			}
			else if
				(
				(String.Compare(extension, "bmp", true) == 0)
				)
			{
				mime =  "image/bmp";
				mediaClass = MediaBuilder.StandardMediaClasses.ImageItem.ToString();
			}
			else if
				(
				(String.Compare(extension, "tif", true) == 0)
				)
			{
				mime =  "image/tif";
				mediaClass = MediaBuilder.StandardMediaClasses.ImageItem.ToString();
			}
			else if
				(
				(String.Compare(extension, "gif", true) == 0)
				)
			{
				mime =  "image/gif";
				mediaClass = MediaBuilder.StandardMediaClasses.ImageItem.ToString();
			}
			else if
				(
				(String.Compare(extension, "txt", true) == 0) ||
				(String.Compare(extension, "text", true) == 0)
				)
			{
				mime =  "text/plain";
				mediaClass = MediaBuilder.StandardMediaClasses.ImageItem.ToString();
			}
			else if
				(
				(String.Compare(extension, "png", true) == 0)
				)
			{
				mime =  "image/png";
				mediaClass = MediaBuilder.StandardMediaClasses.ImageItem.ToString();
			}
			else if
				(
				(String.Compare(extension, "ps", true) == 0)
				)
			{
				mime =  "application/postscript";
				mediaClass = MediaBuilder.StandardMediaClasses.Item.ToString();
			}
			else if
				(
				(String.Compare(extension, "rtf", true) == 0)
				)
			{
				mime =  "text/richtext";
				mediaClass = MediaBuilder.StandardMediaClasses.TextItem.ToString();
			}
			else if
				(
				(String.Compare(extension, "aa", true) == 0)
				)
			{
				mime =  "audio/x-pn/audibleaudio";
				mediaClass = MediaBuilder.StandardMediaClasses.AudioBook.ToString();
			}
			else if
				(
				(String.Compare(extension, "m3u", true) == 0)
				)
			{
				mime =  "audio/x-mpegurl";
				mediaClass = MediaBuilder.StandardMediaClasses.PlaylistContainer.ToString();
			}
			else if
				(
				(String.Compare(extension, "asx", true) == 0)
				)
			{
				mime =  "application/x-ms-asx";
				mediaClass = MediaBuilder.StandardMediaClasses.PlaylistContainer.ToString();
			}
			else if
				(
				(String.Compare(extension, "ppt", true) == 0)
				)
			{
				mime =  "application/mspowerpoint";
				mediaClass = MediaBuilder.StandardMediaClasses.Item.ToString();
			}
			else if
				(
				(String.Compare(extension, "vod", true) == 0)
				)
			{
				mime =  "video/vod";
				mediaClass = MediaBuilder.StandardMediaClasses.Item.ToString();
			}
			else if
				(
				(String.Compare(extension, "3gp", true) == 0)
				)
			{
				mime =  "video/3gp";
				mediaClass = MediaBuilder.StandardMediaClasses.VideoItem.ToString();
			}
			else if
				(
				(String.Compare(extension, "amr", true) == 0)
				)
			{
				mime =  "audio/amr";
				mediaClass = MediaBuilder.StandardMediaClasses.AudioItem.ToString();
			}
			else
			{
				mime =  ApplicationOctetStream;
				mediaClass = MediaBuilder.StandardMediaClasses.Item.ToString();
			}

		}


		/// <summary>
		/// default mime type for unknown things
		/// </summary>
		public const string ApplicationOctetStream = "application/octet-stream";
	}
}
