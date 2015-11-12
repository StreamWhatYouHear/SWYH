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
using System.Xml;
using System.Text;
using OpenSource.UPnP;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using OpenSource.UPnP.AV;
using System.Collections;
using OpenSource.Utilities;
using System.Runtime.Serialization;

namespace OpenSource.UPnP.AV.CdsMetadata
{
    /// <summary>
    /// Provides a read-only interface into reading the metadata properties and values
    /// of a media object.
    /// </summary>
    public interface IMediaProperties
    {
        /// <summary>
        /// Provides a list of the available metadata properties
        /// that are defined for a media object.
        /// </summary>
        IList PropertyNames { get; }

        /// <summary>
        /// Provides the number of available of metadata properties.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Provides a means of writing the normative metadata
        /// associated with a media object in a DIDL-Lite compliant manner,
        /// excluding the top level "item" or "container" element.
        /// </summary>
        /// <param name="data">
        /// Argument must be a <see cref="ToXmlData"/> object.
        /// </param>
        /// <param name="xmlWriter">
        /// <see cref="XmlTextWriter"/> object that will print the 
        /// reprsentation in an XML compliant way.
        /// </param>
        /// <exception cref="InvalidCastException">
        /// Thrown if "data" argument is not a <see cref="ToXmlData"/> object.
        /// </exception>
        void ToXml(ToXmlData data, XmlTextWriter xmlWriter);

        /// <summary>
        /// Indexer for obtaining a listing of metadata values 
        /// for a given (CDS normative) metadataproperty.
        /// </summary>
        IList this[CommonPropertyNames prop] { get; set; }

        /// <summary>
        /// Indexer for setting/obtaining a listing of metadata values 
        /// for a given (CDS normative) metadata property name.
        /// </summary>
        IList this[string propName] { get; set; }

        /// <summary>
        /// Obtains multiple metadata values in a single operation.
        /// Much faster than using the indexers.
        /// </summary>
        Hashtable GetValues(string[] propertyNames);

        /// <summary>
        /// Compare against this value to determine if metadata has changed.
        /// </summary>
        int StateNumber { get; }
    }

    /// <summary>
    /// <para>
    /// Contains a list of provided properties
    /// for each UPnPMedia. Properties that 
    /// appear here may not be required properties
    /// for describing UPNP media. This class
    /// does not store any of the vendor-specific metadata.
    /// </para>
    /// 
    /// <para>
    /// In terms of actual XML, this struct encapsulates
    /// all of the the "upnp:" and "dc:" child-elements
    /// of an Item or Container element.
    /// </para>
    /// </summary>
    [Serializable()]
    public sealed class MediaProperties : IMediaProperties, ISerializable
    {
        /// <summary>
        /// Special ISerializable constructor.
        /// Do basic initialization and then serialize from the info object.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        private MediaProperties(SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            Init();
            this.m_Table = (ArrayList)info.GetValue("m_Table", typeof(ArrayList));
            this.m_Title = (PropertyString)info.GetValue("m_Title", typeof(PropertyString));
            this.m_Creator = (PropertyString)info.GetValue("m_Creator", typeof(PropertyString));
            this.m_Class = (MediaClass)info.GetValue("m_Class", typeof(MediaClass));
            this.IsEnabled_OnMetadataChanged = info.GetBoolean("IsEnabled_OnMetadataChanged");
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
        public void GetObjectData(SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue("m_Table", this.m_Table);
            info.AddValue("m_Title", this.m_Title);
            info.AddValue("m_Creator", this.m_Creator);
            info.AddValue("m_Class", this.m_Class);
            info.AddValue("IsEnabled_OnMetadataChanged", this.IsEnabled_OnMetadataChanged);
        }

        /// <summary>
        /// Initializes
        /// </summary>
        private void Init()
        {
            if (MediaObject.UseStaticLock)
            {
                this.m_LockTable = MediaObject.StaticLock;
            }
            else
            {
                this.m_LockTable = new ReaderWriterLock();
            }

            this.WeakEvent_OnMetadataChanged = new WeakEvent();
            this.m_StateNumber = 0;
            this.ClearProperties();
        }

        /// <summary>
        /// <para>
        /// This method clears the hashtable values for all properties.
        /// </para>
        /// <para>
        /// Derived classes will have access to this and so will internal classes.
        /// Programmers deriving from this object are strongly encouraged to not use 
        /// this unless they ABSOLUTELY know what they're doing.
        /// </para>
        /// </summary>
        internal void ClearProperties()
        {
            try
            {
                this.m_LockTable.AcquireWriterLock(-1);
                this.m_Table = new ArrayList(2);
                this.m_Title = null;
                this.m_Creator = null;
                this.m_Class = null;
                this.IncrementStateNumber();
            }
            finally
            {
                this.m_LockTable.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Although this class doesn't implement
        /// <see cref="ICdsElement"/>
        /// the intent of this method is exactly the same as
        /// <see cref="ICdsElement.ToXml"/>.
        /// </summary>
        /// <param name="data">
        /// Argument must be a <see cref="ToXmlData"/> object.
        /// </param>
        /// <param name="xmlWriter">
        /// <see cref="XmlTextWriter"/> object that will print the 
        /// reprsentation in an XML compliant way.
        /// </param>
        /// <exception cref="InvalidCastException">
        /// Thrown if "data" argument is not a <see cref="ToXmlData"/> object.
        /// </exception>
        public void ToXml(ToXmlData data, XmlTextWriter xmlWriter)
        {
            ToXmlData _d = data;

            IList list; ICdsElement ele;

            string tag_title = T[_DC.title];
            string tag_class = T[_UPNP.Class];

            list = this[tag_title];
            ele = (ICdsElement)list[0];
            xmlWriter.WriteElementString(tag_title, ele.StringValue);

            list = this[tag_class];
            ele = (ICdsElement)list[0];
            xmlWriter.WriteElementString(tag_class, ele.StringValue);

            if (_d.DesiredProperties != null)
            {
                ICollection properties = this.PropertyNames;
                foreach (string propName in properties)
                {
                    if (
                        (_d.DesiredProperties.Count == 0) ||
                        (_d.DesiredProperties.Contains(propName))
                        )
                    {
                        if ((propName != tag_title) && (propName != tag_class))
                        {
                            list = this[propName];

                            if (list != null)
                            {
                                if (list.Count > 0)
                                {
                                    foreach (ICdsElement val in list)
                                    {
                                        val.ToXml(ToXmlFormatter.DefaultFormatter, _d, xmlWriter);
                                        //val.ToXml(false, desiredProperties, xmlWriter);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns a shallow copy of names of provided properties. This
        /// can be used to enumerate what metadata has been
        /// set for a particular media item.
        /// </summary>
        public IList PropertyNames
        {
            get
            {
                ArrayList al = new ArrayList(3);

                if (this.m_Class != null) { al.Add(T[_UPNP.Class]); }
                if (this.m_Creator != null) { al.Add(T[_DC.creator]); }
                if (this.m_Title != null) { al.Add(T[_DC.title]); }

                this.m_LockTable.AcquireReaderLock(-1);
                al.Capacity += this.m_Table.Count;
                ICollection keys = _Hashtable.Keys(m_Table);

                foreach (string p in keys)
                {
                    al.Add(p);
                }
                this.m_LockTable.ReleaseReaderLock();

                return al;
            }
        }

        /// <summary>
        /// Returns number of property names that are available.
        /// </summary>
        public int Count
        {
            get
            {
                int cnt = 0;

                if (this.m_Class != null) { cnt++; }
                if (this.m_Creator != null) { cnt++; }
                if (this.m_Title != null) { cnt++; }


                this.m_LockTable.AcquireReaderLock(-1);
                cnt += this.m_Table.Count;
                this.m_LockTable.ReleaseReaderLock();
                return cnt;
            }
        }

        /// <summary>
        /// <para>
        /// Each property always returns a listing of values
        /// because the schema allows for multiple values.
        /// For example, there may be more than one 
        /// artist, genre, codec, etc. associated with the item.
        /// </para>
        /// 
        /// <para>
        /// The list is a shallow copy.
        /// </para>
        /// 
        /// <para>
        /// Every element in this class implements 
        /// <see cref="ICdsElement"/>.
        /// The interface helps strongly type the classes
        /// when instantiating from xml.
        /// </para>
        /// <para>
        /// If using a <see cref="Tags"/>
        /// object, then do not use the 
        /// <see cref="_DIDL"/>
        /// <see cref="_ATTRIB"/>
        /// enumerators in the Tags indexer. Only the
        /// <see cref="CommonPropertyNames"/>, 
        /// <see cref="_DC"/>, or
        /// <see cref="_UPNP"/>
        /// enumerators will work with this class.
        /// </para>
        /// <para>
        /// The set operation for this indexer will throw an
        /// <see cref="ApplicationException"/> if the
        /// caller does not have the same namespace
        /// and assembly as this class.
        /// </para>
        /// </summary>
        /// <exception cref="ApplicationException">
        /// Thrown on a set-operation if the caller does
        /// is not defined in the same assembly and
        /// namespace as this class.
        /// </exception>
        public IList this[string propName]
        {
            get
            {
                Exception error = null;
                IList retVal = null;
                ICdsElement[] elements;

                elements = new ICdsElement[1];
                if ((propName == T[_DC.title]) && (this.m_Title != null))
                {
                    elements[0] = this.m_Title;
                }
                else if ((propName == T[_DC.creator]) && (this.m_Creator != null))
                {
                    elements[0] = this.m_Creator;
                }
                else if ((propName == T[_UPNP.Class]) && (this.m_Class != null))
                {
                    elements[0] = this.m_Class;
                }
                else
                {
                    // in the worst case, ensure empty list
                    elements = new ICdsElement[0];

                    this.m_LockTable.AcquireReaderLock(-1);

                    try
                    {
                        retVal = (IList)_Hashtable.Get(m_Table, propName);

                        if (error == null)
                        {
                            // return a shallow copy
                            if (retVal != null)
                            {
                                if (retVal.Count > 0)
                                {
                                    elements = new ICdsElement[retVal.Count];
                                    int i = 0;
                                    foreach (ICdsElement e in retVal)
                                    {
                                        elements[i] = e;
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        error = e;
                    }

                    this.m_LockTable.ReleaseReaderLock();

                    if (error != null)
                    {
                        Exception ne = new Exception("MediaProperties.get_this[string]()", error);
                        throw ne;
                    }
                }

                return elements;
            }
            set
            {
                this.CheckProtection();

                int count = value.Count;
                ICdsElement[] elements = new ICdsElement[count];
                for (int i = 0; i < count; i++)
                {
                    elements[i] = (ICdsElement)value[i];
                }
                this.SetVal(propName, elements);
            }
        }

        /// <summary>
        /// Obtains multiple metadata values in a single operation.
        /// Much faster than using the indexers.
        /// </summary>
        public Hashtable GetValues(string[] propertyNames)
        {
            ArrayList properties = new ArrayList(propertyNames.Length);
            Hashtable results = new Hashtable(propertyNames.Length);

            string title = T[_DC.title];
            string creator = T[_DC.creator];
            string Class = T[_UPNP.Class];

            foreach (string pn in propertyNames)
            {
                if (pn == title)
                {
                    ICdsElement[] elements = new ICdsElement[1];
                    elements[0] = this.m_Title;
                    results.Add(title, elements);
                }
                else if (pn == creator)
                {
                    ICdsElement[] elements = new ICdsElement[1];
                    elements[0] = this.m_Creator;
                    results.Add(creator, elements);
                }
                else if (pn == Class)
                {
                    ICdsElement[] elements = new ICdsElement[1];
                    elements[0] = this.m_Class;
                    results.Add(Class, elements);
                }
                else
                {
                    properties.Add(pn);
                }
            }

            this.m_LockTable.AcquireReaderLock(-1);
            _Hashtable.Get(results, this.m_Table, (object[])properties.ToArray(typeof(object)));
            this.m_LockTable.ReleaseReaderLock();

            return results;
        }


        /// <summary>
        /// Allows retrieving common properties through 
        /// CommonPropertyNames enumerator.
        /// <para>
        /// The set operation for this indexer will throw an
        /// <see cref="ApplicationException"/> if the
        /// caller does not have the same namespace
        /// and assembly as this class.
        /// </para>
        /// <para>
        /// This property is effectively the same as the
        /// other indexer that takes a string.
        /// </para>
        /// </summary>
        /// <exception cref="ApplicationException">
        /// Thrown on a set-operation if the caller does
        /// is not defined in the same assembly and
        /// namespace as this class.
        /// </exception>
        public IList this[CommonPropertyNames prop]
        {
            get
            {
                IList retVal = null;

                int i = (int)prop;
                if (prop < CommonPropertyNames.DoNotUse)
                {
                    retVal = (IList)this[T[(_UPNP)i]];
                }
                else if (prop > CommonPropertyNames.DoNotUse)
                {
                    retVal = (IList)this[T[(_DC)i]];
                }

                return retVal;
            }
            set
            {
                int i = (int)prop;
                if (prop < CommonPropertyNames.DoNotUse)
                {
                    this[T[(_UPNP)i]] = value;
                }
                else if (prop > CommonPropertyNames.DoNotUse)
                {
                    this[T[(_DC)i]] = value;
                }
            }
        }

        /// <summary>
        /// Checks the caller of the set operation to ensure that
        /// it has proper run-time binding rights to this
        /// property. The method requires that the caller of set
        /// operation by defined in the same namespace & assembly
        /// of this class. This effectively enforces an
        /// read-only behavior for public-programmers.
        /// </summary>
        private void CheckProtection()
        {
            StackTrace st = new StackTrace();

            StackFrame sf = st.GetFrame(1);

            MethodBase mb = sf.GetMethod();

            Type mt = mb.DeclaringType;
            Type thisType = this.GetType();
            bool ok = false;
            if (mt.Namespace == (thisType.Namespace))
            {
                if (mt.Assembly == thisType.Assembly)
                {
                    ok = true;
                }
            }

            if (!ok)
            {
                throw new ApplicationException("You cannot call this method from your current assembly/module.");
            }
        }


        ~MediaProperties()
        {
            OpenSource.Utilities.InstanceTracker.Remove(this);
        }

        /// <summary>
        /// Builds an empty MediaProperties object.
        /// </summary>
        public MediaProperties()
        {
            OpenSource.Utilities.InstanceTracker.Add(this);
            Init();
        }

        /// <summary>
        /// Sets a value in the hashtable
        /// </summary>
        /// <param name="key">property name</param>
        private void SetVal(string key, ICdsElement[] objs)
        {
            //this.m_Table[key] = objs;
            if ((key == T[_DC.title]))
            {
                this.m_Title = (PropertyString)objs[0];
            }
            else if ((key == T[_DC.creator]))
            {
                this.m_Creator = (PropertyString)objs[0];
            }
            else if ((key == T[_UPNP.Class]))
            {
                this.m_Class = (MediaClass)objs[0];
            }
            else
            {
                Exception error = null;
                try
                {
                    this.m_LockTable.AcquireWriterLock(-1);
                    try
                    {
                        _Hashtable.Set(m_Table, key, objs);
                    }
                    catch (Exception e)
                    {
                        error = e;
                    }
                }
                finally
                {
                    this.m_LockTable.ReleaseWriterLock();
                }

                if (error != null)
                {
                    Exception ne = new Exception("MediaProperties.set_this[string]()", error);
                    throw ne;
                }
            }
            this.IncrementStateNumber();
        }

        /// <summary>
        /// Sets a value in the hashtable. If the
        /// object does not implement ICollection, then it recasts
        /// things so it works.
        /// </summary>
        /// <param name="key">property name</param>
        /// <param name="obj">an object to add, automatically recasted to IList if needed</param>
        internal void AddVal(string key, object obj)
        {
            IList list = this[key];

            ArrayList al;

            if (list != null)
            {
                if (list.GetType() == typeof(ArrayList))
                {
                    al = (ArrayList)list;
                }
                else
                {
                    al = new ArrayList();
                    al.AddRange(list);
                }
            }
            else
            {
                al = new ArrayList();
            }

            if (obj is System.Collections.ICollection)
            {
                al.AddRange((ICollection)obj);
            }
            else
            {
                al.Add(obj);
            }

            this.SetVal(key, (ICdsElement[])al.ToArray(typeof(ICdsElement)));
        }

        internal void RemoveVal(string key, object obj)
        {
            IList list = this[key];
            ArrayList al = list as ArrayList;
            if (al != null)
            {
                al.Remove(obj);
            }
            else
            {
                al = new ArrayList(list.Count);
                foreach (object thingie in list)
                {
                    if (thingie.Equals(obj) == false)
                    {
                        al.Add(obj);
                    }
                }
            }

            if (al.Count > 0)
            {
                this.SetVal(key, (ICdsElement[])al.ToArray(typeof(ICdsElement)));
            }
            else
            {
                this.SetVal(key, null);
            }
        }

        /// <summary>
        /// Locks the hashtable.
        /// </summary>
        [NonSerialized()]
        private ReaderWriterLock m_LockTable = null;

        internal ReaderWriterLock RWLock { get { return this.m_LockTable; } }

        /// <summary>
        /// Hashtable of property names (string) to IList of property values .
        /// </summary>
        private ArrayList m_Table = null;

        /// <summary>
        /// dc:title is not stored in m_Table for performance reasons
        /// </summary>
        private PropertyString m_Title = null;

        /// <summary>
        /// dc:creator is not stored in m_Table for performance reasons
        /// </summary>
        private PropertyString m_Creator = null;

        /// <summary>
        /// upnp:class is not stored in m_Table for performance reasons
        /// </summary>
        private MediaClass m_Class = null;

        /// <summary>
        /// Value changes whenever metadata changes.
        /// </summary>
        private int m_StateNumber = 0;

        /// <summary>
        /// Compare against this value to determine if metadata has changed.
        /// </summary>
        public int StateNumber { get { return m_StateNumber; } }

        /// <summary>
        /// Atomically increments m_StateNumber.
        /// </summary>
        private void IncrementStateNumber()
        {
            // Interlocked.Increment handles the overflow for me.
            System.Threading.Interlocked.Increment(ref m_StateNumber);

            // Fire an event indicating things have changed.
            if (this.IsEnabled_OnMetadataChanged)
            {
                this.WeakEvent_OnMetadataChanged.Fire(this, this.m_StateNumber);
            }
        }

        /// <summary>
        /// If this value is true, then <see cref="OnMetadataChanged"/>
        /// will fire whenever metadata gets changed. If false,
        /// then the event will never not fire, even if metadata
        /// does get changed.
        /// </summary>
        public bool IsEnabled_OnMetadataChanged = false;

        /// <summary>
        /// Whenever <see cref="OnMetadataChanged"/> is fired, we
        /// use a weak event to handle stuff.
        /// </summary>
        [NonSerialized()]
        protected WeakEvent WeakEvent_OnMetadataChanged = null;

        /// <summary>
        /// Delegate is used for <see cref="OnMetadataChanged"/> event.
        /// </summary>
        public delegate void Delegate_OnMetadataChanged(MediaProperties sender, int stateNumber);

        /// <summary>
        /// This event fires when the metadata changes.
        /// </summary>
        public event Delegate_OnMetadataChanged OnMetadataChanged
        {
            add
            {
                this.WeakEvent_OnMetadataChanged.Register(value);
            }
            remove
            {
                this.WeakEvent_OnMetadataChanged.UnRegister(value);
            }
        }

        /// <summary>
        /// Must acquire lock before trimming.
        /// </summary>
        public void TrimTableToSize()
        {
            this.m_Table.TrimToSize();
        }

        /// <summary>
        /// Gets a static instance that allows easy translation
        /// of CommonPropertyNames enumeration into ContentDirectory element
        /// names and attributes.
        /// </summary>
        private static Tags T = Tags.GetInstance();
    }
}
