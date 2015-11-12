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
using System.Collections;
using System.Runtime.Serialization;
using OpenSource.UPnP.AV;

namespace OpenSource.UPnP.AV.CdsMetadata
{
	/// <summary>
	/// Maps to the CDS/UPNP-AV "upnp:class" element, which 
	/// describes the media class for a ContentDirectory
	/// content entry.
	/// </summary>
	[Serializable()]
	public class MediaClass : ICdsElement, IToXmlData, IDeserializationCallback
	{
		/// <summary>
		/// Generates a hashcode using the hashcodes of the fields multiplied by prime numbers,
		/// then summed together.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return
				(this.m_DerivedFrom.GetHashCode()) +
				(this.m_ClassName.GetHashCode() * 2) +
				(this.m_FriendlyName.GetHashCode() * 3);
		}

		/// <summary>
		/// Returns true if the other MediaClass has the same value and attribute values.
		/// </summary>
		/// <param name="cdsElement">a MediaClass instance; derived classes don't count</param>
		/// <returns></returns>
		public override bool Equals (object cdsElement)
		{
			MediaClass other = (MediaClass) cdsElement;

			if (this.GetType() == other.GetType())
			{
				if (this.m_ClassName.Equals(other.m_ClassName))
				{
					if (this.m_DerivedFrom.Equals(other.m_DerivedFrom))
					{
						if (this.m_FriendlyName.Equals(other.m_FriendlyName))
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		public string FullClassName
		{
			get
			{
				return this.StringValue;
			}
		}


		/// <summary>
		/// Returns true if the StringValue values of both media classes are equal.
		/// </summary>
		/// <param name="_class"></param>
		/// <returns></returns>
		public bool FullClassNameMatches (MediaClass _class)
		{
			return (string.Compare(this.ToString(), _class.ToString(), true) == 0);
		}

		/// <summary>
		/// Returns the full name of the media class.
		/// </summary>
		public virtual IComparable ComparableValue
		{
			get
			{
				return this.StringValue;
			}
		}

		/// <summary>
		/// Returns the full name of the media class.
		/// </summary>
		public virtual object Value
		{
			get
			{
				return this.StringValue;
			}
		}

		/// <summary>
		/// Returns the full name of the media class.
		/// </summary>
		public virtual string StringValue 
		{ 
			get
			{
				StringBuilder sb = new StringBuilder((int) (this.m_DerivedFrom.Length + 1 + this.m_ClassName.Length));
				sb.AppendFormat("{0}.{1}", this.m_DerivedFrom, this.m_ClassName);
				return sb.ToString();
			}
		}

		/// <summary>
		/// Only attribute available is "name"
		/// </summary>
		/// <returns></returns>
		public static IList GetPossibleAttributes()
		{
			string[] attributes = {"name"};
			return attributes;
		}

		/// <summary>
		/// Returns the listing of possible attributes.
		/// </summary>
		public virtual IList PossibleAttributes 
		{
			get
			{
				return GetPossibleAttributes();
			}
		}
		/// <summary>
		/// Returns the listing of attributes that have been set.
		/// </summary>
		public virtual IList ValidAttributes 
		{
			get
			{
				if (this.m_FriendlyName != "")
				{
					string[] attributes = {"name"};
					return attributes;
				}

				return new object[0];
			}
		}

		/// <summary>
		/// Extracts the value of an attribute. 
		/// Attribute list: name
		/// </summary>
		/// <param name="attribute">attribute name</param>
		/// <returns>returns a comparable value</returns>
		public virtual IComparable ExtractAttribute(string attribute)
		{
			if (attribute == T[_ATTRIB.name])
			{
				return this.m_FriendlyName;
			}

			return null;
		}

		/// <summary>
		/// Prints the XML representation of the media class.
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
		public virtual void ToXml (ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter)
		{
			ToXmlData.ToXml(this, formatter, (ToXmlData) data, xmlWriter);
		}

		/// <summary>
		/// Instructs the "xmlWriter" argument to start the "upnp:class" element.
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
		public virtual void StartElement(ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteStartElement(Tags.PropertyAttributes.upnp_class);
			ToXmlData _d = (ToXmlData) data;
			if (this.m_AttributeNames == null)
			{
				this.PrintFriendlyName(_d.DesiredProperties, xmlWriter);
			}
			else
			{
				foreach (string attribName in this.m_AttributeNames)
				{
					if (string.Compare(attribName, T[_ATTRIB.name]) == 0)
					{
						this.PrintFriendlyName(_d.DesiredProperties, xmlWriter);
					}
				}
			}
		}
		/// <summary>
		/// Instructs the "xmlWriter" argument to write the value of
		/// the "upnp:class" element.
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
		public virtual void WriteValue(ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteString(this.StringValue);
		}

		/// <summary>
		/// Instructs the "xmlWriter" argument to close the "upnp:class" element.
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
			xmlWriter.WriteEndElement();
		}

		/// <summary>
		/// Empty - "upnp:class" elements have no child XML elements.
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
		public virtual void WriteInnerXml(ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter)
		{
		}

		private void PrintFriendlyName(ArrayList desiredProperties, XmlTextWriter xmlWriter)
		{
			if (desiredProperties != null)
			{
				if ((desiredProperties.Count == 0) || (desiredProperties.Contains(Tags.PropertyAttributes.upnp_className)))
				{
					string val = this.FriendlyName;
					if ((val != null) && (val != ""))
					{
						xmlWriter.WriteAttributeString(T[_ATTRIB.name], this.FriendlyName);
					}
				}
			}
		}

		/// <summary>
		/// Compares the .StringValue value to the ToString() value of the argument.
		/// </summary>
		/// <param name="compareToThis"></param>
		/// <returns></returns>
		public virtual int CompareTo(object compareToThis)
		{
			return string.Compare(this.StringValue, compareToThis.ToString());
		}

		/// <summary>
		/// Returns true, if the item is derived from
		/// or is of the specified class.
		/// </summary>
		/// <param name="derivedFrom">the media class to which to compare to</param>
		/// <returns>true indicates an IS-A relationship with the specified media class</returns>
		public bool IsA (MediaClass derivedFrom)
		{
			return (this.ToString().StartsWith(derivedFrom.ToString()));
		}
		/// <summary>
		/// Returns true, if the item is derived from the
		/// "object.item" media class.
		/// </summary>
		public bool IsItem
		{
			get
			{
				string str = this.ToString();
				return (str.StartsWith("object.item"));
			}
		}
		/// <summary>
		/// Returns true, if the item is derived from the 
		/// "object.container" media class.
		/// </summary>
		public bool IsContainer
		{
			get
			{
				string str = this.ToString();
				return (str.StartsWith("object.container"));
			}
		}

		/// <summary>
		/// Returns the full name of the media class, in its string form.
		/// </summary>
		/// <returns>string representation of the media class.</returns>
		public override string ToString()
		{
			return this.StringValue;
		}
		
		/// <summary>
		/// The class name is the last substring after the
		/// last dot (.) in the class's fullname string representation.
		/// </summary>
		public string ClassName
		{
			get
			{
				return this.m_ClassName;
			}
		}

		/// <summary>
		/// The media class from which the
		/// item is derived from. Returns 
		/// <see cref="MediaClass.NullMediaClass"/>
		/// if there isn't a base class.
		/// </summary>
		public MediaClass BaseMediaClass
		{
			get
			{
				if (this.m_DerivedFrom == "")
				{
					return MediaClass.NullMediaClass;
				}
				else
				{
					return CdsMetadataCaches.MediaClasses.CacheThis(this.m_DerivedFrom, "");
				}
			}
		}

		/// <summary>
		/// Optional friendly name, for use in UI screens.
		/// </summary>
		public string FriendlyName
		{
			get
			{
				return this.m_FriendlyName;
			}
		}

		/// <summary>
		/// Creates a MediaClass object given the class name, the base class,
		/// and an optional friendly name.
		/// </summary>
		/// <param name="className">Specifies the class name</param>
		/// <param name="baseClass">specifies the base class</param>
		/// <param name="friendly">optional friendly name</param>
		public MediaClass (string className, string baseClass, string friendly)
		{
			if (baseClass == null)
			{
				throw new NullReferenceException("MediaClass ctor cannot have null baseClass argument.");
			}

			Init(className, baseClass, friendly);
		}

		/// <summary>
		/// Creates a MediaClass object with the full class name
		/// and an optional friendly name.
		/// </summary>
		/// <param name="fullName">full class name</param>
		/// <param name="friendly">optional friendly name</param>
		public MediaClass (string fullName, string friendly)
		{
			string classType = "";
			string baseClass = "";

			if (fullName == null)
			{
				throw new NullReferenceException("MediaClass ctor cannot have null fullName argument.");
			}

			int dotpos = fullName.LastIndexOf('.');

			if (dotpos > 0)
			{
				baseClass = fullName.Substring(0, dotpos);
				classType = fullName.Substring(dotpos+1);

				Init(classType, baseClass, friendly);
			}
			else
			{
				throw new Error_BadMediaClass(fullName);
			}
		}

		private void Init(string className, string baseClass, string friendly)
		{
			if (
				(baseClass.StartsWith("object.item")) ||
				(baseClass.StartsWith("object.container")) ||
				((baseClass == "object") && (className.StartsWith("item"))) ||
				((baseClass == "object") && (className.StartsWith("container")))
				)
			{
				if (className == null) { className = ""; }
				this.m_ClassName = (string) CdsMetadataCaches.CommonData.CacheThis(className);
				this.m_DerivedFrom = (string) CdsMetadataCaches.CommonData.CacheThis(baseClass);
				this.m_FriendlyName = (string) CdsMetadataCaches.CommonData.CacheThis(friendly);
			}
			else
			{
				throw new Error_BadMediaClass(baseClass+"."+className);
			}
		}

		/// <summary>
		/// Assume that element.Name maps to upnp:class
		/// </summary>
		/// <param name="element"></param>
		public MediaClass (XmlElement element)
		{
			ICollection attribs = element.Attributes;
			this.m_AttributeNames = new string[attribs.Count];
			string friendly = "";
			int i=0;
			foreach (XmlAttribute attrib in attribs)
			{
				if (string.Compare(attrib.Name, T[_ATTRIB.name]) == 0)
				{
					friendly = attrib.Value.ToString();
				}
				this.m_AttributeNames[i] = (string) CdsMetadataCaches.Didl.CacheThis(attrib.Name);
				i++;
			}

			string fullName = element.InnerText.Trim();
			this.SetValues(fullName, friendly);		
		}

		/// <summary>
		/// Assume the reader's current name is upnp:class
		/// </summary>
		/// <param name="reader"></param>
		public MediaClass(XmlReader reader)
		{
			string fullName = reader.Value;
			string friendly = "";
			if (reader.MoveToAttribute(T[_ATTRIB.name]))
			{
				friendly = reader.Value;
				reader.MoveToElement();
			}
			this.SetValues(fullName, friendly);
		}

		/// <summary>
		/// Simply sets the state variables that make up a media class.
		/// </summary>
		/// <param name="fullName"></param>
		/// <param name="friendly"></param>
		private void SetValues(string fullName, string friendly)
		{
			int dotpos = fullName.LastIndexOf('.');

			if (dotpos > 0)
			{
				this.m_ClassName = (string) CdsMetadataCaches.CommonData.CacheThis(fullName.Substring(dotpos+1));
				this.m_DerivedFrom = (string) CdsMetadataCaches.CommonData.CacheThis(fullName.Substring(0, dotpos));
				this.m_FriendlyName = (string) CdsMetadataCaches.CommonData.CacheThis(friendly);
			}
			else
			{
				this.m_ClassName = "object";
				this.m_DerivedFrom = "";
				this.m_FriendlyName = "Generic Object";
			}
		}


		/// <summary>
		/// null media class
		/// </summary>
		protected MediaClass()
		{
			this.m_ClassName = "";
			this.m_DerivedFrom = "";
			this.m_FriendlyName = "";
		}

		/// <summary>
		/// Programmers can use this value to compare against
		/// a non-null MediaClass that has not been initialized.
		/// </summary>
		public static MediaClass NullMediaClass = new MediaClass();

		/// <summary>
		/// the class name
		/// </summary>
		internal string m_ClassName;
		
		/// <summary>
		/// the base class
		/// </summary>
		internal string m_DerivedFrom;

		/// <summary>
		/// optional friendly name
		/// </summary>
		internal string m_FriendlyName;

		/// <summary>
		/// <para>
		/// Provided as a miscellaneous object that public programmers
		/// can use for whatever reason they want. The constructor
		/// may use this object, but it is safe to use after
		/// the constructor for the object (or derived object)
		/// is finished executing.
		/// </para>
		/// </summary>
		protected static Tags T = Tags.GetInstance();

		/// <summary>
		/// Tracks the names of attributes in the order they are listed
		/// in the XmlAttributeCollection when this object is
		/// instantiated from an XmlElement.
		/// </summary>
		protected string[] m_AttributeNames = null;


		public virtual void OnDeserialization(object sender)
		{
			if (this.m_AttributeNames != null)
			{
				for (int i=0; i < this.m_AttributeNames.Length; i++)
				{
					this.m_AttributeNames[i] = (string) CdsMetadataCaches.Didl.CacheThis((string)this.m_AttributeNames[i]);
				}
			}
			this.m_ClassName = (string) CdsMetadataCaches.CommonData.CacheThis(m_ClassName);
			this.m_DerivedFrom = (string) CdsMetadataCaches.CommonData.CacheThis(m_DerivedFrom);
			this.m_FriendlyName = (string) CdsMetadataCaches.CommonData.CacheThis(m_FriendlyName);
		}

	}

}
	
