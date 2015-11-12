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
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using OpenSource.UPnP.AV;

namespace OpenSource.UPnP.AV.CdsMetadata
{

	/// <summary>
	/// Generic interface for obtaining metadata common to both
	/// <see cref="MediaItem"/> and
	/// <see cref="MediaContainer"/>
	/// objects.
	/// <para>
	/// The design goal of this interface is not to allow representation
	/// and programmatic navigation of content hierarchies in memory.
	/// </para>
	/// <para>
	/// The primary purpose of this interface is not to enable the
	/// creation of content hierarchies using classes that implement
	/// this interface. Specifically an implementation of the
	/// AddResource() method for one class does not mean that every
	/// object that implements IMediaResource will be accepted into
	/// the content hierarchy. 
	/// </para>
	/// <para>
	/// The basic reason for this decision
	/// is that content hierarchies may behave differently 
	/// in different contexts. For example a content hierarchy
	/// advertised by the server may need to event changes
	/// regularly through UPnP, whereas a control point
	/// programming scenario wouldn't have such behavior.
	/// However this interface would allow consistent a
	/// programmer to apply consistent logic for creating
	/// content hierarchies, so long as the underlying types 
	/// are compatible with each other.
	/// </para>
	/// </summary>
	public interface IUPnPMedia
	{
		/// <summary>
		/// Allows the setting of any metadata for any property.
		/// Programmer specifies the property name and a listing of <see cref="ICdsElement"/> objects.
		/// </summary>
		/// <param name="propertyName">the tag name, including abbreviated namespace, of the desired UPnP-AV normative element</param>
		/// <param name="values">An <see cref="IList"/> of <see cref="ICdsElement"/> objects.</param>
		void SetPropertyValue (string propertyName, IList values);

		/// <summary>
		/// <para>
		/// Changes the metadata for this media object. Affected data includes data like Title, Creator, Class,
		/// desc nodes, and anything stored in Properties. Data is replaced, using values in 'info'. If 'info'
		/// object fields are null, then the media object will remove the metadata keyed by the approrpiate name.
		/// </para>
		/// 
		/// <para>
		/// Method does not affect resources, custom metadata, child media objects, or underlying media objects.
		/// </para>
		/// 
		/// <para>
		/// Programmers must take extra precaution with this method because 
		/// this method erases all of the values that are in the MediaProperties field of the object.
		/// </para>
		/// 
		/// <para>
		/// </para>
		/// </summary>
		/// <param name="info">
		/// Programmer must choose an appropriate class derived from <see cref="MediaBuilder.CoreMetadata"/>. 
		/// </param>
		void SetMetadata(MediaBuilder.CoreMetadata info);

		/// <summary>
		/// Adds a resource to the media object.
		/// </summary>
		/// <param name="addThis"></param>
		void AddResource(IMediaResource addThis);
		/// <summary>
		/// Adds multiple resources to the media object.
		/// </summary>
		/// <param name="newResources"></param>
		void AddResources(ICollection newResources);
		/// <summary>
		/// Removes a resource from the media object.
		/// </summary>
		/// <param name="removeThis"></param>
		void RemoveResource(IMediaResource removeThis);
		/// <summary>
		/// Removes multiple resources from the media object.
		/// </summary>
		/// <param name="removeThese"></param>
		void RemoveResources(ICollection removeThese);

		/// <summary>
		/// Adds a desc/custom-metadata element to the media object.
		/// The string argument is the complete 'desc' element, in its
		/// XML form.
		/// </summary>
		/// <param name="element"></param>
		void AddDescNode(string element);
		/// <summary>
		/// Adds multiple desc/custom-metadata elements to the media object.
		/// The string arguments must be complete 'desc' elements, in their
		/// proper XML form.
		/// </summary>
		/// <param name="element"></param>
		void AddDescNode(string[] elements);
		/// <summary>
		/// Removes a desc node from the media object.
		/// </summary>
		/// <param name="element"></param>
		void RemoveDescNode(string element);

		/// <summary>
		/// <para>
		/// Prints an XML representation of this media object.
		/// </para>
		/// <para>
		/// Although <see cref="MediaObject"/>-derived classes do not implement
		/// <see cref="ICdsElement"/>, the intent behind this method
		/// is very much like that of <see cref="ICdsElement.ToXml"/>.
		/// </para>
		/// </summary>
		/// <param name="formatter">
		/// Provides delegate references that provide the implementation
		/// for actually priting this media object.
		/// </param>
		/// <param name="data">
		/// Implementation-specific data and instructions that can be
		/// passed as an argument to the <see cref="ToXmlFormatter.Method"/>
		/// fields in theh "formatter" argument.
		/// </param>
		/// <param name="xmlWriter">
		/// The <see cref="XmlTextWriter"/> object that is responsible for
		/// printing the XML representation of the media object.
		/// </param>
		void ToXml (ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter);

		/// <summary>
		/// Updates the metadata for the media object using the metadata
		/// of another media object. All metadata, resources, and desc nodes are copied.
		/// This method will not change the parent-child relationships between containers and items. 
		/// </summary>
		/// <param name="newObj"></param>
		void UpdateObject (IUPnPMedia newObj);
		/// <summary>
		/// Updates the metadata for the media object using the metadata
		/// in an XML element. All metadata, resources, and desc nodes are applied.
		/// </summary>
		/// <param name="xmlElement"></param>
		void UpdateObject (XmlElement xmlElement);

		/// <summary>
		/// Allows direct modification of the properties and resources using a
		/// DIDL-Lite document that has a root DIDL-Lite node
		/// with one "item" or "container" element.
		/// </summary>
		/// <param name="DidlLiteXml"></param>
		void UpdateObject (string DidlLiteXml);

		/// <summary>
		/// Gets a near deep-copy of the object.
		/// The returned object won't necessarily
		/// be of the same class type, won't have
		/// the same object ID, or have the same Parent/ParentID,
		/// and any Tag fields won't be set, but the rest of 
		/// the metadata will be deep-copied.
		/// </summary>
		/// <returns></returns>
		IUPnPMedia MetadataCopy();
		
		/// <summary>
		/// The ID of the item. Guaranteed to be unique
		/// only within a fragment in the XML response
		/// for a ContentDirectory Browse/Search request. Containers
		/// are more likely to have a persistent
		/// ID, as ContentDirectory Browse/Search requests
		/// are specified by container.
		/// </summary>
		string		ID					{ get; set; }

		/// <summary>
		/// The title of the item. This is required
		/// and maps to the "dc:title" element
		/// in an XML response from ContentDirectory.
		/// </summary>
		string		Title				{ get; set; }

		/// <summary>
		/// The creator of the object. This is not
		/// required. It maps to the "dc:creator" element
		/// in an XML response from ContentDirectory.
		/// </summary>
		string		Creator				{ get; set; }

		/// <summary>
		/// Indicates the upnp:writeStatus of the resources
		/// associated with this object.
		/// </summary>
		EnumWriteStatus WriteStatus		{ get; set; }

		/// <summary>
		/// The parent container for this media entry.
		/// It is up to the implementer to ensure consistency
		/// between <see cref="IUPnPMedia.ParentID"/> and
		/// <see cref="IUPnPMedia.Parent"/>.
		/// Implementer may choose to limit set operations.
		/// </summary>
		IMediaContainer Parent			{ get; set; }

		/// <summary>
		/// The ID of the parent container, if it is known.
		/// It is up to the implementer to ensure consistency
		/// between <see cref="IUPnPMedia.ParentID"/> and
		/// <see cref="IUPnPMedia.Parent"/>.
		/// Implementer may choose to limit set operations.
		/// </summary>
		string		ParentID			{ get; set; }

		/// <summary>
		/// Indicates that the media's metadata 
		/// belongs to an "item" element.
		/// </summary>
		bool		IsItem				{ get; }
		/// <summary>
		/// Indicates that the media's metadata 
		/// belongs to an "container" element.
		/// </summary>
		bool		IsContainer			{ get; }
		/// <summary>
		/// Indicates that the entry can be modified
		/// by a control point.
		/// </summary>
		bool		IsRestricted		{ get; set; }

		/// <summary>
		/// Indicates the "item" entry is actually
		/// a reference to another item. In such
		/// scenarios, the item being referred has
		/// to have a somewhat persistent ID to be useful.
		/// Always false for a container.
		/// </summary>
		bool		IsReference			{ get; }

		/// <summary>
		/// Indicates if a container is searchable.
		/// Always false for an item.
		/// </summary>
		bool		IsSearchable { get; set; }

		/// <summary>
		/// All standard ContentDirectory specified
		/// metadata fields are retrieved from this
		/// hashtable-like structure.
		/// </summary>
		IMediaProperties Properties		{ get; }

		/// <summary>
		/// If an item is a reference to another item,
		/// this retrieves a merged set of metadata 
		/// values from both the underlying
		/// item. All metadata associated with
		/// the reference item (the item doing the referring)
		/// appends to or overwrites metadata values
		/// of the underlying item if a conflict is found.
		/// </summary>
		IMediaProperties MergedProperties	{ get; }

		/// <summary>
		/// The resources associated with a container
		/// or item are obtained from this property.
		/// </summary>
		IMediaResource[]		Resources			{ get; }

		/// <summary>
		/// If an item is a reference to another item,
		/// this retrieves a merged set of resources
		/// from both the underlying
		/// item. All metadata associated with
		/// the reference item (the item doing the referring)
		/// appends to or overwrites metadata values
		/// of the underlying item if a conflict is found.
		/// </summary>
		IMediaResource[]		MergedResources			{ get; }

		/// <summary>
		/// All custom metadata is stored as strings objects
		/// in a list. Each string object takes thes form
		/// of an XML fragment that properly represents 
		/// an XML node of custom metadata. The string
		/// must also follow allow CDS conventions
		/// for custom metadata, including the declaration
		/// of a namespace for the custom metadata.
		/// </summary>
		IList		DescNodes			{ get; }

		/// <summary>
		/// Returns a merged list of an underlying media item
		/// and this media object's desc DescNodes.
		/// </summary>
		IList		MergedDescNodes		{ get; }

		/// <summary>
		/// The XML element "upnp:class" is obtained through this
		/// property. ContentDirectory defines classes of 
		/// ContentDirectory entries.
		/// </summary>
		MediaClass	Class				{ get; set; }

		/// <summary>
		/// Used so that child objects can call owner and parent objects
		/// for permission to allow a caller to change metadata.
		/// Object that implements this method will get a stack trace
		/// and is expected to throw an exception if the caller information
		/// in the stack trace indicates a violation of its access rights.
		/// </summary>
		/// <param name="st"></param>
		void CheckRuntimeBindings(StackTrace st);

		/// <summary>
		/// Prints the DIDL-representation of a media object.
		/// </summary>
		/// <returns></returns>
		string ToDidl();

		/// <summary>
		/// Miscellaneous property for application use.
		/// </summary>
		object Tag { get; set; }
	}

	/// <summary>
	/// Simply allows a class that implements IUPnPMedia 
	/// class to be marked as a container.
	/// </summary>
	public interface IMediaContainer : IUPnPMedia
	{
		void AddObject(IUPnPMedia newObject, bool overWrite);
		void AddObjects(ICollection newObjects, bool overWrite);
		void RemoveObject (IUPnPMedia removeThis);
		void RemoveObjects (ICollection removeThese);

		IList CompleteList { get; }
		IList Items { get; }
		IList Containers { get; }

		/// <summary>
		/// Prints the DIDL-representation of a media object.
		/// </summary>
		/// <param name="IsRecursive">If true, then descendents are printed as embedded elements.</param>
		/// <returns></returns>
		string ToDidl(bool IsRecursive);

		int ChildCount { get; }
		IUPnPMedia GetDescendent (string id, Hashtable cache);
		bool IsRootContainer { get; }
	}

	/// <summary>
	/// Simply allows a class that implements IUPnPMedia 
	/// class to be marked as a item.
	/// </summary>
	public interface IMediaItem : IUPnPMedia
	{
		string RefID { get; set; }
		IMediaItem RefItem { get; set; }
	}
}
