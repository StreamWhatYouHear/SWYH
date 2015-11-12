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
using OpenSource.Utilities;

namespace OpenSource.UPnP.AV
{
    /// <summary>
    /// Transparent ClientSide UPnP Service
    /// </summary>
    public class CpConnectionManager
    {
       private Hashtable UnspecifiedTable = Hashtable.Synchronized(new Hashtable());
       internal UPnPService _S;

       public UPnPService GetUPnPService()
       {
            return(_S);
       }
       public static string SERVICE_NAME = "urn:schemas-upnp-org:service:ConnectionManager:";
       public double VERSION
       {
           get
           {
               return(double.Parse(_S.Version));
           }
       }

	   public delegate void PeriodicRenewFailedHandler(CpConnectionManager sender);
	   private WeakEvent OnPeriodicRenewFailedEvent = new WeakEvent();
		public event PeriodicRenewFailedHandler OnPeriodicRenewFailed
		{
			add
			{
				OnPeriodicRenewFailedEvent.Register(value);
			}
			remove
			{
				OnPeriodicRenewFailedEvent.UnRegister(value);
			}
		}

       public delegate void StateVariableModifiedHandler_SourceProtocolInfo(CpConnectionManager sender, System.String NewValue);
       private WeakEvent OnStateVariable_SourceProtocolInfo_Event = new WeakEvent();
       public event StateVariableModifiedHandler_SourceProtocolInfo OnStateVariable_SourceProtocolInfo
       {
			add{OnStateVariable_SourceProtocolInfo_Event.Register(value);}
			remove{OnStateVariable_SourceProtocolInfo_Event.UnRegister(value);}
       }
       protected void SourceProtocolInfo_ModifiedSink(UPnPStateVariable Var, object NewValue)
       {
            OnStateVariable_SourceProtocolInfo_Event.Fire(this, (System.String)NewValue);
       }
       public delegate void StateVariableModifiedHandler_SinkProtocolInfo(CpConnectionManager sender, System.String NewValue);
       private WeakEvent OnStateVariable_SinkProtocolInfo_Event = new WeakEvent();
       public event StateVariableModifiedHandler_SinkProtocolInfo OnStateVariable_SinkProtocolInfo
       {
			add{OnStateVariable_SinkProtocolInfo_Event.Register(value);}
			remove{OnStateVariable_SinkProtocolInfo_Event.UnRegister(value);}
       }
       protected void SinkProtocolInfo_ModifiedSink(UPnPStateVariable Var, object NewValue)
       {
            OnStateVariable_SinkProtocolInfo_Event.Fire(this, (System.String)NewValue);
       }
       public delegate void StateVariableModifiedHandler_CurrentConnectionIDs(CpConnectionManager sender, System.String NewValue);
       private WeakEvent OnStateVariable_CurrentConnectionIDs_Event = new WeakEvent();
       public event StateVariableModifiedHandler_CurrentConnectionIDs OnStateVariable_CurrentConnectionIDs
       {
			add{OnStateVariable_CurrentConnectionIDs_Event.Register(value);}
			remove{OnStateVariable_CurrentConnectionIDs_Event.UnRegister(value);}
       }
       protected void CurrentConnectionIDs_ModifiedSink(UPnPStateVariable Var, object NewValue)
       {
            OnStateVariable_CurrentConnectionIDs_Event.Fire(this, (System.String)NewValue);
       }
       public delegate void SubscribeHandler(CpConnectionManager sender, bool Success);
       public event SubscribeHandler OnSubscribe;
       public delegate void Delegate_OnResult_GetCurrentConnectionInfo(CpConnectionManager sender, System.Int32 ConnectionID, System.Int32 RcsID, System.Int32 AVTransportID, System.String ProtocolInfo, System.String PeerConnectionManager, System.Int32 PeerConnectionID, Enum_A_ARG_TYPE_Direction Direction, Enum_A_ARG_TYPE_ConnectionStatus Status, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_GetCurrentConnectionInfo_Event = new WeakEvent();
       public event Delegate_OnResult_GetCurrentConnectionInfo OnResult_GetCurrentConnectionInfo
       {
			add{OnResult_GetCurrentConnectionInfo_Event.Register(value);}
			remove{OnResult_GetCurrentConnectionInfo_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_PrepareForConnection(CpConnectionManager sender, System.String RemoteProtocolInfo, System.String PeerConnectionManager, System.Int32 PeerConnectionID, Enum_A_ARG_TYPE_Direction Direction, System.Int32 ConnectionID, System.Int32 AVTransportID, System.Int32 RcsID, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_PrepareForConnection_Event = new WeakEvent();
       public event Delegate_OnResult_PrepareForConnection OnResult_PrepareForConnection
       {
			add{OnResult_PrepareForConnection_Event.Register(value);}
			remove{OnResult_PrepareForConnection_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_ConnectionComplete(CpConnectionManager sender, System.Int32 ConnectionID, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_ConnectionComplete_Event = new WeakEvent();
       public event Delegate_OnResult_ConnectionComplete OnResult_ConnectionComplete
       {
			add{OnResult_ConnectionComplete_Event.Register(value);}
			remove{OnResult_ConnectionComplete_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_GetProtocolInfo(CpConnectionManager sender, System.String Source, System.String Sink, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_GetProtocolInfo_Event = new WeakEvent();
       public event Delegate_OnResult_GetProtocolInfo OnResult_GetProtocolInfo
       {
			add{OnResult_GetProtocolInfo_Event.Register(value);}
			remove{OnResult_GetProtocolInfo_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_GetCurrentConnectionIDs(CpConnectionManager sender, System.String ConnectionIDs, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_GetCurrentConnectionIDs_Event = new WeakEvent();
       public event Delegate_OnResult_GetCurrentConnectionIDs OnResult_GetCurrentConnectionIDs
       {
			add{OnResult_GetCurrentConnectionIDs_Event.Register(value);}
			remove{OnResult_GetCurrentConnectionIDs_Event.UnRegister(value);}
       }

        public CpConnectionManager(UPnPService s)
        {
            _S = s;
            _S.OnSubscribe += new UPnPService.UPnPEventSubscribeHandler(_subscribe_sink);
            _S.OnPeriodicRenewFailed += new UPnPService.PeriodicRenewFailedHandler(_periodicrenew_failed_sink);
			if(HasStateVariable_SourceProtocolInfo) _S.GetStateVariableObject("SourceProtocolInfo").OnModified += new UPnPStateVariable.ModifiedHandler(SourceProtocolInfo_ModifiedSink);
            if(HasStateVariable_SinkProtocolInfo) _S.GetStateVariableObject("SinkProtocolInfo").OnModified += new UPnPStateVariable.ModifiedHandler(SinkProtocolInfo_ModifiedSink);
            if(HasStateVariable_CurrentConnectionIDs) _S.GetStateVariableObject("CurrentConnectionIDs").OnModified += new UPnPStateVariable.ModifiedHandler(CurrentConnectionIDs_ModifiedSink);
        }
        public void Dispose()
        {
            _S.OnSubscribe -= new UPnPService.UPnPEventSubscribeHandler(_subscribe_sink);
            OnSubscribe = null;
            if(HasStateVariable_SourceProtocolInfo) _S.GetStateVariableObject("SourceProtocolInfo").OnModified -= new UPnPStateVariable.ModifiedHandler(SourceProtocolInfo_ModifiedSink);
            if(HasStateVariable_SinkProtocolInfo) _S.GetStateVariableObject("SinkProtocolInfo").OnModified -= new UPnPStateVariable.ModifiedHandler(SinkProtocolInfo_ModifiedSink);
            if(HasStateVariable_CurrentConnectionIDs) _S.GetStateVariableObject("CurrentConnectionIDs").OnModified -= new UPnPStateVariable.ModifiedHandler(CurrentConnectionIDs_ModifiedSink);
        }
        public void _subscribe(int Timeout)
        {
            _S.Subscribe(Timeout, null);
        }
		protected void _periodicrenew_failed_sink(UPnPService sender)
		{
			OnPeriodicRenewFailedEvent.Fire(this);
		}
        protected void _subscribe_sink(UPnPService sender, bool OK)
        {
            if(OnSubscribe!=null)
            {
                OnSubscribe(this, OK);
            }
        }
        public void SetUnspecifiedValue(string EnumType, string val)
        {
            string hash = Thread.CurrentThread.GetHashCode().ToString() + ":" + EnumType;
            UnspecifiedTable[hash] = val;
        }
        public string GetUnspecifiedValue(string EnumType)
        {
            string hash = Thread.CurrentThread.GetHashCode().ToString() + ":" + EnumType;
            if(UnspecifiedTable.ContainsKey(hash)==false)
            {
               return("");
            }
            string RetVal = (string)UnspecifiedTable[hash];
            return(RetVal);
        }
        public string[] Values_A_ARG_TYPE_Direction
        {
            get
            {
                UPnPStateVariable sv = _S.GetStateVariableObject("A_ARG_TYPE_Direction");
                return(sv.AllowedStringValues);
            }
        }
        public string Enum_A_ARG_TYPE_Direction_ToString(Enum_A_ARG_TYPE_Direction en)
        {
            string RetVal = "";
            switch(en)
            {
                case Enum_A_ARG_TYPE_Direction.INPUT:
                    RetVal = "Input";
                    break;
                case Enum_A_ARG_TYPE_Direction.OUTPUT:
                    RetVal = "Output";
                    break;
                case Enum_A_ARG_TYPE_Direction._UNSPECIFIED_:
                    RetVal = GetUnspecifiedValue("Enum_A_ARG_TYPE_Direction");
                    break;
            }
            return(RetVal);
        }
        public enum Enum_A_ARG_TYPE_Direction
        {
            _UNSPECIFIED_,
            INPUT,
            OUTPUT,
        }
        public Enum_A_ARG_TYPE_Direction A_ARG_TYPE_Direction
        {
            get
            {
               Enum_A_ARG_TYPE_Direction RetVal = 0;
               string v = (string)_S.GetStateVariable("A_ARG_TYPE_Direction");
               switch(v)
               {
                  case "Input":
                     RetVal = Enum_A_ARG_TYPE_Direction.INPUT;
                     break;
                  case "Output":
                     RetVal = Enum_A_ARG_TYPE_Direction.OUTPUT;
                     break;
                  default:
                     RetVal = Enum_A_ARG_TYPE_Direction._UNSPECIFIED_;
                     SetUnspecifiedValue("Enum_A_ARG_TYPE_Direction", v);
                     break;
               }
               return(RetVal);
           }
        }
        public string[] Values_A_ARG_TYPE_ConnectionStatus
        {
            get
            {
                UPnPStateVariable sv = _S.GetStateVariableObject("A_ARG_TYPE_ConnectionStatus");
                return(sv.AllowedStringValues);
            }
        }
        public string Enum_A_ARG_TYPE_ConnectionStatus_ToString(Enum_A_ARG_TYPE_ConnectionStatus en)
        {
            string RetVal = "";
            switch(en)
            {
                case Enum_A_ARG_TYPE_ConnectionStatus.OK:
                    RetVal = "OK";
                    break;
                case Enum_A_ARG_TYPE_ConnectionStatus.CONTENTFORMATMISMATCH:
                    RetVal = "ContentFormatMismatch";
                    break;
                case Enum_A_ARG_TYPE_ConnectionStatus.INSUFFICIENTBANDWIDTH:
                    RetVal = "InsufficientBandwidth";
                    break;
                case Enum_A_ARG_TYPE_ConnectionStatus.UNRELIABLECHANNEL:
                    RetVal = "UnreliableChannel";
                    break;
                case Enum_A_ARG_TYPE_ConnectionStatus.UNKNOWN:
                    RetVal = "Unknown";
                    break;
                case Enum_A_ARG_TYPE_ConnectionStatus._UNSPECIFIED_:
                    RetVal = GetUnspecifiedValue("Enum_A_ARG_TYPE_ConnectionStatus");
                    break;
            }
            return(RetVal);
        }
        public enum Enum_A_ARG_TYPE_ConnectionStatus
        {
            _UNSPECIFIED_,
            OK,
            CONTENTFORMATMISMATCH,
            INSUFFICIENTBANDWIDTH,
            UNRELIABLECHANNEL,
            UNKNOWN,
        }
        public Enum_A_ARG_TYPE_ConnectionStatus A_ARG_TYPE_ConnectionStatus
        {
            get
            {
               Enum_A_ARG_TYPE_ConnectionStatus RetVal = 0;
               string v = (string)_S.GetStateVariable("A_ARG_TYPE_ConnectionStatus");
               switch(v)
               {
                  case "OK":
                     RetVal = Enum_A_ARG_TYPE_ConnectionStatus.OK;
                     break;
                  case "ContentFormatMismatch":
                     RetVal = Enum_A_ARG_TYPE_ConnectionStatus.CONTENTFORMATMISMATCH;
                     break;
                  case "InsufficientBandwidth":
                     RetVal = Enum_A_ARG_TYPE_ConnectionStatus.INSUFFICIENTBANDWIDTH;
                     break;
                  case "UnreliableChannel":
                     RetVal = Enum_A_ARG_TYPE_ConnectionStatus.UNRELIABLECHANNEL;
                     break;
                  case "Unknown":
                     RetVal = Enum_A_ARG_TYPE_ConnectionStatus.UNKNOWN;
                     break;
                  default:
                     RetVal = Enum_A_ARG_TYPE_ConnectionStatus._UNSPECIFIED_;
                     SetUnspecifiedValue("Enum_A_ARG_TYPE_ConnectionStatus", v);
                     break;
               }
               return(RetVal);
           }
        }
        public System.String A_ARG_TYPE_ProtocolInfo
        {
            get
            {
               return((System.String)_S.GetStateVariable("A_ARG_TYPE_ProtocolInfo"));
            }
        }
        public System.Int32 A_ARG_TYPE_AVTransportID
        {
            get
            {
               return((System.Int32)_S.GetStateVariable("A_ARG_TYPE_AVTransportID"));
            }
        }
        public System.Int32 A_ARG_TYPE_RcsID
        {
            get
            {
               return((System.Int32)_S.GetStateVariable("A_ARG_TYPE_RcsID"));
            }
        }
        public System.Int32 A_ARG_TYPE_ConnectionID
        {
            get
            {
               return((System.Int32)_S.GetStateVariable("A_ARG_TYPE_ConnectionID"));
            }
        }
        public System.String A_ARG_TYPE_ConnectionManager
        {
            get
            {
               return((System.String)_S.GetStateVariable("A_ARG_TYPE_ConnectionManager"));
            }
        }
        public System.String SourceProtocolInfo
        {
            get
            {
               return((System.String)_S.GetStateVariable("SourceProtocolInfo"));
            }
        }
        public System.String SinkProtocolInfo
        {
            get
            {
               return((System.String)_S.GetStateVariable("SinkProtocolInfo"));
            }
        }
        public System.String CurrentConnectionIDs
        {
            get
            {
               return((System.String)_S.GetStateVariable("CurrentConnectionIDs"));
            }
        }
        public bool HasStateVariable_A_ARG_TYPE_ProtocolInfo
        {
            get
            {
               if(_S.GetStateVariableObject("A_ARG_TYPE_ProtocolInfo")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_A_ARG_TYPE_ConnectionStatus
        {
            get
            {
               if(_S.GetStateVariableObject("A_ARG_TYPE_ConnectionStatus")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_A_ARG_TYPE_AVTransportID
        {
            get
            {
               if(_S.GetStateVariableObject("A_ARG_TYPE_AVTransportID")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_A_ARG_TYPE_RcsID
        {
            get
            {
               if(_S.GetStateVariableObject("A_ARG_TYPE_RcsID")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_A_ARG_TYPE_ConnectionID
        {
            get
            {
               if(_S.GetStateVariableObject("A_ARG_TYPE_ConnectionID")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_A_ARG_TYPE_ConnectionManager
        {
            get
            {
               if(_S.GetStateVariableObject("A_ARG_TYPE_ConnectionManager")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_SourceProtocolInfo
        {
            get
            {
               if(_S.GetStateVariableObject("SourceProtocolInfo")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_SinkProtocolInfo
        {
            get
            {
               if(_S.GetStateVariableObject("SinkProtocolInfo")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_A_ARG_TYPE_Direction
        {
            get
            {
               if(_S.GetStateVariableObject("A_ARG_TYPE_Direction")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_CurrentConnectionIDs
        {
            get
            {
               if(_S.GetStateVariableObject("CurrentConnectionIDs")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_GetCurrentConnectionInfo
        {
            get
            {
               if(_S.GetAction("GetCurrentConnectionInfo")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_PrepareForConnection
        {
            get
            {
               if(_S.GetAction("PrepareForConnection")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_ConnectionComplete
        {
            get
            {
               if(_S.GetAction("ConnectionComplete")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_GetProtocolInfo
        {
            get
            {
               if(_S.GetAction("GetProtocolInfo")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_GetCurrentConnectionIDs
        {
            get
            {
               if(_S.GetAction("GetCurrentConnectionIDs")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public void Sync_GetCurrentConnectionInfo(System.Int32 ConnectionID, out System.Int32 RcsID, out System.Int32 AVTransportID, out System.String ProtocolInfo, out System.String PeerConnectionManager, out System.Int32 PeerConnectionID, out Enum_A_ARG_TYPE_Direction Direction, out Enum_A_ARG_TYPE_ConnectionStatus Status)
        {
           UPnPArgument[] args = new UPnPArgument[8];
           args[0] = new UPnPArgument("ConnectionID", ConnectionID);
           args[1] = new UPnPArgument("RcsID", "");
           args[2] = new UPnPArgument("AVTransportID", "");
           args[3] = new UPnPArgument("ProtocolInfo", "");
           args[4] = new UPnPArgument("PeerConnectionManager", "");
           args[5] = new UPnPArgument("PeerConnectionID", "");
           args[6] = new UPnPArgument("Direction", "");
           args[7] = new UPnPArgument("Status", "");
            _S.InvokeSync("GetCurrentConnectionInfo", args);
            for(int i=0;i<args.Length;++i)
            {
                switch(args[i].Name)
                {
                    case "Direction":
                        switch((string)args[i].DataValue)
                        {
                            case "Input":
                                args[i].DataValue = Enum_A_ARG_TYPE_Direction.INPUT;
                                break;
                            case "Output":
                                args[i].DataValue = Enum_A_ARG_TYPE_Direction.OUTPUT;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_Direction", (string)args[i].DataValue);
                               args[i].DataValue = Enum_A_ARG_TYPE_Direction._UNSPECIFIED_;
                               break;
                        }
                        break;
                    case "Status":
                        switch((string)args[i].DataValue)
                        {
                            case "OK":
                                args[i].DataValue = Enum_A_ARG_TYPE_ConnectionStatus.OK;
                                break;
                            case "ContentFormatMismatch":
                                args[i].DataValue = Enum_A_ARG_TYPE_ConnectionStatus.CONTENTFORMATMISMATCH;
                                break;
                            case "InsufficientBandwidth":
                                args[i].DataValue = Enum_A_ARG_TYPE_ConnectionStatus.INSUFFICIENTBANDWIDTH;
                                break;
                            case "UnreliableChannel":
                                args[i].DataValue = Enum_A_ARG_TYPE_ConnectionStatus.UNRELIABLECHANNEL;
                                break;
                            case "Unknown":
                                args[i].DataValue = Enum_A_ARG_TYPE_ConnectionStatus.UNKNOWN;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_ConnectionStatus", (string)args[i].DataValue);
                               args[i].DataValue = Enum_A_ARG_TYPE_ConnectionStatus._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            ConnectionID = (System.Int32) args[0].DataValue;
            RcsID = (System.Int32) args[1].DataValue;
            AVTransportID = (System.Int32) args[2].DataValue;
            ProtocolInfo = (System.String) args[3].DataValue;
            PeerConnectionManager = (System.String) args[4].DataValue;
            PeerConnectionID = (System.Int32) args[5].DataValue;
            Direction = (Enum_A_ARG_TYPE_Direction) args[6].DataValue;
            Status = (Enum_A_ARG_TYPE_ConnectionStatus) args[7].DataValue;
            return;
        }
        public void GetCurrentConnectionInfo(System.Int32 ConnectionID)
        {
            GetCurrentConnectionInfo(ConnectionID, null, null);
        }
        public void GetCurrentConnectionInfo(System.Int32 ConnectionID, object _Tag, Delegate_OnResult_GetCurrentConnectionInfo _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[8];
           args[0] = new UPnPArgument("ConnectionID", ConnectionID);
           args[1] = new UPnPArgument("RcsID", "");
           args[2] = new UPnPArgument("AVTransportID", "");
           args[3] = new UPnPArgument("ProtocolInfo", "");
           args[4] = new UPnPArgument("PeerConnectionManager", "");
           args[5] = new UPnPArgument("PeerConnectionID", "");
           args[6] = new UPnPArgument("Direction", "");
           args[7] = new UPnPArgument("Status", "");
           _S.InvokeAsync("GetCurrentConnectionInfo", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_GetCurrentConnectionInfo), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetCurrentConnectionInfo));
        }
        private void Sink_GetCurrentConnectionInfo(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "Direction":
                        switch((string)Args[i].DataValue)
                        {
                            case "Input":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Direction.INPUT;
                                break;
                            case "Output":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Direction.OUTPUT;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_Direction", (string)Args[i].DataValue);
                               Args[i].DataValue = Enum_A_ARG_TYPE_Direction._UNSPECIFIED_;
                               break;
                        }
                        break;
                    case "Status":
                        switch((string)Args[i].DataValue)
                        {
                            case "OK":
                                Args[i].DataValue = Enum_A_ARG_TYPE_ConnectionStatus.OK;
                                break;
                            case "ContentFormatMismatch":
                                Args[i].DataValue = Enum_A_ARG_TYPE_ConnectionStatus.CONTENTFORMATMISMATCH;
                                break;
                            case "InsufficientBandwidth":
                                Args[i].DataValue = Enum_A_ARG_TYPE_ConnectionStatus.INSUFFICIENTBANDWIDTH;
                                break;
                            case "UnreliableChannel":
                                Args[i].DataValue = Enum_A_ARG_TYPE_ConnectionStatus.UNRELIABLECHANNEL;
                                break;
                            case "Unknown":
                                Args[i].DataValue = Enum_A_ARG_TYPE_ConnectionStatus.UNKNOWN;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_ConnectionStatus", (string)Args[i].DataValue);
                               Args[i].DataValue = Enum_A_ARG_TYPE_ConnectionStatus._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetCurrentConnectionInfo)StateInfo[1])(this, (System.Int32 )Args[0].DataValue, (System.Int32 )Args[1].DataValue, (System.Int32 )Args[2].DataValue, (System.String )Args[3].DataValue, (System.String )Args[4].DataValue, (System.Int32 )Args[5].DataValue, (Enum_A_ARG_TYPE_Direction )Args[6].DataValue, (Enum_A_ARG_TYPE_ConnectionStatus )Args[7].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_GetCurrentConnectionInfo_Event.Fire(this, (System.Int32 )Args[0].DataValue, (System.Int32 )Args[1].DataValue, (System.Int32 )Args[2].DataValue, (System.String )Args[3].DataValue, (System.String )Args[4].DataValue, (System.Int32 )Args[5].DataValue, (Enum_A_ARG_TYPE_Direction )Args[6].DataValue, (Enum_A_ARG_TYPE_ConnectionStatus )Args[7].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_GetCurrentConnectionInfo(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetCurrentConnectionInfo)StateInfo[1])(this, (System.Int32 )Args[0].DataValue, (System.Int32)UPnPService.CreateObjectInstance(typeof(System.Int32),null), (System.Int32)UPnPService.CreateObjectInstance(typeof(System.Int32),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.Int32)UPnPService.CreateObjectInstance(typeof(System.Int32),null), (Enum_A_ARG_TYPE_Direction)0, (Enum_A_ARG_TYPE_ConnectionStatus)0, e, StateInfo[0]);
            }
            else
            {
                OnResult_GetCurrentConnectionInfo_Event.Fire(this, (System.Int32 )Args[0].DataValue, (System.Int32)UPnPService.CreateObjectInstance(typeof(System.Int32),null), (System.Int32)UPnPService.CreateObjectInstance(typeof(System.Int32),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.Int32)UPnPService.CreateObjectInstance(typeof(System.Int32),null), (Enum_A_ARG_TYPE_Direction)0, (Enum_A_ARG_TYPE_ConnectionStatus)0, e, StateInfo[0]);
            }
        }
        public void Sync_PrepareForConnection(System.String RemoteProtocolInfo, System.String PeerConnectionManager, System.Int32 PeerConnectionID, Enum_A_ARG_TYPE_Direction Direction, out System.Int32 ConnectionID, out System.Int32 AVTransportID, out System.Int32 RcsID)
        {
           UPnPArgument[] args = new UPnPArgument[7];
           args[0] = new UPnPArgument("RemoteProtocolInfo", RemoteProtocolInfo);
           args[1] = new UPnPArgument("PeerConnectionManager", PeerConnectionManager);
           args[2] = new UPnPArgument("PeerConnectionID", PeerConnectionID);
           switch(Direction)
           {
               case Enum_A_ARG_TYPE_Direction.INPUT:
                   args[3] = new UPnPArgument("Direction", "Input");
                   break;
               case Enum_A_ARG_TYPE_Direction.OUTPUT:
                   args[3] = new UPnPArgument("Direction", "Output");
                   break;
               default:
                  args[3] = new UPnPArgument("Direction", GetUnspecifiedValue("Enum_A_ARG_TYPE_Direction"));
                  break;
           }
           args[4] = new UPnPArgument("ConnectionID", "");
           args[5] = new UPnPArgument("AVTransportID", "");
           args[6] = new UPnPArgument("RcsID", "");
            _S.InvokeSync("PrepareForConnection", args);
            for(int i=0;i<args.Length;++i)
            {
                switch(args[i].Name)
                {
                    case "Direction":
                        switch((string)args[i].DataValue)
                        {
                            case "Input":
                                args[i].DataValue = Enum_A_ARG_TYPE_Direction.INPUT;
                                break;
                            case "Output":
                                args[i].DataValue = Enum_A_ARG_TYPE_Direction.OUTPUT;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_Direction", (string)args[i].DataValue);
                               args[i].DataValue = Enum_A_ARG_TYPE_Direction._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            RemoteProtocolInfo = (System.String) args[0].DataValue;
            PeerConnectionManager = (System.String) args[1].DataValue;
            PeerConnectionID = (System.Int32) args[2].DataValue;
            Direction = (Enum_A_ARG_TYPE_Direction) args[3].DataValue;
            ConnectionID = (System.Int32) args[4].DataValue;
            AVTransportID = (System.Int32) args[5].DataValue;
            RcsID = (System.Int32) args[6].DataValue;
            return;
        }
        public void PrepareForConnection(System.String RemoteProtocolInfo, System.String PeerConnectionManager, System.Int32 PeerConnectionID, Enum_A_ARG_TYPE_Direction Direction)
        {
            PrepareForConnection(RemoteProtocolInfo, PeerConnectionManager, PeerConnectionID, Direction, null, null);
        }
        public void PrepareForConnection(System.String RemoteProtocolInfo, System.String PeerConnectionManager, System.Int32 PeerConnectionID, Enum_A_ARG_TYPE_Direction Direction, object _Tag, Delegate_OnResult_PrepareForConnection _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[7];
           args[0] = new UPnPArgument("RemoteProtocolInfo", RemoteProtocolInfo);
           args[1] = new UPnPArgument("PeerConnectionManager", PeerConnectionManager);
           args[2] = new UPnPArgument("PeerConnectionID", PeerConnectionID);
           switch(Direction)
           {
               case Enum_A_ARG_TYPE_Direction.INPUT:
                   args[3] = new UPnPArgument("Direction", "Input");
                   break;
               case Enum_A_ARG_TYPE_Direction.OUTPUT:
                   args[3] = new UPnPArgument("Direction", "Output");
                   break;
               default:
                  args[3] = new UPnPArgument("Direction", GetUnspecifiedValue("Enum_A_ARG_TYPE_Direction"));
                  break;
           }
           args[4] = new UPnPArgument("ConnectionID", "");
           args[5] = new UPnPArgument("AVTransportID", "");
           args[6] = new UPnPArgument("RcsID", "");
           _S.InvokeAsync("PrepareForConnection", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_PrepareForConnection), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_PrepareForConnection));
        }
        private void Sink_PrepareForConnection(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "Direction":
                        switch((string)Args[i].DataValue)
                        {
                            case "Input":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Direction.INPUT;
                                break;
                            case "Output":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Direction.OUTPUT;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_Direction", (string)Args[i].DataValue);
                               Args[i].DataValue = Enum_A_ARG_TYPE_Direction._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_PrepareForConnection)StateInfo[1])(this, (System.String )Args[0].DataValue, (System.String )Args[1].DataValue, (System.Int32 )Args[2].DataValue, (Enum_A_ARG_TYPE_Direction )Args[3].DataValue, (System.Int32 )Args[4].DataValue, (System.Int32 )Args[5].DataValue, (System.Int32 )Args[6].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_PrepareForConnection_Event.Fire(this, (System.String )Args[0].DataValue, (System.String )Args[1].DataValue, (System.Int32 )Args[2].DataValue, (Enum_A_ARG_TYPE_Direction )Args[3].DataValue, (System.Int32 )Args[4].DataValue, (System.Int32 )Args[5].DataValue, (System.Int32 )Args[6].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_PrepareForConnection(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "Direction":
                        switch((string)Args[i].DataValue)
                        {
                            case "Input":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Direction.INPUT;
                                break;
                            case "Output":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Direction.OUTPUT;
                                break;
                        }
                        break;
                }
            }
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_PrepareForConnection)StateInfo[1])(this, (System.String )Args[0].DataValue, (System.String )Args[1].DataValue, (System.Int32 )Args[2].DataValue, (Enum_A_ARG_TYPE_Direction )Args[3].DataValue, (System.Int32)UPnPService.CreateObjectInstance(typeof(System.Int32),null), (System.Int32)UPnPService.CreateObjectInstance(typeof(System.Int32),null), (System.Int32)UPnPService.CreateObjectInstance(typeof(System.Int32),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_PrepareForConnection_Event.Fire(this, (System.String )Args[0].DataValue, (System.String )Args[1].DataValue, (System.Int32 )Args[2].DataValue, (Enum_A_ARG_TYPE_Direction )Args[3].DataValue, (System.Int32)UPnPService.CreateObjectInstance(typeof(System.Int32),null), (System.Int32)UPnPService.CreateObjectInstance(typeof(System.Int32),null), (System.Int32)UPnPService.CreateObjectInstance(typeof(System.Int32),null), e, StateInfo[0]);
            }
        }
        public void Sync_ConnectionComplete(System.Int32 ConnectionID)
        {
           UPnPArgument[] args = new UPnPArgument[1];
           args[0] = new UPnPArgument("ConnectionID", ConnectionID);
            _S.InvokeSync("ConnectionComplete", args);
            ConnectionID = (System.Int32) args[0].DataValue;
            return;
        }
        public void ConnectionComplete(System.Int32 ConnectionID)
        {
            ConnectionComplete(ConnectionID, null, null);
        }
        public void ConnectionComplete(System.Int32 ConnectionID, object _Tag, Delegate_OnResult_ConnectionComplete _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[1];
           args[0] = new UPnPArgument("ConnectionID", ConnectionID);
           _S.InvokeAsync("ConnectionComplete", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_ConnectionComplete), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_ConnectionComplete));
        }
        private void Sink_ConnectionComplete(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_ConnectionComplete)StateInfo[1])(this, (System.Int32 )Args[0].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_ConnectionComplete_Event.Fire(this, (System.Int32 )Args[0].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_ConnectionComplete(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_ConnectionComplete)StateInfo[1])(this, (System.Int32 )Args[0].DataValue, e, StateInfo[0]);
            }
            else
            {
                OnResult_ConnectionComplete_Event.Fire(this, (System.Int32 )Args[0].DataValue, e, StateInfo[0]);
            }
        }
        public void Sync_GetProtocolInfo(out System.String Source, out System.String Sink)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("Source", "");
           args[1] = new UPnPArgument("Sink", "");
            _S.InvokeSync("GetProtocolInfo", args);
            Source = (System.String) args[0].DataValue;
            Sink = (System.String) args[1].DataValue;
            return;
        }
        public void GetProtocolInfo()
        {
            GetProtocolInfo(null, null);
        }
        public void GetProtocolInfo(object _Tag, Delegate_OnResult_GetProtocolInfo _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("Source", "");
           args[1] = new UPnPArgument("Sink", "");
           _S.InvokeAsync("GetProtocolInfo", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_GetProtocolInfo), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetProtocolInfo));
        }
        private void Sink_GetProtocolInfo(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetProtocolInfo)StateInfo[1])(this, (System.String )Args[0].DataValue, (System.String )Args[1].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_GetProtocolInfo_Event.Fire(this, (System.String )Args[0].DataValue, (System.String )Args[1].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_GetProtocolInfo(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetProtocolInfo)StateInfo[1])(this, (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_GetProtocolInfo_Event.Fire(this, (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), e, StateInfo[0]);
            }
        }
        public void Sync_GetCurrentConnectionIDs(out System.String ConnectionIDs)
        {
           UPnPArgument[] args = new UPnPArgument[1];
           args[0] = new UPnPArgument("ConnectionIDs", "");
            _S.InvokeSync("GetCurrentConnectionIDs", args);
            ConnectionIDs = (System.String) args[0].DataValue;
            return;
        }
        public void GetCurrentConnectionIDs()
        {
            GetCurrentConnectionIDs(null, null);
        }
        public void GetCurrentConnectionIDs(object _Tag, Delegate_OnResult_GetCurrentConnectionIDs _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[1];
           args[0] = new UPnPArgument("ConnectionIDs", "");
           _S.InvokeAsync("GetCurrentConnectionIDs", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_GetCurrentConnectionIDs), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetCurrentConnectionIDs));
        }
        private void Sink_GetCurrentConnectionIDs(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetCurrentConnectionIDs)StateInfo[1])(this, (System.String )Args[0].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_GetCurrentConnectionIDs_Event.Fire(this, (System.String )Args[0].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_GetCurrentConnectionIDs(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetCurrentConnectionIDs)StateInfo[1])(this, (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_GetCurrentConnectionIDs_Event.Fire(this, (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), e, StateInfo[0]);
            }
        }
    }
}