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
using System.Threading;
using System.Reflection;
using System.Collections;
using OpenSource.UPnP.AV;
using OpenSource.Utilities;
using System.Runtime.Serialization;

namespace OpenSource.UPnP.AV.CdsMetadata
{
	/// <summary>
	/// <para>
	/// This class inherits all basic metadata of a ContentDirectory media entry,
	/// and extends it to support the UPNP-AV Forum defined concept of an "object.container" 
	/// media class and its derived subclasses.
	/// </para>
	/// <para>
	/// All public operations are thread-safe and all returned
	/// data is copy-safe or the data objects provide 
	/// read-only public interfaces.
	/// </para>	
	/// </summary>
	[Serializable()]
	public class MediaContainer : MediaObject, IUPnPMedia, IMediaContainer
	{
		/// <summary>
		/// Special ISerializable constructor.
		/// Do basic initialization and then serialize from the info object.
		/// Serialized MediaContainer objects do not have their child objects
		/// serialized with them.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected MediaContainer(SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base (info, context)
		{
			// Base class constructor calls Init() so all fields are
			// initialized. 
			System.Threading.Interlocked.Increment(ref ContainerCounter);
		}

		/// <summary>
		/// Custom serializer - required for ISerializable.
		/// Serializes all fields that are not marked as [NonSerialized()].
		/// Some fields were originally marked as [NonSerialized()] because
		/// this class did not implement ISerializable. I've continued to
		/// use the attribute in the code.
		/// 
		/// Serialized MediaContainer objects do not have their child objects
		/// serialized with them nor do media container objects serialize
		/// the underlying value for the <see cref="MediaContainer.UpdateID"/>
		/// property.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public override void GetObjectData(SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		~MediaContainer()
		{
			System.Threading.Interlocked.Decrement(ref ContainerCounter);
			this.m_Listing = null;
		}
		/// <summary>
		/// Tracks the number of containers that have been created.
		/// </summary>
		private static long ContainerCounter = 0;

		/// <summary>
		/// Returns false.
		/// </summary>
		override public bool IsReference				{ get { return false ; } }

		/// <summary>
		/// Returns true, if the container is intended to be searchable.
		/// Some containers may not allow a control point to issue a search
		/// from the container.
		/// </summary>
		public override bool IsSearchable			
		{ 
			get 
			{
				return m_Searchable; 
			} 
			set
			{
				this.m_Searchable = value;
			}
		}

		/// <summary>
		/// Every container has a value, indicating its
		/// current state. The value is not guaranteed to be
		/// monotomically increasing, so control points should
		/// simply check against the current value to determine
		/// if they have the most recent state of the container.
		/// </summary>
		public UInt32 UpdateID				{ get { return this.m_UpdateID; } }

		/// <summary>
		/// Returns the number of child containers and items in CompleteList.
		/// Using this method is faster than using CompleteList.Count because
		/// the latter method actually builds a copy of the child list to
		/// obtain the size.
		/// </summary>
		public virtual int ChildCount
		{
			get
			{
				if (this.m_Listing != null)
				{
					return this.m_Listing.Count;
				}

				return 0;
			}
		}

		/// <summary>
		/// Returns true if Parent==null or ParentID == -1.
		/// </summary>
		public virtual bool IsRootContainer
		{
			get
			{
				if (
					(this.Parent == null) ||
					(this.ParentID == "-1")
					)
				{
					return true;
				}

				return false;
			}
		}

		/// <summary>
		/// <para>
		/// Returns a listing of IUPnPMedia objects that can be cast into
		/// MediaContainer or MediaItem classes. The returned list a shallow
		/// copy of the child list, so enumerating on the list is always safe.
		/// </para>
		/// 
		/// <para>
		/// If intending to enumerate through the list multiple times in
		/// a short period of time, saving and using a reference to this property
		/// will lead to better performance as it will reduce the number of times
		/// a shallow copy of the complete list is generated.
		/// </para>
		/// </summary>
		public virtual IList CompleteList		
		{
			get
			{
				this.m_LockListing.AcquireReaderLock(-1);
				ArrayList c;
				if (this.m_Listing != null)
				{
					c = new ArrayList(this.m_Listing);
				}
				else
				{
					c = new ArrayList(0);
				}
				this.m_LockListing.ReleaseReaderLock();
				return c;
			}
		}
		
		/// <summary>
		/// Returns a subset of the CompleteList, where all elements are MediaContainer
		/// instances.
		/// </summary>
		public virtual IList Containers
		{
			get 
			{
				this.m_LockListing.AcquireReaderLock(-1);
				ArrayList c;
				if (this.m_Listing != null)
				{
					c = new ArrayList(m_Listing.Count);
					foreach (IUPnPMedia item in this.m_Listing)
					{
						if (item.IsContainer)
						{
							c.Add(item);
						}
					}
				}
				else
				{
					c = new ArrayList(0);
				}
				this.m_LockListing.ReleaseReaderLock();
				return c;
			} 
		}
		
		/// <summary>
		/// Returns a subset of the CompleteList, where all elements are MediaItem instances.
		/// </summary>
		public virtual IList Items
		{
			get 
			{
				this.m_LockListing.AcquireReaderLock(-1);
				ArrayList c;
				if (this.m_Listing != null)
				{
					c = new ArrayList(this.m_Listing.Count);

					foreach (IUPnPMedia item in this.m_Listing)
					{
						if (item.IsItem)
						{
							c.Add(item);
						}
					}
				}
				else
				{
					c = new ArrayList(0);
				}
				this.m_LockListing.ReleaseReaderLock();
				return c;
			} 
		}
		
		/// <summary>
		/// Returns same values as base.Properties.
		/// </summary>
		public override IMediaProperties MergedProperties
		{
			get
			{
				return base.Properties;
			}
		}

		/// <summary>
		/// Returns same values as base.Resources.
		/// </summary>
		public override IMediaResource[] MergedResources
		{
			get
			{
				return base.Resources;
			}
		}

		/// <summary>
		/// <para>
		/// Returns a listing of 
		/// <see cref="SearchClass"/>
		/// instances representing the types of media objects that can be 
		/// returned in a Search request applied to this container.
		/// </para>
		/// 
		/// <para>
		/// TODO: Should implement something that actually employes SearchClasses in the device-side implementation
		/// of CDS.
		/// </para>
		/// </summary>
		public IList SearchClasses
		{
			get
			{
				IList ret = this.m_Properties[T[_UPNP.searchClass]];
				if (ret == null) ret = new ArrayList();
				return ret;
			}
		}

		/// <summary>
		/// Returns a listing of 
		/// <see cref="CreateClass"/>
		/// instances representing the types of media objects that can be created under this container.
		/// 
		/// <para>
		/// TODO: Should implement something that actually employs CreateClasses in the device-side implementation
		/// of CDS.
		/// </para>
		/// </summary>
		public IList CreateClasses
		{
			get
			{
				IList ret = this.m_Properties[T[_UPNP.createClass]];
				if (ret == null) ret = new ArrayList();
				return ret;
			}
		}
		
		/// <summary>
		/// <para>
		/// Adds a search class category for the container. Search classes give indication
		/// of what types of items are found as descendents or children of a container.
		/// </para>
		/// 
		/// <para>
		/// TODO: Should implement something that actually employs SearchClasses in the device-side implementation
		/// of CDS.
		/// </para>
		/// </summary>
		/// <param name="sc"></param>
		internal void AddSearchClass (SearchClass sc)
		{
			this.m_Properties.AddVal(T[_UPNP.searchClass], sc);
		}

		/// <summary>
		/// <para>
		/// Removes a search class category for the container. Search classes give indication
		/// of what types of items are found as descendents or children of a container.
		/// </para>
		/// 
		/// <para>
		/// TODO: Should implement something that actually employs SearchClasses in the device-side implementation
		/// of CDS.
		/// </para>
		/// </summary>
		/// <param name="sc"></param>
		internal void RemoveSearchClass (SearchClass sc)
		{
			this.m_Properties.RemoveVal (T[_UPNP.searchClass], sc);
		}

		/// <summary>
		/// <para>
		/// Adds a create class category for the container. Create classes give indication
		/// of what types of items can be created as descendents or children of a container.
		/// </para>
		/// 
		/// <para>
		/// TODO: Should implement something that actually employs CreateClasses in the device-side implementation
		/// of CDS.
		/// </para>
		/// </summary>
		/// <param name="cc"></param>
		internal void AddCreateClass (CreateClass cc)
		{
			this.m_Properties.AddVal(T[_UPNP.createClass], cc);
		}

		/// <summary>
		/// <para>
		/// Adds a create class category for the container. Create classes give indication
		/// of what types of items can be created as descendents or children of a container.
		/// </para>
		/// 
		/// <para>
		/// TODO: Should implement something that actually employs CreateClasses in the device-side implementation
		/// of CDS.
		/// </para>
		/// </summary>
		/// <param name="cc"></param>
		internal void RemoveCreateClass (CreateClass cc)
		{
			this.m_Properties.RemoveVal (T[_UPNP.createClass], cc);
		}

		/// <summary>
		/// Default constructor. No metadata is initialized in this
		/// method. It is STRONGLY recommended that programmers
		/// use the MediaBuilder.CreateXXX methods to instantiate
		/// MediaContainer objects.
		/// </summary>
		public MediaContainer() : base()
		{
			System.Threading.Interlocked.Increment(ref ContainerCounter);
			this.SetClass("object.container", "");
			this.m_Searchable = true;
		}

		/// <summary>
		/// <para>
		/// The constructor expects an XmlElement
		/// representing a DIDL-Lite "item" or "container"
		/// element. 
		/// </para>
		/// <para>
		/// Constructors of derived classes (that have the same signature)
		/// should call this base class constructor only to initialize things.
		/// Afterwards they should directly UpdateEverything(), which allows 
		/// the programmer to specify what type of
		/// resources, items, and containers to instantiate if such
		/// elements are encountered in the XML. The types specified
		/// for resources, items, and containers must all be 
		/// classes that can be instantiated from a single
		/// XmlElement. 
		/// </para>
		/// </summary>
		/// <param name="xmlElement">XmlElement that represent a DIDL-Lite container element</param>
		public MediaContainer(XmlElement xmlElement)
			: base(xmlElement)
		{
			System.Threading.Interlocked.Increment(ref ContainerCounter);
			this.m_Searchable = true;
		}

		/// <summary>
		/// Makes it so that a MediaContainer instantiated from an XmlElement
		/// instantiates its child resources as <see cref="MediaResource"/> objects,
		/// and child items and containers are <see cref="MediaItem"/> and <see cref="MediaContainer"/>.
		/// <para>
		/// Derived classes that expect different types for their resources and child
		/// media objects need to override this method.
		/// </para>
		/// </summary>
		/// <param name="xmlElement"></param>
		protected override void FinishInitFromXml(XmlElement xmlElement)
		{
			ArrayList children;
			base.UpdateEverything(true, true, typeof(MediaResource), typeof(MediaItem), typeof(MediaContainer), xmlElement, out children);
			this.AddObjects(children, true);
		}

		/// <summary>
		/// Calls base class implementation of Init()
		/// and then initializes the fields for this class.
		/// </summary>
		protected override void Init()
		{
			base.Init(); 
			this.HashingMethod = HashWithIdNoSorting;
			this.m_Listing = null;

			if (MediaObject.UseStaticLock)
			{
				this.m_LockListing = MediaObject.StaticLock;
			}
			else
			{
				this.m_LockListing = new ReaderWriterLock();
			}

			this.m_UpdateID = 0;
		}

		/// <summary>
		/// Returns true, if the specified media object is a child of the container.
		/// </summary>
		/// <param name="obj">IUPnPMedia object</param>
		/// <returns>True, if the "obj" is a child.</returns>
		protected bool HasChild(IUPnPMedia obj)
		{
			bool result = false;
			this.m_LockListing.AcquireReaderLock(-1);

			if (this.m_Listing != null)
			{
				foreach (IUPnPMedia child in this.m_Listing)
				{
					if (child == obj)
					{
						result = true;
						break;
					}
				}
			}

			this.m_LockListing.ReleaseReaderLock();

			return result;
		}

		
		/// <summary>
		/// Get a shallow copy of the listing hashtable
		/// from ID to IUPnPMedia. If no children exist,
		/// then we return an empty hashtable.
		/// </summary>
		/// <returns></returns>
		protected internal virtual Hashtable GetChildren()
		{
			this.m_LockListing.AcquireReaderLock(-1);
			Hashtable retVal;
			if (this.m_Listing != null)
			{
				retVal = (Hashtable) this.m_Listing.Clone();
			}
			else
			{
				retVal = new Hashtable();
			}
			this.m_LockListing.ReleaseReaderLock();

			return retVal;
		}

		/// <summary>
		/// Swaps the current listing of children with a new listing,
		/// where the key is the ID and the value is the IUPnPMedia.
		/// </summary>
		/// <param name="newChildren"></param>
		protected virtual void SwapChildren (ArrayList newChildren)
		{
			try
			{
				this.m_LockListing.AcquireWriterLock(-1);
				ArrayList oldList = m_Listing;
				this.m_Listing = newChildren;

				if (this.m_Listing != null)
				{
					foreach (IUPnPMedia obj in m_Listing)
					{
						obj.Parent = this;
					}
				}

				if (this.m_Listing.Count == 0)
				{
					this.m_Listing = null;
				}

				if (oldList != null)
				{
					foreach (IUPnPMedia obj in oldList)
					{
						IUPnPMedia newObj = null;

						if (this.m_Listing != null)
						{
							int oldI = HashingMethod.Get(this.m_Listing, obj);

							if ((0 <= oldI) && (oldI < this.m_Listing.Count))
							{
								newObj = (IUPnPMedia) this.m_Listing[oldI];
							}
						}

						if (newObj != obj)
						{
							obj.Parent = null;
						}
					}
				}
			}
			finally
			{
				this.m_LockListing.ReleaseWriterLock();
			}
		}

		/// <summary>
		/// This method adds an <see cref="IUPnPMedia"/> (container or item) as a child
		/// of this container.
		/// </summary>
		/// <param name="newObject">the new container or item that should be added</param>
		/// <param name="overWrite">if true, the method will overwrite a current child if it has the same ID</param>
		/// <exception cref="Error_DuplicateIdException">
		/// Thrown if the object being added has the same ID as an existing child.
		/// </exception>
		/// <exception cref="Error_MediaObjectHasParent">
		/// Thrown if the object being added already is a child of another container.
		/// </exception>
		public virtual void AddObject(IUPnPMedia newObject, bool overWrite)
		{
			IUPnPMedia errorParent = null;
			IUPnPMedia errorDuplicate = null;

			try
			{
				this.m_LockListing.AcquireWriterLock(-1);
			
				if (this.m_Listing == null)
				{
					this.m_Listing = new ArrayList();
				}

				if (newObject.Parent != null)
				{
					if (newObject.Parent != this)
					{
						errorParent = newObject;
					}
				}

				try
				{
					HashingMethod.Set(this.m_Listing, newObject, overWrite);
					if (newObject != null)
					{
						newObject.Parent = this;
					}
				}
				catch (KeyCollisionException)
				{
					errorDuplicate = newObject;
				}
			}
			finally
			{
				this.m_LockListing.ReleaseWriterLock();
			}

			if (errorParent != null)
			{
				throw new Error_MediaObjectHasParent(errorParent);
			}
			else if (errorDuplicate != null)
			{
				throw new Error_DuplicateIdException(errorDuplicate);
			}
		}

		/// <summary>
		/// A better performance version of the AddObject() method
		/// that allows multiple media items/containers to be added
		/// to this container.
		/// </summary>
		/// <param name="newObjects">a collection of IUPnPMedia instances to be added</param>
		/// <param name="overWrite">if true, any current child objects with the same ID as an object to be added are overwritten</param>
		/// <exception cref="Error_DuplicateIdException">
		/// Thrown if an object being added has the same ID as an existing child.
		/// </exception>
		/// <exception cref="Error_MediaObjectHasParent">
		/// Thrown if an object being added is a child of another container.
		/// </exception>
		public virtual void AddObjects(ICollection newObjects, bool overWrite)
		{
			IUPnPMedia errorParent = null;
			IUPnPMedia errorDuplicate = null;

			try
			{
				this.m_LockListing.AcquireWriterLock(-1);
				if (this.m_Listing == null)
				{
					this.m_Listing = new ArrayList(newObjects.Count);
				}

				foreach (IUPnPMedia newObject in newObjects)
				{
					if (newObject.Parent != null)
					{
						if (newObject.Parent != this)
						{
							errorParent = newObject;
						}
					}

					try
					{
						HashingMethod.Set(this.m_Listing, newObject, overWrite);
						if (newObject != null)
						{
							newObject.Parent = this;
						}
					}
					catch (KeyCollisionException)
					{
						errorDuplicate = newObject;
						break;
					}
				}
			}
			finally
			{
				this.m_LockListing.ReleaseWriterLock();
			}

			if (errorParent != null)
			{
				throw new Error_MediaObjectHasParent(errorParent);
			}
			else if (errorDuplicate != null)
			{
				throw new Error_DuplicateIdException(errorDuplicate);
			}
		}

		/// <summary>
		/// Removes an object from the child list.
		/// </summary>
		/// <param name="removeThis">Mediaobject to remove</param>
		public virtual void RemoveObject (IUPnPMedia removeThis)
		{
			try
			{
				this.m_LockListing.AcquireWriterLock(-1);
				if (this.m_Listing != null)
				{
					int i = HashingMethod.Get(this.m_Listing, removeThis);
					if (i >= 0)
					{
						IUPnPMedia obj = (IUPnPMedia) this.m_Listing[i];
						obj.Parent = null;
						this.m_Listing.RemoveAt(i);
					}

					if (this.m_Listing.Count == 0)
					{
						this.m_Listing = null;
					}
				}
			}
			finally
			{
				this.m_LockListing.ReleaseWriterLock();
			}
		}

		/// <summary>
		/// A better performance version of the AddObject() method
		/// that allows multiple media items/containers to be removed
		/// from this container.
		/// </summary>
		/// <param name="removeThese">a collection IUPnPMedia instances</param>
		public virtual void RemoveObjects (ICollection removeThese)
		{
			Exception error = null;
			try
			{
				this.m_LockListing.AcquireWriterLock(-1);
				if (this.m_Listing != null)
				{
					foreach (IUPnPMedia removeThis in removeThese)
					{
						int i = HashingMethod.Get(this.m_Listing, removeThis);

						if (i >= 0)
						{
							IUPnPMedia obj = (IUPnPMedia) this.m_Listing[i];
							obj.Parent = null;
							this.m_Listing.RemoveAt(i);
						}
					}

					if (this.m_Listing.Count == 0)
					{
						this.m_Listing = null;
					}
				}
			}
			catch (Exception e)
			{
				error =e;
			}
			finally
			{
				this.m_LockListing.ReleaseWriterLock();
			}
			
			if (error != null)
			{
				Exception ne = new Exception("MediaContainer.RemoveObjects()", error);
				throw ne;
			}
		}

		/// <summary>
		/// Traverses this container and all subcontainers in an attempt to find 
		/// a descendent IUPnPMedia with the specified ID. It should be
		/// noted that the "ID" field is an unreliable means of finding a unique
		/// media object in a control point scenario because there is no enforcement
		/// of uniqueness for an item's ID. Container ID's are more reliable because
		/// users must be able to browse from any container in a hierarchy.
		/// </summary>
		/// <param name="id">object with the desired id</param>
		/// <param name="cache">optional param: Updates a hashtable for mapping ID's to WeakReferences of IUPnPmedia instances</param>
		/// <returns></returns>
		public virtual IUPnPMedia GetDescendent(string id, Hashtable cache)
		{
			if (id == this.m_ID)
			{
				return this;
			}

			this.m_LockListing.AcquireReaderLock(-1);

			IUPnPMedia retVal = null;
			if (this.m_Listing != null)
			{
				ArrayList containers = new ArrayList(this.m_Listing.Count);

				foreach (IUPnPMedia entry in this.m_Listing)
				{
					if (cache != null)
					{
						// Update the cache as we find containers and items.
						// 
						bool dirty = true;
						WeakReference wr = (WeakReference) cache[entry.ID];

						if (wr != null)
						{
							if (wr.IsAlive)
							{
								dirty = false;
							}
						}
						if (dirty)
						{
							cache[entry.ID] = new WeakReference(entry);
						}
					}

					// Cache is updated, now check to see if the
					// item is the one we wanted.
					// 
					if (entry.ID == id)
					{
						retVal = entry;
						break;
					}
					else if (entry.IsContainer)
					{
						IMediaContainer container = (IMediaContainer) entry;
						retVal = container.GetDescendent(id, cache);

						if (retVal != null)
						{
							break;
						}
					}
				}
			}
			this.m_LockListing.ReleaseReaderLock();

			return retVal;
		}

		/// <summary>
		/// Exposes the ContentDirectory's Browse action (for returning unsorted results) as a method of a container. 
		/// </summary>
		/// <param name="startingIndex">starting index for the browse results, 0-based index</param>
		/// <param name="requestedCount">maximum number of children requested</param>
		/// <param name="totalMatches">total number of possible matches</param>
		/// <returns></returns>
		public virtual IList Browse(UInt32 startingIndex, UInt32 requestedCount, out UInt32 totalMatches)
		{
			this.m_LockListing.AcquireReaderLock(-1);
			IList results = null;
			if (this.m_Listing != null)
			{
				results = this.BrowseCollection(startingIndex, requestedCount, this.m_Listing, out totalMatches);
			}
			else
			{
				results = new object[0];
				totalMatches = 0;
			}
			this.m_LockListing.ReleaseReaderLock();

			return results;
		}

		/// <summary>
		/// Exposes the ContentDirectory's Browse action (for returning sorted results) as a method of a container. 
		/// </summary>
		/// <param name="startingIndex">starting index for the browse results, 0-based index</param>
		/// <param name="requestedCount">maximum number of children requested</param>
		/// <param name="sorter">IMediaSorter object that determines the sorting criteria for the results</param>
		/// <param name="totalMatches">total number of possible matches</param>
		/// <returns></returns>
		public virtual IList BrowseSorted(UInt32 startingIndex, UInt32 requestedCount, IMediaSorter sorter, out UInt32 totalMatches)
		{
			this.m_LockListing.AcquireReaderLock(-1);
			SortedList sorted = null;
			IList results = null;
			if (this.m_Listing != null)
			{

				sorted = new SortedList(sorter, this.m_Listing.Count);

				//NKIDD-DEBUG : BEGIN - Please preserve as it's useful for finding bugs in the sorting logic
//				MediaContainer tempContainer = new MediaContainer();
//				tempContainer.ID = this.ID;
//				foreach (IUPnPMedia child in this.m_Listing)
//				{
//					IUPnPMedia copy = child.MetadataCopy();
//					copy.ID = child.ID;
//					tempContainer.AddObject(copy, true);
//				}
//				IList other = tempContainer.CompleteList;
//				SortedList sorted2 = new SortedList((IMediaSorter)sorter.Clone(), this.m_Listing.Count);
				//NKIDD-DEBUG : END

				for (int i=0; i < this.m_Listing.Count; i++)
				{
					IUPnPMedia child = (IUPnPMedia) this.m_Listing[i];

					//NKIDD-DEBUG : BEGIN - Please preserve as it's useful for finding bugs in the sorting logic
//					IUPnPMedia child2 = (IUPnPMedia) other[i];
					//NKIDD-DEBUG : END

					try
					{
						//NKIDD-DEBUG : BEGIN - Please preserve as it's useful for finding bugs in the sorting logic
//						IDictionaryEnumerator ide1 = sorted.GetEnumerator();
//						IDictionaryEnumerator ide2 = sorted2.GetEnumerator();
//						for (int j=0; j < sorted2.Count; j++)
//						{
//							ide1.MoveNext();
//							ide2.MoveNext();
//							IUPnPMedia s1 = (IUPnPMedia) ide1.Value;
//							IUPnPMedia s2 = (IUPnPMedia) ide2.Value;
//
//							int cmp1 = sorter.Compare(child, s1);
//							int cmp2 = sorter.Compare(child2, s2);
//
//							if (cmp1 != cmp2)
//							{
//								sorter.Compare(child, s1);
//								sorter.Compare(child2, s2);
//							}
//						}
						//NKIDD-DEBUG : END

						sorted.Add(child, child);

						//NKIDD-DEBUG : BEGIN - Please preserve as it's useful for finding bugs in the sorting logic
//						sorted2.Add(child2, child2);
						//NKIDD-DEBUG : END
					}
					catch (Exception ie)
					{
						throw new ApplicationException("MediaContainer.BrowseSorted() had error adding child to sorted list.", ie);
					}
				}

				results = this.BrowseCollection(startingIndex, requestedCount, sorted.Values, out totalMatches);
			}
			else
			{
				results = new object[0];
				totalMatches = 0;
			}
			this.m_LockListing.ReleaseReaderLock();

			return results;
		}

		/// <summary>
		/// Exposes the ContentDirectory's Search action (for returning unsorted results) as a method of a container. 
		/// </summary>
		/// <param name="expression">IMediaComparer object that will indicate if a media object is a match</param>
		/// <param name="startingIndex">starting index for the search results, 0-based index</param>
		/// <param name="requestedCount">maximum number of children requested</param>
		/// <param name="totalMatches">
		/// total number of possible matches, sometimes has uint.MaxValue if the total possible is unknown. 
		/// CHANGED PER DHWG: Returns 0 if total is unknown.
		/// </param>
		/// <returns></returns>
		public virtual IList Search(IMediaComparer expression, UInt32 startingIndex, UInt32 requestedCount, out UInt32 totalMatches)
		{
			ArrayList results = new ArrayList(2500);
			bool searchedSubtree;
			this.SearchCollection(expression, startingIndex, requestedCount, out searchedSubtree, ref results);

			if (searchedSubtree)
			{
				totalMatches = Convert.ToUInt32(results.Count);
			}
			else
			{
				//totalMatches = UInt32.MaxValue;
				totalMatches = 0;
			}

			return results;
		}

		/// <summary>
		/// Exposes the ContentDirectory's Search action (for returning sorted results) as a method of a container. 
		/// </summary>
		/// <param name="expression">IMediaComparer object that will indicate if a media object is a match</param>
		/// <param name="sorter">IMediaSorter object that determines the sorting criteria for the results</param>
		/// <param name="startingIndex">starting index for the search results, 0-based index</param>
		/// <param name="requestedCount">maximum number of children requested</param>
		/// <param name="totalMatches">total number of possible matches, sometimes has uint.MaxValue if the total possible is unknown</param>
		/// <returns></returns>
		public virtual IList SearchSorted(IMediaComparer expression, IMediaSorter sorter, UInt32 startingIndex, UInt32 requestedCount, out UInt32 totalMatches)
		{
			SortedList sorted = null;
			ArrayList results = null;
			if (this.m_Listing != null)
			{
				sorted = new SortedList(sorter, this.m_Listing.Count);
				this.SearchCollection(expression, ref sorted);

				int size = Convert.ToInt32(requestedCount);

				if (sorted.Count < size) size = sorted.Count;

				results = new ArrayList(size);

				int i=0;
				foreach (IUPnPMedia entry in sorted.Values)
				{
					if (i >= startingIndex)
					{
						results.Add(entry);
					}

					i++;

					if ((results.Count >= requestedCount) && (requestedCount != 0))
					{
						break;
					}
				}

				totalMatches = Convert.ToUInt32(sorted.Count);
			}
			else
			{
				results = new ArrayList(0);
				totalMatches = 0;
			}

			return results;
		}


		/// <summary>
		/// This method removes all child items and containers from this container.
		/// </summary>
		public virtual void ClearItems()
		{
			try
			{
				this.m_LockListing.AcquireWriterLock(-1);
			
				if (this.m_Listing != null)
				{
					IList completeList = this.CompleteList;
					this.RemoveObjects (completeList);
					this.m_Listing = null;
				}
			}
			finally
			{
				this.m_LockListing.ReleaseWriterLock();
			}
		}

		/// <summary>
		/// Implements the core logic for packaging up the results for a browse.
		/// </summary>
		/// <param name="startingIndex">starting index for the search results, 0-based index</param>
		/// <param name="requestedCount">maximum number of children requested</param>
		/// <param name="ic">ICollection of IUPnPMedia objects that represent the total possible set of results. Possibly presorted.</param>
		/// <param name="totalMatches">number of items returned in the ArrayList</param>
		/// <returns>An arraylist of IUPnPMedia objects that represent the subset of the desired browse, possibly sorted.</returns>
		private ArrayList BrowseCollection(UInt32 startingIndex, UInt32 requestedCount, ICollection ic, out UInt32 totalMatches)
		{
			ArrayList results = new ArrayList(ic.Count);

			int i=0;
			foreach (object entry in ic)
			{
				if (i >= startingIndex)
				{
					results.Add(entry);
				}

				i++;

				if ((results.Count >= requestedCount) && (requestedCount != 0))
				{
					break;
				}
			}
			totalMatches = Convert.ToUInt32(ic.Count);

			return results;
		}


		/// <summary>
		/// Implements the core logic for packaging up the results for a Search.
		/// </summary>
		/// <param name="expression">IMediaComparer object that will indicate if a media object is a match</param>
		/// <param name="startingIndex">starting index for the search results, 0-based index</param>
		/// <param name="requestedCount">maximum number of children requested</param>
		/// <param name="searchedEntireSubtree">True, if the entire subtree was searched.</param>
		/// <param name="results">ArrayList of the desired results.</param>
		private void SearchCollection(IMediaComparer expression, UInt32 startingIndex, UInt32 requestedCount, out bool searchedEntireSubtree, ref ArrayList results)
		{
			searchedEntireSubtree = true;
			if (this.m_Listing != null)
			{
				ArrayList containers = new ArrayList(this.m_Listing.Count);
				this.m_LockListing.AcquireReaderLock(-1);
				foreach (IUPnPMedia entry in this.m_Listing)
				{
					// If the item is a match, add it to the list of results.
					// 
					if (expression.IsMatch(entry))
					{
						results.Add(entry);
					}

					// If we've reached the max number of results, then indicate
					// that we will not end up searching the entire subtree.
					// 
					if ((results.Count >= requestedCount) && (requestedCount != 0))
					{
						searchedEntireSubtree = false;
						break;
					}

					if (entry.IsContainer)
					{
						containers.Add(entry);
					}
				}

				this.m_LockListing.ReleaseReaderLock();

				// If we're still intent on searching the entire subtree, then
				// recurse through each sub-container.
				// 
				if (searchedEntireSubtree)
				{
					foreach (MediaContainer container in containers)
					{
						// After recursing each subtree, check to see if we stopped
						// searching the entire subtee. If so, then do not continue.
						if (searchedEntireSubtree)
						{
							container.SearchCollection(expression, startingIndex, requestedCount, out searchedEntireSubtree, ref results);
						}
						else
						{
							break;
						}
					}
				}
			}
		}

		/// <summary>
		/// Implements the core logic for a sorted Search.
		/// </summary>
		/// <param name="exoression">IMediaComparer object that will indicate if a media object is a match</param>
		/// <param name="sorted">sorted list of IUPnPMedia objects that make up result set</param>
		private void SearchCollection(IMediaComparer expression, ref SortedList sorted)
		{
			this.m_LockListing.AcquireReaderLock(-1);
			ArrayList containers = null;
			if (this.m_Listing != null)
			{
				containers = new ArrayList(this.m_Listing.Count);
				foreach (IUPnPMedia entry in this.m_Listing)
				{
					// If the item is a match, add it to the list of results.
					// 
					if (expression.IsMatch(entry))
					{
						sorted.Add(entry,entry);
					}
					if (entry.IsContainer)
					{
						containers.Add(entry);
					}
				}
			}
			this.m_LockListing.ReleaseReaderLock();

			if (containers != null)
			{
				foreach (MediaContainer container in containers)
				{
					container.SearchCollection(expression, ref sorted);
				}
			}
		}

		/// <summary>
		/// The override is provided so that callers can specify their
		/// own implementations that will write the xml. 
		/// This is achieved by setting the <see cref="ToXmlFormatter.WriteContainer"/>
		/// field of the 'formatter'. 
		/// </summary>
		/// <param name="formatter">
		/// Allows the caller to specify a custom implementation that writes
		/// the container's XML. Simply assign the <see cref="ToXmlFormatter.WriteContainer"/>
		/// to have this method delegate the responsibility. Otherwise,
		/// the base class implementation of <see cref="MediaObject.ToXml"/>
		/// is called.
		/// </param>
		/// <param name="data">
		/// If the formatter's <see cref="ToXmlFormatter.WriteContainer"/> field
		/// is non-null, then this object must be a type acceptable to that method's
		/// implementation. Otherwise, a <see cref="ToXmlData"/> object is required.
		/// </param>
		/// <param name="xmlWriter">
		/// This is where the xml gets printed.
		/// </param>
		public override void ToXml(ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter)
		{
			if (formatter.WriteContainer != null)
			{
				formatter.StartElement = null;
				formatter.EndElement = null;
				formatter.WriteInnerXml = null;
				formatter.WriteValue  = null;
				formatter.WriteContainer(this, formatter, data, xmlWriter);
			}
			else
			{
				base.ToXml(formatter, data, xmlWriter);
			}
		}

		/// <summary>
		/// <para>
		/// Programmers assign the <see cref="ToXmlFormatter.StartElement"/>
		/// field to this method, when attempting to print this media object.
		/// </para>
		/// <para>
		/// Algorithm:
		///	1. declare the container element
		///
		///	2. If this object will be added as a direct child
		///	of a container in a CreateObject request, then
		///	do not print the object's ID.
		///
		///	3. Print the searchable attribute, which is required.
		///
		///	4. Print the ID of the item that this object points
		///	to if appropriate. If intending to request
		///	a MediaServer to create a child object that points 
		///	to another object then a control point should
		///	use the CreateReference, so in such a case
		///	this method will throw an exception.
		///
		///	5. Print the parent object, taking into account
		///	CreateObject instructions.
		///
		///	6. Print the restricted attribute.
		///
		///	7. Print the optional childcount attribute, if requested to do so.
		/// </para>
		/// </summary>
		/// <param name="formatter">
		/// A <see cref="ToXmlFormatter"/> object that
		/// specifies method implementations for printing
		/// media objects and metadata.
		/// </param>
		/// <param name="data">
		/// This object should be a <see cref="ToXmlData"/>
		/// object that contains additional instructions used
		/// by the "formatter" argument.
		/// </param>
		/// <param name="xmlWriter">
		/// The <see cref="XmlTextWriter"/> object that
		/// will format the representation in an XML
		/// valid way.
		/// </param>
		/// <exception cref="InvalidCastException">
		/// Thrown when the "data" argument is not a
		/// <see cref="ToXmlData"/> object.
		public override void StartElement (ToXmlFormatter formatter, object data, XmlTextWriter xmlWriter)
		{
			ToXmlData _d = (ToXmlData) data;
			ArrayList desiredProperties = _d.DesiredProperties;

			xmlWriter.WriteStartElement(T[_DIDL.Container]);

			if (_d.CreateObjectParentID != null)
			{
				xmlWriter.WriteAttributeString(T[_ATTRIB.id], "");
			}
			else
			{
				xmlWriter.WriteAttributeString(T[_ATTRIB.id], this.ID);
			}
		
			xmlWriter.WriteAttributeString(T[_ATTRIB.searchable], GetBoolAsOneZero(this.IsSearchable).ToString());

			if (_d.CreateObjectParentID != null)
			{
				xmlWriter.WriteAttributeString(T[_ATTRIB.parentID], _d.CreateObjectParentID);
			}
			else if (this.m_Parent != null)
			{
				xmlWriter.WriteAttributeString(T[_ATTRIB.parentID], this.m_Parent.ID);
			}
			else if (this.m_ParentID != null)
			{
				xmlWriter.WriteAttributeString(T[_ATTRIB.parentID], this.m_ParentID);
			}
			else
			{
				xmlWriter.WriteAttributeString(T[_ATTRIB.parentID], "-1");
			}

			xmlWriter.WriteAttributeString(T[_ATTRIB.restricted], GetBoolAsOneZero(this.IsRestricted).ToString());

			if (desiredProperties != null)
			{
				if (
					(desiredProperties.Count == 0) || 
					(desiredProperties.Contains(Tags.PropertyAttributes.container_childCount))
					)
				{
					xmlWriter.WriteAttributeString(T[_ATTRIB.childCount], this.ChildCount.ToString());
				}
			}
		}

		
		/// <summary>
		/// Closes the container element.
		/// </summary>
		/// <param name="xmlWriter"></param>
		protected void EndContainerXml(XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteEndElement();
		}

	
		/// <summary>
		/// Every container has an UpdateID, that indicates the current state of the container.
		/// Although the value is usually a monotomically increasing value, ContentDirectory
		/// does not guarantee it as such.
		/// <para>
		/// Derived classes will have access to this and so will internal classes.
		/// Programmers are strongly encouraged to not mess with this value unless
		/// they ABSOLUTELY know what they're doing.
		/// </para>
		/// </summary>
		[NonSerialized()] protected internal UInt32 m_UpdateID = 0;

		/// <summary>
		/// Enumerates through m_bools.
		/// </summary>
		protected enum EnumBoolsMediaContainer
		{
			Searchable = MediaObject.EnumBools.IgnoreLast,

			IgnoreLast
		}

		/// <summary>
		/// Indicates whether Search operations can be executed on this container from
		/// a control point.
		/// </summary>
		protected internal bool m_Searchable
		{
			get
			{
				return this.m_bools[(int)EnumBoolsMediaContainer.Searchable];
			}
			set
			{
				this.m_bools[(int)EnumBoolsMediaContainer.Searchable] = value;
			}
		}

		public virtual void TrimToSize()
		{
			base.TrimToSize();

			try
			{
				this.m_LockListing.AcquireWriterLock(-1);
				if (this.m_Listing != null)
				{
					this.m_Listing.TrimToSize();
				}
			}
			finally
			{
				this.m_LockListing.ReleaseWriterLock();
			}
		}
		
		/// <summary>
		/// Locks the m_Listing field for reading/writing operations.
		/// <para>
		/// Derived classes will have access to this and so will internal classes.
		/// Programmers deriving from this object are strongly encouraged to not use 
		/// this unless they ABSOLUTELY know what they're doing.
		/// </para>
		/// </summary>
		[NonSerialized()] protected internal System.Threading.ReaderWriterLock m_LockListing = null;

		/// <summary>
		/// ArrayList of "IUPnPMedia" objects that represent
		/// all the children of this container. The list is kept sorted
		/// through <see cref="HashingMethod"/>.
		/// <para>
		/// Derived classes will have access to this and so will internal classes.
		/// Programmers deriving from this object are strongly encouraged to not use 
		/// this unless they ABSOLUTELY know what they're doing.
		/// </para>
		/// </summary>
		[NonSerialized()] protected internal ArrayList m_Listing = null;

		/// <summary>
		/// Allows the programmer to customize how the child list for this
		/// container is managed internally. Usually, this value will be
		/// <see cref="MediaContainer.IdSorter"/> or <see cref="MediaContainer.HashWithIdNoSorting"/>
		/// because those two always hash by the child object's ID.
		/// <para>
		/// Generally, if the programmer needs to preserve order of added child objects,
		/// the programmer should always use <see cref="MediaContainer.HashWithIdNoSorting"/>.
		/// </para>
		/// </summary>
		[NonSerialized()] protected _SortedList HashingMethod = HashWithIdNoSorting;

		/// <summary>
		/// This object can be used to keep m_Listing organized without any sorting.
		/// This is particularly useful in control-point scenarios, where the order
		/// of the objects is significant and tampering with the order is not desired.
		/// </summary>
		public static _SortedList HashWithIdNoSorting = new _SortedList(new SortByID(false), false, true);

		/// <summary>
		/// This object can be used to keep m_Listing sorted by ID.
		/// This is particularly useful for fast lookups by ID, such as for device-side content hierarchies.
		/// </summary>
		public static _SortedList IdSorter = new _SortedList(new SortByID(false), false);
	}	
}
