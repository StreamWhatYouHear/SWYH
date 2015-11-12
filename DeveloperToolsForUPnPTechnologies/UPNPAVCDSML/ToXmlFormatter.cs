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

namespace OpenSource.UPnP.AV.CdsMetadata
{
	/// <summary>
	/// <para>
	/// This struct has delegates as fields, which can be used to hold
	/// references to method implementations that print DIDL-Lite XML
	/// representations of media objects (items and container), their associated resources,
	/// and their associated metadata. 
	/// </para>
	/// 
	/// <para>
	/// There are two techniques for using this structure (with possible
	/// variants between the two). 
	/// </para>
	/// 
	/// <para>
	/// The first is to have a class implement
	/// the <see cref="IToXmlData"/> and <see cref="ICdsElement"/> interfaces,
	/// and make the <see cref="ICdsElement.ToXml"/> implementation call
	/// the <see cref="ToXmlData.ToXml"/>. Such a technique results in 
	/// the class using its <see cref="IToXmlData"/> implementations for
	/// writing XML, yet allows for others to provide their own custom
	/// implementations of <see cref="IToXmlData"/> methods. The catch with
	/// this technique is that using a custom implementation methods 
	/// (eg, implementations that are not defined within the class)
	/// is really only efficient for metadata exposed through <see cref="IMediaProperties"/>.
	/// </para>
	/// 
	/// <para>
	/// The second technique attempts to provide custom XML formatting from
	/// a higher level. Specifically, the programmer provides method implementations
	/// for writing <see cref="IMediaContainer"/>, <see cref="IMediaItem"/>, and
	/// <see cref="IMediaResource"/> objects and then uses the formatter to
	/// call these methods when writing the DIDL-Lite. All of these resources also
	/// require a 'ToXml(ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter)'
	/// implementation as well. The most important requirement for the implementation
	/// of the class' ToXml() method is that it check the <see cref="ToXmlFormatter"/>'s
	/// fields to determine what it will do.
	/// </para>
	/// 
	/// <para>
	/// A combination of both techniques is applied for the implementations of
	/// classes within <see cref="UPNPAVCDSML"/>.
	/// </para>
	/// 
	/// <para>
	/// Classes derived from <see cref="MediaObject"/> 
	/// rely on the <see cref="MediaObject.ToXml"/> implementation to satisfy the <see cref="IUPnPMedia.ToXml"/>
	/// interface requirement. This implementation simply assigns appropriate 
	/// <see cref="ToXmlFormatter.Method"/> delegates to the <see cref="ToXmlFormatter"/>'s
	/// 'StartElement', 'WriteInnerXml', and 'EndElement' fields (the WriteValue is always null)
	/// based on the class' implementations that satisfy the <see cref="IToXmlData"/> interface.
	/// </para>
	/// 
	/// <para>
	/// Classes that implement <see cref="ICdsElement"/> follow a similar technique by implementing <see cref="IToXmlData"/>, 
	/// except that the implementations of <see cref="ICdsElement.ToXml"/>()
	/// all rely on the static <see cref="ToXmlData.ToXml"/>() method. This allows all 
	/// <see cref="ICdsElement"/> classes to leverage the common ability to include and exclude
	/// specific sections of XML (the header, footer, inner xml, and value) without having
	/// each implementation of <see cref="ICdsElement.ToXml"/> to duplicate that logic;
	/// a task that is essential when attempting to organize a content hierarchy in
	/// a nested fashion. The framework described thus far also works well for
	/// programmers that want to create a derived class to affect the way the
	/// media objects/resources/metadata are printed.
	/// </para>
	/// 
	/// <para>
	/// Unfortunately, providing a derived class isn't always ideal. Sometimes,
	/// a programmer doesn't want the behavior of the class to change - just
	/// wants the XML itself to be slightly different. To provide the high-level customizations 
	/// without having to derive a new class (and to avoid the practice of deep copying media
	/// objects and then changing values and then printing the XML), we've 
	/// set up the <see cref="MediaContainer.ToXml"/>, <see cref="MediaItem.ToXml"/>, and
	/// <see cref="MediaResource.ToXml"/>() are implemented to check whether the
	/// <see cref="ToXmlFormatter.WriteContainer"/>, <see cref="ToXmlFormatter.WriteItem"/>, 
	/// or <see cref="ToXmlFormatter.WriteResource"/> fields are assigned. If they are,
	/// the assigned delegate is executed. Otherwise, the implementation simply relies
	/// on the common <see cref="ToXmlFormatter.ToXml"/> or <see cref="ToXmlData.ToXml"/>
	/// method implementations to do the work.
	/// </para>
	/// </summary>
	public struct ToXmlFormatter
	{
		/// <summary>
		/// Defines the method signature needed to serialize a media container into an XML stream.
		/// </summary>
		public delegate void ToXml_FormatContainer (IMediaContainer container, ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter);
		/// <summary>
		/// Defines the method signature needed to serialize a media item into an XML stream.
		/// </summary>
		public delegate void ToXml_FormatItem (IMediaItem item, ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter);
		/// <summary>
		/// Defines the method signature needed to serialize a media object's resource into an XML stream.
		/// </summary>
		public delegate void ToXml_FormatResource (IMediaResource resource, ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter);

		/// <summary>
		/// Set this field to specify a custom-implementation for printing a container.
		/// </summary>
		public ToXml_FormatContainer WriteContainer;

		/// <summary>
		/// Set this field to specify a custom-implementation for printing an item.
		/// </summary>
		public ToXml_FormatItem WriteItem;
		
		/// <summary>
		/// Set this field to specify a custom-implementation for printing a resource.
		/// </summary>
		public ToXml_FormatResource WriteResource;

		/// <summary>
		/// A default formatter, where all fields are null.
		/// </summary>
		public static readonly ToXmlFormatter DefaultFormatter = new ToXmlFormatter();

		/// <summary>
		/// General delegate definition that can be used to print
		/// the XML representations of CDS metadata, including
		/// media objects (items and containers), resources,
		/// and <see cref="ICdsElement"/> metadata.
		/// 
		/// <para>
		/// The reason why we allow any type of of object is because
		/// the 'formatter' argument could point to method implementations
		/// specified by the programmer - which means that the programmer could
		/// use a 'data' object that is not a <see cref="ToXmlData"/> object.
		/// </para>
		/// 
		/// <para>
		/// In practice, most programmers will use an instance of <see cref="ToXmlData"/>
		/// or a derived class of the same name. This would allow the developer to 
		/// leverage existing code for serializing to DIDL-Lite XML.
		/// </para>
		/// </summary>
		public delegate void Method (ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter);

		/// <summary>
		/// <para>
		/// This field should not be assigned, although we require that it be public.
		/// Programmers should use 
		/// <see cref="WriteContainer"/>,
		/// <see cref="WriteItem"/>, and
		/// <see cref="WriteResource"/>
		/// to customize xml-writing of DIDL-Lite objects.
		/// </para>
		/// Used to write the declaration tag of an xml element,
		/// such as an item, container, or resource.
		/// This method does not write child elements.
		/// </summary>
		public Method StartElement;
		
		/// <summary>
		/// <para>
		/// This field should not be assigned, although we require that it be public.
		/// Programmers should use 
		/// <see cref="WriteContainer"/>,
		/// <see cref="WriteItem"/>, and
		/// <see cref="WriteResource"/>
		/// to customize xml-writing of DIDL-Lite objects.
		/// </para>
		/// Writes the inner-XML/child-elements (excluding the value) of the element started through
		/// <see cref="ToXmlFormatter.StartElement"/>.
		/// </summary>
		public Method WriteInnerXml;
		
		/// <summary>
		/// <para>
		/// This field should not be assigned, although we require that it be public.
		/// Programmers should use 
		/// <see cref="WriteContainer"/>,
		/// <see cref="WriteItem"/>, and
		/// <see cref="WriteResource"/>
		/// to customize xml-writing of DIDL-Lite objects.
		/// </para>
		/// Writes the value of the element declared in <see cref="ToXmlFormatter.StartElement"/>.
		/// </summary>
		public Method WriteValue;

		/// <summary>
		/// <para>
		/// This field should not be assigned, although we require that it be public.
		/// Programmers should use 
		/// <see cref="WriteContainer"/>,
		/// <see cref="WriteItem"/>, and
		/// <see cref="WriteResource"/>
		/// to customize xml-writing of DIDL-Lite objects.
		/// </para>
		/// Used to close the element declared in <see cref="ToXmlFormatter.StartElement"/>.
		/// </summary>
		public Method EndElement;

		/// <summary>
		/// A default implementation of the ToXml() method that
		/// simply calls each of the delegate-fields in the "formatter"
		/// argument.
		/// </summary>
		/// <param name="formatter">
		/// A <see cref="ToXmlFormatter"/> object that
		/// specifies method implementations for printing
		/// media objects and metadata.
		/// <para>
		/// If any of the <see cref="ToXmlFormatter.Method"/>
		/// fields are null, then the delegate is not
		/// executed. This feature can be used to print
		/// only the inner XML values.
		/// </para>
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
		public static void ToXml(ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter)
		{
			if (formatter.StartElement != null)
			{
				formatter.StartElement(formatter, data, xmlWriter);
			}

			if (formatter.WriteInnerXml != null)
			{
				formatter.WriteInnerXml(formatter, data, xmlWriter);
			}

			if (formatter.WriteValue != null)
			{
				formatter.WriteValue(formatter, data, xmlWriter);
			}

			if (formatter.EndElement != null)
			{
				formatter.EndElement(formatter, data, xmlWriter);
			}
		}
	}

}
