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
	public sealed class SortWithIComparable : IComparer
	{
		public int Compare (object obj1, object obj2)
		{
			return ((IComparable) obj1).CompareTo(obj2);
		}
	}

	public class KeyCollisionException : Exception
	{
		public KeyCollisionException(string info) : base (info){}
	}
	/// <summary>
	/// This class allows a programmer to take an ArrayList and use it like a sorted list.
	/// The advantage is that the class is simply a set of functions for manipulating
	/// an arraylist. The limitation is that the sorting key has to be the same as the
	/// value object and that the programmer must also supply an IComparer at construction.
	/// </summary>
	public class _SortedList
	{
		/// <summary>
		/// Instantiates an object that manages an ArrayList so that the
		/// items in the ArrayList are sorted by an IComparer.
		/// </summary>
		/// <param name="comparer">
		/// IComparer that will keep the list
		/// sorted during Set() operations. If
		/// null, then the standard 
		/// <see cref="SortWithIComparable"/>
		/// comparer is used.
		/// </param>
		/// <param name="allowCollisions">
		/// If true, then we allow key collisions. Be warned
		/// that this effectively makes the Get() operation 
		/// useless on the array because multiple objects
		/// in the array may evaluate to the same key value.
		/// </param>
		public _SortedList (IComparer comparer, bool allowCollisions)
		{
			if (comparer == null)
			{
				comparer = DefaultSorter;
			}
			this.Comparer = comparer;
			this.AllowKeyCollisions = allowCollisions;
			this.KeepUnsorted = false;
		}

		/// <summary>
		/// Instantiates an object that manages an ArrayList so that the
		/// items in the ArrayList are sorted by an IComparer.
		/// </summary>
		/// <param name="comparer">
		/// IComparer that will keep the list
		/// sorted during Set() operations. If
		/// null, then the standard 
		/// <see cref="SortWithIComparable"/>
		/// comparer is used.
		/// </param>
		/// <param name="allowCollisions">
		/// If true, then we allow key collisions. Be warned
		/// that this effectively makes the Get() operation 
		/// useless on the array because multiple objects
		/// in the array may evaluate to the same key value.
		/// <para>
		/// The only scenario where this value should be true is
		/// when the programmer wants to keep a sorted list of items
		/// but has no desire in obtaining individual items through
		/// using objects that can act as a key after the list is created.
		/// </para>
		/// </param>
		/// <param name="keepUnsorted">
		/// If true, the ArrayList behaves like a normal ArrayList,
		/// except that the IComparer can still be used to find
		/// objects as if they were hashed. The search time however
		/// becomes O(n).
		/// <para>
		/// If false, then we actually want sorting and the search
		/// time is O(log n).
		/// </para>
		/// </param>
		public _SortedList (IComparer comparer, bool allowCollisions, bool keepUnsorted)
		{
			if (comparer == null)
			{
				comparer = DefaultSorter;
			}
			this.Comparer = comparer;
			this.AllowKeyCollisions = allowCollisions;
			this.KeepUnsorted = keepUnsorted;
		}

		private static SortWithIComparable DefaultSorter = new SortWithIComparable();
		public readonly IComparer Comparer;
		//private ReaderWriterLock m_Lock = new ReaderWriterLock();
		public readonly bool AllowKeyCollisions;
		public readonly bool KeepUnsorted;

		/// <summary>
		/// Returns the index where an object can be found
		/// such that the provided object has the same
		/// key value as the object found at the returned
		/// index. 
		/// </para>
		/// </summary>
		/// <param name="AL">
		/// The arraylist to search.
		/// Must assume that the arraylist was built entirely
		/// using _SortedList methods.
		/// </param>
		/// <param name="objectAsKey">
		/// The object to use for keying... the object is not
		/// assumed to be of the same instance as the target value.
		/// The argument is simply used in the Compare method
		/// of the IComparer supplied at construction time.
		/// </param>
		/// <returns>
		/// An index into AL where the first object can be found
		/// that had the same match against objectAsKey
		/// according to the comparer supplied at construction time.
		/// </returns>
		/// <exception cref="KeyCollsionException">
		/// Thrown if the _SortedList is not configured to prevent
		/// key collisions.
		/// </exception>
		public int Get(ArrayList AL, object objectAsKey)
		{
			if (AL == null)
			{
				return -1;
			}

			if (this.AllowKeyCollisions == true)
			{
				throw new KeyCollisionException("_SortedList cannot do Get() when configured to allow key collisions.");
			}

			//this.m_Lock.AcquireReaderLock(-1);

			if (this.KeepUnsorted == false)
			{
				int cmp = 1;
				int middle = 0;
				int left = 0;
				int right = AL.Count;

				object middleObj;

				while (left <= right)
				{
					if (AL.Count == 0)
					{
						middle = 0;
						cmp = 1;
						break;
					}

					int offset = (right - left) / 2; 
					middle = left + offset;

					middleObj = AL[middle];

					cmp = this.Comparer.Compare(middleObj, objectAsKey);

					if ((left == right) || (left == right-1))
					{
						break;
					}

					if (cmp == 0)
					{
						break;
					}
					else if (cmp > 0)
					{
						// middleObj should come after insertThis
						right = middle;
					}
					else if (cmp < 0)
					{
						// middleObj should come before insertThis
						left = middle;
					}
				}

				if (cmp == 0)
				{
					//this.m_Lock.ReleaseReaderLock();
					return middle;
				}
			}
			else
			{
				// iterate linearly
				int i=0;
				foreach (object obj in AL)
				{
					int cmp = this.Comparer.Compare(obj, objectAsKey);

					if (cmp == 0)
					{
						//this.m_Lock.ReleaseReaderLock();
						return i;
					}
					i++;
				}
			}

			//this.m_Lock.ReleaseReaderLock();
			return -1;
		}

		/// <summary>
		/// Inserts an object into the ArrayList in such a way
		/// that the elements in the ArrayList are sorted
		/// using the Comparer supplied at construction time.
		/// </summary>
		/// <param name="AL">
		/// The ArrayList to perform the operation on.
		/// Must assume that the arraylist has been built
		/// using a _SortedList object that uses the
		/// same IComparer.
		/// </param>
		/// <param name="objectAsKey">
		/// Add this object.
		/// </param>
		/// <param name="overwrite">
		/// If true, overwrites the existing object
		/// that has the same key. This argument
		/// is ignored if the _SortedList object is
		/// configured to allow key collisions.
		/// </param>
		/// <returns>
		/// True if a value object with a the specified key was overwritten.
		/// </returns>
		/// <exception cref="KeyCollsionException">
		/// Thrown if the _SortedList is not configured to prevent
		/// key collisions and an existing object has the same key
		/// and we're not allowing overwrites.
		/// </exception>
		public bool Set(ArrayList AL, object objectAsKey, bool overwrite)
		{

			bool result = false;
			//this.m_Lock.AcquireWriterLock(-1);
			Exception error = null;
			result = Set(AL, objectAsKey, overwrite, out error);
			//this.m_Lock.ReleaseWriterLock();

			if (error != null)
			{
				throw new ApplicationException("_SortedList.Set() error", error);
			}
            
			return result;
		}

		public void Add(ArrayList AL, ICollection objects, bool overwrite)
		{
			//this.m_Lock.AcquireWriterLock(-1);
			Exception error = null;

			foreach (object objectAsKey in objects)
			{
				Set(AL, objectAsKey, overwrite, out error);
				if (error != null)
				{
					break;
				}
			}

			//this.m_Lock.ReleaseWriterLock();

			if (error != null)
			{
				throw new ApplicationException("_SortedList.Add() error", error);
			}
		}

		private bool Set(ArrayList AL, object objectAsKey, bool overwrite, out Exception error)
		{
			bool result = false;
			error = null;
			if (this.KeepUnsorted == false)
			{
				int cmp = 0;
				int middle = 0;
				int left = 0;
				int right = AL.Count;
				object middleObj;
				bool collision = false;

				try
				{
					while (left <= right)
					{
						if (AL.Count == 0)
						{
							middle = 0;
							cmp = 1;
							break;
						}

						int offset = (right - left) / 2; 
						middle = left + offset;

						middleObj = AL[middle];

						cmp = this.Comparer.Compare(middleObj, objectAsKey);

						if ((left == right) || (left == right-1))
						{
							break;
						}

						if (cmp == 0)
						{
							break;
						}
						else if (cmp > 0)
						{
							// middleObj should come after insertThis
							right = middle;
						}
						else if (cmp < 0)
						{
							// middleObj should come before insertThis
							left = middle;
						}
					}

					if (middle >= AL.Count)
					{
						AL.Add(objectAsKey);
					}
					else if (cmp > 0)
					{
						// insert objectAsKey before middleObj
						AL.Insert(middle, objectAsKey);
					}
					else if (cmp < 0)
					{
						// insert objectAsKey after middleObj
						AL.Insert(middle+1, objectAsKey);
					}
					else
					{
						if (this.AllowKeyCollisions)
						{
							// insert objectAsKey before middleObj
							AL.Insert(middle, objectAsKey);
						}
						else
						{
							if (overwrite)
							{
								if (objectAsKey == null)
								{
									AL.RemoveAt(middle);
								}
								else
								{
									AL[middle] = objectAsKey;
								}
								result = true;
							}
							else
							{
								collision = true;
							}
						}
					}
				}
				catch (Exception e)
				{
					error = e;
					return result;
				}

				if (collision)
				{
					error = new KeyCollisionException("_SortedList cannot do a Set() with the provided object because of a collision.");
				}
			}
			else
			{
				if ((this.AllowKeyCollisions) && (overwrite == false))
				{
					// we allow collisions but we don't want to overwrite, so simply continue
					AL.Add(objectAsKey);
					result = true;
				}
				else
				{
					// iterate linearly until we find the correct index to overwrite
					for (int i=0; i < AL.Count; i++)
					{
						object obj = AL[i];
						int cmp = this.Comparer.Compare(obj, objectAsKey);

						if (cmp == 0)
						{
							if (overwrite)
							{
								//items match and we want to overwrite
								AL[i] = objectAsKey;
								result = true;
								break;
							}
							else
							{
								if (this.AllowKeyCollisions == false)
								{
									//items match and we don't allow collisions
									error = new KeyCollisionException("_SortedList cannot do a Set() with the provided object because of a collision.");
								}
							}
						}
						else
						{
							//items don't match so continue searching linearly
						}
					}
					//if we get here and result==false, then we never found
					//a collision so go ahead and add the object
					if (result == false)
					{
						AL.Add(objectAsKey);
						result = true;
					}
				}
			}

			return result;
		}


	}
}
