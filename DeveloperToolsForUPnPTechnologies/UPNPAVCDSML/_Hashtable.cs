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
using System.Runtime.Serialization;

namespace OpenSource.UPnP.AV.CdsMetadata
{
	/// <summary>
	/// Implements a Hashtable object using an ArrayList.
	/// Hashtables are usually about 2-3 times bigger
	/// than ArrayLists with thousands of items. An ArrayList
	/// is even better by a factor of 3 against a HybridDictionary 
	/// with a collection size of 10.
	/// </summary>
	public class _Hashtable //: IDictionary, ICollection, IEnumerable, ICloneable
	{
		/// <summary>
		/// Adds an element with the provided key and value to the IDictionary.
		/// </summary>
		/// <param name="AL"></param>
		/// <param name="key"></param>
		/// <param name="val"></param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when key or val is null.
		/// </exception>
		public static void Add (ArrayList AL, object key, object val)
		{
			if ((key == null) || (val == null))
			{
				throw new ArgumentNullException("Cannot have null key or val.");
			}

			//lock (AL.SyncRoot)
			{
				foreach (DictionaryEntry de in AL)
				{
					if (de.Key.Equals(key))
					{
						throw new ArgumentException("Duplicate key");
					}
				}

				DictionaryEntry newEntry = new DictionaryEntry();
				newEntry.Key = key;
				newEntry.Value = val;
				AL.Add(newEntry);
			}
		}

		/// <summary>
		/// Removes a value from the ArrayList with the DictionarEntry items given a key.
		/// </summary>
		/// <param name="AL"></param>
		/// <param name="key"></param>
		public static void Remove (ArrayList AL, object key)
		{
			//lock (AL.SyncRoot)
			{
				int i=0;
				bool remove = false;
				foreach (DictionaryEntry de in AL)
				{
					if (de.Key.Equals(key))
					{
						remove = true;
						break;
					}
					i++;
				}
				if (remove)
				{
					AL.RemoveAt(i);
				}
			}
		}

		/// <summary>
		/// Determines whether the IDictionary contains an element with the specified key.
		/// </summary>
		/// <param name="key">The key to locate in the IDictionary. </param>
		/// <param name="AL"></param>
		/// <returns></returns>
		public static bool Contains (ArrayList AL, object key)
		{
			//lock (AL.SyncRoot)
			{
				foreach (DictionaryEntry de in AL)
				{
					if (de.Key.Equals (key))
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Returns Contains(ArrayList, object)
		/// </summary>
		/// <param name="AL"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static bool ContainsKey (ArrayList AL, object key)
		{
			return Contains(AL, key);
		}


		/// <summary>
		/// Returns an IDictionaryEnumerator for the IDictionary.
		/// </summary>
		/// <returns></returns>
		public static IDictionaryEnumerator GetEnumerator(ArrayList AL)
		{
			return new _HashtableEnumerator(AL);
		}
		
		/// <summary>
		/// Returns the values found in an ArrayList that has IDictionaryElements.
		/// </summary>
		/// <param name="AL"></param>
		/// <returns></returns>
		public static ICollection Values(ArrayList AL)
		{
			return new _HashtableValueCollection(AL);
		}

		/// <summary>
		/// Returns the keys found in an ArrayList that has IDictionaryElements.
		/// </summary>
		/// <param name="AL"></param>
		/// <returns></returns>
		public static ICollection Keys (ArrayList AL)
		{
			return new _HashtableKeyCollection(AL);
		}

		/// <summary>
		/// Returns the value of an item in an ArrayList that has IDictionaryElements,
		/// using the specified key.
		/// </summary>
		/// <param name="AL"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static object Get(ArrayList AL, object key)
		{
			//lock (AL.SyncRoot)
			{
				foreach (DictionaryEntry de in AL)
				{
					if (de.Key.Equals(key))
					{
						return de.Value;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Given a list of keys, returns the results in an
		/// ArrayList, where the order of the results matches
		/// the order of the provided keys.
		/// </summary>
		/// <param name="AL"></param>
		/// <param name="keys"></param>
		/// <returns></returns>
		public static Hashtable Get(Hashtable results, ArrayList AL, object[] keys)
		{
			//lock (AL.SyncRoot)
			{
				foreach (object k in keys)
				{
					foreach (DictionaryEntry de in AL)
					{
						if (de.Key.Equals(k))
						{
							results.Add(k, de.Value);
							break;
						}
					}
				}
			}

			return results;
		}


		public static void Set(ArrayList AL, object key, object val)
		{
			//lock (AL.SyncRoot)
			{
				for (int i=0; i < AL.Count; i++)
				{
					DictionaryEntry de = (DictionaryEntry) AL[i];
					if (de.Key.Equals(key))
					{
						if (val == null)
						{
							AL.RemoveAt(i);
						}
						else
						{
							de.Value = val;
							AL[i] = de;
						}
						return;
					}
				}

				if (val != null)
				{
					Add(AL, key, val);
				}
			}
		}
		

		/// <summary>
		/// Analogous to HashtableValueCollection.
		/// </summary>
		[Serializable()]
		private class _HashtableValueCollection : ICollection
		{
			public _HashtableValueCollection(ArrayList al)
			{
				this.AL = al;
			}
			public int Count
			{
				get
				{
					return this.AL.Count;
				}
			}
			public bool IsSynchronized
			{
				get
				{
					return this.AL.IsSynchronized;
				}
			}
			public object SyncRoot
			{
				get
				{
					return this.AL.SyncRoot;
				}
			}
			public void CopyTo(Array array, int index)
			{
				foreach (DictionaryEntry de in this.AL)
				{
					array.SetValue(de.Value, index);
					index++;
				}
			}
			public IEnumerator GetEnumerator()
			{
				return new _HashtableKeyValueEnumerator(this.AL, false);
			}
			private ArrayList AL;
		}

		[Serializable()]
		/// <summary>
		/// Analogous to HashtableKeyCollection.
		/// </summary>
		private class _HashtableKeyCollection : ICollection
		{
			public _HashtableKeyCollection(ArrayList al)
			{
				this.AL = al;
			}
			public int Count
			{
				get
				{
					return this.AL.Count;
				}
			}
			public bool IsSynchronized
			{
				get
				{
					return this.AL.IsSynchronized;
				}
			}
			public object SyncRoot
			{
				get
				{
					return this.AL.SyncRoot;
				}
			}
			public void CopyTo(Array array, int index)
			{
				foreach (DictionaryEntry de in this.AL)
				{
					array.SetValue(de.Value, index);
					index++;
				}
			}
			public IEnumerator GetEnumerator()
			{
				return new _HashtableKeyValueEnumerator(this.AL, true);
			}
			private ArrayList AL;
		}

		[Serializable()]
		/// <summary>
		/// Enumerator for _Hashtable key value pairs.
		/// </summary>
		private class _HashtableKeyValueEnumerator : IEnumerator
		{
			public _HashtableKeyValueEnumerator(ArrayList al, bool keys)
			{
				IE = al.GetEnumerator();
				keyEnumerator = keys;
			}

			public bool MoveNext()
			{
				return IE.MoveNext();
			}

			public void Reset()
			{
				IE.Reset();
			}

			public object Current
			{
				get
				{
					DictionaryEntry de = (DictionaryEntry) IE.Current;
					if (keyEnumerator)
					{
						return de.Key;
					}
					else
					{
						return de.Value;
					}
				}
			}

			IEnumerator IE;
			bool keyEnumerator;
		}

		[Serializable()]
		/// <summary>
		/// Enumerator for _Hashtable.
		/// </summary>
		private class _HashtableEnumerator : IDictionaryEnumerator, IEnumerator
		{
			public _HashtableEnumerator(ArrayList al)
			{
				IE = al.GetEnumerator();
			}

			public DictionaryEntry Entry
			{
				get
				{
					return (DictionaryEntry) this.Current;
				}
			}

			public object Key
			{
				get
				{
					return this.Entry.Key;
				}
			}

			public object Value
			{
				get
				{
					return this.Entry.Value;
				}
			}

			public object Current
			{
				get
				{
					return IE.Current;
				}
			}

			public bool MoveNext()
			{
				return IE.MoveNext();
			}

			public void Reset()
			{
				IE.Reset();
			}

			private IEnumerator IE;
		}
	}
}
