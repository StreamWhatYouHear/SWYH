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
using System.Threading;

namespace OpenSource.UPnP.AV.CdsMetadata
{
	public class StringComparer : IComparer
	{
		public int Compare(object sv1, object sv2)
		{
			return String.Compare((string)sv1, (string)sv2);
		}
	}

	/// <summary>
	/// Caches a list of sortable objects.
	/// </summary>
	public sealed class SortedObjectCache
	{
		/// <summary>
		/// Sorted list of objects to cache.
		/// </summary>
		private ArrayList Cache = null;

		/// <summary>
		/// Sorting method for the cache.
		/// </summary>
		private _SortedList Sorter = null;

		/// <summary>
		/// Provides thread safety for the cache.
		/// </summary>
		private ReaderWriterLock Lock = null;

		/// <summary>
		/// Constructs an object cache that is sorted.
		/// </summary>
		/// <param name="sorter">
		/// Each object added to the cache must be comparable
		/// with the provided <see cref="IComparer"/> instance.
		/// </param>
		public SortedObjectCache(IComparer sorter)
		{
			this.Init(sorter, new ArrayList());
		}

		/// <summary>
		/// Constructs an object cache that is sorted
		/// and initializes internal array size.
		/// </summary>
		/// <param name="sorter">
		/// Each object added to the cache must be comparable
		/// with the provided <see cref="IComparer"/> instance.
		/// </param>
		/// <param name="size">
		/// Initial size of the internal arraylist.
		/// </param>
		public SortedObjectCache(IComparer sorter, int size)
		{
			this.Init(sorter, new ArrayList(size));
		}

		/// <summary>
		/// Initializes all fields.
		/// </summary>
		/// <param name="sorter"></param>
		/// <param name="al"></param>
		private void Init(IComparer sorter, ArrayList al)
		{
			this.Cache = al;
			this.Sorter = new _SortedList(sorter, false);
			this.Lock = new ReaderWriterLock();
		}

		/// <summary>
		/// Checks the cache for an object that has the same value.
		/// If an object does exist, return it. Otherwise, add it to the cache
		/// and return the sent object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public object CacheThis(object obj)
		{
			int index;
			object retVal;

			if (obj == null) return obj;
			
			// attempt to find duplicate instance with same value properties

			this.Lock.AcquireReaderLock(-1);
			index = this.Sorter.Get(this.Cache, obj);
			this.Lock.ReleaseReaderLock();

			if (index < 0)
			{
				// duplicate object not found
				try
				{
					this.Lock.AcquireWriterLock(-1);
					this.Sorter.Set(this.Cache, obj, false);
				}
				finally
				{
					this.Lock.ReleaseWriterLock();
				}
				retVal = obj;
			}
			else
			{
				retVal = this.Cache[index];
			}

			return retVal;
		}

		public int Size
		{
			get
			{
				this.Lock.AcquireReaderLock(-1);
				int count = this.Cache.Count;
				this.Lock.ReleaseReaderLock();

				return count;
			}
		}

		public void ClearCache()
		{
			try
			{
				this.Lock.AcquireWriterLock(-1);
				this.Cache.Clear();
			}
			finally
			{
				this.Lock.ReleaseWriterLock();
			}
		}
	}
}
