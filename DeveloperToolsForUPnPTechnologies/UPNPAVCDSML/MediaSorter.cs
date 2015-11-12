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
	/// Defines the interface by which media objects can be sorted.
	/// </summary>
	public interface IMediaSorter : IComparer, ICloneable
	{
		/// <summary>
		/// Compares 2 media object instances against each other.
		/// Returns 0 if they are equal, 
		/// -1 if mediaObject1 is less than mediaObject2,
		/// and 1 if mediaObject1 is greater than mediaObject2.
		/// </summary>
		/// <param name="mediaObject1">the object should implement <see cref="IUPnPMedia"/></param>
		/// <param name="mediaObject2">the object should implement <see cref="IUPNPMedia"/></param>
		/// <returns></returns>
		new int Compare (object mediaObject1, object mediaObject2);
	}

	/// <summary>
	/// This class enables the sorting of a ContentDirectory Browse/Search
	/// results set. Programmers should use a standard .NET SortedList object
	/// and use an instance of this class in the constructor
	/// to sort the items that get added to the list.
	/// </summary>
	public class MediaSorter : IMediaSorter, IComparer
	{
		/// <summary>
		/// Member-wise clone.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return this.MemberwiseClone();
		}

		/// <summary>
		/// This method expects instances of
		/// IUPnPMedia for comparison.
		/// </summary>
		/// <param name="m1"><see cref="IUPnPMedia"/></param>
		/// <param name="m2"><see cref="IUPnPMedia"/></param>
		/// <returns></returns>
		public int Compare (object m1, object m2)
		{
			int result = 0;
			try
			{
				int cmp;
				IUPnPMedia mo1 = (IUPnPMedia) m1;
				IUPnPMedia mo2 = (IUPnPMedia) m2;

				foreach (MetadataValueComparer mvc in this.sortKeys)
				{
					cmp = mvc.Compare(m1, m2);
					if (cmp != 0)
					{
						//NKIDD-DEBUG
//						mvc.Compare(m1, m2);
						return cmp;
					}
				}

				if (forceDistinction)
				{
					result = -1;
				}
			}
			catch (Exception e)
			{
				throw new ApplicationException("MediaSorter.Compare(object, object) error: " + e.Message, e);
			}

			return result;
		}
			
		/// <summary>
		/// <para>
		/// Instantiates a sorter based on the sortCriteria string. The basic algorithm for
		/// sorting is to supply a comma-separated-value (CSV) listing of property names
		/// (easily retrieved from using <see cref="Tags"/>'s indexer with the
		/// <see cref="CommonPropertyNames"/>, 
		/// <see cref="_DC"/>, 
		/// <see cref="_UPNP"/>, or
		/// <see cref="_ATTRIB"/>
		/// enumerators.) 
		/// with each property name having a + or - sign to indicate ascending or descending
		/// order of the sorting for that property name. Assuming "T" is an instance of a
		/// <see cref="Tags"/> object.
		/// Example below.
		/// </para>
		/// 
		/// <para>
		/// <list type="table">
		/// <item>
		/// <term>"+" +T[CommonPropertyNames.title]+ ",-" +T[CommonPropertyNames.creator]</term>
		/// <description>Sorts with ascending titles first, then descending creators</description>
		/// </item>
		/// </list>
		/// </para>
		/// 
		/// </summary>
		/// <param name="forceDistinction">if true, then any comparison that evaluates to equal will force an arbitrary ordering of the two objects in question</param>
		/// <param name="sortCriteria">a string with the property names in a CSV list, where
		/// each property name has a + or - sign to indicate ascending or descending order</param>
		public MediaSorter (bool forceDistinction, string sortCriteria)
		{
			this.forceDistinction = forceDistinction;

			if (sortCriteria != null)
			{
				if (sortCriteria != "")
				{
					DText parser = new DText();
					parser.ATTRMARK = ",";
					parser[0] = sortCriteria;

					int size = parser.DCOUNT();
					this.sortKeys = new ArrayList(size);
					
					// iterate through properties for sorting
					//
					for (int i=1; i <= size; i++)
					{
						string prop = parser[i].Trim();
						char asc_dsc = prop[0];

						string property = prop.Substring(1);

						bool ascending = true;
						if (asc_dsc == '-')
						{
							ascending = false;
						}
						else if (asc_dsc == '+')
						{
							ascending = true;
						}
						else
						{
							throw new UPnPCustomException(709, "Invalid sort flag.");
						}

						MetadataValueComparer mvc = new MetadataValueComparer(property, ascending);

						this.sortKeys.Add(mvc);
					}
				}
			}

			if (this.sortKeys == null)
			{
				this.sortKeys = new ArrayList(0);
			}
		}

		/// <summary>
		/// The listing of keys to sort on, in the correct order.
		/// </summary>
		private ArrayList sortKeys;
		/// <summary>
		/// If true, forces any evaluation of "equal" media to be
		/// arbitrarily ordered in the CompareTo() method.
		/// </summary>
		private bool forceDistinction;
		/// <summary>
		/// used for text delimited parsing
		/// </summary>
		private static DText DT = new DText();
	}

}
