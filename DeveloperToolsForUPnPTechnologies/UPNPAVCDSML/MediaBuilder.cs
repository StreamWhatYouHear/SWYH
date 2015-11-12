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
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using OpenSource.UPnP.AV;

namespace OpenSource.UPnP.AV.CdsMetadata
{
	/// <summary>
	/// <para>
	/// This class is a class factory for creating 
	/// <see cref="MediaItem"/> and
	/// <see cref="MediaContainer"/>
	/// objects.
	/// </para>
	/// </summary>
	public class MediaBuilder
	{

		/// <summary>
		/// This interface is not yet used, but theoeretically
		/// it will allow for the use of custom metadata
		/// in <see cref="MediaBuilder.CoreMetadata"/>
		/// derived objects.
		/// </summary>
		public interface ICustomMetadata
		{
			string[] additionalDescNodes { get; set; }
		}

		/// <summary>
		/// Method returns a DIDL-Lite representation of a set of media objects.
		/// </summary>
		/// <param name="formatter">
		/// <para>
		/// Formatter for printing the media objects. Standard formatter is to use
		/// <see cref="ToXmlFormatter.DefaultFormatter"/>.
		/// </para>
		/// 
		/// <para>
		/// Another alternative is to use a <see cref="ToXmlData"/> object,
		/// and assign the delegate-based fields to method implementations
		/// that will provide customized XML-writing of the objects. 
		/// The fields that should be assigned for a task are:
		/// <see cref="ToXmlFormatter.WriteContainet"/>,
		/// <see cref="ToXmlFormatter.WriteItem"/>, and
		/// <see cref="ToXmlFormatter.WriteResource"/>.
		/// </para>
		/// 
		/// <para>
		/// Programmers should never assign the 
		/// <see cref="ToXmlFormatter.StartElement"/>,
		/// <see cref="ToXmlFormatter.WriteInnerXml"/>,
		/// <see cref="ToXmlFormatter.WriteValue"/>, or
		/// <see cref="ToXmlFormatter.EndElement"/>.
		/// </para>
		/// 
		/// <param name="data">
		/// <para>
		/// Data object with instructions to give to the formatter. Standard ones
		/// usable by <see cref="MediaObject.ToXmlFormatter_Default"/> include
		/// <see cref="ToXmlData_Default"/> (used for flat lists), and
		/// <see cref="ToXmlData_AllRecurse"/> (used for representing a trees).
		/// You can use your own settings by instantiating a <see cref="ToXmlData"/>
		/// object and setting the fields yourself.
		/// </para>
		/// 
		/// <para>
		/// If the 
		/// <see cref="ToXmlFormatter.WriteContainet"/>,
		/// <see cref="ToXmlFormatter.WriteItem"/>, or
		/// <see cref="ToXmlFormatter.WriteResource"/>
		/// fields have been assigned, then the object that is sent
		/// must be compatible with the method implementations.
		/// One subtlety that must be noted is that if one of
		/// those fields is not assigned to a custom implementation,
		/// then it follows that the 'data' argument must be a
		/// <see cref="ToXmlData"/> instance. For maximum compatibility,
		/// programmers are encouraged to derive classes from
		/// <see cref="ToXmlData"/> instead of defining their own
		/// from scratch.
		/// </para>
		/// </param>
		/// <param name="entries"><see cref="ICollection"/> of IUPnPMedia objects</param>
		/// <returns></returns>
		public static string BuildDidl(ToXmlFormatter formatter, object data, ICollection entries)
		{
			StringBuilder sb = null;
			StringWriter sw = null;
			MemoryStream ms = null;
			XmlTextWriter xmlWriter = null;
			
			// set up the DIDL-Lite document for UTF8 or UTF16 encodings
			if (MediaObject.ENCODE_UTF8)
			{
				ms = new MemoryStream(MediaObject.XML_BUFFER_SIZE);
				xmlWriter = new XmlTextWriter(ms, System.Text.Encoding.UTF8);
			}
			else
			{
				sb = new StringBuilder(MediaObject.XML_BUFFER_SIZE);
				sw = new StringWriter(sb);
				xmlWriter = new XmlTextWriter(sw);
			}

			// Set up the XML writer for proper formatting and the DIDL-Lite header

			xmlWriter.Formatting = System.Xml.Formatting.Indented;
			xmlWriter.Namespaces = true;
			xmlWriter.WriteStartDocument();
			
			MediaObject.WriteResponseHeader(xmlWriter);
		
			// Make a clone of 'data' if it's a ToXmlData object.
			// We'll need to clone it because we're may flip
			// some bits on it and the original ToXmlData object
			// is not supposed to be modified.
			ToXmlData _d = data as ToXmlData;
			ToXmlData _d2 = null;
			if (_d != null) { _d2 = _d.Clone() as ToXmlData; }

			if (_d2 != null)
			{
				if (_d2.VirtualOwner != null)
				{
					// Do a memberwise clone because we're going to
					// force this virtual container to not print its
					// child elements.
					formatter.StartElement = null;
					formatter.EndElement = null;
					formatter.WriteInnerXml = null;
					formatter.WriteValue  = null;

					_d2.IsRecursive = false;
					_d2.IgnoreBlankParentError = true;
					_d2.IncludeValue = false;
					_d2.IncludeInnerXml = true;

					_d2.VirtualOwner.StartElement(formatter, _d2, xmlWriter);
					_d2.VirtualOwner.WriteInnerXml(formatter, _d2, xmlWriter);
				}
			}

			// print each media object, indicating that we don't
			// want the recursive printing of child objects.
			// Also provide the base URL so that resource objects
			// are propertly printed

			formatter.StartElement = null;
			formatter.EndElement = null;
			formatter.WriteInnerXml = null;
			formatter.WriteValue  = null;
			foreach (IUPnPMedia obj in entries)
			{
				//obj.ToAlternateXml(false, baseUrl, properties, xmlWriter);
				obj.ToXml(formatter, data, xmlWriter);
			}
			
			// Close the virtual-owner container element
			if (_d2 != null)
			{
				if (_d2.VirtualOwner != null)
				{
					_d2.VirtualOwner.EndElement(formatter, _d2, xmlWriter);
				}
			}

			xmlWriter.WriteEndDocument();
			xmlWriter.Flush();
			
			// cast the string builder's data to a string

			string xmlResult;
			if (MediaObject.ENCODE_UTF8)
			{
				// UTF8 encoded stuff is a little different
				// because we need to drop the first 3 bytes
				// because they cause a magic characters to 
				// appear in the string.
				int len = (int) ms.ToArray().Length - MediaObject.TruncateLength_UTF8;
				UTF8Encoding utf8e = new UTF8Encoding(false, true);
				xmlResult = utf8e.GetString(ms.ToArray(), MediaObject.TruncateLength_UTF8, len);
			}
			else
			{
				xmlResult = sb.ToString();
			}
			xmlWriter.Close();

			// Ensure that any preceding carriage returns 
			// are removed from the result string
			int crpos = xmlResult.IndexOf("\r\n");
			crpos = xmlResult.IndexOf('<', crpos);
			string trunc = xmlResult.Remove(0, crpos);

			return trunc;
		}


		/// <summary>
		/// Given a DIDL-Lite document in string form, this method
		/// creates a set of subtrees that represent the document.
		/// </summary>
		/// <param name="DidlLiteXml"></param>
		/// <returns>
		/// arraylist of 
		/// <see cref="MediaItem"/> or
		/// <see cref="MediaContainer"/> objects.
		/// </returns>
		/// <exception cref="Error_BadMetadata">
		/// Thrown when the DIDL-Lite is not well formed or not compliant
		/// with ContentDirectory specifications.
		/// </exception>
		public static ArrayList BuildMediaBranches(string DidlLiteXml)
		{
			return BuildMediaBranches(DidlLiteXml, typeof(MediaItem), typeof(MediaContainer));
		}

		/// <summary>
		/// Calls <see cref="MediaBuilder.BuildMediaBranches"/>(string, Type, Type, bool=false).
		/// </summary>
		/// <param name="DidlLiteXml"></param>
		/// <param name="typeItems"></param>
		/// <param name="typeContainers"></param>
		/// <returns></returns>
		public static ArrayList BuildMediaBranches (string DidlLiteXml, Type typeItems, Type typeContainers)
		{
			return BuildMediaBranches(DidlLiteXml, typeItems, typeContainers, false);
		}

		/// <summary>
		/// Implements the core logic for BuildMediaBranches(string), but enables
		/// new class factories for derived types that use the core logic of MediaBuilder.
		/// </summary>
		/// <param name="DidlLiteXml">string representing a ContentDirectory DIDL-Lite document</param>
		/// <param name="typeItems">
		/// The type of object to instantiate for any item element, class must have a constructor that uses a single XmlElement, and is also derived from 
		/// <see cref="MediaItem"/>.
		/// </param>
		/// <param name="typeContainers">
		/// The type of object to instantiate for any container element, class must have a constructor that uses a single XmlElement, and is also derived from 
		/// <see cref="MediaContainer"/>.
		/// </param>
		/// <param name="useValidatingReader">
		/// TODO: Not implemented.
		/// If true, then the DidlLiteXml is loaded with a schema validating reader. 
		/// Otherwise, it is loaded and schema is implicitly enforced by the UPNPAVCDSML library.
		/// The UPNPAVCDSML library is slightly more forgiving for purposes of interoperability.
		/// </param>
		/// <returns></returns>
		public static ArrayList BuildMediaBranches (string DidlLiteXml, Type typeItems, Type typeContainers, bool useValidatingReader)
		{

			XmlDocument xmlDoc = new XmlDocument();
			if (useValidatingReader)
			{
				try
				{
					/*
					TextReader txtReader = new TextReader();
					txtReader.
					XmlTextReader xmlTextReader = new XmlTextReader(txtReader);
					XmlValidatingReader xmlValidatingReader = new XmlValidatingReader(xmlTextReader);
					xmlValidatingReader.ValidationType = ValidationType.Schema;
					xmlDoc.Load(xmlValidatingReader);
					*/

					xmlDoc.LoadXml(DidlLiteXml);
				}
				catch (Exception badmetadata)
				{
					throw new Error_BadMetadata("The provided DIDL-Lite XML is not well-formed. " + badmetadata.Message);
				}
			}
			else
			{
				try
				{
					xmlDoc.LoadXml(DidlLiteXml);
				}
				catch (Exception badmetadata)
				{
					throw new Error_BadMetadata("The provided DIDL-Lite XML is not well-formed. " + badmetadata.Message);
				}
			}

			// Recurse the xmldom and build items and containers. All new/immediate branches/children
			// will be stored in newBranches.
			// 
			ArrayList newBranches = new ArrayList();

			Type[] argTypes = new Type[1];
			argTypes[0] = typeof(XmlElement);
			ConstructorInfo itemConstructor = typeItems.GetConstructor(argTypes);
			ConstructorInfo containerConstructor = typeContainers.GetConstructor(argTypes);
							
			object[] args = new object[1];

			foreach (XmlElement child in xmlDoc.GetElementsByTagName(T[_DIDL.DIDL_Lite]).Item(0).ChildNodes)
			{
				IUPnPMedia newObj = null;
				args[0] = child;

				if (string.Compare(child.Name, T[_DIDL.Item]) == 0) 
				{
					try
					{
						newObj = (IMediaItem) itemConstructor.Invoke(args);
					}
					catch (Exception ce)
					{
						throw new ApplicationException("Error constructing item from XML", ce);
					}
				}
				else if (string.Compare(child.Name, T[_DIDL.Container]) == 0) 
				{
					try
					{
						newObj = (IMediaContainer) containerConstructor.Invoke(args);
					}
					catch (Exception ce)
					{
						throw new ApplicationException("Error constructing container from XML", ce);
					}
				}
				if (newObj != null)
				{
					// ensure that a few basic attributes are set
					RecursiveValidationOfMediaObject(newObj, true, true);

					newBranches.Add(newObj);
				}
			}

			return newBranches;
		}

		/// <summary>
		/// Recursively checks a media object and all of its descendents, applying the
		/// <see cref="MediaBuilder.ValidateMediaObject"/> as the criteria.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="hasParentID"></param>
		/// <param name="throwException"></param>
		/// <returns></returns>
		public static bool RecursiveValidationOfMediaObject (IUPnPMedia obj, bool hasParentID, bool throwException)
		{
			bool result = true;
			IMediaContainer mc = obj as IMediaContainer;

			if (mc != null)
			{
				foreach (IUPnPMedia child in mc.CompleteList)
				{
					result = RecursiveValidationOfMediaObject(child, hasParentID, throwException);

					if (result == false)
					{
						return result;
					}
				}
			}

			return ValidateMediaObject(obj, hasParentID, throwException);
		}

		/// <summary>
		/// Validates an <see cref="IUPnPMedia"/> object for basic schema compliance.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="hasParentID">If true, then "obj" should have proper parentID information.</param>
		/// <param name="throwException">If true, then any error will cause an exception to be thrown.</param>
		/// <returns>True, if the media object is valid.</returns>
		public static bool ValidateMediaObject (IUPnPMedia obj, bool hasParentID, bool throwException)
		{
			if (obj.ID == null)
			{
				if (throwException)
				{
					throw new Error_BadMetadata("ObjectID is null.");
				}
				return false;
			}
			else if (obj.ID == "")
			{
				if (throwException)
				{
					throw new Error_BadMetadata("Object ID is an empty string.");
				}
				return false;
			}
			else if (obj.Title == null)
			{
				if (throwException)
				{
					throw new Error_BadMetadata("Title is null for ObjectID='" + obj.ID+ "'.");
				}
				return false;
			}
			else if (obj.Title.Trim() == "")
			{
				if (throwException)
				{
					throw new Error_BadMetadata("Title contains only white-space characters for ObjectID='" + obj.ID+ "'.");
				}
				return false;
			}
			else if (obj.Class == null)
			{
				if (throwException)
				{
					throw new Error_BadMetadata("Media class is null for ObjectID='" + obj.ID+ "'.");
				}
				return false;
			}
			else if (obj.Class == MediaClass.NullMediaClass)
			{
				if (throwException)
				{
					throw new Error_BadMetadata("Media class is NullMediaClass for ObjectID='" + obj.ID+ "'.");
				}
				return false;
			}
			else if (obj.Class.FullClassName == null)
			{
				if (throwException)
				{
					throw new Error_BadMetadata("Media class FullClassName is null for ObjectID='" + obj.ID+ "'.");
				}
				return false;
			}
			else if (! ((obj.Class.FullClassName.StartsWith("object.item")) || (obj.Class.FullClassName.StartsWith("object.container"))))
			{
				if (throwException)
				{
					throw new Error_BadMetadata("Media class " +obj.Class.FullClassName+ " does not begin with object.item or object.container.");
				}
				return false;
			}
			else if (obj.IsContainer != obj.Class.IsContainer)
			{
				if (throwException)
				{
					throw new ApplicationException("IUPnPMedia.IsContainer != IUPnPMedia.Class.IsContainer");
				}
				return false;
			}
			else if (obj.IsItem != obj.Class.IsItem)
			{
				if (throwException)
				{
					throw new ApplicationException("IUPnPMedia.IsItem != IUPnPMedia.Class.IsItem");
				}
				return false;
			}
			else if (obj.IsContainer == obj.Class.IsItem)
			{
				if (throwException)
				{
					throw new ApplicationException("IUPnPMedia.IsContainer == IUPnPMedia.Class.IsItem");
				}
				return false;
			}
			else if ((obj.IsContainer) && ((obj is IMediaContainer) == false))
			{
				if (throwException)
				{
					throw new ApplicationException("ObjectID='" +obj.ID+ "' has IsContainer=true, but it does not implement IMediaContainer.");
				}
				return false;
			}			
			else if ((obj.IsItem) && ((obj is IMediaItem) == false))
			{
				if (throwException)
				{
					throw new ApplicationException("ObjectID='" +obj.ID+ "' has IsItem=true, but it does not implement IMediaItem.");
				}
				return false;
			}			
			else if ((hasParentID) && (obj.ParentID == null))
			{
				if (throwException)
				{
					throw new Error_BadMetadata("ParentID is null when it should be non-null.");
				}
				return false;
			}
			else if ((hasParentID) && (obj.ParentID.Trim() == ""))
			{
				if (throwException)
				{
					throw new Error_BadMetadata("ParentID contains only white-space characters.");
				}
				return false;
			}

			return true;
		}


		/// <summary>
		/// Returns a unique ID for a new media item. 
		/// The ID is guaranteed to be 99% unique because
		/// a seed/guid is generated per process on the
		/// very first time the method is called.
		/// </summary>
		/// <returns></returns>
		public static string GetUniqueId()
		{
			string retVal;
			lock(LockNextID)
			{
				TheNextID++;
				retVal = GetMostRecentUniqueId();
			}
			return retVal;
		}

		/// <summary>
		/// Returns the last value that GetUniqueId() returned.
		/// <para>
		/// This method is useful for application-layer logic
		/// that wishes to serialize a tree of media objects. 
		/// Before serializing the media objects, the application-logic 
		/// should call <see cref="MediaBuilder.GetUniqueId"/>() and serialize/save the
		/// string value.
		/// </para>
		/// <para>
		/// When deserializing the media objects, the application-logic 
		/// should recall the saved string value and then call the 
		/// <see cref="MediaBuilder.PrimeNextId"/>() method. A numeric value
		/// will be parsed from the string and will cause <see cref="MediaBuilder.GetUniqueId"/>()
		/// to issue object IDs that do not conflict with previously issued 
		/// object IDs.
		/// </para>
		/// <para>
		/// Application-logic should take care to call <see cref="MediaBuilder.PrimeNextId"/>()
		/// before new media objects are creaed as object ID collisions can
		/// occur in such a way. 
		/// </para>
		/// </summary>
		/// <returns></returns>
		public static string GetMostRecentUniqueId()
		{
			StringBuilder sb = new StringBuilder();
			lock (LockNextID)
			{
				sb.AppendFormat("{0}{1:X16}", Seed, TheNextID);
			}
			return sb.ToString();
		}

		/// <summary>
		/// This makes it so that the GetUniqueId() starts incrementing
		/// ID's from the provided value. This is very useful in
		/// scenarios where the ID's of items are persisted, and
		/// an application does not wish to repeat IDs.
		/// </summary>
		/// <param name="newBaseID"></param>
		public static void SetNextID (long newBaseID)
		{
			lock(LockNextID)
			{
				TheNextID = newBaseID;
			}
		}
		
		/// <summary>
		/// <para>
		/// Applications that serialize media objects (that were built
		/// with MediaBuilder) should call <see cref="GetMostRecentUniqueId"/>()
		/// to note the last object ID that was issued.
		/// </para>
		/// <para>
		/// Before applications instantiate media objects from
		/// the serialized forms, they should call PrimeNextId().
		/// </para>
		/// <para>
		/// This method is useful for application-layer logic
		/// that wishes to serialize a tree of media objects. 
		/// Before serializing the media objects, the application-logic 
		/// should call <see cref="MediaBuilder.GetUniqueId"/>() and serialize/save the
		/// string value.
		/// </para>
		/// <para>
		/// When deserializing the media objects, the application-logic 
		/// should recall the saved string value and then call the 
		/// <see cref="MediaBuilder.PrimeNextId"/>() method. A numeric value
		/// will be parsed from the string and will cause <see cref="MediaBuilder.GetUniqueId"/>()
		/// to issue object IDs that do not conflict with previously issued 
		/// object IDs.
		/// </para>
		/// <para>
		/// Application-logic should take care to call <see cref="MediaBuilder.PrimeNextId"/>()
		/// before new media objects are created as object ID collisions can
		/// occur in such a way. 
		/// </para>
		/// </summary>
		/// <param name="newBaseID">the objectID from which to start issuing from</param>
		public static void PrimeNextId(string newBaseID)
		{
			lock (LockNextID)
			{
				// lop off the seed
				string rawBaseId = newBaseID.Remove(0, Seed.Length);

				// parse the counter value - it should be stored as a hex number
				long val = long.Parse(rawBaseId, System.Globalization.NumberStyles.HexNumber);

				// set the counter value
				SetNextID(val);
			}
		}

		/// <summary>
		/// Keeps a seed for the object IDs, in the form of the current time.
		/// Allow programmers to read the value of the seed for their own needs.
		/// Values returned from <see cref="GetUniqueID"/>() will return the Seed
		/// value prepended to a long value counter.
		/// <para>
		/// The use of this variable to enforce uniqueness has been deprecated.
		/// Value is always empty string.
		/// </para> 
		/// </summary>
		public readonly static string Seed = "";//"_" + (DateTime.Now.Ticks / 10000).ToString();

		/// <summary>
		/// Used for locking TheNextID for safe thread operations.
		/// </summary>
		public static object LockNextID = new object();
		/// <summary>
		/// A simple counter, appended to the seed.
		/// </summary>
		private static long TheNextID = 0;


		/// <summary>
		/// Creates a 
		/// <see cref="MediaItem"/>
		/// object, given a metadata instantiation
		/// block.
		/// </summary>
		/// <param name="info">
		/// The metadata to use when instantiating the media.
		/// </param>
		/// <returns>a new media item</returns>
		public static MediaItem CreateItem (item info)
		{
			MediaItem newObj = new MediaItem();
			SetObjectProperties(newObj, info);
			return newObj;
		}

		/// <summary>
		/// Creates a 
		/// <see cref="MediaContainer"/>
		/// object, given a metadata instantiation
		/// block.
		/// </summary>
		/// <param name="info">
		/// The metadata to use when instantiating the media.
		/// </param>
		/// <returns>a new media container</returns>
		public static MediaContainer CreateContainer (container info)
		{
			MediaContainer newObj = new MediaContainer();
			SetObjectProperties(newObj, info);
			return newObj;
		}



		/// <summary>
		/// This method can be used to set values in a media object. Be forewarned
		/// that the first thing this method does is erase all of the values that
		/// are in the MediaProperties field of the object. The programmer
		/// must also practice caution for possible threading problems. The programmer
		/// must also take caution in choosing which 
		/// <see cref="MediaBuilder.CoreMetadata"/>
		/// derived class to use when setting the new values.
		/// </summary>
		/// <param name="obj">MediaObject to change</param>
		/// <param name="info">the new metadata to use</param>
		public static void SetObjectProperties(MediaObject obj, CoreMetadata info)
		{
			obj.m_Properties.ClearProperties();

			// Set all of the properties that are not designed to be
			// introspected from the class.
			// ID, 
			if (info.IdIsValid == false)
			{
				info.ID = MediaBuilder.GetUniqueId();
			}

			if ((info.title == null) || (info.title == ""))
			{
				throw new Error_BadMetadata("Title for a media object cannot be null nor empty string.");
			}

			obj.m_ID = info.ID;
			obj.m_Restricted = info.IsRestricted;
			obj.SetClass(info.Class.ToString(), info.Class.FriendlyName);

			System.Type infoType = info.GetType();
			System.Type containerType = typeof(MediaBuilder.container);
			if ((infoType.IsSubclassOf(containerType)) || (infoType == containerType))
			{
				MediaContainer mc = (MediaContainer) obj;
				MediaBuilder.container c = (MediaBuilder.container) info;
				mc.m_Searchable = c.Searchable;
			}


			System.Type type = info.GetType();

			// Figure out if this object has custom metadata
			//System.Type hasCustomMetadata = type.GetInterface(typeof(MediaBuilder.ICustomMetadata).ToString());
			//if (hasCustomMetadata != null)
			if (info is MediaBuilder.ICustomMetadata)
			{
				PropertyInfo propInfo = type.GetProperty("MediaBuilder.ICustomMetadata", BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

				string[] elements = (string[]) propInfo.GetValue(info, null);
				obj.AddDescNode(elements);
			}


			// Introspect the info object's public fields.

			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);

			if (fields != null)
			{
				foreach (FieldInfo fi in fields)
				{
					if (
						(fi.Name != "ID") &&
						(fi.Name != "IsRestricted") &&
						(fi.Name != "CdsFriendlyName")&&
						(fi.Name != "Searchable") &&
						(fi.Name != "IdIsValid") &&
						(fi.Name != "IsRestricted")
						)
					{
						// match the name against a common property name
						CommonPropertyNames property = (CommonPropertyNames) Enum.Parse(typeof(CommonPropertyNames), fi.Name, false);
						string propertyName = T[property];

						object fieldValue = fi.GetValue(info);

						//don't attempt to set null values

						if (fieldValue != null)
						{
							IList listValues = null;
							if (
								(fieldValue is System.Collections.ICollection) ||
								(fieldValue is System.Array)
								)
							{
								ArrayList al = new ArrayList((ICollection) fieldValue);
								listValues = al;							}
							else
							{
								// the value isn't in a list form, so wrap it in a list
								ArrayList al = new ArrayList(1);
								al.Add(fieldValue);
								listValues = al;
							}

							// make sure that any IValueType values in listValues are all type-casted
							// from IValueType to their actual value types if appropriate.
							//Type valueType = listValues[0].GetType().GetInterface(typeof(IValueType).ToString());
							//if (valueType != null)
							if (listValues[0] is CdsMetadata.IValueType)
							{
								ArrayList al = new ArrayList(listValues.Count);
								foreach (IValueType val in listValues)
								{
									if (val.IsValid)
									{
										object theValue = val.Value;

										if (theValue is DateTime)
										{
											if (fi.Name == "date")
											{
												theValue = PropertyDate.EnsureNoTime((DateTime) theValue);
											}
										}

										al.Add(theValue);
									}
								}
								listValues = al;
							}

							// now wrap all of the values in the collection with
							// an ICdsElement type...

							if (listValues.Count > 0)
							{
								bool isMultiple;
								Type propType = PropertyMappings.PropertyNameToType(propertyName, out isMultiple);
							
								Type[] constructorArgTypes = new Type[2];
								constructorArgTypes[0] = typeof(string);
								constructorArgTypes[1] = listValues[0].GetType();
				
								ConstructorInfo ci = propType.GetConstructor(constructorArgTypes);

								if (ci == null)
								{
									throw new Error_NoConstructorForProperty(propertyName, constructorArgTypes[0]);
								}

								ArrayList propValues = new ArrayList(listValues.Count);

								foreach (object val in listValues)
								{
									object[] args = new object[2];
									args[0] = propertyName;
									args[1] = val;

									ICdsElement metadata = (ICdsElement) ci.Invoke (args);

									if (propertyName == T[_UPNP.Class])
									{
										MediaClass mc = (MediaClass) metadata;
										metadata = (ICdsElement) CdsMetadataCaches.MediaClasses.CacheThis(mc.FullClassName, mc.FriendlyName);
									}
									else if (propertyName == T[_UPNP.writeStatus])
									{
										PropertyEnumWriteStatus ws = (PropertyEnumWriteStatus) metadata;
										metadata = (ICdsElement) CdsMetadataCaches.PropertyEnumWriteStatus.CacheThis(T[_UPNP.writeStatus], ws._Value);
									}
									else if (propertyName == T[_UPNP.storageMedium])
									{
										PropertyStorageMedium sm = (PropertyStorageMedium) metadata;
										metadata = (ICdsElement) CdsMetadataCaches.PropertyStorageMediums.CacheThis(T[_UPNP.storageMedium], sm._Value);
									}

									propValues.Add(metadata);
								}

								if ((propValues.Count > 1) && (!isMultiple))
								{
									throw new Error_BadMetadata(propertyName + " is not a property that allows multiple values.");
								}
								obj.m_Properties[propertyName] = propValues;
							}
						}
					}
				}
			}

			// find basic problems like empty title or bad media class
			// and throw exception if error found.
			ValidateMediaObject(obj, false, true);
		}




		/// <summary>
		/// <para>
		/// This class is used by all 
		/// <see cref="MediaBuilder"/>.Create
		/// methods to instantiate objects. Derived classes
		/// can extend the metadata properties applied to the
		/// created media object. 
		/// </para>
		/// <para>
		/// The design of goal of this class is to make it extremely
		/// easy for programmers to derive new media classes
		/// using the standard metadata properties. Classes
		/// that desire to add metadata blocks that are outside
		/// the scope of CDS also need to implement the
		/// <see cref="MediaBuilder.ICustomMetadata"/>
		/// interface.
		/// </para>
		/// <para>
		/// The following rules apply when creating derived classes.
		/// <list type="bullet">
		/// <item><description>
		/// Metadata properties that are covered by standard UPNP-AV/ContentDirectory
		/// match to values in <see cref="CommonPropertyNames"/>.
		/// Derived classes must declare public field members that have
		/// a corresponding name from the enumeration, in order to set
		/// a value for that enumeration.
		/// </description></item>
		/// <item><description>
		/// Derived classes must choose a base class of 
		/// <see cref="MediaBuilder.item"/>
		/// or <see cref="MediaBuilder.container"/> or
		/// an existing class that is derived from those classes.
		/// The name of the programmer-defined derived class will determine the name of the media class.
		/// The Class property on the instance will properly reflect the pathing,
		/// which is derived from the introspected class hierarchy of the instance.
		/// </description></item>
		/// <item><description>
		/// Derived classes must declare a public new string 
		/// called CdsFriendly, describing the friendly name for the media class.
		/// </description></item>
		/// <item><description>
		/// Any metadata that is not covered by the 
		/// <see cref="CommonPropertyNames"/> 
		/// enumerator must be set through an array of
		/// XmlElement objects, through implementing the
		/// <see cref="MediaBuilder.ICustomMetadata"/>
		/// interface.
		/// </description></item>
		/// <item><description>
		/// The programmer must ensure that the C# data type
		/// chosen to represent the a field properly prints
		/// a value when the ToString() method is called.
		/// </description></item>
		/// <item><description>
		/// The programmer must ensure that the C# data type
		/// chosen to represent the a field properly matches
		/// the data type specified by the UPNP-AV/ContentDirectory 
		/// specification.
		/// </description></item>
		/// <item><description>
		/// Metadata properties that allow multiple values
		/// must be strongly typed as an array of a C# data type.
		/// For example, the "genre" field allows multiple values
		/// so it uses a string[] for in its type declaration.
		/// </description></item>
		/// </list>
		/// </para>
		/// </summary>
		public abstract class CoreMetadata
		{
			/// <summary>
			/// Optional friendly name for Cds, but required for this 
			/// (and every derived) class.
			/// </summary>
			public string CdsFriendlyName = "Generic Object";

			/// <summary>
			/// Programmers should not instantiate CoreMetadata objects like this,
			/// but we do provide a static instance for easily obtaining the media type.
			/// </summary>
			public CoreMetadata(){}

			/// <summary>
			/// CoreMetadata items need a title.
			/// </summary>
			/// <param name="title"></param>
			public CoreMetadata (string title)
			{
				this.title = title;
			}

			/// <summary>
			/// The <see cref="MediaBuilder"/>.Create methods
			/// use this field to determine if a unique ID should be generated.
			/// Default value is false.
			/// This value is directly tapped and not introspected.
			/// </summary>
			public bool IdIsValid = false;

			/// <summary>
			/// Indicates if the media object can be modified by a control point.
			/// This value is directly tapped and not introspected.
			/// </summary>
			public bool IsRestricted = true;

			/// <summary>
			/// If IdIsValid is true, then this ID is applied to the generated
			/// media object.
			/// </summary>
			public string ID = null;

			/// <summary>
			/// Default title is "". This metadata is always applied.
			/// </summary>
			public string title = null;

			/// <summary>
			/// Default creator is "". This metadata is optional.
			/// </summary>
			public string creator = null;

			/// <summary>
			/// Indicates if the media object's resources can be overwritten.
			/// </summary>
			public EnumWriteStatus writeStatus = EnumWriteStatus.UNKNOWN;

			/// <summary>
			/// Returns the media class of a CoreMetadata instance.
			/// This value is introspected determined by the class name that
			/// was introspected from the actual derived instance.
			/// </summary>
			public MediaClass Class
			{
				get
				{
					// introspect the class and build a media class string
					System.Type type = this.GetType();
					Stack names = new System.Collections.Stack();
					FieldInfo fi;
					string name;
					while (true)
					{
						name = type.Name;

						if (type == typeof(MediaBuilder.CoreMetadata)) 
						{
							name = "object";
							names.Push(name);
							break;
						}
						else
						{
							names.Push(name);
						}

						type = type.BaseType;
					}

					StringBuilder mediaClass = null;
					while (names.Count > 0)
					{
						name = (string) names.Pop();
						if (mediaClass == null)
						{
							mediaClass = new StringBuilder(names.Count * 8);
							mediaClass.Append(name);
						}
						else
						{
							mediaClass.AppendFormat(".{0}", name);
						}
					}

					// introspect the class and get a friendly name
					type = this.GetType();
					try
					{
						fi = type.GetField("CdsFriendlyName");
						name = (string) fi.GetValue(this);
					}
					catch
					{
						name = "";
					}

					MediaClass mc = CdsMetadataCaches.MediaClasses.CacheThis(mediaClass.ToString(), name);
					return mc;
				} 
			}
		}


		/// <summary>
		/// Represents an object.item media.
		/// </summary>
		public class item : CoreMetadata
		{
			/// <summary>
			/// Optional friendly name for Cds, but required for this 
			/// (and every derived) class.
			/// </summary>
			public new string CdsFriendlyName = "Generic Item";

			/// <summary>
			/// Public programmers should not use this, but we do provide
			/// a static instance to ease the acquisition of the media type
			/// for a particular class.
			/// </summary>
			public item() {}
			/// <summary>
			/// Public programmers must specify a title for the item.
			/// </summary>
			/// <param name="title"></param>
			public item(string title) : base (title) {}

		}

		/// <summary>
		/// Represents an object.item.imageItem media.
		/// </summary>
		public class imageItem : item
		{
			/// <summary>
			/// Optional friendly name for Cds, but required for this 
			/// (and every derived) class.
			/// </summary>
			public new string CdsFriendlyName = "Image Item";
			/// <summary>
			/// Public programmers should not use this.
			/// </summary>
			public imageItem() {}
			/// <summary>
			/// Public programmers need to specify a title.
			/// </summary>
			/// <param name="title">title of image</param>
			public imageItem (string title) : base(title) {}
			/// <summary>
			/// long description of the image
			/// </summary>
			public string longDescription;
			/// <summary>
			/// the storage medium where the image is stored
			/// </summary>
			public StorageMediums.Enum storageMedium = StorageMediums.Enum.UNKNOWN;
			/// <summary>
			/// the parental rating of the image, values not standardized
			/// </summary>
			public string rating;
			/// <summary>
			/// short description of the image
			/// </summary>
			public string description;
			/// <summary>
			/// The publishing company
			/// </summary>
			public string publisher;
			/// <summary>
			/// Date when image was published
			/// </summary>
			public _DateTime date;
			/// <summary>
			/// Copyright info
			/// </summary>
			public string[] rights;
		}

		/// Represents an object.item.imageItem.photo media.
		public class photo : imageItem
		{
			/// <summary>
			/// Optional friendly name for Cds, but required for this 
			/// (and every derived) class.
			/// </summary>
			public new string CdsFriendlyName = "Photograph";
			/// <summary>
			/// Public programmers should not use this constructor.
			/// </summary>
			public photo() {}
			/// <summary>
			/// Public programmers need to specify a title.
			/// </summary>
			/// <param name="title">title of photo</param>
			public photo(string title) : base(title) {}
			/// <summary>
			/// The album to which the photo belongs to
			/// </summary>
			public string[] album; 
		}

		/// <summary>
		/// Represents an object.item.audioItem media.
		/// </summary>
		public class audioItem : item
		{
			/// <summary>
			/// Optional friendly name for Cds, but required for this 
			/// (and every derived) class.
			/// </summary>
			public new string CdsFriendlyName = "Audio Item";
			/// <summary>
			/// Public programmers should not use this constructor.
			/// </summary>
			public audioItem() {}

			/// <summary>
			/// Public programmers need to specify a title.
			/// </summary>
			/// <param name="title">the title of the item</param>
			public audioItem(string title) : base(title) {}
			/// <summary>
			/// a listing of genres
			/// </summary>
			public string[] genre;

			/// <summary>
			/// a short description
			/// </summary>
			public string description;
			/// <summary>
			/// a long description
			/// </summary>
			public string longDescription;
			/// <summary>
			/// publishing company
			/// </summary>
			public string publisher;
			/// <summary>
			/// language of the content
			/// </summary>
			public string language;
			/// <summary>
			/// URIs where more info can be found
			/// </summary>
			public Uri[] relation;
			/// <summary>
			/// Copyright info
			/// </summary>
			public string[] rights;
		}

		/// <summary>
		/// Represents an object.item.audioItem.musicTrack media.
		/// </summary>
		public class musicTrack : audioItem
		{
			/// <summary>
			/// Optional friendly name for Cds, but required for this 
			/// (and every derived) class.
			/// </summary>
			public new string CdsFriendlyName = "Music Track";
			/// <summary>
			/// Public programmers should not use this constructor.
			/// </summary>
			public musicTrack() {}
			/// <summary>
			/// Public programmers need to specify a title.
			/// </summary>
			/// <param name="title">the title of the item</param>
			public musicTrack(string title) : base(title) {}
			/// <summary>
			/// A listing of artists for the content.
			/// </summary>
			public PersonWithRole[] artist;
			/// <summary>
			/// The album where the song came from
			/// </summary>
			public string[] album;
			/// <summary>
			/// The original track number on the album.
			/// Only allows 1 value.
			/// </summary>
			public _Int originalTrackNumber;
			/// <summary>
			/// A listing of playlists where the song can be heard.
			/// </summary>
			public string[] playlist;
			/// <summary>
			/// indication of the song's storage medium
			/// </summary>
			public StorageMediums.Enum storageMedium = StorageMediums.Enum.UNKNOWN;
			/// <summary>
			/// a listing of contributors that made this song
			/// </summary>
			public string[] contributor;
			/// <summary>
			/// The date when it was published, in ISO 8601: YYY-MM-DD.
			/// </summary>
			public _DateTime date;
		}

		/// <summary>
		/// Represents an object.item.audioItem.audioBroadcast media.
		/// </summary>
		public class audioBroadcast : audioItem
		{
			/// <summary>
			/// Optional friendly name for Cds, but required for this 
			/// (and every derived) class.
			/// </summary>
			public new string CdsFriendlyName = "Audio Broadcast";
			/// <summary>
			/// Public programmers should not use this constructor.
			/// </summary>
			public audioBroadcast() {}
			/// <summary>
			/// Public programmers need to specify a title.
			/// </summary>
			/// <param name="title">the title of the item</param>
			public audioBroadcast(string title) : base(title) {}

			/// <summary>
			/// Region - not standardized by CDS
			/// </summary>
			public string region;

			/// <summary>
			/// Radio CallSign (like KUFO, KPAM, etc) - not standardized by CDS.
			/// </summary>
			public string radioCallSign;

			/// <summary>
			/// frequency identification, like 1080AM or 107.4FM - not standardized by CDS.
			/// </summary>
			public string radioBand;

			/// <summary>
			/// identification of the tuner channel or info associated with a piece of recorded content
			/// Only allows 1 value.
			/// </summary>
			public _Int channelNr;
		}

		/// <summary>
		/// Represents an object.item.audioItem.audioBook media.
		/// </summary>
		public class audioBook : audioItem
		{
			internal audioBook() {}

			/// <summary>
			/// Requires title
			/// </summary>
			/// <param name="title"></param>
			public audioBook(string title) : base(title) {}

			/// <summary>
			/// Optional friendly name for Cds, but required for this 
			/// (and every derived) class.
			/// </summary>
			public new string CdsFriendlyName = "Audio Book";
			
			/// <summary>
			/// indication of the song's storage medium
			/// </summary>
			public StorageMediums.Enum storageMedium = StorageMediums.Enum.UNKNOWN;
			/// <summary>
			/// producer for this audio book
			/// </summary>
			public string producer;
			/// <summary>
			/// a listing of contributors that made this song
			/// </summary>
			public string[] contributor;
			/// <summary>
			/// The date when it was published, in ISO 8601: YYY-MM-DD.
			/// </summary>
			public _DateTime date;
		}

		/// <summary>
		/// Represents an object.item.videoItem media.
		/// </summary>
		public class videoItem : item
		{
			/// <summary>
			/// Optional friendly name for Cds, but required for this 
			/// (and every derived) class.
			/// </summary>
			public new string CdsFriendlyName = "Video Item";
			public videoItem(){}

			public string[] genre;
			public string longDescription;
			public string producer;
			public string rating;
			public PersonWithRole[] actor;
			public string director;
			public string description;
			public string publisher;
			public CultureInfo language;
			public Uri relation;
			public videoItem(string title) : base(title) {}
		}

		/// <summary>
		/// Represents an object.item.videoItem.movie media.
		/// </summary>
		public class movie: videoItem
		{
			/// <summary>
			/// Optional friendly name for Cds, but required for this 
			/// (and every derived) class.
			/// </summary>
			public new string CdsFriendlyName = "Movie Item";
			public movie(){}

			public StorageMediums.Enum storageMedium = StorageMediums.Enum.UNKNOWN;
			/// <summary>
			/// Only allows 1 value
			/// </summary>
			public _Int DVDRegionCode;
			public string channelName;
			/// <summary>
			/// Only allows 1 value
			/// </summary>
			public _DateTime scheduledStartTime;
			/// <summary>
			/// Only allows 1 value
			/// </summary>
			public _DateTime scheduledEndTime;
			public movie(string title) : base(title) {}
		}

		/// <summary>
		/// Represents an object.item.videoItem.videoBroadcast media.
		/// </summary>
		public class videoBroadcast: videoItem
		{
			/// <summary>
			/// Optional friendly name for Cds, but required for this 
			/// (and every derived) class.
			/// </summary>
			public new string CdsFriendlyName = "Video Broadcast";
			public Uri icon;
			public string region;
			/// <summary>
			/// allows only 1 value.
			/// </summary>
			public _Int channelNr;
			public videoBroadcast(){}
			public videoBroadcast(string title) : base(title) {}
		}

		/// <summary>
		/// Represents an object.item.videoItem.musicVideoClip media.
		/// </summary>
		public class musicVideoClip: videoItem
		{
			/// <summary>
			/// Optional friendly name for Cds, but required for this 
			/// (and every derived) class.
			/// </summary>
			public new string CdsFriendlyName = "Music Video Clip";
			public PersonWithRole[] artist;
			public StorageMediums.Enum storageMedium = StorageMediums.Enum.UNKNOWN;
			public string album;
			/// <summary>
			/// Allows only 1 value;
			/// </summary>
			public _DateTime scheduledStartTime;
			/// <summary>
			/// Allows only 1 value
			/// </summary>
			public _DateTime scheduledEndTime;
			public string[] contributor;
			/// <summary>
			/// Allows only one value
			/// </summary>
			public _DateTime date;
			public musicVideoClip(){}
			public musicVideoClip(string title) : base(title) {}
		}

		/// <summary>
		/// Represents an object.item.playlistItem media.
		/// </summary>
		public class playlistItem : item
		{
			/// <summary>
			/// Optional friendly name for Cds, but required for this 
			/// (and every derived) class.
			/// </summary>
			public new string CdsFriendlyName = "Playlist Item";
			public PersonWithRole[] artist;
			public string[] genre;
			public string longDescription;
			public StorageMediums.Enum storageMedium = StorageMediums.Enum.UNKNOWN;
			public string description;
			public _DateTime date;
			public CultureInfo language;
			public playlistItem(){}
			public playlistItem(string title) : base(title) {}
		}
		
		/// <summary>
		/// Represents an object.item.textItem media.
		/// </summary>
		public class textItem : item
		{
			/// <summary>
			/// Optional friendly name for Cds, but required for this 
			/// (and every derived) class.
			/// </summary>
			public new string CdsFriendlyName = "Text Document";
			public PersonWithRole[] author;
			public string protection;
			public string longDescription ;
			public StorageMediums.Enum storageMedium = StorageMediums.Enum.UNKNOWN;
			public string rating;
			public string description;
			public string publisher;
			public string[] contributor;
			public _DateTime date;
			public Uri relation;
			public CultureInfo language;
			public string[] rights;
			public textItem(){}
			public textItem(string title) : base(title) {}
		}


		/// <summary>
		/// Represents an object.container media.
		/// </summary>
		public class container : CoreMetadata
		{
			/// <summary>
			/// Optional friendly name for Cds, but required for this 
			/// (and every derived) class.
			/// </summary>
			public new string CdsFriendlyName = "Generic Container";

			/// <summary>
			/// Public programmers should not use this, but we do provide
			/// a static instance to ease the acquisition of the media type
			/// for a particular class.
			/// </summary>
			public container() {}
			/// <summary>
			/// Public programmers must specify a title for the item.
			/// </summary>
			/// <param name="title"></param>
			public container(string title) : base (title) {}
			
			/// <summary>
			/// Indicates if the container should be searchable when advertised.
			/// This value is directly tapped, and not introspected.
			/// </summary>
			public bool Searchable;
		}

		/// <summary>
		/// Represents an object.container.person media.
		/// </summary>
		public class person : container
		{
			/// <summary>
			/// Optional friendly name for Cds, but required for this 
			/// (and every derived) class.
			/// </summary>
			public new string CdsFriendlyName = "Person Container";
			public person(){}
			public person(string title) : base(title) {}
		}

		/// <summary>
		/// Represents an object.container.person.musicArtist media.
		/// </summary>
		public class musicArtist : person
		{
			/// <summary>
			/// Optional friendly name for Cds, but required for this 
			/// (and every derived) class.
			/// </summary>
			public new string CdsFriendlyName = "Music Artist";

			public CultureInfo language;
			public string[] genre;
			public Uri artistDiscographyURI;
			public musicArtist(){}
			public musicArtist(string title) : base(title) {}
		}

		/// <summary>
		/// Represents an object.container.playlistContainer media.
		/// </summary>
		public class playlistContainer : container
		{
			/// <summary>
			/// Optional friendly name for Cds, but required for this 
			/// (and every derived) class.
			/// </summary>
			public new string CdsFriendlyName = "Playlists";
			public PersonWithRole[] artist;
			public string[] genre;
			public string longDescription;
			public string producer;
			public StorageMediums.Enum storageMedium = StorageMediums.Enum.UNKNOWN;
			public string description;
			public string contributor;
			public _DateTime date;
			public CultureInfo language;
			public string[] rights;
			public playlistContainer(){}
			public playlistContainer(string title) : base(title) {}
		}

		/// <summary>
		/// Represents an object.container.album media.
		/// </summary>
		public class album : container
		{
			/// <summary>
			/// Optional friendly name for Cds, but required for this 
			/// (and every derived) class.
			/// </summary>
			public new string CdsFriendlyName = "Generic Album";
			
			public StorageMediums.Enum storageMedium = StorageMediums.Enum.UNKNOWN;
			public string longDescription;
			public string description;
			public string publisher;
			public string constributor;
			public _DateTime date;
			public Uri relation;
			public string[] rights;
			public album(){}
			public album(string title) : base(title) {}
		}

		/// <summary>
		/// Represents an object.container.album.musicAlbum media.
		/// </summary>
		public class musicAlbum : album
		{
			/// <summary>
			/// Optional friendly name for Cds, but required for this 
			/// (and every derived) class.
			/// </summary>
			public new string CdsFriendlyName = "Music Album";
			
			public PersonWithRole[] artist;
			public string[] genre;
			public string producer;
			public Uri albumArtURI;
			public string toc;
			public musicAlbum(){}
			public musicAlbum(string title) : base(title) {}
		}

		/// <summary>
		/// Represents an object.container.photoAlbum media.
		/// </summary>
		public class photoAlbum : album
		{
			/// <summary>
			/// Optional friendly name for Cds, but required for this 
			/// (and every derived) class.
			/// </summary>
			public new string CdsFriendlyName = "Photo Album";
			public photoAlbum(){}
			public photoAlbum(string title) : base(title) {}
		}

		/// <summary>
		/// Represents an object.container.genre media.
		/// </summary>
		public class genre : container
		{
			/// <summary>
			/// Optional friendly name for Cds, but required for this 
			/// (and every derived) class.
			/// </summary>
			public new string CdsFriendlyName = "Genre Container";
			public string longDescription;
			public string description;
			public genre(){}
			public genre(string title) : base(title) {}
		}

		/// <summary>
		/// Represents an object.container.genre.musicGenre media.
		/// </summary>
		public class musicGenre : genre
		{
			/// <summary>
			/// Optional friendly name for Cds, but required for this 
			/// (and every derived) class.
			/// </summary>
			public new string CdsFriendlyName = "Music Genre";
			public musicGenre(){}
			public musicGenre(string title) : base(title) {}
		}

		/// <summary>
		/// Represents an object.container.genre.movieGenre media.
		/// </summary>
		public class movieGenre : genre
		{
			/// <summary>
			/// Optional friendly name for Cds, but required for this 
			/// (and every derived) class.
			/// </summary>
			public new string CdsFriendlyName = "Movie Genre";
			public movieGenre(){}
			public movieGenre(string title) : base(title) {}
		}

		/// <summary>
		/// Represents an object.container.storageFolder media.
		/// </summary>
		public class storageFolder : container
		{
			/// <summary>
			/// Optional friendly name for Cds, but required for this 
			/// (and every derived) class.
			/// </summary>
			public new string CdsFriendlyName = "Storage Folder";
			public _Long storageUsed = new _Long(-1);
			public storageFolder(){}
			public storageFolder(string title) : base(title) {}
		}

		/// <summary>
		/// Represents an object.container.storageVolume media.
		/// </summary>
		public class storageVolume : container
		{
			/// <summary>
			/// Optional friendly name for Cds, but required for this 
			/// (and every derived) class.
			/// </summary>
			public new string CdsFriendlyName = "Storage Volume";
			public _Long storageTotal = new _Long(-1);
			public _Long storageUsed = new _Long(-1);
			public _Long storageFree = new _Long(-1);
			public StorageMediums.Enum storageMedium = StorageMediums.Enum.UNKNOWN;
			public storageVolume(){}
			public storageVolume(string title) : base(title) {}
		}

		/// <summary>
		/// Represents an object.container.storageSystem media.
		/// </summary>
		public class storageSystem : container
		{
			/// <summary>
			/// Optional friendly name for Cds, but required for this 
			/// (and every derived) class.
			/// </summary>
			public new string CdsFriendlyName = "Storage System";
			public _Long storageTotal = new _Long(-1);
			public _Long storageUsed = new _Long(-1);
			public _Long storageFree = new _Long(-1);
			public _Long storageMaxPartition = new _Long(-1);
			public StorageMediums.Enum storageMedium = StorageMediums.Enum.UNKNOWN;			
			public storageSystem(){}
			public storageSystem(string title) : base(title) {}
		}

		/// <summary>
		/// Gets a static instance that allows easy translation
		/// of CommonPropertyNames enumeration into ContentDirectory element
		/// names and attributes.
		/// </summary>
		private static Tags T = Tags.GetInstance();
		/// <summary>
		/// Allows easy accessing of storage medium enumeration.
		/// </summary>
		private static StorageMediums sm = new StorageMediums();


		/// <summary>
		/// Keeps a static version of metadata block for populating
		/// <see cref="MediaBuilder.StandardMediaClasses"/>.
		/// </summary>
		private readonly static item S_Item = new item();
		/// <summary>
		/// Keeps a static version of metadata block for populating
		/// <see cref="MediaBuilder.StandardMediaClasses"/>.
		/// </summary>
		private readonly static container S_Container = new container();
		/// <summary>
		/// Keeps a static version of metadata block for populating
		/// <see cref="MediaBuilder.StandardMediaClasses"/>.
		/// </summary>
		private readonly static storageFolder S_StorageFolder = new storageFolder();
		/// <summary>
		/// Keeps a static version of metadata block for populating
		/// <see cref="MediaBuilder.StandardMediaClasses"/>.
		/// </summary>
		private readonly static storageSystem S_StorageSystem = new storageSystem();
		/// <summary>
		/// Keeps a static version of metadata block for populating
		/// <see cref="MediaBuilder.StandardMediaClasses"/>.
		/// </summary>
		private readonly static storageVolume S_StorageVolume = new storageVolume();
		/// <summary>
		/// Keeps a static version of metadata block for populating
		/// <see cref="MediaBuilder.StandardMediaClasses"/>.
		/// </summary>
		private readonly static album S_Album = new album();
		/// <summary>
		/// Keeps a static version of metadata block for populating
		/// <see cref="MediaBuilder.StandardMediaClasses"/>.
		/// </summary>
		private readonly static audioBook S_AudioBook = new audioBook();
		/// <summary>
		/// Keeps a static version of metadata block for populating
		/// <see cref="MediaBuilder.StandardMediaClasses"/>.
		/// </summary>
		private readonly static genre S_Genre= new genre();
		/// <summary>
		/// Keeps a static version of metadata block for populating
		/// <see cref="MediaBuilder.StandardMediaClasses"/>.
		/// </summary>
		private readonly static movieGenre S_MovieGenre = new movieGenre();
		/// <summary>
		/// Keeps a static version of metadata block for populating
		/// <see cref="MediaBuilder.StandardMediaClasses"/>.
		/// </summary>
		private readonly static musicGenre S_MusicGenre = new musicGenre();
		/// <summary>
		/// Keeps a static version of metadata block for populating
		/// <see cref="MediaBuilder.StandardMediaClasses"/>.
		/// </summary>
		private readonly static musicVideoClip S_MusicVideoClip = new musicVideoClip();
		/// <summary>
		/// Keeps a static version of metadata block for populating
		/// <see cref="MediaBuilder.StandardMediaClasses"/>.
		/// </summary>
		private readonly static person S_Person = new person();
		/// <summary>
		/// Keeps a static version of metadata block for populating
		/// <see cref="MediaBuilder.StandardMediaClasses"/>.
		/// </summary>
		private readonly static photoAlbum S_PhotoAlbum = new photoAlbum();
		/// <summary>
		/// Keeps a static version of metadata block for populating
		/// <see cref="MediaBuilder.StandardMediaClasses"/>.
		/// </summary>
		private readonly static musicAlbum S_MusicAlbum = new musicAlbum();
		/// <summary>
		/// Keeps a static version of metadata block for populating
		/// <see cref="MediaBuilder.StandardMediaClasses"/>.
		/// </summary>
		private readonly static playlistContainer S_PlaylistContainer = new playlistContainer();
		/// <summary>
		/// Keeps a static version of metadata block for populating
		/// <see cref="MediaBuilder.StandardMediaClasses"/>.
		/// </summary>
		private readonly static playlistItem S_PlaylistItem = new playlistItem();
		/// <summary>
		/// Keeps a static version of metadata block for populating
		/// <see cref="MediaBuilder.StandardMediaClasses"/>.
		/// </summary>
		private readonly static videoBroadcast S_VideoBroadcast = new videoBroadcast();
		/// <summary>
		/// Keeps a static version of metadata block for populating
		/// <see cref="MediaBuilder.StandardMediaClasses"/>.
		/// </summary>
		private readonly static audioItem S_AudioItem = new audioItem();
		/// <summary>
		/// Keeps a static version of metadata block for populating
		/// <see cref="MediaBuilder.StandardMediaClasses"/>.
		/// </summary>
		private readonly static audioBroadcast S_AudioBroadcast = new audioBroadcast();
		/// <summary>
		/// Keeps a static version of metadata block for populating
		/// <see cref="MediaBuilder.StandardMediaClasses"/>.
		/// </summary>
		private readonly static musicTrack S_MusicTrack = new musicTrack();
		/// <summary>
		/// Keeps a static version of metadata block for populating
		/// <see cref="MediaBuilder.StandardMediaClasses"/>.
		/// </summary>
		private readonly static videoItem S_VideoItem = new videoItem();
		/// <summary>
		/// Keeps a static version of metadata block for populating
		/// <see cref="MediaBuilder.StandardMediaClasses"/>.
		/// </summary>
		private readonly static movie S_Movie = new movie();
		/// <summary>
		/// Keeps a static version of metadata block for populating
		/// <see cref="MediaBuilder.StandardMediaClasses"/>.
		/// </summary>
		private readonly static imageItem S_ImageItem = new imageItem();
		/// <summary>
		/// Keeps a static version of metadata block for populating
		/// <see cref="MediaBuilder.StandardMediaClasses"/>.
		/// </summary>
		private readonly static photo S_Photo = new photo();
		/// <summary>
		/// Keeps a static version of metadata block for populating
		/// <see cref="MediaBuilder.StandardMediaClasses"/>.
		/// </summary>
		private readonly static textItem S_TextItem = new textItem();

		/// <summary>
		/// Lists a static link to the media classes defined by the ContentDirectory spec. 
		/// </summary>
		public class StandardMediaClasses
		{
			/// <summary>
			/// MediaClass for an object.item.
			/// </summary>
			public readonly static MediaClass Item = S_Item.Class;
			/// <summary>
			/// MediaClass for an object.container.
			/// </summary>
			public readonly static MediaClass Container = S_Container.Class;
			/// <summary>
			/// MediaClass for an object.container.storageFolder
			/// </summary>
			public readonly static MediaClass StorageFolder = S_StorageFolder.Class;
			/// <summary>
			/// MediaClass for an object.container.storageSystem
			/// </summary>
			public readonly static MediaClass StorageSystem = S_StorageSystem.Class;
			/// <summary>
			/// MediaClass for an object.container.storageVolume
			/// </summary>
			public readonly static MediaClass StorageVolume = S_StorageVolume.Class;
			/// <summary>
			/// MediaClass for an object.container.person
			/// </summary>
			public readonly static MediaClass Person = S_Person.Class;
			/// <summary>
			/// MediaClass for an object.container.playlistContainer
			/// </summary>
			public readonly static MediaClass PlaylistContainer = S_PlaylistContainer.Class;
			/// <summary>
			/// MediaClass for an object.container.playlistItem
			/// </summary>
			public readonly static MediaClass PlaylistItem = S_PlaylistItem.Class;
			/// <summary>
			/// MediaClass for an object.container.album
			/// </summary>
			public readonly static MediaClass Album = S_Album.Class;
			/// <summary>
			/// MediaClass for an object.container.album.musicAlbum
			/// </summary>
			public readonly static MediaClass MusicAlbum = S_MusicAlbum.Class;
			/// <summary>
			/// MediaClass for an object.container.
			/// </summary>
			public readonly static MediaClass PhotoAlbum = S_PhotoAlbum.Class;
			/// <summary>
			/// MediaClass for an object.container.
			/// </summary>
			public readonly static MediaClass Genre = S_Genre.Class;
			/// <summary>
			/// MediaClass for an object.container.
			/// </summary>
			public readonly static MediaClass MusicGenre = S_MusicGenre.Class;
			/// <summary>
			/// MediaClass for an object.container.
			/// </summary>
			public readonly static MediaClass MovieGenre = S_MovieGenre.Class;
			/// <summary>
			/// MediaClass for an object.item.audioItem
			/// </summary>
			public readonly static MediaClass AudioItem = S_AudioItem.Class;
			/// <summary>
			/// MediaClass for an object.item.audioItem.musicTrack
			/// </summary>
			public readonly static MediaClass MusicTrack = S_MusicTrack.Class;
			/// <summary>
			/// MediaClass for an object.item.audioItem.audioBook
			/// </summary>
			public readonly static MediaClass AudioBook = S_AudioBook.Class;
			/// <summary>
			/// MediaClass for an object.item.audioItem.audioBroadcast
			/// </summary>
			public readonly static MediaClass AudioBroadcast = S_AudioBroadcast.Class;
			/// <summary>
			/// MediaClass for an object.item.videoItem
			/// </summary>
			public readonly static MediaClass VideoItem = S_VideoItem.Class;
			/// <summary>
			/// MediaClass for an object.item.videoItem.movie
			/// </summary>
			public readonly static MediaClass Movie = S_Movie.Class;
			/// <summary>
			/// MediaClass for an object.item.videoItem.videoBroadcast
			/// </summary>
			public readonly static MediaClass VideoBroadcast = S_VideoBroadcast.Class;
			/// <summary>
			/// MediaClass for an object.item.videoItem.musicVideoClip
			/// </summary>
			public readonly static MediaClass MusicVideoClip = S_MusicVideoClip.Class;
			/// <summary>
			/// MediaClass for an object.item.imageItem
			/// </summary>
			public readonly static MediaClass ImageItem = S_ImageItem.Class;
			/// <summary>
			/// MediaClass for an object.item.imageItem.Photo
			/// </summary>
			public readonly static MediaClass Photo = S_Photo.Class;
			/// <summary>
			/// MediaClass for an object.item.textItem
			/// </summary>
			public readonly static MediaClass TextItem = S_TextItem.Class;

		}

	}
	
}
