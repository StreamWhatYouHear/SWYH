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
	/// Class defines static methods that are used to print the InnerXml DIDL-Lite
	/// elements of media objects.
	/// </summary>
	public class InnerXmlWriter
	{
		/// <summary>
		/// Delegate type that defines what type of method to use when 
		/// <see cref="WriteInnerXml"/> is used to print the inner xml
		/// of a media object - purpose of calling the method is to determine
		/// if resources should be printed. 
		/// </summary>
		public delegate bool DelegateShouldPrintResources (ArrayList desiredProperties);

		/// <summary>
		/// Delegate type defines what type of method to use when 
		/// <see cref="WriteInnerXml"/> is used to print the properties 
		/// of a media object.
		/// </summary>
		public delegate void DelegateWriteProperties(IUPnPMedia mo, ToXmlData data, XmlTextWriter xmlWriter);

		/// <summary>
		/// Delegate type defines what type of method to use when 
		/// <see cref="WriteInnerXml"/> is used to print the resources 
		/// of a media object.
		/// </summary>
		public delegate void DelegateWriteResources(IUPnPMedia mo, DelegateShouldPrintResources shouldPrintResources, ToXmlFormatter formatter, ToXmlData data, XmlTextWriter xmlWriter);

		/// <summary>
		/// Delegate type defines what type of method to use when 
		/// <see cref="WriteInnerXml"/> is used to print the desc nodes
		/// of a media object.
		/// </summary>
		public delegate void DelegateWriteDescNodes(IUPnPMedia mo, ToXmlData data, XmlTextWriter xmlWriter);

		/// <summary>
		/// Default implementation to use when printing media object properties.
		/// </summary>
		/// <param name="mo"></param>
		/// <param name="data"></param>
		/// <param name="xmlWriter"></param>
		public static void WriteInnerXmlProperties(IUPnPMedia mo, ToXmlData data, XmlTextWriter xmlWriter)
		{
			mo.MergedProperties.ToXml(data, xmlWriter);
		}

		/// <summary>
		/// Default implementation to use when printing media object resources.
		/// </summary>
		/// <param name="mo"></param>
		/// <param name="shouldPrintResources"></param>
		/// <param name="formatter"></param>
		/// <param name="data"></param>
		/// <param name="xmlWriter"></param>
		public static void WriteInnerXmlResources(IUPnPMedia mo, DelegateShouldPrintResources shouldPrintResources, ToXmlFormatter formatter, ToXmlData data, XmlTextWriter xmlWriter)
		{
			if (shouldPrintResources(data.DesiredProperties))
			{
				// Set up the resource formatter to use the
				// default StartElement, EndElement, WriteInnerXml, and WriteValue
				// implementations. This stuff has no effect
				// if the WriteResource field has been assigned.
				ToXmlFormatter resFormatter = formatter;
				resFormatter.StartElement = null;
				resFormatter.EndElement = null;
				resFormatter.WriteInnerXml = null;
				resFormatter.WriteValue = null;

				// print the resources
				foreach (IMediaResource res in mo.MergedResources)
				{
					res.ToXml(resFormatter, data, xmlWriter);
				}
			}
		}

		/// <summary>
		/// Default implementation to use when printing media object desc nodes.
		/// </summary>
		/// <param name="mo"></param>
		/// <param name="data"></param>
		/// <param name="xmlWriter"></param>
		public static void WriteInnerXmlDescNodes(IUPnPMedia mo, ToXmlData data, XmlTextWriter xmlWriter)
		{
			foreach (string descNode in mo.MergedDescNodes)
			{
				// do nothing for custom metadata for now
				if (
					(data.DesiredProperties.Count == 0) ||
					(data.DesiredProperties.Contains(Tags.GetInstance()[_DIDL.Desc]) == true)
					)
				{
					// desc nodes are simply strings that 
					// conform to CDS-compliant desc node elements
					xmlWriter.WriteRaw(descNode);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mo"></param>
		/// <param name="writeProperties"></param>
		/// <param name="shouldPrintResources"></param>
		/// <param name="writeResources"></param>
		/// <param name="writeDescNodes"></param>
		/// <param name="formatter"></param>
		/// <param name="data"></param>
		/// <param name="xmlWriter"></param>
		public static void WriteInnerXml (
			IUPnPMedia mo, 
			DelegateWriteProperties writeProperties, 
			DelegateShouldPrintResources shouldPrintResources, 
			DelegateWriteResources writeResources, 
			DelegateWriteDescNodes writeDescNodes, 
			ToXmlFormatter formatter, 
			ToXmlData data, 
			XmlTextWriter xmlWriter
			)
		{
			// When serializing the properties, resources, and desc nodes,
			// we must always write the inner xml, the value, and the element
			// declaration. Save the original values, print these portions
			// of the DIDL-Lite and then revert back.

			ToXmlData d2 = data;
			
			data.IncludeElementDeclaration = true;
			data.IncludeInnerXml = true;
			data.IncludeValue = true;

			if (writeProperties != null)
			{
				writeProperties(mo, data, xmlWriter); 
			}
			
			if (writeResources != null) 
			{
				writeResources(mo, shouldPrintResources, formatter, data, xmlWriter); 
			}
			
			if (writeDescNodes != null) 
			{
				writeDescNodes(mo, data, xmlWriter); 
			}

			data.IncludeElementDeclaration = d2.IncludeElementDeclaration;
			data.IncludeInnerXml = d2.IncludeInnerXml;
			data.IncludeValue = d2.IncludeValue;

			IMediaContainer mc = mo as IMediaContainer;
			if (mc != null)
			{
				if (data.IsRecursive)
				{
					foreach (IUPnPMedia child in mc.CompleteList)
					{
						ToXmlFormatter f2 = formatter;
						f2.StartElement = null;
						f2.EndElement = null;
						f2.WriteInnerXml = null;
						f2.WriteValue = null;

						child.ToXml(f2, data, xmlWriter);
					}
				}
			}
		}
	}
}
