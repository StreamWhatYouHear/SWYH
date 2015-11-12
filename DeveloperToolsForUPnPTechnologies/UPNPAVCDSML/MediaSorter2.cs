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
using System.Xml;
using System.Text;
using OpenSource.UPnP;
using System.Threading;
using System.Collections;
using OpenSource.UPnP.AV;
using OpenSource.Utilities;
using System.Text.RegularExpressions;

namespace OpenSource.UPnP.AV.CdsMetadata
{

	/// <summary>
	/// The class abstracts a simple means by which items and containers
	/// can be sorted alphabetically by their _ID.
	/// </summary>
	public sealed class SortByID : IMediaSorter
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="forceDistinctMatches">
		/// If true, then any two objects that have the same ID 
		/// when compared will return a nonzero value. If
		/// using this object with a SortedList, then the
		/// value should be true.
		/// </param>
		public SortByID(bool forceDistinctMatches)
		{
			this.forceDistinct = forceDistinctMatches;
		}
		private bool forceDistinct;
		/// <summary>
		/// Member-wise clone.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return this.MemberwiseClone();
		}
		public int Compare (object mediaObject1, object mediaObject2)
		{
			IUPnPMedia obj1 = mediaObject1 as IUPnPMedia;
			IUPnPMedia obj2 = mediaObject2 as IUPnPMedia;

			int cmp = string.Compare(obj1.ID, obj2.ID);
			if (forceDistinct)
			{
				if (cmp == 0)
				{
					return -1;
				}
			}
			return cmp;
		}
	}

	
	/// <summary>
	/// The class abstracts a simple means by which items and containers
	/// can be sorted alphabetically by their title.
	/// If the objects match, then an arbitrary objected is declared greater
	/// than the other, such that the comparison value can be used
	/// with a SortedList.
	/// </summary>
	public sealed class SortByTitle : IMediaSorter
	{
		/// <summary>
		/// Member-wise clone.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return this.MemberwiseClone();
		}
		public int Compare (object mediaObject1, object mediaObject2)
		{
			return Sorter.Compare(mediaObject1, mediaObject2);
		}
		private static Tags T = Tags.GetInstance();
		private static MediaSorter Sorter = new MediaSorter(true, "+"+T[_DC.title]);
	}

	/// <summary>
	/// The class abstracts a simple means by which items and containers
	/// can be sorted alphabetically by their creator.
	/// If the objects match, then an arbitrary objected is declared greater
	/// than the other, such that the comparison value can be used
	/// with a SortedList.
	/// </summary>
	public sealed class SortByCreator : IMediaSorter
	{
		/// <summary>
		/// Member-wise clone.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return this.MemberwiseClone();
		}
		public int Compare (object mediaObject1, object mediaObject2)
		{
			return Sorter.Compare(mediaObject1, mediaObject2);
		}
		private static Tags T = Tags.GetInstance();
		private static MediaSorter Sorter = new MediaSorter(true, "+"+T[_DC.creator]);
	}

	/// <summary>
	/// The class abstracts a simple means by which items and containers
	/// can be sorted alphabetically by their creator and title.
	/// If the objects match, then an arbitrary objected is declared greater
	/// than the other, such that the comparison value can be used
	/// with a SortedList.
	/// </summary>
	public sealed class SortByCreatorTitle : IMediaSorter
	{
		/// <summary>
		/// Member-wise clone.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return this.MemberwiseClone();
		}
		public int Compare (object mediaObject1, object mediaObject2)
		{
			return Sorter.Compare(mediaObject1, mediaObject2);
		}
		private static Tags T = Tags.GetInstance();
		private static MediaSorter Sorter = new MediaSorter(true, "+"+T[_DC.creator]+",+"+T[_DC.title]);
	}

	/// <summary>
	/// The class abstracts a simple means by which items and containers
	/// can be sorted alphabetically by their media class.
	/// If the objects match, then an arbitrary objected is declared greater
	/// than the other, such that the comparison value can be used
	/// with a SortedList.
	/// </summary>
	public sealed class SortByClass : IMediaSorter
	{
		/// <summary>
		/// Member-wise clone.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return this.MemberwiseClone();
		}
		public int Compare (object mediaObject1, object mediaObject2)
		{
			return Sorter.Compare(mediaObject1, mediaObject2);
		}
		private static Tags T = Tags.GetInstance();
		private static MediaSorter Sorter = new MediaSorter(true, "+"+T[_UPNP.Class]);
	}

	/// <summary>
	/// The class abstracts a simple means by which items and containers
	/// can be sorted alphabetically by their media class, creator, and title.
	/// If the objects match, then an arbitrary objected is declared greater
	/// than the other, such that the comparison value can be used
	/// with a SortedList.
	/// </summary>
	public sealed class SortByClassCreatorTitle : IMediaSorter
	{
		/// <summary>
		/// Member-wise clone.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return this.MemberwiseClone();
		}
		public int Compare (object mediaObject1, object mediaObject2)
		{
			return Sorter.Compare(mediaObject1, mediaObject2);
		}
		private static Tags T = Tags.GetInstance();
		private static MediaSorter Sorter = new MediaSorter(true, "+"+T[_UPNP.Class]+",+"+T[_DC.creator]+",+"+T[_DC.title]);
	}
}
