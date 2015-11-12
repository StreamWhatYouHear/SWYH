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
using System.Text;
using OpenSource.UPnP;
using OpenSource.UPnP.AV;
using System.Runtime.Serialization;

namespace OpenSource.UPnP.AV.CdsMetadata
{
	/// <summary>
	/// <para>
	/// ProtocolInfoString extracts the UPNP-AV concept of a
	/// ProtocolInfo string. ProtocolInfo strings have the
	/// format of a 
	/// "[protocol]:[network]:[mime type]:[info]".
	/// </para>
	/// 
	/// <para>
	/// For HTTP resources, the protocol is always
	/// "http-get" and the network is "*". Info is
	/// usually "*".
	/// </para>
	/// </summary>
	[Serializable()]
	public sealed class ProtocolInfoString : IComparable, IDeserializationCallback
	{
		/// <summary>
		/// Simply compares the argument against the protocolInfo string's value.
		/// </summary>
		/// <param name="compareToThis"></param>
		/// <returns></returns>
		public int CompareTo(object compareToThis)
		{
			return string.Compare(this.ToString(), compareToThis.ToString());
		}

		/// <summary>
		/// Returns the hashcode for the underlying string.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return this._value.GetHashCode();
		}
		/// <summary>
		/// Returns true if the provided ProtocolInfoString
		/// object is equivalent to this object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			ProtocolInfoString protInfo = (ProtocolInfoString) obj;

			return (protInfo._value == this._value);
		}

		/// <summary>
		/// Convenient method of creating an http-get protocolInfo string from a file.
		/// The mime-type is determiend by the file extension.
		/// See <see cref="MimeTypes.ExtensionToMimeType"/>
		/// for more information.
		/// </summary>
		/// <param name="f"></param>
		/// <returns>a protocolInfo string with form: http-get:*:mime-type:*</returns>
		public static ProtocolInfoString CreateHttpGetProtocolInfoString(FileInfo f)
		{
			string mime, mediaClass;
			MimeTypes.ExtensionToMimeType(f.Extension, out mime, out mediaClass);
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("http-get:*:{0}:*", mime);
			return new ProtocolInfoString(sb.ToString());
		}

		/// <summary>
		/// Constructs a ProtocolInfoString using string.
		/// </summary>
		/// <param name="protocolInfo">proposed value of the protocolInfo string</param>
		public ProtocolInfoString(string protocolInfo)
		{
			DText parser = new DText();
			parser.ATTRMARK = ":";

			parser[0] = protocolInfo;

			_Protocol = parser[1];
			_Network = parser[2];
			_MimeType = parser[3];
			_Info = parser[4];

			StringBuilder sb = new StringBuilder(100);
			sb.AppendFormat("{0}:{1}:{2}:{3}", _Protocol, _Network, _MimeType, _Info);
			_value = sb.ToString();
		}

		
		/// <summary>
		/// Returns the full protocolInfo value.
		/// </summary>
		/// <returns></returns>
		public override string ToString() { return _value; }

		/// <summary>
		/// Returns the protocol portion of the protocolInfo.
		/// </summary>
		public string Protocol { get { return this._Protocol; } }
		private string _Protocol;
		
		/// <summary>
		/// Returns the network portion of the protocolInfo.
		/// </summary>
		public string Network { get { return this._Network; } }
		private string _Network;
		
		/// <summary>
		/// Returns the mime type portion of the protocolInfo.
		/// </summary>
		public string MimeType { get { return this._MimeType; } }
		private string _MimeType;
		
		/// <summary>
		/// Returns the info portion of the protocolInfo.
		/// </summary>
		public string Info { get { return this._Info; } }
		private string _Info;
		
		/// <summary>
		/// Contains the full protocolInfo value.
		/// </summary>
		private string _value;

		/// <summary>
		/// Returns true if the supplied ProtocolInfoString is an acceptable match.
		/// The supplied ProtocolInfoString's mime-type and info fields must represent a superset or the
		/// same set as this instance. Basically, the algorithm works like this.
		/// <list type="number">
		/// <item><description>If the protocols match..</description></item>
		/// <item><description>...and the networks match</description></item>
		/// <item><description>...and (if the mime types match) OR (if the argument protocolInfo mime type is *)</description></item>
		/// <item><description>...and (if the info fields match) OR (if the argument protocolInfo info field is *)</description></item>
		/// <item><description>Then we return true.</description></item>
		/// </list>
		/// </summary>
		/// <param name="protocolInfo">supplied protocolInfo string; the * symbol can be applied to indicate any value</param>
		/// <returns>True indicates that the supplied protocolInfo is the same or a superset.</returns>
		public bool Matches (ProtocolInfoString protocolInfo)
		{
			if (protocolInfo.Protocol == this.Protocol)
			{
				if (protocolInfo.Network == this.Network)
				{
					if ((protocolInfo.MimeType == "*") || (protocolInfo.MimeType == this.MimeType))
					{
						if ((protocolInfo.Info == "*") || (protocolInfo.Info == this.Info))
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Defines the default value for a protocol info string.
		/// </summary>
		public const string DefaultValue = "*:*:*:*";

		/// <summary>
		/// Used for token-delimited text parsing.
		/// </summary>
		private static DText DT = new DText();

		public void OnDeserialization(object sender)
		{
			_Protocol = (string) CdsMetadataCaches.CommonData.CacheThis(this._Protocol);
			_Network = (string) CdsMetadataCaches.CommonData.CacheThis(this._Network);
			_MimeType = (string) CdsMetadataCaches.CommonData.CacheThis(this._MimeType);
			_Info = (string) CdsMetadataCaches.CommonData.CacheThis(this._Info);
			_value = (string) CdsMetadataCaches.CommonData.CacheThis(this._value);
		}

	}
}
