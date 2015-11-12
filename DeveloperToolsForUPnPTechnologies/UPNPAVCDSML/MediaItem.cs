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
using System.Collections;
using OpenSource.UPnP.AV;
using System.Runtime.Serialization;

namespace OpenSource.UPnP.AV.CdsMetadata
{
	/// <summary>
	/// <para>
	/// This class inherits all basic metadata of a ContentDirectory media entry,
	/// and extends it to support the UPNP-AV Forum defined concept of an "object.item" 
	/// media class and its derived subclasses.
	/// </para>
	/// <para>
	/// All public operations are thread-safe and all returned
	/// data is copy-safe or the data objects provide 
	/// read-only public interfaces.
	/// </para>	
	/// </summary>
	[Serializable()]
	public class MediaItem : MediaObject, IUPnPMedia, ICloneable, IMediaItem
	{
		/// <summary>
		/// Destructor - decrements item counter
		/// </summary>
		~MediaItem()
		{
			System.Threading.Interlocked.Decrement(ref ItemCounter);
			this.m_RefID = null;
		}
		/// <summary>
		/// Tracks the number of containers that have been created.
		/// </summary>
		private static long ItemCounter = 0;

		/// <summary>
		/// Special ISerializable constructor.
		/// Do basic initialization and then serialize from the info object.
		/// Serialized MediaContainer objects do not have their child objects
		/// serialized with them.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected MediaItem(SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base (info, context)
		{
			// Base class constructor calls Init() so all fields are
			// initialized. Simply serialize from info.
			System.Threading.Interlocked.Increment(ref ItemCounter);
			this.m_RefID = info.GetString("m_RefID");
		}

		/// <summary>
		/// Custom serializer - required for ISerializable.
		/// Serializes all fields that are not marked as [NonSerialized()].
		/// Some fields were originally marked as [NonSerialized()] because
		/// this class did not implement ISerializable. I've continued to
		/// use the attribute in the code.
		/// 
		/// If a MediaItem objects points to another media item (eg, reference item),
		/// the serialized MediaItem object will store the underlying media object's
		/// ID that provides the value for the "RefID" property.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public override void GetObjectData(SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("m_RefID", this.m_RefID);
		}

		/// <summary>
		/// Initializes the object.
		/// Calls base class implementation of Init()
		/// and then initailizes fields for this class.
		/// </summary>
		protected override void Init()
		{
			base.Init();
			this.m_RefID = "";
		}

		/// <summary>
		/// Create a completely empty item.
		/// </summary>
		public MediaItem() : base()
		{
			System.Threading.Interlocked.Increment(ref ItemCounter);
			this.SetClass("object.item", "");
		}

		/// <summary>
		/// Does a basic memberwise clone of the object,
		/// except that the cloned object's parent is null.
		/// </summary>
		/// <returns></returns>
		public virtual object Clone()
		{
			object RetVal = this.MemberwiseClone();
			((MediaItem)RetVal).m_Parent = null;
			return(RetVal);
		}

		/// <summary>
		/// <para>
		/// The constructor expects an XmlElement
		/// representing a DIDL-Lite "item" or "container"
		/// element. 
		/// </para>
		/// <para>
		/// Constructors of derived classes (that have the same signature)
		/// should call this base class constructor only to initialize things.
		/// Afterwards they should directly UpdateEverything(), which allows 
		/// the programmer to specify what type of
		/// resources, items, and containers to instantiate if such
		/// elements are encountered in the XML. The types specified
		/// for resources, items, and containers must all be 
		/// classes that can be instantiated from a single
		/// XmlElement. 
		/// </para>
		/// </summary>
		/// <param name="xmlElement">XmlElement representing a DIDL-Lite item element</param>
		public MediaItem(XmlElement xmlElement)
			: base(xmlElement)
		{
			System.Threading.Interlocked.Increment(ref ItemCounter);
		}

		/// <summary>
		/// Makes it so that a MediaItem instantiated from an XmlElement
		/// instantiates its child resources as <see cref="MediaResource"/> objects.
		/// 
		/// <para>
		/// Derived classes that expect different types for their resources and child
		/// media objects need to override this method.
		/// </para>
		/// </summary>
		/// <param name="xmlElement"></param>
		protected override void FinishInitFromXml(XmlElement xmlElement)
		{
			ArrayList children;
			base.UpdateEverything(true, false, typeof(MediaResource), typeof(MediaItem), typeof(MediaContainer), xmlElement, out children);
		}

		/// <summary>
		/// Returns true if the instance represents an item
		/// that points to another item in the content hierarchy.
		/// </summary>
		public override bool IsReference
		{
			get
			{
				bool isRef = (this.RefID != "");
				return isRef;
			}
		}

		/// <summary>
		/// Always false.
		/// Throws exception if set.
		/// </summary>
		public override bool IsSearchable
		{
			get
			{
				return false;
			}
			set
			{
				throw new ApplicationException("Items do not have this property so it cannot be set.");
			}
		}

		/// <summary>
		/// If IsReference is true, then the property returns
		/// the ID of the item that is referenced. Otherwise
		/// the property returns an empty string.
		/// 
		/// <para>
		/// The <see cref="OpenSource.UPnP.AV.CdsMetadata"/> namespace is designed so
		/// that CDS hierarchies can be represented without an implied
		/// programming context (eg, control-point or device-side context).
		/// As a result there are some instances where a programmer wants to 
		/// be able to represent that this item is a reference item simply by
		/// using the underlying item's object ID as the RefID. In other
		/// scenarios, a developer may want to use an actual <see cref="IMediaItem"/>
		/// object to represent the relationship. The implementation here
		/// allows for both.
		/// </para>
		/// 
		/// <para>
		/// The decision to use an <see cref="IMediaItem"/> or a <see cref="String"/>
		/// to represent the relationship is dependent on the developer's needs.
		/// </para>
		/// 
		/// </summary>
		/// <exception cref="ApplicationException">
		/// Thrown if attempting to set a RefID when RefItem != null.
		/// </exception>
		public virtual string RefID
		{
			get
			{
				IMediaItem refItem = this.RefItem;
				if (refItem == null)
				{
					return this.m_RefID;
				}
				else
				{
					return refItem.ID;
				}
			}
			set
			{
				IMediaItem refItem = this.RefItem;
				if (refItem == null)
				{
					this.m_RefID = value;
				}
				else
				{
					throw new ApplicationException("Cannot set RefID if RefItem != null.");
				}
			}
		}

		/// <summary>
		/// Get always returns null, unless override does otherwise.
		/// Set will always throw an exception, unless override is provided.
		/// </summary>
		/// <exception cref="ApplicationException">
		/// Thrown if set is called.
		/// </exception>
		public virtual IMediaItem RefItem
		{
			get
			{
				return null;
			}
			set
			{
				throw new ApplicationException("This implementation does not allow you to set the RefItem explicitly.");
			}
		}
		

		/// <summary>
		/// <para>
		/// Returns the title of the item. 
		/// </para>
		/// 
		/// <para>
		/// If IsReference is true, the programmer should be aware that
		/// a ContentDirectory may specify a different title for the
		/// referenced item. For example, the title may be "Shortcut to song.mp3"
		/// whilst the underlying/referred item's title may be "song.mp3".
		/// </para>
		/// </summary>
		public override string Title
		{
			get
			{
				return base.Title;
			}
			set
			{
				base.Title = value;
			}
		}
		
		/// <summary>
		/// <para>
		/// Returns the creator metadata as a string. If none
		/// is specified, the value is an empty string.
		/// </para>
		/// 
		/// <para>
		/// If IsReference is true, the programmer should be aware that
		/// a ContentDirectory may specify a different creator for the
		/// referenced item. This is probably unlikely, but it's always possible.
		/// </para>
		/// </summary>
		public override string Creator
		{
			get
			{
				return base.Creator;
			}
			set
			{
				base.Creator = value;
			}
		}

		/// <summary>
		/// <para>
		/// Returns the resources associated with the item. 
		/// </para>
		/// <para>
		/// If IsReference is true, the programmer should be aware that
		/// a ContentDirectory may specify additional resources for the
		/// item. For example the underlying item may have a an MP3 resource,
		/// whilst the referring item may specify a WMA resource. 
		/// The semantics for interpreting how an underlying item's resources
		/// relate to a referring item is undefined by ContentDirectory,
		/// and is left to the control point.
		/// </para>
		/// </summary>
		public override IMediaResource[] Resources
		{
			get
			{
				return base.Resources;
			}
		}

		/// <summary>
		/// If IsReference is true, then the resource lists for both
		/// the underlying item and the referring item are merged
		/// into a list and returned through this property.
		/// </summary>
		public override IMediaResource[] MergedResources
		{
			get
			{
				IUPnPMedia refItem = this.RefItem;
				if (refItem != null)
				{
					ArrayList resources = this.MergeResources(base.m_Resources, refItem.MergedResources);
					return (IMediaResource[]) resources.ToArray(typeof(IMediaResource));
				}
				else
				{
					return base.Resources;
				}
			}
		}

		/// <summary>
		/// <para>
		/// Returns the custom/vendor-specific metadata associated with the item. 
		/// </para>
		/// <para>
		/// If IsReference is true, the programmer should be aware that
		/// a ContentDirectory may specify additional vendor-specific metadata.
		/// </para>
		/// </summary>
		public override IList DescNodes
		{
			get
			{
				// we make an override just so to allow this implementation 
				// to provide the additional comments above on reference items.
				return base.DescNodes;
			}
		}

		/// <summary>
		/// If IsReference is true, the property returns a merged listing
		/// of all custom metadata fields of both the underlying and
		/// the referring item.
		/// </summary>
		public override IList MergedDescNodes
		{
			get
			{
				// obtain a shallow copy of this object's desc nodes
				ArrayList al = new ArrayList((ICollection) base.DescNodes);

				// if this item is a reference item (by virtue of having the refItem
				// field set), then we also add the desc nodes from the underlying
				// item.
				IMediaItem refItem = this.RefItem;

				while (refItem != null)
				{
					foreach (string str in refItem.DescNodes)
					{
						if (al.Contains(str) == false)  { al.Add(str); }
					}
					refItem = refItem.RefItem;
				}

				return al;
			}
		}

		/// <summary>
		/// <para>
		/// This property returns an object that stores all of the
		/// standard UPNP-AV defined metadata fields. 
		/// </para>
		/// 
		/// <para>
		/// If IsReference is true, the programmer should be aware that
		/// a ContentDirectory may specify additional metadata for the item. 
		/// </para>
		/// <para>
		/// Programmers that intend to make several calls on this property
		/// in a sequence should save a reference and make calls on the
		/// reference, instead of invoking it directly from this property
		/// each time.
		/// </para>
		/// </summary>
		public override IMediaProperties Properties
		{
			get
			{
				return base.Properties;
			}
		}

		/// <summary>
		/// If IsReference is true, then the standard metadata properties
		/// for both this media item and the underlying media item are
		/// merged together and returned in a shallow copy list.
		/// Programmers that intend to make several calls on this property
		/// in a sequence should save a reference and make calls on the
		/// saved reference, instead of invoking it directly from this property
		/// each time.
		/// </summary>
		public override IMediaProperties MergedProperties
		{
			get
			{
				IUPnPMedia refItem = this.RefItem;
				if (refItem != null)
				{
					IMediaProperties mergedProps = this.MergeProperties(base.Properties, refItem.MergedProperties);
					return mergedProps;
				}
				else
				{
					return base.Properties;
				}
			}
		}


		/// <summary>
		/// <para>
		/// This method merges the resources of a referring item and the resources
		/// of an underlying item.
		/// </para>
		/// </summary>
		/// <param name="baseRes">listing of resources for the this/referring item</param>
		/// <param name="refRes">listing of resources for the referred/underlying item</param>
		/// <returns></returns>
		private ArrayList MergeResources(IList baseRes, IList refRes)
		{
			ArrayList al = new ArrayList();

			if (baseRes != null)
			{
				if (baseRes.Count > 0)
				{
					al.AddRange(baseRes);
				}
			}

			if (refRes != null)
			{
				if (refRes.Count > 0)
				{
					al.AddRange(refRes);
				}
			}

			return al;
		}

		/// <summary>
		/// <para>
		/// This method merges the standard metadata properties of a referring item 
		/// and the metadat properties of an underlying item.
		/// </para>
		/// </summary>
		/// <param name="baseProps">metadata properties of this/referring item</param>
		/// <param name="refProps">metadata properties of the underlying/referred item</param>
		/// <returns></returns>
		private MediaProperties MergeProperties(IMediaProperties baseProps, IMediaProperties refProps)
		{
			MediaProperties mergedProps = new MediaProperties();

			ICollection basePropNames = baseProps.PropertyNames;
			ICollection refPropNames = refProps.PropertyNames;

			// The referring item's metadata is always appended to the
			// underlying item's metadata.
			// 
			foreach (string basePropName in basePropNames)
			{
				ArrayList al = this.GetMergedValues(basePropName, mergedProps, baseProps);
				mergedProps[basePropName] = al;
			}

			foreach (string refPropName in refPropNames)
			{
				ArrayList al = this.GetMergedValues(refPropName, mergedProps, refProps);
				mergedProps[refPropName] = al;
			}


			return mergedProps;
		}

		/// <summary>
		/// This method is used to take values of a specific metadata field from a MediaProperties object,
		/// and store the values in another MediaProperties object that is used for merging values
		/// from multiple MediaProperties objects.
		/// </summary>
		/// <param name="propName">Property name, usually acquired through Tags[CommonPropertyNames] enumeration.</param>
		/// <param name="mergedProps">The IMediaProperties object that will hold the merged values.</param>
		/// <param name="source">The IMediaProperties object that provides a set of values to copy from.</param>
		/// <returns></returns>
		private ArrayList GetMergedValues (string propName, IMediaProperties mergedProps, IMediaProperties source)
		{
			// contains a shallow copy of the results
			ArrayList results = new ArrayList();
			
			// get the values from the merged table
			IList list = (IList) mergedProps[propName];

			if (list != null)
			{
				if (list.Count > 0)
				{
					results.AddRange(list);
				}
			}

			bool allowsMultiple;
			PropertyMappings.PropertyNameToType(propName, out allowsMultiple);

			// get the values that we will be merging with
			list = (IList) source[propName];

			if (list != null)
			{
				if (list.Count > 0)
				{
					if (allowsMultiple == false)
					{
						results = new ArrayList();
						results.Add(list[0]);
					}
					else
					{
						results.AddRange(list);
					}
				}
			}

			return results;
		}

		/// <summary>
		/// The override is provided so that callers can specify their
		/// own implementations that will write the xml. 
		/// This is achieved by setting the <see cref="ToXmlFormatter.WriteItem"/>
		/// field of the 'formatter'. 
		/// </summary>
		/// <param name="formatter">
		/// Allows the caller to specify a custom implementation that writes
		/// the item's XML. Simply assign the <see cref="ToXmlFormatter.WriteItem"/>
		/// to have this method delegate the responsibility. Otherwise,
		/// the base class implementation of <see cref="MediaObject.ToXml"/>
		/// is called.
		/// </param>
		/// <param name="data">
		/// If the formatter's <see cref="ToXmlFormatter.WriteItem"/> field
		/// is non-null, then this object must be a type acceptable to that method's
		/// implementation. Otherwise, a <see cref="ToXmlData"/> object is required.
		/// </param>
		/// <param name="xmlWriter">
		/// This is where the xml gets printed.
		/// </param>
		public override void ToXml(ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter)
		{
			if (formatter.WriteItem != null)
			{
				formatter.StartElement = null;
				formatter.EndElement = null;
				formatter.WriteInnerXml = null;
				formatter.WriteValue  = null;
				formatter.WriteItem(this, formatter, data, xmlWriter);
			}
			else
			{
				base.ToXml(formatter, data, xmlWriter);
			}
		}

		/// <summary>
		/// <para>
		/// Programmers assign the <see cref="ToXmlFormatter.StartElement"/>
		/// field to this method, when attempting to print this media object.
		/// </para>
		/// <para>
		/// Algorithm:
		///	1. declare the item element
		///
		///	2. If this object will be added as a direct child
		///	of a container in a CreateObject request, then
		///	do not print the object's ID.
		///
		///	3. Print the ID of the item that this object points
		///	to if appropriate. If intending to request
		///	a MediaServer to create a child object that points 
		///	to another object then a control point should
		///	use the CreateReference, so in such a case
		///	this method will throw an exception.
		///
		///	4. Print the parent object, taking into account
		///	CreateObject instructions.
		///
		///	5. Print the restricted attribute.
		/// </para>
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
		/// 
		/// <para>
		/// For an explanation why this argument allows any object,
		/// please consult the documentation for the
		/// <see cref="ToXmlFormatter.Method"/> delegate type.
		/// </para>
		/// 
		/// </param>
		/// <param name="xmlWriter">
		/// The <see cref="XmlTextWriter"/> object that
		/// will format the representation in an XML
		/// valid way.
		/// </param>
		/// <exception cref="InvalidCastException">
		/// Thrown when the "data" argument is not a
		/// <see cref="ToXmlData"/> object.
		public override void StartElement (ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter)
		{
			ToXmlData _d = (ToXmlData) data;

			xmlWriter.WriteStartElement(T[_DIDL.Item]);

			if (_d.CreateObjectParentID != null)
			{
				xmlWriter.WriteAttributeString(T[_ATTRIB.id], "");
			}
			else
			{
				xmlWriter.WriteAttributeString(T[_ATTRIB.id], this.ID);
			}

			if (this.IsReference)
			{
				if (_d.CreateObjectParentID != null)
				{
					throw new Error_BadMetadata("Cannot print this object's XML for use with CreateObject because it is a reference item. Use the CreateReference action instead. Error found on ID='" +this.m_ID+"'");
				}
				xmlWriter.WriteAttributeString(T[_ATTRIB.refID], this.RefID);
			}

			if (_d.CreateObjectParentID != null)
			{
				xmlWriter.WriteAttributeString(T[_ATTRIB.parentID], _d.CreateObjectParentID);
			}
			else if (this.m_Parent != null)
			{
				xmlWriter.WriteAttributeString(T[_ATTRIB.parentID], this.m_Parent.ID);
			}
			else if (this.m_ParentID != null)
			{
				xmlWriter.WriteAttributeString(T[_ATTRIB.parentID], this.m_ParentID);
			}
			else if (_d.IgnoreBlankParentError)
			{
				xmlWriter.WriteAttributeString(T[_ATTRIB.parentID], "-1");
			}
			else
			{
				throw new Error_BadMetadata("Object must have a parent in order to be printed as DIDL-Lite. Error found on ID='" +this.m_ID+"'");
			}

			xmlWriter.WriteAttributeString(T[_ATTRIB.restricted], GetBoolAsOneZero(this.IsRestricted).ToString());
		}


		/// <summary>
		/// Closes the "item" element tag.
		/// </summary>
		/// <param name="xmlWriter"></param>
		protected void EndItemXml(XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteEndElement();
		}

		/// <summary>
		/// Tracks the ID of the item that this object points to, if needed.
		/// </summary>
		protected internal string m_RefID = "";

		protected enum EnumBoolsMediaItem
		{
			IgnoreLast = MediaObject.EnumBools.IgnoreLast
		}
	}
}
