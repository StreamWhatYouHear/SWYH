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
using OpenSource.UPnP.AV;
using System.Collections;
using OpenSource.Utilities;
using System.Globalization;
using System.Text.RegularExpressions;

namespace OpenSource.UPnP.AV.CdsMetadata

{
	/// <summary>
	/// This class is a helper class for sorting ContentDirectory entries for an XML response.
	/// Its function is to compare the values of a media entry's xml element or attribute.
	/// By default, the class only compares the first value within a set of acceptable properties.
	/// If the values match, then does the next value get compared. If all values of a property set
	/// match then the result comparison is 0.
	/// </summary>
	public class MetadataValueComparer : IComparer
	{
		/// <summary>
		/// Instantiates a MetadataValueComparer with a metadata property name
		/// (easily retrieved from using <see cref="Tags"/>'s indexer with the
		/// <see cref="CommonPropertyNames"/>, 
		/// <see cref="_DC"/>, 
		/// <see cref="_UPNP"/>, or
		/// <see cref="_ATTRIB"/>
		/// enumerators) 
		/// and an indication if the results should compare in ascending or descending order.
		/// </summary>
		/// <param name="property">metadata property name</param>
		/// <param name="ascending">true, if Compare results are ascending order</param>
		public MetadataValueComparer (string property, bool ascending)
		{
			this.m_Ascending = ascending;
			this.m_TE = new TagExtractor(property);
		}

		/// <summary>
		/// This compares two <see cref="IUPnPMedia"/> objects
		/// and compares them based on the values of a single property name
		/// (supplied at construction time).
		/// </summary>
		/// <param name="m1"><see cref="IUPnPMedia"/></param>
		/// <param name="m2"><see cref="IUPnPMedia"/></param>
		/// <returns>comparison result with ascending or descending flag taken into account: 0, 1, -1 </returns>
		public int Compare (object m1, object m2)
		{
			IUPnPMedia e1 = (IUPnPMedia) m1;
			IUPnPMedia e2 = (IUPnPMedia) m2;

			IList vals1 = this.m_TE.Extract(e1);
			IList vals2 = this.m_TE.Extract(e2);

			try
			{
				int i = 0;
				int max = Math.Min(vals1.Count, vals2.Count);

				while (i < max)
				{
					object obj1 = vals1[i];
					object obj2 = vals2[i];

					int cmp;
					try
					{
						cmp = CompareObject(obj1, obj2, false);
					}
					catch (Exception e)
					{
						throw new Exception("MetadataValueComparer.Compare() error", e);
					}

					if (cmp != 0)
					{
						if (this.m_Ascending == false)
						{
							cmp = cmp * -1;
						}
						return cmp;
					}
					i++;
				}
			}
			catch (Exception e)
			{
				throw new Exception("MetadataValueComparer.Compare() error", e);
			}

			// If we get here, then all values matched...
			// so use the difference of the number of
			// values to determine comparison result.

			int diff = vals1.Count - vals2.Count;

			if (this.m_Ascending == false)
			{
				diff = diff * -1;
			}
			return diff;
		}

		/// <summary>
		/// This method is useful if metadata values are already obtained and a
		/// comparison is needed. The method compares val1 against val2.
		/// If the types don't match, then the type of val1 is
		/// used to determine the semantics of val2, and an attempt
		/// to cast val2 into the same type of val1 is made. This is useful
		/// when val1 is a uri/number and val2 is a string.
		/// </summary>
		/// <param name="val1">a value of some type</param>
		/// <param name="val2">another value of the same type, or a string that can be cast into the type of val1</param>
		/// <param name="ignoreCase">indicates if we should ignore the case for text-related comparisons</param>
		/// <returns>a comparison result: 0, 1, -1</returns>
		public static int CompareTagValues(object val1, object val2, bool ignoreCase)
		{
			string type1 = val1.GetType().ToString();
			string type2 = val2.GetType().ToString();

			object v1 = val1;
			object v2 = val2;

			if (type1 != type2)
			{
				// If the data types for the two objects don't match
				// I do my best to convert the second object 
				// into a data type of the first object.
				string strval2 = val2.ToString();
				switch (type1)
				{
					case "System.Char":
						v2 = strval2[0];
						break;
					case "System.String":
						v2 = strval2;
						break;
					case "System.Boolean":
						if ((strval2 == "0") || (strval2 == "no"))
						{
							v2 = false;
						}
						else if ((strval2 == "1") || (strval2 == "yes"))
						{
							v2 = true;
						}
						else
						{
							try
							{
								v2 = bool.Parse(strval2);
							}
							catch
							{
								v2 = strval2;
							}
						}
						break;
					case "System.Uri":
						//Uri is an exception - we'll always compare as a string.
						v2 = strval2;
						break;
					case "System.Byte":
						v2 = byte.Parse(strval2);
						break;
					case "System.UInt16":
						v2 = UInt16.Parse(strval2);
						break;
					case "System.UInt32":
						v2 = UInt32.Parse(strval2);
						break;
					case "System.Int32":
						v2 = Int32.Parse(strval2);
						break;
					case "System.Int16":
						v2 = Int16.Parse(strval2);
						break;
					case "System.SByte":
						v2 = sbyte.Parse(strval2);
						break;
					case "System.Single":
						v2 = Single.Parse(strval2);
						break;
					case "System.Double":
						v2 = Double.Parse(strval2);
						break;
					case "System.DateTime":
						DateTimeFormatInfo formatInfo = PropertyDateTime.GetFormatInfo();
						v2 = DateTime.Parse(strval2, formatInfo);
						break;
					default:
						throw new Exception("Cannot recast object of " + type2 + " into object of " + type1);
				}
			}

			return CompareObject(v1, v2, ignoreCase);
		}

		/// <summary>
		/// Implements the core logic for comparing two objects of the same primitive type.
		/// If the objects are not of a primitive type, then the ToString() operation
		/// is used and then the comparison is made.
		/// </summary>
		/// <param name="obj1">object with value</param>
		/// <param name="obj2">object with value</param>
		/// <param name="ignoreCase">indicates if we should ignore the case for text-related comparisons</param>
		/// <returns>comparison result:	0, 1, -1</returns>
		private static int CompareObject(object obj1, object obj2, bool ignoreCase)
		{
			if ((obj1 == null) && (obj2 == null))
			{
				return 0;
			}
			else if ((obj1 != null) && (obj2 == null))
			{
				return 1;
			}
			else if ((obj1 == null) && (obj2 != null))
			{
				return -1;
			}

			Type t1 = obj1.GetType();
			Type t2 = obj1.GetType();
			string type1 = t1.ToString();
			string type2 = t2.ToString();

			if (type1 != type2)
			{
				throw new Exception("Cannot compare 2 values with a different data type.");
			}

			switch (type1)
			{
				case "System.Char":
					char v1 = (char) obj1;
					char v2 = (char) obj2;

					if (v1 == v2) return 0;
					if (v1 > v2) return 1;
					if (v1 < v2) return -1;
					break;
				case "System.String":
					string s1 = obj1.ToString();
					string s2 = obj2.ToString();

					int cmps = string.Compare(s1, s2, ignoreCase);
					return cmps;
				case "System.Boolean":
					bool b1 = (bool) obj1;
					bool b2 = (bool) obj2;

					if (b1 == b2) return 0;
					if (b1) return 1;
					if (b2) return -1;
					break;
				case "System.Uri":
					string u1 = obj1.ToString();
					string u2 = obj2.ToString();

					int cmp = string.Compare(u1, u2, ignoreCase);
					return cmp;
				case "System.Byte":
					byte byte1 = (byte) obj1;
					byte byte2 = (byte) obj2;

					if (byte1 == byte2) return 0;
					if (byte1 > byte2) return 1;
					if (byte1 < byte2) return -1;
					break;
				case "System.UInt16":
					UInt16 ui16_1 = (UInt16) obj1;
					UInt16 ui16_2 = (UInt16) obj2;

					if (ui16_1 == ui16_2) return 0;
					if (ui16_1 > ui16_2) return 1;
					if (ui16_1 < ui16_2) return -1;
					break;
				case "System.UInt32":
					UInt32 ui32_1 = (UInt32) obj1;
					UInt32 ui32_2 = (UInt32) obj2;

					if (ui32_1 == ui32_2) return 0;
					if (ui32_1 > ui32_2) return 1;
					if (ui32_1 < ui32_2) return -1;
					break;
				case "System.Int32":
					Int32 i32_v1 = (Int32) obj1;
					Int32 i32_v2 = (Int32) obj2;

					if (i32_v1 == i32_v2) return 0;
					if (i32_v1 > i32_v2) return 1;
					if (i32_v1 < i32_v2) return -1;
					break;
				case "System.Int16":
					Int16 i16_v1 = (Int16) obj1;
					Int16 i16_v2 = (Int16) obj2;

					if (i16_v1 == i16_v2) return 0;
					if (i16_v1 > i16_v2) return 1;
					if (i16_v1 < i16_v2) return -1;
					break;
				case "System.SByte":
					sbyte sb_v1 = (sbyte) obj1;
					sbyte sb_v2 = (sbyte) obj2;

					if (sb_v1 == sb_v2) return 0;
					if (sb_v1 > sb_v2) return 1;
					if (sb_v1 < sb_v2) return -1;
					break;
				case "System.Single":
					Single s_v1 = (Single) obj1;
					Single s_v2 = (Single) obj2;

					if (s_v1 == s_v2) return 0;
					if (s_v1 > s_v2) return 1;
					if (s_v1 < s_v2) return -1;
					break;
				case "System.Double":
					Double d_v1 = (Double) obj1;
					Double d_v2 = (Double) obj2;

					if (d_v1 == d_v2) return 0;
					if (d_v1 > d_v2) return 1;
					if (d_v1 < d_v2) return -1;
					break;
				case "System.DateTime":
					DateTime dt1 = (DateTime) obj1;
					DateTime dt2 = (DateTime) obj2;

					if (dt1.Ticks == dt2.Ticks) return 0;
					if (dt1.Ticks > dt2.Ticks) return 1;
					if (dt1.Ticks < dt2.Ticks) return -1;
					break;
				default:
					//throw new Exception("Cannot compare objects of type " + type1);

					IComparable comparable1 = obj1 as IComparable;
					IComparable comparable2 = obj2 as IComparable;

					if (comparable1 != null)
					{
						if (comparable2 != null)
						{
							return comparable1.CompareTo(comparable2);
						}
					}

					string default1 = obj1.ToString();
					string default2 = obj2.ToString();

					int def_cmps = string.Compare(default1, default2, ignoreCase);
					return def_cmps;
			}

			return 0;
		}

		/// <summary>
		/// Indicates if the results should be ascending or descending.
		/// </summary>
		public readonly bool m_Ascending = false;

		/// <summary>
		/// Extracts the metadata values for a given property name or attribute.
		/// </summary>
		private TagExtractor m_TE;
	}
}
