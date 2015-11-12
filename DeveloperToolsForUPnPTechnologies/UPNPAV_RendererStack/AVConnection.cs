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
using OpenSource.UPnP;
using OpenSource.UPnP.AV;
using OpenSource.Utilities;
using OpenSource.UPnP.AV.CdsMetadata;

namespace OpenSource.UPnP.AV.RENDERER.CP
{
	/// <summary>
	/// This is the main abstraction object that a developer will be interacting
	/// with, when controlling a rendering device.
	/// <para>
	/// This object abstracts all the functionality that a renderer exposes to control points, such as volume, position, 
	/// etc. This object is pretty primitive in how it is used, as it is basically a portal into the renderer session. Most of the grunt work of a control point will
	/// rest in obtaining these AVConnection objects. Yet, one will never need to directly instantiate one of these objects, as they will be created for you, by the <see cref="OpenSource.UPnP.AVRENDERERSTACK.AVRenderer"/>
	/// object.
	/// </para>
	/// </summary>
	public class AVConnection
	{
		public class MimeTypeMismatchException : Exception
		{
			public MimeTypeMismatchException(String msg):base(msg)
			{
			}
		}


		internal string ProtocolInfoString = null;
		private int StateCounter = 0;
		private object Tag = null;
		public delegate void OnReadyHandler(AVConnection sender, object Tag);
		public event OnReadyHandler OnReady;

		public delegate void OnReuseConnectionHandler(AVConnection sender, object Tag);
		public event OnReuseConnectionHandler OnReuseConnection;
		public event OnReuseConnectionHandler OnReuseConnectionFailed;

		private Hashtable PlayListTable = new Hashtable();

		public void CreateConnection(IMediaContainer Container)
		{
			CreateConnection(Container,null);
		}
		public void CreateConnection(IMediaContainer Container, object Tag)
		{
			ArrayList RList = new ArrayList();
			foreach(IMediaItem item in Container.Items)
			{
				foreach(IMediaResource r in item.MergedResources)
				{
					if(Parent.SupportsProtocolInfo(r.ProtocolInfo))
					{
						RList.Add(r);
						break;
					}
				}
			}

			if(RList.Count==0)
			{
				//ToDo: Throw Exception here
				return;
			}

			CreateConnection((IMediaResource[])RList.ToArray(typeof(IMediaResource)),Tag);
		}
		public void CreateConnection(IMediaItem item)
		{
			CreateConnection(item,null);
		}
		public void CreateConnection(IMediaItem item, object Tag)
		{
			CreateConnection(new IMediaItem[1]{item},Tag);
		}
		public void CreateConnection(IMediaItem[] items)
		{
			CreateConnection(items,null);
		}
		public void CreateConnection(IMediaItem[] items, object Tag)
		{
			ArrayList RList = new ArrayList();

			foreach(IMediaItem item in items)
			{
				foreach(IMediaResource resource in item.MergedResources)
				{
					if(Parent.SupportsProtocolInfo(resource.ProtocolInfo))
					{
						// Use this resource
						RList.Add(resource);
						break;
					}
				}
			}

			if(RList.Count==0)
			{
				//ToDo: Throw Exception Here
				return;
			}
			CreateConnection((IMediaResource[])RList.ToArray(typeof(IMediaResource)),Tag);
		}
		public void CreateConnection(IMediaResource media)
		{
			CreateConnection(media,null);
		}
		public void CreateConnection(IMediaResource media, object Tag)
		{
			CreateConnection(new IMediaResource[1]{media}, Tag);
		}
		public void CreateConnection(IMediaResource[] resources)
		{
			CreateConnection(resources,null);
		}
		public void CreateConnection(IMediaResource[] resources, object Tag)
		{
			if(resources.Length>1)
			{
				if(!(new ProtocolInfoString(this.ProtocolInfoString)).Matches(new ProtocolInfoString("http-get:*:audio/mpegurl:*")))
				{
					throw(new MimeTypeMismatchException("Cannot call SetAVTransportURI with a different MimeType"));
				}
			}
			else
			{
				if (this.Parent.HasConnectionHandling)
				{
					if(!(new ProtocolInfoString(this.ProtocolInfoString).Matches(resources[0].ProtocolInfo)))
					{
						throw(new MimeTypeMismatchException("Cannot call SetAVTransportURI with a different MimeType"));
					}
				}
				else
				{
					if(!this.Parent.SupportsProtocolInfo(resources[0].ProtocolInfo))
					{
						throw(new MimeTypeMismatchException("Cannot call SetAVTransportURI with a different MimeType"));
					}
				}
			}

			AVPlayList pl = new AVPlayList(this,resources,new AVPlayList.ReadyHandler(PlayListSink), new AVPlayList.FailedHandler(PlayListFailedSink),Tag);
			PlayListTable[pl.GetHashCode()] = pl;
		}

		private void PlayListSink(AVPlayList sender, AVConnection c, object Tag)
		{
			PlayListTable.Remove(sender.GetHashCode());
			if(OnReuseConnection!=null) OnReuseConnection(this,Tag);
		}
		private void PlayListFailedSink(AVPlayList sender, AVRenderer.CreateFailedReason reason)
		{
			PlayListTable.Remove(sender.GetHashCode());
			if(OnReuseConnectionFailed!=null) OnReuseConnectionFailed(this,Tag);
		}


		public void Dispose()
		{
			AVTransport.Dispose();
			RenderingControl.Dispose();
			ConnectionManager.Dispose();

			this.AV_LastChange.Dispose();
			this.AV_LastChange = null;
			this.RC_LastChange.Dispose();
			this.RC_LastChange = null;
			if(CurrentPlayList!=null)
				CurrentPlayList.Dispose();
			this.CurrentPlayList = null;

			this.OnCurrentMetaDataChanged = null;

			this.OnMediaResourceChanged = null;
			this.OnMute = null;
			this.OnNumberOfTracksChanged = null;

			this.OnPositionChanged = null;
			this.OnReady = null;
			this.OnRemoved = null;

			this.OnTrackChanged = null;
			this.OnTrackURIChanged = null;
			this.OnVolume = null;

			if(OnRemoved!=null) OnRemoved(this);
		}

		/// <summary>
		/// Enumerations representing the possible PlayStates
		/// </summary>
		public enum PlayState
		{
			PLAYING,
			RECORDING,
			SEEKING,
			STOPPED,
			PAUSED,
			TRANSITIONING,
		}

		public enum PlayMode
		{
			NORMAL,
			INTRO,
			DIRECT_1,
			SHUFFLE,
			RANDOM,
			REPEAT_ONE,
			REPEAT_ALL,
		}

		// This event handles the Current Track URI
		public delegate void TrackURIChangedHandler(AVConnection sender);
		public event TrackURIChangedHandler OnTrackURIChanged;

		// This event handles the CurrentPlayMode
		public delegate void CurrentPlayModeChangedHandler(AVConnection sender, PlayMode NewMode);
		public event CurrentPlayModeChangedHandler OnCurrentPlayModeChanged;

		// This event handles the Current Meta Data
		public delegate void CurrentMetaDataChangedHandler(AVConnection sender);
		public event CurrentMetaDataChangedHandler OnCurrentMetaDataChanged;

		// This internal event handles the current AVTransportURI
		internal delegate void CurrentURIChangedHandler(AVConnection sender);
		private WeakEvent OnCurrentURIChangedEvent = new WeakEvent();
		internal event CurrentURIChangedHandler OnCurrentURIChanged
		{
			add
			{
				OnCurrentURIChangedEvent.Register(value);
			}
			remove
			{
				OnCurrentURIChangedEvent.UnRegister(value);
			}
		}

		public delegate void MediaResourceChangedHandler(AVConnection sender, IMediaResource NewResource);
		/// <summary>
		/// Fired when the Current Media Resource changed
		/// </summary>
		public event MediaResourceChangedHandler OnMediaResourceChanged;

		public delegate void RendererHandler(AVConnection sender);
		/// <summary>
		/// Fired when this connection was closed
		/// </summary>
		public event RendererHandler OnRemoved;

		public delegate void CurrentTrackChangedHandler(AVConnection sender, UInt32 NewTrackNumber);
		/// <summary>
		/// Fired when the current track changed
		/// </summary>
		public event CurrentTrackChangedHandler OnTrackChanged;

		public delegate void NumberOfTracksChangedHandler(AVConnection sender, UInt32 NewNumberOfTracks);
		/// <summary>
		/// Fired when the number of tracks changed
		/// </summary>
		public event NumberOfTracksChangedHandler OnNumberOfTracksChanged;

		public delegate void TransportStatusChangedHandler(AVConnection sender, string NewTransportStatus);
		private WeakEvent OnTransportStatusChangedEvent = new WeakEvent();
		public event TransportStatusChangedHandler OnTransportStatusChanged
		{
			add
			{
				OnTransportStatusChangedEvent.Register(value);
			}
			remove
			{
				OnTransportStatusChangedEvent.UnRegister(value);
			}
		}


		public delegate void PlayStateChangedHandler(AVConnection sender, PlayState NewState);
		private WeakEvent OnPlayStateChangedEvent = new WeakEvent();
		/// <summary>
		/// Fired when the current play state changed
		/// </summary>
		public event PlayStateChangedHandler OnPlayStateChanged
		{
			add
			{
				OnPlayStateChangedEvent.Register(value);
			}
			remove
			{
				OnPlayStateChangedEvent.UnRegister(value);
			}
		}


		public delegate void MuteStateChangedHandler(AVConnection sender, bool NewMuteStatus);
		/// <summary>
		/// Fired when the mute state changed
		/// </summary>
		public event MuteStateChangedHandler OnMute;

		public delegate void VolumeChangedHandler(AVConnection sender, UInt16 Volume);
		/// <summary>
		/// Fired when the volume changed
		/// </summary>
		public event VolumeChangedHandler OnVolume;

		public delegate void PositionChangedHandler(AVConnection sender, TimeSpan Position);
		/// <summary>
		/// Fired when the current position changed
		/// </summary>
		public event PositionChangedHandler OnPositionChanged;

		// This internal event is triggered when SetAVTransportURI completes
		public delegate void AVTransportSetHandler(AVConnection sender, object Tag);
		private WeakEvent OnSetAVTransportURIEvent = new WeakEvent();
		internal event AVTransportSetHandler OnSetAVTransportURI
		{
			add
			{
				OnSetAVTransportURIEvent.Register(value);
			}
			remove
			{
				OnSetAVTransportURIEvent.UnRegister(value);
			}
		}


		protected CpAVTransport AVTransport;
		protected CpRenderingControl RenderingControl;
		protected CpConnectionManager ConnectionManager;
		protected RenderingControlLastChange RC_LastChange;
		protected AVTransportLastChange AV_LastChange;
		/// <summary>
		/// Connection Manager ID
		/// </summary>
		protected Int32 CMid;
		/// <summary>
		/// AVTransport ID and Rendering Control ID
		/// </summary>
		protected int AVTid,RCid;
		/// <summary>
		/// The UPnP Friendly name for the RendererDevice
		/// </summary>
		public string FriendlyName;
		/// <summary>
		/// A Unique identifier for this AVConnection
		/// </summary>
		public string Identifier;
		
		private IMediaContainer _Container = null;
		private IMediaItem _CurrentItem = null;

		public IMediaContainer Container
		{
			get
			{
				return(_Container);
			}
		}
		/// <summary>
		/// The Current Item that is playing
		/// </summary>
		public IMediaItem CurrentItem
		{
			get
			{
				return(_CurrentItem);
			}
		}

		protected UInt16 MaxVolume = 0;
		protected UInt16 MinVolume = 0;


		protected IMediaResource _resource = null;
		internal AVRenderer _Parent = null;
		/// <summary>
		/// Upon successful connection, this is the sole reference to the AVPlaylist that
		/// is streaming the playlist
		/// </summary>
		internal AVPlayList CurrentPlayList = null;

		public string GetFileNameForLocallyServedMedia(Uri MediaUri)
		{
			if(this.CurrentPlayList!=null)
			{
				return(this.CurrentPlayList.GetFilenameFromURI(MediaUri));
			}
			else
			{
				return(null);
			}
		}

		/// <summary>
		/// The UPnP A/V Connection ID
		/// </summary>
		public Int32 ConnectionID
		{
			get
			{
				return(this.CMid);
			}
		}
		
		/// <summary>
		/// The AVRenderer that owns this AVConnection
		/// </summary>
		public AVRenderer Parent
		{
			get
			{
				return(_Parent);
			}
		}

		/// <summary>
		/// bool indicating if this renderer implements ConnectionComplete
		/// </summary>
		public bool IsCloseSupported
		{
			get
			{
				return(ConnectionManager.HasAction_ConnectionComplete);
			}
		}

		/// <summary>
		/// Closes the connection, if supported
		/// </summary>
		public void Close()
		{
			if(IsCloseSupported)
			{
				ConnectionManager.ConnectionComplete(ConnectionID);
			}
		}

		~AVConnection()
		{
			OpenSource.Utilities.InstanceTracker.Remove(this);
		}

		/// <summary>
		/// This construct is only called by the AVRenderer object.
		/// </summary>
		/// <param name="device"></param>
		/// <param name="AVTransportID"></param>
		/// <param name="RenderingControlID"></param>
		/// <param name="ConnectionID"></param>
		/// <param name="ReadyCallback"></param>
		/// <param name="StateObject"></param>
		internal AVConnection(UPnPDevice device, int AVTransportID, int RenderingControlID, Int32 ConnectionID, AVConnection.OnReadyHandler ReadyCallback, object StateObject)
		{
			OpenSource.Utilities.InstanceTracker.Add(this);
			this.Tag = StateObject;
			this.OnReady += ReadyCallback;

			FriendlyName = device.FriendlyName;
			Identifier = device.UniqueDeviceName + ":" + ConnectionID.ToString();
			AVTid = AVTransportID;
			RCid = RenderingControlID;
			CMid = ConnectionID;

			AVTransport = new CpAVTransport(device.GetServices(CpAVTransport.SERVICE_NAME)[0]);
			RenderingControl = new CpRenderingControl(device.GetServices(CpRenderingControl.SERVICE_NAME)[0]);
			ConnectionManager = new CpConnectionManager(device.GetServices(CpConnectionManager.SERVICE_NAME)[0]);

			if(RenderingControl.HasStateVariable_Volume)
			{
				// If the renderer has defined ranges, use those
				if(RenderingControl.HasMaximum_Volume)
				{
					MaxVolume = (UInt16)RenderingControl.Maximum_Volume;
				}
				else
				{
					MaxVolume = UInt16.MaxValue;
				}

				if(RenderingControl.HasMinimum_Volume)
				{
					MinVolume = (UInt16)RenderingControl.Minimum_Volume;
				}
				else
				{
					MinVolume = UInt16.MinValue;
				}
			}
			
			lock(this)
			{
				if(AVTransport.HasStateVariable_LastChange)
				{
					// Hook up to the LastChange event of AVTransport
					++this.StateCounter;
					AV_LastChange = new AVTransportLastChange(AVTransport,device.UniqueDeviceName, AVTid, new AVTransportLastChange.ReadyHandler(AVTLC));
					AV_LastChange.OnCurrentPositionChanged += new AVTransportLastChange.VariableChangeHandler(PositionSink);
					AV_LastChange.OnPlayStateChanged += new AVTransportLastChange.VariableChangeHandler(PlayStateSink);
					AV_LastChange.OnAVTransportURIChanged += new AVTransportLastChange.VariableChangeHandler(AVTransportURISink);
					AV_LastChange.OnCurrentTrackChanged += new AVTransportLastChange.VariableChangeHandler(TrackChangedSink);
					AV_LastChange.OnNumberOfTracksChanged += new AVTransportLastChange.VariableChangeHandler(NumberOfTracksChangedSink);
					AV_LastChange.OnTrackURIChanged += new AVTransportLastChange.VariableChangeHandler(TrackURIChangedSink);
					AV_LastChange.OnCurrentURIMetaDataChanged += new AVTransportLastChange.VariableChangeHandler(URIMetaDataChangedSink);
					AV_LastChange.OnCurrentPlayModeChanged += new AVTransportLastChange.VariableChangeHandler(CurrentPlayModeChangedSink);
					AV_LastChange.OnTransportStatusChanged += new AVTransportLastChange.VariableChangeHandler(TransportStatusChangedSink);
				}

				if(RenderingControl.HasStateVariable_LastChange)
				{
					// Hook up to the LastChange event of RenderingControl
					++this.StateCounter;
					RC_LastChange = new RenderingControlLastChange(RenderingControl,device.UniqueDeviceName, RCid, new RenderingControlLastChange.OnReadyHandler(RCLC));
					RC_LastChange.OnMuteChanged += new RenderingControlLastChange.VariableChangeHandler(MuteSink);
					RC_LastChange.OnVolumeChanged += new RenderingControlLastChange.VariableChangeHandler(VolumeSink);
				}

				/* Get ProtocolInfo Value of current connection */
				++this.StateCounter;
				ConnectionManager.GetCurrentConnectionInfo(ConnectionID,this.GetHashCode(),new CpConnectionManager.Delegate_OnResult_GetCurrentConnectionInfo(InitialState_GetCurrentConnectionInfoSink));
			}
			RenderingControl._subscribe(500);
			AVTransport._subscribe(500);
		}

		public PlayMode[] GetSupportedPlayModes()
		{
			// Returns all the PlayModes that the device is advertising it supports
			string[] RawModes = AVTransport.Values_CurrentPlayMode;
			ArrayList t = new ArrayList();
			foreach(string s in RawModes)
			{
				t.Add(StringToPlayMode(s));
			}
			return((PlayMode[])t.ToArray(typeof(PlayMode)));
		}
		/// <summary>
		/// This method converts a string value of a play mode
		/// into an enum type
		/// </summary>
		/// <param name="v"></param>
		/// <returns></returns>
		private PlayMode StringToPlayMode(string v)
		{
			PlayMode RetVal = PlayMode.NORMAL;
			switch(v)
			{
				case "DIRECT_1":
					RetVal = PlayMode.DIRECT_1;
					break;
				case "INTRO":
					RetVal = PlayMode.INTRO;
					break;
				case "NORMAL":
					RetVal = PlayMode.NORMAL;
					break;
				case "RANDOM":
					RetVal = PlayMode.RANDOM;
					break;
				case "REPEAT_ALL":
					RetVal = PlayMode.REPEAT_ALL;
					break;
				case "REPEAT_ONE":
					RetVal = PlayMode.REPEAT_ONE;
					break;
				case "SHUFFLE":
					RetVal = PlayMode.SHUFFLE;
					break;
			}
			return(RetVal);
		}

		/// <summary>
		/// This is called when AVTransportLastChange has acquired state
		/// </summary>
		/// <param name="sender"></param>
		private void AVTLC(AVTransportLastChange sender)
		{
			bool OK = false;
			lock(this)
			{
				--StateCounter;
				if(StateCounter==0)
				{
					this.UpdateCurrentItem();
					OK = true;
				}
			}

			if(OK)
				if(OnReady!=null) OnReady(this, Tag);
		}
		/// <summary>
		/// This is called when RenderingControlLastChange has acquired state
		/// </summary>
		/// <param name="sender"></param>
		private void RCLC(RenderingControlLastChange sender)
		{
			bool OK = false;
			lock(this)
			{
				--StateCounter;
				if(StateCounter==0)
				{
					this.UpdateCurrentItem();
					OK = true;
				}
			}
			if(OK)
				if(OnReady!=null) OnReady(this, Tag);
		}

		private void CurrentPlayModeChangedSink(AVTransportLastChange sender)
		{
			if(this.OnCurrentPlayModeChanged!=null)
			{
				OnCurrentPlayModeChanged(this,CurrentPlayMode);
			}
		}

		protected void URIMetaDataChangedSink(AVTransportLastChange sender)
		{
			
			if(sender.CurrentURIMetaData!="" && sender.CurrentURIMetaData!="NOT_IMPLEMENTED")
			{
				ArrayList a = MediaBuilder.BuildMediaBranches(sender.CurrentURIMetaData);
				if(a.Count>0)
				{
					// Since new metadata was evented, we need to update the container, and then
					// update the current item
					_Container = (IMediaContainer)a[0];
					UpdateCurrentItem();
				}
			}
			
		}

		protected void TrackChangedSink(AVTransportLastChange sender)
		{
			if(this.OnTrackChanged!=null) OnTrackChanged(this,sender.CurrentTrack);
		}
		protected void NumberOfTracksChangedSink(AVTransportLastChange sender)
		{
			if(this.OnNumberOfTracksChanged!=null) OnNumberOfTracksChanged(this,sender.NumberOfTracks);
		}
		/// <summary>
		/// Based on the MetaData that was evented, and the current track uri, this method
		/// will determine what is currently playing, and reflect it in the CurrentItem property
		/// </summary>
		/// <returns></returns>
		protected bool UpdateCurrentItem()
		{
			bool RetVal = false;
			this._CurrentItem = null;
			if(this.Container!=null)
			{
				foreach(IMediaItem Item in this.Container.Items)
				{
					foreach(IMediaResource R in Item.MergedResources)
					{
						if(R.ContentUri==this.TrackURI)
						{
							_CurrentItem = (IMediaItem)R.Owner;
							RetVal = true;
							break;
						}
					}
				}
			}

			if(RetVal)
			{
				if(this.OnCurrentMetaDataChanged!=null)
					OnCurrentMetaDataChanged(this);
			}
			return(RetVal);
		}
		protected void TrackURIChangedSink(AVTransportLastChange sender)
		{
			UpdateCurrentItem();
			if(this.OnTrackURIChanged!=null) OnTrackURIChanged(this);
		}

		public string TrackURI
		{
			get
			{
				return(AV_LastChange.TrackURI);
			}
		}

		/// <summary>
		/// bool indicating if the renderer implements seek
		/// </summary>
		public bool SupportsSeek
		{
			get
			{
				return(AVTransport.HasAction_Seek);
			}
		}
		/// <summary>
		/// bool indicating if the renderer implements pause
		/// </summary>
		public bool SupportsPause
		{
			get
			{
				return(AVTransport.HasAction_Pause);
			}
		}
		/// <summary>
		/// bool indicating if the renderer implements record
		/// </summary>
		public bool SupportsRecord
		{
			get
			{
				return(AVTransport.HasAction_Record);
			}
		}
		/// <summary>
		/// bool indicating if the renderer implements current position
		/// </summary>
		public bool SupportsCurrentPosition
		{
			get
			{
				return(AVTransport.HasStateVariable_RelativeTimePosition);
			}
		}

		/// <summary>
		/// The current track
		/// </summary>
		public UInt32 CurrentTrack
		{
			get
			{
				if(AV_LastChange==null) return(0);
				return(AV_LastChange.CurrentTrack);
			}
		}
		/// <summary>
		/// The total number of tracks
		/// </summary>
		public UInt32 NumberOfTracks
		{
			get
			{
				if(AV_LastChange==null) return(0);
				return(AV_LastChange.NumberOfTracks);
			}
		}

		/// <summary>
		/// The duration of the current track
		/// </summary>
		public TimeSpan Duration
		{
			get
			{
				if(AV_LastChange==null) return(TimeSpan.FromTicks(0));
				return(AV_LastChange.CurrentDuration);
			}
		}

		/// <summary>
		/// The current relative time position
		/// </summary>
		public TimeSpan CurrentPosition
		{
			get
			{
				if(AV_LastChange==null) return(TimeSpan.FromTicks(0));
				return(AV_LastChange.CurrentPosition);
			}
		}

		/// <summary>
		/// The current IMediaResource
		/// </summary>
		public IMediaResource MediaResource
		{
			get
			{
				return(_resource);
			}
			set
			{
				_resource = value;
				//this.SetAVTransportURI(_resource.ContentUri);
			}
		}

		protected void TransportStatusChangedSink(AVTransportLastChange sender)
		{
			this.OnTransportStatusChangedEvent.Fire(this,TransportStatus);
		}

		/// <summary>
		/// This is called when AVTransportLastChange receives an event that this has changed
		/// </summary>
		/// <param name="sender"></param>
		protected void AVTransportURISink(AVTransportLastChange sender)
		{
			this.MediaResource = ResourceBuilder.CreateResource(sender.AVTransportURI,"");
			OnCurrentURIChangedEvent.Fire(this);
			ConnectionManager.GetCurrentConnectionInfo(this.ConnectionID,null,new CpConnectionManager.Delegate_OnResult_GetCurrentConnectionInfo(ConnectionInfoSink));
		}
		/// <summary>
		/// This is called when GetCurrentConnectionInfo completes. 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="ConnectionID"></param>
		/// <param name="RcsID"></param>
		/// <param name="AVTransportID"></param>
		/// <param name="ProtocolInfo"></param>
		/// <param name="PeerConnectionManager"></param>
		/// <param name="PeerConnectionID"></param>
		/// <param name="Direction"></param>
		/// <param name="Status"></param>
		/// <param name="e"></param>
		/// <param name="Tag"></param>
		protected void ConnectionInfoSink(CpConnectionManager sender, System.Int32 ConnectionID, System.Int32 RcsID, System.Int32 AVTransportID, System.String ProtocolInfo, System.String PeerConnectionManager, System.Int32 PeerConnectionID, CpConnectionManager.Enum_A_ARG_TYPE_Direction Direction, CpConnectionManager.Enum_A_ARG_TYPE_ConnectionStatus Status, UPnPInvokeException e, object Tag)
		{
			if(e!=null) return;
			MediaResource = ResourceBuilder.CreateResource(MediaResource.ContentUri,ProtocolInfo);
			if(OnMediaResourceChanged!=null) OnMediaResourceChanged(this,MediaResource);
		}
		protected void InitialState_GetCurrentConnectionInfoSink(CpConnectionManager sender, System.Int32 ConnectionID, System.Int32 RcsID, System.Int32 AVTransportID, System.String ProtocolInfo, System.String PeerConnectionManager, System.Int32 PeerConnectionID, CpConnectionManager.Enum_A_ARG_TYPE_Direction Direction, CpConnectionManager.Enum_A_ARG_TYPE_ConnectionStatus Status, UPnPInvokeException e, object __tag)
		{
			this.ProtocolInfoString = ProtocolInfo;

			bool OK = false;
			lock(this)
			{
				--StateCounter;
				if(StateCounter==0)
				{
					this.UpdateCurrentItem();
					OK = true;
				}
			}

			if(OK)
				if(OnReady!=null) OnReady(this, Tag);

		}
		protected void PositionSink(AVTransportLastChange sender)
		{
			if(OnPositionChanged!=null) OnPositionChanged(this,sender.CurrentPosition);
		}
		protected void VolumeSink(RenderingControlLastChange sender)
		{
			double VolP = ((double)sender.GetVolume("Master")-(double)MinVolume)/(double)(MaxVolume-MinVolume);
			UInt16 NewVolume = (UInt16)(VolP*(double)100);

			if(OnVolume!=null) OnVolume(this,NewVolume);
		}
		protected void MuteSink(RenderingControlLastChange sender)
		{
			if(OnMute!=null) OnMute(this, sender.Mute);
		}
		protected void PlayStateSink(AVTransportLastChange sender)
		{
			PlayState state = PlayState.STOPPED;
			switch(sender.PlayState)
			{
				case CpAVTransport.Enum_TransportState.PLAYING:
					state = AVConnection.PlayState.PLAYING;
					break;
				case CpAVTransport.Enum_TransportState.PAUSED_PLAYBACK:
					state = AVConnection.PlayState.PAUSED;
					break;
				case CpAVTransport.Enum_TransportState.PAUSED_RECORDING:
					state = AVConnection.PlayState.PAUSED;
					break;
				case CpAVTransport.Enum_TransportState.STOPPED:
					state = AVConnection.PlayState.STOPPED;
					break;
				case CpAVTransport.Enum_TransportState.TRANSITIONING:
					state = AVConnection.PlayState.TRANSITIONING;
					break;
				case CpAVTransport.Enum_TransportState.RECORDING:
					state = AVConnection.PlayState.RECORDING;
					break;
			}
			OnPlayStateChangedEvent.Fire(this, state);
		}

		/// <summary>
		/// Play the current resource
		/// </summary>
		public void Play()
		{
			AVTransport.Play((UInt32)AVTid,CpAVTransport.Enum_TransportPlaySpeed._1);
		}
		/// <summary>
		/// Record the current resource
		/// </summary>
		public void Record()
		{
			AVTransport.Record((UInt32)AVTid);
		}

		/// <summary>
		/// Seek to the given relative time position
		/// </summary>
		/// <param name="TrackPosition">Time Position to seek to</param>
		public void SeekPosition(TimeSpan TrackPosition)
		{
			if(AVTransport.HasAction_Seek)
			{
				if(TrackPosition.TotalSeconds <0) TrackPosition = new TimeSpan(0,0,0,0);
				string Target = string.Format("{0:00}",TrackPosition.Hours) + ":" + string.Format("{0:00}",TrackPosition.Minutes) + ":" + string.Format("{0:00}",TrackPosition.Seconds);
				AVTransport.Seek((UInt32)AVTid,CpAVTransport.Enum_A_ARG_TYPE_SeekMode.REL_TIME,Target);
			}
		}
		/// <summary>
		/// Seek to the given track number
		/// </summary>
		/// <param name="TrackNumber">The track number to seek to</param>
		public void SeekTrack(int TrackNumber)
		{
			if(AVTransport.HasAction_Seek)
			{
				AVTransport.Seek((UInt32)AVTid,CpAVTransport.Enum_A_ARG_TYPE_SeekMode.TRACK_NR,TrackNumber.ToString());
			}
		}

		/// <summary>
		/// stop the current resource
		/// </summary>
		public void Stop()
		{
			AVTransport.Stop((UInt32)AVTid);
		}
		/// <summary>
		/// paues the current resource
		/// </summary>
		public void Pause()
		{
			if(AVTransport.HasAction_Pause)
			{
				AVTransport.Pause((UInt32)AVTid);
			}
		}
		/// <summary>
		/// Toggle the current mute state
		/// </summary>
		public void ToggleMute()
		{
			RenderingControl.SetMute((UInt32)RCid,CpRenderingControl.Enum_A_ARG_TYPE_Channel.MASTER,!RC_LastChange.Mute);
		}
		/// <summary>
		/// Set the mute state, to the given state
		/// </summary>
		/// <param name="MuteState">Desired Mute state</param>
		public void Mute(bool MuteState)
		{
			RenderingControl.SetMute((UInt32)RCid,CpRenderingControl.Enum_A_ARG_TYPE_Channel.MASTER,MuteState);
		}

		public void Mute(bool MuteState, string channel)
		{
			RenderingControl.SetUnspecifiedValue("Enum_A_ARG_TYPE_Channel",channel);
			RenderingControl.SetMute((UInt32)RCid,CpRenderingControl.Enum_A_ARG_TYPE_Channel._UNSPECIFIED_,MuteState);			
		}
		public bool GetMute(string channel)
		{
			return(RC_LastChange.GetMute(channel));
		}

		/// <summary>
		/// An array of strings representing the implemented channels
		/// </summary>
		public string[] SupportedChannels
		{
			get
			{
				return(RenderingControl.Values_A_ARG_TYPE_Channel);
			}
		}

		/// <summary>
		/// Get the current volume for the specified channel
		/// <para>
		/// The volume level is abstracted to an integer representing the volume as a percentage from 0 to 100
		/// </para>
		/// </summary>
		/// <param name="channel">Select Channel</param>
		/// <returns>Current Volume</returns>
		public UInt16 GetVolume(string channel)
		{
			UInt16 _Vol = RC_LastChange.GetVolume(channel);
			double VolP = ((double)_Vol-(double)MinVolume)/(double)(MaxVolume-MinVolume);
			UInt16 NewVolume = (UInt16)(VolP*(double)100);
			return(NewVolume);
		}
		/// <summary>
		/// Set the volume for the specified channel
		/// <para>
		/// The volume level is abstracted to an integer from 0 to 100 representing the percentage of maximum volume.
		/// </para>
		/// </summary>
		/// <param name="channel">DesiredChannel</param>
		/// <param name="DesiredVolume">Volume Level</param>
		public void SetVolume(string channel, UInt16 DesiredVolume)
		{
			double p = ((double)DesiredVolume)/((double)100);
			UInt16 R = (UInt16)(((double)(MaxVolume-MinVolume))*p);
			UInt16 NewVolume = (UInt16)((int)R + (int)MinVolume);
			RenderingControl.SetUnspecifiedValue("Enum_A_ARG_TYPE_Channel",channel);
			RenderingControl.SetVolume((UInt32)RCid,CpRenderingControl.Enum_A_ARG_TYPE_Channel._UNSPECIFIED_,NewVolume);
		}

		/// <summary>
		/// Change the PlayMode on the device
		/// </summary>
		/// <param name="NewMode">Desired Mode</param>
		public void SetPlayMode(PlayMode NewMode)
		{
			CpAVTransport.Enum_CurrentPlayMode RetVal = CpAVTransport.Enum_CurrentPlayMode._UNSPECIFIED_;
			AVTransport.SetUnspecifiedValue("Enum_CurrentPlayMode",NewMode.ToString());
			AVTransport.SetPlayMode((UInt32)this.AVTid,RetVal);
		}

		/// <summary>
		/// Get/Set the MasterVolume level
		/// <para>
		/// The volume level is abstracted to an integer from 0 to 100
		/// </para>
		/// </summary>
		public UInt16 MasterVolume
		{
			get
			{
				UInt16 _Vol = RC_LastChange.GetVolume("Master");
				double VolP = ((double)_Vol - (double)MinVolume)/(double)(MaxVolume-MinVolume);
				UInt16 NewVolume = (UInt16)(VolP*(double)100);
				return(NewVolume);
			}
			set
			{
				double p = ((double)value)/((double)100);
				UInt16 R = (UInt16)(((double)(MaxVolume-MinVolume))*p);
				UInt16 NewVolume = (UInt16)((int)R + (int)MinVolume);
				RenderingControl.SetVolume((UInt32)RCid,CpRenderingControl.Enum_A_ARG_TYPE_Channel.MASTER,NewVolume);
			}
		}
		/// <summary>
		/// Skip to the next track
		/// </summary>
		public void NextTrack()
		{
			AVTransport.Next((UInt32)AVTid);
		}
		/// <summary>
		/// Skip to the previous track
		/// </summary>
		public void PreviousTrack()
		{
			AVTransport.Previous((UInt32)AVTid);
		}

		internal void SetAVTransportURI(string NewUri, string MetaData)
		{
			SetAVTransportURI(NewUri,MetaData, null, null);
		}
		internal void SetAVTransportURI(string NewUri, string MetaData, object Tag, CpAVTransport.Delegate_OnResult_SetAVTransportURI Callback)
		{
			AVTransport.SetAVTransportURI((UInt32)AVTid,NewUri,MetaData,Tag, Callback);
			//AVTransport.Sync_SetAVTransportURI((UInt32)AVTid, NewUri);
		}
		protected void SetAVTransportURISink(CpAVTransport sender, System.UInt32 InstanceID, System.String CurrentURI, System.String CurrentURIMetaData, UPnPInvokeException e, object Tag)
		{
			if(e!=null) return;

			OnSetAVTransportURIEvent.Fire(this,Tag);

		}
		/// <summary>
		/// bool indicating the current mute state
		/// </summary>
		public bool IsMute
		{
			get
			{
				return(RC_LastChange.Mute);
			}
		}

		public string TransportStatus
		{
			get
			{
				return(AV_LastChange.TransportStatus);
			}
		}

		/// <summary>
		/// Property returns the currnet playmode on the device
		/// </summary>
		public AVConnection.PlayMode CurrentPlayMode
		{
			get
			{
				PlayMode RetVal = PlayMode.NORMAL;
				if(AV_LastChange==null) return(RetVal);
				switch(AV_LastChange.CurrentPlayMode)
				{
					case CpAVTransport.Enum_CurrentPlayMode.DIRECT_1:
						RetVal = PlayMode.DIRECT_1;
						break;
					case CpAVTransport.Enum_CurrentPlayMode.INTRO:
						RetVal = PlayMode.INTRO;
						break;
					case CpAVTransport.Enum_CurrentPlayMode.NORMAL:
						RetVal = PlayMode.NORMAL;
						break;
					case CpAVTransport.Enum_CurrentPlayMode.RANDOM:
						RetVal = PlayMode.RANDOM;
						break;
					case CpAVTransport.Enum_CurrentPlayMode.REPEAT_ALL:
						RetVal = PlayMode.REPEAT_ALL;
						break;
					case CpAVTransport.Enum_CurrentPlayMode.REPEAT_ONE:
						RetVal = PlayMode.REPEAT_ONE;
						break;
					case CpAVTransport.Enum_CurrentPlayMode.SHUFFLE:
						RetVal = PlayMode.SHUFFLE;
						break;
				}
				return(RetVal);
			}
		}
		/// <summary>
		/// This method is called by the AVRenderer object, from a single thread, that is
		/// used to poll AVTransport for position information.
		/// </summary>
		internal void _Poll()
		{
			this.AV_LastChange._Poll();
		}
		/// <summary>
		/// The current PlayState
		/// </summary>
		public AVConnection.PlayState CurrentState
		{
			get
			{
				PlayState RetVal = 0;
				if(AV_LastChange==null) return(PlayState.STOPPED);
				switch(AV_LastChange.PlayState)
				{
					case CpAVTransport.Enum_TransportState.STOPPED:
						RetVal = PlayState.STOPPED;
						break;
					case CpAVTransport.Enum_TransportState.PLAYING:
						RetVal = PlayState.PLAYING;
						break;
					case CpAVTransport.Enum_TransportState.PAUSED_PLAYBACK:
						RetVal = PlayState.PAUSED;
						break;
					case CpAVTransport.Enum_TransportState.PAUSED_RECORDING:
						RetVal = PlayState.PAUSED;
						break;
					case CpAVTransport.Enum_TransportState.RECORDING:
						RetVal = PlayState.RECORDING;
						break;
					case CpAVTransport.Enum_TransportState.TRANSITIONING:
						RetVal = PlayState.TRANSITIONING;
						break;
				}
				return(RetVal);
			}
		}
	}
}
