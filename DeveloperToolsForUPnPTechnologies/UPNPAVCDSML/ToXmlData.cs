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
	/// Classes that can leverage the <see cref="ToXmlData.ToXml"/>
	/// method implementation implement this interface.
	/// </summary>
	public interface IToXmlData
	{
		/// <summary>
		/// Prints the starting element declaration for a piece
		/// of metadata.
		/// </summary>
		/// <param name="formatter">
		/// A <see cref="ToXmlFormatter"/> object that
		/// specifies method implementations for printing
		/// media objects and metadata.
		/// </param>
		/// <param name="data">
		/// Argument must be a <see cref="ToXmlData"/> object.
		/// </param>
		/// <param name="xmlWriter">
		/// <see cref="XmlTextWriter"/> object that will print the 
		/// reprsentation in an XML compliant way.
		/// </param>
		void StartElement (ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter); 
		
		/// <summary>
		/// Writes the inner xml element (excluding the value) 
		/// of the starting element.
		/// </summary>
		/// <param name="formatter">
		/// A <see cref="ToXmlFormatter"/> object that
		/// specifies method implementations for printing
		/// media objects and metadata.
		/// </param>
		/// <param name="data">
		/// Argument must be a <see cref="ToXmlData"/> object.
		/// </param>
		/// <param name="xmlWriter">
		/// <see cref="XmlTextWriter"/> object that will print the 
		/// reprsentation in an XML compliant way.
		/// </param>
		void WriteInnerXml (ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter); 
		
		/// <summary>
		/// Writes the value of the starting element.
		/// </summary>
		/// <param name="formatter">
		/// A <see cref="ToXmlFormatter"/> object that
		/// specifies method implementations for printing
		/// media objects and metadata.
		/// </param>
		/// <param name="data">
		/// Argument must be a <see cref="ToXmlData"/> object.
		/// </param>
		/// <param name="xmlWriter">
		/// <see cref="XmlTextWriter"/> object that will print the 
		/// reprsentation in an XML compliant way.
		/// </param>
		void WriteValue (ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter); 
		
		/// <summary>
		/// Closes the starting element.
		/// </summary>
		/// <param name="formatter">
		/// A <see cref="ToXmlFormatter"/> object that
		/// specifies method implementations for printing
		/// media objects and metadata.
		/// </param>
		/// <param name="data">
		/// Argument must be a <see cref="ToXmlData"/> object.
		/// </param>
		/// <param name="xmlWriter">
		/// <see cref="XmlTextWriter"/> object that will print the 
		/// reprsentation in an XML compliant way.
		/// </param>
		void EndElement (ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter); 
	}

	/// <summary>
	/// This is the data structure used when calling the 
	/// <see cref="MediaObject.ToXml"/> method.
	/// Default value is false.
	/// </summary>
	public class ToXmlData : ICloneable
	{
		/// <summary>
		/// Does a basic memberwise clone of the object.
		/// </summary>
		/// <returns></returns>
		public virtual object Clone()
		{
			object RetVal = this.MemberwiseClone();
			return RetVal;
		}

		/// <summary>
		/// Programmers can set this field if they intend to call 
		/// <see cref="MediaBuilder.BuildDidl"/> and desire that all
		/// branches be serialized as children of another virtual
		/// container. Because this virtual container requires access
		/// to the XML writing methods, it has to be a <see cref="MediaContainer"/>
		/// object (or a similarly derived object).
		/// </summary>
		public MediaContainer VirtualOwner = null;

		/// <summary>
		/// If true, then empty/null parent objects of will cause
		/// the parent of an item object to be listed as "-1". Otherwise,
		/// an empty parent (for n item) will cause an exception to be thrown.
		/// DIDL-Lite requires that a parent always be present,
		/// but improper programmer usage may cause the creation
		/// of a content hierarchy. Items should never have a parent of "-1".
		/// This method is provided only as a convenience
		/// to the programmer for odd scenarios. By default, all
		/// containers without a parent print "-1" to indicate themselves
		/// as a root container.
		/// </summary>
		public bool IgnoreBlankParentError = false;

		/// <summary>
		/// If true, then the child media objects of container
		/// objects are printed as child elements of the 
		/// container elements.
		/// Default value is false.
		/// </summary>
		public bool IsRecursive = false;

		/// <summary>
		/// If the value is non-null, then the object and its resources are
		/// printed in such a way that they are valid for
		/// the ContentDirectory's CreateObject method. The
		/// value of the field should match the ID of the
		/// intended parent container.
		/// Default value is null.
		/// </summary>
		public string CreateObjectParentID = null;

		/// <summary>
		/// <para>
		/// If this field is null, then only the required
		/// metadata is to be printed.
		/// </para>
		/// <para>
		/// If this field is empty, but non-null, then all
		/// metadata properties are to be printed.
		/// </para>
		/// <para>
		/// If this field is not empty then only the 
		/// specified metadata in the list is to be
		/// printed along with the required metadata.
		/// </para>
		/// Default value is null.
		/// </summary>
		public ArrayList DesiredProperties = null;

		/// <summary>
		/// <para>
		/// If this value is non-null, then the URI's of
		/// resources that use the <see cref="MediaResource.AUTOMAPFILE"/>
		/// convention will be automatically translated
		/// into valid relative URIs.
		/// </para>
		/// Default value is null.
		/// </summary>
		public string BaseUri = null;

		/// <summary>
		/// If true, then the <see cref="ToXmlFormatter.StartElement"/>
		/// and <see cref="ToXmlFormatter.EndElement"/> fields 
		/// must be assigned.
		/// Default value is true.
		/// </summary>
		public bool IncludeElementDeclaration = true;

		/// <summary>
		/// If true, then the <see cref="ToXmlFormatter.WriteInnerXml"/>
		/// field must be assigned.
		/// Default value is true.
		/// </summary>
		public bool IncludeInnerXml = true;

		/// <summary>
		/// If true, then the <see cref="ToXmlFormatter.WriteValue"/>
		/// field must be assigned.
		/// Default value is true.
		/// </summary>
		public bool IncludeValue = true;

		/// <summary>
		/// Used primarily by <see cref="ICdsElement.ToXml"/> implementations.
		/// <para>
		/// Implementation will assign some default implementaitons
		/// to the unassigned <see cref="ToXmlFormatter.Method"/> fields 
		/// of the "formatter" argument, unless information in
		/// the "data" argument specify otherwise.
		/// The assigned delegates are obtained from the "obj" argument's
		/// implementations of the <see cref="IToXmlData"/> methods.
		/// </para>
		/// <para>
		/// After assigning delegate implementations to the "formatter", the
		/// method calls <see cref="ToXmlFormatter.ToXml"/> to complete
		/// the standard implementation.
		/// </para>
		/// </summary>
		/// <param name="obj">This object provides the implementations assigned to the formatter argument.</param>
		/// <param name="formatter">
		/// A <see cref="ToXmlFormatter"/> object that
		/// specifies method implementations for printing
		/// media objects. Delegate fields may be reassigned here
		/// if they are null.
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
		public static void ToXml(IToXmlData obj, ToXmlFormatter formatter, ToXmlData data, XmlTextWriter xmlWriter)
		{
			ToXmlData _d = (ToXmlData) data;
			if ((formatter.StartElement == null) && (_d.IncludeElementDeclaration == true))
			{
				formatter.StartElement = new ToXmlFormatter.Method (obj.StartElement);
			}
			
			if ((formatter.WriteInnerXml == null) && (_d.IncludeInnerXml == true))
			{
				formatter.WriteInnerXml = new ToXmlFormatter.Method (obj.WriteInnerXml);
			}

			if ((formatter.WriteValue == null) && (_d.IncludeValue == true))
			{
				formatter.WriteValue = new ToXmlFormatter.Method (obj.WriteValue);
			}

			if ((formatter.EndElement == null) && (_d.IncludeElementDeclaration == true))
			{
				formatter.EndElement = new ToXmlFormatter.Method (obj.EndElement);
			}

			// Now call those implementations.
			ToXmlFormatter.ToXml(formatter, data, xmlWriter);
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ToXmlData(){}

		/// <summary>
		/// Instantiates object with custom recurse and desired properties options.
		/// </summary>
		/// <param name="isRecursive"></param>
		/// <param name="desiredProperties"></param>
		public ToXmlData(bool isRecursive, ArrayList desiredProperties)
		{
			this.IsRecursive = isRecursive;
			this.DesiredProperties = desiredProperties;
		}

		public ToXmlData(bool isRecursive, ArrayList desiredProperties, bool includeElementDeclaration)
		{
			this.IsRecursive = isRecursive;
			this.DesiredProperties = desiredProperties;
			this.IncludeElementDeclaration = includeElementDeclaration;
		}
	}
}
