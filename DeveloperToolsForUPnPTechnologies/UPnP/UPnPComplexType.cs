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
using System.Collections;

namespace OpenSource.UPnP
{
	/// <summary>
	/// Summary description for UPnPComplexType.
	/// </summary>
	public class UPnPComplexType
	{
		#region Field
		public struct Field
		{
			public string Name;
			public string Type;
			public string TypeNS;
			public string MinOccurs;
			public string MaxOccurs;
			public ArrayList AttributeList;
		}
		#endregion

		#region Restrictions and Extensions
		public abstract class RestrictionExtension
		{
			public string baseType;
			public string baseTypeNS;
		}
		public class Restriction : RestrictionExtension
		{
			public string PATTERN;
		}
		public class Extension : RestrictionExtension
		{
		}
		#endregion

		#region Content Types

		public abstract class ContentData
		{
			public string Name;
			public string Type;
			public string TypeNS;
			public string MinOccurs = "1";
			public string MaxOccurs = "1";
			public ItemCollection Parent;

			public override string ToString()
			{
				return(this.GetType().Name + ": " + Name);
			}

		}

		public class Attribute : ContentData
		{
		}
		public class Element : ContentData
		{
			public ArrayList AttributeList;
			public Attribute[] Attributes
			{
				get
				{
					return((Attribute[])AttributeList.ToArray(typeof(Attribute)));
				}
			}
		}
		public class ItemCollection
		{
			public string MinOccurs="1";
			public string MaxOccurs="1";

			public ArrayList ItemList = new ArrayList();
			public ArrayList NestedCollectionList = new ArrayList();
			public GenericContainer ParentContainer;
			public ItemCollection ParentCollection;
			

			public ItemCollection[] NestedCollections
			{
				get
				{
					return((ItemCollection[])NestedCollectionList.ToArray(typeof(ItemCollection)));
				}
			}
			public void AddCollection(ItemCollection ic)
			{
				NestedCollectionList.Add(ic);
				ic.ParentCollection = this;
			}
			public void RemoveCollection(ItemCollection ic)
			{
				NestedCollectionList.Remove(ic);
				ic.ParentCollection = null;
			}

			public ContentData[] Items
			{
				get
				{
					return((ContentData[])ItemList.ToArray(typeof(ContentData)));
				}
			}
			public ContentData CurrentItem
			{
				get
				{
					if (ItemList.Count==0)
					{
						return(null);
					}
					else
					{
						return((ContentData)ItemList[ItemList.Count-1]);
					}
				}
			}
			public void AddContentItem(ContentData c)
			{
				c.Parent = this;
				ItemList.Add(c);
			}
			public void RemoveContentItem(ContentData c)
			{
				c.Parent = null;
				ItemList.Remove(c);
			}
			public void MoveContentItem_UP(ContentData c)
			{
				int i = ItemList.IndexOf(c);
				if (i>=1)
				{
					ItemList.Reverse(i-1,2);
				}
			}
			public void MoveContentItem_DOWN(ContentData c)
			{
				int i = ItemList.IndexOf(c);
				if (i>=0 && i+1<=ItemList.Count)
				{
					ItemList.Reverse(i,2);
				}
			}
		}
		public class Choice : ItemCollection
		{
		}
		public class Sequence : ItemCollection
		{
		}

		public class GenericContainer
		{
			public UPnPComplexType ParentComplexType;
			public string documentation = "";
			public ArrayList CollectionList = new ArrayList();

			public GenericContainer()
			{
			}
			public ItemCollection[] Collections
			{
				get
				{
					return((ItemCollection[])CollectionList.ToArray(typeof(ItemCollection)));
				}
			}
			public void AddCollection(ItemCollection e)
			{
				CollectionList.Add(e);
				e.ParentContainer = this;
			}
			public void RemoveCollection(ItemCollection e)
			{
				CollectionList.Remove(e);
				e.ParentContainer = null;
			}
		}
		public class Group : UPnPComplexType
		{
		}
		public class ComplexContent : GenericContainer
		{
			public RestrictionExtension RestExt;
		}
		public class SimpleContent : GenericContainer
		{
			public RestrictionExtension RestExt;
		}
		#endregion


		public Field[] GetFields()
		{
			return(null);
		}

		private string NameSpace;
		private string LocalName;

		private ArrayList m_CollectionList;

		internal UPnPService ParentService=null;
		
		public void AddContainer(GenericContainer c)
		{
			m_CollectionList.Add(c);
			c.ParentComplexType = this;
		}
		public void RemoveContainer(GenericContainer c)
		{
			m_CollectionList.Remove(c);
			c.ParentComplexType = null;
		}
		public GenericContainer[] Containers
		{
			get
			{
				return((GenericContainer[])m_CollectionList.ToArray(typeof(GenericContainer)));
			}
		}
		public GenericContainer CurrentContainer
		{
			get
			{
				if (m_CollectionList.Count==0)
				{
					return(null);
				}
				else
				{
					return((GenericContainer)m_CollectionList[m_CollectionList.Count-1]);
				}
			}
		}


		public UPnPComplexType()
		{
			m_CollectionList = new ArrayList();
		}
		public void ClearCollections()
		{
			m_CollectionList.Clear();
		}
		public UPnPComplexType(string Name, string Namespace):this()
		{
			NameSpace = Namespace;
			LocalName = Name;
		}
		public override string ToString()
		{
			return(this.Name_LOCAL);
		}

		public string Name_LOCAL
		{
			get
			{
				return(LocalName);
			}
		}
		public string Name_NAMESPACE
		{
			get
			{
				return(NameSpace);
			}
		}


		private void GetComplexTypeSchemaPart3(XmlTextWriter X, ItemCollection ic)
		{
			if (ic.GetType().Name!="ItemCollection")
			{
				X.WriteStartElement("xs",ic.GetType().Name.ToLower(),null);
			}
			foreach(ContentData cd in ic.Items)
			{
				X.WriteStartElement("xs",cd.GetType().Name.ToLower(),null);
				X.WriteAttributeString("name",cd.Name);
				if (cd.TypeNS=="http://www.w3.org/2001/XMLSchema")
				{
					X.WriteAttributeString("type","xs:"+cd.Type);
				}
				else
				{
					X.WriteAttributeString("type",cd.Type);
				}

				if (cd.MinOccurs!="")
				{
					X.WriteAttributeString("minOccurs",cd.MinOccurs);
				}
				if (cd.MaxOccurs!="")
				{
					X.WriteAttributeString("maxOccurs",cd.MaxOccurs);
				}
				X.WriteEndElement();	
			}
			if (ic.GetType().Name!="ItemCollection")
			{
				X.WriteEndElement();
			}
		}

		private void GetComplexTypeSchemaPart2(XmlTextWriter X, GenericContainer gc)
		{
			foreach(ItemCollection ic in gc.Collections)
			{
				GetComplexTypeSchemaPart3(X,ic);
			}
		}
		
		public void GetComplexTypeSchemaPart(XmlTextWriter X)
		{
			X.WriteStartElement("xs","complexType",null);
			X.WriteAttributeString("name",this.Name_LOCAL);
			

			foreach(GenericContainer gc in this.Containers)
			{
				GetComplexTypeSchemaPart2(X,gc);
			}
			
			X.WriteEndElement();
		}


		public static ItemCollection ParseCollection(XmlTextReader X)
		{
			return(null);
		}

		public static UPnPComplexType[] Parse(string xml)
		{
			// Parse the XSD Schema
			ArrayList a = new ArrayList(); // ComplexTypes
			Hashtable g = new Hashtable(); // Table of Groups
			StringReader s = new StringReader(xml);
			XmlTextReader X = new XmlTextReader(s);
	
			while(X.Read())
			{
				switch(X.NodeType)
				{
					case XmlNodeType.Element:
						switch(X.LocalName)
						{
							case "complexType":
								UPnPComplexType _complextype = ParseComplexType(X);
								a.Add(_complextype);
								g[_complextype.Name_NAMESPACE+":"+_complextype.Name_LOCAL] = _complextype;
								break;
							case "group":
								Group _group = (Group)ParseComplexType(X,new Group());
								g[_group.Name_NAMESPACE + ":" + _group.Name_LOCAL] = _group;
								a.Add(_group);
								break;
						}
						break;
					case XmlNodeType.EndElement:
						break;
					case XmlNodeType.Text:
						break;
				}
			}
			return((UPnPComplexType[])a.ToArray(typeof(UPnPComplexType)));
		}
		private static ItemCollection ParseComplexType_SequenceChoice(XmlTextReader X)
		{
			bool done = false;
			ItemCollection RetVal=null;
			string elementName = X.LocalName;
			DText p = new DText();
			p.ATTRMARK = ":";

			if (X.LocalName=="choice")
			{
				RetVal = new Choice();
			}
			else
			{
				RetVal = new Sequence();
			}
							
			if (X.HasAttributes)
			{
				for(int i=0;i<X.AttributeCount;i++)
				{
					X.MoveToAttribute(i);
					switch(X.LocalName)
					{
						case "minOccurs":
							RetVal.MinOccurs = X.Value;
							break;
						case "maxOccurs":
							RetVal.MaxOccurs = X.Value;
							break;
					}
				}
				X.MoveToElement();
			}
			X.Read();					


			do
			{
				switch(X.NodeType)
				{
					case XmlNodeType.Element:
						switch(X.LocalName)
						{
							case "group":
								if (X.HasAttributes)
								{
									for(int i=0;i<X.AttributeCount;i++)
									{
										X.MoveToAttribute(i);
										switch(X.LocalName)
										{
											case "ref":
												string sample = X.Value;
												break;
										}
									}
									X.MoveToElement();
								}
								break;
							case "sequence":
							case "choice":
								RetVal.AddCollection(ParseComplexType_SequenceChoice(X));
								break;
							case "element":
								RetVal.AddContentItem(new Element());
								if (X.HasAttributes)
								{
									for(int i=0;i<X.AttributeCount;i++)
									{
										X.MoveToAttribute(i);
										switch(X.LocalName)
										{
											case "name":
												RetVal.CurrentItem.Name = X.Value;
												break;
											case "type":
												p[0] = X.Value;
												if (p.DCOUNT()==1)
												{
													RetVal.CurrentItem.Type = X.Value;
													RetVal.CurrentItem.TypeNS = X.LookupNamespace("");
												}
												else
												{
													RetVal.CurrentItem.Type = p[2];
													RetVal.CurrentItem.TypeNS = X.LookupNamespace(p[1]);
												}
												break;
											case "minOccurs":
												RetVal.CurrentItem.MinOccurs = X.Value;
												break;
											case "maxOccurs":
												RetVal.CurrentItem.MaxOccurs = X.Value;
												break;
										}
									}
									X.MoveToElement();
								}
								break;
							case "attribute":
								break;
						}
						break;
					case XmlNodeType.EndElement:
						if (X.LocalName==elementName)
						{
							done=true;
						}
						break;
					case XmlNodeType.Text:
						break;
				}
			}while(!done && X.Read());

			return(RetVal);
		}

		private static UPnPComplexType ParseComplexType(XmlTextReader X)
		{
			return(ParseComplexType(X,new UPnPComplexType()));
		}
		private static UPnPComplexType ParseComplexType(XmlTextReader X, UPnPComplexType RetVal)
		{
			string elementName = X.LocalName;
			int count=0;
			bool done = false;
			DText P = new DText();
			P.ATTRMARK = ":";

			RetVal.AddContainer(new GenericContainer());

			do
			{
				switch(X.NodeType)
				{
					case XmlNodeType.Element:
						switch(X.LocalName)
						{
							case "complexType":
							case "group":
								++count;
								if (X.HasAttributes)
								{
									for(int i=0;i<X.AttributeCount;i++)
									{
										X.MoveToAttribute(i);
										if (X.Name=="name")
										{
											P[0] = X.Value;
											if (P.DCOUNT()==1)
											{
												RetVal.LocalName = X.Value;
												RetVal.NameSpace = X.LookupNamespace("");
											}
											else
											{
												RetVal.LocalName = P[2];
												RetVal.NameSpace = X.LookupNamespace(P[1]);
											}
										}
										else if (X.Name=="ref")
										{
											// NOP
										}
									}
									X.MoveToElement();
								}
								break;
							case "sequence":
							case "choice":
								RetVal.CurrentContainer.AddCollection(ParseComplexType_SequenceChoice(X));
								//ParseComplexType_Sequence(X,RetVal);
								break;
							case "complexContent":
								RetVal.AddContainer(new ComplexContent());
								break;
							case "simpleContent":
								RetVal.AddContainer(new SimpleContent());
								break;
							case "restriction":
								Restriction r = new Restriction();
								if (RetVal.CurrentContainer.GetType()==typeof(ComplexContent))
								{
									((ComplexContent)RetVal.CurrentContainer).RestExt = r;
								}
								else if (RetVal.CurrentContainer.GetType()==typeof(SimpleContent))
								{
									((SimpleContent)RetVal.CurrentContainer).RestExt = r;
								}
								if (X.HasAttributes)
								{
									for(int i=0;i<X.AttributeCount;i++)
									{
										X.MoveToAttribute(i);
										if (X.Name=="base")
										{
											P[0] = X.Value;
											if (P.DCOUNT()==1)
											{
												r.baseType = X.Value;
												r.baseTypeNS = X.LookupNamespace("");
											}
											else
											{
												r.baseType = P[2];
												r.baseTypeNS = X.LookupNamespace(P[1]);
											}
										}
									}
									X.MoveToElement();
								}
								break;
						}
						break;
					case XmlNodeType.EndElement:
						if (X.LocalName==elementName)
						{
							--count;
							if (count==0)
							{
								done=true;
							}
						}
						break;
					case XmlNodeType.Text:
						break;
				}
			}while(!done && X.Read());
			return(RetVal);
		}
	}
}
