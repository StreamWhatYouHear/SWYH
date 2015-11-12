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
using System.Text;
using System.Threading;
using System.Collections;

namespace OpenSource.UPnP.AV.CdsMetadata
{
	public sealed class HashtableCache
	{
		private Hashtable Cache = null;
		private ReaderWriterLock Lock;

		public HashtableCache(int size)
		{
			this.Init(new Hashtable(size));
		}

		public HashtableCache()
		{
			this.Init(new Hashtable());
		}

		private void Init(Hashtable cache)
		{
			this.Cache = cache;
			if (MediaObject.UseStaticLock)
			{
				this.Lock = MediaObject.StaticLock;
			}
			else
			{
				this.Lock = new ReaderWriterLock();
			}
		}

		public void Clear()
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

		public object CacheThis(object key, object obj)
		{
			//bool cached = false;
			object retVal = null;

			try
			{
				this.Lock.AcquireReaderLock(-1);
				retVal = this.Cache[key];
			}
			finally
			{
				this.Lock.ReleaseReaderLock();
			}

			if (retVal == null)
			{
				try
				{
					this.Lock.AcquireWriterLock(-1);
					this.Cache[key] = obj;
				}
				finally
				{
					this.Lock.ReleaseWriterLock();
				}
				retVal = obj;
			}

			return retVal;
		}
	}

	public sealed class Cache_MediaClass
	{
		private HashtableCache Cache = new HashtableCache();

		public MediaClass CacheThis(string fullName, string friendlyName)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("{0}{1}{2}", fullName, Delimiter, friendlyName);
			return (MediaClass) this.Cache.CacheThis(sb.ToString(), new MediaClass(fullName, friendlyName));
		}

		private const string Delimiter = "?=";
	}

	public sealed class Cache_ProtocolInfoString
	{
		private HashtableCache Cache = new HashtableCache();

		public ProtocolInfoString CacheThis(string protInfo)
		{
			return (ProtocolInfoString) this.Cache.CacheThis(protInfo, new ProtocolInfoString(protInfo));
		}
	}


	public sealed class Cache_PropertyEnumWriteStatus
	{
		private HashtableCache Cache = new HashtableCache(5);

		public PropertyEnumWriteStatus CacheThis(string namespace_tag, EnumWriteStatus val)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("{0}{1}{2}", namespace_tag, Delimiter, val.ToString());
			return (PropertyEnumWriteStatus) this.Cache.CacheThis(sb.ToString(), new PropertyEnumWriteStatus(namespace_tag, val));
		}

		private const string Delimiter = "?=";
	}


	public sealed class Cache_PropertyStorageMedium
	{
		private HashtableCache Cache = new HashtableCache(3);

		public PropertyStorageMedium CacheThis(string namespace_tag, StorageMediums.Enum val)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("{0}{1}{2}", namespace_tag, Delimiter, val.ToString());
			return (PropertyStorageMedium) this.Cache.CacheThis(sb.ToString(), new PropertyStorageMedium(namespace_tag, val));
		}

		private const string Delimiter = "?=";
	}

	/// <summary>
	/// Summary description for CdsMetadataCaches.
	/// </summary>
	public abstract class CdsMetadataCaches
	{
		public static SortedObjectCache Data = new SortedObjectCache(new StringComparer(), 500);
		public static SortedObjectCache CommonData = new SortedObjectCache(new StringComparer(), 80);
		public static SortedObjectCache Didl = new SortedObjectCache(new StringComparer(), 20);
		
		public static Cache_MediaClass MediaClasses = new Cache_MediaClass();
		public static Cache_ProtocolInfoString ProtocolInfoStrings = new Cache_ProtocolInfoString();
		public static Cache_PropertyEnumWriteStatus PropertyEnumWriteStatus = new Cache_PropertyEnumWriteStatus();
		public static Cache_PropertyStorageMedium PropertyStorageMediums = new Cache_PropertyStorageMedium();
	}
}
