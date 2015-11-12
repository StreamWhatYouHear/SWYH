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
using System.Threading;
using System.Collections;
using OpenSource.UPnP.AV;

namespace OpenSource.UPnP.AV.RENDERER.CP
{
	/// <summary>
	/// Summary description for AVTransportLastChange.
	/// </summary>
	public class AVTransportLastChange
	{
		private int StateCounter = 0;
		public delegate void ReadyHandler(AVTransportLastChange sender);
		public event ReadyHandler OnReady;

		public string Identifier = null;
		public delegate void VariableChangeHandler(AVTransportLastChange sender);
		public event VariableChangeHandler OnPlayStateChanged;
		public event VariableChangeHandler OnCurrentPositionChanged;
		public event VariableChangeHandler OnAVTransportURIChanged;
		public event VariableChangeHandler OnNextAVTransportURIChanged;
		public event VariableChangeHandler OnCurrentTrackChanged;
		public event VariableChangeHandler OnNumberOfTracksChanged;
		public event VariableChangeHandler OnTrackURIChanged;
		public event VariableChangeHandler OnCurrentURIMetaDataChanged;
		public event VariableChangeHandler OnCurrentPlayModeChanged;
		public event VariableChangeHandler OnTransportStatusChanged;

		public void Dispose()
		{
			this.OnAVTransportURIChanged = null;
			this.OnCurrentPositionChanged = null;
			this.OnCurrentTrackChanged = null;
			this.OnCurrentURIMetaDataChanged = null;
			this.OnNextAVTransportURIChanged = null;
			this.OnNumberOfTracksChanged = null;
			this.OnPlayStateChanged= null;
			this.OnReady = null;
			this.OnTrackURIChanged = null;
			OnTransportStatusChanged = null;
		}
		public string TransportStatus
		{
			get
			{
				return(_TransportStatus);
			}
		}
		public string CurrentURIMetaData
		{
			get
			{
				return(this._AVTransportMetaData);
			}
		}
		protected CpAVTransport _cp;

		protected LifeTimeMonitor PollerTimeout = new LifeTimeMonitor();
		protected ManualResetEvent PollerEvent = new ManualResetEvent(false);
		protected bool PollOK = false;
		

		protected UInt32 _Volume = 0;
		protected bool _Mute = false;
		protected string _PlayState = "STOPPED";
		protected string InstanceID;
		protected string _AVTransportURI = "";
		protected string _NextAVTransportURI = "";
		protected string _AVTransportMetaData = "";
		protected string _NextAVTransportMetaData = "";
		protected UInt32 _CurrentTrack = 0;
		protected UInt32 _NumberOfTracks = 0;
		protected string _TrackURI = "";
		protected string _CurrentPlayMode = "NORMAL";
		protected string _TransportStatus = "Unknown";

		protected TimeSpan _CurrentDuration = TimeSpan.FromSeconds(0);
		protected TimeSpan _CurrentPosition = TimeSpan.FromSeconds(0);

		public UInt32 CurrentTrack
		{
			get
			{
				return(_CurrentTrack);
			}
		}
		public UInt32 NumberOfTracks
		{
			get
			{
				return(_NumberOfTracks);
			}
		}
		public string TrackURI
		{
			get
			{
				return(_TrackURI);
			}
		}

		public string AVTransportURI
		{
			get
			{
				return(_AVTransportURI);
			}
		}
		public TimeSpan CurrentDuration
		{
			get
			{
				return(_CurrentDuration);
			}
		}
		public TimeSpan CurrentPosition
		{
			get
			{
				return(_CurrentPosition);
			}
		}

		public CpAVTransport.Enum_TransportState PlayState
		{
			get
			{
				CpAVTransport.Enum_TransportState RetVal = 0;
				switch(_PlayState)
				{
					case "STOPPED":
						RetVal = CpAVTransport.Enum_TransportState.STOPPED;
						break;
					case "PAUSED_PLAYBACK":
						RetVal = CpAVTransport.Enum_TransportState.PAUSED_PLAYBACK;
						break;
					case "PLAYING":
						RetVal = CpAVTransport.Enum_TransportState.PLAYING;
						break;
					case "PAUSED_RECORDING":
						RetVal = CpAVTransport.Enum_TransportState.PAUSED_RECORDING;
						break;
					case "RECORDING":
						RetVal = CpAVTransport.Enum_TransportState.RECORDING;
						break;
					case "TRANSITIONING":
						RetVal = CpAVTransport.Enum_TransportState.TRANSITIONING;
						break;
				}
				return(RetVal);
			}
		}

		public CpAVTransport.Enum_CurrentPlayMode CurrentPlayMode
		{
			get
			{
				CpAVTransport.Enum_CurrentPlayMode RetVal = CpAVTransport.Enum_CurrentPlayMode.NORMAL;
				switch(_CurrentPlayMode)
				{
					case "DIRECT_1":
						RetVal = CpAVTransport.Enum_CurrentPlayMode.DIRECT_1;
						break;
					case "INTRO":
						RetVal = CpAVTransport.Enum_CurrentPlayMode.INTRO;
						break;
					case "NORMAL":
						RetVal = CpAVTransport.Enum_CurrentPlayMode.NORMAL;
						break;
					case "RANDOM":
						RetVal = CpAVTransport.Enum_CurrentPlayMode.RANDOM;
						break;
					case "REPEAT_ALL":
						RetVal = CpAVTransport.Enum_CurrentPlayMode.REPEAT_ALL;
						break;
					case "REPEAT_ONE":
						RetVal = CpAVTransport.Enum_CurrentPlayMode.REPEAT_ONE;
						break;
					case "SHUFFLE":
						RetVal = CpAVTransport.Enum_CurrentPlayMode.SHUFFLE;
						break;
				}
				return(RetVal);
			}
		}

		public bool Mute
		{
			get
			{
				return(_Mute);
			}
		}
		public UInt32 Volume
		{
			get
			{
				return(_Volume);
			}
		}

		~AVTransportLastChange()
		{
			OpenSource.Utilities.InstanceTracker.Remove(this);
		}
		public AVTransportLastChange(CpAVTransport cpAV, string Ident, int ID, AVTransportLastChange.ReadyHandler ReadyCallback)
		{
			OpenSource.Utilities.InstanceTracker.Add(this);
			this.OnReady += ReadyCallback;
			InstanceID = ID.ToString();
			Identifier = Ident;
			_cp = cpAV;
			_cp.OnStateVariable_LastChange += new CpAVTransport.StateVariableModifiedHandler_LastChange(LastChangeSink);

			_cp._subscribe(500);


			lock(this)
			{
				if(_cp.HasAction_GetPositionInfo)
				{
					++StateCounter;
					//PollerTimeoutHandler = new LifeTimeMonitor.LifeTimeHandler(PollerTimeoutSink);
					//PollerTimeout.AddWeakEvent_OnExpired(PollerTimeoutHandler);

					_cp.GetPositionInfo((UInt32)ID,null,new CpAVTransport.Delegate_OnResult_GetPositionInfo(PositionInfoSink));
				}

				if(_cp.HasAction_GetMediaInfo)
				{
					++StateCounter;
					_cp.GetMediaInfo((UInt32)ID,null,new CpAVTransport.Delegate_OnResult_GetMediaInfo(MediaInfoSink));
				}

				if(_cp.HasAction_GetTransportInfo)
				{
					++StateCounter;
					_cp.GetTransportInfo((UInt32)ID,null,new CpAVTransport.Delegate_OnResult_GetTransportInfo(GetTransportInfoSink));
				}

				if(_cp.HasAction_GetTransportSettings)
				{
					++StateCounter;
					_cp.GetTransportSettings((UInt32)ID,null,new CpAVTransport.Delegate_OnResult_GetTransportSettings(GetTransportSettingsSink));
				}
			}
		}

		internal void _Poll()
		{
			if(this.PlayState==CpAVTransport.Enum_TransportState.PLAYING||
				this.PlayState==CpAVTransport.Enum_TransportState.RECORDING)
			{
				_cp.GetPositionInfo(UInt32.Parse(InstanceID),null,new CpAVTransport.Delegate_OnResult_GetPositionInfo(PollSink));
			}
		}

		private void GetTransportSettingsSink(CpAVTransport sender, System.UInt32 InstanceID, CpAVTransport.Enum_CurrentPlayMode PlayMode, CpAVTransport.Enum_CurrentRecordQualityMode RecQualityMode, UPnPInvokeException e, object _Tag)
		{
			if(e==null)
			{
				_CurrentPlayMode = _cp.Enum_CurrentPlayMode_ToString(PlayMode);
				if(this.OnCurrentPlayModeChanged!=null) OnCurrentPlayModeChanged(this);
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

		protected void PollSink(CpAVTransport sender, UInt32 InstanceID, 
			UInt32 Track,
			string TrackDuration,
			string TrackMetaData,
			string TrackURI,
			string RelTime,
			string AbsTime,
			int RelCount,
			int AbsCount,
			UPnPInvokeException e,
			object Handle)
		{

			DText p = new DText();
			p.ATTRMARK = ":";
			TimeSpan ts;


			try
			{
				p[0]= RelTime;
				_CurrentPosition = new TimeSpan(int.Parse(p[1]),int.Parse(p[2]),int.Parse(p[3]));
				
				if (this._CurrentTrack != Track) 
				{ 
					this._CurrentTrack = Track; 
					if (this.OnCurrentTrackChanged != null)
					{
						this.OnCurrentTrackChanged(this);
					}
				}
				
				p[0] = TrackDuration;
				ts = new TimeSpan(int.Parse(p[1]), int.Parse(p[2]), int.Parse(p[3]));
				if (this._CurrentDuration != ts) 
				{
					this._CurrentDuration = ts; 
				}

				if (this._TrackURI.CompareTo(TrackURI) != 0)
				{
					this._TrackURI = TrackURI;
					if (this.OnCurrentTrackChanged != null)
					{
						this.OnCurrentTrackChanged(this);
					}
				}

				if (this._AVTransportMetaData.CompareTo(TrackMetaData) != 0) 
				{ 
					this._AVTransportMetaData = TrackMetaData; 
					if (this.OnCurrentURIMetaDataChanged != null)
					{
						this.OnCurrentURIMetaDataChanged(this);
					}
				}
			}
			catch(Exception)
			{
			}

			if(OnCurrentPositionChanged!=null) OnCurrentPositionChanged(this);
		}

		protected void MediaInfoSink(CpAVTransport sender, System.UInt32 InstanceID, System.UInt32 NrTracks, System.String MediaDuration, System.String CurrentURI, System.String CurrentURIMetaData, System.String NextURI, System.String NextURIMetaData, CpAVTransport.Enum_PlaybackStorageMedium PlayMedium, CpAVTransport.Enum_RecordStorageMedium RecordMedium, CpAVTransport.Enum_RecordMediumWriteStatus WriteStatus, UPnPInvokeException e, object Handle)
		{
			_AVTransportURI = CurrentURI;
			_AVTransportMetaData = CurrentURIMetaData;
			if(this.OnAVTransportURIChanged!=null) OnAVTransportURIChanged(this);
		
			_NextAVTransportURI = NextURI;
			_NextAVTransportMetaData = NextURIMetaData;
			if(this.OnNextAVTransportURIChanged!=null) OnNextAVTransportURIChanged(this);
		
			_NumberOfTracks = NrTracks;
			this._AVTransportMetaData = CurrentURIMetaData;
			if(this.OnCurrentURIMetaDataChanged!=null) OnCurrentURIMetaDataChanged(this);
			if(OnNumberOfTracksChanged!=null) OnNumberOfTracksChanged(this);
			lock(this)
			{
				--StateCounter;
				if(StateCounter==0)
				{
					if(OnReady!=null)OnReady(this);
				}
			}
		}
		protected void GetTransportInfoSink(CpAVTransport sender, System.UInt32 InstanceID, CpAVTransport.Enum_TransportState CurrentTransportState, CpAVTransport.Enum_TransportStatus CurrentTransportStatus, CpAVTransport.Enum_TransportPlaySpeed CurrentSpeed, UPnPInvokeException e, object Handle)
		{
			if(e!=null) return;

			_TransportStatus = _cp.Enum_TransportStatus_ToString(CurrentTransportStatus);

			switch(CurrentTransportState)
			{
				case CpAVTransport.Enum_TransportState.PLAYING:
					this._PlayState = "PLAYING";
					break;
				case CpAVTransport.Enum_TransportState.STOPPED:
					this._PlayState = "STOPPED";
					break;
				case CpAVTransport.Enum_TransportState.PAUSED_PLAYBACK:
					this._PlayState = "PAUSED_PLAYBACK";
					break;
				case CpAVTransport.Enum_TransportState.PAUSED_RECORDING:
					this._PlayState = "PAUSED_RECORDING";
					break;
				case CpAVTransport.Enum_TransportState.RECORDING:
					this._PlayState = "RECORDING";
					break;
				case CpAVTransport.Enum_TransportState.TRANSITIONING:
					this._PlayState = "TRANSITIONING";
					break;
			}

			if(this.OnPlayStateChanged!=null) OnPlayStateChanged(this);
			if(this.OnTransportStatusChanged!=null) OnTransportStatusChanged(this);

			lock(this)
			{
				--StateCounter;
				if(StateCounter==0)
				{
					if(OnReady!=null)OnReady(this);
				}
			}
		}
		protected void PositionInfoSink(CpAVTransport sender, UInt32 InstanceID, 
			UInt32 Track,
			string TrackDuration,
			string TrackMetaData,
			string TrackURI,
			string RelTime,
			string AbsTime,
			int RelCount,
			int AbsCount,
			UPnPInvokeException e,
			object Handle)
		{

			DText p = new DText();
			p.ATTRMARK = ":";
			try
			{
				p[0] = TrackDuration;
				_CurrentDuration = new TimeSpan(int.Parse(p[1]),int.Parse(p[2]),int.Parse(p[3]));
			}
			catch(Exception)
			{
			}

			try
			{
				p[0]= RelTime;
				_CurrentPosition = new TimeSpan(int.Parse(p[1]),int.Parse(p[2]),int.Parse(p[3]));
			}
			catch(Exception)
			{
			}

			if(OnCurrentPositionChanged!=null) OnCurrentPositionChanged(this);

			_CurrentTrack = Track;
			this._TrackURI = HTTPMessage.UnEscapeString(TrackURI);
			if(this.OnTrackURIChanged!=null) OnTrackURIChanged(this);
			if(OnCurrentTrackChanged!=null) OnCurrentTrackChanged(this);
			lock(this)
			{
				--StateCounter;
				if(StateCounter==0)
				{
					if(OnReady!=null)OnReady(this);
				}
			}
		}

		protected void LastChangeSink(CpAVTransport sender, string LC)
		{
			if(LC=="")return;
			//LC = UPnPStringFormatter.UnEscapeString(LC);
			if(LC.Substring(0,1)!="<") LC = UPnPStringFormatter.UnEscapeString(LC);
			if(LC.Substring(0,1)!="<") LC = UPnPStringFormatter.UnEscapeString(LC);

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
						if(XMLDoc.IsStartElement()==true)
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
									VarValue = UPnPStringFormatter.UnEscapeString(XMLDoc.GetAttribute(a_idx));
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
			string TempString;
			DText TempParser = new DText();

			while(inEn.MoveNext())
			{
				if(inEn.Key.ToString() == InstanceID)
				{
					TT = (Hashtable)inEn.Value;
					EvEn = TT.GetEnumerator();
					while(EvEn.MoveNext())
					{
						switch((string)EvEn.Key)
						{
							case "AVTransportURI":
								_AVTransportURI = (string)EvEn.Value;
								if(OnAVTransportURIChanged!=null) OnAVTransportURIChanged(this);
								break;	
						
							case "TransportState":
								_PlayState = (string)EvEn.Value;	
								if(OnPlayStateChanged!=null) OnPlayStateChanged(this);
								break;

							case "CurrentTrackDuration":
								TempString = (string)EvEn.Value;
								TempParser.ATTRMARK = ":";
								TempParser[0] = TempString;
								try
								{
									this._CurrentDuration = new TimeSpan(int.Parse(TempParser[1]),int.Parse(TempParser[2]),int.Parse(TempParser[3]));
								}
								catch(Exception)
								{}
								if(this.OnCurrentPositionChanged!=null) this.OnCurrentPositionChanged(this);
								break;

							case "RelativeTimePosition":
								TempString = (string)EvEn.Value;
								TempParser.ATTRMARK = ":";
								TempParser[0] = TempString;
								this._CurrentPosition = new TimeSpan(int.Parse(TempParser[1]),int.Parse(TempParser[2]),int.Parse(TempParser[3]));
								if(this.OnCurrentPositionChanged!=null) this.OnCurrentPositionChanged(this);
								break;

							case "NumberOfTracks":
								try
								{
									_NumberOfTracks = UInt32.Parse((string)EvEn.Value);	
									if(OnNumberOfTracksChanged!=null) OnNumberOfTracksChanged(this);
								}
								catch(Exception)
								{}
								break;

							case "CurrentTrack":
								try
								{
									_CurrentTrack = UInt32.Parse((string)EvEn.Value);	
									if(OnCurrentTrackChanged!=null) OnCurrentTrackChanged(this);
								}
								catch(Exception)
								{}
								break;

							case "CurrentTrackURI":
								_TrackURI = (string)EvEn.Value;
								if(OnTrackURIChanged!=null) OnTrackURIChanged(this);
								break;
							case "TransportStatus":
								_TransportStatus = (string)EvEn.Value;
								if(OnTransportStatusChanged!=null) OnTransportStatusChanged(this);
								break;
							case "AVTransportURIMetaData":
								_AVTransportMetaData = (string)EvEn.Value;
								if(this.OnCurrentURIMetaDataChanged!=null) OnCurrentURIMetaDataChanged(this);
								break;

							case "CurrentPlayMode":
								_CurrentPlayMode = (string)EvEn.Value;
								if(this.OnCurrentPlayModeChanged!=null) OnCurrentPlayModeChanged(this);
								break;
						}
					}
				}
			}
		}

	}
}

