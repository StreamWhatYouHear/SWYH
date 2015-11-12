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
using System.Threading;
using System.Diagnostics;
using System.Collections;
using OpenSource.UPnP.AV;
using OpenSource.Utilities;
using System.Runtime.Serialization;

namespace OpenSource.UPnP.AV.CdsMetadata
{
	/// <summary>
	/// <para>
	/// Base class for MediaContainer and MediaItem. This class
	/// contains the basic metadata required for any type of media
	/// in addition to the commonly used (but optional) "creator" metadata field.
	/// The class effectively maps to the "object" abstract base class in the CDS/UPNP-AV
	/// class hierarchy.
	/// </para>
	/// 
	/// <para>
	/// All public operations are thread-safe and all returned
	/// data is copy-safe or the data objects provide 
	/// read-only public interfaces.
	/// </para>
	/// 
	/// <para>
	/// The class has internal functions for setting metadata fields
	/// with strongly typed values. 
	/// </para>
	/// </summary>
	[Serializable()]
	public abstract class MediaObject : IUPnPMedia, ISerializable, IDeserializationCallback
	{
		/// <summary>
		/// Special ISerializable constructor.
		/// Do basic initialization and then serialize from the info object.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected MediaObject(SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			System.Threading.Interlocked.Increment(ref ObjectCounter);
			OpenSource.Utilities.InstanceTracker.Add(this);
			Init();
			this.m_bools = (BitArray) info.GetValue("m_bools", typeof(BitArray));
			this.m_Desc = (ArrayList) info.GetValue("m_Desc", typeof(ArrayList));
			this.m_ID = info.GetString("m_ID");
			this.m_Properties = (MediaProperties) info.GetValue("m_Properties", typeof(MediaProperties));
			this.m_Resources = (ArrayList) info.GetValue("m_Resources", typeof(ArrayList));
			this._Tag = info.GetValue("_Tag", typeof(object));
		}

		/// <summary>
		/// This method executes when all descendents for this object (in the object graph) are instantiated.
		/// </summary>
		/// <param name="sender"></param>
		public virtual void OnDeserialization(object sender)
		{
			//subscribe to the 
			this.m_Properties.OnMetadataChanged += new MediaProperties.Delegate_OnMetadataChanged(this.Sink_OnMediaPropertiesChanged);

			// ensure that media resource objects have their owner set
			if (this.m_Resources != null)
			{
				foreach (IMediaResource res in this.m_Resources)
				{
					res.Owner = this;
				}
			}

			// ensure that the media class & property enum writestatus instances
			// are cached instances to prevent duplicates
			MediaClass mc = this.Class;
			this.SetClass(mc.FullClassName, mc.FriendlyName);

			IList elements = this.Properties[CommonPropertyNames.writeStatus];
			if ((elements != null) && (elements.Count > 0) && (elements[0] != null))
			{
				PropertyEnumWriteStatus ws = (PropertyEnumWriteStatus) elements[0];
				this.SetWriteStatus(ws._Value);
			}
		}

		/// <summary>
		/// Custom serializer - required for ISerializable.
		/// Serializes all fields that are not marked as [NonSerialized()].
		/// Some fields were originally marked as [NonSerialized()] because
		/// this class did not implement ISerializable. I've continued to
		/// use the attribute in the code.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public virtual void GetObjectData(SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("m_bools", this.m_bools);
			info.AddValue("m_Desc", this.m_Desc);
			info.AddValue("m_ID", this.m_ID);
			info.AddValue("m_Properties", this.m_Properties);
			info.AddValue("m_Resources", this.m_Resources);
			info.AddValue("_Tag", this._Tag);
		}

		/// <summary>
		/// Prints the XML representation of this media object.
		/// <para>
		/// Implementation will assign some default implementaitons
		/// to the unassigned <see cref="ToXmlFormatter.Method"/> fields 
		/// of the "formatter" argument, unless information in
		/// the "data" argument specify otherwise.
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
		/// Thrown when the "data" argument is not a <see cref="ToXmlData"/> object.
		/// </exception>
		public virtual void ToXml(ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter)
		{
			formatter.StartElement = null;
			formatter.EndElement = null;
			formatter.WriteInnerXml = null;
			formatter.WriteValue  = null;

			ToXmlData _d = (ToXmlData) data;
			if (_d.IncludeElementDeclaration == true)
			{
				formatter.StartElement = new ToXmlFormatter.Method (this.StartElement);
			}
			
			if (_d.IncludeInnerXml == true)
			{
				formatter.WriteInnerXml = new ToXmlFormatter.Method (this.WriteInnerXml);
			}

			if (_d.IncludeElementDeclaration == true)
			{
				formatter.EndElement = new ToXmlFormatter.Method (this.EndElement);
			}

			// Now call those implementations.
			ToXmlFormatter.ToXml(formatter, data, xmlWriter);
		}

		/// <summary>
		/// Starts the xml element.
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
		public abstract void StartElement (ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter);

		/// <summary>
		/// Closes the element.
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
		public virtual void EndElement (ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteEndElement();
		}

		/// <summary>
		/// <para>
		/// Programmers assign the <see cref="ToXmlFormatter.WriteInnerXml"/>
		/// field to this method, when attempting to print this media object.
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
		/// by this implementation.
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
			InnerXmlWriter.WriteInnerXml
				(
				this, 
				new InnerXmlWriter.DelegateWriteProperties(InnerXmlWriter.WriteInnerXmlProperties),
				new InnerXmlWriter.DelegateShouldPrintResources(this.PrintResources),
				new InnerXmlWriter.DelegateWriteResources(InnerXmlWriter.WriteInnerXmlResources),
				new InnerXmlWriter.DelegateWriteDescNodes(InnerXmlWriter.WriteInnerXmlDescNodes),
				formatter,
				(ToXmlData) data,
				xmlWriter
				);
		}

		/// <summary>
		/// Does nothing - allows all to modify metadata.
		/// </summary>
		/// <param name="st"></param>
		public virtual void CheckRuntimeBindings (StackTrace st)
		{
		}

		/// <summary>
		/// Returns a deep copy of the object's metadata in a new MediaObject.
		/// The class returned instance will be either
		/// <see cref="MediaItem"/> or <see cref="MediaContainer"/>.
		/// <para>
		/// The deep-copy object will not have the same ID,
		/// nor will the .Tag field be set.
		/// </para>
		/// </summary>
		/// <returns></returns>
		public IUPnPMedia MetadataCopy()
		{
			StringBuilder sbXml = null;
			sbXml = new StringBuilder(XML_BUFFER_SIZE);
			StringWriter sw = new StringWriter(sbXml);
			XmlTextWriter xmlWriter = new XmlTextWriter(sw);
			xmlWriter.Namespaces = true;

			MediaObject.WriteResponseHeader(xmlWriter);

			// set up the ToXml() method to write everything
			ToXmlData _d = (ToXmlData) MediaObject.ToXmlData_Default.Clone();
			_d.IgnoreBlankParentError = true;
			this.ToXml(ToXmlFormatter.DefaultFormatter, _d, xmlWriter);
			MediaObject.WriteResponseFooter(xmlWriter);
			xmlWriter.Flush();
			string xmlstr = sbXml.ToString();
			xmlWriter.Close();

			MediaContainer mc = this as MediaContainer;
			MediaItem mi = this as MediaItem;
			MediaObject newObj = null;

			XmlDocument xmldoc = new XmlDocument();
			xmldoc.LoadXml(xmlstr);
			XmlElement child = (XmlElement) xmldoc.ChildNodes[0].ChildNodes[0];

			if (mc != null)
			{
				newObj = new MediaContainer(child);
			}
			else if (mi != null)
			{
				newObj = new MediaItem(child);
			}
			else
			{
				throw new Exception("Bad evil. Should always have an item or container.");
			}
			newObj.m_ID = MediaBuilder.GetUniqueId();

			return newObj;
		}

		/// <summary>
		/// Finalizer
		/// </summary>
		~MediaObject()
		{
			System.Threading.Interlocked.Decrement(ref ObjectCounter);
			OpenSource.Utilities.InstanceTracker.Remove(this);
			this.m_Resources = null;
			this.m_Properties = null;
		}

		/// <summary>
		/// Tracks the number of objects that have been created.
		/// </summary>
		private static long ObjectCounter = 0;
		
		/// <summary>
		/// Enumerates through m_bools
		/// </summary>
		protected enum EnumBools
		{
			/// <summary>
			/// Maps to the "restricted" attribute of an "item" or "container" element.
			/// <para>
			/// Derived classes will have access to this and so will internal classes.
			/// Programmers are strongly encouraged to not mess with this value unless
			/// they ABSOLUTELY know what they're doing.
			/// </para>
			Restricted = 0,

			/// <summary>
			/// Ignored this one...
			/// </summary>
			IgnoreLast
		}
		protected BitArray m_bools = null;

		/// <summary>
		/// <para>
		/// WARNING: You must manually set the required values:
		/// (m_ID, m_Title, m_Creator, m_Class).
		/// </para>
		/// 
		/// <para>
		/// Public programmers generally should not need to refer
		/// to the underlying MediaObject interface. The 
		/// <see cref="IUPnPMedia"/> interface
		/// is much more robust and suitable for handling the
		/// extraction of information for both items and containers.
		/// </para>
		/// 
		/// <para>
		/// Constructor parameters are not provided because 
		/// of tasks involved when instantiating a MediaObject
		/// from an XmlElement. It is easier in the current
		/// framework to populate a blank MediaObject than it is
		/// to instantiate one with the correct information.
		/// </para>
		/// </summary>
		public MediaObject()
		{
			System.Threading.Interlocked.Increment(ref ObjectCounter);
			OpenSource.Utilities.InstanceTracker.Add(this);
			Init();
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
		/// <param name="xmlElement">XML representing the item or container</param>
		internal MediaObject(XmlElement xmlElement)
		{
			System.Threading.Interlocked.Increment(ref ObjectCounter);
			OpenSource.Utilities.InstanceTracker.Add(this);
			Init();
			FinishInitFromXml(xmlElement);
		}

		/// <summary>
		/// Initializes the object.
		/// </summary>
		protected virtual void Init()
		{
			this.m_bools = new BitArray(8, false);
			this.m_Desc = null;
			this.m_ID = "";
			//this.m_LockDesc = new ReaderWriterLock();

			if (MediaObject.UseStaticLock)
			{
				this.m_LockResources = MediaObject.StaticLock;
			}
			else
			{
				this.m_LockResources = new ReaderWriterLock();
			}

			this.m_Parent = null;
			this.m_ParentID = null;
			this.ResetProperties();
			this.ResetResources();
			this._Tag = null;
			this.m_PropertiesStateNumber = int.MaxValue;
		}

		/// <summary>
		/// Every derived class needs to implement this method. 
		/// The purpose of the method is to call <see cref="UpdateEverything"/>
		/// in such a way that the appropriate types are specified
		/// for instantiating media resources and child media objects.
		/// </summary>
		/// <param name="xmlElement"></param>
		/// <param name="children"></param>
		protected abstract void FinishInitFromXml(XmlElement xmlElement);

		/// <summary>
		/// Updates the metadata obtained retrievable
		/// from the Properties property. Such metadata is 
		/// limited to metadata in the CommonPropertyNames attributes
		/// and does not include 
		/// associated children, resources, ID, searchable/restricted attributes
		/// of the object.
		/// </summary>
		/// <param name="child">
		/// XmlElement that's a child of the item or container element.
		/// The element's name must map to something in CommonPropertyNames,
		/// as defined by the <see cref="PropertyMappings.PropertyNameToType"/>
		/// method.
		/// </param>
		protected void UpdateProperty (XmlElement child)
		{
			bool isMultiple;
			// figure out what .NET type the data should
			Type type = PropertyMappings.PropertyNameToType(child.Name, out isMultiple);

			if (type != null)
			{
				try
				{
					Type[] constructorArgTypes = new Type[1];
					constructorArgTypes[0] = typeof(XmlElement);
				
					ConstructorInfo ci = type.GetConstructor(constructorArgTypes);

					ICdsElement metadata = null;
					if (ci != null)
					{
						object[] obj = new object[1];
						obj[0] = child;
						metadata = (ICdsElement) ci.Invoke(obj);
					}

					// ensure proper case-sensitive spelling of property

					CommonPropertyNames prop = (CommonPropertyNames) Enum.Parse(typeof(CommonPropertyNames), child.LocalName, true);
					string propName = T[prop];

					if (propName == T[_UPNP.Class])
					{
						MediaClass mc = (MediaClass) metadata;
						metadata = (ICdsElement) CdsMetadataCaches.MediaClasses.CacheThis(mc.FullClassName, mc.FriendlyName);
					}
					else if (propName == T[_UPNP.writeStatus])
					{
						PropertyEnumWriteStatus ws = (PropertyEnumWriteStatus) metadata;
						metadata = (ICdsElement) CdsMetadataCaches.PropertyEnumWriteStatus.CacheThis(T[_UPNP.writeStatus], ws._Value);
					}
					else if (propName == T[_UPNP.storageMedium])
					{
						PropertyStorageMedium sm = (PropertyStorageMedium) metadata;
						metadata = (ICdsElement) CdsMetadataCaches.PropertyStorageMediums.CacheThis(T[_UPNP.storageMedium], sm._Value);
					}

					if (isMultiple)
					{
						this.m_Properties.AddVal(propName, metadata);
					}
					else
					{
						//this.m_Properties.SetVal(propName, metadata);
						ICdsElement[] elements = new ICdsElement[1];
						elements[0] = metadata;
						this.m_Properties[propName] = elements;
					}
				}
				catch (Exception e)
				{
					// if we failed to instantiate a property, just keep going
					// with the next one and don't worry about other people's mistakes.
					// just do the best you can with what they give you.
					OpenSource.Utilities.EventLogger.Log(e);
				}
			}
		}

		public static bool ShouldPrintResources(ArrayList desiredProperties)
		{
			if (desiredProperties != null)
			{
				if (desiredProperties.Count == 0)
				{
					return true;
				}
				else
				{
					foreach (string key in desiredProperties)
					{
						string lowerKey = key.ToLower();
						if (lowerKey.StartsWith(T[_DIDL.Res]))
						{
							return true;
						}
						else
						{
							// check for @[attribname] notation.
							foreach (string attrib in MediaResource.GetPossibleAttributes())
							{
								string lowerAttrib = "@" + attrib.ToLower();

								if (lowerAttrib == lowerKey)
								{
									return true;
								}
							}
						}
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Returns true if the desiredProperties array
		/// indicates the need to print resource elements.
		/// </summary>
		/// <param name="desiredProperties"></param>
		/// <returns></returns>
		protected bool PrintResources(ArrayList desiredProperties)
		{
			return MediaObject.ShouldPrintResources(desiredProperties);
		}

		/// <summary>
		/// Provides a static readonly formatting option for printing CDS metadata - strongly
		/// recommended that the values on the formatter are not modified.
		/// </summary>
		//public static readonly ToXmlFormatter ToXmlFormatter_Default = new ToXmlFormatter();

		/// <summary>
		/// Provides a static readonly data option for print CDS metadata 
		/// such that all metadata is printed but no child media objects are recursed
		/// - strongly recommended that the values on the formatter are not modified.
		/// </summary>
		public static readonly ToXmlData ToXmlData_Default = new ToXmlData(false, new ArrayList(0));

		/// <summary>
		/// Provides a static readonly data option for printing CDS metadata 
		/// such that all metadata is printed and all child media objects are recursed.
		/// - strongly recommended that the values on the formatter are not modified.
		/// </summary>
		public static readonly ToXmlData ToXmlData_AllRecurse = new ToXmlData(true, new ArrayList(0));

		/// <summary>
		/// Provides a static readonly data option for printing CDS metadata 
		/// such that all inner text metadata is printed but no child media objects are recursed.
		/// - strongly recommended that the values on the formatter are not modified.
		/// </summary>
		public static readonly ToXmlData ToXmlData_AllInnerTextOnly = new ToXmlData(true, new ArrayList(0), false);

		/// <summary>
		/// Media objects are items or containers, and items
		/// can be references.
		/// </summary>
		public abstract bool IsReference { get; }
		public abstract bool IsSearchable { get; set; }

		/// <summary>
		/// Programmers should set this value to false if the UTF16 encoding needs
		/// to be used.
		/// </summary>
		public static bool ENCODE_UTF8 = false;
		/// <summary>
		/// Programmers can set this value to another value if they want to increase
		/// the default buffer allocated for DIDL-Lite XML strings.
		/// </summary>
		public static int XML_BUFFER_SIZE = 400;

		/// <summary>
		/// This method will properly create a DIDL-Lite representation for
		/// the object.
		/// </summary>
		/// <returns></returns>
		public virtual string ToDidl()
		{
			return this.ToDidl(false);
		}

		/// <summary>
		/// This method will properly create a DIDL-Lite representation for
		/// the object or the content subtree that begins with this object. 
		/// </summary>
		/// <param name="isRecursive">
		/// indicates whether just the object's children should be included
		/// in the string.
		/// </param>
		/// <returns></returns>
		public virtual string ToDidl(bool isRecursive)
		{
			ArrayList properties = new ArrayList();

			StringBuilder sb = null;
			StringWriter sw = null;
			MemoryStream ms = null;
			XmlTextWriter xmlWriter = null;
			
			if (ENCODE_UTF8)
			{
				ms = new MemoryStream(XML_BUFFER_SIZE);
				xmlWriter = new XmlTextWriter(ms, System.Text.Encoding.UTF8);
			}
			else
			{
				sb = new StringBuilder(XML_BUFFER_SIZE);
				sw = new StringWriter(sb);
				xmlWriter = new XmlTextWriter(sw);
			}
			
			xmlWriter.Formatting = System.Xml.Formatting.Indented;
			xmlWriter.Namespaces = true;
			xmlWriter.WriteStartDocument();
			
			WriteResponseHeader(xmlWriter);

			// set up the ToXml() method to write everything recursively
			ToXmlData _d = (ToXmlData) MediaObject.ToXmlData_AllRecurse.Clone();
			_d.IsRecursive = isRecursive;
			this.ToXml(ToXmlFormatter.DefaultFormatter, _d, xmlWriter);
			//this.ToXml(isRecursive, properties, xmlWriter);
			
			xmlWriter.WriteEndDocument();
			xmlWriter.Flush();
			
			string xmlResult;
			if (ENCODE_UTF8)
			{
				int len = (int) ms.ToArray().Length - MediaObject.TruncateLength_UTF8;
				UTF8Encoding utf8e = new UTF8Encoding(false, true);
				xmlResult = utf8e.GetString(ms.ToArray(), MediaObject.TruncateLength_UTF8, len);
			}
			else
			{
				xmlResult = sb.ToString();
			}
			xmlWriter.Close();

			int crpos = xmlResult.IndexOf("\r\n");
			crpos = xmlResult.IndexOf('<', crpos);
			string trunc = xmlResult.Remove(0, crpos);
			return trunc;
		}

		/// <summary>
		/// Specifies the number of bytes to truncate from the stream
		/// if doing a UTF8 encoding. This is required because garbage
		/// characters are appended in the various .NET string operations.
		/// </summary>
		public const int TruncateLength_UTF8 = 3;

		/// <summary>
		/// Writes the DIDL-Lite header.
		/// </summary>
		/// <param name="xmlWriter"></param>
		public static void WriteResponseHeader(XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteStartElement(T[_DIDL.DIDL_Lite]);
			
			//ROOT - ATTRIBUTES
			xmlWriter.WriteAttributeString(Tags.XMLNS, Tags.XMLNSDIDL_VALUE);
			xmlWriter.WriteAttributeString(Tags.XMLNS_DC, Tags.XMLNSDC_VALUE);
			xmlWriter.WriteAttributeString(Tags.XMLNS_UPNP, Tags.XMLNSUPNP_VALUE);
		}
		
		/// <summary>
		/// Closes the DIDL-Lite header
		/// </summary>
		/// <param name="xmlWriter"></param>
		public static void WriteResponseFooter(XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteEndElement();
		}


		/// <summary>
		/// Stores value for Tag property.
		/// </summary>
		private object _Tag;

		/// <summary>
		/// <para>
		/// Provided as a miscellaneous object that public programmers
		/// can use for whatever reason they want. The constructor
		/// may use this object, but it is safe to use after
		/// the constructor for the object (or derived object)
		/// is finished executing.
		/// </para>
		/// </summary>
		public object Tag
		{
			get
			{
				return _Tag;
			}
			set
			{
				_Tag = value;
			}
		}

		/// <summary>
		/// Required metadata property.
		/// Containers will have an ID that persists. 
		/// Items are not guaranteed to have an ID that persists.
		/// ID values are strings, so they are not always numerical.
		/// </summary>
		public virtual string ID		
		{ 
			get 
			{ 
				return this.m_ID; 
			} 
			set 
			{ 
				if ((value == null) || (value == ""))
				{
					throw new Error_BadMetadata("Cannot have empty or null object ID");
				}
				this.m_ID = value;
			} 
		}
		
		/// <summary>
		/// Required metadata property.
		/// Every object has a title. This maps to the
		/// "dc:title" element of an entry in a ContentDirectory.
		/// 
		/// <para>
		/// If using the get operation, then a null title will
		/// automatically be translated into an empty string. This
		/// field is provided as a courtesy so that developers don't
		/// need to call .Properties["dc:title"] and check for zero-length
		/// lists or null title values.
		/// </para>
		/// </summary>
		/// <exception cref="Error_BadMetadata">
		/// Thrown during set operation if the value is null or empty.
		/// </exception>
		public virtual string Title		
		{
			get
			{
				this.UpdateCache();
				return this.m_CacheTitle;
			}
			set
			{
				ThrowExceptionIfBadTitle(value);
				this.SetPropertyValue_String(T[_DC.title], value);
			}
		}

		/// <summary>
		/// Used to validate a title. 
		/// </summary>
		/// <param name="title"></param>
		/// <exception cref="Error_BadMetadata">
		/// Thrown if the title is invalid.
		/// </exception>
		protected void ThrowExceptionIfBadTitle(string title)
		{
			if (title == null)
			{
				throw new Error_BadMetadata("Title value must not be null. Error found on ID='" +this.m_ID+"'");
			}
			if (title == "")
			{
				throw new Error_BadMetadata("Title value must not be empty. Error found on ID='" +this.m_ID+"'");
			}
		}

		/// <summary>
		/// Returns true if the object is a container.
		/// </summary>
		public bool IsContainer
		{
			get
			{
                if (this.Class == null)
                {
                    if (this.GetType().ToString().Equals("OpenSource.UPnP.AV.MediaServer.CP.CpMediaContainer")) return true;    // This is a really bad hack to fix this problem.
                    return false;
                }
				return this.Class.IsContainer;
			}
		}
		
		/// <summary>
		/// Returns true if the object is an item.
		/// </summary>
		public bool IsItem
		{
			get
			{
                if (this.Class == null)
                {
                    if (this.GetType().ToString().Equals("OpenSource.UPnP.AV.MediaServer.CP.CpMediaContainer")) return false;    // This is a really bad hack to fix this problem.
                    return true;
                }
				return this.Class.IsItem;
			}
		}

		/// <summary>
		/// Optional metadata property.
		/// Creator maps to dc:creator.
		/// <para>
		/// If using the get operation, then a null creator will
		/// automatically be translated into an empty string. This
		/// field is provided as a courtesy so that developers don't
		/// need to call .Properties["dc:creator"] and check for zero-length
		/// lists or null creator values.
		/// </para>
		/// </summary>
		public virtual string Creator	
		{
			get
			{
				this.UpdateCache();
				return this.m_CacheCreator;
			}
			set
			{
				this.SetPropertyValue_String(T[_DC.creator], value);
			}
		}

		/// <summary>
		/// Returns the same values from Resources.
		/// </summary>
		public virtual IMediaResource[] MergedResources { get { return this.Resources; } }

		/// <summary>
		/// Optional metadata property.
		/// Returns a listing of IMediaResource objects that
		/// represent binaries associated with the metadata
		/// of this instance.
		/// </summary>
		public virtual IMediaResource[] Resources	
		{
			get 
			{
				this.m_LockResources.AcquireReaderLock(-1);
				IMediaResource[] copy;
				if (this.m_Resources != null)
				{
					copy = new IMediaResource[this.m_Resources.Count];
					for (int i=0; i < this.m_Resources.Count; i++)
					{
						copy[i] = (IMediaResource) this.m_Resources[i];
					}
				}
				else
				{
					copy = new IMediaResource[0];
				}
				this.m_LockResources.ReleaseReaderLock();
				return copy;
			} 
		}
		/// <summary>
		/// Required metadata property.
		/// Returns the class of the object, describing if
		/// the object is an item, playlist, storage folder, etc.
		/// Maps to the "upnp:class" element.
		/// 
		/// <para>
		/// This field is provided as a courtesy so that developers don't
		/// need to call .Properties["upnp:class"] and check for zero-length
		/// lists. In the worst case, the returned class will be null.
		/// </para>
		/// </summary>
		public virtual MediaClass Class
		{
			get 
			{
				this.UpdateCache();
				return this.m_CacheClass;
			}
			set
			{
				ThrowExceptionIfBadClass(value);
				this.SetClass(value.FullClassName, value.FriendlyName);
			}
		}

		/// <summary>
		/// Used to validate a media class. 
		/// </summary>
		/// <param name="mclass"></param>
		/// <exception cref="Error_BadMetadata">
		/// Thrown if the media class is null/invalid.
		/// </exception>
		protected void ThrowExceptionIfBadClass(MediaClass mclass)
		{
			if ((mclass == null))
			{
				throw new Error_BadMetadata("Must specify a media class. Error found on ID='" +this.m_ID+"'");
			}
		}

		/// <summary>
		/// Required metadata property. 
		/// Indicates whether the object can be modified.
		/// If the object is a container, it also
		/// indicates if child objects can be created.
		/// </summary>
		public virtual bool IsRestricted 
		{ 
			get 
			{ 
				return this.m_Restricted; 
			} 
			set
			{
				this.m_Restricted = value;
			}
		}

		/// <summary>
		/// Optional metadata property.
		/// Indicates if the associated resources can be overwritten.
		/// </summary>
		public virtual EnumWriteStatus WriteStatus 
		{
			get 
			{
				IList list = (IList) this.m_Properties[T[_UPNP.writeStatus]];

				EnumWriteStatus retVal = EnumWriteStatus.UNKNOWN;
				
				if (list != null)
				{
					if (list.Count > 0)
					{
						retVal = (EnumWriteStatus) ((PropertyEnumWriteStatus) list[0]).Value;
					}
				}

				return retVal;
			}
			set
			{
				this.SetWriteStatus(value);
			}
		}

		/// <summary>
		/// Returns the same values in Properties.
		/// </summary>
		public virtual IMediaProperties MergedProperties { get { return this.Properties; } }

		/// <summary>
		/// This property holds all metadata values for any standard ContentDirectory
		/// metadata field defined by the UPNP-AV Forum. Custom metadata elements
		/// are not stored in this structure.
		/// </summary>
		public virtual IMediaProperties Properties { get { return this.m_Properties; } }

		/// <summary>
		/// This listing holds all custom/vendor-specified metadata as a listing
		/// of string objects that represent an XML node.
		/// </summary>
		public virtual IList DescNodes 
		{
			get 
			{
				//this.m_LockDesc.AcquireReaderLock(-1);
				this.m_Properties.RWLock.AcquireReaderLock(-1);
				string[] descnodes ;
				if (this.m_Desc != null)
				{
					descnodes = new string[this.m_Desc.Count];
					for (int i=0; i < this.m_Desc.Count; i++)
					{
						descnodes[i] = (string) this.m_Desc[i];
					}
				}
				else
				{
					descnodes = new string[0];
				}
				//this.m_LockDesc.ReleaseReaderLock();
				this.m_Properties.RWLock.ReleaseReaderLock();
				return descnodes;
			} 
		}

		/// <summary>
		/// Returns same values as this.Resources.
		/// </summary>
		public virtual IList MergedDescNodes
		{
			get
			{
				return this.DescNodes;
			}
		}

		/// <summary>
		/// Returns the MediaContainer that owns the item as direct child item.
		/// A MediaItem can only have one parent.
		/// </summary>
		/// <exception cref="InvalidCastException">
		/// Thrown if the set(value) object is not an instance
		/// of <see cref="MediaContainer"/>.
		/// </exception>
		public virtual IMediaContainer Parent 
		{ 
			get 
			{
				return this._Parent; 
			} 
			set
			{
				this._Parent = (MediaContainer) value;
			}
		}

		/// <summary>
		/// If the instance has a parent, then the value returned is the
		/// ID of the parent container. Otherwise, the property returns
		/// an empty string if an item, or a "-1" if a container.
		/// </summary>
		/// <exception cref="ApplicationException">
		/// Thrown if the set-method on the property is called.
		/// </exception>
		public virtual string ParentID
		{
			get
			{
				if (this._Parent == null)
				{
					if (this.m_ParentID != null)
					{
						return this.m_ParentID;
					}

					if (this.IsItem)
					{
						return MediaObject.NoParentId;
					}
					
					return MediaObject.NoParentIdContainer;
				}
				else
				{
					return this._Parent.ID;
				}
			}
			set
			{
				throw new ApplicationException("This implementation does not allow application to set ParentID property. Please use Parent property directly.");
			}
		}

		/// <summary>
		/// If a media item has no parent, it should report not report a parent ID of -1.
		/// This is semantically incorrect, but then again... a media item
		/// without a parent cannot exist in a proper CDS hierarchy.
		/// </summary>
		public const string NoParentId = "-1";

		/// <summary>
		/// If a container has no parent, assume it is a root container with parentID -1.
		/// </summary>
		public const string NoParentIdContainer = "-1";
		
		/// <summary>
		/// Completely clears the media object of its
		/// reources and properties.
		/// </summary>
		private void ResetProperties()
		{
			if (this.m_Properties != null)
			{
				this.m_Properties.ClearProperties();
			}
			else
			{
				this.m_Properties = new MediaProperties();
				this.m_Properties.OnMetadataChanged += new MediaProperties.Delegate_OnMetadataChanged(this.Sink_OnMediaPropertiesChanged);
			}

			try
			{
				//this.m_LockDesc.AcquireWriterLock(-1);
				this.m_Properties.RWLock.AcquireWriterLock(-1);
				this.m_Desc = null;
			}
			finally
			{
				//this.m_LockDesc.ReleaseWriterLock();
				this.m_Properties.RWLock.ReleaseWriterLock();
			}
		}

		/// <summary>
		/// Clears the list of resources for this object.
		/// </summary>
		private void ResetResources()
		{
			try
			{
				this.m_LockResources.AcquireWriterLock(-1);
				if (this.m_Resources != null)
				{
					foreach (IMediaResource res in this.m_Resources)
					{
						res.Owner = null;
					}
				}
				this.m_Resources = null;
			}
			finally
			{
				this.m_LockResources.ReleaseWriterLock();
			}
		}



		/// <summary>
		/// <para>
		/// Updates the resources and metadata of this object
		/// to match another object. Allowed changes can apply
		/// to the following metadata fields:
		/// <list type="bullet">
		///		<item><description>ID</description></item>
		///		<item><description>IsRestricted</description></item>
		///		<item><description>Properties</description></item>
		///		<item><description>Resources</description></item>
		///		<item><description>DescNodes</description></item>
		/// </list>
		/// </para>
		/// 
		/// <para>
		/// This method will not change the parent-child relationships between containers and items. 
		/// </para>
		/// </summary>
		/// <param name="newObj">the MediaObject instance with the new values that should be applied</param>
		/// <exception cref="OpenSource.UPnP.AV.CdsMetadata.Error_BaseClassMisMatch">
		/// Thrown when when one of the objects is a container and the other is an item.
		/// </exception>
		/// <exception cref="ApplicationException">
		/// Thrown if something bad happened.
		/// </exception>
		public virtual void UpdateObject (IUPnPMedia newObj)
		{
			// If the base classes are inconsistent, throw an exception.
			// 
			if ((this.Class.IsContainer) && (newObj.Class.IsContainer == false))
			{
				throw new Error_BaseClassMisMatch(this, newObj);
			}
			else if ((this.Class.IsItem) && (newObj.Class.IsItem == false))
			{
				throw new Error_BaseClassMisMatch(this, newObj);
			}

			// Always lock the resources before locking desc nodes
			try
			{
				this.m_LockResources.AcquireWriterLock(-1);
				//this.m_LockDesc.AcquireWriterLock(-1);
			}
			finally
			{
				this.m_Properties.RWLock.AcquireWriterLock(-1);
			}

			this.m_Desc = new ArrayList((ICollection)newObj.DescNodes);
			this.m_ID = newObj.ID;

			// copy the metadata from the new obj and apply this one
			this.m_Properties.ClearProperties();

			IMediaProperties properties = newObj.Properties;
			foreach (string propName in properties.PropertyNames)
			{
				this.m_Properties[propName] = new ArrayList( (ICollection)properties[propName] );
			}

			// Copy the metadata from the new resources and apply to existing
			// resources. Add resources as needed.
			int iThis = 0;
			int iNew = 0;
			IList newResources = newObj.Resources;
			IList oldResources = this.m_Resources;
			if ((oldResources == null) || (oldResources.Count == 0))
			{
				oldResources = new ArrayList(newResources.Count);
			}
			int oldCount = oldResources.Count;
			int iMax = Math.Max(oldResources.Count, newResources.Count);

			if (this.m_Resources == null)
			{
				m_Resources = new ArrayList(0);
			}

			for (int i = 0; i < iMax; i++)
			{
				IMediaResource oldR;
				IMediaResource newR;

				if ((i < oldCount) && (i < newResources.Count))
				{
					// simply transfer metadata from one resource to another
					newR = (IMediaResource) newResources[iNew];
					oldR = (IMediaResource) oldResources[iThis];
					oldR.UpdateResource(newR);
					iThis++;
					iNew++;
				}
				else if ((i < oldCount) && (i >= newResources.Count))
				{
					// we have an old resource and no new resource to replace it
					// so remove the old resource
					oldR = (IMediaResource) oldResources[iThis];
					oldR.Owner = null;
					this.m_Resources.RemoveAt(iThis);
				}
				else if ((i >= oldCount) && (i < newResources.Count))
				{
					// we don't have an old resource to remove but we have a new
					// one to add
					newR = (IMediaResource) newResources[iNew];
					oldR = (IMediaResource) newR.Clone();
					oldR.Owner = this;
					this.m_Resources.Add(oldR);
				}
				else
				{
					throw new ApplicationException("Bad evil. Updateobject() had an error.");
				}
			}

			this.m_Restricted = newObj.IsRestricted;

			//this.m_LockDesc.ReleaseWriterLock();
			this.m_Properties.RWLock.ReleaseWriterLock();
			this.m_LockResources.ReleaseWriterLock();
		}

		/// <summary>
		/// Allows direct modification of the properties using an XmlElement
		/// that represents the object.
		/// Programmers should note that the first thing that happens
		/// is that the properties are cleared.
		/// <para>
		/// Derived classes that override this method should call
		/// <see cref="MediaObject.UpdateEverything"/>
		/// to cause the changes. This will ensure that the correct
		/// types of objects are instantiated.
		/// A sample implementation is shown below.
		/// </para>
		/// <code>
		/// ArrayList proposedChildren;
		/// this.UpdateEverything(false, false, typeof(MediaResource), typeof(MediaItem), typeof(MediaContainer), xmlElement, out proposedChildren);
		/// </code>
		/// </summary>
		/// <param name="xmlElement"></param>
		public virtual void UpdateMetadata (XmlElement xmlElement)
		{
			ArrayList proposedChildren;
			this.UpdateEverything(false, false, typeof(MediaResource), typeof(MediaItem), typeof(MediaContainer), xmlElement, out proposedChildren);
		}

		/// <summary>
		/// Allows direct modification of the properties using a
		/// DIDL-Lite document that has a root DIDL-Lite node
		/// with one "item" or "container" element.
		/// that represents the object.
		/// Programmers should note that the first thing that happens
		/// is that the properties are cleared.
		/// </summary>
		/// <param name="DidlLiteXml"></param>
		public virtual void UpdateMetadata (string DidlLiteXml)
		{
			XmlDocument xml = DidlLiteXmlToXmlDOM(DidlLiteXml);
			this.UpdateMetadata( (XmlElement) (xml.ChildNodes[0].ChildNodes[0]) );
		}

		/// <summary>
		/// Allows direct modification of the properties and resources using an XmlElement
		/// that represents the object. Child resources are expected to be child nodes
		/// of the element.
		/// Programmers should note that the first thing that happens
		/// is that the properties and resources are cleared.
		/// <para>
		/// Derived classes that override this method should call
		/// <see cref="MediaObject.UpdateEverything"/>
		/// to cause the changes. This will ensure that the correct
		/// types of objects are instantiated for the child resources.
		/// A sample implementation is shown below.
		/// </para>
		/// <code>
		/// ArrayList proposedChildren;
		/// this.UpdateEverything(false, false, typeof(MediaResource), typeof(MediaItem), typeof(MediaContainer), xmlElement, out proposedChildren);
		/// </code>
		/// </summary>
		/// <param name="xmlElement"></param>
		public virtual void UpdateObject (XmlElement xmlElement)
		{
			ArrayList proposedChildren;
			this.UpdateEverything(true, false, typeof(MediaResource), typeof(MediaItem), typeof(MediaContainer), xmlElement, out proposedChildren);
		}

		/// <summary>
		/// Allows direct modification of the properties and resources using a
		/// DIDL-Lite document that has a root DIDL-Lite node
		/// with one "item" or "container" element.
		/// 
		/// Child resources are expected to be child nodes
		/// of the element.
		/// Programmers should note that the first thing that happens
		/// is that the properties and resources are cleared.
		/// </summary>
		/// <param name="DidlLiteXml"></param>
		public virtual void UpdateObject (string DidlLiteXml)
		{
			XmlDocument xml = DidlLiteXmlToXmlDOM(DidlLiteXml);
			this.UpdateObject( (XmlElement) (xml.ChildNodes[0].ChildNodes[0]) );
		}

		/// <summary>
		/// Casts a DIDL-Lite xml document in string form
		/// into an actual xml dom object.
		/// </summary>
		/// <param name="DidlLiteXml"></param>
		/// <returns></returns>
		/// <exception cref="Error_BadMetadata">
		/// Thrown when the string is not well-formed XML.
		/// </exception>
		public static XmlDocument DidlLiteXmlToXmlDOM (string DidlLiteXml)
		{
			XmlDocument xml = new XmlDocument();
			try
			{
				xml.LoadXml(DidlLiteXml);
			}
			catch (Exception e)
			{
				throw new Error_BadMetadata("DIDL-Lite document is not XML compliant. (" + e.Message + ")", e);
			}
			return xml;
		}

		/// <summary>
		/// Helper function for <see cref="UpdateEverything"/>.
		/// Resets the metadata for the ojbect.
		/// </summary>
		/// <param name="updateResources">If true, resources should be reset.</param>
		private void UpdateEverything_Reset(bool updateResources)
		{
			this.ResetProperties();
			
			if (updateResources)
			{
				this.ResetResources();
			}
		}

		/// <summary>
		/// Helper function for <see cref="UpdateEverything"/>.
		/// Checks the xml element for a proper name and
		/// returns flag values indicating if it's an item or container.
		/// </summary>
		/// <param name="xmlElement"></param>
		/// <param name="isItem"></param>
		/// <param name="isContainer"></param>
		/// <exception cref="Error_BadMetadata">
		/// Thrown if the xml element is not an item or container.
		/// </exception>
		private void UpdateEverything_CheckElementName(XmlElement xmlElement, out bool isItem, out bool isContainer)
		{
			isItem = false;
			isContainer = false;

			if (xmlElement.Name == T[_DIDL.Item])
			{
				isItem = true;
			}
			else if (xmlElement.Name == T[_DIDL.Container])
			{
				isContainer = true;
			}
			else
			{
				throw new Error_BadMetadata("Expecting item or container element in MediaObject constructor.");
			}
		}

		/// <summary>
		/// Helper function for <see cref="UpdateEverything"/>.
		/// Sets the media object's metadata based on the attributes
		/// of the xml element.
		/// </summary>
		/// <param name="xmlElement"></param>
		/// <param name="isItem"></param>
		/// <param name="isContainer"></param>
		private void UpdateEverything_SetObjectAttributes(XmlElement xmlElement, bool isItem, bool isContainer)
		{
			bool setID = false;

			foreach (XmlAttribute attrib in xmlElement.Attributes)
			{
				if (String.Compare(attrib.LocalName, T[_ATTRIB.restricted], true) == 0)
				{
					this.m_Restricted = IsAttributeValueTrue(attrib, true);
				}
				else if ((String.Compare(attrib.LocalName, T[_ATTRIB.id], true) == 0))
				{
					if (attrib.Value == "")
					{
						this.m_ID = MediaBuilder.GetUniqueId();
					}
					else
					{
						this.m_ID = attrib.Value;
					}
					setID = true;
				}
				else if ((String.Compare(attrib.LocalName, T[_ATTRIB.parentID], true) == 0))
				{
					if (attrib.Value != "")
					{
						this.m_ParentID = attrib.Value;
					}
				}
				else if (isItem)
				{
					if (String.Compare(attrib.LocalName, T[_ATTRIB.refID], true) == 0)
					{
						IMediaItem mi = (IMediaItem) this;
						mi.RefID = attrib.Value;
					}
				}
				else if (isContainer)
				{
					IMediaContainer mc = (IMediaContainer) this;
					if (String.Compare(attrib.LocalName, T[_ATTRIB.searchable], true) == 0)
					{
						mc.IsSearchable = IsAttributeValueTrue(attrib, true);
					}
				}
			}

			if (setID == false)
			{
				this.m_ID = MediaBuilder.GetUniqueId();
			}
		}

		/// <summary>
		/// Helper function for <see cref="UpdateEverything"/>.
		/// Parses the child nodes of the specified XML element and sets the media object
		/// to match the metadata stored in the XML element. If specified, resources
		/// of the media object will also get updated. 
		/// 
		/// <para>
		/// The method will not update child media objects of a container because child
		/// objects are not "metadata" of this media object... however, the method can store
		/// a list of child media objects in an allocated <see cref="ArrayList"/> if
		/// instructed to do so.
		/// </para>
		/// </summary>
		/// <param name="xmlElement">
		/// The xml element representing the media object with its child elements, including resources and child media objects.
		/// </param>
		/// <param name="isItem">true, if the media object is an item</param>
		/// <param name="isContainer">true, if the media object is a container</param>
		/// <param name="updateResources">true, if we are to add resources found in the xml element</param>
		/// <param name="proposedResources">store found resources here</param>
		/// <param name="updateChildren">true, if we are to add child media objects found in the xml element</param>
		/// <param name="proposedChildren">store found child media objects here</param>
		/// <param name="instantiateTheseForResources">
		/// If we encounter a resource element in the XML and we want to instantiate an object to represent it,
		/// instantiate the type specified by this argument.
		/// </param>
		/// <param name="instantiateTheseForChildItems">
		/// If we encounter a media item element in the XML and we want to instantiate an object to represent it,
		/// instantiate the type specified by this argument.
		/// </param>
		/// <param name="instantiateTheseForChildContainers">
		/// If we encounter a media container element in the XML and we want to instantiate an object to represent it,
		/// instantiate the type specified by this argument.
		/// </param>
		private void UpdateEverything_SetChildNodes
			(
			XmlElement xmlElement, 
			bool isItem, 
			bool isContainer, 
			bool updateResources,
			ArrayList proposedResources,
			bool updateChildren, 
			ArrayList proposedChildren, 
			System.Type instantiateTheseForResources,
			System.Type instantiateTheseForChildItems,
			System.Type instantiateTheseForChildContainers
			)
		{
			// Iterate through the child nodes (eg, metadata)

			foreach (XmlNode childNode in xmlElement.ChildNodes)
			{
				XmlElement child = childNode as XmlElement;
				
				if (child != null)
				{

					bool goAhead = true;

					if (
						(isItem) &&
						(
						(string.Compare (child.Name, T[CommonPropertyNames.createClass], true) == 0) ||
						(string.Compare (child.Name, T[CommonPropertyNames.searchClass], true) == 0)
						)
						)
					{
						goAhead = false;
					}
					else if (string.Compare(child.Name, T[_DIDL.Desc], true) == 0)
					{
						// push desc elements for now and do nothing with them
						this.AddDescNode(child.OuterXml);
						goAhead = false;
					}
					else if (string.Compare(child.Name, T[_DIDL.Res], true) == 0)
					{
						if (updateResources)
						{
							IMediaResource newRes;

							Type[] argTypes = new Type[1];
							argTypes[0] = typeof(XmlElement);
							ConstructorInfo ci = instantiateTheseForResources.GetConstructor(argTypes);
						
							object[] args = new object[1];
							args[0] = child;

							newRes = (IMediaResource) ci.Invoke(args);

							proposedResources.Add(newRes);
						}
						goAhead = false;
					}
					else if (isContainer)
					{
						if (string.Compare(child.Name, T[_DIDL.Item], true) == 0)
						{
							if (updateChildren)
							{
								IMediaItem newObj = null;

								Type[] argTypes = new Type[1];
								argTypes[0] = typeof(XmlElement);
								ConstructorInfo ci = instantiateTheseForChildItems.GetConstructor(argTypes);
						
								object[] args = new object[1];
								args[0] = child;

								newObj = (IMediaItem) ci.Invoke(args);

								proposedChildren.Add(newObj);
							}
							goAhead = false;
						}
						else if (string.Compare(child.Name, T[_DIDL.Container], true) == 0)
						{
							if (updateChildren)
							{
								IMediaContainer newObj;

								Type[] argTypes = new Type[1];
								argTypes[0] = typeof(XmlElement);
								ConstructorInfo ci = instantiateTheseForChildContainers.GetConstructor(argTypes);
						
								object[] args = new object[1];
								args[0] = child;

								newObj = (IMediaContainer) ci.Invoke(args);

								proposedChildren.Add(newObj);
							}
							goAhead = false;
						}
					}

					if (goAhead)
					{
						this.UpdateProperty(child);
					}
				}
			}

		}

		/// <summary>
		/// Ensures that the media class for the element matches the base media class for the object.
		/// </summary>
		/// <param name="xmlElement"></param>
		/// <param name="isItem"></param>
		/// <param name="isContainer"></param>
		/// <exception cref="Error_BadMetadata">
		/// Thrown if the base upnp:class is not valid for the specified element.
		/// </exception>
		private void UpdateEverything_CheckBaseMediaClass(XmlElement xmlElement, bool isItem, bool isContainer)
		{
			if (isContainer)
			{
				if (xmlElement.Name == T[_DIDL.Container])
				{
				}
				else
				{
					throw new Error_BadMetadata("Found \""+xmlElement.Name+"\" in XML when expecting \"item\" or \"container\". Error found on ID='" +this.m_ID+"'");
				}

				if (this.Class.IsItem)
				{
					throw new Error_BadMetadata("Created container with upnp:class as item type. Error found on ID='" +this.m_ID+"'");
				}
			}

			if (isItem)
			{
				if (xmlElement.Name == T[_DIDL.Item])
				{
				}
				else
				{
					throw new Error_BadMetadata("Found \""+xmlElement.Name+"\" in XML when expecting \"item\" or \"container\". Error found on ID='" +this.m_ID+"'");
				}
				if (this.Class.IsContainer)
				{
					throw new Error_BadMetadata("Created item with upnp:class as container type. Error found on ID='" +this.m_ID+"'");
				}
			}
		}

		/// <summary>
		/// <para>
		/// Constructors of derived classes can call this method when instantiating
		/// an object from an XML. After the constructor calls Init()
		/// the constructor needs to specify the type of the
		/// resources, items, and containers to instantiate if such
		/// elements are encountered in the XML. The types specified
		/// for resources, items, and containers must all be 
		/// classes that can be instantiated from a single
		/// XmlElement. This method will introspect the class and
		/// call the appropriate constructor.
		/// </para>
		/// <para>
		/// A constructor of a derived class that calls this method should
		/// ONLY call this method if it can guarantee that the System.Type
		/// for the instance matches the instance of the constructor.
		/// This is to allow classes derived from other MediaObject-derived
		/// classes to call the method themselves and to specify their own
		/// types for resources, items, and children as desired.
		/// </para>
		/// </summary>
		/// <param name="updateResources">true, if associated resources should be set</param>
		/// <param name="updateChildren">true, if child items and containers should be instantiated</param>
		/// <param name="instantiateTheseForResources">the type to instantiate for a resource, must be derived from <see cref="MediaResource"/></param>
		/// <param name="instantiateTheseForChildItems">the type to instantiate for an item, must be derived from <see cref="MediaItem"/></param>
		/// <param name="instantiateTheseForChildContainers">the type to instantiate for a container, must be derived from <see cref="MediaContainer"/></param>
		/// <param name="xmlElement">the XML that represents this element and all child elements of a content hierarchy</param>
		/// <param name="proposedChildren">a collection of items and containers that were instantiated, child containers may have their own set of children</param>
		protected void UpdateEverything 
			(
			bool updateResources, 
			bool updateChildren, 
			System.Type instantiateTheseForResources,
			System.Type instantiateTheseForChildItems,
			System.Type instantiateTheseForChildContainers,
			XmlElement xmlElement,
			out ArrayList proposedChildren
			)
		{
			proposedChildren = new ArrayList();
			ArrayList proposedResources = new ArrayList();
			if (xmlElement == null) return;

			bool isItem, isContainer;
			Type thisType = this.GetType();

			this.UpdateEverything_Reset(updateResources);
			this.UpdateEverything_CheckElementName(xmlElement, out isItem, out isContainer);
			this.UpdateEverything_SetObjectAttributes(xmlElement, isItem, isContainer);
			this.UpdateEverything_SetChildNodes
				(
				xmlElement, 
				isItem, 
				isContainer, 
				updateResources, 
				proposedResources,
				updateChildren, 
				proposedChildren, 
				instantiateTheseForResources, 
				instantiateTheseForChildItems, 
				instantiateTheseForChildContainers
				);

			this.ThrowExceptionIfBadTitle(this.Title);
			this.ThrowExceptionIfBadClass(this.Class);	

			if (updateResources)
			{
				// After obtaining the resources declared in the xml element,
				// create a new arraylist and add the resources
				this.m_LockResources.AcquireWriterLock(-1);
				try
				{
					this.m_Resources = new ArrayList();
					this.AddResources(proposedResources);
				}
				finally
				{
					this.m_LockResources.ReleaseWriterLock();
				}
			}

			// Unlike resources, we don't update the child objects - as child media objects
			// are not considered metadata of this object. The proposedChildren
			// array is primarily an output result of this method. The caller of this method
			// is responsible for adding child objects if it deems appropriate.

#if (DEBUG)
			UpdateEverything_CheckBaseMediaClass(xmlElement, isItem, isContainer);
#endif
		}

		
		/// <summary>
		/// Adds a resource to the media object.
		/// </summary>
		/// <param name="addThis">A new resource, often built using a 
		/// <see cref="ResourceBuilder"/>.CreateXXX method.</param>
		/// <exception cref="OpenSource.UPnP.AV.CdsMetadata.Error_MediaResourceHasParent">
		/// Thrown when attempting to add a IMediaResource instance to a MediaObject instance
		/// when the IMediaResource instance has already been added to different MediaObject instance.
		/// </exception>
		public virtual void AddResource(IMediaResource addThis)
		{
			if (addThis.Owner != null)
			{
				throw new Error_MediaResourceHasParent(addThis);
			}
			else
			{
				addThis.Owner = this;
			}

			try
			{
				this.m_LockResources.AcquireWriterLock(-1);

				if (this.m_Resources == null)
				{
					this.m_Resources = new ArrayList(1);
				}

				this.m_Resources.Add(addThis);
			}
			finally
			{
				this.m_LockResources.ReleaseWriterLock();
			}
		}

		/// <summary>
		/// Removes resource from the media object.
		/// </summary>
		/// <param name="removeThis">IMediaResource object that is attached to this instance.</param>
		public virtual void RemoveResource(IMediaResource removeThis)
		{
			try
			{
				this.m_LockResources.AcquireWriterLock(-1);
				this.m_Resources.Remove(removeThis);
			}
			finally
			{
				this.m_LockResources.ReleaseWriterLock();
			}

			if (this.m_Resources.Count == 0)
			{
				this.m_Resources = null;
			}
			removeThis.Owner = null;
		}

		/// <summary>
		/// Adds resources to the media object.
		/// </summary>
		/// <param name="newResources">A collection of new <see cref="IMediaResource"/> objects.</param>
		/// <exception cref="OpenSource.UPnP.AV.CdsMetadata.Error_MediaResourceHasParent">
		/// Thrown when attempting to add a IMediaResource instance to a MediaObject instance
		/// when the IMediaResource instance has already been added to different MediaObject instance.
		/// </exception>
		public virtual void AddResources(ICollection newResources)
		{
			foreach (IMediaResource res in newResources)
			{
				if (res.Owner != null)
				{
					throw new Error_MediaResourceHasParent(res);
				}
				else
				{
					res.Owner = this;
				}
			}
			
			try
			{
				this.m_LockResources.AcquireWriterLock(-1);

				if (this.m_Resources == null)
				{
					this.m_Resources = new ArrayList(newResources);
				}
				else
				{
					this.m_Resources.AddRange(newResources);
				}
			}
			finally
			{
				this.m_LockResources.ReleaseWriterLock();
			}
		}

		/// <summary>
		/// Removes resources from the media object.
		/// </summary>
		/// <param name="removeThese">A collection of IMediaResource objects that are attached to this media object.</param>
		public virtual void RemoveResources(ICollection removeThese)
		{
			try
			{
				this.m_LockResources.AcquireWriterLock(-1);

				foreach (IMediaResource removeThis in removeThese)
				{
					this.m_Resources.Remove(removeThis);
					removeThis.Owner = null;
				}

				if (this.m_Resources.Count == 0)
				{
					this.m_Resources = null;
				}
			}
			finally
			{
				this.m_LockResources.ReleaseWriterLock();
			}
		}

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
		public virtual void SetMetadata(MediaBuilder.CoreMetadata info)
		{
			info.ID = this.ID;
			info.IdIsValid = true;
			MediaBuilder.SetObjectProperties(this, info);
		}

		/// <summary>
		/// Media entries can indicate if they allow control points to
		/// overwrite resources.
		/// </summary>
		/// <param name="writeStatus">the writeStatus value to apply to this object</param>
		public virtual void SetWriteStatus(EnumWriteStatus writeStatus)
		{
			PropertyEnumWriteStatus[] ar = new PropertyEnumWriteStatus[1];
			ar[0] = CdsMetadataCaches.PropertyEnumWriteStatus.CacheThis(T[_UPNP.writeStatus], writeStatus);
			this.m_Properties[T[_UPNP.writeStatus]] = ar;
		}

		/// <summary>
		/// Media entries can indicate how their resources are stored.
		/// </summary>
		/// <param name="values">the storage mediums that apply to this object object</param>
		public virtual void SetStorageMediums(PropertyStorageMedium[] values)
		{
			ArrayList al = new ArrayList(values.Length);
			al.AddRange(values);
			this.m_Properties[T[_UPNP.storageMedium]] = al;
		}

		/// <summary>
		/// Each media object can only have one media class, and this method
		/// makes it easy for a developer to change the media class of the instance.
		/// The ability for a public programmer to change the instance's media class 
		/// directly is available only from a device-side programming scenario. 
		/// </summary>
		/// <param name="classType">a class path, such as "object.item" or "object.container"</param>
		/// <param name="friendlyName">an optional friendly name for the class</param>
		/// <exception cref="OpenSource.UPnP.AV.CdsMetadata.Error_BadMediaClass">
		/// Thrown if the classType string is not qualified as an "object.item" or "object.container"
		/// type of class.
		/// </exception>
		public virtual void SetClass(string classType, string friendlyName)
		{
			MediaClass mc = CdsMetadataCaches.MediaClasses.CacheThis(classType, friendlyName);
			this.SetPropertyValue_MediaClass(T[_UPNP.Class], mc);
		}

		/// <summary>
		/// Allows the setting of any metadata for any property.
		/// Programmer specifies the property name and a listing of <see cref="ICdsElement"/> objects.
		/// </summary>
		/// <param name="propertyName">the tag name, including abbreviated namespace, of the desired UPnP-AV normative element</param>
		/// <param name="values">An <see cref="IList"/> of <see cref="ICdsElement"/> objects.</param>
		/// <exception cref="InvalidCastException">
		/// Thrown when an item in values is not an ICdsElement object.
		/// </exception>
		public virtual void SetPropertyValue (string propertyName, IList values)
		{
			ArrayList al = new ArrayList(values.Count);
			foreach (ICdsElement v in values)
			{
				al.Add(v);
			}
			this.m_Properties[propertyName] = al;
		}

		/// <summary>
		/// Metadata often benefits from having strongly typed values, and
		/// this method allows a programmer to easily set a CDS entry's property
		/// with strongly typed values very easily. A public programmer's ability to modify
		/// an object's metadata directly is available only from a device-side programming
		/// scenario.
		/// </summary>
		/// <param name="propertyName">
		/// Standard metadata properties can be obtained by using the OpenSource.UPnP.AV.CdsMetadata.CommonPropertyNames enumerator
		/// in conjunction with an instantiation of the Tags class, which returns string representations of common CDS tagnames and attributes.
		/// </param>
		/// <param name="values">array of string values that will overwrite all values associated with the specified metadata field</param>
		public virtual void SetPropertyValue_String(string propertyName, string[] values)
		{
			ArrayList al = new ArrayList(values.Length);
			foreach (string val in values)
			{
				al.Add( new PropertyString(propertyName, val) );
			}

			this.m_Properties[propertyName] = al;
		}

		/// <summary>
		/// Metadata often benefits from having strongly typed values, and
		/// this method allows a programmer to easily set a CDS entry's property
		/// with strongly typed values very easily. A public programmer's ability to modify
		/// an object's metadata directly is available only from a device-side programming
		/// scenario.
		/// </summary>
		/// <param name="propertyName">
		/// Standard metadata properties can be obtained by using the OpenSource.UPnP.AV.CdsMetadata.CommonPropertyNames enumerator
		/// in conjunction with an instantiation of the OpenSource.UPnP.AV.CdsMetadata.Tags class, which returns string representations of common CDS tagnames and attributes.
		/// </param>
		/// <param name="val">a single string value</param>
		public virtual void SetPropertyValue_String(string propertyName, string val)
		{
			string[] values = new string[1];
			values[0] = val;
			SetPropertyValue_String(propertyName, values);
		}

		/// <summary>
		/// Metadata often benefits from having strongly typed values, and
		/// this method allows a programmer to easily set a CDS entry's property
		/// with strongly typed values very easily. A public programmer's ability to modify
		/// an object's metadata directly is available only from a device-side programming
		/// scenario.
		/// </summary>
		/// <param name="propertyName">
		/// Standard metadata properties can be obtained by using the OpenSource.UPnP.AV.CdsMetadata.CommonPropertyNames enumerator
		/// in conjunction with an instantiation of the OpenSource.UPnP.AV.CdsMetadata.Tags class, which returns string representations of common CDS tagnames and attributes.
		/// </param>
		/// <param name="values">array of int values that will overwrite all values associated with the specified metadata field</param>
		public virtual void SetPropertyValue_Int(string propertyName, int[] values)
		{
			ArrayList al = new ArrayList(values.Length);
			foreach (int val in values)
			{
				al.Add( new PropertyInt(propertyName, val) );
			}

			this.m_Properties[propertyName] = al;
		}

		/// <summary>
		/// Metadata often benefits from having strongly typed values, and
		/// this method allows a programmer to easily set a CDS entry's property
		/// with strongly typed values very easily. A public programmer's ability to modify
		/// an object's metadata directly is available only from a device-side programming
		/// scenario.
		/// </summary>
		/// <param name="propertyName">
		/// Standard metadata properties can be obtained by using the OpenSource.UPnP.AV.CdsMetadata.CommonPropertyNames enumerator
		/// in conjunction with an instantiation of the OpenSource.UPnP.AV.CdsMetadata.Tags class, which returns string representations of common CDS tagnames and attributes.
		/// </param>
		/// <param name="val">a single int value that will overwrite all values associated with the specified metadata field</param>
		public virtual void SetPropertyValue_Int(string propertyName, int val)
		{
			int[] values = new int[1];
			values[0] = val;
			SetPropertyValue_Int(propertyName, values);
		}
		
		/// <summary>
		/// Metadata often benefits from having strongly typed values, and
		/// this method allows a programmer to easily set a CDS entry's property
		/// with strongly typed values very easily. A public programmer's ability to modify
		/// an object's metadata directly is available only from a device-side programming
		/// scenario.
		/// </summary>
		/// <param name="propertyName">
		/// Standard metadata properties can be obtained by using the OpenSource.UPnP.AV.CdsMetadata.CommonPropertyNames enumerator
		/// in conjunction with an instantiation of the OpenSource.UPnP.AV.CdsMetadata.Tags class, which returns string representations of common CDS tagnames and attributes.
		/// </param>
		/// <param name="values">array of long values that will overwrite all values associated with the specified metadata field</param>
		public virtual void SetPropertyValue_Long(string propertyName, long[] values)
		{
			ArrayList al = new ArrayList(values.Length);
			foreach (long val in values)
			{
				al.Add( new PropertyLong(propertyName, val) );
			}

			this.m_Properties[propertyName] = al;
		}

		/// <summary>
		/// Metadata often benefits from having strongly typed values, and
		/// this method allows a programmer to easily set a CDS entry's property
		/// with strongly typed values very easily. A public programmer's ability to modify
		/// an object's metadata directly is available only from a device-side programming
		/// scenario.
		/// </summary>
		/// <param name="propertyName">
		/// Standard metadata properties can be obtained by using the OpenSource.UPnP.AV.CdsMetadata.CommonPropertyNames enumerator
		/// in conjunction with an instantiation of the OpenSource.UPnP.AV.CdsMetadata.Tags class, which returns string representations of common CDS tagnames and attributes.
		/// </param>
		/// <param name="val">a single long value that will overwrite all values associated with the specified metadata field</param>
		public virtual void SetPropertyValue_Long(string propertyName, long val)
		{
			long [] values = new long[1];
			values[0] = val;
			SetPropertyValue_Long(propertyName, values);
		}

		/// <summary>
		/// Metadata often benefits from having strongly typed values, and
		/// this method allows a programmer to easily set a CDS entry's property
		/// with strongly typed values very easily. A public programmer's ability to modify
		/// an object's metadata directly is available only from a device-side programming
		/// scenario.
		/// </summary>
		/// <param name="propertyName">
		/// Standard metadata properties can be obtained by using the OpenSource.UPnP.AV.CdsMetadata.CommonPropertyNames enumerator
		/// in conjunction with an instantiation of the OpenSource.UPnP.AV.CdsMetadata.Tags class, which returns string representations of common CDS tagnames and attributes.
		/// </param>
		/// <param name="values">array of MediaClass values that will overwrite all values associated with the specified metadata field</param>
		public virtual void SetPropertyValue_MediaClass(string propertyName, MediaClass[] values)
		{
			ArrayList al = new ArrayList(values.Length);
			al.AddRange(values);
			this.m_Properties[propertyName] = al;
		}

		/// <summary>
		/// Metadata often benefits from having strongly typed values, and
		/// this method allows a programmer to easily set a CDS entry's property
		/// with strongly typed values very easily. A public programmer's ability to modify
		/// an object's metadata directly is available only from a device-side programming
		/// scenario.
		/// </summary>
		/// <param name="propertyName">
		/// Standard metadata properties can be obtained by using the OpenSource.UPnP.AV.CdsMetadata.CommonPropertyNames enumerator
		/// in conjunction with an instantiation of the OpenSource.UPnP.AV.CdsMetadata.Tags class, which returns string representations of common CDS tagnames and attributes.
		/// </param>
		/// <param name="val">a single MediaClass value that will overwrite all values associated with the specified metadata field</param>
		public virtual void SetPropertyValue_MediaClass(string propertyName, MediaClass val)
		{
			MediaClass[] values = new MediaClass[1];
			values[0] = val;
			SetPropertyValue_MediaClass(propertyName, values);
		}

		/// <summary>
		/// <para>
		/// Stores the ID of the media object, whether it be a container or an item.
		/// </para>
		/// <para>
		/// Derived classes will have access to this and so will internal classes.
		/// Programmers are strongly encouraged to not mess with this value unless
		/// they ABSOLUTELY know what they're doing.
		/// </para>
		/// 
		/// <para>
		/// Access is given to protected and internal to allow faster access
		/// to modifying the field.
		/// </para>
		/// </summary>
		protected internal string m_ID;

		/// <summary>
		/// Maps to the "restricted" attribute of an "item" or "container" element.
		/// <para>
		/// Derived classes will have access to this and so will internal classes.
		/// Programmers are strongly encouraged to not mess with this value unless
		/// they ABSOLUTELY know what they're doing.
		/// </para>
		/// </summary>
		protected internal bool m_Restricted
		{
			get
			{
				return this.m_bools[(int)EnumBools.Restricted];
			}
			set
			{
				this.m_bools[(int)EnumBools.Restricted] = value;
			}
		}

		/// <summary>
		/// This accessor forces the parent object to always be
		/// a MediaContainer object.
		/// </summary>
		internal MediaContainer _Parent
		{
			get
			{
				return this.m_Parent;
			}
			set
			{
				this.m_Parent = value;

				if (value != null)
				{
					this.m_ParentID = value.ID;
				}
				else
				{
					this.m_ParentID = null;
				}
			}
		}

		/// <summary>
		/// Reference to a MediaContainer object that acts as this object's parent
		/// in a content hierarchy.
		/// <para>
		/// Derived classes will have access to this and so will internal classes.
		/// Programmers are strongly encouraged to not mess with this value unless
		/// they ABSOLUTELY know what they're doing.
		/// </para>
		/// </summary>
		[NonSerialized()] protected MediaContainer m_Parent;

		/// <summary>
		/// This field allows m_Parent to be null, whilst allows
		/// ParentID to to return a value. This value never has
		/// precedence over <see cref="MediaObject.m_Parent"/>.
		/// The <see cref="MediaObject.m_Parent"/> field should
		/// always supercede this value when representing the parent.
		/// </summary>
		[NonSerialized()] protected string m_ParentID = null;

		/// <summary>
		/// Keeps the actual reference returned in the Properties field.
		/// <para>
		/// Derived classes will have access to this and so will internal classes.
		/// Programmers are strongly encouraged to not mess with this value unless
		/// they ABSOLUTELY know what they're doing.
		/// </para>
		/// </summary>
		protected internal MediaProperties m_Properties;

		/// <summary>
		/// Locks the m_Desc arraylist for reading/writing.
		/// Always lock this after <see cref="m_LockResources"/>
		/// </summary>
		//[NonSerialized()] protected ReaderWriterLock m_LockDesc = null;

		/// <summary>
		/// Keeps a listing of custom/vendor-specific metadata as "desc" elements.
		/// Each element is in string form.
		/// </summary>
		protected ArrayList m_Desc = null;

		/// <summary>
		/// Allows an additional custom/vendor-specified metadata field
		/// to get added to the media object. The ability to add
		/// metadata directly from a public context is not available
		/// in control-point scenarios.
		/// <para>
		/// Derived classes will have access to this and so will internal classes.
		/// Programmers deriving from this object are strongly encouraged to not use 
		/// this unless they ABSOLUTELY know what they're doing.
		/// </para>
		/// </summary>
		/// <param name="element">The metadata block must be in xml form.</param>
		public virtual void AddDescNode(string element)
		{
			try
			{
				//m_LockDesc.AcquireWriterLock(-1);
				this.m_Properties.RWLock.AcquireWriterLock(-1);
				if (this.m_Desc == null)
				{
					this.m_Desc = new ArrayList(DescNodeArrayListSize);
				}
				m_Desc.Add(element);
			}
			finally
			{
				//m_LockDesc.ReleaseWriterLock();
				this.m_Properties.RWLock.ReleaseWriterLock();
			}
		}

		/// <summary>
		/// Generally speaking, we'll assume a media object will have few
		/// desc nodes, but an implementation can opt to change the 
		/// initial capacity of desc nodes for all media objects.
		/// </summary>
		public static int DescNodeArrayListSize = 2;
		
		/// <summary>
		/// Allows an additional custom/vendor-specified metadata field
		/// to get added to the media object. The ability to add
		/// metadata directly from a public context is not available
		/// in control-point scenarios.
		/// <para>
		/// Derived classes will have access to this and so will internal classes.
		/// Programmers deriving from this object are strongly encouraged to not use 
		/// this unless they ABSOLUTELY know what they're doing.
		/// </para>
		/// </summary>
		/// <param name="elements">The metadata blocks must be in xml form.</param>
		public virtual void AddDescNode(string[] elements)
		{
			if (m_Desc == null) m_Desc = new ArrayList(elements.Length);

			try
			{
				//m_LockDesc.AcquireWriterLock(-1);
				this.m_Properties.RWLock.AcquireWriterLock(-1);
				if (this.m_Desc == null)
				{
					this.m_Desc = new ArrayList((ICollection) elements);
				}
				else
				{
					m_Desc.AddRange(elements);
				}
			}
			finally
			{
				//m_LockDesc.ReleaseWriterLock();
				this.m_Properties.RWLock.ReleaseWriterLock();
			}
		}
		/// <summary>
		/// Allows an additional custom/vendor-specified metadata field
		/// to be removed from the media object. The ability to remove
		/// metadata directly from a public context is not available
		/// in control-point scenarios.
		/// <para>
		/// Derived classes will have access to this and so will internal classes.
		/// Programmers deriving from this object are strongly encouraged to not use 
		/// this unless they ABSOLUTELY know what they're doing.
		/// </para>
		/// </summary>
		/// <param name="element">The metadata block in xml form that should be removed.</param>
		public virtual void RemoveDescNode(string element)
		{
			if (m_Desc == null) return;

			try
			{
				//m_LockDesc.AcquireWriterLock(-1);
				this.m_Properties.RWLock.AcquireWriterLock(-1);
				m_Desc.Remove(element);
				if (this.m_Desc.Count == 0)
				{
					this.m_Desc = null;
				}
			}
			finally
			{
				//m_LockDesc.ReleaseWriterLock();
				this.m_Properties.RWLock.ReleaseWriterLock();
			}
		}

		/// <summary>
		/// This property indicates if the media object's properties
		/// will report metadata changes asynchronously. If true,
		/// then the object's implementation of
		/// <see cref="Sink_OnMediaPropertiesChanged"/> will execute
		/// whenever the metadata in <see cref="Properties"/> changes.
		/// </summary>
		public virtual bool TrackMetadataChanges
		{
			get
			{
				return this.m_Properties.IsEnabled_OnMetadataChanged;
			}
			set
			{
				this.m_Properties.IsEnabled_OnMetadataChanged = value;
			}
		}

		/// <summary>
		/// This lock should be used when modifying or reading m_Resources.
		/// <para>
		/// Derived classes will have access to this and so will internal classes.
		/// Programmers deriving from this object are strongly encouraged to not use 
		/// this unless they ABSOLUTELY know what they're doing.
		/// </para>
		/// </summary>
		[NonSerialized()] protected internal ReaderWriterLock m_LockResources;

		/// <summary>
		/// This keeps a list of all IMediaResource objects associated with
		/// this MediaObject instance.
		/// <para>
		/// Derived classes will have access to this and so will internal classes.
		/// Programmers deriving from this object are strongly encouraged to not use 
		/// this unless they ABSOLUTELY know what they're doing.
		/// </para>
		/// </summary>
		protected internal ArrayList m_Resources = null;

		/// <summary>
		/// Whenever data is accessed in Properties, we should note the
		/// state number as it may indicate if we need to do error
		/// checking when examining Class, Title, and Creator.
		/// </summary>
		[NonSerialized()] protected int m_PropertiesStateNumber;

		/// <summary>
		/// Cached value returned in Title property.
		/// </summary>
		[NonSerialized()] protected string m_CacheTitle = null;
		/// <summary>
		/// Cached value returned in Creator property.
		/// </summary>
		[NonSerialized()] protected string m_CacheCreator = null;
		/// <summary>
		/// Cached value returned in Class property.
		/// </summary>
		[NonSerialized()] protected MediaClass m_CacheClass = null;


		/// <summary>
		/// Method executes when m_Properties.OnMetadataChanged fires.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="stateNumber"></param>
		protected virtual void Sink_OnMediaPropertiesChanged (MediaProperties sender, int stateNumber)
		{
			this.UpdateCache();
		}

		/// <summary>
		/// Updates m_CacheTitle, m_CacheCreator, and m_CacheClass.
		/// </summary>
		protected void UpdateCache()
		{
			if (this.m_PropertiesStateNumber != this.m_Properties.StateNumber)
			{
				// Some have made an argument that the extensive amount of
				// error checking is unnecessary given that I ensure that
				// the value is proper before setting.
				//
				// Unfortunately, simply returning list[0].Value has problems.
				// A number of applications have been written with the 
				// expectation that this property will never return null
				// or throw an InvalidCast/NullReference exception.
				//
				// The whole purpose of UpdateCache is to minimize
				// the error-checking done for the Title, Creator, and
				// Class properties (which are all properties provided
				// as a courtesy so programmers don't have to do tedious
				// error checking). 
				//
				// Given that the vast majority of
				// implementations won't change metadata regularly,
				// the caching is actually quite effective.

				string title = T[_DC.title];
				string creator = T[_DC.creator];
				string Class = T[_UPNP.Class];
				string[] properties = {title, creator, Class};
				Hashtable lists = this.m_Properties.GetValues(properties);
				IList list;

				this.m_CacheTitle = "";
				this.m_CacheCreator = "";
				this.m_CacheClass = null;

				if (lists != null)
				{
					list = (IList) lists[title];
					if ((list != null) && (list.Count > 0) && (list[0] is CdsMetadata.ICdsElement))
					{
						this.m_CacheTitle = (string) ((ICdsElement)list[0]).Value;
					}
				
					list = (IList) lists[creator];
					if ((list != null) && (list.Count > 0) && (list[0] is CdsMetadata.ICdsElement))
					{
						this.m_CacheCreator = (string) ((ICdsElement)list[0]).Value;
					}

					list = (IList) lists[Class];
					if ((list != null) && (list.Count > 0) && (list[0] is CdsMetadata.MediaClass))
					{
						this.m_CacheClass = (MediaClass) list[0];
					}
				}

				this.m_PropertiesStateNumber = this.m_Properties.StateNumber;
			}
		}

		/// <summary>
		/// Used for obtaining the attribute and tag names of standard metadata properties.
		/// </summary>
		protected static Tags T = Tags.GetInstance();

		/// <summary>
		/// Expects the attribute value to be one of the UPNP values:
		/// <list type="bullet">
		/// <item><description>true</description></item>
		/// <item><description>1</description></item>
		/// <item><description>yes</description></item>
		/// <item><description>false</description></item>
		/// <item><description>0</description></item>
		/// <item><description>no</description></item>
		/// </list>
		/// Returns true if the value is affirmative.
		/// 
		/// <para>
		/// Derived classes will have access to this and so will internal classes.
		/// Programmers deriving from this object are strongly encouraged to not use 
		/// this unless they ABSOLUTELY know what they're doing.
		/// </para>
		/// </summary>
		/// <param name="attrib">XmlAttribute with the value type expected to be like a boolean</param>
		/// <param name="defValue">if accpetable value not found, use this default value</param>
		/// <returns>Returns true if the value is a true equivalent.</returns>
		protected internal static bool IsAttributeValueTrue (XmlAttribute attrib, bool defValue)
		{
			if (attrib == null) return defValue;

			string val = attrib.Value.Trim();

			return IsValueTrue(val, defValue);
		}

		/// <summary>
		/// Implements the core logic for determining whether a string
		/// mataches to true or false. (true = "true, 1, yes"; false = "false", "0", "no")
		/// All comparisons are case insensitive.
		/// </summary>
		/// <param name="val"></param>
		/// <param name="defValue"></param>
		/// <returns></returns>
		protected internal static bool IsValueTrue (string val, bool defValue)
		{
			if (
				(String.Compare(val, "true", true) == 0) ||
				(String.Compare(val, "1", true) == 0) ||
				(String.Compare(val, "yes", true) == 0)
				)
			{
				return true;
			}
			else if (
				(String.Compare(val, "false", true) == 0) ||
				(String.Compare(val, "0", true) == 0) ||
				(String.Compare(val, "no", true) == 0)
				)
			{
				return false;
			}
			else
			{
				return defValue;
			}
		}

		/// <summary>
		/// Returns a boolean in a zero or one representation.
		/// </summary>
		/// <param name="bval"></param>
		/// <returns></returns>
		public static int GetBoolAsOneZero (bool bval)
		{
			if (bval == true) return 1;

			return 0;
		}

		public virtual void TrimToSize()
		{
			if (this.m_Properties != null)
			{
				try
				{
					this.m_Properties.RWLock.AcquireWriterLock(-1);
					this.m_Properties.TrimTableToSize();
					if (this.m_Desc != null)
					{
						this.m_Desc.TrimToSize();
					}
				}
				finally
				{
					this.m_Properties.RWLock.ReleaseWriterLock();
				}
			}

			try
			{
				this.m_LockResources.AcquireWriterLock(-1);
				if (this.m_Resources != null)
				{
					foreach (MediaResource res in this.m_Resources)
					{
						res.TrimToSize();
					}
					this.m_Resources.TrimToSize();
				}
			}
			finally
			{
				this.m_LockResources.ReleaseWriterLock();
			}
		}

		public static readonly ReaderWriterLock StaticLock = new ReaderWriterLock();

		public static bool UseStaticLock = false;
	}
}
