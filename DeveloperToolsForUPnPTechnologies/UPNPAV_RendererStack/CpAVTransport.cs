using System;
using System.Collections;
using System.Threading;
using Intel.UPNP;

namespace Intel.UPNP.AVRENDERERSTACK
{
    /// <summary>
    /// Transparent ClientSide UPnP Service
    /// </summary>
    public class CpAVTransport
    {
       private Hashtable UnspecifiedTable = Hashtable.Synchronized(new Hashtable());
       internal UPnPService _S;

       public static string SERVICE_NAME = "urn:schemas-upnp-org:service:AVTransport:";
       public double VERSION
       {
           get
           {
               return(double.Parse(_S.Version));
           }
       }

       public delegate void StateVariableModifiedHandler_LastChange(System.String NewValue);
       public event StateVariableModifiedHandler_LastChange OnStateVariable_LastChange;
       protected ArrayList WeakList_LastChange = new ArrayList();
       public void AddWeakEvent_StateVariable_LastChange(StateVariableModifiedHandler_LastChange cb)
       {
           WeakList_LastChange.Add(new WeakReference(cb));
       }
       protected void LastChange_ModifiedSink(UPnPStateVariable Var, object NewValue)
       {
            if(OnStateVariable_LastChange != null) OnStateVariable_LastChange((System.String)NewValue);
            WeakReference[] w = (WeakReference[])WeakList_LastChange.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                    ((StateVariableModifiedHandler_LastChange)wr.Target)((System.String)NewValue);
                }
                else
                {
                    WeakList_LastChange.Remove(wr);
                }
            }
       }
       public delegate void SubscribeHandler(bool Success);
       public event SubscribeHandler OnSubscribe;
       public delegate void Delegate_OnResult_Play(System.UInt32 InstanceID, Enum_TransportPlaySpeed Speed, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_Play OnResult_Play;
       protected ArrayList WeakList_Play = new ArrayList();
       public void AddWeakEvent_Result_Play(Delegate_OnResult_Play d)
       {
           WeakList_Play.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_Stop(System.UInt32 InstanceID, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_Stop OnResult_Stop;
       protected ArrayList WeakList_Stop = new ArrayList();
       public void AddWeakEvent_Result_Stop(Delegate_OnResult_Stop d)
       {
           WeakList_Stop.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_Next(System.UInt32 InstanceID, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_Next OnResult_Next;
       protected ArrayList WeakList_Next = new ArrayList();
       public void AddWeakEvent_Result_Next(Delegate_OnResult_Next d)
       {
           WeakList_Next.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_Previous(System.UInt32 InstanceID, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_Previous OnResult_Previous;
       protected ArrayList WeakList_Previous = new ArrayList();
       public void AddWeakEvent_Result_Previous(Delegate_OnResult_Previous d)
       {
           WeakList_Previous.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_SetNextAVTransportURI(System.UInt32 InstanceID, System.String NextURI, System.String NextURIMetaData, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_SetNextAVTransportURI OnResult_SetNextAVTransportURI;
       protected ArrayList WeakList_SetNextAVTransportURI = new ArrayList();
       public void AddWeakEvent_Result_SetNextAVTransportURI(Delegate_OnResult_SetNextAVTransportURI d)
       {
           WeakList_SetNextAVTransportURI.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_GetMediaInfo(System.UInt32 InstanceID, System.UInt32 NrTracks, System.String MediaDuration, System.String CurrentURI, System.String CurrentURIMetaData, System.String NextURI, System.String NextURIMetaData, Enum_PlaybackStorageMedium PlayMedium, Enum_RecordStorageMedium RecordMedium, Enum_RecordMediumWriteStatus WriteStatus, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_GetMediaInfo OnResult_GetMediaInfo;
       protected ArrayList WeakList_GetMediaInfo = new ArrayList();
       public void AddWeakEvent_Result_GetMediaInfo(Delegate_OnResult_GetMediaInfo d)
       {
           WeakList_GetMediaInfo.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_GetDeviceCapabilities(System.UInt32 InstanceID, System.String PlayMedia, System.String RecMedia, System.String RecQualityModes, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_GetDeviceCapabilities OnResult_GetDeviceCapabilities;
       protected ArrayList WeakList_GetDeviceCapabilities = new ArrayList();
       public void AddWeakEvent_Result_GetDeviceCapabilities(Delegate_OnResult_GetDeviceCapabilities d)
       {
           WeakList_GetDeviceCapabilities.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_SetAVTransportURI(System.UInt32 InstanceID, System.String CurrentURI, System.String CurrentURIMetaData, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_SetAVTransportURI OnResult_SetAVTransportURI;
       protected ArrayList WeakList_SetAVTransportURI = new ArrayList();
       public void AddWeakEvent_Result_SetAVTransportURI(Delegate_OnResult_SetAVTransportURI d)
       {
           WeakList_SetAVTransportURI.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_SetPlayMode(System.UInt32 InstanceID, Enum_CurrentPlayMode NewPlayMode, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_SetPlayMode OnResult_SetPlayMode;
       protected ArrayList WeakList_SetPlayMode = new ArrayList();
       public void AddWeakEvent_Result_SetPlayMode(Delegate_OnResult_SetPlayMode d)
       {
           WeakList_SetPlayMode.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_GetCurrentTransportActions(System.UInt32 InstanceID, System.String Actions, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_GetCurrentTransportActions OnResult_GetCurrentTransportActions;
       protected ArrayList WeakList_GetCurrentTransportActions = new ArrayList();
       public void AddWeakEvent_Result_GetCurrentTransportActions(Delegate_OnResult_GetCurrentTransportActions d)
       {
           WeakList_GetCurrentTransportActions.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_GetTransportSettings(System.UInt32 InstanceID, Enum_CurrentPlayMode PlayMode, Enum_CurrentRecordQualityMode RecQualityMode, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_GetTransportSettings OnResult_GetTransportSettings;
       protected ArrayList WeakList_GetTransportSettings = new ArrayList();
       public void AddWeakEvent_Result_GetTransportSettings(Delegate_OnResult_GetTransportSettings d)
       {
           WeakList_GetTransportSettings.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_GetTransportInfo(System.UInt32 InstanceID, Enum_TransportState CurrentTransportState, Enum_TransportStatus CurrentTransportStatus, Enum_TransportPlaySpeed CurrentSpeed, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_GetTransportInfo OnResult_GetTransportInfo;
       protected ArrayList WeakList_GetTransportInfo = new ArrayList();
       public void AddWeakEvent_Result_GetTransportInfo(Delegate_OnResult_GetTransportInfo d)
       {
           WeakList_GetTransportInfo.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_Pause(System.UInt32 InstanceID, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_Pause OnResult_Pause;
       protected ArrayList WeakList_Pause = new ArrayList();
       public void AddWeakEvent_Result_Pause(Delegate_OnResult_Pause d)
       {
           WeakList_Pause.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_Seek(System.UInt32 InstanceID, Enum_A_ARG_TYPE_SeekMode Unit, System.String Target, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_Seek OnResult_Seek;
       protected ArrayList WeakList_Seek = new ArrayList();
       public void AddWeakEvent_Result_Seek(Delegate_OnResult_Seek d)
       {
           WeakList_Seek.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_GetPositionInfo(System.UInt32 InstanceID, System.UInt32 Track, System.String TrackDuration, System.String TrackMetaData, System.String TrackURI, System.String RelTime, System.String AbsTime, System.Int32 RelCount, System.Int32 AbsCount, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_GetPositionInfo OnResult_GetPositionInfo;
       protected ArrayList WeakList_GetPositionInfo = new ArrayList();
       public void AddWeakEvent_Result_GetPositionInfo(Delegate_OnResult_GetPositionInfo d)
       {
           WeakList_GetPositionInfo.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_Record(System.UInt32 InstanceID, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_Record OnResult_Record;
       protected ArrayList WeakList_Record = new ArrayList();
       public void AddWeakEvent_Result_Record(Delegate_OnResult_Record d)
       {
           WeakList_Record.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_SetRecordQualityMode(System.UInt32 InstanceID, Enum_CurrentRecordQualityMode NewRecordQualityMode, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_SetRecordQualityMode OnResult_SetRecordQualityMode;
       protected ArrayList WeakList_SetRecordQualityMode = new ArrayList();
       public void AddWeakEvent_Result_SetRecordQualityMode(Delegate_OnResult_SetRecordQualityMode d)
       {
           WeakList_SetRecordQualityMode.Add(new WeakReference(d));
       }

        public CpAVTransport(UPnPService s)
        {
            _S = s;
            _S.OnSubscribe += new UPnPService.UPnPEventSubscribeHandler(_subscribe_sink);
            if(HasStateVariable_LastChange) _S.GetStateVariableObject("LastChange").OnModified += new UPnPStateVariable.ModifiedHandler(LastChange_ModifiedSink);
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
        public string[] Values_TransportState
        {
            get
            {
                UPnPStateVariable sv = _S.GetStateVariableObject("TransportState");
                return(sv.AllowedStringValues);
            }
        }
        public enum Enum_TransportState
        {
            _UNSPECIFIED_,
            STOPPED,
            PAUSED_PLAYBACK,
            PAUSED_RECORDING,
            PLAYING,
            RECORDING,
            TRANSITIONING,
            NO_MEDIA_PRESENT,
        }
        public Enum_TransportState TransportState
        {
            get
            {
               Enum_TransportState RetVal = 0;
               string v = (string)_S.GetStateVariable("TransportState");
               switch(v)
               {
                  case "STOPPED":
                     RetVal = Enum_TransportState.STOPPED;
                     break;
                  case "PAUSED_PLAYBACK":
                     RetVal = Enum_TransportState.PAUSED_PLAYBACK;
                     break;
                  case "PAUSED_RECORDING":
                     RetVal = Enum_TransportState.PAUSED_RECORDING;
                     break;
                  case "PLAYING":
                     RetVal = Enum_TransportState.PLAYING;
                     break;
                  case "RECORDING":
                     RetVal = Enum_TransportState.RECORDING;
                     break;
                  case "TRANSITIONING":
                     RetVal = Enum_TransportState.TRANSITIONING;
                     break;
                  case "NO_MEDIA_PRESENT":
                     RetVal = Enum_TransportState.NO_MEDIA_PRESENT;
                     break;
                  default:
                     RetVal = Enum_TransportState._UNSPECIFIED_;
                     SetUnspecifiedValue("Enum_TransportState", v);
                     break;
               }
               return(RetVal);
           }
        }
        public string[] Values_RecordMediumWriteStatus
        {
            get
            {
                UPnPStateVariable sv = _S.GetStateVariableObject("RecordMediumWriteStatus");
                return(sv.AllowedStringValues);
            }
        }
        public enum Enum_RecordMediumWriteStatus
        {
            _UNSPECIFIED_,
            WRITABLE,
            PROTECTED,
            NOT_WRITABLE,
            UNKNOWN,
            NOT_IMPLEMENTED,
        }
        public Enum_RecordMediumWriteStatus RecordMediumWriteStatus
        {
            get
            {
               Enum_RecordMediumWriteStatus RetVal = 0;
               string v = (string)_S.GetStateVariable("RecordMediumWriteStatus");
               switch(v)
               {
                  case "WRITABLE":
                     RetVal = Enum_RecordMediumWriteStatus.WRITABLE;
                     break;
                  case "PROTECTED":
                     RetVal = Enum_RecordMediumWriteStatus.PROTECTED;
                     break;
                  case "NOT_WRITABLE":
                     RetVal = Enum_RecordMediumWriteStatus.NOT_WRITABLE;
                     break;
                  case "UNKNOWN":
                     RetVal = Enum_RecordMediumWriteStatus.UNKNOWN;
                     break;
                  case "NOT_IMPLEMENTED":
                     RetVal = Enum_RecordMediumWriteStatus.NOT_IMPLEMENTED;
                     break;
                  default:
                     RetVal = Enum_RecordMediumWriteStatus._UNSPECIFIED_;
                     SetUnspecifiedValue("Enum_RecordMediumWriteStatus", v);
                     break;
               }
               return(RetVal);
           }
        }
        public string[] Values_CurrentRecordQualityMode
        {
            get
            {
                UPnPStateVariable sv = _S.GetStateVariableObject("CurrentRecordQualityMode");
                return(sv.AllowedStringValues);
            }
        }
        public enum Enum_CurrentRecordQualityMode
        {
            _UNSPECIFIED_,
            _0_EP,
            _1_LP,
            _2_SP,
            _0_BASIC,
            _1_MEDIUM,
            _2_HIGH,
            NOT_IMPLEMENTED,
            _VENDOR_DEFINED_,
        }
        public Enum_CurrentRecordQualityMode CurrentRecordQualityMode
        {
            get
            {
               Enum_CurrentRecordQualityMode RetVal = 0;
               string v = (string)_S.GetStateVariable("CurrentRecordQualityMode");
               switch(v)
               {
                  case "0:EP":
                     RetVal = Enum_CurrentRecordQualityMode._0_EP;
                     break;
                  case "1:LP":
                     RetVal = Enum_CurrentRecordQualityMode._1_LP;
                     break;
                  case "2:SP":
                     RetVal = Enum_CurrentRecordQualityMode._2_SP;
                     break;
                  case "0:BASIC":
                     RetVal = Enum_CurrentRecordQualityMode._0_BASIC;
                     break;
                  case "1:MEDIUM":
                     RetVal = Enum_CurrentRecordQualityMode._1_MEDIUM;
                     break;
                  case "2:HIGH":
                     RetVal = Enum_CurrentRecordQualityMode._2_HIGH;
                     break;
                  case "NOT_IMPLEMENTED":
                     RetVal = Enum_CurrentRecordQualityMode.NOT_IMPLEMENTED;
                     break;
                  case " vendor-defined ":
                     RetVal = Enum_CurrentRecordQualityMode._VENDOR_DEFINED_;
                     break;
                  default:
                     RetVal = Enum_CurrentRecordQualityMode._UNSPECIFIED_;
                     SetUnspecifiedValue("Enum_CurrentRecordQualityMode", v);
                     break;
               }
               return(RetVal);
           }
        }
        public string[] Values_TransportPlaySpeed
        {
            get
            {
                UPnPStateVariable sv = _S.GetStateVariableObject("TransportPlaySpeed");
                return(sv.AllowedStringValues);
            }
        }
        public enum Enum_TransportPlaySpeed
        {
            _UNSPECIFIED_,
            _1,
            _VENDOR_DEFINED_,
        }
        public Enum_TransportPlaySpeed TransportPlaySpeed
        {
            get
            {
               Enum_TransportPlaySpeed RetVal = 0;
               string v = (string)_S.GetStateVariable("TransportPlaySpeed");
               switch(v)
               {
                  case "1":
                     RetVal = Enum_TransportPlaySpeed._1;
                     break;
                  case " vendor-defined ":
                     RetVal = Enum_TransportPlaySpeed._VENDOR_DEFINED_;
                     break;
                  default:
                     RetVal = Enum_TransportPlaySpeed._UNSPECIFIED_;
                     SetUnspecifiedValue("Enum_TransportPlaySpeed", v);
                     break;
               }
               return(RetVal);
           }
        }
        public string[] Values_CurrentPlayMode
        {
            get
            {
                UPnPStateVariable sv = _S.GetStateVariableObject("CurrentPlayMode");
                return(sv.AllowedStringValues);
            }
        }
        public enum Enum_CurrentPlayMode
        {
            _UNSPECIFIED_,
            NORMAL,
            SHUFFLE,
            REPEAT_ONE,
            REPEAT_ALL,
            RANDOM,
            DIRECT_1,
            INTRO,
        }
        public Enum_CurrentPlayMode CurrentPlayMode
        {
            get
            {
               Enum_CurrentPlayMode RetVal = 0;
               string v = (string)_S.GetStateVariable("CurrentPlayMode");
               switch(v)
               {
                  case "NORMAL":
                     RetVal = Enum_CurrentPlayMode.NORMAL;
                     break;
                  case "SHUFFLE":
                     RetVal = Enum_CurrentPlayMode.SHUFFLE;
                     break;
                  case "REPEAT_ONE":
                     RetVal = Enum_CurrentPlayMode.REPEAT_ONE;
                     break;
                  case "REPEAT_ALL":
                     RetVal = Enum_CurrentPlayMode.REPEAT_ALL;
                     break;
                  case "RANDOM":
                     RetVal = Enum_CurrentPlayMode.RANDOM;
                     break;
                  case "DIRECT_1":
                     RetVal = Enum_CurrentPlayMode.DIRECT_1;
                     break;
                  case "INTRO":
                     RetVal = Enum_CurrentPlayMode.INTRO;
                     break;
                  default:
                     RetVal = Enum_CurrentPlayMode._UNSPECIFIED_;
                     SetUnspecifiedValue("Enum_CurrentPlayMode", v);
                     break;
               }
               return(RetVal);
           }
        }
        public string[] Values_PlaybackStorageMedium
        {
            get
            {
                UPnPStateVariable sv = _S.GetStateVariableObject("PlaybackStorageMedium");
                return(sv.AllowedStringValues);
            }
        }
        public enum Enum_PlaybackStorageMedium
        {
            _UNSPECIFIED_,
            UNKNOWN,
            DV,
            MINI_DV,
            VHS,
            W_VHS,
            S_VHS,
            D_VHS,
            VHSC,
            VIDEO8,
            HI8,
            CD_ROM,
            CD_DA,
            CD_R,
            CD_RW,
            VIDEO_CD,
            SACD,
            MD_AUDIO,
            MD_PICTURE,
            DVD_ROM,
            DVD_VIDEO,
            DVD_R,
            DVD_RW,
            DVD_minus_RW,
            DVD_RAM,
            DVD_AUDIO,
            DAT,
            LD,
            HDD,
            MICRO_MV,
            NETWORK,
            NONE,
            NOT_IMPLEMENTED,
            _VENDOR_DEFINED_,
        }
        public Enum_PlaybackStorageMedium PlaybackStorageMedium
        {
            get
            {
               Enum_PlaybackStorageMedium RetVal = 0;
               string v = (string)_S.GetStateVariable("PlaybackStorageMedium");
               switch(v)
               {
                  case "UNKNOWN":
                     RetVal = Enum_PlaybackStorageMedium.UNKNOWN;
                     break;
                  case "DV":
                     RetVal = Enum_PlaybackStorageMedium.DV;
                     break;
                  case "MINI-DV":
                     RetVal = Enum_PlaybackStorageMedium.MINI_DV;
                     break;
                  case "VHS":
                     RetVal = Enum_PlaybackStorageMedium.VHS;
                     break;
                  case "W-VHS":
                     RetVal = Enum_PlaybackStorageMedium.W_VHS;
                     break;
                  case "S-VHS":
                     RetVal = Enum_PlaybackStorageMedium.S_VHS;
                     break;
                  case "D-VHS":
                     RetVal = Enum_PlaybackStorageMedium.D_VHS;
                     break;
                  case "VHSC":
                     RetVal = Enum_PlaybackStorageMedium.VHSC;
                     break;
                  case "VIDEO8":
                     RetVal = Enum_PlaybackStorageMedium.VIDEO8;
                     break;
                  case "HI8":
                     RetVal = Enum_PlaybackStorageMedium.HI8;
                     break;
                  case "CD-ROM":
                     RetVal = Enum_PlaybackStorageMedium.CD_ROM;
                     break;
                  case "CD-DA":
                     RetVal = Enum_PlaybackStorageMedium.CD_DA;
                     break;
                  case "CD-R":
                     RetVal = Enum_PlaybackStorageMedium.CD_R;
                     break;
                  case "CD-RW":
                     RetVal = Enum_PlaybackStorageMedium.CD_RW;
                     break;
                  case "VIDEO-CD":
                     RetVal = Enum_PlaybackStorageMedium.VIDEO_CD;
                     break;
                  case "SACD":
                     RetVal = Enum_PlaybackStorageMedium.SACD;
                     break;
                  case "MD-AUDIO":
                     RetVal = Enum_PlaybackStorageMedium.MD_AUDIO;
                     break;
                  case "MD-PICTURE":
                     RetVal = Enum_PlaybackStorageMedium.MD_PICTURE;
                     break;
                  case "DVD-ROM":
                     RetVal = Enum_PlaybackStorageMedium.DVD_ROM;
                     break;
                  case "DVD-VIDEO":
                     RetVal = Enum_PlaybackStorageMedium.DVD_VIDEO;
                     break;
                  case "DVD-R":
                     RetVal = Enum_PlaybackStorageMedium.DVD_R;
                     break;
                  case "DVD+RW":
                     RetVal = Enum_PlaybackStorageMedium.DVD_RW;
                     break;
                  case "DVD-RW":
                     RetVal = Enum_PlaybackStorageMedium.DVD_minus_RW;
                     break;
                  case "DVD-RAM":
                     RetVal = Enum_PlaybackStorageMedium.DVD_RAM;
                     break;
                  case "DVD-AUDIO":
                     RetVal = Enum_PlaybackStorageMedium.DVD_AUDIO;
                     break;
                  case "DAT":
                     RetVal = Enum_PlaybackStorageMedium.DAT;
                     break;
                  case "LD":
                     RetVal = Enum_PlaybackStorageMedium.LD;
                     break;
                  case "HDD":
                     RetVal = Enum_PlaybackStorageMedium.HDD;
                     break;
                  case "MICRO-MV":
                     RetVal = Enum_PlaybackStorageMedium.MICRO_MV;
                     break;
                  case "NETWORK":
                     RetVal = Enum_PlaybackStorageMedium.NETWORK;
                     break;
                  case "NONE":
                     RetVal = Enum_PlaybackStorageMedium.NONE;
                     break;
                  case "NOT_IMPLEMENTED":
                     RetVal = Enum_PlaybackStorageMedium.NOT_IMPLEMENTED;
                     break;
                  case " vendor-defined ":
                     RetVal = Enum_PlaybackStorageMedium._VENDOR_DEFINED_;
                     break;
                  default:
                     RetVal = Enum_PlaybackStorageMedium._UNSPECIFIED_;
                     SetUnspecifiedValue("Enum_PlaybackStorageMedium", v);
                     break;
               }
               return(RetVal);
           }
        }
        public string[] Values_RecordStorageMedium
        {
            get
            {
                UPnPStateVariable sv = _S.GetStateVariableObject("RecordStorageMedium");
                return(sv.AllowedStringValues);
            }
        }
        public enum Enum_RecordStorageMedium
        {
            _UNSPECIFIED_,
            UNKNOWN,
            DV,
            MINI_DV,
            VHS,
            W_VHS,
            S_VHS,
            D_VHS,
            VHSC,
            VIDEO8,
            HI8,
            CD_ROM,
            CD_DA,
            CD_R,
            CD_RW,
            VIDEO_CD,
            SACD,
            MD_AUDIO,
            MD_PICTURE,
            DVD_ROM,
            DVD_VIDEO,
            DVD_R,
            DVD_RW,
            DVD_minus_RW,
            DVD_RAM,
            DVD_AUDIO,
            DAT,
            LD,
            HDD,
            MICRO_MV,
            NETWORK,
            NONE,
            NOT_IMPLEMENTED,
            _VENDOR_DEFINED_,
        }
        public Enum_RecordStorageMedium RecordStorageMedium
        {
            get
            {
               Enum_RecordStorageMedium RetVal = 0;
               string v = (string)_S.GetStateVariable("RecordStorageMedium");
               switch(v)
               {
                  case "UNKNOWN":
                     RetVal = Enum_RecordStorageMedium.UNKNOWN;
                     break;
                  case "DV":
                     RetVal = Enum_RecordStorageMedium.DV;
                     break;
                  case "MINI-DV":
                     RetVal = Enum_RecordStorageMedium.MINI_DV;
                     break;
                  case "VHS":
                     RetVal = Enum_RecordStorageMedium.VHS;
                     break;
                  case "W-VHS":
                     RetVal = Enum_RecordStorageMedium.W_VHS;
                     break;
                  case "S-VHS":
                     RetVal = Enum_RecordStorageMedium.S_VHS;
                     break;
                  case "D-VHS":
                     RetVal = Enum_RecordStorageMedium.D_VHS;
                     break;
                  case "VHSC":
                     RetVal = Enum_RecordStorageMedium.VHSC;
                     break;
                  case "VIDEO8":
                     RetVal = Enum_RecordStorageMedium.VIDEO8;
                     break;
                  case "HI8":
                     RetVal = Enum_RecordStorageMedium.HI8;
                     break;
                  case "CD-ROM":
                     RetVal = Enum_RecordStorageMedium.CD_ROM;
                     break;
                  case "CD-DA":
                     RetVal = Enum_RecordStorageMedium.CD_DA;
                     break;
                  case "CD-R":
                     RetVal = Enum_RecordStorageMedium.CD_R;
                     break;
                  case "CD-RW":
                     RetVal = Enum_RecordStorageMedium.CD_RW;
                     break;
                  case "VIDEO-CD":
                     RetVal = Enum_RecordStorageMedium.VIDEO_CD;
                     break;
                  case "SACD":
                     RetVal = Enum_RecordStorageMedium.SACD;
                     break;
                  case "MD-AUDIO":
                     RetVal = Enum_RecordStorageMedium.MD_AUDIO;
                     break;
                  case "MD-PICTURE":
                     RetVal = Enum_RecordStorageMedium.MD_PICTURE;
                     break;
                  case "DVD-ROM":
                     RetVal = Enum_RecordStorageMedium.DVD_ROM;
                     break;
                  case "DVD-VIDEO":
                     RetVal = Enum_RecordStorageMedium.DVD_VIDEO;
                     break;
                  case "DVD-R":
                     RetVal = Enum_RecordStorageMedium.DVD_R;
                     break;
                  case "DVD+RW":
                     RetVal = Enum_RecordStorageMedium.DVD_RW;
                     break;
                  case "DVD-RW":
                     RetVal = Enum_RecordStorageMedium.DVD_minus_RW;
                     break;
                  case "DVD-RAM":
                     RetVal = Enum_RecordStorageMedium.DVD_RAM;
                     break;
                  case "DVD-AUDIO":
                     RetVal = Enum_RecordStorageMedium.DVD_AUDIO;
                     break;
                  case "DAT":
                     RetVal = Enum_RecordStorageMedium.DAT;
                     break;
                  case "LD":
                     RetVal = Enum_RecordStorageMedium.LD;
                     break;
                  case "HDD":
                     RetVal = Enum_RecordStorageMedium.HDD;
                     break;
                  case "MICRO-MV":
                     RetVal = Enum_RecordStorageMedium.MICRO_MV;
                     break;
                  case "NETWORK":
                     RetVal = Enum_RecordStorageMedium.NETWORK;
                     break;
                  case "NONE":
                     RetVal = Enum_RecordStorageMedium.NONE;
                     break;
                  case "NOT_IMPLEMENTED":
                     RetVal = Enum_RecordStorageMedium.NOT_IMPLEMENTED;
                     break;
                  case " vendor-defined ":
                     RetVal = Enum_RecordStorageMedium._VENDOR_DEFINED_;
                     break;
                  default:
                     RetVal = Enum_RecordStorageMedium._UNSPECIFIED_;
                     SetUnspecifiedValue("Enum_RecordStorageMedium", v);
                     break;
               }
               return(RetVal);
           }
        }
        public string[] Values_A_ARG_TYPE_SeekMode
        {
            get
            {
                UPnPStateVariable sv = _S.GetStateVariableObject("A_ARG_TYPE_SeekMode");
                return(sv.AllowedStringValues);
            }
        }
        public enum Enum_A_ARG_TYPE_SeekMode
        {
            _UNSPECIFIED_,
            ABS_TIME,
            REL_TIME,
            ABS_COUNT,
            REL_COUNT,
            TRACK_NR,
            CHANNEL_FREQ,
            TAPE_INDEX,
            FRAME,
        }
        public Enum_A_ARG_TYPE_SeekMode A_ARG_TYPE_SeekMode
        {
            get
            {
               Enum_A_ARG_TYPE_SeekMode RetVal = 0;
               string v = (string)_S.GetStateVariable("A_ARG_TYPE_SeekMode");
               switch(v)
               {
                  case "ABS_TIME":
                     RetVal = Enum_A_ARG_TYPE_SeekMode.ABS_TIME;
                     break;
                  case "REL_TIME":
                     RetVal = Enum_A_ARG_TYPE_SeekMode.REL_TIME;
                     break;
                  case "ABS_COUNT":
                     RetVal = Enum_A_ARG_TYPE_SeekMode.ABS_COUNT;
                     break;
                  case "REL_COUNT":
                     RetVal = Enum_A_ARG_TYPE_SeekMode.REL_COUNT;
                     break;
                  case "TRACK_NR":
                     RetVal = Enum_A_ARG_TYPE_SeekMode.TRACK_NR;
                     break;
                  case "CHANNEL_FREQ":
                     RetVal = Enum_A_ARG_TYPE_SeekMode.CHANNEL_FREQ;
                     break;
                  case "TAPE-INDEX":
                     RetVal = Enum_A_ARG_TYPE_SeekMode.TAPE_INDEX;
                     break;
                  case "FRAME":
                     RetVal = Enum_A_ARG_TYPE_SeekMode.FRAME;
                     break;
                  default:
                     RetVal = Enum_A_ARG_TYPE_SeekMode._UNSPECIFIED_;
                     SetUnspecifiedValue("Enum_A_ARG_TYPE_SeekMode", v);
                     break;
               }
               return(RetVal);
           }
        }
        public string[] Values_TransportStatus
        {
            get
            {
                UPnPStateVariable sv = _S.GetStateVariableObject("TransportStatus");
                return(sv.AllowedStringValues);
            }
        }
        public enum Enum_TransportStatus
        {
            _UNSPECIFIED_,
            OK,
            ERROR_OCCURRED,
            _VENDOR_DEFINED_,
        }
        public Enum_TransportStatus TransportStatus
        {
            get
            {
               Enum_TransportStatus RetVal = 0;
               string v = (string)_S.GetStateVariable("TransportStatus");
               switch(v)
               {
                  case "OK":
                     RetVal = Enum_TransportStatus.OK;
                     break;
                  case "ERROR_OCCURRED":
                     RetVal = Enum_TransportStatus.ERROR_OCCURRED;
                     break;
                  case " vendor-defined ":
                     RetVal = Enum_TransportStatus._VENDOR_DEFINED_;
                     break;
                  default:
                     RetVal = Enum_TransportStatus._UNSPECIFIED_;
                     SetUnspecifiedValue("Enum_TransportStatus", v);
                     break;
               }
               return(RetVal);
           }
        }
        public System.String LastChange
        {
            get
            {
               return((System.String)_S.GetStateVariable("LastChange"));
            }
        }
        public System.String RelativeTimePosition
        {
            get
            {
               return((System.String)_S.GetStateVariable("RelativeTimePosition"));
            }
        }
        public System.String CurrentTrackURI
        {
            get
            {
               return((System.String)_S.GetStateVariable("CurrentTrackURI"));
            }
        }
        public System.String CurrentTrackDuration
        {
            get
            {
               return((System.String)_S.GetStateVariable("CurrentTrackDuration"));
            }
        }
        public System.String PossibleRecordQualityModes
        {
            get
            {
               return((System.String)_S.GetStateVariable("PossibleRecordQualityModes"));
            }
        }
        public System.String CurrentMediaDuration
        {
            get
            {
               return((System.String)_S.GetStateVariable("CurrentMediaDuration"));
            }
        }
        public System.Int32 AbsoluteCounterPosition
        {
            get
            {
               return((System.Int32)_S.GetStateVariable("AbsoluteCounterPosition"));
            }
        }
        public System.Int32 RelativeCounterPosition
        {
            get
            {
               return((System.Int32)_S.GetStateVariable("RelativeCounterPosition"));
            }
        }
        public System.UInt32 A_ARG_TYPE_InstanceID
        {
            get
            {
               return((System.UInt32)_S.GetStateVariable("A_ARG_TYPE_InstanceID"));
            }
        }
        public System.String AVTransportURI
        {
            get
            {
               return((System.String)_S.GetStateVariable("AVTransportURI"));
            }
        }
        public System.String CurrentTrackMetaData
        {
            get
            {
               return((System.String)_S.GetStateVariable("CurrentTrackMetaData"));
            }
        }
        public System.String NextAVTransportURI
        {
            get
            {
               return((System.String)_S.GetStateVariable("NextAVTransportURI"));
            }
        }
        public System.String AVTransportURIMetaData
        {
            get
            {
               return((System.String)_S.GetStateVariable("AVTransportURIMetaData"));
            }
        }
        public System.String PossibleRecordStorageMedia
        {
            get
            {
               return((System.String)_S.GetStateVariable("PossibleRecordStorageMedia"));
            }
        }
        public System.String AbsoluteTimePosition
        {
            get
            {
               return((System.String)_S.GetStateVariable("AbsoluteTimePosition"));
            }
        }
        public System.String NextAVTransportURIMetaData
        {
            get
            {
               return((System.String)_S.GetStateVariable("NextAVTransportURIMetaData"));
            }
        }
        public System.String CurrentTransportActions
        {
            get
            {
               return((System.String)_S.GetStateVariable("CurrentTransportActions"));
            }
        }
        public System.String PossiblePlaybackStorageMedia
        {
            get
            {
               return((System.String)_S.GetStateVariable("PossiblePlaybackStorageMedia"));
            }
        }
        public System.UInt32 NumberOfTracks
        {
            get
            {
               return((System.UInt32)_S.GetStateVariable("NumberOfTracks"));
            }
        }
        public System.UInt32 CurrentTrack
        {
            get
            {
               return((System.UInt32)_S.GetStateVariable("CurrentTrack"));
            }
        }
        public System.String A_ARG_TYPE_SeekTarget
        {
            get
            {
               return((System.String)_S.GetStateVariable("A_ARG_TYPE_SeekTarget"));
            }
        }
        public bool HasStateVariable_CurrentPlayMode
        {
            get
            {
               if(_S.GetStateVariableObject("CurrentPlayMode")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_RecordStorageMedium
        {
            get
            {
               if(_S.GetStateVariableObject("RecordStorageMedium")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_LastChange
        {
            get
            {
               if(_S.GetStateVariableObject("LastChange")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_RelativeTimePosition
        {
            get
            {
               if(_S.GetStateVariableObject("RelativeTimePosition")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_CurrentTrackURI
        {
            get
            {
               if(_S.GetStateVariableObject("CurrentTrackURI")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_CurrentTrackDuration
        {
            get
            {
               if(_S.GetStateVariableObject("CurrentTrackDuration")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_CurrentRecordQualityMode
        {
            get
            {
               if(_S.GetStateVariableObject("CurrentRecordQualityMode")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_PossibleRecordQualityModes
        {
            get
            {
               if(_S.GetStateVariableObject("PossibleRecordQualityModes")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_CurrentMediaDuration
        {
            get
            {
               if(_S.GetStateVariableObject("CurrentMediaDuration")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_AbsoluteCounterPosition
        {
            get
            {
               if(_S.GetStateVariableObject("AbsoluteCounterPosition")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_RelativeCounterPosition
        {
            get
            {
               if(_S.GetStateVariableObject("RelativeCounterPosition")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_A_ARG_TYPE_InstanceID
        {
            get
            {
               if(_S.GetStateVariableObject("A_ARG_TYPE_InstanceID")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_AVTransportURI
        {
            get
            {
               if(_S.GetStateVariableObject("AVTransportURI")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_CurrentTrackMetaData
        {
            get
            {
               if(_S.GetStateVariableObject("CurrentTrackMetaData")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_TransportPlaySpeed
        {
            get
            {
               if(_S.GetStateVariableObject("TransportPlaySpeed")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_NextAVTransportURI
        {
            get
            {
               if(_S.GetStateVariableObject("NextAVTransportURI")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_AVTransportURIMetaData
        {
            get
            {
               if(_S.GetStateVariableObject("AVTransportURIMetaData")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_PossibleRecordStorageMedia
        {
            get
            {
               if(_S.GetStateVariableObject("PossibleRecordStorageMedia")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_AbsoluteTimePosition
        {
            get
            {
               if(_S.GetStateVariableObject("AbsoluteTimePosition")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_NextAVTransportURIMetaData
        {
            get
            {
               if(_S.GetStateVariableObject("NextAVTransportURIMetaData")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_PlaybackStorageMedium
        {
            get
            {
               if(_S.GetStateVariableObject("PlaybackStorageMedium")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_CurrentTransportActions
        {
            get
            {
               if(_S.GetStateVariableObject("CurrentTransportActions")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_RecordMediumWriteStatus
        {
            get
            {
               if(_S.GetStateVariableObject("RecordMediumWriteStatus")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_PossiblePlaybackStorageMedia
        {
            get
            {
               if(_S.GetStateVariableObject("PossiblePlaybackStorageMedia")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_TransportState
        {
            get
            {
               if(_S.GetStateVariableObject("TransportState")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasMaximum_NumberOfTracks
        {
             get
             {
                 return(_S.GetStateVariableObject("NumberOfTracks").Maximum!=null);
             }
        }
        public bool HasMinimum_NumberOfTracks
        {
             get
             {
                 return(_S.GetStateVariableObject("NumberOfTracks").Minimum!=null);
             }
        }
        public System.UInt32 Maximum_NumberOfTracks
        {
             get
             {
                 return((System.UInt32)_S.GetStateVariableObject("NumberOfTracks").Maximum);
             }
        }
        public System.UInt32 Minimum_NumberOfTracks
        {
             get
             {
                 return((System.UInt32)_S.GetStateVariableObject("NumberOfTracks").Minimum);
             }
        }
        public bool HasStateVariable_NumberOfTracks
        {
            get
            {
               if(_S.GetStateVariableObject("NumberOfTracks")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_A_ARG_TYPE_SeekMode
        {
            get
            {
               if(_S.GetStateVariableObject("A_ARG_TYPE_SeekMode")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasMaximum_CurrentTrack
        {
             get
             {
                 return(_S.GetStateVariableObject("CurrentTrack").Maximum!=null);
             }
        }
        public bool HasMinimum_CurrentTrack
        {
             get
             {
                 return(_S.GetStateVariableObject("CurrentTrack").Minimum!=null);
             }
        }
        public System.UInt32 Maximum_CurrentTrack
        {
             get
             {
                 return((System.UInt32)_S.GetStateVariableObject("CurrentTrack").Maximum);
             }
        }
        public System.UInt32 Minimum_CurrentTrack
        {
             get
             {
                 return((System.UInt32)_S.GetStateVariableObject("CurrentTrack").Minimum);
             }
        }
        public bool HasStateVariable_CurrentTrack
        {
            get
            {
               if(_S.GetStateVariableObject("CurrentTrack")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_TransportStatus
        {
            get
            {
               if(_S.GetStateVariableObject("TransportStatus")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_A_ARG_TYPE_SeekTarget
        {
            get
            {
               if(_S.GetStateVariableObject("A_ARG_TYPE_SeekTarget")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_Play
        {
            get
            {
               if(_S.GetAction("Play")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_Stop
        {
            get
            {
               if(_S.GetAction("Stop")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_Next
        {
            get
            {
               if(_S.GetAction("Next")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_Previous
        {
            get
            {
               if(_S.GetAction("Previous")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_SetNextAVTransportURI
        {
            get
            {
               if(_S.GetAction("SetNextAVTransportURI")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_GetMediaInfo
        {
            get
            {
               if(_S.GetAction("GetMediaInfo")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_GetDeviceCapabilities
        {
            get
            {
               if(_S.GetAction("GetDeviceCapabilities")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_SetAVTransportURI
        {
            get
            {
               if(_S.GetAction("SetAVTransportURI")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_SetPlayMode
        {
            get
            {
               if(_S.GetAction("SetPlayMode")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_GetCurrentTransportActions
        {
            get
            {
               if(_S.GetAction("GetCurrentTransportActions")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_GetTransportSettings
        {
            get
            {
               if(_S.GetAction("GetTransportSettings")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_GetTransportInfo
        {
            get
            {
               if(_S.GetAction("GetTransportInfo")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_Pause
        {
            get
            {
               if(_S.GetAction("Pause")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_Seek
        {
            get
            {
               if(_S.GetAction("Seek")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_GetPositionInfo
        {
            get
            {
               if(_S.GetAction("GetPositionInfo")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_Record
        {
            get
            {
               if(_S.GetAction("Record")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_SetRecordQualityMode
        {
            get
            {
               if(_S.GetAction("SetRecordQualityMode")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public void Sync_Play(System.UInt32 InstanceID, Enum_TransportPlaySpeed Speed)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           switch(Speed)
           {
               case Enum_TransportPlaySpeed._1:
                   args[1] = new UPnPArgument("Speed", "1");
                   break;
               case Enum_TransportPlaySpeed._VENDOR_DEFINED_:
                   args[1] = new UPnPArgument("Speed", " vendor-defined ");
                   break;
               default:
                  args[1] = new UPnPArgument("Speed", GetUnspecifiedValue("Enum_TransportPlaySpeed"));
                  break;
           }
            _S.InvokeSync("Play", args);
            for(int i=0;i<args.Length;++i)
            {
                switch(args[i].Name)
                {
                    case "Speed":
                        switch((string)args[i].DataValue)
                        {
                            case "1":
                                args[i].DataValue = Enum_TransportPlaySpeed._1;
                                break;
                            case " vendor-defined ":
                                args[i].DataValue = Enum_TransportPlaySpeed._VENDOR_DEFINED_;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_TransportPlaySpeed", (string)args[i].DataValue);
                               args[i].DataValue = Enum_TransportPlaySpeed._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            InstanceID = (System.UInt32) args[0].DataValue;
            Speed = (Enum_TransportPlaySpeed) args[1].DataValue;
            return;
        }
        public int Play(System.UInt32 InstanceID, Enum_TransportPlaySpeed Speed)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           switch(Speed)
           {
               case Enum_TransportPlaySpeed._1:
                   args[1] = new UPnPArgument("Speed", "1");
                   break;
               case Enum_TransportPlaySpeed._VENDOR_DEFINED_:
                   args[1] = new UPnPArgument("Speed", " vendor-defined ");
                   break;
               default:
                  args[1] = new UPnPArgument("Speed", GetUnspecifiedValue("Enum_TransportPlaySpeed"));
                  break;
           }
           return(_S.InvokeAsync("Play", args, new UPnPService.UPnPServiceInvokeHandler(Sink_Play), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_Play)));
        }
        private void Sink_Play(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "Speed":
                        switch((string)Args[i].DataValue)
                        {
                            case "1":
                                Args[i].DataValue = Enum_TransportPlaySpeed._1;
                                break;
                            case " vendor-defined ":
                                Args[i].DataValue = Enum_TransportPlaySpeed._VENDOR_DEFINED_;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_TransportPlaySpeed", (string)Args[i].DataValue);
                               Args[i].DataValue = Enum_TransportPlaySpeed._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            if(OnResult_Play != null)
            {
               OnResult_Play((System.UInt32 )Args[0].DataValue, (Enum_TransportPlaySpeed )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_Play.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_Play)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_TransportPlaySpeed )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_Play.Remove(wr);
                }
            }
        }
        private void Error_Sink_Play(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "Speed":
                        switch((string)Args[i].DataValue)
                        {
                            case "1":
                                Args[i].DataValue = Enum_TransportPlaySpeed._1;
                                break;
                            case " vendor-defined ":
                                Args[i].DataValue = Enum_TransportPlaySpeed._VENDOR_DEFINED_;
                                break;
                        }
                        break;
                }
            }
            if(OnResult_Play != null)
            {
                 OnResult_Play((System.UInt32 )Args[0].DataValue, (Enum_TransportPlaySpeed )Args[1].DataValue, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_Play.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_Play)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_TransportPlaySpeed )Args[1].DataValue, e, Handle);
                }
                else
                {
                    WeakList_Play.Remove(wr);
                }
            }
        }
        public void Sync_Stop(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[1];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
            _S.InvokeSync("Stop", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            return;
        }
        public int Stop(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[1];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           return(_S.InvokeAsync("Stop", args, new UPnPService.UPnPServiceInvokeHandler(Sink_Stop), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_Stop)));
        }
        private void Sink_Stop(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_Stop != null)
            {
               OnResult_Stop((System.UInt32 )Args[0].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_Stop.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_Stop)wr.Target)((System.UInt32 )Args[0].DataValue, null, Handle);
                }
                else
                {
                    WeakList_Stop.Remove(wr);
                }
            }
        }
        private void Error_Sink_Stop(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_Stop != null)
            {
                 OnResult_Stop((System.UInt32 )Args[0].DataValue, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_Stop.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_Stop)wr.Target)((System.UInt32 )Args[0].DataValue, e, Handle);
                }
                else
                {
                    WeakList_Stop.Remove(wr);
                }
            }
        }
        public void Sync_Next(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[1];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
            _S.InvokeSync("Next", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            return;
        }
        public int Next(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[1];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           return(_S.InvokeAsync("Next", args, new UPnPService.UPnPServiceInvokeHandler(Sink_Next), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_Next)));
        }
        private void Sink_Next(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_Next != null)
            {
               OnResult_Next((System.UInt32 )Args[0].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_Next.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_Next)wr.Target)((System.UInt32 )Args[0].DataValue, null, Handle);
                }
                else
                {
                    WeakList_Next.Remove(wr);
                }
            }
        }
        private void Error_Sink_Next(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_Next != null)
            {
                 OnResult_Next((System.UInt32 )Args[0].DataValue, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_Next.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_Next)wr.Target)((System.UInt32 )Args[0].DataValue, e, Handle);
                }
                else
                {
                    WeakList_Next.Remove(wr);
                }
            }
        }
        public void Sync_Previous(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[1];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
            _S.InvokeSync("Previous", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            return;
        }
        public int Previous(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[1];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           return(_S.InvokeAsync("Previous", args, new UPnPService.UPnPServiceInvokeHandler(Sink_Previous), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_Previous)));
        }
        private void Sink_Previous(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_Previous != null)
            {
               OnResult_Previous((System.UInt32 )Args[0].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_Previous.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_Previous)wr.Target)((System.UInt32 )Args[0].DataValue, null, Handle);
                }
                else
                {
                    WeakList_Previous.Remove(wr);
                }
            }
        }
        private void Error_Sink_Previous(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_Previous != null)
            {
                 OnResult_Previous((System.UInt32 )Args[0].DataValue, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_Previous.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_Previous)wr.Target)((System.UInt32 )Args[0].DataValue, e, Handle);
                }
                else
                {
                    WeakList_Previous.Remove(wr);
                }
            }
        }
        public void Sync_SetNextAVTransportURI(System.UInt32 InstanceID, System.String NextURI, System.String NextURIMetaData)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("NextURI", NextURI);
           args[2] = new UPnPArgument("NextURIMetaData", NextURIMetaData);
            _S.InvokeSync("SetNextAVTransportURI", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            NextURI = (System.String) args[1].DataValue;
            NextURIMetaData = (System.String) args[2].DataValue;
            return;
        }
        public int SetNextAVTransportURI(System.UInt32 InstanceID, System.String NextURI, System.String NextURIMetaData)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("NextURI", NextURI);
           args[2] = new UPnPArgument("NextURIMetaData", NextURIMetaData);
           return(_S.InvokeAsync("SetNextAVTransportURI", args, new UPnPService.UPnPServiceInvokeHandler(Sink_SetNextAVTransportURI), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetNextAVTransportURI)));
        }
        private void Sink_SetNextAVTransportURI(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_SetNextAVTransportURI != null)
            {
               OnResult_SetNextAVTransportURI((System.UInt32 )Args[0].DataValue, (System.String )Args[1].DataValue, (System.String )Args[2].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetNextAVTransportURI.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetNextAVTransportURI)wr.Target)((System.UInt32 )Args[0].DataValue, (System.String )Args[1].DataValue, (System.String )Args[2].DataValue, null, Handle);
                }
                else
                {
                    WeakList_SetNextAVTransportURI.Remove(wr);
                }
            }
        }
        private void Error_Sink_SetNextAVTransportURI(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_SetNextAVTransportURI != null)
            {
                 OnResult_SetNextAVTransportURI((System.UInt32 )Args[0].DataValue, (System.String )Args[1].DataValue, (System.String )Args[2].DataValue, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetNextAVTransportURI.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetNextAVTransportURI)wr.Target)((System.UInt32 )Args[0].DataValue, (System.String )Args[1].DataValue, (System.String )Args[2].DataValue, e, Handle);
                }
                else
                {
                    WeakList_SetNextAVTransportURI.Remove(wr);
                }
            }
        }
        public void Sync_GetMediaInfo(System.UInt32 InstanceID, out System.UInt32 NrTracks, out System.String MediaDuration, out System.String CurrentURI, out System.String CurrentURIMetaData, out System.String NextURI, out System.String NextURIMetaData, out Enum_PlaybackStorageMedium PlayMedium, out Enum_RecordStorageMedium RecordMedium, out Enum_RecordMediumWriteStatus WriteStatus)
        {
           UPnPArgument[] args = new UPnPArgument[10];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("NrTracks", "");
           args[2] = new UPnPArgument("MediaDuration", "");
           args[3] = new UPnPArgument("CurrentURI", "");
           args[4] = new UPnPArgument("CurrentURIMetaData", "");
           args[5] = new UPnPArgument("NextURI", "");
           args[6] = new UPnPArgument("NextURIMetaData", "");
           args[7] = new UPnPArgument("PlayMedium", "");
           args[8] = new UPnPArgument("RecordMedium", "");
           args[9] = new UPnPArgument("WriteStatus", "");
            _S.InvokeSync("GetMediaInfo", args);
            for(int i=0;i<args.Length;++i)
            {
                switch(args[i].Name)
                {
                    case "PlayMedium":
                        switch((string)args[i].DataValue)
                        {
                            case "UNKNOWN":
                                args[i].DataValue = Enum_PlaybackStorageMedium.UNKNOWN;
                                break;
                            case "DV":
                                args[i].DataValue = Enum_PlaybackStorageMedium.DV;
                                break;
                            case "MINI-DV":
                                args[i].DataValue = Enum_PlaybackStorageMedium.MINI_DV;
                                break;
                            case "VHS":
                                args[i].DataValue = Enum_PlaybackStorageMedium.VHS;
                                break;
                            case "W-VHS":
                                args[i].DataValue = Enum_PlaybackStorageMedium.W_VHS;
                                break;
                            case "S-VHS":
                                args[i].DataValue = Enum_PlaybackStorageMedium.S_VHS;
                                break;
                            case "D-VHS":
                                args[i].DataValue = Enum_PlaybackStorageMedium.D_VHS;
                                break;
                            case "VHSC":
                                args[i].DataValue = Enum_PlaybackStorageMedium.VHSC;
                                break;
                            case "VIDEO8":
                                args[i].DataValue = Enum_PlaybackStorageMedium.VIDEO8;
                                break;
                            case "HI8":
                                args[i].DataValue = Enum_PlaybackStorageMedium.HI8;
                                break;
                            case "CD-ROM":
                                args[i].DataValue = Enum_PlaybackStorageMedium.CD_ROM;
                                break;
                            case "CD-DA":
                                args[i].DataValue = Enum_PlaybackStorageMedium.CD_DA;
                                break;
                            case "CD-R":
                                args[i].DataValue = Enum_PlaybackStorageMedium.CD_R;
                                break;
                            case "CD-RW":
                                args[i].DataValue = Enum_PlaybackStorageMedium.CD_RW;
                                break;
                            case "VIDEO-CD":
                                args[i].DataValue = Enum_PlaybackStorageMedium.VIDEO_CD;
                                break;
                            case "SACD":
                                args[i].DataValue = Enum_PlaybackStorageMedium.SACD;
                                break;
                            case "MD-AUDIO":
                                args[i].DataValue = Enum_PlaybackStorageMedium.MD_AUDIO;
                                break;
                            case "MD-PICTURE":
                                args[i].DataValue = Enum_PlaybackStorageMedium.MD_PICTURE;
                                break;
                            case "DVD-ROM":
                                args[i].DataValue = Enum_PlaybackStorageMedium.DVD_ROM;
                                break;
                            case "DVD-VIDEO":
                                args[i].DataValue = Enum_PlaybackStorageMedium.DVD_VIDEO;
                                break;
                            case "DVD-R":
                                args[i].DataValue = Enum_PlaybackStorageMedium.DVD_R;
                                break;
                            case "DVD+RW":
                                args[i].DataValue = Enum_PlaybackStorageMedium.DVD_RW;
                                break;
                            case "DVD-RW":
                                args[i].DataValue = Enum_PlaybackStorageMedium.DVD_minus_RW;
                                break;
                            case "DVD-RAM":
                                args[i].DataValue = Enum_PlaybackStorageMedium.DVD_RAM;
                                break;
                            case "DVD-AUDIO":
                                args[i].DataValue = Enum_PlaybackStorageMedium.DVD_AUDIO;
                                break;
                            case "DAT":
                                args[i].DataValue = Enum_PlaybackStorageMedium.DAT;
                                break;
                            case "LD":
                                args[i].DataValue = Enum_PlaybackStorageMedium.LD;
                                break;
                            case "HDD":
                                args[i].DataValue = Enum_PlaybackStorageMedium.HDD;
                                break;
                            case "MICRO-MV":
                                args[i].DataValue = Enum_PlaybackStorageMedium.MICRO_MV;
                                break;
                            case "NETWORK":
                                args[i].DataValue = Enum_PlaybackStorageMedium.NETWORK;
                                break;
                            case "NONE":
                                args[i].DataValue = Enum_PlaybackStorageMedium.NONE;
                                break;
                            case "NOT_IMPLEMENTED":
                                args[i].DataValue = Enum_PlaybackStorageMedium.NOT_IMPLEMENTED;
                                break;
                            case " vendor-defined ":
                                args[i].DataValue = Enum_PlaybackStorageMedium._VENDOR_DEFINED_;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_PlaybackStorageMedium", (string)args[i].DataValue);
                               args[i].DataValue = Enum_PlaybackStorageMedium._UNSPECIFIED_;
                               break;
                        }
                        break;
                    case "RecordMedium":
                        switch((string)args[i].DataValue)
                        {
                            case "UNKNOWN":
                                args[i].DataValue = Enum_RecordStorageMedium.UNKNOWN;
                                break;
                            case "DV":
                                args[i].DataValue = Enum_RecordStorageMedium.DV;
                                break;
                            case "MINI-DV":
                                args[i].DataValue = Enum_RecordStorageMedium.MINI_DV;
                                break;
                            case "VHS":
                                args[i].DataValue = Enum_RecordStorageMedium.VHS;
                                break;
                            case "W-VHS":
                                args[i].DataValue = Enum_RecordStorageMedium.W_VHS;
                                break;
                            case "S-VHS":
                                args[i].DataValue = Enum_RecordStorageMedium.S_VHS;
                                break;
                            case "D-VHS":
                                args[i].DataValue = Enum_RecordStorageMedium.D_VHS;
                                break;
                            case "VHSC":
                                args[i].DataValue = Enum_RecordStorageMedium.VHSC;
                                break;
                            case "VIDEO8":
                                args[i].DataValue = Enum_RecordStorageMedium.VIDEO8;
                                break;
                            case "HI8":
                                args[i].DataValue = Enum_RecordStorageMedium.HI8;
                                break;
                            case "CD-ROM":
                                args[i].DataValue = Enum_RecordStorageMedium.CD_ROM;
                                break;
                            case "CD-DA":
                                args[i].DataValue = Enum_RecordStorageMedium.CD_DA;
                                break;
                            case "CD-R":
                                args[i].DataValue = Enum_RecordStorageMedium.CD_R;
                                break;
                            case "CD-RW":
                                args[i].DataValue = Enum_RecordStorageMedium.CD_RW;
                                break;
                            case "VIDEO-CD":
                                args[i].DataValue = Enum_RecordStorageMedium.VIDEO_CD;
                                break;
                            case "SACD":
                                args[i].DataValue = Enum_RecordStorageMedium.SACD;
                                break;
                            case "MD-AUDIO":
                                args[i].DataValue = Enum_RecordStorageMedium.MD_AUDIO;
                                break;
                            case "MD-PICTURE":
                                args[i].DataValue = Enum_RecordStorageMedium.MD_PICTURE;
                                break;
                            case "DVD-ROM":
                                args[i].DataValue = Enum_RecordStorageMedium.DVD_ROM;
                                break;
                            case "DVD-VIDEO":
                                args[i].DataValue = Enum_RecordStorageMedium.DVD_VIDEO;
                                break;
                            case "DVD-R":
                                args[i].DataValue = Enum_RecordStorageMedium.DVD_R;
                                break;
                            case "DVD+RW":
                                args[i].DataValue = Enum_RecordStorageMedium.DVD_RW;
                                break;
                            case "DVD-RW":
                                args[i].DataValue = Enum_RecordStorageMedium.DVD_minus_RW;
                                break;
                            case "DVD-RAM":
                                args[i].DataValue = Enum_RecordStorageMedium.DVD_RAM;
                                break;
                            case "DVD-AUDIO":
                                args[i].DataValue = Enum_RecordStorageMedium.DVD_AUDIO;
                                break;
                            case "DAT":
                                args[i].DataValue = Enum_RecordStorageMedium.DAT;
                                break;
                            case "LD":
                                args[i].DataValue = Enum_RecordStorageMedium.LD;
                                break;
                            case "HDD":
                                args[i].DataValue = Enum_RecordStorageMedium.HDD;
                                break;
                            case "MICRO-MV":
                                args[i].DataValue = Enum_RecordStorageMedium.MICRO_MV;
                                break;
                            case "NETWORK":
                                args[i].DataValue = Enum_RecordStorageMedium.NETWORK;
                                break;
                            case "NONE":
                                args[i].DataValue = Enum_RecordStorageMedium.NONE;
                                break;
                            case "NOT_IMPLEMENTED":
                                args[i].DataValue = Enum_RecordStorageMedium.NOT_IMPLEMENTED;
                                break;
                            case " vendor-defined ":
                                args[i].DataValue = Enum_RecordStorageMedium._VENDOR_DEFINED_;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_RecordStorageMedium", (string)args[i].DataValue);
                               args[i].DataValue = Enum_RecordStorageMedium._UNSPECIFIED_;
                               break;
                        }
                        break;
                    case "WriteStatus":
                        switch((string)args[i].DataValue)
                        {
                            case "WRITABLE":
                                args[i].DataValue = Enum_RecordMediumWriteStatus.WRITABLE;
                                break;
                            case "PROTECTED":
                                args[i].DataValue = Enum_RecordMediumWriteStatus.PROTECTED;
                                break;
                            case "NOT_WRITABLE":
                                args[i].DataValue = Enum_RecordMediumWriteStatus.NOT_WRITABLE;
                                break;
                            case "UNKNOWN":
                                args[i].DataValue = Enum_RecordMediumWriteStatus.UNKNOWN;
                                break;
                            case "NOT_IMPLEMENTED":
                                args[i].DataValue = Enum_RecordMediumWriteStatus.NOT_IMPLEMENTED;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_RecordMediumWriteStatus", (string)args[i].DataValue);
                               args[i].DataValue = Enum_RecordMediumWriteStatus._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            InstanceID = (System.UInt32) args[0].DataValue;
            NrTracks = (System.UInt32) args[1].DataValue;
            MediaDuration = (System.String) args[2].DataValue;
            CurrentURI = (System.String) args[3].DataValue;
            CurrentURIMetaData = (System.String) args[4].DataValue;
            NextURI = (System.String) args[5].DataValue;
            NextURIMetaData = (System.String) args[6].DataValue;
            PlayMedium = (Enum_PlaybackStorageMedium) args[7].DataValue;
            RecordMedium = (Enum_RecordStorageMedium) args[8].DataValue;
            WriteStatus = (Enum_RecordMediumWriteStatus) args[9].DataValue;
            return;
        }
        public int GetMediaInfo(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[10];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("NrTracks", "");
           args[2] = new UPnPArgument("MediaDuration", "");
           args[3] = new UPnPArgument("CurrentURI", "");
           args[4] = new UPnPArgument("CurrentURIMetaData", "");
           args[5] = new UPnPArgument("NextURI", "");
           args[6] = new UPnPArgument("NextURIMetaData", "");
           args[7] = new UPnPArgument("PlayMedium", "");
           args[8] = new UPnPArgument("RecordMedium", "");
           args[9] = new UPnPArgument("WriteStatus", "");
           return(_S.InvokeAsync("GetMediaInfo", args, new UPnPService.UPnPServiceInvokeHandler(Sink_GetMediaInfo), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetMediaInfo)));
        }
        private void Sink_GetMediaInfo(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "PlayMedium":
                        switch((string)Args[i].DataValue)
                        {
                            case "UNKNOWN":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.UNKNOWN;
                                break;
                            case "DV":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.DV;
                                break;
                            case "MINI-DV":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.MINI_DV;
                                break;
                            case "VHS":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.VHS;
                                break;
                            case "W-VHS":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.W_VHS;
                                break;
                            case "S-VHS":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.S_VHS;
                                break;
                            case "D-VHS":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.D_VHS;
                                break;
                            case "VHSC":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.VHSC;
                                break;
                            case "VIDEO8":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.VIDEO8;
                                break;
                            case "HI8":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.HI8;
                                break;
                            case "CD-ROM":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.CD_ROM;
                                break;
                            case "CD-DA":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.CD_DA;
                                break;
                            case "CD-R":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.CD_R;
                                break;
                            case "CD-RW":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.CD_RW;
                                break;
                            case "VIDEO-CD":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.VIDEO_CD;
                                break;
                            case "SACD":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.SACD;
                                break;
                            case "MD-AUDIO":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.MD_AUDIO;
                                break;
                            case "MD-PICTURE":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.MD_PICTURE;
                                break;
                            case "DVD-ROM":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.DVD_ROM;
                                break;
                            case "DVD-VIDEO":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.DVD_VIDEO;
                                break;
                            case "DVD-R":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.DVD_R;
                                break;
                            case "DVD+RW":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.DVD_RW;
                                break;
                            case "DVD-RW":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.DVD_minus_RW;
                                break;
                            case "DVD-RAM":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.DVD_RAM;
                                break;
                            case "DVD-AUDIO":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.DVD_AUDIO;
                                break;
                            case "DAT":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.DAT;
                                break;
                            case "LD":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.LD;
                                break;
                            case "HDD":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.HDD;
                                break;
                            case "MICRO-MV":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.MICRO_MV;
                                break;
                            case "NETWORK":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.NETWORK;
                                break;
                            case "NONE":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.NONE;
                                break;
                            case "NOT_IMPLEMENTED":
                                Args[i].DataValue = Enum_PlaybackStorageMedium.NOT_IMPLEMENTED;
                                break;
                            case " vendor-defined ":
                                Args[i].DataValue = Enum_PlaybackStorageMedium._VENDOR_DEFINED_;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_PlaybackStorageMedium", (string)Args[i].DataValue);
                               Args[i].DataValue = Enum_PlaybackStorageMedium._UNSPECIFIED_;
                               break;
                        }
                        break;
                    case "RecordMedium":
                        switch((string)Args[i].DataValue)
                        {
                            case "UNKNOWN":
                                Args[i].DataValue = Enum_RecordStorageMedium.UNKNOWN;
                                break;
                            case "DV":
                                Args[i].DataValue = Enum_RecordStorageMedium.DV;
                                break;
                            case "MINI-DV":
                                Args[i].DataValue = Enum_RecordStorageMedium.MINI_DV;
                                break;
                            case "VHS":
                                Args[i].DataValue = Enum_RecordStorageMedium.VHS;
                                break;
                            case "W-VHS":
                                Args[i].DataValue = Enum_RecordStorageMedium.W_VHS;
                                break;
                            case "S-VHS":
                                Args[i].DataValue = Enum_RecordStorageMedium.S_VHS;
                                break;
                            case "D-VHS":
                                Args[i].DataValue = Enum_RecordStorageMedium.D_VHS;
                                break;
                            case "VHSC":
                                Args[i].DataValue = Enum_RecordStorageMedium.VHSC;
                                break;
                            case "VIDEO8":
                                Args[i].DataValue = Enum_RecordStorageMedium.VIDEO8;
                                break;
                            case "HI8":
                                Args[i].DataValue = Enum_RecordStorageMedium.HI8;
                                break;
                            case "CD-ROM":
                                Args[i].DataValue = Enum_RecordStorageMedium.CD_ROM;
                                break;
                            case "CD-DA":
                                Args[i].DataValue = Enum_RecordStorageMedium.CD_DA;
                                break;
                            case "CD-R":
                                Args[i].DataValue = Enum_RecordStorageMedium.CD_R;
                                break;
                            case "CD-RW":
                                Args[i].DataValue = Enum_RecordStorageMedium.CD_RW;
                                break;
                            case "VIDEO-CD":
                                Args[i].DataValue = Enum_RecordStorageMedium.VIDEO_CD;
                                break;
                            case "SACD":
                                Args[i].DataValue = Enum_RecordStorageMedium.SACD;
                                break;
                            case "MD-AUDIO":
                                Args[i].DataValue = Enum_RecordStorageMedium.MD_AUDIO;
                                break;
                            case "MD-PICTURE":
                                Args[i].DataValue = Enum_RecordStorageMedium.MD_PICTURE;
                                break;
                            case "DVD-ROM":
                                Args[i].DataValue = Enum_RecordStorageMedium.DVD_ROM;
                                break;
                            case "DVD-VIDEO":
                                Args[i].DataValue = Enum_RecordStorageMedium.DVD_VIDEO;
                                break;
                            case "DVD-R":
                                Args[i].DataValue = Enum_RecordStorageMedium.DVD_R;
                                break;
                            case "DVD+RW":
                                Args[i].DataValue = Enum_RecordStorageMedium.DVD_RW;
                                break;
                            case "DVD-RW":
                                Args[i].DataValue = Enum_RecordStorageMedium.DVD_minus_RW;
                                break;
                            case "DVD-RAM":
                                Args[i].DataValue = Enum_RecordStorageMedium.DVD_RAM;
                                break;
                            case "DVD-AUDIO":
                                Args[i].DataValue = Enum_RecordStorageMedium.DVD_AUDIO;
                                break;
                            case "DAT":
                                Args[i].DataValue = Enum_RecordStorageMedium.DAT;
                                break;
                            case "LD":
                                Args[i].DataValue = Enum_RecordStorageMedium.LD;
                                break;
                            case "HDD":
                                Args[i].DataValue = Enum_RecordStorageMedium.HDD;
                                break;
                            case "MICRO-MV":
                                Args[i].DataValue = Enum_RecordStorageMedium.MICRO_MV;
                                break;
                            case "NETWORK":
                                Args[i].DataValue = Enum_RecordStorageMedium.NETWORK;
                                break;
                            case "NONE":
                                Args[i].DataValue = Enum_RecordStorageMedium.NONE;
                                break;
                            case "NOT_IMPLEMENTED":
                                Args[i].DataValue = Enum_RecordStorageMedium.NOT_IMPLEMENTED;
                                break;
                            case " vendor-defined ":
                                Args[i].DataValue = Enum_RecordStorageMedium._VENDOR_DEFINED_;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_RecordStorageMedium", (string)Args[i].DataValue);
                               Args[i].DataValue = Enum_RecordStorageMedium._UNSPECIFIED_;
                               break;
                        }
                        break;
                    case "WriteStatus":
                        switch((string)Args[i].DataValue)
                        {
                            case "WRITABLE":
                                Args[i].DataValue = Enum_RecordMediumWriteStatus.WRITABLE;
                                break;
                            case "PROTECTED":
                                Args[i].DataValue = Enum_RecordMediumWriteStatus.PROTECTED;
                                break;
                            case "NOT_WRITABLE":
                                Args[i].DataValue = Enum_RecordMediumWriteStatus.NOT_WRITABLE;
                                break;
                            case "UNKNOWN":
                                Args[i].DataValue = Enum_RecordMediumWriteStatus.UNKNOWN;
                                break;
                            case "NOT_IMPLEMENTED":
                                Args[i].DataValue = Enum_RecordMediumWriteStatus.NOT_IMPLEMENTED;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_RecordMediumWriteStatus", (string)Args[i].DataValue);
                               Args[i].DataValue = Enum_RecordMediumWriteStatus._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            if(OnResult_GetMediaInfo != null)
            {
               OnResult_GetMediaInfo((System.UInt32 )Args[0].DataValue, (System.UInt32 )Args[1].DataValue, (System.String )Args[2].DataValue, (System.String )Args[3].DataValue, (System.String )Args[4].DataValue, (System.String )Args[5].DataValue, (System.String )Args[6].DataValue, (Enum_PlaybackStorageMedium )Args[7].DataValue, (Enum_RecordStorageMedium )Args[8].DataValue, (Enum_RecordMediumWriteStatus )Args[9].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetMediaInfo.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetMediaInfo)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt32 )Args[1].DataValue, (System.String )Args[2].DataValue, (System.String )Args[3].DataValue, (System.String )Args[4].DataValue, (System.String )Args[5].DataValue, (System.String )Args[6].DataValue, (Enum_PlaybackStorageMedium )Args[7].DataValue, (Enum_RecordStorageMedium )Args[8].DataValue, (Enum_RecordMediumWriteStatus )Args[9].DataValue, null, Handle);
                }
                else
                {
                    WeakList_GetMediaInfo.Remove(wr);
                }
            }
        }
        private void Error_Sink_GetMediaInfo(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_GetMediaInfo != null)
            {
                 OnResult_GetMediaInfo((System.UInt32 )Args[0].DataValue, (System.UInt32)UPnPService.CreateObjectInstance(typeof(System.UInt32),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (Enum_PlaybackStorageMedium)0, (Enum_RecordStorageMedium)0, (Enum_RecordMediumWriteStatus)0, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetMediaInfo.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetMediaInfo)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt32)UPnPService.CreateObjectInstance(typeof(System.UInt32),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (Enum_PlaybackStorageMedium)0, (Enum_RecordStorageMedium)0, (Enum_RecordMediumWriteStatus)0, e, Handle);
                }
                else
                {
                    WeakList_GetMediaInfo.Remove(wr);
                }
            }
        }
        public void Sync_GetDeviceCapabilities(System.UInt32 InstanceID, out System.String PlayMedia, out System.String RecMedia, out System.String RecQualityModes)
        {
           UPnPArgument[] args = new UPnPArgument[4];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("PlayMedia", "");
           args[2] = new UPnPArgument("RecMedia", "");
           args[3] = new UPnPArgument("RecQualityModes", "");
            _S.InvokeSync("GetDeviceCapabilities", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            PlayMedia = (System.String) args[1].DataValue;
            RecMedia = (System.String) args[2].DataValue;
            RecQualityModes = (System.String) args[3].DataValue;
            return;
        }
        public int GetDeviceCapabilities(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[4];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("PlayMedia", "");
           args[2] = new UPnPArgument("RecMedia", "");
           args[3] = new UPnPArgument("RecQualityModes", "");
           return(_S.InvokeAsync("GetDeviceCapabilities", args, new UPnPService.UPnPServiceInvokeHandler(Sink_GetDeviceCapabilities), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetDeviceCapabilities)));
        }
        private void Sink_GetDeviceCapabilities(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_GetDeviceCapabilities != null)
            {
               OnResult_GetDeviceCapabilities((System.UInt32 )Args[0].DataValue, (System.String )Args[1].DataValue, (System.String )Args[2].DataValue, (System.String )Args[3].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetDeviceCapabilities.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetDeviceCapabilities)wr.Target)((System.UInt32 )Args[0].DataValue, (System.String )Args[1].DataValue, (System.String )Args[2].DataValue, (System.String )Args[3].DataValue, null, Handle);
                }
                else
                {
                    WeakList_GetDeviceCapabilities.Remove(wr);
                }
            }
        }
        private void Error_Sink_GetDeviceCapabilities(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_GetDeviceCapabilities != null)
            {
                 OnResult_GetDeviceCapabilities((System.UInt32 )Args[0].DataValue, (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetDeviceCapabilities.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetDeviceCapabilities)wr.Target)((System.UInt32 )Args[0].DataValue, (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), e, Handle);
                }
                else
                {
                    WeakList_GetDeviceCapabilities.Remove(wr);
                }
            }
        }
        public void Sync_SetAVTransportURI(System.UInt32 InstanceID, System.String CurrentURI, System.String CurrentURIMetaData)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentURI", CurrentURI);
           args[2] = new UPnPArgument("CurrentURIMetaData", CurrentURIMetaData);
            _S.InvokeSync("SetAVTransportURI", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            CurrentURI = (System.String) args[1].DataValue;
            CurrentURIMetaData = (System.String) args[2].DataValue;
            return;
        }
        public int SetAVTransportURI(System.UInt32 InstanceID, System.String CurrentURI, System.String CurrentURIMetaData)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentURI", CurrentURI);
           args[2] = new UPnPArgument("CurrentURIMetaData", CurrentURIMetaData);
           return(_S.InvokeAsync("SetAVTransportURI", args, new UPnPService.UPnPServiceInvokeHandler(Sink_SetAVTransportURI), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetAVTransportURI)));
        }
        private void Sink_SetAVTransportURI(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_SetAVTransportURI != null)
            {
               OnResult_SetAVTransportURI((System.UInt32 )Args[0].DataValue, (System.String )Args[1].DataValue, (System.String )Args[2].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetAVTransportURI.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetAVTransportURI)wr.Target)((System.UInt32 )Args[0].DataValue, (System.String )Args[1].DataValue, (System.String )Args[2].DataValue, null, Handle);
                }
                else
                {
                    WeakList_SetAVTransportURI.Remove(wr);
                }
            }
        }
        private void Error_Sink_SetAVTransportURI(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_SetAVTransportURI != null)
            {
                 OnResult_SetAVTransportURI((System.UInt32 )Args[0].DataValue, (System.String )Args[1].DataValue, (System.String )Args[2].DataValue, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetAVTransportURI.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetAVTransportURI)wr.Target)((System.UInt32 )Args[0].DataValue, (System.String )Args[1].DataValue, (System.String )Args[2].DataValue, e, Handle);
                }
                else
                {
                    WeakList_SetAVTransportURI.Remove(wr);
                }
            }
        }
        public void Sync_SetPlayMode(System.UInt32 InstanceID, Enum_CurrentPlayMode NewPlayMode)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           switch(NewPlayMode)
           {
               case Enum_CurrentPlayMode.NORMAL:
                   args[1] = new UPnPArgument("NewPlayMode", "NORMAL");
                   break;
               case Enum_CurrentPlayMode.SHUFFLE:
                   args[1] = new UPnPArgument("NewPlayMode", "SHUFFLE");
                   break;
               case Enum_CurrentPlayMode.REPEAT_ONE:
                   args[1] = new UPnPArgument("NewPlayMode", "REPEAT_ONE");
                   break;
               case Enum_CurrentPlayMode.REPEAT_ALL:
                   args[1] = new UPnPArgument("NewPlayMode", "REPEAT_ALL");
                   break;
               case Enum_CurrentPlayMode.RANDOM:
                   args[1] = new UPnPArgument("NewPlayMode", "RANDOM");
                   break;
               case Enum_CurrentPlayMode.DIRECT_1:
                   args[1] = new UPnPArgument("NewPlayMode", "DIRECT_1");
                   break;
               case Enum_CurrentPlayMode.INTRO:
                   args[1] = new UPnPArgument("NewPlayMode", "INTRO");
                   break;
               default:
                  args[1] = new UPnPArgument("NewPlayMode", GetUnspecifiedValue("Enum_CurrentPlayMode"));
                  break;
           }
            _S.InvokeSync("SetPlayMode", args);
            for(int i=0;i<args.Length;++i)
            {
                switch(args[i].Name)
                {
                    case "NewPlayMode":
                        switch((string)args[i].DataValue)
                        {
                            case "NORMAL":
                                args[i].DataValue = Enum_CurrentPlayMode.NORMAL;
                                break;
                            case "SHUFFLE":
                                args[i].DataValue = Enum_CurrentPlayMode.SHUFFLE;
                                break;
                            case "REPEAT_ONE":
                                args[i].DataValue = Enum_CurrentPlayMode.REPEAT_ONE;
                                break;
                            case "REPEAT_ALL":
                                args[i].DataValue = Enum_CurrentPlayMode.REPEAT_ALL;
                                break;
                            case "RANDOM":
                                args[i].DataValue = Enum_CurrentPlayMode.RANDOM;
                                break;
                            case "DIRECT_1":
                                args[i].DataValue = Enum_CurrentPlayMode.DIRECT_1;
                                break;
                            case "INTRO":
                                args[i].DataValue = Enum_CurrentPlayMode.INTRO;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_CurrentPlayMode", (string)args[i].DataValue);
                               args[i].DataValue = Enum_CurrentPlayMode._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            InstanceID = (System.UInt32) args[0].DataValue;
            NewPlayMode = (Enum_CurrentPlayMode) args[1].DataValue;
            return;
        }
        public int SetPlayMode(System.UInt32 InstanceID, Enum_CurrentPlayMode NewPlayMode)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           switch(NewPlayMode)
           {
               case Enum_CurrentPlayMode.NORMAL:
                   args[1] = new UPnPArgument("NewPlayMode", "NORMAL");
                   break;
               case Enum_CurrentPlayMode.SHUFFLE:
                   args[1] = new UPnPArgument("NewPlayMode", "SHUFFLE");
                   break;
               case Enum_CurrentPlayMode.REPEAT_ONE:
                   args[1] = new UPnPArgument("NewPlayMode", "REPEAT_ONE");
                   break;
               case Enum_CurrentPlayMode.REPEAT_ALL:
                   args[1] = new UPnPArgument("NewPlayMode", "REPEAT_ALL");
                   break;
               case Enum_CurrentPlayMode.RANDOM:
                   args[1] = new UPnPArgument("NewPlayMode", "RANDOM");
                   break;
               case Enum_CurrentPlayMode.DIRECT_1:
                   args[1] = new UPnPArgument("NewPlayMode", "DIRECT_1");
                   break;
               case Enum_CurrentPlayMode.INTRO:
                   args[1] = new UPnPArgument("NewPlayMode", "INTRO");
                   break;
               default:
                  args[1] = new UPnPArgument("NewPlayMode", GetUnspecifiedValue("Enum_CurrentPlayMode"));
                  break;
           }
           return(_S.InvokeAsync("SetPlayMode", args, new UPnPService.UPnPServiceInvokeHandler(Sink_SetPlayMode), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetPlayMode)));
        }
        private void Sink_SetPlayMode(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "NewPlayMode":
                        switch((string)Args[i].DataValue)
                        {
                            case "NORMAL":
                                Args[i].DataValue = Enum_CurrentPlayMode.NORMAL;
                                break;
                            case "SHUFFLE":
                                Args[i].DataValue = Enum_CurrentPlayMode.SHUFFLE;
                                break;
                            case "REPEAT_ONE":
                                Args[i].DataValue = Enum_CurrentPlayMode.REPEAT_ONE;
                                break;
                            case "REPEAT_ALL":
                                Args[i].DataValue = Enum_CurrentPlayMode.REPEAT_ALL;
                                break;
                            case "RANDOM":
                                Args[i].DataValue = Enum_CurrentPlayMode.RANDOM;
                                break;
                            case "DIRECT_1":
                                Args[i].DataValue = Enum_CurrentPlayMode.DIRECT_1;
                                break;
                            case "INTRO":
                                Args[i].DataValue = Enum_CurrentPlayMode.INTRO;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_CurrentPlayMode", (string)Args[i].DataValue);
                               Args[i].DataValue = Enum_CurrentPlayMode._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            if(OnResult_SetPlayMode != null)
            {
               OnResult_SetPlayMode((System.UInt32 )Args[0].DataValue, (Enum_CurrentPlayMode )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetPlayMode.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetPlayMode)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_CurrentPlayMode )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_SetPlayMode.Remove(wr);
                }
            }
        }
        private void Error_Sink_SetPlayMode(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "NewPlayMode":
                        switch((string)Args[i].DataValue)
                        {
                            case "NORMAL":
                                Args[i].DataValue = Enum_CurrentPlayMode.NORMAL;
                                break;
                            case "SHUFFLE":
                                Args[i].DataValue = Enum_CurrentPlayMode.SHUFFLE;
                                break;
                            case "REPEAT_ONE":
                                Args[i].DataValue = Enum_CurrentPlayMode.REPEAT_ONE;
                                break;
                            case "REPEAT_ALL":
                                Args[i].DataValue = Enum_CurrentPlayMode.REPEAT_ALL;
                                break;
                            case "RANDOM":
                                Args[i].DataValue = Enum_CurrentPlayMode.RANDOM;
                                break;
                            case "DIRECT_1":
                                Args[i].DataValue = Enum_CurrentPlayMode.DIRECT_1;
                                break;
                            case "INTRO":
                                Args[i].DataValue = Enum_CurrentPlayMode.INTRO;
                                break;
                        }
                        break;
                }
            }
            if(OnResult_SetPlayMode != null)
            {
                 OnResult_SetPlayMode((System.UInt32 )Args[0].DataValue, (Enum_CurrentPlayMode )Args[1].DataValue, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetPlayMode.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetPlayMode)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_CurrentPlayMode )Args[1].DataValue, e, Handle);
                }
                else
                {
                    WeakList_SetPlayMode.Remove(wr);
                }
            }
        }
        public void Sync_GetCurrentTransportActions(System.UInt32 InstanceID, out System.String Actions)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("Actions", "");
            _S.InvokeSync("GetCurrentTransportActions", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            Actions = (System.String) args[1].DataValue;
            return;
        }
        public int GetCurrentTransportActions(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("Actions", "");
           return(_S.InvokeAsync("GetCurrentTransportActions", args, new UPnPService.UPnPServiceInvokeHandler(Sink_GetCurrentTransportActions), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetCurrentTransportActions)));
        }
        private void Sink_GetCurrentTransportActions(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_GetCurrentTransportActions != null)
            {
               OnResult_GetCurrentTransportActions((System.UInt32 )Args[0].DataValue, (System.String )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetCurrentTransportActions.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetCurrentTransportActions)wr.Target)((System.UInt32 )Args[0].DataValue, (System.String )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_GetCurrentTransportActions.Remove(wr);
                }
            }
        }
        private void Error_Sink_GetCurrentTransportActions(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_GetCurrentTransportActions != null)
            {
                 OnResult_GetCurrentTransportActions((System.UInt32 )Args[0].DataValue, (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetCurrentTransportActions.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetCurrentTransportActions)wr.Target)((System.UInt32 )Args[0].DataValue, (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), e, Handle);
                }
                else
                {
                    WeakList_GetCurrentTransportActions.Remove(wr);
                }
            }
        }
        public void Sync_GetTransportSettings(System.UInt32 InstanceID, out Enum_CurrentPlayMode PlayMode, out Enum_CurrentRecordQualityMode RecQualityMode)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("PlayMode", "");
           args[2] = new UPnPArgument("RecQualityMode", "");
            _S.InvokeSync("GetTransportSettings", args);
            for(int i=0;i<args.Length;++i)
            {
                switch(args[i].Name)
                {
                    case "PlayMode":
                        switch((string)args[i].DataValue)
                        {
                            case "NORMAL":
                                args[i].DataValue = Enum_CurrentPlayMode.NORMAL;
                                break;
                            case "SHUFFLE":
                                args[i].DataValue = Enum_CurrentPlayMode.SHUFFLE;
                                break;
                            case "REPEAT_ONE":
                                args[i].DataValue = Enum_CurrentPlayMode.REPEAT_ONE;
                                break;
                            case "REPEAT_ALL":
                                args[i].DataValue = Enum_CurrentPlayMode.REPEAT_ALL;
                                break;
                            case "RANDOM":
                                args[i].DataValue = Enum_CurrentPlayMode.RANDOM;
                                break;
                            case "DIRECT_1":
                                args[i].DataValue = Enum_CurrentPlayMode.DIRECT_1;
                                break;
                            case "INTRO":
                                args[i].DataValue = Enum_CurrentPlayMode.INTRO;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_CurrentPlayMode", (string)args[i].DataValue);
                               args[i].DataValue = Enum_CurrentPlayMode._UNSPECIFIED_;
                               break;
                        }
                        break;
                    case "RecQualityMode":
                        switch((string)args[i].DataValue)
                        {
                            case "0:EP":
                                args[i].DataValue = Enum_CurrentRecordQualityMode._0_EP;
                                break;
                            case "1:LP":
                                args[i].DataValue = Enum_CurrentRecordQualityMode._1_LP;
                                break;
                            case "2:SP":
                                args[i].DataValue = Enum_CurrentRecordQualityMode._2_SP;
                                break;
                            case "0:BASIC":
                                args[i].DataValue = Enum_CurrentRecordQualityMode._0_BASIC;
                                break;
                            case "1:MEDIUM":
                                args[i].DataValue = Enum_CurrentRecordQualityMode._1_MEDIUM;
                                break;
                            case "2:HIGH":
                                args[i].DataValue = Enum_CurrentRecordQualityMode._2_HIGH;
                                break;
                            case "NOT_IMPLEMENTED":
                                args[i].DataValue = Enum_CurrentRecordQualityMode.NOT_IMPLEMENTED;
                                break;
                            case " vendor-defined ":
                                args[i].DataValue = Enum_CurrentRecordQualityMode._VENDOR_DEFINED_;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_CurrentRecordQualityMode", (string)args[i].DataValue);
                               args[i].DataValue = Enum_CurrentRecordQualityMode._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            InstanceID = (System.UInt32) args[0].DataValue;
            PlayMode = (Enum_CurrentPlayMode) args[1].DataValue;
            RecQualityMode = (Enum_CurrentRecordQualityMode) args[2].DataValue;
            return;
        }
        public int GetTransportSettings(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("PlayMode", "");
           args[2] = new UPnPArgument("RecQualityMode", "");
           return(_S.InvokeAsync("GetTransportSettings", args, new UPnPService.UPnPServiceInvokeHandler(Sink_GetTransportSettings), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetTransportSettings)));
        }
        private void Sink_GetTransportSettings(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "PlayMode":
                        switch((string)Args[i].DataValue)
                        {
                            case "NORMAL":
                                Args[i].DataValue = Enum_CurrentPlayMode.NORMAL;
                                break;
                            case "SHUFFLE":
                                Args[i].DataValue = Enum_CurrentPlayMode.SHUFFLE;
                                break;
                            case "REPEAT_ONE":
                                Args[i].DataValue = Enum_CurrentPlayMode.REPEAT_ONE;
                                break;
                            case "REPEAT_ALL":
                                Args[i].DataValue = Enum_CurrentPlayMode.REPEAT_ALL;
                                break;
                            case "RANDOM":
                                Args[i].DataValue = Enum_CurrentPlayMode.RANDOM;
                                break;
                            case "DIRECT_1":
                                Args[i].DataValue = Enum_CurrentPlayMode.DIRECT_1;
                                break;
                            case "INTRO":
                                Args[i].DataValue = Enum_CurrentPlayMode.INTRO;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_CurrentPlayMode", (string)Args[i].DataValue);
                               Args[i].DataValue = Enum_CurrentPlayMode._UNSPECIFIED_;
                               break;
                        }
                        break;
                    case "RecQualityMode":
                        switch((string)Args[i].DataValue)
                        {
                            case "0:EP":
                                Args[i].DataValue = Enum_CurrentRecordQualityMode._0_EP;
                                break;
                            case "1:LP":
                                Args[i].DataValue = Enum_CurrentRecordQualityMode._1_LP;
                                break;
                            case "2:SP":
                                Args[i].DataValue = Enum_CurrentRecordQualityMode._2_SP;
                                break;
                            case "0:BASIC":
                                Args[i].DataValue = Enum_CurrentRecordQualityMode._0_BASIC;
                                break;
                            case "1:MEDIUM":
                                Args[i].DataValue = Enum_CurrentRecordQualityMode._1_MEDIUM;
                                break;
                            case "2:HIGH":
                                Args[i].DataValue = Enum_CurrentRecordQualityMode._2_HIGH;
                                break;
                            case "NOT_IMPLEMENTED":
                                Args[i].DataValue = Enum_CurrentRecordQualityMode.NOT_IMPLEMENTED;
                                break;
                            case " vendor-defined ":
                                Args[i].DataValue = Enum_CurrentRecordQualityMode._VENDOR_DEFINED_;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_CurrentRecordQualityMode", (string)Args[i].DataValue);
                               Args[i].DataValue = Enum_CurrentRecordQualityMode._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            if(OnResult_GetTransportSettings != null)
            {
               OnResult_GetTransportSettings((System.UInt32 )Args[0].DataValue, (Enum_CurrentPlayMode )Args[1].DataValue, (Enum_CurrentRecordQualityMode )Args[2].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetTransportSettings.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetTransportSettings)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_CurrentPlayMode )Args[1].DataValue, (Enum_CurrentRecordQualityMode )Args[2].DataValue, null, Handle);
                }
                else
                {
                    WeakList_GetTransportSettings.Remove(wr);
                }
            }
        }
        private void Error_Sink_GetTransportSettings(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_GetTransportSettings != null)
            {
                 OnResult_GetTransportSettings((System.UInt32 )Args[0].DataValue, (Enum_CurrentPlayMode)0, (Enum_CurrentRecordQualityMode)0, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetTransportSettings.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetTransportSettings)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_CurrentPlayMode)0, (Enum_CurrentRecordQualityMode)0, e, Handle);
                }
                else
                {
                    WeakList_GetTransportSettings.Remove(wr);
                }
            }
        }
        public void Sync_GetTransportInfo(System.UInt32 InstanceID, out Enum_TransportState CurrentTransportState, out Enum_TransportStatus CurrentTransportStatus, out Enum_TransportPlaySpeed CurrentSpeed)
        {
           UPnPArgument[] args = new UPnPArgument[4];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentTransportState", "");
           args[2] = new UPnPArgument("CurrentTransportStatus", "");
           args[3] = new UPnPArgument("CurrentSpeed", "");
            _S.InvokeSync("GetTransportInfo", args);
            for(int i=0;i<args.Length;++i)
            {
                switch(args[i].Name)
                {
                    case "CurrentTransportState":
                        switch((string)args[i].DataValue)
                        {
                            case "STOPPED":
                                args[i].DataValue = Enum_TransportState.STOPPED;
                                break;
                            case "PAUSED_PLAYBACK":
                                args[i].DataValue = Enum_TransportState.PAUSED_PLAYBACK;
                                break;
                            case "PAUSED_RECORDING":
                                args[i].DataValue = Enum_TransportState.PAUSED_RECORDING;
                                break;
                            case "PLAYING":
                                args[i].DataValue = Enum_TransportState.PLAYING;
                                break;
                            case "RECORDING":
                                args[i].DataValue = Enum_TransportState.RECORDING;
                                break;
                            case "TRANSITIONING":
                                args[i].DataValue = Enum_TransportState.TRANSITIONING;
                                break;
                            case "NO_MEDIA_PRESENT":
                                args[i].DataValue = Enum_TransportState.NO_MEDIA_PRESENT;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_TransportState", (string)args[i].DataValue);
                               args[i].DataValue = Enum_TransportState._UNSPECIFIED_;
                               break;
                        }
                        break;
                    case "CurrentTransportStatus":
                        switch((string)args[i].DataValue)
                        {
                            case "OK":
                                args[i].DataValue = Enum_TransportStatus.OK;
                                break;
                            case "ERROR_OCCURRED":
                                args[i].DataValue = Enum_TransportStatus.ERROR_OCCURRED;
                                break;
                            case " vendor-defined ":
                                args[i].DataValue = Enum_TransportStatus._VENDOR_DEFINED_;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_TransportStatus", (string)args[i].DataValue);
                               args[i].DataValue = Enum_TransportStatus._UNSPECIFIED_;
                               break;
                        }
                        break;
                    case "CurrentSpeed":
                        switch((string)args[i].DataValue)
                        {
                            case "1":
                                args[i].DataValue = Enum_TransportPlaySpeed._1;
                                break;
                            case " vendor-defined ":
                                args[i].DataValue = Enum_TransportPlaySpeed._VENDOR_DEFINED_;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_TransportPlaySpeed", (string)args[i].DataValue);
                               args[i].DataValue = Enum_TransportPlaySpeed._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            InstanceID = (System.UInt32) args[0].DataValue;
            CurrentTransportState = (Enum_TransportState) args[1].DataValue;
            CurrentTransportStatus = (Enum_TransportStatus) args[2].DataValue;
            CurrentSpeed = (Enum_TransportPlaySpeed) args[3].DataValue;
            return;
        }
        public int GetTransportInfo(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[4];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentTransportState", "");
           args[2] = new UPnPArgument("CurrentTransportStatus", "");
           args[3] = new UPnPArgument("CurrentSpeed", "");
           return(_S.InvokeAsync("GetTransportInfo", args, new UPnPService.UPnPServiceInvokeHandler(Sink_GetTransportInfo), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetTransportInfo)));
        }
        private void Sink_GetTransportInfo(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "CurrentTransportState":
                        switch((string)Args[i].DataValue)
                        {
                            case "STOPPED":
                                Args[i].DataValue = Enum_TransportState.STOPPED;
                                break;
                            case "PAUSED_PLAYBACK":
                                Args[i].DataValue = Enum_TransportState.PAUSED_PLAYBACK;
                                break;
                            case "PAUSED_RECORDING":
                                Args[i].DataValue = Enum_TransportState.PAUSED_RECORDING;
                                break;
                            case "PLAYING":
                                Args[i].DataValue = Enum_TransportState.PLAYING;
                                break;
                            case "RECORDING":
                                Args[i].DataValue = Enum_TransportState.RECORDING;
                                break;
                            case "TRANSITIONING":
                                Args[i].DataValue = Enum_TransportState.TRANSITIONING;
                                break;
                            case "NO_MEDIA_PRESENT":
                                Args[i].DataValue = Enum_TransportState.NO_MEDIA_PRESENT;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_TransportState", (string)Args[i].DataValue);
                               Args[i].DataValue = Enum_TransportState._UNSPECIFIED_;
                               break;
                        }
                        break;
                    case "CurrentTransportStatus":
                        switch((string)Args[i].DataValue)
                        {
                            case "OK":
                                Args[i].DataValue = Enum_TransportStatus.OK;
                                break;
                            case "ERROR_OCCURRED":
                                Args[i].DataValue = Enum_TransportStatus.ERROR_OCCURRED;
                                break;
                            case " vendor-defined ":
                                Args[i].DataValue = Enum_TransportStatus._VENDOR_DEFINED_;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_TransportStatus", (string)Args[i].DataValue);
                               Args[i].DataValue = Enum_TransportStatus._UNSPECIFIED_;
                               break;
                        }
                        break;
                    case "CurrentSpeed":
                        switch((string)Args[i].DataValue)
                        {
                            case "1":
                                Args[i].DataValue = Enum_TransportPlaySpeed._1;
                                break;
                            case " vendor-defined ":
                                Args[i].DataValue = Enum_TransportPlaySpeed._VENDOR_DEFINED_;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_TransportPlaySpeed", (string)Args[i].DataValue);
                               Args[i].DataValue = Enum_TransportPlaySpeed._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            if(OnResult_GetTransportInfo != null)
            {
               OnResult_GetTransportInfo((System.UInt32 )Args[0].DataValue, (Enum_TransportState )Args[1].DataValue, (Enum_TransportStatus )Args[2].DataValue, (Enum_TransportPlaySpeed )Args[3].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetTransportInfo.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetTransportInfo)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_TransportState )Args[1].DataValue, (Enum_TransportStatus )Args[2].DataValue, (Enum_TransportPlaySpeed )Args[3].DataValue, null, Handle);
                }
                else
                {
                    WeakList_GetTransportInfo.Remove(wr);
                }
            }
        }
        private void Error_Sink_GetTransportInfo(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_GetTransportInfo != null)
            {
                 OnResult_GetTransportInfo((System.UInt32 )Args[0].DataValue, (Enum_TransportState)0, (Enum_TransportStatus)0, (Enum_TransportPlaySpeed)0, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetTransportInfo.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetTransportInfo)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_TransportState)0, (Enum_TransportStatus)0, (Enum_TransportPlaySpeed)0, e, Handle);
                }
                else
                {
                    WeakList_GetTransportInfo.Remove(wr);
                }
            }
        }
        public void Sync_Pause(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[1];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
            _S.InvokeSync("Pause", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            return;
        }
        public int Pause(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[1];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           return(_S.InvokeAsync("Pause", args, new UPnPService.UPnPServiceInvokeHandler(Sink_Pause), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_Pause)));
        }
        private void Sink_Pause(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_Pause != null)
            {
               OnResult_Pause((System.UInt32 )Args[0].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_Pause.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_Pause)wr.Target)((System.UInt32 )Args[0].DataValue, null, Handle);
                }
                else
                {
                    WeakList_Pause.Remove(wr);
                }
            }
        }
        private void Error_Sink_Pause(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_Pause != null)
            {
                 OnResult_Pause((System.UInt32 )Args[0].DataValue, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_Pause.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_Pause)wr.Target)((System.UInt32 )Args[0].DataValue, e, Handle);
                }
                else
                {
                    WeakList_Pause.Remove(wr);
                }
            }
        }
        public void Sync_Seek(System.UInt32 InstanceID, Enum_A_ARG_TYPE_SeekMode Unit, System.String Target)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           switch(Unit)
           {
               case Enum_A_ARG_TYPE_SeekMode.ABS_TIME:
                   args[1] = new UPnPArgument("Unit", "ABS_TIME");
                   break;
               case Enum_A_ARG_TYPE_SeekMode.REL_TIME:
                   args[1] = new UPnPArgument("Unit", "REL_TIME");
                   break;
               case Enum_A_ARG_TYPE_SeekMode.ABS_COUNT:
                   args[1] = new UPnPArgument("Unit", "ABS_COUNT");
                   break;
               case Enum_A_ARG_TYPE_SeekMode.REL_COUNT:
                   args[1] = new UPnPArgument("Unit", "REL_COUNT");
                   break;
               case Enum_A_ARG_TYPE_SeekMode.TRACK_NR:
                   args[1] = new UPnPArgument("Unit", "TRACK_NR");
                   break;
               case Enum_A_ARG_TYPE_SeekMode.CHANNEL_FREQ:
                   args[1] = new UPnPArgument("Unit", "CHANNEL_FREQ");
                   break;
               case Enum_A_ARG_TYPE_SeekMode.TAPE_INDEX:
                   args[1] = new UPnPArgument("Unit", "TAPE-INDEX");
                   break;
               case Enum_A_ARG_TYPE_SeekMode.FRAME:
                   args[1] = new UPnPArgument("Unit", "FRAME");
                   break;
               default:
                  args[1] = new UPnPArgument("Unit", GetUnspecifiedValue("Enum_A_ARG_TYPE_SeekMode"));
                  break;
           }
           args[2] = new UPnPArgument("Target", Target);
            _S.InvokeSync("Seek", args);
            for(int i=0;i<args.Length;++i)
            {
                switch(args[i].Name)
                {
                    case "Unit":
                        switch((string)args[i].DataValue)
                        {
                            case "ABS_TIME":
                                args[i].DataValue = Enum_A_ARG_TYPE_SeekMode.ABS_TIME;
                                break;
                            case "REL_TIME":
                                args[i].DataValue = Enum_A_ARG_TYPE_SeekMode.REL_TIME;
                                break;
                            case "ABS_COUNT":
                                args[i].DataValue = Enum_A_ARG_TYPE_SeekMode.ABS_COUNT;
                                break;
                            case "REL_COUNT":
                                args[i].DataValue = Enum_A_ARG_TYPE_SeekMode.REL_COUNT;
                                break;
                            case "TRACK_NR":
                                args[i].DataValue = Enum_A_ARG_TYPE_SeekMode.TRACK_NR;
                                break;
                            case "CHANNEL_FREQ":
                                args[i].DataValue = Enum_A_ARG_TYPE_SeekMode.CHANNEL_FREQ;
                                break;
                            case "TAPE-INDEX":
                                args[i].DataValue = Enum_A_ARG_TYPE_SeekMode.TAPE_INDEX;
                                break;
                            case "FRAME":
                                args[i].DataValue = Enum_A_ARG_TYPE_SeekMode.FRAME;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_SeekMode", (string)args[i].DataValue);
                               args[i].DataValue = Enum_A_ARG_TYPE_SeekMode._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            InstanceID = (System.UInt32) args[0].DataValue;
            Unit = (Enum_A_ARG_TYPE_SeekMode) args[1].DataValue;
            Target = (System.String) args[2].DataValue;
            return;
        }
        public int Seek(System.UInt32 InstanceID, Enum_A_ARG_TYPE_SeekMode Unit, System.String Target)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           switch(Unit)
           {
               case Enum_A_ARG_TYPE_SeekMode.ABS_TIME:
                   args[1] = new UPnPArgument("Unit", "ABS_TIME");
                   break;
               case Enum_A_ARG_TYPE_SeekMode.REL_TIME:
                   args[1] = new UPnPArgument("Unit", "REL_TIME");
                   break;
               case Enum_A_ARG_TYPE_SeekMode.ABS_COUNT:
                   args[1] = new UPnPArgument("Unit", "ABS_COUNT");
                   break;
               case Enum_A_ARG_TYPE_SeekMode.REL_COUNT:
                   args[1] = new UPnPArgument("Unit", "REL_COUNT");
                   break;
               case Enum_A_ARG_TYPE_SeekMode.TRACK_NR:
                   args[1] = new UPnPArgument("Unit", "TRACK_NR");
                   break;
               case Enum_A_ARG_TYPE_SeekMode.CHANNEL_FREQ:
                   args[1] = new UPnPArgument("Unit", "CHANNEL_FREQ");
                   break;
               case Enum_A_ARG_TYPE_SeekMode.TAPE_INDEX:
                   args[1] = new UPnPArgument("Unit", "TAPE-INDEX");
                   break;
               case Enum_A_ARG_TYPE_SeekMode.FRAME:
                   args[1] = new UPnPArgument("Unit", "FRAME");
                   break;
               default:
                  args[1] = new UPnPArgument("Unit", GetUnspecifiedValue("Enum_A_ARG_TYPE_SeekMode"));
                  break;
           }
           args[2] = new UPnPArgument("Target", Target);
           return(_S.InvokeAsync("Seek", args, new UPnPService.UPnPServiceInvokeHandler(Sink_Seek), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_Seek)));
        }
        private void Sink_Seek(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "Unit":
                        switch((string)Args[i].DataValue)
                        {
                            case "ABS_TIME":
                                Args[i].DataValue = Enum_A_ARG_TYPE_SeekMode.ABS_TIME;
                                break;
                            case "REL_TIME":
                                Args[i].DataValue = Enum_A_ARG_TYPE_SeekMode.REL_TIME;
                                break;
                            case "ABS_COUNT":
                                Args[i].DataValue = Enum_A_ARG_TYPE_SeekMode.ABS_COUNT;
                                break;
                            case "REL_COUNT":
                                Args[i].DataValue = Enum_A_ARG_TYPE_SeekMode.REL_COUNT;
                                break;
                            case "TRACK_NR":
                                Args[i].DataValue = Enum_A_ARG_TYPE_SeekMode.TRACK_NR;
                                break;
                            case "CHANNEL_FREQ":
                                Args[i].DataValue = Enum_A_ARG_TYPE_SeekMode.CHANNEL_FREQ;
                                break;
                            case "TAPE-INDEX":
                                Args[i].DataValue = Enum_A_ARG_TYPE_SeekMode.TAPE_INDEX;
                                break;
                            case "FRAME":
                                Args[i].DataValue = Enum_A_ARG_TYPE_SeekMode.FRAME;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_SeekMode", (string)Args[i].DataValue);
                               Args[i].DataValue = Enum_A_ARG_TYPE_SeekMode._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            if(OnResult_Seek != null)
            {
               OnResult_Seek((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_SeekMode )Args[1].DataValue, (System.String )Args[2].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_Seek.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_Seek)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_SeekMode )Args[1].DataValue, (System.String )Args[2].DataValue, null, Handle);
                }
                else
                {
                    WeakList_Seek.Remove(wr);
                }
            }
        }
        private void Error_Sink_Seek(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "Unit":
                        switch((string)Args[i].DataValue)
                        {
                            case "ABS_TIME":
                                Args[i].DataValue = Enum_A_ARG_TYPE_SeekMode.ABS_TIME;
                                break;
                            case "REL_TIME":
                                Args[i].DataValue = Enum_A_ARG_TYPE_SeekMode.REL_TIME;
                                break;
                            case "ABS_COUNT":
                                Args[i].DataValue = Enum_A_ARG_TYPE_SeekMode.ABS_COUNT;
                                break;
                            case "REL_COUNT":
                                Args[i].DataValue = Enum_A_ARG_TYPE_SeekMode.REL_COUNT;
                                break;
                            case "TRACK_NR":
                                Args[i].DataValue = Enum_A_ARG_TYPE_SeekMode.TRACK_NR;
                                break;
                            case "CHANNEL_FREQ":
                                Args[i].DataValue = Enum_A_ARG_TYPE_SeekMode.CHANNEL_FREQ;
                                break;
                            case "TAPE-INDEX":
                                Args[i].DataValue = Enum_A_ARG_TYPE_SeekMode.TAPE_INDEX;
                                break;
                            case "FRAME":
                                Args[i].DataValue = Enum_A_ARG_TYPE_SeekMode.FRAME;
                                break;
                        }
                        break;
                }
            }
            if(OnResult_Seek != null)
            {
                 OnResult_Seek((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_SeekMode )Args[1].DataValue, (System.String )Args[2].DataValue, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_Seek.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_Seek)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_SeekMode )Args[1].DataValue, (System.String )Args[2].DataValue, e, Handle);
                }
                else
                {
                    WeakList_Seek.Remove(wr);
                }
            }
        }
        public void Sync_GetPositionInfo(System.UInt32 InstanceID, out System.UInt32 Track, out System.String TrackDuration, out System.String TrackMetaData, out System.String TrackURI, out System.String RelTime, out System.String AbsTime, out System.Int32 RelCount, out System.Int32 AbsCount)
        {
           UPnPArgument[] args = new UPnPArgument[9];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("Track", "");
           args[2] = new UPnPArgument("TrackDuration", "");
           args[3] = new UPnPArgument("TrackMetaData", "");
           args[4] = new UPnPArgument("TrackURI", "");
           args[5] = new UPnPArgument("RelTime", "");
           args[6] = new UPnPArgument("AbsTime", "");
           args[7] = new UPnPArgument("RelCount", "");
           args[8] = new UPnPArgument("AbsCount", "");
            _S.InvokeSync("GetPositionInfo", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            Track = (System.UInt32) args[1].DataValue;
            TrackDuration = (System.String) args[2].DataValue;
            TrackMetaData = (System.String) args[3].DataValue;
            TrackURI = (System.String) args[4].DataValue;
            RelTime = (System.String) args[5].DataValue;
            AbsTime = (System.String) args[6].DataValue;
            RelCount = (System.Int32) args[7].DataValue;
            AbsCount = (System.Int32) args[8].DataValue;
            return;
        }
        public int GetPositionInfo(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[9];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("Track", "");
           args[2] = new UPnPArgument("TrackDuration", "");
           args[3] = new UPnPArgument("TrackMetaData", "");
           args[4] = new UPnPArgument("TrackURI", "");
           args[5] = new UPnPArgument("RelTime", "");
           args[6] = new UPnPArgument("AbsTime", "");
           args[7] = new UPnPArgument("RelCount", "");
           args[8] = new UPnPArgument("AbsCount", "");
           return(_S.InvokeAsync("GetPositionInfo", args, new UPnPService.UPnPServiceInvokeHandler(Sink_GetPositionInfo), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetPositionInfo)));
        }
        private void Sink_GetPositionInfo(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_GetPositionInfo != null)
            {
               OnResult_GetPositionInfo((System.UInt32 )Args[0].DataValue, (System.UInt32 )Args[1].DataValue, (System.String )Args[2].DataValue, (System.String )Args[3].DataValue, (System.String )Args[4].DataValue, (System.String )Args[5].DataValue, (System.String )Args[6].DataValue, (System.Int32 )Args[7].DataValue, (System.Int32 )Args[8].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetPositionInfo.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetPositionInfo)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt32 )Args[1].DataValue, (System.String )Args[2].DataValue, (System.String )Args[3].DataValue, (System.String )Args[4].DataValue, (System.String )Args[5].DataValue, (System.String )Args[6].DataValue, (System.Int32 )Args[7].DataValue, (System.Int32 )Args[8].DataValue, null, Handle);
                }
                else
                {
                    WeakList_GetPositionInfo.Remove(wr);
                }
            }
        }
        private void Error_Sink_GetPositionInfo(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_GetPositionInfo != null)
            {
                 OnResult_GetPositionInfo((System.UInt32 )Args[0].DataValue, (System.UInt32)UPnPService.CreateObjectInstance(typeof(System.UInt32),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.Int32)UPnPService.CreateObjectInstance(typeof(System.Int32),null), (System.Int32)UPnPService.CreateObjectInstance(typeof(System.Int32),null), e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetPositionInfo.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetPositionInfo)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt32)UPnPService.CreateObjectInstance(typeof(System.UInt32),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.Int32)UPnPService.CreateObjectInstance(typeof(System.Int32),null), (System.Int32)UPnPService.CreateObjectInstance(typeof(System.Int32),null), e, Handle);
                }
                else
                {
                    WeakList_GetPositionInfo.Remove(wr);
                }
            }
        }
        public void Sync_Record(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[1];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
            _S.InvokeSync("Record", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            return;
        }
        public int Record(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[1];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           return(_S.InvokeAsync("Record", args, new UPnPService.UPnPServiceInvokeHandler(Sink_Record), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_Record)));
        }
        private void Sink_Record(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_Record != null)
            {
               OnResult_Record((System.UInt32 )Args[0].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_Record.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_Record)wr.Target)((System.UInt32 )Args[0].DataValue, null, Handle);
                }
                else
                {
                    WeakList_Record.Remove(wr);
                }
            }
        }
        private void Error_Sink_Record(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_Record != null)
            {
                 OnResult_Record((System.UInt32 )Args[0].DataValue, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_Record.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_Record)wr.Target)((System.UInt32 )Args[0].DataValue, e, Handle);
                }
                else
                {
                    WeakList_Record.Remove(wr);
                }
            }
        }
        public void Sync_SetRecordQualityMode(System.UInt32 InstanceID, Enum_CurrentRecordQualityMode NewRecordQualityMode)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           switch(NewRecordQualityMode)
           {
               case Enum_CurrentRecordQualityMode._0_EP:
                   args[1] = new UPnPArgument("NewRecordQualityMode", "0:EP");
                   break;
               case Enum_CurrentRecordQualityMode._1_LP:
                   args[1] = new UPnPArgument("NewRecordQualityMode", "1:LP");
                   break;
               case Enum_CurrentRecordQualityMode._2_SP:
                   args[1] = new UPnPArgument("NewRecordQualityMode", "2:SP");
                   break;
               case Enum_CurrentRecordQualityMode._0_BASIC:
                   args[1] = new UPnPArgument("NewRecordQualityMode", "0:BASIC");
                   break;
               case Enum_CurrentRecordQualityMode._1_MEDIUM:
                   args[1] = new UPnPArgument("NewRecordQualityMode", "1:MEDIUM");
                   break;
               case Enum_CurrentRecordQualityMode._2_HIGH:
                   args[1] = new UPnPArgument("NewRecordQualityMode", "2:HIGH");
                   break;
               case Enum_CurrentRecordQualityMode.NOT_IMPLEMENTED:
                   args[1] = new UPnPArgument("NewRecordQualityMode", "NOT_IMPLEMENTED");
                   break;
               case Enum_CurrentRecordQualityMode._VENDOR_DEFINED_:
                   args[1] = new UPnPArgument("NewRecordQualityMode", " vendor-defined ");
                   break;
               default:
                  args[1] = new UPnPArgument("NewRecordQualityMode", GetUnspecifiedValue("Enum_CurrentRecordQualityMode"));
                  break;
           }
            _S.InvokeSync("SetRecordQualityMode", args);
            for(int i=0;i<args.Length;++i)
            {
                switch(args[i].Name)
                {
                    case "NewRecordQualityMode":
                        switch((string)args[i].DataValue)
                        {
                            case "0:EP":
                                args[i].DataValue = Enum_CurrentRecordQualityMode._0_EP;
                                break;
                            case "1:LP":
                                args[i].DataValue = Enum_CurrentRecordQualityMode._1_LP;
                                break;
                            case "2:SP":
                                args[i].DataValue = Enum_CurrentRecordQualityMode._2_SP;
                                break;
                            case "0:BASIC":
                                args[i].DataValue = Enum_CurrentRecordQualityMode._0_BASIC;
                                break;
                            case "1:MEDIUM":
                                args[i].DataValue = Enum_CurrentRecordQualityMode._1_MEDIUM;
                                break;
                            case "2:HIGH":
                                args[i].DataValue = Enum_CurrentRecordQualityMode._2_HIGH;
                                break;
                            case "NOT_IMPLEMENTED":
                                args[i].DataValue = Enum_CurrentRecordQualityMode.NOT_IMPLEMENTED;
                                break;
                            case " vendor-defined ":
                                args[i].DataValue = Enum_CurrentRecordQualityMode._VENDOR_DEFINED_;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_CurrentRecordQualityMode", (string)args[i].DataValue);
                               args[i].DataValue = Enum_CurrentRecordQualityMode._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            InstanceID = (System.UInt32) args[0].DataValue;
            NewRecordQualityMode = (Enum_CurrentRecordQualityMode) args[1].DataValue;
            return;
        }
        public int SetRecordQualityMode(System.UInt32 InstanceID, Enum_CurrentRecordQualityMode NewRecordQualityMode)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           switch(NewRecordQualityMode)
           {
               case Enum_CurrentRecordQualityMode._0_EP:
                   args[1] = new UPnPArgument("NewRecordQualityMode", "0:EP");
                   break;
               case Enum_CurrentRecordQualityMode._1_LP:
                   args[1] = new UPnPArgument("NewRecordQualityMode", "1:LP");
                   break;
               case Enum_CurrentRecordQualityMode._2_SP:
                   args[1] = new UPnPArgument("NewRecordQualityMode", "2:SP");
                   break;
               case Enum_CurrentRecordQualityMode._0_BASIC:
                   args[1] = new UPnPArgument("NewRecordQualityMode", "0:BASIC");
                   break;
               case Enum_CurrentRecordQualityMode._1_MEDIUM:
                   args[1] = new UPnPArgument("NewRecordQualityMode", "1:MEDIUM");
                   break;
               case Enum_CurrentRecordQualityMode._2_HIGH:
                   args[1] = new UPnPArgument("NewRecordQualityMode", "2:HIGH");
                   break;
               case Enum_CurrentRecordQualityMode.NOT_IMPLEMENTED:
                   args[1] = new UPnPArgument("NewRecordQualityMode", "NOT_IMPLEMENTED");
                   break;
               case Enum_CurrentRecordQualityMode._VENDOR_DEFINED_:
                   args[1] = new UPnPArgument("NewRecordQualityMode", " vendor-defined ");
                   break;
               default:
                  args[1] = new UPnPArgument("NewRecordQualityMode", GetUnspecifiedValue("Enum_CurrentRecordQualityMode"));
                  break;
           }
           return(_S.InvokeAsync("SetRecordQualityMode", args, new UPnPService.UPnPServiceInvokeHandler(Sink_SetRecordQualityMode), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetRecordQualityMode)));
        }
        private void Sink_SetRecordQualityMode(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "NewRecordQualityMode":
                        switch((string)Args[i].DataValue)
                        {
                            case "0:EP":
                                Args[i].DataValue = Enum_CurrentRecordQualityMode._0_EP;
                                break;
                            case "1:LP":
                                Args[i].DataValue = Enum_CurrentRecordQualityMode._1_LP;
                                break;
                            case "2:SP":
                                Args[i].DataValue = Enum_CurrentRecordQualityMode._2_SP;
                                break;
                            case "0:BASIC":
                                Args[i].DataValue = Enum_CurrentRecordQualityMode._0_BASIC;
                                break;
                            case "1:MEDIUM":
                                Args[i].DataValue = Enum_CurrentRecordQualityMode._1_MEDIUM;
                                break;
                            case "2:HIGH":
                                Args[i].DataValue = Enum_CurrentRecordQualityMode._2_HIGH;
                                break;
                            case "NOT_IMPLEMENTED":
                                Args[i].DataValue = Enum_CurrentRecordQualityMode.NOT_IMPLEMENTED;
                                break;
                            case " vendor-defined ":
                                Args[i].DataValue = Enum_CurrentRecordQualityMode._VENDOR_DEFINED_;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_CurrentRecordQualityMode", (string)Args[i].DataValue);
                               Args[i].DataValue = Enum_CurrentRecordQualityMode._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            if(OnResult_SetRecordQualityMode != null)
            {
               OnResult_SetRecordQualityMode((System.UInt32 )Args[0].DataValue, (Enum_CurrentRecordQualityMode )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetRecordQualityMode.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetRecordQualityMode)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_CurrentRecordQualityMode )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_SetRecordQualityMode.Remove(wr);
                }
            }
        }
        private void Error_Sink_SetRecordQualityMode(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "NewRecordQualityMode":
                        switch((string)Args[i].DataValue)
                        {
                            case "0:EP":
                                Args[i].DataValue = Enum_CurrentRecordQualityMode._0_EP;
                                break;
                            case "1:LP":
                                Args[i].DataValue = Enum_CurrentRecordQualityMode._1_LP;
                                break;
                            case "2:SP":
                                Args[i].DataValue = Enum_CurrentRecordQualityMode._2_SP;
                                break;
                            case "0:BASIC":
                                Args[i].DataValue = Enum_CurrentRecordQualityMode._0_BASIC;
                                break;
                            case "1:MEDIUM":
                                Args[i].DataValue = Enum_CurrentRecordQualityMode._1_MEDIUM;
                                break;
                            case "2:HIGH":
                                Args[i].DataValue = Enum_CurrentRecordQualityMode._2_HIGH;
                                break;
                            case "NOT_IMPLEMENTED":
                                Args[i].DataValue = Enum_CurrentRecordQualityMode.NOT_IMPLEMENTED;
                                break;
                            case " vendor-defined ":
                                Args[i].DataValue = Enum_CurrentRecordQualityMode._VENDOR_DEFINED_;
                                break;
                        }
                        break;
                }
            }
            if(OnResult_SetRecordQualityMode != null)
            {
                 OnResult_SetRecordQualityMode((System.UInt32 )Args[0].DataValue, (Enum_CurrentRecordQualityMode )Args[1].DataValue, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetRecordQualityMode.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetRecordQualityMode)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_CurrentRecordQualityMode )Args[1].DataValue, e, Handle);
                }
                else
                {
                    WeakList_SetRecordQualityMode.Remove(wr);
                }
            }
        }
    }
}