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
using System.Collections;

namespace OpenSource.UPnP
{
	public class ReadOnlyObjectCollection : ReadOnlyCollectionBase
	{
		public ReadOnlyObjectCollection(ICollection items)
		{
			if (items != null)
			{
				if (items.Count > 0)
				{
					foreach (object obj in items)
					{
						base.InnerList.Add(obj);
					}
				}
			}
		}

		public Object Item(int Index)
		{
			return base.InnerList[Index];
		}
	}

	/*
	/// <summary>
	/// Summary description for ReadonlyStringCollection.
	/// </summary>
	public class ReadOnlyStringCollection : ReadOnlyCollectionBase
	{
		public ReadOnlyStringCollection(string[] items)
		{
			if (items != null)
			{
				if (items.Length > 0)
				{
					foreach (string s in items)
					{
						base.InnerList.Add(s);
					}
				}
			}
		}

		public ReadOnlyStringCollection(ArrayList items)
		{
			if (items != null)
			{
				if (items.Count > 0)
				{
					foreach (object s in items)
					{
						base.InnerList.Add(s.ToString());
					}
				}
			}
		}

		public string Item(int Index)
		{
			return (string) base.InnerList[Index];
		}
	}

	/// <summary>
	/// Summary description for ReadonlyIntCollection.
	/// </summary>
	public class ReadOnlyIntCollection : ReadOnlyCollectionBase
	{
		public ReadOnlyIntCollection(int[] items)
		{
			if (items != null)
			{
				if (items.Length > 0)
				{
					foreach (int s in items)
					{
						base.InnerList.Add(s);
					}
				}
			}
		}

		public ReadOnlyIntCollection(ArrayList items)
		{
			if (items != null)
			{
				if (items.Count > 0)
				{
					foreach (object s in items)
					{
						string vtype = s.GetType().FullName;
						if (vtype.EndsWith("&")==true)
						{
							vtype = vtype.Substring(0,vtype.Length-1);
						}
						
						if (
							(vtype == "System.Int32") || 
							(vtype == "System.Byte") || 
							(vtype == "System.Int16")
							)
						{
							base.InnerList.Add((int) s);
						}
					}
				}
			}
		}

		public int Item(int Index)
		{
			return (int) base.InnerList[Index];
		}
	}

	/// <summary>
	/// Summary description for ReadonlyUriCollection.
	/// </summary>
	public class ReadOnlyUriCollection : ReadOnlyCollectionBase
	{
		public ReadOnlyUriCollection(Uri[] items)
		{
			if (items != null)
			{
				if (items.Length > 0)
				{
					foreach (Uri s in items)
					{
						base.InnerList.Add(s);
					}
				}
			}
		}

		public ReadOnlyUriCollection(ArrayList items)
		{
			if (items != null)
			{
				if (items.Count > 0)
				{
					foreach (object s in items)
					{
						string vtype = s.GetType().FullName;
						if (vtype.EndsWith("&")==true)
						{
							vtype = vtype.Substring(0,vtype.Length-1);
						}
						
						if ((vtype == "System.Uri"))
						{
							base.InnerList.Add((Uri) s);
						}
					}
				}
			}
		}

		public Uri Item(int Index)
		{
			return (Uri) base.InnerList[Index];
		}
	}
	*/
}
