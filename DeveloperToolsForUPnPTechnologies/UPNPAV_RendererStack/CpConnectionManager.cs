using System;
using System.Collections;
using System.Threading;
using Intel.UPNP;

namespace Intel.UPNP.AVRENDERERSTACK
{
    /// <summary>
    /// Transparent ClientSide UPnP Service
    /// </summary>
    public class CpConnectionManager
    {
       private Hashtable UnspecifiedTable = Hashtable.Synchronized(new Hashtable());
       internal UPnPService _S;

       public static string SERVICE_NAME = "urn:schemas-upnp-org:service:ConnectionManager:";
       public double VERSION
       {
           get
           {
               return(double.Parse(_S.Version));
           }
       }

       public delegate void StateVariableModifiedHandler_SourceProtocolInfo(System.String NewValue);
       public event StateVariableModifiedHandler_SourceProtocolInfo OnStateVariable_SourceProtocolInfo;
       protected ArrayList WeakList_SourceProtocolInfo = new ArrayList();
       public void AddWeakEvent_StateVariable_SourceProtocolInfo(StateVariableModifiedHandler_SourceProtocolInfo cb)
       {
           WeakList_SourceProtocolInfo.Add(new WeakReference(cb));
       }
       protected void SourceProtocolInfo_ModifiedSink(UPnPStateVariable Var, object NewValue)
       {
            if(OnStateVariable_SourceProtocolInfo != null) OnStateVariable_SourceProtocolInfo((System.String)NewValue);
            WeakReference[] w = (WeakReference[])WeakList_SourceProtocolInfo.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                    ((StateVariableModifiedHandler_SourceProtocolInfo)wr.Target)((System.String)NewValue);
                }
                else
                {
                    WeakList_SourceProtocolInfo.Remove(wr);
                }
            }
       }
       public delegate void StateVariableModifiedHandler_SinkProtocolInfo(System.String NewValue);
       public event StateVariableModifiedHandler_SinkProtocolInfo OnStateVariable_SinkProtocolInfo;
       protected ArrayList WeakList_SinkProtocolInfo = new ArrayList();
       public void AddWeakEvent_StateVariable_SinkProtocolInfo(StateVariableModifiedHandler_SinkProtocolInfo cb)
       {
           WeakList_SinkProtocolInfo.Add(new WeakReference(cb));
       }
       protected void SinkProtocolInfo_ModifiedSink(UPnPStateVariable Var, object NewValue)
       {
            if(OnStateVariable_SinkProtocolInfo != null) OnStateVariable_SinkProtocolInfo((System.String)NewValue);
            WeakReference[] w = (WeakReference[])WeakList_SinkProtocolInfo.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                    ((StateVariableModifiedHandler_SinkProtocolInfo)wr.Target)((System.String)NewValue);
                }
                else
                {
                    WeakList_SinkProtocolInfo.Remove(wr);
                }
            }
       }
       public delegate void StateVariableModifiedHandler_CurrentConnectionIDs(System.String NewValue);
       public event StateVariableModifiedHandler_CurrentConnectionIDs OnStateVariable_CurrentConnectionIDs;
       protected ArrayList WeakList_CurrentConnectionIDs = new ArrayList();
       public void AddWeakEvent_StateVariable_CurrentConnectionIDs(StateVariableModifiedHandler_CurrentConnectionIDs cb)
       {
           WeakList_CurrentConnectionIDs.Add(new WeakReference(cb));
       }
       protected void CurrentConnectionIDs_ModifiedSink(UPnPStateVariable Var, object NewValue)
       {
            if(OnStateVariable_CurrentConnectionIDs != null) OnStateVariable_CurrentConnectionIDs((System.String)NewValue);
            WeakReference[] w = (WeakReference[])WeakList_CurrentConnectionIDs.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                    ((StateVariableModifiedHandler_CurrentConnectionIDs)wr.Target)((System.String)NewValue);
                }
                else
                {
                    WeakList_CurrentConnectionIDs.Remove(wr);
                }
            }
       }
       public delegate void SubscribeHandler(bool Success);
       public event SubscribeHandler OnSubscribe;
       public delegate void Delegate_OnResult_GetCurrentConnectionInfo(System.Int32 ConnectionID, System.Int32 RcsID, System.Int32 AVTransportID, System.String ProtocolInfo, System.String PeerConnectionManager, System.Int32 PeerConnectionID, Enum_A_ARG_TYPE_Direction Direction, Enum_A_ARG_TYPE_ConnectionStatus Status, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_GetCurrentConnectionInfo OnResult_GetCurrentConnectionInfo;
       protected ArrayList WeakList_GetCurrentConnectionInfo = new ArrayList();
       public void AddWeakEvent_Result_GetCurrentConnectionInfo(Delegate_OnResult_GetCurrentConnectionInfo d)
       {
           WeakList_GetCurrentConnectionInfo.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_PrepareForConnection(System.String RemoteProtocolInfo, System.String PeerConnectionManager, System.Int32 PeerConnectionID, Enum_A_ARG_TYPE_Direction Direction, System.Int32 ConnectionID, System.Int32 AVTransportID, System.Int32 RcsID, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_PrepareForConnection OnResult_PrepareForConnection;
       protected ArrayList WeakList_PrepareForConnection = new ArrayList();
       public void AddWeakEvent_Result_PrepareForConnection(Delegate_OnResult_PrepareForConnection d)
       {
           WeakList_PrepareForConnection.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_ConnectionComplete(System.Int32 ConnectionID, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_ConnectionComplete OnResult_ConnectionComplete;
       protected ArrayList WeakList_ConnectionComplete = new ArrayList();
       public void AddWeakEvent_Result_ConnectionComplete(Delegate_OnResult_ConnectionComplete d)
       {
           WeakList_ConnectionComplete.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_GetProtocolInfo(System.String Source, System.String Sink, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_GetProtocolInfo OnResult_GetProtocolInfo;
       protected ArrayList WeakList_GetProtocolInfo = new ArrayList();
       public void AddWeakEvent_Result_GetProtocolInfo(Delegate_OnResult_GetProtocolInfo d)
       {
           WeakList_GetProtocolInfo.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_GetCurrentConnectionIDs(System.String ConnectionIDs, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_GetCurrentConnectionIDs OnResult_GetCurrentConnectionIDs;
       protected ArrayList WeakList_GetCurrentConnectionIDs = new ArrayList();
       public void AddWeakEvent_Result_GetCurrentConnectionIDs(Delegate_OnResult_GetCurrentConnectionIDs d)
       {
           WeakList_GetCurrentConnectionIDs.Add(new WeakReference(d));
       }

        public CpConnectionManager(UPnPService s)
        {
            _S = s;
            _S.OnSubscribe += new UPnPService.UPnPEventSubscribeHandler(_subscribe_sink);
            if(HasStateVariable_SourceProtocolInfo) _S.GetStateVariableObject("SourceProtocolInfo").OnModified += new UPnPStateVariable.ModifiedHandler(SourceProtocolInfo_ModifiedSink);
            if(HasStateVariable_SinkProtocolInfo) _S.GetStateVariableObject("SinkProtocolInfo").OnModified += new UPnPStateVariable.ModifiedHandler(SinkProtocolInfo_ModifiedSink);
            if(HasStateVariable_CurrentConnectionIDs) _S.GetStateVariableObject("CurrentConnectionIDs").OnModified += new UPnPStateVariable.ModifiedHandler(CurrentConnectionIDs_ModifiedSink);
        }
        public void _subscribe(int Timeout, UPnPService.UPnPEventHandler UPnPEventCallback)
        {
            _S.Subscribe(Timeout, UPnPEventCallback);
        }
        protected void _subscribe_sink(UPnPService sender, bool OK)
        {
            if(OnSubscribe!=null)
            {
                OnSubscribe(OK);
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
        public int GetCurrentConnectionInfo(System.Int32 ConnectionID)
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
           return(_S.InvokeAsync("GetCurrentConnectionInfo", args, new UPnPService.UPnPServiceInvokeHandler(Sink_GetCurrentConnectionInfo), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetCurrentConnectionInfo)));
        }
        private void Sink_GetCurrentConnectionInfo(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
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
            if(OnResult_GetCurrentConnectionInfo != null)
            {
               OnResult_GetCurrentConnectionInfo((System.Int32 )Args[0].DataValue, (System.Int32 )Args[1].DataValue, (System.Int32 )Args[2].DataValue, (System.String )Args[3].DataValue, (System.String )Args[4].DataValue, (System.Int32 )Args[5].DataValue, (Enum_A_ARG_TYPE_Direction )Args[6].DataValue, (Enum_A_ARG_TYPE_ConnectionStatus )Args[7].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetCurrentConnectionInfo.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetCurrentConnectionInfo)wr.Target)((System.Int32 )Args[0].DataValue, (System.Int32 )Args[1].DataValue, (System.Int32 )Args[2].DataValue, (System.String )Args[3].DataValue, (System.String )Args[4].DataValue, (System.Int32 )Args[5].DataValue, (Enum_A_ARG_TYPE_Direction )Args[6].DataValue, (Enum_A_ARG_TYPE_ConnectionStatus )Args[7].DataValue, null, Handle);
                }
                else
                {
                    WeakList_GetCurrentConnectionInfo.Remove(wr);
                }
            }
        }
        private void Error_Sink_GetCurrentConnectionInfo(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_GetCurrentConnectionInfo != null)
            {
                 OnResult_GetCurrentConnectionInfo((System.Int32 )Args[0].DataValue, (System.Int32)UPnPService.CreateObjectInstance(typeof(System.Int32),null), (System.Int32)UPnPService.CreateObjectInstance(typeof(System.Int32),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.Int32)UPnPService.CreateObjectInstance(typeof(System.Int32),null), (Enum_A_ARG_TYPE_Direction)0, (Enum_A_ARG_TYPE_ConnectionStatus)0, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetCurrentConnectionInfo.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetCurrentConnectionInfo)wr.Target)((System.Int32 )Args[0].DataValue, (System.Int32)UPnPService.CreateObjectInstance(typeof(System.Int32),null), (System.Int32)UPnPService.CreateObjectInstance(typeof(System.Int32),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.Int32)UPnPService.CreateObjectInstance(typeof(System.Int32),null), (Enum_A_ARG_TYPE_Direction)0, (Enum_A_ARG_TYPE_ConnectionStatus)0, e, Handle);
                }
                else
                {
                    WeakList_GetCurrentConnectionInfo.Remove(wr);
                }
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
        public int PrepareForConnection(System.String RemoteProtocolInfo, System.String PeerConnectionManager, System.Int32 PeerConnectionID, Enum_A_ARG_TYPE_Direction Direction)
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
           return(_S.InvokeAsync("PrepareForConnection", args, new UPnPService.UPnPServiceInvokeHandler(Sink_PrepareForConnection), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_PrepareForConnection)));
        }
        private void Sink_PrepareForConnection(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
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
            if(OnResult_PrepareForConnection != null)
            {
               OnResult_PrepareForConnection((System.String )Args[0].DataValue, (System.String )Args[1].DataValue, (System.Int32 )Args[2].DataValue, (Enum_A_ARG_TYPE_Direction )Args[3].DataValue, (System.Int32 )Args[4].DataValue, (System.Int32 )Args[5].DataValue, (System.Int32 )Args[6].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_PrepareForConnection.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_PrepareForConnection)wr.Target)((System.String )Args[0].DataValue, (System.String )Args[1].DataValue, (System.Int32 )Args[2].DataValue, (Enum_A_ARG_TYPE_Direction )Args[3].DataValue, (System.Int32 )Args[4].DataValue, (System.Int32 )Args[5].DataValue, (System.Int32 )Args[6].DataValue, null, Handle);
                }
                else
                {
                    WeakList_PrepareForConnection.Remove(wr);
                }
            }
        }
        private void Error_Sink_PrepareForConnection(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
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
            if(OnResult_PrepareForConnection != null)
            {
                 OnResult_PrepareForConnection((System.String )Args[0].DataValue, (System.String )Args[1].DataValue, (System.Int32 )Args[2].DataValue, (Enum_A_ARG_TYPE_Direction )Args[3].DataValue, (System.Int32)UPnPService.CreateObjectInstance(typeof(System.Int32),null), (System.Int32)UPnPService.CreateObjectInstance(typeof(System.Int32),null), (System.Int32)UPnPService.CreateObjectInstance(typeof(System.Int32),null), e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_PrepareForConnection.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_PrepareForConnection)wr.Target)((System.String )Args[0].DataValue, (System.String )Args[1].DataValue, (System.Int32 )Args[2].DataValue, (Enum_A_ARG_TYPE_Direction )Args[3].DataValue, (System.Int32)UPnPService.CreateObjectInstance(typeof(System.Int32),null), (System.Int32)UPnPService.CreateObjectInstance(typeof(System.Int32),null), (System.Int32)UPnPService.CreateObjectInstance(typeof(System.Int32),null), e, Handle);
                }
                else
                {
                    WeakList_PrepareForConnection.Remove(wr);
                }
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
        public int ConnectionComplete(System.Int32 ConnectionID)
        {
           UPnPArgument[] args = new UPnPArgument[1];
           args[0] = new UPnPArgument("ConnectionID", ConnectionID);
           return(_S.InvokeAsync("ConnectionComplete", args, new UPnPService.UPnPServiceInvokeHandler(Sink_ConnectionComplete), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_ConnectionComplete)));
        }
        private void Sink_ConnectionComplete(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_ConnectionComplete != null)
            {
               OnResult_ConnectionComplete((System.Int32 )Args[0].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_ConnectionComplete.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_ConnectionComplete)wr.Target)((System.Int32 )Args[0].DataValue, null, Handle);
                }
                else
                {
                    WeakList_ConnectionComplete.Remove(wr);
                }
            }
        }
        private void Error_Sink_ConnectionComplete(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_ConnectionComplete != null)
            {
                 OnResult_ConnectionComplete((System.Int32 )Args[0].DataValue, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_ConnectionComplete.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_ConnectionComplete)wr.Target)((System.Int32 )Args[0].DataValue, e, Handle);
                }
                else
                {
                    WeakList_ConnectionComplete.Remove(wr);
                }
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
        public int GetProtocolInfo()
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("Source", "");
           args[1] = new UPnPArgument("Sink", "");
           return(_S.InvokeAsync("GetProtocolInfo", args, new UPnPService.UPnPServiceInvokeHandler(Sink_GetProtocolInfo), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetProtocolInfo)));
        }
        private void Sink_GetProtocolInfo(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_GetProtocolInfo != null)
            {
               OnResult_GetProtocolInfo((System.String )Args[0].DataValue, (System.String )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetProtocolInfo.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetProtocolInfo)wr.Target)((System.String )Args[0].DataValue, (System.String )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_GetProtocolInfo.Remove(wr);
                }
            }
        }
        private void Error_Sink_GetProtocolInfo(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_GetProtocolInfo != null)
            {
                 OnResult_GetProtocolInfo((System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetProtocolInfo.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetProtocolInfo)wr.Target)((System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), e, Handle);
                }
                else
                {
                    WeakList_GetProtocolInfo.Remove(wr);
                }
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
        public int GetCurrentConnectionIDs()
        {
           UPnPArgument[] args = new UPnPArgument[1];
           args[0] = new UPnPArgument("ConnectionIDs", "");
           return(_S.InvokeAsync("GetCurrentConnectionIDs", args, new UPnPService.UPnPServiceInvokeHandler(Sink_GetCurrentConnectionIDs), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetCurrentConnectionIDs)));
        }
        private void Sink_GetCurrentConnectionIDs(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_GetCurrentConnectionIDs != null)
            {
               OnResult_GetCurrentConnectionIDs((System.String )Args[0].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetCurrentConnectionIDs.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetCurrentConnectionIDs)wr.Target)((System.String )Args[0].DataValue, null, Handle);
                }
                else
                {
                    WeakList_GetCurrentConnectionIDs.Remove(wr);
                }
            }
        }
        private void Error_Sink_GetCurrentConnectionIDs(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_GetCurrentConnectionIDs != null)
            {
                 OnResult_GetCurrentConnectionIDs((System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetCurrentConnectionIDs.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetCurrentConnectionIDs)wr.Target)((System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), e, Handle);
                }
                else
                {
                    WeakList_GetCurrentConnectionIDs.Remove(wr);
                }
            }
        }
    }
}