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
using OpenSource.UPnP;
using System.Collections;
using OpenSource.UPnP.AV;

namespace OpenSource.UPnP.AV.CdsMetadata
{
	/// <summary>
	/// This class is the general purpose class for getting 
	/// the strings for various element tag names and properties.
	/// </summary>
	public class Tags
	{
		/// <summary>
		/// dublin core namespace value
		/// </summary>
		public const string XMLNSDC_VALUE = "http://purl.org/dc/elements/1.1/";
		/// <summary>
		/// upnp-av metadata namespace value
		/// </summary>
		public const string XMLNSUPNP_VALUE= "urn:schemas-upnp-org:metadata-1-0/upnp/";
		/// <summary>
		/// upnp-av subset of dublin-core namespace
		/// </summary>
		public const string XMLNSDIDL_VALUE= "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/";

		/// <summary>
		/// UPNP-AV defined abbreviation for dublin-core namespace is "dc"
		/// </summary>
		public const string XMLNS_DC = "xmlns:dc";
		/// <summary>
		/// UPNP-AV defined abbreviation for upnp-av metadata namespace is "upnp"
		/// </summary>
		public const string XMLNS_UPNP = "xmlns:upnp";
		/// <summary>
		/// XML namespace attribute name
		/// </summary>
		public const string XMLNS = "xmlns";

		/// <summary>
		/// A collection of all dublin-core element names without namespace.
		/// </summary>
		public ICollection DC { get { return m_DC; } }
		/// <summary>
		/// Collection of all upnp-av metadata element names without namespace.
		/// </summary>
		public ICollection UPNP { get { return m_UPNP; } }
		/// <summary>
		/// Collection of all DIDL-Lite element names without namespace.
		/// </summary>
		public ICollection DIDL { get { return m_DIDL; } }
		/// <summary>
		/// Collection of all attribute
		/// names spanning all UPNP-AV/ContentDirectory namespaces,
		/// without namespace.
		/// </summary>
		public ICollection ATTRIB { get { return m_ATTRIB; } }

		/// <summary>
		/// Enforce static instance.
		/// </summary>
		private Tags()
		{
			System.Array vals;
			
			_DC dc = 0;
			vals = Enum.GetValues(dc.GetType());
			m_DC = new String[vals.Length];
			for (int i=0; i < vals.Length; i++)
			{
				m_DC[i] = this[(_DC)vals.GetValue(i)];
			}

			_UPNP UPNP = 0;
			vals = Enum.GetValues(UPNP.GetType());
			m_UPNP = new String[vals.Length];
			for (int i=0; i < vals.Length; i++)
			{
				m_UPNP[i] = this[(_UPNP)vals.GetValue(i)];
			}

			_DIDL DIDL = 0;
			vals = Enum.GetValues(DIDL.GetType());
			m_DIDL = new String[vals.Length];
			for (int i=0; i < vals.Length; i++)
			{
				m_DIDL[i] = this[(_DIDL)vals.GetValue(i)];
			}

			_ATTRIB ATTRIB = 0;
			vals = Enum.GetValues(ATTRIB.GetType());
			m_ATTRIB = new String[vals.Length];
			for (int i=0; i < vals.Length; i++)
			{
				m_ATTRIB[i] = this[(_ATTRIB)vals.GetValue(i)];
			}
		}




//		public static string BuildXmlRepresentation(string baseUrl, ICollection entries)
//		{
//			return BuildXmlRepresentation("", new ArrayList(), entries);
//		}

		/// <summary>
		/// the actual string values returned in DC
		/// </summary>
		private readonly string[] m_DC;
		/// <summary>
		/// actual string values in UPNP
		/// </summary>
		private readonly string[] m_UPNP;
		/// <summary>
		/// actual string values in DIDL
		/// </summary>
		private readonly string[] m_DIDL;
		/// <summary>
		/// actual string values in ATTRIB
		/// </summary>
		private readonly string[] m_ATTRIB;

		/// <summary>
		/// Allows easy retrieval of strings formatted both with
		/// namespace and element name, given a value
		/// from the <see cref="CommonPropertyNames"/>
		/// enumerator.
		/// </summary>
		public string this[CommonPropertyNames tag]
		{
			get
			{
				StringBuilder sb = new StringBuilder(25);
				string ns = "";
				if (tag > CommonPropertyNames.DoNotUse)
				{
					ns = "dc:";
				}
				else if (tag < CommonPropertyNames.DoNotUse)
				{
					ns = "upnp:";
				}
				else
				{
					throw new Exception("Bad Evil. Improper value used to index Tags.");
				}
				if (tag == CommonPropertyNames.Class)
				{
					sb.AppendFormat("{1}{0}", "class", ns);
				}
				else
				{
					sb.AppendFormat("{1}{0}", tag.ToString(), ns);
				}
				return sb.ToString();
			}
		}

		/// <summary>
		/// Allows easy retrieval of strings formatted both with
		/// namespace and element name, given a value
		/// from the <see cref="_DC"/>
		/// enumerator.
		/// </summary>
		public string this[_DC tag]
		{ 
			get 
			{ 
				StringBuilder sb = new StringBuilder(25);
				sb.AppendFormat("dc:{0}", tag.ToString());
				return sb.ToString();
			}
		}

		/// <summary>
		/// Allows easy retrieval of strings formatted both with
		/// namespace and element name, given a value
		/// from the <see cref="_UPNP"/>
		/// enumerator.
		/// </summary>
		public string this[_UPNP tag]
		{ 
			get 
			{
				string str = tag.ToString();
				if (tag == _UPNP.Class)
				{
					str = "class";
				}

				StringBuilder sb = new StringBuilder(25);
				sb.AppendFormat("upnp:{0}", str);
				return sb.ToString();
			}
		}

		/// <summary>
		/// Allows easy retrieval of strings form of an
		/// <see cref="_ATTRIB"/>
		/// value.
		/// </summary>
		public string this[_ATTRIB attrib]
		{ 
			get 
			{ 
				StringBuilder sb = new StringBuilder(25);
				sb.AppendFormat("{0}", attrib.ToString());
				return sb.ToString();
			}
		}
		
		/// <summary>
		/// Allows easy retrieval of strings form of an
		/// <see cref="_RESATTRIB"/>
		/// value.
		/// </summary>
		public string this[_RESATTRIB attrib]
		{ 
			get 
			{ 
				StringBuilder sb = new StringBuilder(25);
				sb.AppendFormat("{0}", attrib.ToString());
				return sb.ToString();
			}
		}
		/// <summary>
		/// Allows easy retrieval of DIDL-Lite element names given a value
		/// from the <see cref="_DIDL"/>
		/// enumerator.
		/// </summary>
		public string this[_DIDL tag]
		{
			get
			{
				string t;
				if (tag == _DIDL.DIDL_Lite)
				{
					return "DIDL-Lite";
				}
				else
				{
					t = tag.ToString();
				}
				StringBuilder sb = new StringBuilder(25);
				sb.AppendFormat("{0}", t);
				sb[0] = char.ToLower(sb[0]);
				
				return sb.ToString();
			}
		}

		/// <summary>
		/// The properties hashtable of a media object is not set.
		/// </summary>
		public class NullPropertiesException : Exception {}
		/// <summary>
		/// A media item improperly lacks a title.
		/// </summary>
		public class EmptyTitleException: Exception{}



		/// <summary>
		/// The one and only tags instance.
		/// </summary>
		private static Tags T = new Tags();

		/// <summary>
		/// Exposes a bunch of metadata properties that are attributes of elements 
		/// in their full name form, as used in ContentDirectory SearchCriteria
		/// argument of Search action.
		/// </summary>
		public class PropertyAttributes
		{
			public const string item_parentID = "item@parentID";
			public const string container_parentID = "container@parentID";
			public const string container_childCount = "container@childCount";
			public const string res_importUri = "res@importUri";
			public const string res_size = "res@size";
			public const string res_duration = "res@duration";
			public const string res_bitrate = "res@bitrate";
			public const string res_sampleFrequency = "res@sampleFrequency";
			public const string res_bitsPerSample = "res@bitsPerSample";
			public const string res_nrAudioChannels = "res@nrAudioChannels";
			public const string res_resolution = "res@resolution";
			public const string res_colorDepth = "res@colorDepth";
			public const string res_protection = "res@protection";

			public const string upnp_class = "upnp:class";
			public const string upnp_className = "upnp:class@name";

			public const string upnp_searchClass = "upnp:searchClass";
			public const string upnp_searchClassName = "upnp:searchClass@name";
			public const string upnp_searchClassIncludeDerived = "upnp:searchClass@includeDerived";

			public const string upnp_createClass = "upnp:createClass";
			public const string upnp_createClassName = "upnp:createClass@name";
			public const string upnp_createClassIncludeDerived = "upnp:createClass@includeDerived";

			public const string container_searchable = "container@searchable";
		}

		/// <summary>
		/// Returns the static instance of Tags.
		/// </summary>
		/// <returns></returns>
		public static Tags GetInstance()
		{
			return T;
		}
	}
}
