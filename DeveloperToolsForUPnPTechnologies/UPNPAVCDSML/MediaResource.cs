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
using OpenSource.UPnP.AV;
using System.Diagnostics;
using System.Collections;
using System.Runtime.Serialization;

namespace OpenSource.UPnP.AV.CdsMetadata
{
	/// <summary>
	/// <para>
	/// This structure represents the UPNP-AV/ContentDirectory equivalent
	/// of a "res" (resource) element. Resources are different from items
	/// and containers because they are the only ContentDirectory elements
	/// that have a direct binding to a consumable binary.
	/// </para>
	/// 
	/// <para>
	/// A number of resource metadata properties are numerical and require
	/// a default value because an empty string is not a valid number. For
	/// this reason, negative values have been assigned to indicate
	/// that the value has not been assigned, and not intended for consumption.
	/// </para>
	/// 
	/// <para>
	/// MediaResource objects are added to 
	/// <see cref="IUPnPMedia"/> 
	/// instances. This allows multiple binaries to be associated
	/// with the same item or container. Examples are listed 
	/// below for why this might happen.
	/// </para>
	/// 
	/// <list type="bullet">
	/// <item>
	/// <term>Multiple files with the same content</term>
	/// <description>
	/// <para>
	/// If a MediaServer desires to expose a music video from a particular artist and has 3
	/// different files for different formats: low-quality 320x200 AVI, medium quality 640x480 mpeg4,
	/// and super quality 1280x1024 in mpeg2, the MediaServer does not need to create an item for each 
	/// version of the content. It simply can use the same item entry, and associate multiple
	/// resources with that entry. This method of organizing resources can apply to playlist containers as well,
	/// such that multiple playlist formats can be associated with a single container.
	/// </para>
	/// <para>
	/// It should be noted that this methodology of organizing multiple files is not required
	/// by any implementation. The architecture simply allows for it. 
	/// </para>
	/// </description>
	/// </item>
	/// 
	/// <item>
	/// <term>Multimedia content entry</term>
	/// <description>
	/// <para>
	/// If a MediaServer desires to expose a vendor-specific content entry that might have
	/// multiple types of media, then having a single item entry and multiple resources
	/// is allowed to do this. 
	/// </para>
	/// <para>
	/// It should be noted that this ContentDirectory assumes no ordering to the resources
	/// associated with an item or container.
	/// </para>
	/// </description>
	/// </item>	
	/// </list>
	/// </summary>
	[Serializable()]
	public class MediaResource : ICdsElement, ICloneable, IMediaResource, IToXmlData, IDeserializationCallback
	{
		/// <summary>
		/// Updates the metadata (contentUri and xml attributes) for this
		/// resource using another resource's metadata. 
		/// Values for the contentUri and attributes of the resource
		/// are shallow-copied.
		/// </summary>
		/// <param name="newMetadata"></param>
		public virtual void UpdateResource (IMediaResource newMetadata)
		{
			ICollection attribs = Enum.GetValues(typeof(_RESATTRIB));

			foreach (_RESATTRIB attrib in attribs)
			{
				string key = T[attrib];
				object val = newMetadata[key];
				IValueType ivt = val as IValueType;
				if (ivt != null)
				{
					// If the data associated with the resource's metata 
					// is stored as an IValueType, then we need to check
					// for validity. Remember that IValueType objects
					// are object representations of value-types that
					// cannot have a null value (eg, integers, et al).
					//
					// If the value is valid, then we apply it.
					// Otherwise, we remove the current value from
					// the resource's attribute value correctly.
					if (ivt.IsValid)
					{
						this.SetAttribute(key, ivt);
					}
					else
					{
						this.SetAttribute(key, null);
					}
				}
				else
				{
					// This code executes when the value is not an IValueType,
					// or when the sent value is null. Either case, we modify
					// the resources's attribute value correctly.
					this.SetAttribute(key, val);
				}
			}
		}

		/// <summary>
		/// Does a memberwise clone, but sets the
		/// of the resource to be null.
		/// </summary>
		/// <returns></returns>
		public virtual object Clone()
		{
			object RetVal = this.MemberwiseClone();
			((MediaResource)RetVal).m_Owner = null;
			return(RetVal);
		}

		/// <summary>
		/// Returns itself as the value.
		/// <para>
		/// This method satisfies an interface requirement for <see cref="ICdsElement"/>.
		/// Given that a resource always has to have a protocolInfo and that a contentURI
		/// is largely the useful component, this property returns the object itself.
		/// </para>
		/// </summary>
		public object Value { get { return this; } }

		/// <summary>
		/// Returns the contentUri as a comparable string.
		/// </summary>
		public IComparable ComparableValue { get { return this.ContentUri; } }

		/// <summary>
		/// Returns the contentUri.
		/// </summary>
		public string StringValue { get { return this.ContentUri; } }

		/// <summary>
		/// Returns a listing of possible attributes that apply to resources.
		/// </summary>
		public IList PossibleAttributes 
		{
			get
			{
				return GetPossibleAttributes();
			}
		}

		/// <summary>
		/// Only attribute available is "name"
		/// </summary>
		/// <returns></returns>
		public static IList GetPossibleAttributes()
		{
			ArrayList attribs = new ArrayList();
			foreach (_RESATTRIB attrib in Enum.GetValues(typeof(_RESATTRIB)))
			{
				attribs.Add (T[attrib]);
			}
			return attribs;
		}


		/// <summary>
		/// Returns a listing of attributes that have been set.
		/// </summary>
		public virtual IList ValidAttributes 
		{
			get
			{
				// Get a reference collection from the _Hashtable.Keys method.
				// Then return a shallow copy so we can safely enumerate on 
				// the listing of keys.

				ICollection ic = _Hashtable.Keys(m_Attributes);
				ArrayList attribs = new ArrayList(ic.Count);

				foreach (object key in ic)
				{
					attribs.Add(key);
				}
				return attribs;
			}
		}

		/// <summary>
		/// Extracts the value of an attribute.
		///		size
		///		duration
		///		bitrate
		///		sampleFrequency
		///		bitsPerSample
		///		nrAudioChannels
		///		resolution
		///		colorDepth
		///		protocolInfo
		///		protection
		/// </summary>
		/// <param name="attribute">attribute name</param>
		/// <returns>returns a comparable value, casting to string form if ncessary</returns>
		public IComparable ExtractAttribute(string attribute)
		{
			if (string.Compare(attribute, "protocolInfo", true) == 0)
			{
				// because protocol info is required, it is not stored
				// the resource's hashtable of attribute=>values.
				return this.m_ProtocolInfo;
			}

			IComparable obj;
			lock (this.m_Attributes.SyncRoot)
			{
				obj = (IComparable) _Hashtable.Get(m_Attributes, attribute);
			}
			return obj;
		}

		protected void SetAttribute(string attribute, object val)
		{
			if (string.Compare(attribute, "protocolInfo", true) == 0)
			{
				// m_ProtocolInfo used to be a field but was later turned into a property. 
				// Under the covers, we actually add the value to m_Attributes array...
				// The difference is that m_ProtocolInfo does some checking to ensure that
				// the protocol info is valid.
				this.m_ProtocolInfo = (ProtocolInfoString) val;
			}
			else
			{
				lock (this.m_Attributes.SyncRoot)
				{
					_Hashtable.Set(m_Attributes, attribute, (IComparable) val);
				}
			}
		}

		/// <summary>
		/// Returns an object, often an
		/// <see cref="IValueType"/>
		/// but not always, for any of the attributes.
		/// The exception to the 
		/// <see cref="IValueType"/>
		/// case is importUri.
		/// </summary>
		public virtual object this[_RESATTRIB attrib]
		{
			get
			{
				return this[T[attrib]];
			}
			set
			{
				this.SetAttribute(T[attrib], value);
			}
		}
		
		/// <summary>
		/// Returns an object, often an
		/// <see cref="IValueType"/>
		/// but not always, for any of the attributes.
		/// The exception to the 
		/// <see cref="IValueType"/>
		/// case is importUri.
		/// </summary>
		public virtual object this[string attrib]
		{
			get
			{
				return this.ExtractAttribute(attrib);
			}
			set
			{
				this.SetAttribute(attrib, value);
			}
		}

		/// <summary>
		/// Prints the XML representation of the resource.
		/// <para>
		/// If the formatter's <see cref="ToXmlFormatter.WriteResource"/>
		/// field is non-null, then that delegate is executed. Otherwise,
		/// the implementation calls the <see cref="ToXmlData.ToXml"/>
		/// method for its implementation.
		/// </para>
		/// </summary>
		/// 
		/// <param name="formatter">
		/// Allows the caller to specify a custom implementation that writes
		/// the resources's XML. Simply assign the <see cref="ToXmlFormatter.WriteResource"/>
		/// to have this method delegate the responsibility. Otherwise,
		/// the the implementation of <see cref="ToXmlData.ToXml"/>
		/// is called.
		/// </param>
		/// 
		/// <param name="data">
		/// If the formatter's <see cref="ToXmlFormatter.WriteResource"/> field
		/// is non-null, then this object must be a type acceptable to that method's
		/// implementation. Otherwise, a <see cref="ToXmlData"/> object is required.
		/// </param>
		/// 
		/// <param name="xmlWriter">
		/// The <see cref="XmlTextWriter"/> object that
		/// will format the representation in an XML
		/// valid way.
		/// </param>
		/// <exception cref="InvalidCastException">
		/// Thrown if the "data" argument is not a <see cref="ToXmlData"/> object.
		/// </exception>
		public virtual void ToXml (ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter)
		{
			if (formatter.WriteResource != null)
			{
				formatter.WriteResource(this, formatter, data, xmlWriter);
			}
			else
			{
				ToXmlData.ToXml(this, formatter, (ToXmlData) data, xmlWriter);
			}
		}

		/// <summary>
		/// Instructs the "xmlWriter" argument to start the "res" element.
		/// </summary>
		/// <param name="formatter">
		/// A <see cref="ToXmlFormatter"/> object that
		/// specifies method implementations for printing
		/// media objects and metadata.
		/// </param>
		/// <param name="data">
		/// This object should be a <see cref="ToXmlData"/>
		/// object that contains additional instructions used
		/// by the "formatter" argument.
		/// </param>
		/// <param name="xmlWriter">
		/// The <see cref="XmlTextWriter"/> object that
		/// will format the representation in an XML
		/// valid way.
		/// </param>
		/// <exception cref="InvalidCastException">
		/// Thrown if the "data" argument is not a <see cref="ToXmlData"/> object.
		/// </exception>
		public virtual void StartElement(ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter)
		{
			ToXmlData _d = (ToXmlData) data;
			this.StartResourceXml(_d.DesiredProperties, xmlWriter);
			this.WriteAttributesXml(data, _d.DesiredProperties, xmlWriter);
		}

		/// <summary>
		/// Instructs the "xmlWriter" argument to write the value of
		/// the "res" element.
		/// </summary>
		/// <param name="formatter">
		/// A <see cref="ToXmlFormatter"/> object that
		/// specifies method implementations for printing
		/// media objects and metadata.
		/// </param>
		/// <param name="data">
		/// This object should be a <see cref="ToXmlData"/>
		/// object that contains additional instructions used
		/// by the "formatter" argument.
		/// </param>
		/// <param name="xmlWriter">
		/// The <see cref="XmlTextWriter"/> object that
		/// will format the representation in an XML
		/// valid way.
		/// </param>
		/// <exception cref="InvalidCastException">
		/// Thrown if the "data" argument is not a <see cref="ToXmlData"/> object.
		/// </exception>
		public virtual void WriteValue(ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter)
		{
			ToXmlData _d = (ToXmlData) data;
			this.WriteContentUriXml(_d.BaseUri, xmlWriter);
		}

		/// <summary>
		/// Instructs the "xmlWriter" argument to close the "res" element.
		/// </summary>
		/// <param name="formatter">
		/// A <see cref="ToXmlFormatter"/> object that
		/// specifies method implementations for printing
		/// media objects and metadata.
		/// </param>
		/// <param name="data">
		/// This object should be a <see cref="ToXmlData"/>
		/// object that contains additional instructions used
		/// by the "formatter" argument.
		/// </param>
		/// <param name="xmlWriter">
		/// The <see cref="XmlTextWriter"/> object that
		/// will format the representation in an XML
		/// valid way.
		/// </param>
		public virtual void EndElement(ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter)
		{
			this.EndResourceXml(xmlWriter);
		}

		/// <summary>
		/// Empty - res elements have no child xml elements.
		/// </summary>
		/// <param name="formatter">
		/// A <see cref="ToXmlFormatter"/> object that
		/// specifies method implementations for printing
		/// media objects and metadata.
		/// </param>
		/// <param name="data">
		/// This object should be a <see cref="ToXmlData"/>
		/// object that contains additional instructions used
		/// by the "formatter" argument.
		/// </param>
		/// <param name="xmlWriter">
		/// The <see cref="XmlTextWriter"/> object that
		/// will format the representation in an XML
		/// valid way.
		/// </param>
		public virtual void WriteInnerXml(ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter)
		{
			// This method intentionally left blank.
			;
		}

		/// <summary>
		/// Writes the attributes for this xml element, except importUri.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="desiredProperties"></param>
		/// <param name="xmlWriter"></param>
		protected virtual void WriteAttributesXml(object data, ArrayList desiredProperties, XmlTextWriter xmlWriter)
		{
			// always write protocolInfo attribute
			foreach (DictionaryEntry de in this.m_Attributes)
			{
				if (de.Key.ToString() != T[_RESATTRIB.protocolInfo])
				{
					StringBuilder sb = new StringBuilder(MediaResource.ATTRIBUTE_SIZE);
					sb.AppendFormat("{0}@{1}", T[_DIDL.Res], de.Key.ToString());
					string filterName = sb.ToString();

					if (desiredProperties == null)
					{
					}
					else if ((desiredProperties.Count == 0) || desiredProperties.Contains(filterName))
					{
						System.Diagnostics.Debug.Assert(de.Key != null);
						System.Diagnostics.Debug.Assert(de.Value != null);
						xmlWriter.WriteAttributeString(de.Key.ToString(), de.Value.ToString());
					}
				}
				else
				{
					xmlWriter.WriteAttributeString(T[_RESATTRIB.protocolInfo], de.Value.ToString());
				}
			}
		}

		/// <summary>
		/// Worst case size for a resource attribute name.
		/// </summary>
		private const int ATTRIBUTE_SIZE = 20;

		/// <summary>
		/// Declares the XML tag for this element.
		/// </summary>
		/// <param name="desiredProperties"></param>
		/// <param name="xmlWriter"></param>
		protected virtual void StartResourceXml(ArrayList desiredProperties, XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteStartElement(T[_DIDL.Res]);
		}


		/// <summary>
		/// Writes the contentUri value for the xml.
		/// </summary>
		/// <param name="baseUriInfo"></param>
		/// <param name="xmlWriter"></param>
		protected virtual void WriteContentUriXml(string baseUriInfo, XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteString(this.ContentUri);
		}

		/// <summary>
		/// Closes the xml tag for this element.
		/// </summary>
		/// <param name="xmlWriter"></param>
		protected virtual void EndResourceXml(XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteEndElement();
		}

		/// <summary>
		/// Returns true if the object is an
		/// <see cref="IValueType"/>
		/// has been properly set.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		protected bool IsAttribValid(object obj)
		{
			if (obj != null)
			{
				if (!(obj is IValueType))
				{
					// If the supplied object is not an IValueType,
					// we assume that the value is true because
					// it indicates the non-null object is a value
					// of some sort.
					return true;
				}
				else
				{
					// If the supplied object is an IValueType,
					// then value validity is determined by 
					// the IsValid property.
					return (((IValueType)obj).IsValid == true);
				}
			}
			return false;
		}

		/// <summary>
		/// Returns true if the bitrate has been set.
		/// </summary>
		public bool HasBitrate 
		{
			get 
			{ 
				object obj;
				lock (this.m_Attributes.SyncRoot)
				{
					obj = _Hashtable.Get(m_Attributes, T[_RESATTRIB.bitrate]);
				}
				return IsAttribValid(obj);
			}
		}

		/// <summary>
		/// Returns true if the bitsPerSample attribute has been set.
		/// </summary>
		public bool HasBitsPerSample
		{
			get 
			{ 
				object obj;
				lock (this.m_Attributes.SyncRoot)
				{
					obj = _Hashtable.Get(m_Attributes, T[_RESATTRIB.bitsPerSample]);
				}
				return IsAttribValid(obj);
			}
		}

		/// <summary>
		/// Returns true if the colorDepth attribute has been set.
		/// </summary>
		public bool HasColorDepth
		{
			get 
			{ 
				object obj;
				lock (this.m_Attributes.SyncRoot)
				{
					obj = _Hashtable.Get(m_Attributes, T[_RESATTRIB.colorDepth]);
				}
				return IsAttribValid(obj);
			}
		}

		/// <summary>
		/// Returns true if the duration attribute has been set.
		/// </summary>
		public bool HasDuration
		{
			get 
			{ 
				object obj;
				lock (this.m_Attributes.SyncRoot)
				{
					obj = _Hashtable.Get(m_Attributes, T[_RESATTRIB.duration]);
				}
				return IsAttribValid(obj);
			}
		}

		/// <summary>
		/// Returns true if the importUri attribute has been set.
		/// Derived classes may have a special value for the uri.
		/// </summary>
		public virtual bool HasImportUri
		{
			get 
			{ 
				object obj;
				lock (this.m_Attributes.SyncRoot)
				{
					obj = _Hashtable.Get(m_Attributes, T[_RESATTRIB.importUri]);
				}
				return IsAttribValid(obj);
			}
		}

		/// <summary>
		/// Returns true if the bitsPerSample attribute has been set.
		/// </summary>
		public bool HasNrAudioChannels
		{
			get 
			{ 
				object obj;
				lock (this.m_Attributes.SyncRoot)
				{
					obj = _Hashtable.Get(m_Attributes, T[_RESATTRIB.nrAudioChannels]);
				}
				return IsAttribValid(obj);
			}
		}

		/// <summary>
		/// Returns true if the protection attribute has been set.
		/// </summary>
		public bool HasProtection
		{
			get 
			{ 
				object obj;
				lock (this.m_Attributes.SyncRoot)
				{
					obj = _Hashtable.Get(m_Attributes, T[_RESATTRIB.protection]);
				}
				return IsAttribValid(obj);
			}
		}

		/// <summary>
		/// Returns true if protocol info has been set.
		/// </summary>
		public bool HasProtocolInfo
		{
			get 
			{ 
				object obj;
				lock (this.m_Attributes.SyncRoot)
				{
					obj = _Hashtable.Get(m_Attributes, T[_RESATTRIB.protocolInfo]);
				}
				return IsAttribValid(obj);
			}
		}


		/// <summary>
		/// Returns true if the resolution attribute has been set.
		/// </summary>
		public bool HasResolution
		{
			get 
			{ 
				object obj;
				lock (this.m_Attributes.SyncRoot)
				{
					obj = _Hashtable.Get(m_Attributes, T[_RESATTRIB.resolution]);
				}
				return IsAttribValid(obj);
			}
		}

		/// <summary>
		/// Returns true if the sampleFrequency attribute has been set.
		/// </summary>
		public bool HasSampleFrequency
		{
			get 
			{ 
				object obj;
				lock (this.m_Attributes.SyncRoot)
				{
					obj = _Hashtable.Get(m_Attributes, T[_RESATTRIB.sampleFrequency]);
				}
				return IsAttribValid(obj);
			}
		}

		/// <summary>
		/// Returns true if the size attribute has been set.
		/// </summary>
		public bool HasSize
		{
			get 
			{ 
				object obj;
				lock (this.m_Attributes.SyncRoot)
				{
					obj = _Hashtable.Get(m_Attributes, T[_RESATTRIB.size]);
				}
				return IsAttribValid(obj);
			}
		}


		/// <summary>
		/// Provides a general comparison test between two MediaResource
		/// objects. The method does not test for true equality between
		/// resources, rather this comparison provides a simple means
		/// for a resource to compare itself to another resource based
		/// on the key metadata fields that determine uniqueness.
		/// 
		/// <para>
		/// If the object is a MediaResource, then we compare 
		/// first on contentUri, then on protocolInfo, then on importUri.
		/// Attributes are not compared. 
		/// Otherwise, we ToString() the argument and compare
		/// the contentUri.
		/// </para>
		/// 
		/// <para>
		/// The primary motivation behind this implementation of CompareTo
		/// is that a MediaResource's value is determined by its
		/// content URI value and its protocolInfo. The ImportUri
		/// value is also included in the comparison because it can
		/// also add to the uniqueness factor in identifying a resource.
		/// </para>
		/// 
		/// <para>
		/// Thus, two resources have equal comparison by
		/// having the same contentUri, protocolInfo, and importUri
		/// even though they may have slightly different values
		/// for other attributes... a likely result in the case where
		/// two MediaServers point to the same content, but simply
		/// support different metadata attributes.
		/// </para>
		/// </summary>
		/// <param name="compareToThis"></param>
		/// <returns></returns>
		public int CompareTo (object compareToThis)
		{
			MediaResource mr = compareToThis as MediaResource;

			if (mr != null)
			{
				MediaResource res = (MediaResource) compareToThis;

				int c = string.Compare(this.ContentUri, res.ContentUri);

				if (c == 0)
				{
					c = string.Compare(this.ProtocolInfo.ToString(), res.ProtocolInfo.ToString());
					
					if (c == 0)
					{
						if (this.HasImportUri)
						{
							c = string.Compare(this.ImportUri, res.ImportUri);
						}
					}
				}
				
				return c;
			}
			else
			{
				// As a fallback, just take the string value of the other object
				// and compare it against the contentUri. This is useful when
				// we want to compare a resource against string representing
				// a URI - although the practice is not recommended. Programmers,
				// should do explicit comparison against the URI.
				return string.Compare(this.ContentUri, compareToThis.ToString());
			}
		}


		/// <summary>
		/// Keeps all of the attributes.
		/// <para>
		/// Derived classes will have access to this and so will internal classes.
		/// Programmers are strongly encouraged to not mess with this value unless
		/// they ABSOLUTELY know what they're doing.
		/// </para>
		/// </summary>
		private ArrayList m_Attributes = new ArrayList(1);

		/// <summary>
		/// Gets an enumerator for all of the attributes.
		/// </summary>
		/// <returns></returns>
		protected IDictionaryEnumerator GetAttributesEnumerator()
		{
			//return this.m_Attributes.GetEnumerator();
			return _Hashtable.GetEnumerator(m_Attributes);
		}

		/// <summary>
		/// Miscellaneous object for public programmer convenience.
		/// </summary>
		private object _Tag;

		/// <summary>
		/// Miscellaneous object for public programmer convenience.
		/// </summary>
		public object Tag { get { return _Tag; } set { _Tag = value; } }

		/// <summary>
		/// Hashcode is determined by every using the hashcode of 
		/// every field (except the owner) and returning the combined XOR result.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			/*
			StringBuilder sb = new StringBuilder(MediaResource.HashCodeSize);
			sb.Append(this.Bitrate.ToString());
			sb.Append(this.BitsPerSample.ToString());
			sb.Append(this.ColorDepth.ToString());
			sb.Append(this.Duration.ToString());
			sb.Append(this.m_ContentUri.ToString());
			sb.Append(this.m_ProtocolInfo.ToString());
			sb.Append(this.nrAudioChannels.ToString());
			sb.Append(this.Protection.ToString());
			sb.Append(this.Resolution.ToString());
			sb.Append(this.SampleFrequency.ToString());
			sb.Append(this.Size.ToString());
			return sb.ToString().GetHashCode();
			*/

			if (this.m_Hashcode == 0)
			{
				this.m_Hashcode = (
					this.Bitrate.GetHashCode() ^
					this.BitsPerSample.GetHashCode() ^
					this.ColorDepth.GetHashCode() ^
					this.Duration.GetHashCode() ^
					this.m_ContentUri.GetHashCode() ^
					this.m_ProtocolInfo.GetHashCode() ^
					this.nrAudioChannels.GetHashCode() ^
					this.Protection.GetHashCode() ^
					this.Resolution.GetHashCode() ^
					this.SampleFrequency.GetHashCode() ^
					this.Size.GetHashCode() 
					);
			}

			return this.m_Hashcode;
		}

		/// <summary>
		/// If this value is zero, then it means we need to recalculate the hashcode.
		/// </summary>
		private int m_Hashcode = 0;

		/// <summary>
		/// Returns true if the supplied MediaResource is equal to this one. Equality is based
		/// on matching of every field, except the owner.
		/// 
		/// <para>
		/// This Equals() operation performs a comparison of every field to determine equality, which is
		/// very different than the <see cref="MediaResource.CompareTo"/> method. This method is 
		/// designed to indicate if two MediaResource objects have the exact same metadata.
		/// The <see cref="MediaResource.CompareTo"/> method is designed to indicate if two MediaResource
		/// objects point to the same content on a MediaServer.
		/// </para>
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			MediaResource mr = obj as MediaResource;

			if (mr != null)
			{
				if (
					(mr.Bitrate.Equals(this.Bitrate)) &&
					(mr.BitsPerSample.Equals(this.BitsPerSample)) &&
					(mr.ColorDepth.Equals(this.ColorDepth)) &&
					(mr.Duration.Equals(this.Duration)) &&
					(mr.m_ContentUri.Equals(this.m_ContentUri)) &&
					(mr.m_ProtocolInfo.Equals(this.m_ProtocolInfo)) &&
					(mr.nrAudioChannels.Equals(this.nrAudioChannels)) &&
					(mr.Protection.Equals(this.Protection)) &&
					(mr.Resolution.Equals(this.Resolution)) &&
					(mr.SampleFrequency.Equals(this.SampleFrequency)) &&
					(mr.Size.Equals(this.Size)) && 
					(mr.ImportUri.Equals(this.ImportUri))
					)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// <para>
		/// Maps the "contentUri" element of a resource. This
		/// property has the URI of a resource, from which the
		/// content can be consumed. Sometimes the value is
		/// an empty string, to indicate that resource
		/// is not available for consumption at the current time.
		/// </para>
		/// 
		/// <para>
		/// In terms of content acquired through the "http-get"
		/// protocol, this maps to a fully-qualified 
		/// http url when viewed from the UPNP network.
		/// </para>
		/// </summary>
		public virtual string ContentUri	
		{ 
			get 
			{ 
				return this.m_ContentUri; 
			} 
			set
			{
				this.m_Hashcode = 0;
				this.m_ContentUri = value;
			}
		}

		/// <summary>
		/// This indicates the URI where control points can POST a binary
		/// for replacing the existing binary for the resource. This URI is
		/// also used in the 
		/// ContentDirectory service's ImportResource action as the
		/// destination parameter.
		/// </summary>
		public virtual string ImportUri 
		{
			get 
			{ 
				object obj;
				lock (this.m_Attributes.SyncRoot)
				{
					obj = _Hashtable.Get(m_Attributes, T[_ATTRIB.importUri]);
				}
				if (obj != null)
				{
					return (string) obj;
				}
				
				return "";
			}
			set
			{
				this.m_Hashcode = 0;
				lock (this.m_Attributes.SyncRoot)
				{
					_Hashtable.Set(m_Attributes, T[_RESATTRIB.importUri], value);
				}
			}
		}

		/// <summary>
		/// This is the "protocolInfo" attribute of a "res" element.
		/// This metadata is required, and indicates the type of
		/// resource, how and where it can be consumed.
		/// </summary>
		public virtual ProtocolInfoString ProtocolInfo	
		{ 
			get 
			{ 
				return m_ProtocolInfo; 
			}
			set
			{
				this.m_ProtocolInfo = value;
			}
		}

		/// <summary>
		/// Gets/Sets the specified attribute. If the value hasn't been
		/// set, the IsValid property will be false.
		/// </summary>
		public virtual _UInt Bitrate 
		{ 
			get 
			{ 
				object obj;
				lock (this.m_Attributes.SyncRoot)
				{
					obj = _Hashtable.Get(m_Attributes, T[_RESATTRIB.bitrate]);
				}
				return PreventNullCast.CastUInt(obj);
			} 
			set
			{
				this.m_Hashcode = 0;
				lock (this.m_Attributes.SyncRoot)
				{
					_Hashtable.Set(m_Attributes, T[_RESATTRIB.bitrate], value);
				}
			}
		}
		
		/// <summary>
		/// Gets/Sets the specified attribute. If the value hasn't been
		/// set, the IsValid property will be false.
		/// </summary>
		public virtual _UInt BitsPerSample 
		{ 
			get 
			{ 
				object obj;
				lock (this.m_Attributes.SyncRoot)
				{
					obj = _Hashtable.Get(m_Attributes, T[_RESATTRIB.bitsPerSample]);
				}
				return PreventNullCast.CastUInt(obj);
			} 
			set
			{
				this.m_Hashcode = 0;
				lock (this.m_Attributes.SyncRoot)
				{
					_Hashtable.Set(m_Attributes, T[_RESATTRIB.bitsPerSample], value);
				}
			}		
		}

		/// <summary>
		/// Gets/Sets the specified attribute. If the value hasn't been
		/// set, the IsValid property will be false.
		/// </summary>
		public virtual _UInt ColorDepth 
		{ 
			get 
			{ 
				object obj;
				lock (this.m_Attributes.SyncRoot)
				{
					obj = _Hashtable.Get(m_Attributes, T[_RESATTRIB.colorDepth]);
				}
				return PreventNullCast.CastUInt(obj);
			} 
			set
			{
				this.m_Hashcode = 0;
				lock (this.m_Attributes.SyncRoot)
				{
					_Hashtable.Set(m_Attributes, T[_RESATTRIB.colorDepth], value);
				}
			}			
		}
		
		/// <summary>
		/// Gets/Sets the specified attribute. If the value hasn't been
		/// set, the IsValid property will be false.
		/// </summary>
		public virtual _TimeSpan Duration
		{ 
			get 
			{ 
				object obj;
				lock (this.m_Attributes.SyncRoot)
				{
					obj = _Hashtable.Get(m_Attributes, T[_RESATTRIB.duration]);
				}
				return PreventNullCast.CastTimeSpan(obj);
			} 
			set
			{
				this.m_Hashcode = 0;
				lock (this.m_Attributes.SyncRoot)
				{
					_Hashtable.Set(m_Attributes, T[_RESATTRIB.duration], value);
				}
			}			
		}

		/// <summary>
		/// Gets/Sets the specified attribute. If the value hasn't been
		/// set, the IsValid property will be false.
		/// </summary>
		public virtual _UInt nrAudioChannels
		{ 
			get 
			{ 
				object obj;
				lock (this.m_Attributes.SyncRoot)
				{
					obj = _Hashtable.Get(m_Attributes, T[_RESATTRIB.nrAudioChannels]);
				}
				return PreventNullCast.CastUInt(obj);
			} 
			set
			{
				this.m_Hashcode = 0;
				lock (this.m_Attributes.SyncRoot)
				{
					_Hashtable.Set(m_Attributes, T[_RESATTRIB.nrAudioChannels], value);
				}
			}			
		}

		/// <summary>
		/// Gets/Sets the specified attribute. If the value hasn't been
		/// set, the value is an empty string.
		/// </summary>
		public virtual string Protection
		{ 
			get 
			{ 
				object obj;
				lock (this.m_Attributes.SyncRoot)
				{
					obj = _Hashtable.Get(m_Attributes, T[_RESATTRIB.protection]);
				}
				return PreventNullCast.CastString(obj);
			} 
			set
			{
				this.m_Hashcode = 0;
				lock (this.m_Attributes.SyncRoot)
				{
					_Hashtable.Set(m_Attributes, T[_RESATTRIB.protection], value);
				}
			}			
		}

		/// <summary>
		/// Gets/Sets the specified attribute. If the value hasn't been
		/// set, the IsValid property will be false.
		/// </summary>
		public virtual ImageDimensions Resolution
		{
			get
			{
				object obj;
				lock (this.m_Attributes.SyncRoot)
				{
					obj = _Hashtable.Get(m_Attributes, T[_RESATTRIB.resolution]);
				}
				if (obj != null)
				{
					ImageDimensions res = (ImageDimensions) obj;
					return res;
				}
				
				ImageDimensions nulld = new ImageDimensions(false);
				return nulld;
			}
			set
			{
				this.m_Hashcode = 0;
				lock (this.m_Attributes.SyncRoot)
				{
					_Hashtable.Set(m_Attributes, T[_RESATTRIB.resolution], value);
				}
			}		
		}

		/// <summary>
		/// Gets/Sets the specified attribute. If the value hasn't been
		/// set, the IsValid property will be false.
		/// </summary>
		public virtual _UInt SampleFrequency
		{ 
			get 
			{ 
				object obj;
				lock (this.m_Attributes.SyncRoot)
				{
					obj = _Hashtable.Get(m_Attributes, T[_RESATTRIB.sampleFrequency]);
				}
				return PreventNullCast.CastUInt(obj);
			} 
			set
			{
				this.m_Hashcode = 0;
				lock (this.m_Attributes.SyncRoot)
				{
					_Hashtable.Set(m_Attributes, T[_RESATTRIB.sampleFrequency], value);
				}
			}			
		}

		/// <summary>
		/// Gets/Sets the specified attribute. If the value hasn't been
		/// set, the IsValid property will be false.
		/// </summary>
		public virtual _ULong Size
		{ 
			get 
			{ 
				object obj;
				lock (this.m_Attributes.SyncRoot)
				{
					obj = _Hashtable.Get(m_Attributes, T[_RESATTRIB.size]);
				}
				return PreventNullCast.CastULong (obj);
			} 
			set
			{
				this.m_Hashcode = 0;
				lock (this.m_Attributes.SyncRoot)
				{
					_Hashtable.Set(m_Attributes, T[_RESATTRIB.size], value);
				}
			}			
		}
		
		/// <summary>
		/// This indicates the item or container that owns the resource.
		/// Each resource can only be owned by a single item or container,
		/// although items and containers can have multiple resources.
		/// </summary>
		public virtual IUPnPMedia Owner
		{
			get
			{
				return this.m_Owner;
			}
			set
			{
				this.m_Owner = value;
			}
		}

		/// <summary>
		/// The value of the contentUri property can be changed by setting this field.
		/// <para>
		/// Derived classes will have access to this and so will internal classes.
		/// Programmers are strongly encouraged to not mess with this value unless
		/// they ABSOLUTELY know what they're doing.
		/// </para>
		/// </summary>
		protected string m_ContentUri
		{
			get
			{
				lock (this.m_ContentUriPath)
				{
					int l0=0, l1=0, l2=0, l3;

					l0 = this.m_ContentUriScheme.Length;
					l1 = this.m_ContentUriPath.Length;
					l2 = this.m_ContentUriFile.Length;

					l3 = l0+l1+l2;
					if (l3 == 0)
					{
						return "";
					}
					else
					{
						StringBuilder sb = new StringBuilder(l3);
						if (l0 > 0)
						{
							sb.Append(this.m_ContentUriScheme);
						}
						if (l1 > 0)
						{
							sb.Append(this.m_ContentUriPath);
						}
						if (l2 > 0)
						{
							sb.Append(this.m_ContentUriFile);
						}

						return sb.ToString();
					}
				}
			}
			set
			{
				lock (this.m_ContentUriPath)
				{
					string full = value;
					string queryLess = value;

					int queryPos = full.IndexOf("?");

					if (queryPos > 0)
					{
						queryLess = full.Substring(0, queryPos);
					}

					int lastFrwdSlash = queryLess.LastIndexOf('/');
					int lastBackSlash = queryLess.LastIndexOf('\\');
					int slashPos = Math.Max(lastFrwdSlash, lastBackSlash);
					int slashPos2 = slashPos + 1;

					if ((slashPos > 0) && (slashPos2 < full.Length))
					{
						this.m_ContentUriPath = full.Substring(0, slashPos2);
						this.m_ContentUriFile = full.Substring(slashPos2);
					}
					else
					{
						this.m_ContentUriPath = value;
						this.m_ContentUriFile = "";
					}

					// look specifically for the scheme marker of a URI
					int schemePos = this.m_ContentUri.IndexOf("://");

					if (schemePos >= 0)
					{
						this.m_ContentUriScheme = (string) CdsMetadataCaches.Data.CacheThis(this.m_ContentUriPath.Substring(0, schemePos+3));
						this.m_ContentUriPath = (string) CdsMetadataCaches.Data.CacheThis(this.m_ContentUriPath.Substring(schemePos+3));
					}
					else
					{
						this.m_ContentUriPath = (string) CdsMetadataCaches.Data.CacheThis(this.m_ContentUriPath);
						m_ContentUriScheme = "";
					}
				}
			}
		}

		private string m_ContentUriScheme = "";
		private string m_ContentUriPath = "";
		private string m_ContentUriFile = "";

		/// <summary>
		/// The protocolInfo property can be changed directly by setting this field.
		/// <para>
		/// Derived classes will have access to this and so will internal classes.
		/// Programmers are strongly encouraged to not mess with this value unless
		/// they ABSOLUTELY know what they're doing.
		/// </para>
		/// </summary>
		protected internal ProtocolInfoString m_ProtocolInfo
		{
			get
			{
				object obj;
				lock (this.m_Attributes.SyncRoot)
				{
					obj = _Hashtable.Get(this.m_Attributes, T[_RESATTRIB.protocolInfo]);
				}
				if (obj == null)
				{
					return CdsMetadataCaches.ProtocolInfoStrings.CacheThis(ProtocolInfoString.DefaultValue);
				}
				else
				{
					ProtocolInfoString protInfo = (ProtocolInfoString) obj;
					return protInfo;
				}
			}
			set
			{
				ProtocolInfoString protInfo;

				this.m_Hashcode = 0;
				lock (this.m_Attributes.SyncRoot)
				{
					protInfo = (ProtocolInfoString) CdsMetadataCaches.ProtocolInfoStrings.CacheThis(value.ToString());
					_Hashtable.Set(this.m_Attributes, T[_RESATTRIB.protocolInfo], protInfo);
				}
			}
		}

		/// <summary>
		/// The Owner property can be changed directly by setting this field.
		/// <para>
		/// Derived classes will have access to this and so will internal classes.
		/// Programmers are strongly encouraged to not mess with this value unless
		/// they ABSOLUTELY know what they're doing.
		/// </para>
		/// </summary>
		[NonSerialized()] protected IUPnPMedia m_Owner = null;

		~MediaResource()
		{
			OpenSource.Utilities.InstanceTracker.Remove(this);
		}

		/// <summary>
		/// Default constructor. 
		/// </summary>
		public MediaResource()
		{
			OpenSource.Utilities.InstanceTracker.Add(this);
		}


		/// <summary>
		/// Allows construction of a media resource from an XmlElement
		/// that represents the "res" node, and is also DIDL-Lite compliant.
		/// </summary>
		/// <param name="xmlElement"></param>
		public MediaResource(XmlElement xmlElement)
		{
			OpenSource.Utilities.InstanceTracker.Add(this);
			bool foundProtInfo = false;
			lock (this.m_Attributes.SyncRoot)
			{
				foreach (XmlAttribute attrib in xmlElement.Attributes)
				{
					if (String.Compare(attrib.LocalName, T[_ATTRIB.protocolInfo], true) == 0)
					{
						this.m_ProtocolInfo = new ProtocolInfoString(attrib.Value);
						foundProtInfo = true;
					}
					else if (String.Compare(attrib.LocalName, T[_ATTRIB.importUri], true) == 0)
					{
						if (attrib.Value.ToString() != "")
						{
							_Hashtable.Set(m_Attributes, T[_ATTRIB.importUri], attrib.Value);
						}
					}
					else if (String.Compare(attrib.LocalName, T[_ATTRIB.bitrate], true) == 0)
					{
						try
						{
							_Hashtable.Set(m_Attributes, T[_ATTRIB.bitrate], new _UInt(uint.Parse(attrib.Value)));
						}
						catch
						{
						}
					}
					else if (String.Compare(attrib.LocalName, T[_ATTRIB.bitsPerSample], true) == 0)
					{
						try
						{
							_Hashtable.Set(m_Attributes, T[_ATTRIB.bitsPerSample], new _UInt(uint.Parse(attrib.Value)));
						}
						catch
						{
						}
					}
					else if (String.Compare(attrib.LocalName, T[_ATTRIB.colorDepth], true) == 0)
					{
						try
						{
							_Hashtable.Set(m_Attributes, T[_ATTRIB.colorDepth], new _UInt(uint.Parse(attrib.Value)));
						}
						catch
						{
						}
					}
					else if (String.Compare(attrib.LocalName, T[_ATTRIB.duration], true) == 0)
					{
						try
						{
							_Hashtable.Set(m_Attributes, T[_ATTRIB.duration], new _TimeSpan(attrib.Value));
						}
						catch
						{
						}
					}
					else if (String.Compare(attrib.LocalName, T[_ATTRIB.nrAudioChannels], true) == 0)
					{
						try
						{
							_Hashtable.Set(m_Attributes, T[_ATTRIB.nrAudioChannels], new _UInt(uint.Parse(attrib.Value)));
						}
						catch
						{
						}
					}
					else if (String.Compare(attrib.LocalName, T[_ATTRIB.protection], true) == 0)
					{
						try
						{
							if (attrib.Value.ToString() != "")
							{
								_Hashtable.Set(m_Attributes, T[_ATTRIB.protection], attrib.Value);
							}
						}
						catch
						{
						}
					}
					else if (String.Compare(attrib.LocalName, T[_ATTRIB.resolution], true) == 0)
					{
						try
						{
							_Hashtable.Set(m_Attributes, T[_ATTRIB.resolution], new ImageDimensions(attrib.Value));
						}
						catch
						{
						}
					}
					else if (String.Compare(attrib.LocalName, T[_ATTRIB.sampleFrequency], true) == 0)
					{
						try
						{
							_Hashtable.Set(m_Attributes, T[_ATTRIB.sampleFrequency], new _UInt(uint.Parse(attrib.Value)));
						}
						catch
						{
						}

					}
					else if (String.Compare(attrib.LocalName, T[_ATTRIB.size], true) == 0)
					{
						try
						{
							_Hashtable.Set(m_Attributes, T[_ATTRIB.size], new _ULong(ulong.Parse(attrib.Value)));
						}
						catch
						{
						}
					}
				}
				if (foundProtInfo == false)
				{
					this.m_ProtocolInfo = new ProtocolInfoString(ProtocolInfoString.DefaultValue);
				}
			}

			m_ContentUri = xmlElement.InnerText.Trim();
		}

		/// <summary>
		/// MediaResources must have a contentURI and a valid protocolInfo string
		/// at minimum.
		/// </summary>
		/// <param name="contentUri">The URI from which the resource can be consumed.</param>
		/// <param name="protocolInfo">The valid protocolInfo string in the form: "[protocol]:[network]:[mime type]:[info]".</param>
		public MediaResource(string contentUri, string protocolInfo)
		{
			OpenSource.Utilities.InstanceTracker.Add(this);
			m_ContentUri = contentUri.Trim();
			m_ProtocolInfo = new ProtocolInfoString(protocolInfo);
		}

		/// <summary>
		/// Used for obtaining the attribute and tag names of standard metadata properties.
		/// </summary>
		protected static Tags T = Tags.GetInstance();
		/// <summary>
		/// Used for delimited text parsing.
		/// </summary>
		protected static DText DT = new DText();

		public void OnDeserialization(object sender)
		{
			this.m_ProtocolInfo = new ProtocolInfoString( this.m_ProtocolInfo.ToString() );
		}	
		
		public virtual void TrimToSize()
		{
			if (this.m_Attributes != null)
			{
				lock (this.m_Attributes.SyncRoot)
				{
					this.m_Attributes.TrimToSize();
				}
			}
		}

		/// <summary>
		/// This is the protocol to use to specify a local file as a contentUri.
		/// Architecturally, this belongs in a different namespace, but it is declared
		/// in here for convenience. The default value is
		/// "://" because it's unique and can never properly map to a URI. 
		/// Public programmers can set this value themselves, but they should keep 
		/// the name consistent throughout during run-time.
		/// 
		/// <para>
		/// If programmers wish to embed additional info into the path, they MUST
		/// delimit using the '?' character, much like the query. Other illegal
		/// characters in the contentUri will cause errors.
		/// </para>
		/// </summary>
		public static string AUTOMAPFILE = "://";
	}
}
