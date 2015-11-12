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
	/// <para>
	/// Maps to the CDS/UPNP-AV "upnp:createClass" element, which 
	/// describes the types of media that can be returned in a 
	/// Search request from the container. Search class is always a per container,
	/// and no parent-child relationship is assumed.
	/// </para>
	/// 
	/// <para>
	/// This class maps to the "upnp:createClass" element in
	/// the ContentDirectory content hierarchy XML schema.
	/// </para>
	/// </summary>
	[Serializable()]
	public class CreateClass : MediaClass
	{
		/// <summary>
		/// Returns true if the other CreateClass has the same value and attribute values.
		/// </summary>
		/// <param name="cdsElement">a SearchClass instance; derived classes don't count</param>
		/// <returns></returns>
		public override bool Equals (object cdsElement)
		{
			CreateClass other = (CreateClass) cdsElement;

			if (this.GetType() == other.GetType())
			{
				if (this.m_ClassName.Equals(other.m_ClassName))
				{
					if (this.m_DerivedFrom.Equals(other.m_DerivedFrom))
					{
						if (this.m_FriendlyName.Equals(other.m_FriendlyName))
						{
							if (this.m_IncludeDerived.Equals(other.m_IncludeDerived))
							{
								return true;
							}
						}
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Generates a hashcode using the hashcodes of the fields multiplied by prime numbers,
		/// then summed together.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return
				base.GetHashCode() +
				(this.m_IncludeDerived.GetHashCode() * 11);
		}



		/// <summary>
		/// Returns a static listing of attributes that can apply to the
		/// xml form of this object.
		/// </summary>
		/// <returns></returns>
		public new static IList GetPossibleAttributes()
		{
			string[] attributes = {T[_ATTRIB.name], T[_ATTRIB.includeDerived]};
			return attributes;
		}

		/// <summary>
		/// Returns the listing of attributes that can apply to the
		/// xml form of this object.
		/// </summary>
		public override IList PossibleAttributes 
		{
			get
			{
				return GetPossibleAttributes();
			}
		}
		/// <summary>
		/// Returns the list of attributes that would be present if
		/// this object was cast to its xml form.
		/// </summary>
		public override IList ValidAttributes 
		{
			get
			{
				ArrayList al = new ArrayList(2);
				if (this.m_FriendlyName != "")
				{
					al.Add(T[_ATTRIB.name]);
				}

				al.Add(T[_ATTRIB.includeDerived]);

				return al;
			}
		}
		
		/// <summary>
		/// Instructs the "xmlWriter" argument to start the "upnp:createClass" element.
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
		/// Thrown when the "data" argument is not a <see cref="ToXmlData"/> object.
		/// </exception>
		public override void StartElement(ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter)
		{
			ToXmlData _d = (ToXmlData) data;
			xmlWriter.WriteStartElement(Tags.PropertyAttributes.upnp_createClass);

			if (this.m_AttributeNames == null)
			{
				this.PrintFriendlyName(_d.DesiredProperties, xmlWriter);
				this.PrintIncludeDerived(_d.DesiredProperties, xmlWriter);
			}
			else
			{
				foreach (string attribName in this.m_AttributeNames)
				{
					if (string.Compare(attribName, T[_ATTRIB.name]) == 0)
					{
						this.PrintFriendlyName(_d.DesiredProperties, xmlWriter);
					}
					else if (string.Compare(attribName, T[_ATTRIB.includeDerived]) == 0)
					{
						this.PrintIncludeDerived(_d.DesiredProperties, xmlWriter);
					}
				}
			}
		}

		private void PrintFriendlyName(ArrayList desiredProperties, XmlTextWriter xmlWriter)
		{
			if (desiredProperties != null)
			{
				if ((desiredProperties.Count == 0) || (desiredProperties.Contains(Tags.PropertyAttributes.upnp_createClassName)))
				{
					string val = this.FriendlyName;
					if ((val != null) && (val != ""))
					{
						xmlWriter.WriteAttributeString(T[_ATTRIB.name], this.FriendlyName);
					}
				}
			}
		}

		private void PrintIncludeDerived(ArrayList desiredProperties, XmlTextWriter xmlWriter)
		{
			if (desiredProperties != null)
			{
				if ((desiredProperties.Count == 0) || (desiredProperties.Contains(Tags.PropertyAttributes.upnp_createClassIncludeDerived)))
				{
					string val = this.IncludeDerived.ToString();
					val = val.ToLower();
					xmlWriter.WriteAttributeString(T[_ATTRIB.includeDerived], val);
				}
			}
		}

		/// <summary>
		/// Creates a search class given the class name,
		/// base class, optional friendly name, and an indication
		/// if objects derived from this specified media class 
		/// are included when searching for the specified media class. 
		/// </summary>
		/// <param name="className">class name (substring after the last dot "." character in full class name)</param>
		/// <param name="derivedFrom">base class (substring before the last dot "." character in full class name)</param>
		/// <param name="friendly">optional friendly name</param>
		/// <param name="includeDerived">indication if a search request on the container for a particular type includes all derived types</param>
		public CreateClass(string className, string derivedFrom, string friendly, bool includeDerived)
			: base (className, derivedFrom, friendly)
		{
			this.m_IncludeDerived = includeDerived;
		}
		
		/// <summary>
		/// Creates a search class given the full class name,
		/// optional friendly name, and an indication
		/// if objects derived from this specified media class 
		/// should be flagged 
		/// </summary>
		/// <param name="fullName">full class name, in "[baseClass].[class name]" format</param>
		/// <param name="friendly">optional friendly name</param>
		/// <param name="includeDerived">indication if a search request on the container for a particular type includes all derived types</param>
		public CreateClass (string fullName, string friendly, bool includeDerived)
			: base (fullName, friendly)
		{
			this.m_IncludeDerived = includeDerived;
		}

		/// <summary>
		/// The element.Name should be upnp:createClass
		/// </summary>
		/// <param name="element"></param>
		public CreateClass (XmlElement element)
			: base(element)
		{
			XmlAttribute attrib = element.Attributes[T[_ATTRIB.includeDerived]];
			this.m_IncludeDerived = MediaObject.IsAttributeValueTrue(attrib, false);
		}

		/// <summary>
		/// Extracts the value of an attribute. 
		/// Attribute list: name
		/// </summary>
		/// <param name="attribute">attribute name</param>
		/// <returns>returns a comparable value</returns>
		public override IComparable ExtractAttribute(string attribute)
		{
			if (attribute == T[_ATTRIB.name])
			{
				return this.m_FriendlyName;
			}
			else if (attribute == T[_ATTRIB.includeDerived])
			{
				return this.m_IncludeDerived;
			}

			return null;
		}

		/// <summary>
		/// Indicates that the specfied search class is applicable for all derived classes.
		/// </summary>
		public bool IncludeDerived
		{
			get
			{
				return this.m_IncludeDerived;
			}
		}

		/// <summary>
		/// The value for the IncludeDerived property. 
		/// The variable was at one point internally accessible.
		/// </summary>
		private readonly bool m_IncludeDerived;
	}

}
