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
	/// A value type, that can be used in metadata blocks,
	/// such as the various inner classes of
	/// <see cref="MediaBuilder"/>
	/// to instantate a new 
	/// <see cref="PropertyPersonWithRole"/>.
	/// </summary>
	[Serializable()]
	public sealed class PersonWithRole
	{
		/// <summary>
		/// The role of the person, such as primary author, editor,
		/// guitarist, drummer, lead vocals, starring actor, etc.
		/// </summary>
		public string Role;
		/// <summary>
		/// The name of the person.
		/// </summary>
		public string Name;
	}

	/// <summary>
	/// Class can represent any ContentDirectory approved
	/// upnp or dublin-core metadata elements that 
	/// represent a string with a string attribute called "role".
	/// </summary>
	[Serializable()]
	public sealed class PropertyPersonWithRole : ICdsElement, IToXmlData, IDeserializationCallback
	{
		/// <summary>
		/// Returns the Name in string form.
		/// </summary>
		/// <returns></returns>
		public override string ToString() { return this.StringValue; }

		/// <summary>
		/// Returns "role" as the only attribute in a list.
		/// </summary>
		/// <returns></returns>
		public static IList GetPossibleAttributes()
		{
			object[] objs = new object[1];
			objs[0] = "role";
			return objs;
		}

		/// <summary>
		/// Returns "role" as the only attribute in a list.
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
		/// Returns "role" as the only attribute in a list
		/// if the role is actually set. Otherwise
		/// the list is empty.
		/// </summary>
		/// <returns></returns>
		public IList ValidAttributes 
		{
			get
			{
				if (this.m_role != null)
				{
					if (this.m_role != "")
					{
						return GetPossibleAttributes();
					}
				}

				return new object[0];
			}
		}

		/// <summary>
		/// Returns the role, if specified.
		/// </summary>
		/// <param name="attribute"></param>
		/// <returns></returns>
		public IComparable ExtractAttribute(string attribute)
		{ 
			if (string.Compare(attribute, "role", true) == 0)
			{
				return this.m_role;
			}

			return null;
		}

		/// <summary>
		/// Returns the name in string form.
		/// </summary>
		public IComparable ComparableValue { get { return m_value; } }

		/// <summary>
		/// Returns the name in string form.
		/// </summary>
		public object Value { get { return m_value; } }

		/// <summary>
		/// Returns the name in string form.
		/// </summary>
		public string StringValue { get { return m_value; } }
		
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
		/// <exception cref="InvalidCastException">
		/// Thrown if the "data" argument is not a <see cref="ToXmlData"/> object.
		/// </exception>
		public void StartElement(ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter)
		{
			ToXmlData _d = (ToXmlData) data;

			xmlWriter.WriteStartElement(this.ns_tag);
			if (_d.DesiredProperties != null)
			{
				if (_d.DesiredProperties.Contains(this.ns_tag+"@role"))
				{
					if ((this.m_role != null) && (this.m_role != ""))
					{
						xmlWriter.WriteAttributeString("role", this.m_role);
					}
				}
			}
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
		/// Compares against string, PropertyPersonWithRole objects,
		/// or simply calls to ToString() on the
		/// object and then compares.
		/// Given that people will not likely enter
		/// case-sensitive searches, the
		/// comparison is case insensitive.
		/// </summary>
		/// <param name="compareToThis"></param>
		/// <returns></returns>
		public int CompareTo (object compareToThis)
		{
			System.Type type = compareToThis.GetType();

			if (type == typeof(PropertyPersonWithRole))
			{
				return string.Compare(this.m_value, ((PropertyPersonWithRole)compareToThis).m_value, true);
			}
			else
			{
				return string.Compare(this.m_value, compareToThis.ToString(), true);
			}
		}

		/// <summary>
		/// Extracts the Name of the element and uses
		/// the InnerText as the value.
		/// </summary>
		/// <param name="element"></param>
		public PropertyPersonWithRole(XmlElement element)
		{

			this.m_value = element.InnerText;
			this.ns_tag = (string) CdsMetadataCaches.Didl.CacheThis(element.Name);
			
			string attribVal = element.GetAttribute(T[_ATTRIB.role]);

			this.m_role = attribVal;
		}

		/// <summary>
		/// Instantiates a new property that can represent an xml element with 
		/// an PersonWithRole dissected into arguments.
		/// </summary>
		/// <param name="namespace_tag">property obtained from Tags[CommonPropertyNames.value]</param>
		/// <param name="val"></param>
		/// <param name="role">the role of the person, such as "guitarist", or "editor", or "starring actor"</param>
		public PropertyPersonWithRole (string namespace_tag, string val, string role)
		{
			this.ns_tag = (string) CdsMetadataCaches.Didl.CacheThis(namespace_tag);
			this.m_value = val;
			this.m_role = role;
		}

		/// <summary>
		/// Instantiates a new property that can represent an xml element with 
		/// an PersonWithRole as its value.
		/// </summary>
		/// <param name="namespace_tag">property obtained from Tags[CommonPropertyNames.value]</param>
		/// <param name="person">the person's information</param>
		public PropertyPersonWithRole (string namespace_tag, PersonWithRole person)
		{
			this.ns_tag = (string) CdsMetadataCaches.Didl.CacheThis(namespace_tag);
			this.m_value = person.Name;
			this.m_role = person.Role;
		}
		
		/// <summary>
		/// Assume the name has been read.
		/// </summary>
		/// <param name="reader"></param>
		public PropertyPersonWithRole(XmlReader reader)
		{

			this.m_value = reader.Value;
			this.ns_tag = (string) CdsMetadataCaches.Didl.CacheThis(reader.Name);

			if (reader.MoveToAttribute(T[_ATTRIB.role]))
			{
				this.m_role = reader.Value;
				reader.MoveToElement();
			}
			else
			{
				this.m_role = "";
			}
		}

		/// <summary>
		/// Underlying string for the name.
		/// </summary>
		public string _Value { get { return this.m_value; } }
		internal string m_value;

		/// <summary>
		/// The namespace and element name to represent.
		/// </summary>
		private string ns_tag;

		/// <summary>
		/// The role of the person.
		/// </summary>
		public string _Role { get { return this.m_role; } }
		internal string m_role;

		/// <summary>
		/// Allows for easy acquisition of attributes and tag names.
		/// </summary>
		private static Tags T = Tags.GetInstance();
		
		public void OnDeserialization(object sender)
		{
			this.m_role = (string) CdsMetadataCaches.CommonData.CacheThis(this.m_role);
			this.m_value = (string) CdsMetadataCaches.Data.CacheThis(this.m_value);
			this.ns_tag = (string) CdsMetadataCaches.Didl.CacheThis(this.ns_tag);
		}	
	}
}
