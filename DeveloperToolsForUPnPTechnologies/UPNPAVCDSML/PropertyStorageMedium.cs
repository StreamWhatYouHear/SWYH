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
	/// Class can represent any ContentDirectory approved
	/// upnp or dublin-core metadata elements that are
	/// simple integers without any attributes.
	/// </summary>
	[Serializable()]
	public sealed class PropertyStorageMedium : ICdsElement, IToXmlData, IDeserializationCallback
	{
		/// <summary>
		/// Returns same value as StringValue
		/// </summary>
		/// <returns></returns>
		public override string ToString() { return this.StringValue; }

		/// <summary>
		/// Returns an empty list.
		/// </summary>
		/// <returns></returns>
		public static IList GetPossibleAttributes()
		{
			return new object[0];
		}

		/// <summary>
		/// Returns an empty list.
		/// </summary>
		/// <returns></returns>
		public IList PossibleAttributes 
		{
			get
			{
				return GetPossibleAttributes();
			}
		}

		/// <summary>
		/// Returns an empty list.
		/// </summary>
		/// <returns></returns>
		public IList ValidAttributes 
		{
			get
			{
				return GetPossibleAttributes();
			}
		}

		
		/// <summary>
		/// No attributes, so always returns null
		/// </summary>
		/// <param name="attribute"></param>
		/// <returns></returns>
		public IComparable ExtractAttribute(string attribute) { return null; }

		/// <summary>
		/// Casts the EnumWriteStatus value to an int so that it is comparable.
		/// </summary>
		public IComparable ComparableValue { get { return (int) m_value; } }
		
		/// <summary>
		/// Returns the underlying enumeration value.
		/// </summary>
		public object Value { get { return m_value; } }
		
		/// <summary>
		/// Returns the enumeration value in its string form.
		/// </summary>
		public string StringValue { get { return m_value.ToString(); } }
		
		/// <summary>
		/// Prints the XML representation of the metadata element.
		/// <para>
		/// Implementation calls the <see cref="ToXmlData.ToXml"/>
		/// method for its implementation.
		/// </para>
		/// </summary>
		/// <param name="formatter">
		/// A <see cref="ToXmlFormatter"/> object that
		/// specifies method implementations for printing
		/// media objects.
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
		public void ToXml (ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter)
		{
			ToXmlData.ToXml(this, formatter, (ToXmlData) data, xmlWriter);
		}

		/// <summary>
		/// Instructs the "xmlWriter" argument to start the metadata element.
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
		public void StartElement(ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteStartElement(this.ns_tag);
		}

		/// <summary>
		/// Instructs the "xmlWriter" argument to write the value of
		/// the metadata element.
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
		public void WriteValue(ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteString(this.StringValue);
		}

		/// <summary>
		/// Instructs the "xmlWriter" argument to close the metadata element.
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
		public void EndElement(ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteEndElement();
		}

		/// <summary>
		/// Empty - this element has no child xml elements.
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
		public void WriteInnerXml(ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter)
		{
		}

		/// <summary>
		/// Attempts to case compareToThis as an EnumWriteStatus
		/// to do the comparison. If the cast
		/// fails, then it compares the current
		/// value against EnumWriteStatus.MinValue.
		/// </summary>
		/// <param name="compareToThis"></param>
		/// <returns></returns>
		public int CompareTo (object compareToThis)
		{
			System.Type type = compareToThis.GetType();

			EnumWriteStatus compareTo = (EnumWriteStatus) Enum.Parse(typeof(EnumWriteStatus), compareToThis.ToString(), true);

			int o1 = (int) this.m_value;
			int o2 = (int) compareTo;

			return o1.CompareTo(o2);
		}

		/// <summary>
		/// Extracts the StorageMedium.Enum value (in string form) from the element and uses
		/// the InnerText as the value.
		/// </summary>
		/// <param name="element"></param>
		public PropertyStorageMedium(XmlElement element)
		{
			this.m_value = (StorageMediums.Enum) Enum.Parse(typeof(StorageMediums.Enum), element.InnerText, true);
			this.ns_tag = (string) CdsMetadataCaches.Didl.CacheThis(element.Name);
		}
		
		/// <summary>
		/// Instantiates a new property that can represent an xml element with 
		/// the StorageMedium.Enum enumeration as its value.
		/// </summary>
		/// <param name="namespace_tag">property obtained from Tags[CommonPropertyNames.value]</param>
		/// <param name="val"></param>
		public PropertyStorageMedium (string namespace_tag, StorageMediums.Enum val)
		{
			this.ns_tag = (string) CdsMetadataCaches.Didl.CacheThis(namespace_tag);
			this.m_value = val;
		}

		/// <summary>
		/// Assume the name has been read.
		/// </summary>
		/// <param name="reader"></param>
		public PropertyStorageMedium(XmlReader reader)
		{

			this.m_value = (StorageMediums.Enum) Enum.Parse(typeof(StorageMediums.Enum), reader.Value, true);
			this.ns_tag = (string) CdsMetadataCaches.Didl.CacheThis(reader.Name);
		}

		/// <summary>
		/// Underlying enumeration value.
		/// </summary>
		public StorageMediums.Enum _Value { get { return this.m_value; } }
		internal StorageMediums.Enum m_value;

		/// <summary>
		/// the namespace and element name to represent
		/// </summary>
		private string ns_tag;

		public void OnDeserialization(object sender)
		{
			this.ns_tag = (string) CdsMetadataCaches.Didl.CacheThis(this.ns_tag);
		}	
	}
}
