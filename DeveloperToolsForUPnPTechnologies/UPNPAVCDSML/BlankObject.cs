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
using System.Runtime.Serialization;
using OpenSource.UPnP.AV;
using OpenSource.Utilities;

namespace OpenSource.UPnP.AV.CdsMetadata
{
	/// <summary>
	/// <see cref="IUPnPMedia"/> instances in <see cref="MediaContainer"/> are sorted
	/// by IDs... so we can use this object for as "key" to find objects with
	/// the same ID when using <see cref="MediaObject"/>.IdSorter.
	/// <para>
	/// In just about every other usage scenario, this object is useless.
	/// All methods are simple no-op stubs, excepting the ID property.
	/// </para>
	/// </summary>
	public class BlankObject : IUPnPMedia
	{
		private object _Tag;
		public object Tag { get { return _Tag; } set { _Tag = value; } }
		public void SetMetadata(MediaBuilder.CoreMetadata info) { }
		public string ToDidl() { return ""; }
		public EnumWriteStatus WriteStatus { get { return EnumWriteStatus.UNKNOWN; } set {}}
		public void UpdateObject (XmlElement xml) {}
		public void UpdateObject (string xml) {}
		public void UpdateObject(IUPnPMedia obj) {}
		public void CheckRuntimeBindings(StackTrace st) {}
		public void WriteInnerXml(ArrayList desiredProperties, XmlTextWriter xmlWriter){}
		public IUPnPMedia MetadataCopy()
		{
			return null;
		}

		public void AddObject(IUPnPMedia newObject, bool overWrite){}
		public void AddObjects(ICollection newObjects, bool overWrite){}
		public void RemoveObject (IUPnPMedia removeThis){}
		public void RemoveObjects (ICollection removeThese){}
		public void AddDescNode(string element){}
		public void AddDescNode(string[] elements){}
		public void RemoveDescNode(string element) {}

		public void AddResource (IMediaResource r)
		{
		}
		public void AddResources (ICollection ic)
		{
		}
		public void RemoveResource (IMediaResource r)
		{
		}
		public void RemoveResources (ICollection ic)
		{
		}
		public void ToXml(ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter)
		{
		}

		public MediaClass Class { get { return MediaClass.NullMediaClass; } set {}}

		public string Creator { get { return ""; } set {} }

		public IList DescNodes { get { return null; } }

		public string ID { get { return this.m_ID; } set { this.m_ID = value; } }

		public bool IsContainer { get { return false; } }

		public bool IsItem { get { return false; } }

		public bool IsReference { get { return false; } }

		public bool IsRestricted { get { return false; } set { }}

		public bool IsSearchable { get { return false; } set { ; } }

		public IMediaProperties MergedProperties { get { return new MediaProperties(); } }

		public IMediaProperties Properties { get { return new MediaProperties(); } }

		public IMediaContainer Parent { get { return null; } set{}}

		public string ParentID { get { return ""; } set { }}

		public IMediaResource[] Resources { get { return null; } }

		public IMediaResource[] MergedResources { get { return null; } }

		public IList MergedDescNodes { get { return null; } }

		//public void SetTitle(string title){}
		//public void SetWriteStatus(EnumWriteStatus writeStatus){}
//		public void SetClass(string classType, string friendlyName){}
		public void SetPropertyValue (string propertyName, IList values){}
//		public void SetPropertyValue_String(string propertyName, string[] values){}
//		public void SetPropertyValue_String(string propertyName, string val){}
//		public void SetPropertyValue_Int(string propertyName, int[] values){}
//		public void SetPropertyValue_Int(string propertyName, int val){}
//		public void SetPropertyValue_Long(string propertyName, long[] values){}
//		public void SetPropertyValue_Long(string propertyName, long val){}
//		public void SetPropertyValue_MediaClass(string propertyName, MediaClass[] values){}
//		public void SetPropertyValue_MediaClass(string propertyName, MediaClass val){}


		public string Title { get { return ""; } set {} }
		public string m_ID = null;
	}
}
