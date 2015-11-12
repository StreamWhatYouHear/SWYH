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
using System.Net;
using System.Text;
using System.Xml;
using OpenSource.UPnP;
using System.Threading;
using System.Reflection;
using OpenSource.UPnP.AV;
using System.Collections;
using OpenSource.Utilities;
using System.Text.RegularExpressions;

namespace OpenSource.UPnP.AV.CdsMetadata
{
	/// <summary>
	/// <para>
	/// This class takes the a metadata property in the form
	/// "[namespace]:[element name]@[attribute]" 
	/// and attempts to return a list of values associated with that metadata property
	/// by extracting it from an <see cref="IUPnPMedia"/> object.
	/// The "@[attribute]" portion can be omitted if an attribute property is not desired.
	/// If the DIDL namespace is desired, the "[namespace]:" portion is omitted.
	/// If the DIDL namespace is desired and any DIDL element is allowed, then the 
	/// "[namespace]:[element]" portion is omitted.
	/// </para>
	/// 
	/// <para>
	/// The "[namespace]:[element]" portion can be obtained using the
	/// <see cref="Tags"/>'s indexer with the
	/// <see cref="CommonPropertyNames"/>, 
	/// <see cref="_DC"/>, or
	/// <see cref="_UPNP"/>, enumerators, whilst
	/// the "[attribute]" portion can be obtained from using the 
	/// <see cref="Tags"/>'s indexer with the
	/// <see cref="_ATTRIB"/> 
	/// enumerator.
	/// </para>
	/// 
	/// <para>
	/// UPNP-AV valid property attributes can be also be found at
	/// <see cref="Tags.PropertyAttributes"/>
	/// as a series of public const strings.
	/// </para>
	/// </summary>
	public class TagExtractor
	{
		/// <summary>
		/// Constructs an instance designed to extract a specific metadata property
		/// from an <see cref="IUPnPMedia"/> object
		/// </summary>
		/// <param name="property">
		/// string formatted in the form "[namespace]:
		/// </param>
		/// <exception cref="OpenSource.UPnP.AV.CdsMetadata.Error_InvalidAttribute">
		/// Thrown if the provided property maps to an invalid attribute,
		/// as per ContentDirectory metadata conventions.
		/// </exception>
		public TagExtractor(string property)
		{
			// obtain the specified namespace, tagname, and attribute name
			// for the metadata that needs to be extracted
			ParseNsTagAttrib(property, out m_Ns, out m_Tag, out m_Attrib);
			
			bool checkedTags = false;

			// Do some preprocessing that will properly set all of
			// the m_SearchXXX booleans to true to properly
			// indicate what objects the tag extractor should
			// examine.

			if (m_Ns == "")
			{
				// Blank namespace assumes DIDL-Lite top level tags.
				// Attempt to match the tag name against one of the
				// top level DIDL-Lite top level tags. Also accept
				// the case where the tag name is not specified,
				// in which case we assume any top-level DIDL-Lite
				// element. 
				switch (m_Tag)
				{
						// search item objects
					case "item":
						this.m_SearchItem = true;
						break;

						// search container objects
					case "container":
						this.m_SearchContainer = true;
						break;

						// search resource objects
					case "res":
						this.m_SearchRes = true;
						break;

						// search any DIDL top level object?
					case "":
						if (
							(string.Compare(this.m_Attrib, T[_ATTRIB.id]) == 0) ||
							(string.Compare(this.m_Attrib, T[_ATTRIB.restricted]) == 0) ||
							(string.Compare(this.m_Attrib, T[_ATTRIB.parentID]) == 0) ||
							(string.Compare(this.m_Attrib, T[_ATTRIB.childCount]) == 0) ||
							(string.Compare(this.m_Attrib, T[_ATTRIB.refID]) == 0)
							)
						{
							// limit the search to item and container objects
							checkedTags = true;
							this.m_SearchItem = true;
							this.m_SearchContainer = true;
						}
						else if (string.Compare(this.m_Attrib, T[_ATTRIB.searchable]) == 0)
						{
							// limit the search to container objects
							checkedTags = true;
							this.m_SearchContainer = true;
						}
						else
						{
							// limit the search to resource objects
							this.m_SearchRes = true;
						}
						break;
				}
				
				if (! (this.m_SearchItem || this.m_SearchContainer || this.m_SearchRes) )
				{
					this.m_SearchDesc = true;
				}

			}
			else if ((m_Ns == "dc") || (m_Ns == "upnp"))
			{
				// if the namespace is dublin-core or upnp, then
				// we can't be searching vendor-specific metadata.
				this.m_SearchDesc = false;
			}

			// At this point, we're either configuring to extract
			// custom metadata or we're configuring to extract
			// CDS normative metadata.

			if (this.m_SearchDesc == false)
			{
				// If extracting CDS normative metadata...

				if (this.m_SearchItem || this.m_SearchContainer)
				{
					// If we're searching item and container elements

					if (!checkedTags)
					{
						// If by chance we haven't examined the attribute
						// that is to be extracted, then check the
						// attribute against the CDS normative attributes
						// for item and container elements.

//						string[] attribs = { T[_ATTRIB.id], T[_ATTRIB.parentID], T[_ATTRIB.restricted] };
//
//						bool c = this.CheckAttribute(this.m_Ns, this.m_Tag, this.m_Attrib, attribs);
//						if (!c)
//						{
//							throw new Error_InvalidAttribute(this.m_Attrib + " is not a legal attribute of the item or container element.");
//						}

						if ((m_Attrib != null) && (m_Attrib != ""))
						{
							bool c = true;
							if (this.m_SearchItem && this.m_SearchContainer)
							{
								string[] refid = { T[_ATTRIB.id], T[_ATTRIB.parentID], T[_ATTRIB.restricted], T[_ATTRIB.refID]};
								c = this.CheckAttribute(this.m_Ns, this.m_Tag, this.m_Attrib, refid);
								if (!c)
								{
									throw new Error_InvalidAttribute(this.m_Attrib + " is not a legal attribute of an item or container element.");
								}
							}
							else if (this.m_SearchItem)
							{
								string[] refid = { T[_ATTRIB.id], T[_ATTRIB.parentID], T[_ATTRIB.restricted], T[_ATTRIB.refID]};
								c = this.CheckAttribute(this.m_Ns, this.m_Tag, this.m_Attrib, refid);
								if (!c)
								{
									throw new Error_InvalidAttribute(this.m_Attrib + " is not a legal attribute of an item element.");
								}
							}
							else if (this.m_SearchContainer)
							{
								string[] searchable = { T[_ATTRIB.id], T[_ATTRIB.parentID], T[_ATTRIB.restricted], T[_ATTRIB.searchable]};
								c = this.CheckAttribute(this.m_Ns, this.m_Tag, this.m_Attrib, searchable);
								if (!c)
								{
									throw new Error_InvalidAttribute(this.m_Attrib + " is not a legal attribute of a container element.");
								}
							}
						}
					}
					else
					{
						// Otherwise, we've already matched the attribute with a CDS normative
						// attribute and we're ok.
					}
				}
				else if (this.m_SearchRes)
				{
					// Compare the desired attribute for extraction and ensure
					// that the attribute is CDS-normative.

					if ((m_Attrib != null) && (m_Attrib != ""))
					{
						string[] attribs = { 
											   T[_ATTRIB.bitrate], 
											   T[_ATTRIB.bitsPerSample], 
											   T[_ATTRIB.colorDepth], 
											   T[_ATTRIB.duration], 
											   T[_ATTRIB.importUri], 
											   T[_ATTRIB.nrAudioChannels], 
											   T[_ATTRIB.protection], 
											   T[_ATTRIB.protocolInfo], 
											   T[_ATTRIB.resolution], 
											   T[_ATTRIB.sampleFrequency], 
											   T[_ATTRIB.size] };

						bool c = this.CheckAttribute(this.m_Ns, this.m_Tag, this.m_Attrib, attribs);
						if (!c)
						{
							throw new Error_InvalidAttribute(this.m_Attrib + " is not a legal attribute of the res element.");
						}
					}
				}				
				else 
				{
					// Otherwise we're attempting to extract something from upnp: or dc:
					// In such a case, we'll take the element name and find the ICdsElement
					// class type and call the static GetPossibleAttributes() method.
					// This will provide us with a list of valid attributes for that particular 
					// metadata class. Then we'll compare the attribute that we're
					// supposed to extract with the provided list and throw an exception
					// if no match exists.

					StringBuilder elementName = new StringBuilder();
					elementName.AppendFormat("{0}:{1}", this.m_Ns, this.m_Tag);

					if (this.m_Attrib != "")
					{
						bool ignore;
						System.Type type = PropertyMappings.PropertyNameToType(elementName.ToString(), out ignore);

						MethodInfo mi = type.GetMethod("GetPossibleAttributes", BindingFlags.Public | BindingFlags.Static);

						if (mi != null)
						{
							IList possibleAttributes = (IList) mi.Invoke(null, null);

							if (possibleAttributes.Contains(this.m_Attrib) == false)
							{
								throw new Error_InvalidAttribute(this.m_Attrib + " is not a legal attribute of " + elementName.ToString());
							}
						}
						else
						{
							throw new ApplicationException(type.ToString() + " does not implement GetPossibleAttributes.");
						}
					}
				}
			}
			else
			{
				// Do not extract custom metadata values for now.
				// 
				throw new Error_InvalidAttribute("Cannot extract metadata value from <desc> elements or child elements in <desc> elements. Feature is not yet implemented.");
			}
		}

		/// <summary>
		/// Takes the separate fields of a [namespace]:[element]@[attribute name]
		/// and checks to see if the attributes map to any of the allowed attributes in the list.
		/// </summary>
		/// <param name="ns">namespace</param>
		/// <param name="tag">element name</param>
		/// <param name="attrib">attribute name</param>
		/// <param name="allowedAttribs">listing of legal attributes</param>
		/// <returns>true, if the attribute is in the list</returns>
		private bool CheckAttribute(string ns, string tag, string attrib, string[] allowedAttribs)
		{
			bool ok = false;
			foreach (string allowed in allowedAttribs)
			{
				if (attrib == allowed)
				{
					ok = true;
				}
			}

			return ok;
		}

		/// <summary>
		/// Given a property in one of the following formats
		/// <list type="table">
		/// <listheader><term>Format</term><description>Effect</description></listheader>
		/// <item><term>[namespace]:[element name]@[attribute]</term><description>Specific namespace, element, and attribute</description></item>
		/// <item><term>[element name]@[attribute]</term><description>DIDL namespace, specific element, specific attribute</description></item>
		/// <item><term>@[attribute]</term><description>DIDL namespace, any DIDL (item, container, res) element, specific attribute</description></item>
		/// </list>
		/// and determines the proper namespace, element name, and attribute values.
		/// </summary>
		/// <param name="property">specified metadata property</param>
		/// <param name="ns">namespace</param>
		/// <param name="tag">element name</param>
		/// <param name="attribute">attribute name</param>
		public static void ParseNsTagAttrib(string property, out string ns, out string tag, out string attribute)
		{
			if (property.StartsWith("@"))
			{
				ns = "";
				tag = "";
				attribute = property.Substring(1);
				return;
			}


			int colonPos = property.IndexOf(':');
			if (colonPos >= 0)
			{
				ns = property.Substring(0, colonPos);
			}
			else
			{
				ns = "";
			}

			int ampersandPos = property.IndexOf('@');

			int startTagPos = colonPos + 1;

			if (ampersandPos >= 0)
			{
				int len = ampersandPos - startTagPos;
				tag = property.Substring(startTagPos, len);

				attribute = property.Substring(ampersandPos+1);

				if (attribute == "")
				{
					throw new UPnPCustomException(709, "Cannot specify empty string for attribute if @ symbol is used.");
				}
			}
			else
			{
				tag = property.Substring(startTagPos);

				attribute = "";
			}
		}


		/// <summary>
		/// Given an <see cref="IUPnPMedia"/> the returned values
		/// are a list (or null) of all values associated with the supplied
		/// metadata property.
		/// </summary>
		/// <param name="media">the <see cref="IUPnPMedia"/> to interrogage</param>
		/// <returns>a list of values associated with metadata</returns>
		public IList Extract(IUPnPMedia media)
		{			
			ArrayList results = new ArrayList();
			
			// Obtain the values of the appropriate property.
			// 
			if (this.m_SearchDesc == false)
			{
				if (this.m_SearchContainer || this.m_SearchItem)
				{
					//grab attributes of item and container

					if (this.m_Attrib != "")
					{
						// Compare element attribute
						// 
						if (string.Compare(this.m_Attrib, T[_ATTRIB.id], true) == 0)
						{
							results.Add(media.ID);
						}
						else if (string.Compare(this.m_Attrib, T[_ATTRIB.parentID], true) == 0)
						{
							results.Add(media.ParentID);
						}
						else if (
							(string.Compare(this.m_Attrib, T[_ATTRIB.refID], true) == 0) ||
							(string.Compare(this.m_Attrib, T[_ATTRIB.childCount], true) == 0)
							)
						{
							IMediaContainer container = media as IMediaContainer;
							if (container != null)
							{
								results.Add(container.ChildCount);
							}
							else
							{
								IMediaItem mi = media as IMediaItem;

								if (mi != null)
								{
									if (mi.IsReference)
									{
										results.Add(mi.RefID);
									}
								}
							}
						}
						else if (string.Compare(this.m_Attrib, T[_ATTRIB.restricted], true) == 0)
						{
							// Regardless of 0/1 or false/true, a standard of string comparison will do.
							// 
							results.Add(media.IsRestricted);
						}
						else if (string.Compare(this.m_Attrib, T[_ATTRIB.searchable], true) == 0)
						{
							IMediaContainer container = media as IMediaContainer;
							if (container != null)
							{
								results.Add(container.IsSearchable);
							}
						}
					}
					else
					{
						// Grab the innerText value of an item or container object
						// because that's really the value of an item or container.
						// 
						results.Add(this.GrabInnerText(media));
					}	
				}
				else if (this.m_SearchRes)
				{
					foreach (IMediaResource res in media.MergedResources)
					{
						if (this.m_Attrib != "")
						{
							IList resAttribs= res.ValidAttributes;
							if (resAttribs.Contains(this.m_Attrib))
							{
								if (string.Compare(this.m_Attrib, T[_ATTRIB.bitrate], true) == 0)
								{
									results.Add(res.Bitrate);
								}
								else if (string.Compare(this.m_Attrib, T[_ATTRIB.bitsPerSample], true) == 0)
								{
									results.Add(res.BitsPerSample);
								}
								else if (string.Compare(this.m_Attrib, T[_ATTRIB.colorDepth], true) == 0)
								{
									results.Add(res.ColorDepth);
								}
								else if (string.Compare(this.m_Attrib, T[_ATTRIB.duration], true) == 0)
								{
									results.Add(res.Duration);
								}
								else if (string.Compare(this.m_Attrib, T[_ATTRIB.importUri], true) == 0)
								{
									results.Add(res.ImportUri);
								}
								else if (string.Compare(this.m_Attrib, T[_ATTRIB.nrAudioChannels], true) == 0)
								{
									results.Add(res.nrAudioChannels);
								}
								else if (string.Compare(this.m_Attrib, T[_ATTRIB.protection], true) == 0)
								{
									results.Add(res.Protection);
								}
								else if (string.Compare(this.m_Attrib, T[_ATTRIB.protocolInfo], true) == 0)
								{
									results.Add(res.ProtocolInfo.ToString());
								}
								else if (string.Compare(this.m_Attrib, T[_ATTRIB.resolution], true) == 0)
								{
									results.Add(res[_RESATTRIB.resolution]);
								}
								else if (string.Compare(this.m_Attrib, T[_ATTRIB.sampleFrequency], true) == 0)
								{
									results.Add(res.SampleFrequency);
								}
								else if (string.Compare(this.m_Attrib, T[_ATTRIB.size], true) == 0)
								{
									results.Add(res.Size);
								}
							}
						}
						else
						{
							Type resType = res.GetType();

							PropertyInfo pi = resType.GetProperty("RelativeContentUri");

							if (pi == null)
							{
								results.Add(res.ContentUri);
							}
							else
							{
								string relativeUri = (string) pi.GetValue(res, null);
								if ((relativeUri != null) && (relativeUri != ""))
								{
									results.Add(relativeUri);
								}
								else
								{
									throw new ApplicationException("res.RelativeContentUri is null or empty.");
								}
							}
						}
					} //end for
				}
				else if (string.Compare(this.m_Tag, _DC.title.ToString(), true) == 0)
				{
					string title = media.Title;
					results.Add(title);
				}
				else
				{
					StringBuilder sb = new StringBuilder();
					sb.AppendFormat("{0}:{1}", this.m_Ns, this.m_Tag);
					IList values = null;

					IList propValues = media.MergedProperties[sb.ToString()];
					
					if (propValues != null)
					{
						foreach (ICdsElement propertyValue in propValues)
						{
							if (values == null)
							{
								values = new ArrayList();
							}

							object val = null;
							if (this.m_Attrib != "")
							{
								val = propertyValue.ExtractAttribute(this.m_Attrib);
							}
							else
							{
								val = propertyValue.ComparableValue;
							}

							if (val != null)
							{
								values.Add(val);
							}
						}

						if (values != null)
						{
							results.AddRange(values);
						}
					}
				}
			}
			else
			{
				// Limit our searches to the "desc" elements.
				// 
				throw new UPnPCustomException(709, "Extraction, sorting, or comparison of <desc> elements or child elements in <desc> nodes is not implemented yet.");
			}
			return results;
		}
		/// <summary>
		/// Takes the contents of an <see cref="IUPnPMedia"/> object
		/// casts the data into its XML string form and returns it.
		/// </summary>
		/// <param name="entry">the object to extract</param>
		/// <returns>everything under the item or container element</returns>
		private string GrabInnerText(IUPnPMedia entry)
		{
			StringBuilder sb = new StringBuilder(1000);
			StringWriter sw = new StringWriter(sb);
			XmlTextWriter xmlWriter = new XmlTextWriter(sw);

			// set up the ToXml() method to only provide the InnerText of the element
			entry.ToXml(ToXmlFormatter.DefaultFormatter, MediaObject.ToXmlData_AllInnerTextOnly, xmlWriter);
			xmlWriter.Flush();

			string result = xmlWriter.ToString();
			xmlWriter.Close();

			return result;
		}

		/// <summary>
		/// indicates that an item elemetn should be searched
		/// </summary>
		private bool m_SearchItem = false;

		/// <summary>
		/// indicates that a container element should be searched
		/// </summary>
		private bool m_SearchContainer = false;

		/// <summary>
		/// indicates that a res element should be searched
		/// </summary>
		private bool m_SearchRes = false;

		/// <summary>
		/// indicates that a "desc" node should be searched
		/// </summary>
		private bool m_SearchDesc = false;

		/// <summary>
		/// namespace of property
		/// </summary>
		private string m_Ns = "";
		/// <summary>
		/// element name of property
		/// </summary>
		private string m_Tag = "";
		/// <summary>
		/// attribute of property
		/// </summary>
		private string m_Attrib = "";
		/// <summary>
		/// Used for token-delimited text parsing
		/// </summary>
		private static Tags T = Tags.GetInstance();
	}
}
