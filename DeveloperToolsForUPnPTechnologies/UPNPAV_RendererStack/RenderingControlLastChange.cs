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
using System.Collections;
using OpenSource.UPnP.AV;

namespace OpenSource.UPnP.AV.RENDERER.CP
{
	/// <summary>
	/// Summary description for RenderingControlLastChange.
	/// </summary>
	public class RenderingControlLastChange
	{
		private int StateCounter = 0;
		public delegate void OnReadyHandler(RenderingControlLastChange sender);
		public event OnReadyHandler OnReady;

		public string Identifier = null;
		public delegate void VariableChangeHandler(RenderingControlLastChange sender);
		public event VariableChangeHandler OnVolumeChanged;
		public event VariableChangeHandler OnMuteChanged;

		protected Hashtable VolumeByChannel = new Hashtable();
		protected Hashtable MuteByChannel = new Hashtable();

		protected CpRenderingControl _cp;
		protected UInt16 _Volume = 0;
		protected int InstanceID;


		public void Dispose()
		{
			this.OnMuteChanged = null;
			this.OnReady = null;
			this.OnVolumeChanged = null;
		}
		public bool Mute
		{
			get
			{
				return(GetMute("Master"));
			}
		}/*
		public UInt16 Volume
		{
			get
			{
				return(_Volume);
			}
		}*/

		public bool GetMute(string channel)
		{
			if(MuteByChannel.ContainsKey(channel)==false) return(false);
			return((bool)MuteByChannel[channel]);
		}

		public UInt16 GetVolume(string channel)
		{
			if(VolumeByChannel.ContainsKey(channel)==false) return((UInt16)0);
			return((UInt16)VolumeByChannel[channel]);
		}


		public RenderingControlLastChange(CpRenderingControl cpRC, string Ident, int ID, RenderingControlLastChange.OnReadyHandler ReadyCallback)
		{
			OpenSource.Utilities.InstanceTracker.Add(this);
			this.OnReady += ReadyCallback;
			InstanceID = ID;
			Identifier = Ident;
			_cp = cpRC;
			_cp.OnStateVariable_LastChange += new CpRenderingControl.StateVariableModifiedHandler_LastChange(LastChangeSink);
			_cp._subscribe(500);

			lock(this)
			{
				if(_cp.HasAction_GetVolume)
				{
					CpRenderingControl.Delegate_OnResult_GetVolume TD =  new CpRenderingControl.Delegate_OnResult_GetVolume(VolumeSink);
				
					StateCounter += _cp.Values_A_ARG_TYPE_Channel.Length;

					foreach(string voltype in _cp.Values_A_ARG_TYPE_Channel)
					{
						_cp.SetUnspecifiedValue("Enum_A_ARG_TYPE_Channel",voltype);
						_cp.GetVolume((UInt32)ID,CpRenderingControl.Enum_A_ARG_TYPE_Channel._UNSPECIFIED_,null,TD);
					}
				}
				if(_cp.HasAction_GetMute)
				{
					CpRenderingControl.Delegate_OnResult_GetMute RGM = new CpRenderingControl.Delegate_OnResult_GetMute(MuteSink);
					StateCounter += _cp.Values_A_ARG_TYPE_Channel.Length;

					foreach(string voltype in _cp.Values_A_ARG_TYPE_Channel)
					{
						_cp.SetUnspecifiedValue("Enum_A_ARG_TYPE_Channel",voltype);
						_cp.GetMute((UInt32)ID,CpRenderingControl.Enum_A_ARG_TYPE_Channel._UNSPECIFIED_,null,RGM);
					}
				}

				if(StateCounter==0)
				{
					if(OnReady!=null) OnReady(this);
				}
			}
		}
		private void MuteSink(CpRenderingControl sender, System.UInt32 InstanceID, CpRenderingControl.Enum_A_ARG_TYPE_Channel Channel, System.Boolean CurrentMute, UPnPInvokeException e, object _Tag)
		{
			if(e==null)
			{	
				MuteByChannel[_cp.Enum_A_ARG_TYPE_Channel_ToString(Channel)] = CurrentMute;
			}
			lock(this)
			{
				--StateCounter;
				if(StateCounter==0)
				{
					if(OnReady!=null)OnReady(this);
				}
			}
		}

		private void VolumeSink(CpRenderingControl sender, UInt32 id, CpRenderingControl.Enum_A_ARG_TYPE_Channel Channel, UInt16 VolumeLevel, UPnPInvokeException e, object Handle)
		{
			if(e==null)
			{	
				VolumeByChannel[_cp.Enum_A_ARG_TYPE_Channel_ToString(Channel)] = VolumeLevel;
			}
			lock(this)
			{
				--StateCounter;
				if(StateCounter==0)
				{
					if(OnReady!=null)OnReady(this);
				}
			}
		}
		protected void LastChangeSink(CpRenderingControl sender, string LC)
		{
			if(LC=="")return;
			if(LC.Substring(0,1)!="<") LC = UPnPStringFormatter.UnEscapeString(LC);
			if(LC.Substring(0,1)!="<") LC = UPnPStringFormatter.UnEscapeString(LC);


			/*
			if(LC.StartsWith("<?")==true)
			{
				int dx = LC.IndexOf(">");
				LC = LC.Substring(0,dx+1) + "<MYROOT>" + LC.Substring(dx+1) + "</MYROOT>";
			}
			else
			{
				LC = "<MYROOT>" + LC + "</MYROOT>";
			}*/

			StringReader MyString = new StringReader(LC);
			XmlTextReader XMLDoc = new XmlTextReader(MyString);

			Hashtable T = new Hashtable();

			int ID = -1;
			string VarName = "";
			string VarValue = "";
			string AttrName = "";
			string AttrValue = "";

			XMLDoc.Read();
			XMLDoc.MoveToContent();

			XMLDoc.Read();
			XMLDoc.MoveToContent();

			while((XMLDoc.LocalName!="Event")&&(XMLDoc.EOF==false))
			{
				// At Start, should be InstanceID
				if(XMLDoc.LocalName=="InstanceID")
				{
					XMLDoc.MoveToAttribute("val");
					ID = int.Parse(XMLDoc.GetAttribute("val"));
					if(T.ContainsKey(ID)==false) T[ID] = new Hashtable();
					XMLDoc.MoveToContent();

					XMLDoc.Read();
					XMLDoc.MoveToContent();

					while(XMLDoc.LocalName!="InstanceID")
					{
						VarName = XMLDoc.LocalName;
						VarValue = "";
						AttrName = "";
						AttrValue = "";

						for(int a_idx=0;a_idx<XMLDoc.AttributeCount;++a_idx)
						{
							XMLDoc.MoveToAttribute(a_idx);
							if(XMLDoc.LocalName=="val")
							{
								VarValue = XMLDoc.GetAttribute(a_idx);
							}
							else
							{
								AttrName = XMLDoc.LocalName;
								AttrValue = XMLDoc.GetAttribute(a_idx);
							}
						}

						XMLDoc.MoveToContent();

						if(AttrName=="")
						{
							((Hashtable)T[ID])[VarName] = VarValue;
						}
						else
						{
							if(((Hashtable)T[ID]).ContainsKey(VarName)==false)
							{
								((Hashtable)T[ID])[VarName] = new Hashtable();
							}
							if(((Hashtable)((Hashtable)T[ID])[VarName]).ContainsKey(AttrName)==false)
							{
								((Hashtable)((Hashtable)T[ID])[VarName])[AttrName] = new Hashtable();
							}
							((Hashtable)((Hashtable)((Hashtable)T[ID])[VarName])[AttrName])[AttrValue] = VarValue;
						}
						XMLDoc.Read();
						XMLDoc.MoveToContent();
					}
				}
				else
				{
					XMLDoc.Skip();
				}
				XMLDoc.Read();
				XMLDoc.MoveToContent();
			}

			XMLDoc.Close();


			IDictionaryEnumerator inEn = T.GetEnumerator();
			IDictionaryEnumerator EvEn;
			Hashtable TT;
			while(inEn.MoveNext())
			{
				if((int)inEn.Key == InstanceID)
				{
					TT = (Hashtable)inEn.Value;
					EvEn = TT.GetEnumerator();
					while(EvEn.MoveNext())
					{
						switch((string)EvEn.Key)
						{
							case "Mute":
								if(EvEn.Value.GetType().FullName=="System.String")
								{
									MuteByChannel["MASTER"] = (bool)UPnPService.CreateObjectInstance(typeof(bool),(string)EvEn.Value);
								}
								else
								{
									IDictionaryEnumerator E2 = ((Hashtable)EvEn.Value).GetEnumerator();
									while(E2.MoveNext())
									{
										IDictionaryEnumerator E3 = ((Hashtable)((Hashtable)E2.Value)).GetEnumerator();
										while(E3.MoveNext())
										{
											MuteByChannel[E3.Key.ToString()] = (bool)UPnPService.CreateObjectInstance(typeof(bool),E3.Value.ToString());
										}
									}
								}

								if(OnMuteChanged!=null) OnMuteChanged(this);
								break;
							case "Volume":
								if(EvEn.Value.GetType().FullName=="System.String")
								{
									VolumeByChannel["MASTER"] = UInt16.Parse((string)EvEn.Value);
								}
								else
								{
									IDictionaryEnumerator E2 = ((Hashtable)EvEn.Value).GetEnumerator();
									while(E2.MoveNext())
									{
										IDictionaryEnumerator E3 = ((Hashtable)((Hashtable)E2.Value)).GetEnumerator();
										while(E3.MoveNext())
										{
											VolumeByChannel[E3.Key.ToString()] = UInt16.Parse(E3.Value.ToString());
										}
									}
								}
								//_Volume = UInt16.Parse((string)EvEn.Value);
								if(OnVolumeChanged!=null) OnVolumeChanged(this);
								break;
						}
					}
				}
			}
		}
	}
}
