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
using System.Threading;
using System.Collections;
using OpenSource.UPnP;
using OpenSource.UPnP.AV;
using OpenSource.Utilities;
using OpenSource.UPnP.AV.CdsMetadata;

namespace OpenSource.UPnP.AV.RENDERER.CP
{

	/// <summary>
	/// AVRenderer Abstraction. 
	/// <para>
	/// This is the container class that is used to interact with a Renderer at the physical level.
	/// This object exposes all the events and methods necessary to manage rendering sessions, otherwise known as <see cref="OpenSource.UPnP.AVRENDERERSTACK.AVConnection"/>
	/// To instansiate a new session, simply invoke <see cref="OpenSource.UPnP.AVRENDERERSTACK.AVRenderer.CreateConnection"/> and pass in IMediaResource(s) or IMediaItem(s). The
	/// abstraction layer will automatically handle play list generation based on the capabilities of the renderer, and
	/// will automatically determine which MediaItems are renderable on the device based on renderer capabilities.
	/// </para>
	/// <para>
	/// The main points of interest in this object are the events you should subscribe to, when managing/controlling a renderer
	/// <list type="bullet">
	/// <item>
	/// <term><see cref="OpenSource.UPnP.AVRENDERERSTACK.AVRenderer.OnCreateConnection"/></term>
	/// <description>New Connection was created</description>
	/// </item>
	/// <item>
	/// <term><see cref="OpenSource.UPnP.AVRENDERERSTACK.AVRenderer.OnRecycledConnection"/></term>
	/// <description>Connection was reused</description>
	/// </item>
	/// <term><see cref="OpenSource.UPnP.AVRENDERERSTACK.AVRenderer.OnRemovedConnection"/></term>
	/// <description>Connection was deleted</description>
	/// <item>
	/// </item>
	/// </list>
	/// </para>
	/// </summary>
	public class AVRenderer
	{
		private Hashtable PlayListTable = Hashtable.Synchronized(new Hashtable());
		private object CreateConnectionLock = new object();
		private int PendingCreateConnection = 0;

		private LifeTimeMonitor ConnectionMonitor = new LifeTimeMonitor();


		public delegate void EventRenewalFailureHandler(AVRenderer sender);
		private WeakEvent EventRenewalFailureEvent = new WeakEvent();
		public event EventRenewalFailureHandler OnEventRenewalFailure
		{
			add
			{
				EventRenewalFailureEvent.Register(value);
			}
			remove
			{
				EventRenewalFailureEvent.UnRegister(value);
			}
		}


		protected bool DontEverDelete = false;
		/// <summary>
		/// The reasons the AVRenderer will report that a CreateConnection attempt failed.
		/// </summary>
		public enum CreateFailedReason
		{
			/// <summary>
			/// This means that the MediaType(s) supported by the renderer do not match that of the given IMediaResource
			/// </summary>
			UNSUPPORTED_MEDIA_ITEM,
			/// <summary>
			/// This has many possible reasons. Some of which include an error in handling the request on the server side, or not enough 
			/// resources being available on the server, or too many connections on the server, etc.
			/// </summary>
			CREATE_ATTEMPT_DENIED,
			SetAVTransport_FAILED,
		}

		public delegate void OnInitializedHandler(AVRenderer sender);
		public event OnInitializedHandler OnInitialized;
		private bool __Init = false;

		public bool IsInit
		{
			get
			{
				return(__Init);
			}
		}
		public delegate void FailedConnectionHandler(AVRenderer sender, CreateFailedReason reason, object Tag);
		private WeakEvent OnCreateConnectionFailedEvent = new WeakEvent();
		private WeakEvent OnCreateConnectionFailedEvent2 = new WeakEvent();
		/// <summary>
		/// This will be fired when a CreateConnection attempt failed
		/// </summary>
		public event FailedConnectionHandler OnCreateConnectionFailed
		{
			add
			{
				OnCreateConnectionFailedEvent.Register(value);
			}
			remove
			{
				OnCreateConnectionFailedEvent.UnRegister(value);
			}
		}
		/// <summary>
		/// This event is used exclusively by the AVPlayList class. It is triggered the same
		/// as the other OnCreateConnection event, except this one also triggers
		/// when PrepareForConnection completes
		/// </summary>
		internal event FailedConnectionHandler OnCreateConnectionFailed2
		{
			add
			{
				OnCreateConnectionFailedEvent2.Register(value);
			}
			remove
			{
				OnCreateConnectionFailedEvent2.UnRegister(value);
			}
		}

		public delegate void ConnectionHandler(AVRenderer sender, AVConnection r, object Tag);
		private WeakEvent OnCreateConnectionEvent = new WeakEvent();
		private WeakEvent OnCreateConnectionEvent2 = new WeakEvent();
		/// <summary>
		/// This will be fired when a CreateConnection attempt resulted in a new Session
		/// </summary>
		public event ConnectionHandler OnCreateConnection
		{
			add
			{
				OnCreateConnectionEvent.Register(value);
			}
			remove
			{
				OnCreateConnectionEvent.UnRegister(value);
			}
		}
		/// <summary>
		/// This event is used exclusively by the AVPlayList class. It is triggered the same
		/// as the other OnCreateConnection event, except this one also triggers
		/// when PrepareForConnection completes
		/// </summary>
		internal event ConnectionHandler OnCreateConnection2
		{
			add
			{
				OnCreateConnectionEvent2.Register(value);
			}
			remove
			{
				OnCreateConnectionEvent2.UnRegister(value);
			}
		}
		
		private WeakEvent OnRecycledConnectionEvent = new WeakEvent();
		private WeakEvent OnRecycledConnectionEvent2 = new WeakEvent();
		/// <summary>
		/// This will be fired when a CreateConnection attempt resulted in the reuse of an existing connection.
		/// </summary>
		public event ConnectionHandler OnRecycledConnection
		{
			add
			{
				OnRecycledConnectionEvent.Register(value);
			}
			remove
			{
				OnRecycledConnectionEvent.UnRegister(value);
			}
		}
		/// <summary>
		/// This event is used exclusively by the AVPlayList class. It is triggered the same
		/// as the other OnRecycledConnection event, except this one also triggers
		/// when PrepareForConnection completes
		/// </summary>
		internal event ConnectionHandler OnRecycledConnection2
		{
			add
			{
				OnRecycledConnectionEvent2.Register(value);
			}
			remove
			{
				OnRecycledConnectionEvent2.UnRegister(value);
			}
		}

		private WeakEvent OnRemovedConnectionEvent = new WeakEvent();
		/// <summary>
		/// This will be fired when a connection has closed
		/// </summary>
		public event ConnectionHandler OnRemovedConnection
		{
			add
			{
				OnRemovedConnectionEvent.Register(value);
			}
			remove
			{
				OnRemovedConnectionEvent.UnRegister(value);
			}
		}

		internal CpConnectionManager ConnectionManager;
		internal UPnPDevice MainDevice;

		protected ArrayList InstanceList = new ArrayList();
		protected ArrayList ProtocolInfoList = new ArrayList();
		protected Hashtable HandleTable = Hashtable.Synchronized(new Hashtable());
		/// <summary>
		/// This returns the UPnPDevice object associated with this renderer
		/// </summary>
		public UPnPDevice device
		{
			get
			{
				return(MainDevice);
			}
		}

		/// <summary>
		/// This constructor is called with the UPnPDevice that contains the services of a MediaRenderer device.
		/// </summary>
		/// <param name="device">The UPnPDevice</param>
		public AVRenderer(UPnPDevice device)
		{
			OpenSource.Utilities.InstanceTracker.Add(this);
			this.ConnectionMonitor.OnExpired += new LifeTimeMonitor.LifeTimeHandler(ConnectionMonitorSink);

			MainDevice = device;
			ConnectionManager = new CpConnectionManager(device.GetServices(CpConnectionManager.SERVICE_NAME)[0]);
			ConnectionManager.OnStateVariable_CurrentConnectionIDs += new CpConnectionManager.StateVariableModifiedHandler_CurrentConnectionIDs(ConnectionIDEventSink);
			ConnectionManager._subscribe(90);
            //TODO: Fails to compile after using generated code from DeviceBuilderV23. Seems like CpConnectionManager.PeriodicRenewFailedHandler is no longer defined?
			//ConnectionManager.OnPeriodicRenewFailed += new CpConnectionManager.PeriodicRenewFailedHandler(PeriodicRenewFailedSink);

			// Grab initial state of the ConnectionManager Service
			if(ConnectionManager.HasAction_GetProtocolInfo)
			{
				ConnectionManager.GetProtocolInfo(null,new CpConnectionManager.Delegate_OnResult_GetProtocolInfo(GetProtocolInfoSink));
			}
			if(ConnectionManager.HasAction_GetCurrentConnectionIDs) 
			{
				ConnectionManager.GetCurrentConnectionIDs(null,new CpConnectionManager.Delegate_OnResult_GetCurrentConnectionIDs(IDSink));
			}
			if(ConnectionManager.HasAction_PrepareForConnection==false)
			{
				lock(InstanceList)
				{
					AVConnection ac = new AVConnection(MainDevice,0,0,0,new AVConnection.OnReadyHandler(ReadySink), null);
					ac._Parent = this;
					DontEverDelete = true;
					if(InstanceList.Count==0) InstanceList.Add(ac);
				}
				/*  Wait for Ready
				if(InstanceList.Count>0)
				{
					if(OnCreateConnection!=null) OnCreateConnection(this,(AVConnection)InstanceList[0],Guid.NewGuid().GetHashCode());
				}
				*/
			}
		}

		protected void PeriodicRenewFailedSink(CpConnectionManager sender)
		{
			EventRenewalFailureEvent.Fire(this);
		}

		public void ReSync()
		{
			if(ConnectionManager.HasAction_GetCurrentConnectionIDs) 
			{
				ConnectionManager.GetCurrentConnectionIDs(null,new CpConnectionManager.Delegate_OnResult_GetCurrentConnectionIDs(IDSink));
			}
		}

		/// <summary>
		/// This method can be triggered 30 seconds after an event was received
		/// notifying us what ConnectionIDs are valid. There could be a 30 seconds delay, because
		/// if there are pending connection attempts, this event may not contain the complete ID list,
		/// so we need to wait for PrepareForConnection to return. Or vice versa. If PrepareForConnection
		/// returns with its ID, we need to make sure the event arrived, because otherwise we
		/// could receive an event without that connection id, and the CP may think that the ID we just added
		/// was deleted, which is bad... Very bad race conditions here :)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="obj"></param>
		private void ConnectionMonitorSink(LifeTimeMonitor sender, object obj)
		{
			// Lets fetch information about the connection id specicifed in obj
			ConnectionManager.GetCurrentConnectionInfo((int)obj,null,new CpConnectionManager.Delegate_OnResult_GetCurrentConnectionInfo(ConnectionInfoSink));				
		}

		/// <summary>
		/// This returns a debugging object for the ConnectionManager
		/// </summary>
		public UPnPServiceWatcher ConnectionManagerWatcher
		{
			get
			{
				return(new UPnPServiceWatcher(MainDevice.GetServices(CpConnectionManager.SERVICE_NAME)[0],null));
			}
		}
		/// <summary>
		/// This returns a debugging object for the AVTransport
		/// </summary>
		public UPnPServiceWatcher AVTransportWatcher
		{
			get
			{
				return(new UPnPServiceWatcher(MainDevice.GetServices(CpAVTransport.SERVICE_NAME)[0],null));
			}
		}
		/// <summary>
		/// This returns a debugging object for the RenderingControl
		/// </summary>
		public UPnPServiceWatcher RenderingControlWatcher
		{
			get
			{
				return(new UPnPServiceWatcher(MainDevice.GetServices(CpRenderingControl.SERVICE_NAME)[0],null));
			}
		}

		/// <summary>
		/// This method is triggered whenever a ConnectionManagerEvent arrives, which tells us
		/// all of the current ConnectionIDs
		/// </summary>
		/// <param name="sender">The CpConnectionManager class that sent the event</param>
		/// <param name="CurrentIDs">A CSV list of ConnectionIDs</param>
		protected void ConnectionIDEventSink(CpConnectionManager sender, string CurrentIDs)
		{
			// We need to return immediately if this flag is set.
			// This flag is only set if PrepareForConnection in not implemented on this
			// renderer, in which case, there will be a default ConnectionID of 0, which
			// must never disappear.
			if(DontEverDelete == true) return;

			// This is a temp collection used to create an index of the ConnectionIDs that
			// were recieved in this event
			Hashtable h = new Hashtable();
			// This is a temp parser used to parse the CSV list of IDs
			DText p = new DText();
			p.ATTRMARK = ",";

			if(CurrentIDs!="")
			{
				p[0] = CurrentIDs;
				int len = p.DCOUNT();
				for(int i=1;i<=len;++i)
				{
					// Adding a ConnectionID into the temp collection
					h[Int32.Parse(p[i])] = "";
				}
			}

			// Lets find ones that were removed first
			foreach(AVConnection a in Connections)
			{
				if(h.ContainsKey(a.ConnectionID)==false)
				{
					// This ID was removed
					InstanceList.Remove(a);
					a.Dispose();
					OnRemovedConnectionEvent.Fire(this,a,Guid.NewGuid().GetHashCode());	
				}
			}

			// Now lets look for new ones... This is easy
			IDSink(sender, CurrentIDs,null,Guid.NewGuid().GetHashCode());
		}


		/// <summary>
		/// This returns the supported Protocols for the Renderer
		/// </summary>
		public ProtocolInfoString[] ProtocolInfoStrings
		{
			get
			{
				return((ProtocolInfoString[])ProtocolInfoList.ToArray(typeof(ProtocolInfoString)));
			}
		}

		/// <summary>
		/// This method is called when an AsyncCall to GetProtocolInfo completes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="Source"></param>
		/// <param name="Sink"></param>
		/// <param name="e"></param>
		/// <param name="Handle"></param>
		protected void GetProtocolInfoSink(CpConnectionManager sender, System.String Source, System.String Sink, UPnPInvokeException e, object Handle)
		{
			if(e!=null) return;
			if(Sink=="") return;

			// This is a temp parser to parse the supported ProtocolInfo values
			DText p = new DText();
			p.ATTRMARK = ",";
			p[0] = Sink;
			int len = p.DCOUNT();
			ProtocolInfoString istring;
			for(int i=1;i<=len;++i)
			{
				istring = new ProtocolInfoString(p[i]);
				// Add each individual entry
				ProtocolInfoList.Add(istring);
			}
			if(!this.__Init)
			{
				this.__Init = true;
				// Upon discovery of a renderer, we can't return the renderer to the user
				// until we at least parsed the supported ProtocolInfo values, otherwise
				// the user will experience incorrect behavior. Since we have just parsed
				// these values, we can fire this event.
				if(this.OnInitialized!=null) OnInitialized(this);
			}
		}

		/// <summary>
		/// This is used to determine if the renderer supports a given protocol type.
		/// </summary>
		/// <param name="ProtocolInfo">The ProtocolInfo you wish to check</param>
		/// <returns>true if supported</returns>
		public bool SupportsProtocolInfo(ProtocolInfoString ProtocolInfo)
		{
			bool RetVal = false;
			foreach(ProtocolInfoString pis in ProtocolInfoList)
			{
				if(pis.Matches(ProtocolInfo))
				{
					RetVal = true;
					break;
				}
			}
			return(RetVal);
		}

		/*
		public SortedList SortResourcesByAddress(IMediaResource[] resources)
		{
			long matchValue;
			SortedList retVal = new SortedList();

			// Iterate through all resources and sort them according 
			// to a bitwise-AND comparison of the IP address for the 
			// resource's URI and the target renderer's IP address.

			foreach (IMediaResource res in resources)
			{
				try
				{
					Uri uri = new Uri(res.ContentUri);
					if (uri.HostNameType == UriHostNameType.IPv4)
					{
						System.Net.IPAddress ipaddr = System.Net.IPAddress.Parse(uri.Host);
						matchValue = this.MainDevice.RemoteEndPoint.Address.Address & ipaddr.Address;
						retVal.Add(matchValue, res);
					}
				}
				catch
				{
					retVal.Add(0, res);
				}
			}

			return retVal;
		}

		public SortedList SortResourcesByProtocolInfo(IMediaResource resources)
		{
			long matchValue;
			SortedList retVal = new SortedList();

			// Iterate through all resources and sort them according 
			// to a bitwise-AND comparison of the IP address for the 
			// resource's URI and the target renderer's IP address.

			foreach (IMediaResource res in resources)
			{
				matchValue = this.ProtocolInfoList.Count + 1;
				foreach (ProtocolInfoString pis in this.ProtocolInfoList)
				{
					matchValue--;

					if (pis.Matches(res.ProtocolInfo))
					{
						retVal.Add(matchValue, res);
					}
				}
			}

			return retVal;
		}
		*/

		public IMediaResource GetBestMatch(IMediaResource[] resources)
		{
			long bestIpMatch = 0xFFFFFFFF;
			int bestProtInfoMatch = 0;
			long reverseThis;
			System.Net.IPAddress ipaddr;

			IMediaResource bestMatch = null;
			foreach (IMediaResource res in resources)
			{
				long ipMatch = 0;
				int piMatch = 0;

				piMatch = this.ProtocolInfoList.Count + 1;
				foreach(ProtocolInfoString pis in this.ProtocolInfoList)
				{
					piMatch--;
					if(pis.Matches(res.ProtocolInfo)) { break; }
				}

				if (piMatch > 0)
				{
					ipMatch = 0;
					try
					{
						Uri uri = new Uri(res.ContentUri);
						if (uri.HostNameType == UriHostNameType.IPv4)
						{
							ipaddr = System.Net.IPAddress.Parse(uri.Host);

							reverseThis = (this.MainDevice.RemoteEndPoint.Address.Address ^ ipaddr.Address);
							ipMatch = 
								((reverseThis & 0x000000ff) << 24) |
								((reverseThis & 0x0000ff00) << 8) |
								((reverseThis & 0x00ff0000) >> 8) |
								((reverseThis & 0xff000000) >> 24);
						}
					}
					catch
					{
						ipMatch = 0xFFFFFFFF;
					}
				}

				if (
					(ipMatch < bestIpMatch) ||
					((ipMatch == bestIpMatch) && (piMatch > bestProtInfoMatch))
					)
				{
					bestMatch = res;
					bestIpMatch = ipMatch;
					bestProtInfoMatch = piMatch;
				}
			}

			return bestMatch;
		}

		/// <summary>
		/// This returns the interface on which this Control Point uses to reach the renderer
		/// </summary>
		public System.Net.IPAddress Interface
		{
			get
			{
				return(MainDevice.InterfaceToHost);
			}
		}

		/// <summary>
		/// This property lets the user know if this renderer has implemented the
		/// PrepareForConnection method in the ConnectionManager service.
		/// </summary>
		public bool HasConnectionHandling
		{
			get
			{
				return(this.ConnectionManager.HasAction_PrepareForConnection);
			}
		}
		/// <summary>
		/// The UPnP Friendly name of the Renderer
		/// </summary>
		public string FriendlyName
		{
			get
			{
				return(MainDevice.FriendlyName);
			}
		}
		/// <summary>
		/// The UPnP UniqueDeviceName of the renderer
		/// </summary>
		public string UniqueDeviceName
		{
			get
			{
				return(MainDevice.UniqueDeviceName);
			}
		}

		/// <summary>
		/// This method is called when an Async call to PrepareForConnection returns. Only
		/// the AVPlayList class will ever call that method.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="RemoteProtocolInfo"></param>
		/// <param name="PeerConnectionManager"></param>
		/// <param name="PeerConnectionID"></param>
		/// <param name="Direction"></param>
		/// <param name="ConnectionID"></param>
		/// <param name="AVTransportID"></param>
		/// <param name="RcsID"></param>
		/// <param name="e"></param>
		/// <param name="Handle"></param>
		protected void PrepareForConnectionSink(CpConnectionManager sender, System.String RemoteProtocolInfo, System.String PeerConnectionManager, System.Int32 PeerConnectionID, CpConnectionManager.Enum_A_ARG_TYPE_Direction Direction, System.Int32 ConnectionID, System.Int32 AVTransportID, System.Int32 RcsID, UPnPInvokeException e, object Handle)
		{
			AVConnection c = null;
			bool IsNew = true;

			if(e!=null)
			{
				// Since only the AVPlayList class will call PrepareForConnection, we need to fire
				// this other event, because it's too early to notify the user. Only AVPlayList needs
				// to know about this, so it can continue to setup the connection for the user
				OnCreateConnectionFailedEvent2.Fire(this,AVRenderer.CreateFailedReason.CREATE_ATTEMPT_DENIED,Handle);
				return;
			}

			lock(InstanceList)
			{
				foreach(AVConnection a in InstanceList)
				{
					if(a.ConnectionID==ConnectionID)
					{
						// We Already Have this ID From somewhere
						IsNew = false;
						c = a;
						break;
					}
				}
				if(IsNew==true)
				{
					// Does not Exist
					c = new AVConnection(MainDevice, AVTransportID, RcsID, ConnectionID,new AVConnection.OnReadyHandler(_ReadySink), Handle);
					c._Parent = this;
					InstanceList.Add(c);
				}
			}

			if(IsNew==true)
			{
				// Wait for Ready event from the AVConnection, since we can't return it
				// until it is initialized.
			}
			else
			{
				// Recycled
				OnRecycledConnectionEvent2.Fire(this,c,Handle);
			}
		}

		protected void CreateConnectionEx(object _state)
		{
			object[] state = (object[])_state;

			string ConnectionInfoString = (string)state[0];
			string PeerConnectionManagerString = (string)state[1];
			int Handle = (int)state[2];

			OnRecycledConnectionEvent2.Fire(this,(AVConnection)this.InstanceList[0],Handle);
		}

		/// <summary>
		/// This method is used to call the PrepareForConnection method on the renderer
		/// if it is implemented. Only AVPlayList should call this method, which is why
		/// this is an internal method.
		/// </summary>
		/// <param name="ConnectionInfoString"></param>
		/// <param name="PeerConnectionManagerString"></param>
		/// <param name="PeerConnectionID"></param>
		internal void CreateConnection(string ConnectionInfoString, string PeerConnectionManagerString, int PeerConnectionID)
		{
			CreateConnection(ConnectionInfoString,PeerConnectionManagerString,PeerConnectionID,null);
		}
		/// <summary>
		/// This is invoked to instantiate a new rendering session. Same as other internal CreateConnection
		/// method, but this one has an extra user object that can be passed. The other method will
		/// actually call this one, and pass null.
		/// </summary>
		/// <param name="ConnectionInfoString">The ProtocolInfo for the session you wish to begin</param>
		/// <param name="PeerConnectionManagerString">Information about the source of the new stream. Forward slash if none.</param>
		/// <param name="PeerConnectionID">The ID of the source. -1 if none</param>
		/// <returns>Unique Handle</returns>
		internal void CreateConnection(string ConnectionInfoString, string PeerConnectionManagerString, int PeerConnectionID, object Tag)
		{
			// If this renderer implements PrepareForConnection we must do some more
			// processing.
			if(ConnectionManager.HasAction_PrepareForConnection)
			{
					ConnectionManager.PrepareForConnection(ConnectionInfoString,
					PeerConnectionManagerString,
					PeerConnectionID,
					CpConnectionManager.Enum_A_ARG_TYPE_Direction.INPUT,
					Tag,
					new CpConnectionManager.Delegate_OnResult_PrepareForConnection(PrepareForConnectionSink));
					return;
			}

			// If this renderer does not implement PrepareForConnection, then we just notify
			// that we are recycling the ConnectionID that we are already using
			OnRecycledConnectionEvent2.Fire(this,(AVConnection)this.InstanceList[0],Tag);
		}

		/// <summary>
		/// This method just checks a MediaContainer, and lets the user know
		/// if this renderer is capable of renderering it. Note, that compatible
		/// in the sense of how the AV spec defines compatible. This does NOT
		/// gaurantee that it will actually be able to be renderered on the device.
		/// </summary>
		/// <param name="Container"></param>
		/// <returns></returns>
		public bool IsCompatible(IMediaContainer Container)
		{
			foreach(IMediaItem item in Container.Items)
			{
				foreach(IMediaResource r in item.MergedResources)
				{
					if(SupportsProtocolInfo(r.ProtocolInfo)) return(true);
				}
			}
			return(false);
		}
		/// <summary>
		/// Create a new Session on the renderer to play this IMediaContainer
		/// </summary>
		/// <param name="Container">Container to render</param>
		public void CreateConnection(IMediaContainer Container)
		{
			CreateConnection(Container,null);
		}
		public void CreateConnection(IMediaContainer Container, object Tag)
		{
			if (Container.MergedResources.Length > 0)
			{
				CreateConnection(this.GetBestMatch(Container.MergedResources));
			}
			else
			{
				ArrayList RList = new ArrayList();
				foreach(IMediaItem item in Container.Items)
				{
					foreach(IMediaResource r in item.MergedResources)
					{
						if(SupportsProtocolInfo(r.ProtocolInfo))
						{
							RList.Add(r);
							break;
						}
					}
				}

				if(RList.Count==0)
				{
					OnCreateConnectionFailedEvent.Fire(this,CreateFailedReason.UNSUPPORTED_MEDIA_ITEM,Tag);
					return;
				}

				CreateConnection((IMediaResource[])RList.ToArray(typeof(IMediaResource)),Tag);
			}
		}

		/// <summary>
		/// This method just checks a MediaItem, and lets the user know
		/// if this renderer is capable of renderering it. Note, that compatible
		/// in the sense of how the AV spec defines compatible. This does NOT
		/// gaurantee that it will actually be able to be renderered on the device.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool IsCompatible(IMediaItem item)
		{
			return(IsCompatible(new IMediaItem[1]{item}));
		}

		/// <summary>
		/// Creates a new session on the renderer to play this IMediaItem
		/// </summary>
		/// <param name="item"></param>
		public void CreateConnection(IMediaItem item)
		{
			CreateConnection(item,null);
		}
		/// <summary>
		/// Creates a new session on the renderer to play this array of IMediaItems.
		/// This stack will take care of figuring out how to do it, whether it involves
		/// creating a playlist, what type of playlist, or to fake a playlist.
		/// </summary>
		/// <param name="items">Array of items to play</param>
		public void CreateConnection(IMediaItem[] items)
		{
			CreateConnection(items,null);
		}
		public void CreateConnection(IMediaItem item, object Tag)
		{
			CreateConnection(new IMediaItem[1]{item},Tag);
		}
		public void CreateConnection(IMediaItem[] items, object Tag)
		{
			ArrayList RList = new ArrayList();

			foreach(IMediaItem item in items)
			{
				IMediaResource res = GetBestMatch(item.MergedResources);
				if (res != null) RList.Add(res);
//				foreach(IMediaResource resource in item.MergedResources)
//				{
//					if(this.SupportsProtocolInfo(resource.ProtocolInfo))
//					{
//						// Use this resource
//						RList.Add(resource);
//						break;
//					}
//				}
			}

			if(RList.Count==0)
			{
				OnCreateConnectionFailedEvent.Fire(this,CreateFailedReason.UNSUPPORTED_MEDIA_ITEM,Tag);
				return;
			}
			CreateConnection((IMediaResource[])RList.ToArray(typeof(IMediaResource)),Tag);
		}
		/// <summary>
		/// This lets the user verify that at least on of the items specified can be
		/// rendered on the device. NOTE that this is according to the AV spec and its use
		/// of the ProtocolInfo value field. This does NOT gaurantee that this can actually
		/// be played on the renderer.
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		public bool IsCompatible(IMediaItem[] items)
		{
			foreach(IMediaItem item in items)
			{
				foreach(IMediaResource r in item.MergedResources)
				{
					if(SupportsProtocolInfo(r.ProtocolInfo)) return(true);
				}
			}
			return(false);
		}

		/// <summary>
		/// This lets the user verify that at least on of the resources specified can be
		/// rendered on the device. NOTE that this is according to the AV spec and its use
		/// of the ProtocolInfo value field. This does NOT gaurantee that this can actually
		/// be played on the renderer.
		/// </summary>
		/// <param name="resources"></param>
		/// <returns></returns>
		public bool IsCompatible(IMediaResource[] resources)
		{
			foreach(IMediaResource r in resources)
			{
				if(SupportsProtocolInfo(r.ProtocolInfo)) return(true);
			}
			return(false);
		}

		/// <summary>
		/// Creates a new session on the renderer to play this array of resources.
		/// This stack will take care of figuring out how to do it, whether it involves
		/// creating a playlist, what type of playlist, or to fake a playlist.
		/// </summary>
		/// <param name="resources"></param>
		public void CreateConnection(IMediaResource[] resources)
		{
			CreateConnection(resources,null);
		}
		/// <summary>
		/// This is invoked when you wish to instantiate a new rendering session.
		/// </summary>
		/// <param name="resources">the IMediaResource(s) you wish to render</param>
		/// <returns>Unique Handle</returns>
		public void CreateConnection(IMediaResource[] resources, object Tag)
		{
			lock(this.CreateConnectionLock)
			{
				++this.PendingCreateConnection;
			}
			AVPlayList pl = new AVPlayList(this,resources,new AVPlayList.ReadyHandler(PlayListSink), new AVPlayList.FailedHandler(PlayListFailedSink),Tag);
			PlayListTable[pl.GetHashCode()] = pl;
		}

		protected void PlayListFailedSink(AVPlayList sender, AVRenderer.CreateFailedReason reason)
		{
			PlayListTable.Remove(sender.GetHashCode());
			OnCreateConnectionFailedEvent.Fire(this,reason,sender.PlayListHandle);
		}
		protected void PlayListSink(AVPlayList sender, AVConnection c, object Tag)
		{
			PlayListTable.Remove(sender.GetHashCode());
			lock(this.CreateConnectionLock)
			{
				--this.PendingCreateConnection;
				if(this.PendingCreateConnection<0) PendingCreateConnection = 0;
				ConnectionMonitor.Remove(c.ConnectionID);
			}

			if(sender.IsRecycled)
			{
				OnRecycledConnectionEvent.Fire(this,c,Tag);
			}
			else
			{
				OnCreateConnectionEvent.Fire(this,c,Tag);
			}
		}
		public void CreateConnection(IMediaResource media)
		{
			CreateConnection(media,null);
		}
		public void CreateConnection(IMediaResource media, object Tag)
		{
			CreateConnection(new IMediaResource[1]{media}, Tag);
		}

		/// <summary>
		/// This returns an IList of the current AVConnections
		/// </summary>
		public IList Connections
		{
			get
			{
				return((IList)InstanceList.Clone());
			}
		}
		/// <summary>
		/// This is called by the discovery object when the render device disappeared
		/// off the network.
		/// </summary>
		internal void Removed()
		{
			foreach(AVConnection c in this.Connections)
			{
				c.Dispose();
			}
			this.ConnectionManager.Dispose();
		}
		/// <summary>
		/// This method is called whenever we need to inspect a list of ConnectionIDs
		/// to see if any of them are new
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="IDString"></param>
		/// <param name="e"></param>
		/// <param name="Handle"></param>
		protected void IDSink(CpConnectionManager sender, string IDString, UPnPInvokeException e, object Handle)
		{
			if((e!=null) || (IDString=="")) return;

			// This is a temp parser that will parse the CSV list of IDs
			DText p = new DText();
			p.ATTRMARK = ",";
			p[0] = IDString;
			int len = p.DCOUNT();
			for(int i=1;i<=len;++i)
			{
//				bool FOUND = false;
				int cid = int.Parse(p[i].Trim());
				
//				lock(InstanceList)
//				{	
//					foreach(AVConnection _AVC in InstanceList)
//					{
//						if(_AVC.ConnectionID == cid)
//						{
//							FOUND = true;
//							break;
//						}
//					}
//				}

				lock(this.CreateConnectionLock)
				{
					if(this.PendingCreateConnection==0)
					{
						ConnectionManager.GetCurrentConnectionInfo(cid,null,new CpConnectionManager.Delegate_OnResult_GetCurrentConnectionInfo(ConnectionInfoSink));				
					}
					else
					{
						// We possible need to wait a maximum of 30 seconds, just in case events arrive
						// that involve this connection ID. When/if that happens, we will remove this 
						// object from the monitor, and continue directly.
						this.ConnectionMonitor.Add(cid,30);
					}
				}
			}
		}

		private void ReadySink(AVConnection sender, object Tag)
		{
			OnCreateConnectionEvent.Fire(this,sender,Tag);
		}
		private void _ReadySink(AVConnection sender, object Tag)
		{
			OnCreateConnectionEvent2.Fire(this,sender,Tag);
		}

		/// <summary>
		/// This method is called on a regular interval from a single thread. This is
		/// giving the renderer a thread to poll the device for position information.
		/// The AVConnection object will only poll if it is in the PLAY state, otherwise
		/// it will just return immediately.
		/// </summary>
		internal void _Poll()
		{
			foreach(AVConnection C in Connections)
			{
				C._Poll();
			}
		}

		protected void ConnectionInfoSink(CpConnectionManager sender, System.Int32 ConnectionID, System.Int32 RcsID, System.Int32 AVTransportID, System.String ProtocolInfo, System.String PeerConnectionManager, System.Int32 PeerConnectionID, CpConnectionManager.Enum_A_ARG_TYPE_Direction Direction, CpConnectionManager.Enum_A_ARG_TYPE_ConnectionStatus Status, UPnPInvokeException e, object Handle)
		{
			if(e!=null) return;

			AVConnection av=null;
			
			lock(InstanceList)
			{
				foreach(AVConnection a in InstanceList)
				{
					if(a.ConnectionID==ConnectionID)
					{
						av = a;
						break;
					}
				}
				if(av==null)
				{
					av = new AVConnection(MainDevice, AVTransportID, RcsID, ConnectionID,new AVConnection.OnReadyHandler(ReadySink), Handle);
					av._Parent = this;
					InstanceList.Add(av);
				}
				else
				{
					return; // Don't need to trigger event
				}
			}

			// Wait for Ready before sending OnCreateConnection
		}
	}
}
