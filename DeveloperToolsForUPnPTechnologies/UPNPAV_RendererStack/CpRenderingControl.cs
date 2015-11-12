using System;
using System.Collections;
using System.Threading;
using Intel.UPNP;

namespace Intel.UPNP.AVRENDERERSTACK
{
    /// <summary>
    /// Transparent ClientSide UPnP Service
    /// </summary>
    public class CpRenderingControl
    {
       private Hashtable UnspecifiedTable = Hashtable.Synchronized(new Hashtable());
       internal UPnPService _S;

       public static string SERVICE_NAME = "urn:schemas-upnp-org:service:RenderingControl:";
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
       public delegate void Delegate_OnResult_GetHorizontalKeystone(System.UInt32 InstanceID, System.Int16 CurrentHorizontalKeystone, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_GetHorizontalKeystone OnResult_GetHorizontalKeystone;
       protected ArrayList WeakList_GetHorizontalKeystone = new ArrayList();
       public void AddWeakEvent_Result_GetHorizontalKeystone(Delegate_OnResult_GetHorizontalKeystone d)
       {
           WeakList_GetHorizontalKeystone.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_GetVolume(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.UInt16 CurrentVolume, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_GetVolume OnResult_GetVolume;
       protected ArrayList WeakList_GetVolume = new ArrayList();
       public void AddWeakEvent_Result_GetVolume(Delegate_OnResult_GetVolume d)
       {
           WeakList_GetVolume.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_SelectPreset(System.UInt32 InstanceID, Enum_A_ARG_TYPE_PresetName PresetName, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_SelectPreset OnResult_SelectPreset;
       protected ArrayList WeakList_SelectPreset = new ArrayList();
       public void AddWeakEvent_Result_SelectPreset(Delegate_OnResult_SelectPreset d)
       {
           WeakList_SelectPreset.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_SetVolume(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.UInt16 DesiredVolume, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_SetVolume OnResult_SetVolume;
       protected ArrayList WeakList_SetVolume = new ArrayList();
       public void AddWeakEvent_Result_SetVolume(Delegate_OnResult_SetVolume d)
       {
           WeakList_SetVolume.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_ListPresets(System.UInt32 InstanceID, System.String CurrentPresetNameList, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_ListPresets OnResult_ListPresets;
       protected ArrayList WeakList_ListPresets = new ArrayList();
       public void AddWeakEvent_Result_ListPresets(Delegate_OnResult_ListPresets d)
       {
           WeakList_ListPresets.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_SetVolumeDB(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.Int16 DesiredVolume, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_SetVolumeDB OnResult_SetVolumeDB;
       protected ArrayList WeakList_SetVolumeDB = new ArrayList();
       public void AddWeakEvent_Result_SetVolumeDB(Delegate_OnResult_SetVolumeDB d)
       {
           WeakList_SetVolumeDB.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_SetRedVideoBlackLevel(System.UInt32 InstanceID, System.UInt16 DesiredRedVideoBlackLevel, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_SetRedVideoBlackLevel OnResult_SetRedVideoBlackLevel;
       protected ArrayList WeakList_SetRedVideoBlackLevel = new ArrayList();
       public void AddWeakEvent_Result_SetRedVideoBlackLevel(Delegate_OnResult_SetRedVideoBlackLevel d)
       {
           WeakList_SetRedVideoBlackLevel.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_SetContrast(System.UInt32 InstanceID, System.UInt16 DesiredContrast, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_SetContrast OnResult_SetContrast;
       protected ArrayList WeakList_SetContrast = new ArrayList();
       public void AddWeakEvent_Result_SetContrast(Delegate_OnResult_SetContrast d)
       {
           WeakList_SetContrast.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_SetLoudness(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.Boolean DesiredLoudness, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_SetLoudness OnResult_SetLoudness;
       protected ArrayList WeakList_SetLoudness = new ArrayList();
       public void AddWeakEvent_Result_SetLoudness(Delegate_OnResult_SetLoudness d)
       {
           WeakList_SetLoudness.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_SetBrightness(System.UInt32 InstanceID, System.UInt16 DesiredBrightness, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_SetBrightness OnResult_SetBrightness;
       protected ArrayList WeakList_SetBrightness = new ArrayList();
       public void AddWeakEvent_Result_SetBrightness(Delegate_OnResult_SetBrightness d)
       {
           WeakList_SetBrightness.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_GetLoudness(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.Boolean CurrentLoudness, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_GetLoudness OnResult_GetLoudness;
       protected ArrayList WeakList_GetLoudness = new ArrayList();
       public void AddWeakEvent_Result_GetLoudness(Delegate_OnResult_GetLoudness d)
       {
           WeakList_GetLoudness.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_GetColorTemperature(System.UInt32 InstanceID, System.UInt16 CurrentColorTemperature, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_GetColorTemperature OnResult_GetColorTemperature;
       protected ArrayList WeakList_GetColorTemperature = new ArrayList();
       public void AddWeakEvent_Result_GetColorTemperature(Delegate_OnResult_GetColorTemperature d)
       {
           WeakList_GetColorTemperature.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_GetSharpness(System.UInt32 InstanceID, System.UInt16 CurrentSharpness, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_GetSharpness OnResult_GetSharpness;
       protected ArrayList WeakList_GetSharpness = new ArrayList();
       public void AddWeakEvent_Result_GetSharpness(Delegate_OnResult_GetSharpness d)
       {
           WeakList_GetSharpness.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_GetContrast(System.UInt32 InstanceID, System.UInt16 CurrentContrast, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_GetContrast OnResult_GetContrast;
       protected ArrayList WeakList_GetContrast = new ArrayList();
       public void AddWeakEvent_Result_GetContrast(Delegate_OnResult_GetContrast d)
       {
           WeakList_GetContrast.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_GetGreenVideoGain(System.UInt32 InstanceID, System.UInt16 CurrentGreenVideoGain, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_GetGreenVideoGain OnResult_GetGreenVideoGain;
       protected ArrayList WeakList_GetGreenVideoGain = new ArrayList();
       public void AddWeakEvent_Result_GetGreenVideoGain(Delegate_OnResult_GetGreenVideoGain d)
       {
           WeakList_GetGreenVideoGain.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_SetRedVideoGain(System.UInt32 InstanceID, System.UInt16 DesiredRedVideoGain, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_SetRedVideoGain OnResult_SetRedVideoGain;
       protected ArrayList WeakList_SetRedVideoGain = new ArrayList();
       public void AddWeakEvent_Result_SetRedVideoGain(Delegate_OnResult_SetRedVideoGain d)
       {
           WeakList_SetRedVideoGain.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_SetGreenVideoBlackLevel(System.UInt32 InstanceID, System.UInt16 DesiredGreenVideoBlackLevel, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_SetGreenVideoBlackLevel OnResult_SetGreenVideoBlackLevel;
       protected ArrayList WeakList_SetGreenVideoBlackLevel = new ArrayList();
       public void AddWeakEvent_Result_SetGreenVideoBlackLevel(Delegate_OnResult_SetGreenVideoBlackLevel d)
       {
           WeakList_SetGreenVideoBlackLevel.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_GetRedVideoBlackLevel(System.UInt32 InstanceID, System.UInt16 CurrentRedVideoBlackLevel, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_GetRedVideoBlackLevel OnResult_GetRedVideoBlackLevel;
       protected ArrayList WeakList_GetRedVideoBlackLevel = new ArrayList();
       public void AddWeakEvent_Result_GetRedVideoBlackLevel(Delegate_OnResult_GetRedVideoBlackLevel d)
       {
           WeakList_GetRedVideoBlackLevel.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_GetBlueVideoBlackLevel(System.UInt32 InstanceID, System.UInt16 CurrentBlueVideoBlackLevel, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_GetBlueVideoBlackLevel OnResult_GetBlueVideoBlackLevel;
       protected ArrayList WeakList_GetBlueVideoBlackLevel = new ArrayList();
       public void AddWeakEvent_Result_GetBlueVideoBlackLevel(Delegate_OnResult_GetBlueVideoBlackLevel d)
       {
           WeakList_GetBlueVideoBlackLevel.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_GetBlueVideoGain(System.UInt32 InstanceID, System.UInt16 CurrentBlueVideoGain, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_GetBlueVideoGain OnResult_GetBlueVideoGain;
       protected ArrayList WeakList_GetBlueVideoGain = new ArrayList();
       public void AddWeakEvent_Result_GetBlueVideoGain(Delegate_OnResult_GetBlueVideoGain d)
       {
           WeakList_GetBlueVideoGain.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_SetBlueVideoBlackLevel(System.UInt32 InstanceID, System.UInt16 DesiredBlueVideoBlackLevel, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_SetBlueVideoBlackLevel OnResult_SetBlueVideoBlackLevel;
       protected ArrayList WeakList_SetBlueVideoBlackLevel = new ArrayList();
       public void AddWeakEvent_Result_SetBlueVideoBlackLevel(Delegate_OnResult_SetBlueVideoBlackLevel d)
       {
           WeakList_SetBlueVideoBlackLevel.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_GetMute(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.Boolean CurrentMute, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_GetMute OnResult_GetMute;
       protected ArrayList WeakList_GetMute = new ArrayList();
       public void AddWeakEvent_Result_GetMute(Delegate_OnResult_GetMute d)
       {
           WeakList_GetMute.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_SetBlueVideoGain(System.UInt32 InstanceID, System.UInt16 DesiredBlueVideoGain, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_SetBlueVideoGain OnResult_SetBlueVideoGain;
       protected ArrayList WeakList_SetBlueVideoGain = new ArrayList();
       public void AddWeakEvent_Result_SetBlueVideoGain(Delegate_OnResult_SetBlueVideoGain d)
       {
           WeakList_SetBlueVideoGain.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_GetVerticalKeystone(System.UInt32 InstanceID, System.Int16 CurrentVerticalKeystone, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_GetVerticalKeystone OnResult_GetVerticalKeystone;
       protected ArrayList WeakList_GetVerticalKeystone = new ArrayList();
       public void AddWeakEvent_Result_GetVerticalKeystone(Delegate_OnResult_GetVerticalKeystone d)
       {
           WeakList_GetVerticalKeystone.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_SetVerticalKeystone(System.UInt32 InstanceID, System.Int16 DesiredVerticalKeystone, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_SetVerticalKeystone OnResult_SetVerticalKeystone;
       protected ArrayList WeakList_SetVerticalKeystone = new ArrayList();
       public void AddWeakEvent_Result_SetVerticalKeystone(Delegate_OnResult_SetVerticalKeystone d)
       {
           WeakList_SetVerticalKeystone.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_GetBrightness(System.UInt32 InstanceID, System.UInt16 CurrentBrightness, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_GetBrightness OnResult_GetBrightness;
       protected ArrayList WeakList_GetBrightness = new ArrayList();
       public void AddWeakEvent_Result_GetBrightness(Delegate_OnResult_GetBrightness d)
       {
           WeakList_GetBrightness.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_GetVolumeDB(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.Int16 CurrentVolume, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_GetVolumeDB OnResult_GetVolumeDB;
       protected ArrayList WeakList_GetVolumeDB = new ArrayList();
       public void AddWeakEvent_Result_GetVolumeDB(Delegate_OnResult_GetVolumeDB d)
       {
           WeakList_GetVolumeDB.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_GetGreenVideoBlackLevel(System.UInt32 InstanceID, System.UInt16 CurrentGreenVideoBlackLevel, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_GetGreenVideoBlackLevel OnResult_GetGreenVideoBlackLevel;
       protected ArrayList WeakList_GetGreenVideoBlackLevel = new ArrayList();
       public void AddWeakEvent_Result_GetGreenVideoBlackLevel(Delegate_OnResult_GetGreenVideoBlackLevel d)
       {
           WeakList_GetGreenVideoBlackLevel.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_GetRedVideoGain(System.UInt32 InstanceID, System.UInt16 CurrentRedVideoGain, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_GetRedVideoGain OnResult_GetRedVideoGain;
       protected ArrayList WeakList_GetRedVideoGain = new ArrayList();
       public void AddWeakEvent_Result_GetRedVideoGain(Delegate_OnResult_GetRedVideoGain d)
       {
           WeakList_GetRedVideoGain.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_SetMute(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.Boolean DesiredMute, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_SetMute OnResult_SetMute;
       protected ArrayList WeakList_SetMute = new ArrayList();
       public void AddWeakEvent_Result_SetMute(Delegate_OnResult_SetMute d)
       {
           WeakList_SetMute.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_SetGreenVideoGain(System.UInt32 InstanceID, System.UInt16 DesiredGreenVideoGain, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_SetGreenVideoGain OnResult_SetGreenVideoGain;
       protected ArrayList WeakList_SetGreenVideoGain = new ArrayList();
       public void AddWeakEvent_Result_SetGreenVideoGain(Delegate_OnResult_SetGreenVideoGain d)
       {
           WeakList_SetGreenVideoGain.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_SetSharpness(System.UInt32 InstanceID, System.UInt16 DesiredSharpness, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_SetSharpness OnResult_SetSharpness;
       protected ArrayList WeakList_SetSharpness = new ArrayList();
       public void AddWeakEvent_Result_SetSharpness(Delegate_OnResult_SetSharpness d)
       {
           WeakList_SetSharpness.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_SetHorizontalKeystone(System.UInt32 InstanceID, System.Int16 DesiredHorizontalKeystone, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_SetHorizontalKeystone OnResult_SetHorizontalKeystone;
       protected ArrayList WeakList_SetHorizontalKeystone = new ArrayList();
       public void AddWeakEvent_Result_SetHorizontalKeystone(Delegate_OnResult_SetHorizontalKeystone d)
       {
           WeakList_SetHorizontalKeystone.Add(new WeakReference(d));
       }
       public delegate void Delegate_OnResult_SetColorTemperature(System.UInt32 InstanceID, System.UInt16 DesiredColorTemperature, UPnPInvokeException e, int Handle);
       public event Delegate_OnResult_SetColorTemperature OnResult_SetColorTemperature;
       protected ArrayList WeakList_SetColorTemperature = new ArrayList();
       public void AddWeakEvent_Result_SetColorTemperature(Delegate_OnResult_SetColorTemperature d)
       {
           WeakList_SetColorTemperature.Add(new WeakReference(d));
       }

        public CpRenderingControl(UPnPService s)
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
        public string[] Values_A_ARG_TYPE_PresetName
        {
            get
            {
                UPnPStateVariable sv = _S.GetStateVariableObject("A_ARG_TYPE_PresetName");
                return(sv.AllowedStringValues);
            }
        }
        public string Enum_A_ARG_TYPE_PresetName_ToString(Enum_A_ARG_TYPE_PresetName en)
        {
            string RetVal = "";
            switch(en)
            {
                case Enum_A_ARG_TYPE_PresetName.FACTORYDEFAULTS:
                    RetVal = "FactoryDefaults";
                    break;
                case Enum_A_ARG_TYPE_PresetName.INSTALLATIONDEFAULTS:
                    RetVal = "InstallationDefaults";
                    break;
                case Enum_A_ARG_TYPE_PresetName.VENDOR_DEFINED:
                    RetVal = "Vendor defined";
                    break;
                case Enum_A_ARG_TYPE_PresetName._UNSPECIFIED_:
                    RetVal = GetUnspecifiedValue("Enum_A_ARG_TYPE_PresetName");
                    break;
            }
            return(RetVal);
        }
        public enum Enum_A_ARG_TYPE_PresetName
        {
            _UNSPECIFIED_,
            FACTORYDEFAULTS,
            INSTALLATIONDEFAULTS,
            VENDOR_DEFINED,
        }
        public Enum_A_ARG_TYPE_PresetName A_ARG_TYPE_PresetName
        {
            get
            {
               Enum_A_ARG_TYPE_PresetName RetVal = 0;
               string v = (string)_S.GetStateVariable("A_ARG_TYPE_PresetName");
               switch(v)
               {
                  case "FactoryDefaults":
                     RetVal = Enum_A_ARG_TYPE_PresetName.FACTORYDEFAULTS;
                     break;
                  case "InstallationDefaults":
                     RetVal = Enum_A_ARG_TYPE_PresetName.INSTALLATIONDEFAULTS;
                     break;
                  case "Vendor defined":
                     RetVal = Enum_A_ARG_TYPE_PresetName.VENDOR_DEFINED;
                     break;
                  default:
                     RetVal = Enum_A_ARG_TYPE_PresetName._UNSPECIFIED_;
                     SetUnspecifiedValue("Enum_A_ARG_TYPE_PresetName", v);
                     break;
               }
               return(RetVal);
           }
        }
        public string[] Values_A_ARG_TYPE_Channel
        {
            get
            {
                UPnPStateVariable sv = _S.GetStateVariableObject("A_ARG_TYPE_Channel");
                return(sv.AllowedStringValues);
            }
        }
        public string Enum_A_ARG_TYPE_Channel_ToString(Enum_A_ARG_TYPE_Channel en)
        {
            string RetVal = "";
            switch(en)
            {
                case Enum_A_ARG_TYPE_Channel.MASTER:
                    RetVal = "Master";
                    break;
                case Enum_A_ARG_TYPE_Channel.LF:
                    RetVal = "LF";
                    break;
                case Enum_A_ARG_TYPE_Channel.RF:
                    RetVal = "RF";
                    break;
                case Enum_A_ARG_TYPE_Channel.CF:
                    RetVal = "CF";
                    break;
                case Enum_A_ARG_TYPE_Channel.LFE:
                    RetVal = "LFE";
                    break;
                case Enum_A_ARG_TYPE_Channel.LS:
                    RetVal = "LS";
                    break;
                case Enum_A_ARG_TYPE_Channel.RS:
                    RetVal = "RS";
                    break;
                case Enum_A_ARG_TYPE_Channel.LFC:
                    RetVal = "LFC";
                    break;
                case Enum_A_ARG_TYPE_Channel.RFC:
                    RetVal = "RFC";
                    break;
                case Enum_A_ARG_TYPE_Channel.SD:
                    RetVal = "SD";
                    break;
                case Enum_A_ARG_TYPE_Channel.SL:
                    RetVal = "SL";
                    break;
                case Enum_A_ARG_TYPE_Channel.SR_:
                    RetVal = "SR ";
                    break;
                case Enum_A_ARG_TYPE_Channel.T:
                    RetVal = "T";
                    break;
                case Enum_A_ARG_TYPE_Channel.B:
                    RetVal = "B";
                    break;
                case Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED:
                    RetVal = "Vendor defined";
                    break;
                case Enum_A_ARG_TYPE_Channel._UNSPECIFIED_:
                    RetVal = GetUnspecifiedValue("Enum_A_ARG_TYPE_Channel");
                    break;
            }
            return(RetVal);
        }
        public enum Enum_A_ARG_TYPE_Channel
        {
            _UNSPECIFIED_,
            MASTER,
            LF,
            RF,
            CF,
            LFE,
            LS,
            RS,
            LFC,
            RFC,
            SD,
            SL,
            SR_,
            T,
            B,
            VENDOR_DEFINED,
        }
        public Enum_A_ARG_TYPE_Channel A_ARG_TYPE_Channel
        {
            get
            {
               Enum_A_ARG_TYPE_Channel RetVal = 0;
               string v = (string)_S.GetStateVariable("A_ARG_TYPE_Channel");
               switch(v)
               {
                  case "Master":
                     RetVal = Enum_A_ARG_TYPE_Channel.MASTER;
                     break;
                  case "LF":
                     RetVal = Enum_A_ARG_TYPE_Channel.LF;
                     break;
                  case "RF":
                     RetVal = Enum_A_ARG_TYPE_Channel.RF;
                     break;
                  case "CF":
                     RetVal = Enum_A_ARG_TYPE_Channel.CF;
                     break;
                  case "LFE":
                     RetVal = Enum_A_ARG_TYPE_Channel.LFE;
                     break;
                  case "LS":
                     RetVal = Enum_A_ARG_TYPE_Channel.LS;
                     break;
                  case "RS":
                     RetVal = Enum_A_ARG_TYPE_Channel.RS;
                     break;
                  case "LFC":
                     RetVal = Enum_A_ARG_TYPE_Channel.LFC;
                     break;
                  case "RFC":
                     RetVal = Enum_A_ARG_TYPE_Channel.RFC;
                     break;
                  case "SD":
                     RetVal = Enum_A_ARG_TYPE_Channel.SD;
                     break;
                  case "SL":
                     RetVal = Enum_A_ARG_TYPE_Channel.SL;
                     break;
                  case "SR ":
                     RetVal = Enum_A_ARG_TYPE_Channel.SR_;
                     break;
                  case "T":
                     RetVal = Enum_A_ARG_TYPE_Channel.T;
                     break;
                  case "B":
                     RetVal = Enum_A_ARG_TYPE_Channel.B;
                     break;
                  case "Vendor defined":
                     RetVal = Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED;
                     break;
                  default:
                     RetVal = Enum_A_ARG_TYPE_Channel._UNSPECIFIED_;
                     SetUnspecifiedValue("Enum_A_ARG_TYPE_Channel", v);
                     break;
               }
               return(RetVal);
           }
        }
        public System.Int16 VerticalKeystone
        {
            get
            {
               return((System.Int16)_S.GetStateVariable("VerticalKeystone"));
            }
        }
        public System.String LastChange
        {
            get
            {
               return((System.String)_S.GetStateVariable("LastChange"));
            }
        }
        public System.Boolean Loudness
        {
            get
            {
               return((System.Boolean)_S.GetStateVariable("Loudness"));
            }
        }
        public System.Int16 HorizontalKeystone
        {
            get
            {
               return((System.Int16)_S.GetStateVariable("HorizontalKeystone"));
            }
        }
        public System.UInt32 A_ARG_TYPE_InstanceID
        {
            get
            {
               return((System.UInt32)_S.GetStateVariable("A_ARG_TYPE_InstanceID"));
            }
        }
        public System.UInt16 BlueVideoBlackLevel
        {
            get
            {
               return((System.UInt16)_S.GetStateVariable("BlueVideoBlackLevel"));
            }
        }
        public System.UInt16 RedVideoGain
        {
            get
            {
               return((System.UInt16)_S.GetStateVariable("RedVideoGain"));
            }
        }
        public System.UInt16 GreenVideoBlackLevel
        {
            get
            {
               return((System.UInt16)_S.GetStateVariable("GreenVideoBlackLevel"));
            }
        }
        public System.UInt16 Volume
        {
            get
            {
               return((System.UInt16)_S.GetStateVariable("Volume"));
            }
        }
        public System.Boolean Mute
        {
            get
            {
               return((System.Boolean)_S.GetStateVariable("Mute"));
            }
        }
        public System.UInt16 Brightness
        {
            get
            {
               return((System.UInt16)_S.GetStateVariable("Brightness"));
            }
        }
        public System.String PresetNameList
        {
            get
            {
               return((System.String)_S.GetStateVariable("PresetNameList"));
            }
        }
        public System.UInt16 ColorTemperature
        {
            get
            {
               return((System.UInt16)_S.GetStateVariable("ColorTemperature"));
            }
        }
        public System.Int16 VolumeDB
        {
            get
            {
               return((System.Int16)_S.GetStateVariable("VolumeDB"));
            }
        }
        public System.UInt16 Contrast
        {
            get
            {
               return((System.UInt16)_S.GetStateVariable("Contrast"));
            }
        }
        public System.UInt16 GreenVideoGain
        {
            get
            {
               return((System.UInt16)_S.GetStateVariable("GreenVideoGain"));
            }
        }
        public System.UInt16 RedVideoBlackLevel
        {
            get
            {
               return((System.UInt16)_S.GetStateVariable("RedVideoBlackLevel"));
            }
        }
        public System.UInt16 BlueVideoGain
        {
            get
            {
               return((System.UInt16)_S.GetStateVariable("BlueVideoGain"));
            }
        }
        public System.UInt16 Sharpness
        {
            get
            {
               return((System.UInt16)_S.GetStateVariable("Sharpness"));
            }
        }
        public bool HasMaximum_VerticalKeystone
        {
             get
             {
                 return(_S.GetStateVariableObject("VerticalKeystone").Maximum!=null);
             }
        }
        public bool HasMinimum_VerticalKeystone
        {
             get
             {
                 return(_S.GetStateVariableObject("VerticalKeystone").Minimum!=null);
             }
        }
        public System.Int16 Maximum_VerticalKeystone
        {
             get
             {
                 return((System.Int16)_S.GetStateVariableObject("VerticalKeystone").Maximum);
             }
        }
        public System.Int16 Minimum_VerticalKeystone
        {
             get
             {
                 return((System.Int16)_S.GetStateVariableObject("VerticalKeystone").Minimum);
             }
        }
        public bool HasStateVariable_VerticalKeystone
        {
            get
            {
               if(_S.GetStateVariableObject("VerticalKeystone")==null)
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
        public bool HasStateVariable_A_ARG_TYPE_PresetName
        {
            get
            {
               if(_S.GetStateVariableObject("A_ARG_TYPE_PresetName")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_Loudness
        {
            get
            {
               if(_S.GetStateVariableObject("Loudness")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasMaximum_HorizontalKeystone
        {
             get
             {
                 return(_S.GetStateVariableObject("HorizontalKeystone").Maximum!=null);
             }
        }
        public bool HasMinimum_HorizontalKeystone
        {
             get
             {
                 return(_S.GetStateVariableObject("HorizontalKeystone").Minimum!=null);
             }
        }
        public System.Int16 Maximum_HorizontalKeystone
        {
             get
             {
                 return((System.Int16)_S.GetStateVariableObject("HorizontalKeystone").Maximum);
             }
        }
        public System.Int16 Minimum_HorizontalKeystone
        {
             get
             {
                 return((System.Int16)_S.GetStateVariableObject("HorizontalKeystone").Minimum);
             }
        }
        public bool HasStateVariable_HorizontalKeystone
        {
            get
            {
               if(_S.GetStateVariableObject("HorizontalKeystone")==null)
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
        public bool HasMaximum_BlueVideoBlackLevel
        {
             get
             {
                 return(_S.GetStateVariableObject("BlueVideoBlackLevel").Maximum!=null);
             }
        }
        public bool HasMinimum_BlueVideoBlackLevel
        {
             get
             {
                 return(_S.GetStateVariableObject("BlueVideoBlackLevel").Minimum!=null);
             }
        }
        public System.UInt16 Maximum_BlueVideoBlackLevel
        {
             get
             {
                 return((System.UInt16)_S.GetStateVariableObject("BlueVideoBlackLevel").Maximum);
             }
        }
        public System.UInt16 Minimum_BlueVideoBlackLevel
        {
             get
             {
                 return((System.UInt16)_S.GetStateVariableObject("BlueVideoBlackLevel").Minimum);
             }
        }
        public bool HasStateVariable_BlueVideoBlackLevel
        {
            get
            {
               if(_S.GetStateVariableObject("BlueVideoBlackLevel")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasMaximum_RedVideoGain
        {
             get
             {
                 return(_S.GetStateVariableObject("RedVideoGain").Maximum!=null);
             }
        }
        public bool HasMinimum_RedVideoGain
        {
             get
             {
                 return(_S.GetStateVariableObject("RedVideoGain").Minimum!=null);
             }
        }
        public System.UInt16 Maximum_RedVideoGain
        {
             get
             {
                 return((System.UInt16)_S.GetStateVariableObject("RedVideoGain").Maximum);
             }
        }
        public System.UInt16 Minimum_RedVideoGain
        {
             get
             {
                 return((System.UInt16)_S.GetStateVariableObject("RedVideoGain").Minimum);
             }
        }
        public bool HasStateVariable_RedVideoGain
        {
            get
            {
               if(_S.GetStateVariableObject("RedVideoGain")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasMaximum_GreenVideoBlackLevel
        {
             get
             {
                 return(_S.GetStateVariableObject("GreenVideoBlackLevel").Maximum!=null);
             }
        }
        public bool HasMinimum_GreenVideoBlackLevel
        {
             get
             {
                 return(_S.GetStateVariableObject("GreenVideoBlackLevel").Minimum!=null);
             }
        }
        public System.UInt16 Maximum_GreenVideoBlackLevel
        {
             get
             {
                 return((System.UInt16)_S.GetStateVariableObject("GreenVideoBlackLevel").Maximum);
             }
        }
        public System.UInt16 Minimum_GreenVideoBlackLevel
        {
             get
             {
                 return((System.UInt16)_S.GetStateVariableObject("GreenVideoBlackLevel").Minimum);
             }
        }
        public bool HasStateVariable_GreenVideoBlackLevel
        {
            get
            {
               if(_S.GetStateVariableObject("GreenVideoBlackLevel")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasMaximum_Volume
        {
             get
             {
				 if(_S.GetStateVariableObject("Volume")==null) return(false);
                 return(_S.GetStateVariableObject("Volume").Maximum!=null);
             }
        }
        public bool HasMinimum_Volume
        {
             get
             {
				 if(_S.GetStateVariableObject("Volume")==null) return(false);
                 return(_S.GetStateVariableObject("Volume").Minimum!=null);
             }
        }
        public System.UInt16 Maximum_Volume
        {
             get
             {
                 return((System.UInt16)_S.GetStateVariableObject("Volume").Maximum);
             }
        }
        public System.UInt16 Minimum_Volume
        {
             get
             {
                 return((System.UInt16)_S.GetStateVariableObject("Volume").Minimum);
             }
        }
        public bool HasStateVariable_Volume
        {
            get
            {
               if(_S.GetStateVariableObject("Volume")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_Mute
        {
            get
            {
               if(_S.GetStateVariableObject("Mute")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasMaximum_Brightness
        {
             get
             {
                 return(_S.GetStateVariableObject("Brightness").Maximum!=null);
             }
        }
        public bool HasMinimum_Brightness
        {
             get
             {
                 return(_S.GetStateVariableObject("Brightness").Minimum!=null);
             }
        }
        public System.UInt16 Maximum_Brightness
        {
             get
             {
                 return((System.UInt16)_S.GetStateVariableObject("Brightness").Maximum);
             }
        }
        public System.UInt16 Minimum_Brightness
        {
             get
             {
                 return((System.UInt16)_S.GetStateVariableObject("Brightness").Minimum);
             }
        }
        public bool HasStateVariable_Brightness
        {
            get
            {
               if(_S.GetStateVariableObject("Brightness")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_PresetNameList
        {
            get
            {
               if(_S.GetStateVariableObject("PresetNameList")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_A_ARG_TYPE_Channel
        {
            get
            {
               if(_S.GetStateVariableObject("A_ARG_TYPE_Channel")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasMaximum_ColorTemperature
        {
             get
             {
                 return(_S.GetStateVariableObject("ColorTemperature").Maximum!=null);
             }
        }
        public bool HasMinimum_ColorTemperature
        {
             get
             {
                 return(_S.GetStateVariableObject("ColorTemperature").Minimum!=null);
             }
        }
        public System.UInt16 Maximum_ColorTemperature
        {
             get
             {
                 return((System.UInt16)_S.GetStateVariableObject("ColorTemperature").Maximum);
             }
        }
        public System.UInt16 Minimum_ColorTemperature
        {
             get
             {
                 return((System.UInt16)_S.GetStateVariableObject("ColorTemperature").Minimum);
             }
        }
        public bool HasStateVariable_ColorTemperature
        {
            get
            {
               if(_S.GetStateVariableObject("ColorTemperature")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasMaximum_VolumeDB
        {
             get
             {
                 return(_S.GetStateVariableObject("VolumeDB").Maximum!=null);
             }
        }
        public bool HasMinimum_VolumeDB
        {
             get
             {
                 return(_S.GetStateVariableObject("VolumeDB").Minimum!=null);
             }
        }
        public System.Int16 Maximum_VolumeDB
        {
             get
             {
                 return((System.Int16)_S.GetStateVariableObject("VolumeDB").Maximum);
             }
        }
        public System.Int16 Minimum_VolumeDB
        {
             get
             {
                 return((System.Int16)_S.GetStateVariableObject("VolumeDB").Minimum);
             }
        }
        public bool HasStateVariable_VolumeDB
        {
            get
            {
               if(_S.GetStateVariableObject("VolumeDB")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasMaximum_Contrast
        {
             get
             {
                 return(_S.GetStateVariableObject("Contrast").Maximum!=null);
             }
        }
        public bool HasMinimum_Contrast
        {
             get
             {
                 return(_S.GetStateVariableObject("Contrast").Minimum!=null);
             }
        }
        public System.UInt16 Maximum_Contrast
        {
             get
             {
                 return((System.UInt16)_S.GetStateVariableObject("Contrast").Maximum);
             }
        }
        public System.UInt16 Minimum_Contrast
        {
             get
             {
                 return((System.UInt16)_S.GetStateVariableObject("Contrast").Minimum);
             }
        }
        public bool HasStateVariable_Contrast
        {
            get
            {
               if(_S.GetStateVariableObject("Contrast")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasMaximum_GreenVideoGain
        {
             get
             {
                 return(_S.GetStateVariableObject("GreenVideoGain").Maximum!=null);
             }
        }
        public bool HasMinimum_GreenVideoGain
        {
             get
             {
                 return(_S.GetStateVariableObject("GreenVideoGain").Minimum!=null);
             }
        }
        public System.UInt16 Maximum_GreenVideoGain
        {
             get
             {
                 return((System.UInt16)_S.GetStateVariableObject("GreenVideoGain").Maximum);
             }
        }
        public System.UInt16 Minimum_GreenVideoGain
        {
             get
             {
                 return((System.UInt16)_S.GetStateVariableObject("GreenVideoGain").Minimum);
             }
        }
        public bool HasStateVariable_GreenVideoGain
        {
            get
            {
               if(_S.GetStateVariableObject("GreenVideoGain")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasMaximum_RedVideoBlackLevel
        {
             get
             {
                 return(_S.GetStateVariableObject("RedVideoBlackLevel").Maximum!=null);
             }
        }
        public bool HasMinimum_RedVideoBlackLevel
        {
             get
             {
                 return(_S.GetStateVariableObject("RedVideoBlackLevel").Minimum!=null);
             }
        }
        public System.UInt16 Maximum_RedVideoBlackLevel
        {
             get
             {
                 return((System.UInt16)_S.GetStateVariableObject("RedVideoBlackLevel").Maximum);
             }
        }
        public System.UInt16 Minimum_RedVideoBlackLevel
        {
             get
             {
                 return((System.UInt16)_S.GetStateVariableObject("RedVideoBlackLevel").Minimum);
             }
        }
        public bool HasStateVariable_RedVideoBlackLevel
        {
            get
            {
               if(_S.GetStateVariableObject("RedVideoBlackLevel")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasMaximum_BlueVideoGain
        {
             get
             {
                 return(_S.GetStateVariableObject("BlueVideoGain").Maximum!=null);
             }
        }
        public bool HasMinimum_BlueVideoGain
        {
             get
             {
                 return(_S.GetStateVariableObject("BlueVideoGain").Minimum!=null);
             }
        }
        public System.UInt16 Maximum_BlueVideoGain
        {
             get
             {
                 return((System.UInt16)_S.GetStateVariableObject("BlueVideoGain").Maximum);
             }
        }
        public System.UInt16 Minimum_BlueVideoGain
        {
             get
             {
                 return((System.UInt16)_S.GetStateVariableObject("BlueVideoGain").Minimum);
             }
        }
        public bool HasStateVariable_BlueVideoGain
        {
            get
            {
               if(_S.GetStateVariableObject("BlueVideoGain")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasMaximum_Sharpness
        {
             get
             {
                 return(_S.GetStateVariableObject("Sharpness").Maximum!=null);
             }
        }
        public bool HasMinimum_Sharpness
        {
             get
             {
                 return(_S.GetStateVariableObject("Sharpness").Minimum!=null);
             }
        }
        public System.UInt16 Maximum_Sharpness
        {
             get
             {
                 return((System.UInt16)_S.GetStateVariableObject("Sharpness").Maximum);
             }
        }
        public System.UInt16 Minimum_Sharpness
        {
             get
             {
                 return((System.UInt16)_S.GetStateVariableObject("Sharpness").Minimum);
             }
        }
        public bool HasStateVariable_Sharpness
        {
            get
            {
               if(_S.GetStateVariableObject("Sharpness")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_GetHorizontalKeystone
        {
            get
            {
               if(_S.GetAction("GetHorizontalKeystone")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_GetVolume
        {
            get
            {
               if(_S.GetAction("GetVolume")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_SelectPreset
        {
            get
            {
               if(_S.GetAction("SelectPreset")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_SetVolume
        {
            get
            {
               if(_S.GetAction("SetVolume")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_ListPresets
        {
            get
            {
               if(_S.GetAction("ListPresets")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_SetVolumeDB
        {
            get
            {
               if(_S.GetAction("SetVolumeDB")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_SetRedVideoBlackLevel
        {
            get
            {
               if(_S.GetAction("SetRedVideoBlackLevel")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_SetContrast
        {
            get
            {
               if(_S.GetAction("SetContrast")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_SetLoudness
        {
            get
            {
               if(_S.GetAction("SetLoudness")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_SetBrightness
        {
            get
            {
               if(_S.GetAction("SetBrightness")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_GetLoudness
        {
            get
            {
               if(_S.GetAction("GetLoudness")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_GetColorTemperature
        {
            get
            {
               if(_S.GetAction("GetColorTemperature")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_GetSharpness
        {
            get
            {
               if(_S.GetAction("GetSharpness")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_GetContrast
        {
            get
            {
               if(_S.GetAction("GetContrast")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_GetGreenVideoGain
        {
            get
            {
               if(_S.GetAction("GetGreenVideoGain")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_SetRedVideoGain
        {
            get
            {
               if(_S.GetAction("SetRedVideoGain")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_SetGreenVideoBlackLevel
        {
            get
            {
               if(_S.GetAction("SetGreenVideoBlackLevel")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_GetRedVideoBlackLevel
        {
            get
            {
               if(_S.GetAction("GetRedVideoBlackLevel")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_GetBlueVideoBlackLevel
        {
            get
            {
               if(_S.GetAction("GetBlueVideoBlackLevel")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_GetBlueVideoGain
        {
            get
            {
               if(_S.GetAction("GetBlueVideoGain")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_SetBlueVideoBlackLevel
        {
            get
            {
               if(_S.GetAction("SetBlueVideoBlackLevel")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_GetMute
        {
            get
            {
               if(_S.GetAction("GetMute")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_SetBlueVideoGain
        {
            get
            {
               if(_S.GetAction("SetBlueVideoGain")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_GetVerticalKeystone
        {
            get
            {
               if(_S.GetAction("GetVerticalKeystone")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_SetVerticalKeystone
        {
            get
            {
               if(_S.GetAction("SetVerticalKeystone")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_GetBrightness
        {
            get
            {
               if(_S.GetAction("GetBrightness")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_GetVolumeDB
        {
            get
            {
               if(_S.GetAction("GetVolumeDB")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_GetGreenVideoBlackLevel
        {
            get
            {
               if(_S.GetAction("GetGreenVideoBlackLevel")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_GetRedVideoGain
        {
            get
            {
               if(_S.GetAction("GetRedVideoGain")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_SetMute
        {
            get
            {
               if(_S.GetAction("SetMute")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_SetGreenVideoGain
        {
            get
            {
               if(_S.GetAction("SetGreenVideoGain")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_SetSharpness
        {
            get
            {
               if(_S.GetAction("SetSharpness")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_SetHorizontalKeystone
        {
            get
            {
               if(_S.GetAction("SetHorizontalKeystone")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_SetColorTemperature
        {
            get
            {
               if(_S.GetAction("SetColorTemperature")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public void Sync_GetHorizontalKeystone(System.UInt32 InstanceID, out System.Int16 CurrentHorizontalKeystone)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentHorizontalKeystone", "");
            _S.InvokeSync("GetHorizontalKeystone", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            CurrentHorizontalKeystone = (System.Int16) args[1].DataValue;
            return;
        }
        public int GetHorizontalKeystone(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentHorizontalKeystone", "");
           return(_S.InvokeAsync("GetHorizontalKeystone", args, new UPnPService.UPnPServiceInvokeHandler(Sink_GetHorizontalKeystone), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetHorizontalKeystone)));
        }
        private void Sink_GetHorizontalKeystone(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_GetHorizontalKeystone != null)
            {
               OnResult_GetHorizontalKeystone((System.UInt32 )Args[0].DataValue, (System.Int16 )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetHorizontalKeystone.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetHorizontalKeystone)wr.Target)((System.UInt32 )Args[0].DataValue, (System.Int16 )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_GetHorizontalKeystone.Remove(wr);
                }
            }
        }
        private void Error_Sink_GetHorizontalKeystone(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_GetHorizontalKeystone != null)
            {
                 OnResult_GetHorizontalKeystone((System.UInt32 )Args[0].DataValue, (System.Int16)UPnPService.CreateObjectInstance(typeof(System.Int16),null), e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetHorizontalKeystone.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetHorizontalKeystone)wr.Target)((System.UInt32 )Args[0].DataValue, (System.Int16)UPnPService.CreateObjectInstance(typeof(System.Int16),null), e, Handle);
                }
                else
                {
                    WeakList_GetHorizontalKeystone.Remove(wr);
                }
            }
        }
        public void Sync_GetVolume(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, out System.UInt16 CurrentVolume)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           switch(Channel)
           {
               case Enum_A_ARG_TYPE_Channel.MASTER:
                   args[1] = new UPnPArgument("Channel", "Master");
                   break;
               case Enum_A_ARG_TYPE_Channel.LF:
                   args[1] = new UPnPArgument("Channel", "LF");
                   break;
               case Enum_A_ARG_TYPE_Channel.RF:
                   args[1] = new UPnPArgument("Channel", "RF");
                   break;
               case Enum_A_ARG_TYPE_Channel.CF:
                   args[1] = new UPnPArgument("Channel", "CF");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFE:
                   args[1] = new UPnPArgument("Channel", "LFE");
                   break;
               case Enum_A_ARG_TYPE_Channel.LS:
                   args[1] = new UPnPArgument("Channel", "LS");
                   break;
               case Enum_A_ARG_TYPE_Channel.RS:
                   args[1] = new UPnPArgument("Channel", "RS");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFC:
                   args[1] = new UPnPArgument("Channel", "LFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.RFC:
                   args[1] = new UPnPArgument("Channel", "RFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.SD:
                   args[1] = new UPnPArgument("Channel", "SD");
                   break;
               case Enum_A_ARG_TYPE_Channel.SL:
                   args[1] = new UPnPArgument("Channel", "SL");
                   break;
               case Enum_A_ARG_TYPE_Channel.SR_:
                   args[1] = new UPnPArgument("Channel", "SR ");
                   break;
               case Enum_A_ARG_TYPE_Channel.T:
                   args[1] = new UPnPArgument("Channel", "T");
                   break;
               case Enum_A_ARG_TYPE_Channel.B:
                   args[1] = new UPnPArgument("Channel", "B");
                   break;
               case Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED:
                   args[1] = new UPnPArgument("Channel", "Vendor defined");
                   break;
               default:
                  args[1] = new UPnPArgument("Channel", GetUnspecifiedValue("Enum_A_ARG_TYPE_Channel"));
                  break;
           }
           args[2] = new UPnPArgument("CurrentVolume", "");
            _S.InvokeSync("GetVolume", args);
            for(int i=0;i<args.Length;++i)
            {
                switch(args[i].Name)
                {
                    case "Channel":
                        switch((string)args[i].DataValue)
                        {
                            case "Master":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.MASTER;
                                break;
                            case "LF":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LF;
                                break;
                            case "RF":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.RF;
                                break;
                            case "CF":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.CF;
                                break;
                            case "LFE":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFE;
                                break;
                            case "LS":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LS;
                                break;
                            case "RS":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.RS;
                                break;
                            case "LFC":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFC;
                                break;
                            case "RFC":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.RFC;
                                break;
                            case "SD":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SD;
                                break;
                            case "SL":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SL;
                                break;
                            case "SR ":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR_;
                                break;
                            case "T":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.T;
                                break;
                            case "B":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.B;
                                break;
                            case "Vendor defined":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_Channel", (string)args[i].DataValue);
                               args[i].DataValue = Enum_A_ARG_TYPE_Channel._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            InstanceID = (System.UInt32) args[0].DataValue;
            Channel = (Enum_A_ARG_TYPE_Channel) args[1].DataValue;
            CurrentVolume = (System.UInt16) args[2].DataValue;
            return;
        }
        public int GetVolume(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           switch(Channel)
           {
               case Enum_A_ARG_TYPE_Channel.MASTER:
                   args[1] = new UPnPArgument("Channel", "Master");
                   break;
               case Enum_A_ARG_TYPE_Channel.LF:
                   args[1] = new UPnPArgument("Channel", "LF");
                   break;
               case Enum_A_ARG_TYPE_Channel.RF:
                   args[1] = new UPnPArgument("Channel", "RF");
                   break;
               case Enum_A_ARG_TYPE_Channel.CF:
                   args[1] = new UPnPArgument("Channel", "CF");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFE:
                   args[1] = new UPnPArgument("Channel", "LFE");
                   break;
               case Enum_A_ARG_TYPE_Channel.LS:
                   args[1] = new UPnPArgument("Channel", "LS");
                   break;
               case Enum_A_ARG_TYPE_Channel.RS:
                   args[1] = new UPnPArgument("Channel", "RS");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFC:
                   args[1] = new UPnPArgument("Channel", "LFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.RFC:
                   args[1] = new UPnPArgument("Channel", "RFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.SD:
                   args[1] = new UPnPArgument("Channel", "SD");
                   break;
               case Enum_A_ARG_TYPE_Channel.SL:
                   args[1] = new UPnPArgument("Channel", "SL");
                   break;
               case Enum_A_ARG_TYPE_Channel.SR_:
                   args[1] = new UPnPArgument("Channel", "SR ");
                   break;
               case Enum_A_ARG_TYPE_Channel.T:
                   args[1] = new UPnPArgument("Channel", "T");
                   break;
               case Enum_A_ARG_TYPE_Channel.B:
                   args[1] = new UPnPArgument("Channel", "B");
                   break;
               case Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED:
                   args[1] = new UPnPArgument("Channel", "Vendor defined");
                   break;
               default:
                  args[1] = new UPnPArgument("Channel", GetUnspecifiedValue("Enum_A_ARG_TYPE_Channel"));
                  break;
           }
           args[2] = new UPnPArgument("CurrentVolume", "");
           return(_S.InvokeAsync("GetVolume", args, new UPnPService.UPnPServiceInvokeHandler(Sink_GetVolume), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetVolume)));
        }
        private void Sink_GetVolume(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "Channel":
                        switch((string)Args[i].DataValue)
                        {
                            case "Master":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.MASTER;
                                break;
                            case "LF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LF;
                                break;
                            case "RF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RF;
                                break;
                            case "CF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.CF;
                                break;
                            case "LFE":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFE;
                                break;
                            case "LS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LS;
                                break;
                            case "RS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RS;
                                break;
                            case "LFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFC;
                                break;
                            case "RFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RFC;
                                break;
                            case "SD":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SD;
                                break;
                            case "SL":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SL;
                                break;
                            case "SR ":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR_;
                                break;
                            case "T":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.T;
                                break;
                            case "B":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.B;
                                break;
                            case "Vendor defined":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_Channel", (string)Args[i].DataValue);
                               Args[i].DataValue = Enum_A_ARG_TYPE_Channel._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            if(OnResult_GetVolume != null)
            {
               OnResult_GetVolume((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.UInt16 )Args[2].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetVolume.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetVolume)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.UInt16 )Args[2].DataValue, null, Handle);
                }
                else
                {
                    WeakList_GetVolume.Remove(wr);
                }
            }
        }
        private void Error_Sink_GetVolume(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "Channel":
                        switch((string)Args[i].DataValue)
                        {
                            case "Master":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.MASTER;
                                break;
                            case "LF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LF;
                                break;
                            case "RF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RF;
                                break;
                            case "CF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.CF;
                                break;
                            case "LFE":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFE;
                                break;
                            case "LS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LS;
                                break;
                            case "RS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RS;
                                break;
                            case "LFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFC;
                                break;
                            case "RFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RFC;
                                break;
                            case "SD":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SD;
                                break;
                            case "SL":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SL;
                                break;
                            case "SR ":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR_;
                                break;
                            case "T":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.T;
                                break;
                            case "B":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.B;
                                break;
                            case "Vendor defined":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED;
                                break;
                        }
                        break;
                }
            }
            if(OnResult_GetVolume != null)
            {
                 OnResult_GetVolume((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetVolume.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetVolume)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, Handle);
                }
                else
                {
                    WeakList_GetVolume.Remove(wr);
                }
            }
        }
        public void Sync_SelectPreset(System.UInt32 InstanceID, Enum_A_ARG_TYPE_PresetName PresetName)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           switch(PresetName)
           {
               case Enum_A_ARG_TYPE_PresetName.FACTORYDEFAULTS:
                   args[1] = new UPnPArgument("PresetName", "FactoryDefaults");
                   break;
               case Enum_A_ARG_TYPE_PresetName.INSTALLATIONDEFAULTS:
                   args[1] = new UPnPArgument("PresetName", "InstallationDefaults");
                   break;
               case Enum_A_ARG_TYPE_PresetName.VENDOR_DEFINED:
                   args[1] = new UPnPArgument("PresetName", "Vendor defined");
                   break;
               default:
                  args[1] = new UPnPArgument("PresetName", GetUnspecifiedValue("Enum_A_ARG_TYPE_PresetName"));
                  break;
           }
            _S.InvokeSync("SelectPreset", args);
            for(int i=0;i<args.Length;++i)
            {
                switch(args[i].Name)
                {
                    case "PresetName":
                        switch((string)args[i].DataValue)
                        {
                            case "FactoryDefaults":
                                args[i].DataValue = Enum_A_ARG_TYPE_PresetName.FACTORYDEFAULTS;
                                break;
                            case "InstallationDefaults":
                                args[i].DataValue = Enum_A_ARG_TYPE_PresetName.INSTALLATIONDEFAULTS;
                                break;
                            case "Vendor defined":
                                args[i].DataValue = Enum_A_ARG_TYPE_PresetName.VENDOR_DEFINED;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_PresetName", (string)args[i].DataValue);
                               args[i].DataValue = Enum_A_ARG_TYPE_PresetName._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            InstanceID = (System.UInt32) args[0].DataValue;
            PresetName = (Enum_A_ARG_TYPE_PresetName) args[1].DataValue;
            return;
        }
        public int SelectPreset(System.UInt32 InstanceID, Enum_A_ARG_TYPE_PresetName PresetName)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           switch(PresetName)
           {
               case Enum_A_ARG_TYPE_PresetName.FACTORYDEFAULTS:
                   args[1] = new UPnPArgument("PresetName", "FactoryDefaults");
                   break;
               case Enum_A_ARG_TYPE_PresetName.INSTALLATIONDEFAULTS:
                   args[1] = new UPnPArgument("PresetName", "InstallationDefaults");
                   break;
               case Enum_A_ARG_TYPE_PresetName.VENDOR_DEFINED:
                   args[1] = new UPnPArgument("PresetName", "Vendor defined");
                   break;
               default:
                  args[1] = new UPnPArgument("PresetName", GetUnspecifiedValue("Enum_A_ARG_TYPE_PresetName"));
                  break;
           }
           return(_S.InvokeAsync("SelectPreset", args, new UPnPService.UPnPServiceInvokeHandler(Sink_SelectPreset), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SelectPreset)));
        }
        private void Sink_SelectPreset(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "PresetName":
                        switch((string)Args[i].DataValue)
                        {
                            case "FactoryDefaults":
                                Args[i].DataValue = Enum_A_ARG_TYPE_PresetName.FACTORYDEFAULTS;
                                break;
                            case "InstallationDefaults":
                                Args[i].DataValue = Enum_A_ARG_TYPE_PresetName.INSTALLATIONDEFAULTS;
                                break;
                            case "Vendor defined":
                                Args[i].DataValue = Enum_A_ARG_TYPE_PresetName.VENDOR_DEFINED;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_PresetName", (string)Args[i].DataValue);
                               Args[i].DataValue = Enum_A_ARG_TYPE_PresetName._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            if(OnResult_SelectPreset != null)
            {
               OnResult_SelectPreset((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_PresetName )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SelectPreset.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SelectPreset)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_PresetName )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_SelectPreset.Remove(wr);
                }
            }
        }
        private void Error_Sink_SelectPreset(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "PresetName":
                        switch((string)Args[i].DataValue)
                        {
                            case "FactoryDefaults":
                                Args[i].DataValue = Enum_A_ARG_TYPE_PresetName.FACTORYDEFAULTS;
                                break;
                            case "InstallationDefaults":
                                Args[i].DataValue = Enum_A_ARG_TYPE_PresetName.INSTALLATIONDEFAULTS;
                                break;
                            case "Vendor defined":
                                Args[i].DataValue = Enum_A_ARG_TYPE_PresetName.VENDOR_DEFINED;
                                break;
                        }
                        break;
                }
            }
            if(OnResult_SelectPreset != null)
            {
                 OnResult_SelectPreset((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_PresetName )Args[1].DataValue, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SelectPreset.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SelectPreset)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_PresetName )Args[1].DataValue, e, Handle);
                }
                else
                {
                    WeakList_SelectPreset.Remove(wr);
                }
            }
        }
        public void Sync_SetVolume(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.UInt16 DesiredVolume)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           switch(Channel)
           {
               case Enum_A_ARG_TYPE_Channel.MASTER:
                   args[1] = new UPnPArgument("Channel", "Master");
                   break;
               case Enum_A_ARG_TYPE_Channel.LF:
                   args[1] = new UPnPArgument("Channel", "LF");
                   break;
               case Enum_A_ARG_TYPE_Channel.RF:
                   args[1] = new UPnPArgument("Channel", "RF");
                   break;
               case Enum_A_ARG_TYPE_Channel.CF:
                   args[1] = new UPnPArgument("Channel", "CF");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFE:
                   args[1] = new UPnPArgument("Channel", "LFE");
                   break;
               case Enum_A_ARG_TYPE_Channel.LS:
                   args[1] = new UPnPArgument("Channel", "LS");
                   break;
               case Enum_A_ARG_TYPE_Channel.RS:
                   args[1] = new UPnPArgument("Channel", "RS");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFC:
                   args[1] = new UPnPArgument("Channel", "LFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.RFC:
                   args[1] = new UPnPArgument("Channel", "RFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.SD:
                   args[1] = new UPnPArgument("Channel", "SD");
                   break;
               case Enum_A_ARG_TYPE_Channel.SL:
                   args[1] = new UPnPArgument("Channel", "SL");
                   break;
               case Enum_A_ARG_TYPE_Channel.SR_:
                   args[1] = new UPnPArgument("Channel", "SR ");
                   break;
               case Enum_A_ARG_TYPE_Channel.T:
                   args[1] = new UPnPArgument("Channel", "T");
                   break;
               case Enum_A_ARG_TYPE_Channel.B:
                   args[1] = new UPnPArgument("Channel", "B");
                   break;
               case Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED:
                   args[1] = new UPnPArgument("Channel", "Vendor defined");
                   break;
               default:
                  args[1] = new UPnPArgument("Channel", GetUnspecifiedValue("Enum_A_ARG_TYPE_Channel"));
                  break;
           }
           args[2] = new UPnPArgument("DesiredVolume", DesiredVolume);
            _S.InvokeSync("SetVolume", args);
            for(int i=0;i<args.Length;++i)
            {
                switch(args[i].Name)
                {
                    case "Channel":
                        switch((string)args[i].DataValue)
                        {
                            case "Master":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.MASTER;
                                break;
                            case "LF":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LF;
                                break;
                            case "RF":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.RF;
                                break;
                            case "CF":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.CF;
                                break;
                            case "LFE":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFE;
                                break;
                            case "LS":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LS;
                                break;
                            case "RS":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.RS;
                                break;
                            case "LFC":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFC;
                                break;
                            case "RFC":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.RFC;
                                break;
                            case "SD":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SD;
                                break;
                            case "SL":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SL;
                                break;
                            case "SR ":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR_;
                                break;
                            case "T":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.T;
                                break;
                            case "B":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.B;
                                break;
                            case "Vendor defined":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_Channel", (string)args[i].DataValue);
                               args[i].DataValue = Enum_A_ARG_TYPE_Channel._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            InstanceID = (System.UInt32) args[0].DataValue;
            Channel = (Enum_A_ARG_TYPE_Channel) args[1].DataValue;
            DesiredVolume = (System.UInt16) args[2].DataValue;
            return;
        }
        public int SetVolume(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.UInt16 DesiredVolume)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           switch(Channel)
           {
               case Enum_A_ARG_TYPE_Channel.MASTER:
                   args[1] = new UPnPArgument("Channel", "Master");
                   break;
               case Enum_A_ARG_TYPE_Channel.LF:
                   args[1] = new UPnPArgument("Channel", "LF");
                   break;
               case Enum_A_ARG_TYPE_Channel.RF:
                   args[1] = new UPnPArgument("Channel", "RF");
                   break;
               case Enum_A_ARG_TYPE_Channel.CF:
                   args[1] = new UPnPArgument("Channel", "CF");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFE:
                   args[1] = new UPnPArgument("Channel", "LFE");
                   break;
               case Enum_A_ARG_TYPE_Channel.LS:
                   args[1] = new UPnPArgument("Channel", "LS");
                   break;
               case Enum_A_ARG_TYPE_Channel.RS:
                   args[1] = new UPnPArgument("Channel", "RS");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFC:
                   args[1] = new UPnPArgument("Channel", "LFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.RFC:
                   args[1] = new UPnPArgument("Channel", "RFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.SD:
                   args[1] = new UPnPArgument("Channel", "SD");
                   break;
               case Enum_A_ARG_TYPE_Channel.SL:
                   args[1] = new UPnPArgument("Channel", "SL");
                   break;
               case Enum_A_ARG_TYPE_Channel.SR_:
                   args[1] = new UPnPArgument("Channel", "SR ");
                   break;
               case Enum_A_ARG_TYPE_Channel.T:
                   args[1] = new UPnPArgument("Channel", "T");
                   break;
               case Enum_A_ARG_TYPE_Channel.B:
                   args[1] = new UPnPArgument("Channel", "B");
                   break;
               case Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED:
                   args[1] = new UPnPArgument("Channel", "Vendor defined");
                   break;
               default:
                  args[1] = new UPnPArgument("Channel", GetUnspecifiedValue("Enum_A_ARG_TYPE_Channel"));
                  break;
           }
           args[2] = new UPnPArgument("DesiredVolume", DesiredVolume);
           return(_S.InvokeAsync("SetVolume", args, new UPnPService.UPnPServiceInvokeHandler(Sink_SetVolume), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetVolume)));
        }
        private void Sink_SetVolume(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "Channel":
                        switch((string)Args[i].DataValue)
                        {
                            case "Master":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.MASTER;
                                break;
                            case "LF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LF;
                                break;
                            case "RF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RF;
                                break;
                            case "CF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.CF;
                                break;
                            case "LFE":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFE;
                                break;
                            case "LS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LS;
                                break;
                            case "RS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RS;
                                break;
                            case "LFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFC;
                                break;
                            case "RFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RFC;
                                break;
                            case "SD":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SD;
                                break;
                            case "SL":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SL;
                                break;
                            case "SR ":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR_;
                                break;
                            case "T":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.T;
                                break;
                            case "B":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.B;
                                break;
                            case "Vendor defined":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_Channel", (string)Args[i].DataValue);
                               Args[i].DataValue = Enum_A_ARG_TYPE_Channel._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            if(OnResult_SetVolume != null)
            {
               OnResult_SetVolume((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.UInt16 )Args[2].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetVolume.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetVolume)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.UInt16 )Args[2].DataValue, null, Handle);
                }
                else
                {
                    WeakList_SetVolume.Remove(wr);
                }
            }
        }
        private void Error_Sink_SetVolume(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "Channel":
                        switch((string)Args[i].DataValue)
                        {
                            case "Master":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.MASTER;
                                break;
                            case "LF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LF;
                                break;
                            case "RF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RF;
                                break;
                            case "CF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.CF;
                                break;
                            case "LFE":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFE;
                                break;
                            case "LS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LS;
                                break;
                            case "RS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RS;
                                break;
                            case "LFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFC;
                                break;
                            case "RFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RFC;
                                break;
                            case "SD":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SD;
                                break;
                            case "SL":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SL;
                                break;
                            case "SR ":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR_;
                                break;
                            case "T":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.T;
                                break;
                            case "B":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.B;
                                break;
                            case "Vendor defined":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED;
                                break;
                        }
                        break;
                }
            }
            if(OnResult_SetVolume != null)
            {
                 OnResult_SetVolume((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.UInt16 )Args[2].DataValue, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetVolume.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetVolume)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.UInt16 )Args[2].DataValue, e, Handle);
                }
                else
                {
                    WeakList_SetVolume.Remove(wr);
                }
            }
        }
        public void Sync_ListPresets(System.UInt32 InstanceID, out System.String CurrentPresetNameList)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentPresetNameList", "");
            _S.InvokeSync("ListPresets", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            CurrentPresetNameList = (System.String) args[1].DataValue;
            return;
        }
        public int ListPresets(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentPresetNameList", "");
           return(_S.InvokeAsync("ListPresets", args, new UPnPService.UPnPServiceInvokeHandler(Sink_ListPresets), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_ListPresets)));
        }
        private void Sink_ListPresets(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_ListPresets != null)
            {
               OnResult_ListPresets((System.UInt32 )Args[0].DataValue, (System.String )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_ListPresets.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_ListPresets)wr.Target)((System.UInt32 )Args[0].DataValue, (System.String )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_ListPresets.Remove(wr);
                }
            }
        }
        private void Error_Sink_ListPresets(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_ListPresets != null)
            {
                 OnResult_ListPresets((System.UInt32 )Args[0].DataValue, (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_ListPresets.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_ListPresets)wr.Target)((System.UInt32 )Args[0].DataValue, (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), e, Handle);
                }
                else
                {
                    WeakList_ListPresets.Remove(wr);
                }
            }
        }
        public void Sync_SetVolumeDB(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.Int16 DesiredVolume)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           switch(Channel)
           {
               case Enum_A_ARG_TYPE_Channel.MASTER:
                   args[1] = new UPnPArgument("Channel", "Master");
                   break;
               case Enum_A_ARG_TYPE_Channel.LF:
                   args[1] = new UPnPArgument("Channel", "LF");
                   break;
               case Enum_A_ARG_TYPE_Channel.RF:
                   args[1] = new UPnPArgument("Channel", "RF");
                   break;
               case Enum_A_ARG_TYPE_Channel.CF:
                   args[1] = new UPnPArgument("Channel", "CF");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFE:
                   args[1] = new UPnPArgument("Channel", "LFE");
                   break;
               case Enum_A_ARG_TYPE_Channel.LS:
                   args[1] = new UPnPArgument("Channel", "LS");
                   break;
               case Enum_A_ARG_TYPE_Channel.RS:
                   args[1] = new UPnPArgument("Channel", "RS");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFC:
                   args[1] = new UPnPArgument("Channel", "LFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.RFC:
                   args[1] = new UPnPArgument("Channel", "RFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.SD:
                   args[1] = new UPnPArgument("Channel", "SD");
                   break;
               case Enum_A_ARG_TYPE_Channel.SL:
                   args[1] = new UPnPArgument("Channel", "SL");
                   break;
               case Enum_A_ARG_TYPE_Channel.SR_:
                   args[1] = new UPnPArgument("Channel", "SR ");
                   break;
               case Enum_A_ARG_TYPE_Channel.T:
                   args[1] = new UPnPArgument("Channel", "T");
                   break;
               case Enum_A_ARG_TYPE_Channel.B:
                   args[1] = new UPnPArgument("Channel", "B");
                   break;
               case Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED:
                   args[1] = new UPnPArgument("Channel", "Vendor defined");
                   break;
               default:
                  args[1] = new UPnPArgument("Channel", GetUnspecifiedValue("Enum_A_ARG_TYPE_Channel"));
                  break;
           }
           args[2] = new UPnPArgument("DesiredVolume", DesiredVolume);
            _S.InvokeSync("SetVolumeDB", args);
            for(int i=0;i<args.Length;++i)
            {
                switch(args[i].Name)
                {
                    case "Channel":
                        switch((string)args[i].DataValue)
                        {
                            case "Master":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.MASTER;
                                break;
                            case "LF":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LF;
                                break;
                            case "RF":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.RF;
                                break;
                            case "CF":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.CF;
                                break;
                            case "LFE":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFE;
                                break;
                            case "LS":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LS;
                                break;
                            case "RS":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.RS;
                                break;
                            case "LFC":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFC;
                                break;
                            case "RFC":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.RFC;
                                break;
                            case "SD":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SD;
                                break;
                            case "SL":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SL;
                                break;
                            case "SR ":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR_;
                                break;
                            case "T":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.T;
                                break;
                            case "B":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.B;
                                break;
                            case "Vendor defined":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_Channel", (string)args[i].DataValue);
                               args[i].DataValue = Enum_A_ARG_TYPE_Channel._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            InstanceID = (System.UInt32) args[0].DataValue;
            Channel = (Enum_A_ARG_TYPE_Channel) args[1].DataValue;
            DesiredVolume = (System.Int16) args[2].DataValue;
            return;
        }
        public int SetVolumeDB(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.Int16 DesiredVolume)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           switch(Channel)
           {
               case Enum_A_ARG_TYPE_Channel.MASTER:
                   args[1] = new UPnPArgument("Channel", "Master");
                   break;
               case Enum_A_ARG_TYPE_Channel.LF:
                   args[1] = new UPnPArgument("Channel", "LF");
                   break;
               case Enum_A_ARG_TYPE_Channel.RF:
                   args[1] = new UPnPArgument("Channel", "RF");
                   break;
               case Enum_A_ARG_TYPE_Channel.CF:
                   args[1] = new UPnPArgument("Channel", "CF");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFE:
                   args[1] = new UPnPArgument("Channel", "LFE");
                   break;
               case Enum_A_ARG_TYPE_Channel.LS:
                   args[1] = new UPnPArgument("Channel", "LS");
                   break;
               case Enum_A_ARG_TYPE_Channel.RS:
                   args[1] = new UPnPArgument("Channel", "RS");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFC:
                   args[1] = new UPnPArgument("Channel", "LFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.RFC:
                   args[1] = new UPnPArgument("Channel", "RFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.SD:
                   args[1] = new UPnPArgument("Channel", "SD");
                   break;
               case Enum_A_ARG_TYPE_Channel.SL:
                   args[1] = new UPnPArgument("Channel", "SL");
                   break;
               case Enum_A_ARG_TYPE_Channel.SR_:
                   args[1] = new UPnPArgument("Channel", "SR ");
                   break;
               case Enum_A_ARG_TYPE_Channel.T:
                   args[1] = new UPnPArgument("Channel", "T");
                   break;
               case Enum_A_ARG_TYPE_Channel.B:
                   args[1] = new UPnPArgument("Channel", "B");
                   break;
               case Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED:
                   args[1] = new UPnPArgument("Channel", "Vendor defined");
                   break;
               default:
                  args[1] = new UPnPArgument("Channel", GetUnspecifiedValue("Enum_A_ARG_TYPE_Channel"));
                  break;
           }
           args[2] = new UPnPArgument("DesiredVolume", DesiredVolume);
           return(_S.InvokeAsync("SetVolumeDB", args, new UPnPService.UPnPServiceInvokeHandler(Sink_SetVolumeDB), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetVolumeDB)));
        }
        private void Sink_SetVolumeDB(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "Channel":
                        switch((string)Args[i].DataValue)
                        {
                            case "Master":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.MASTER;
                                break;
                            case "LF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LF;
                                break;
                            case "RF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RF;
                                break;
                            case "CF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.CF;
                                break;
                            case "LFE":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFE;
                                break;
                            case "LS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LS;
                                break;
                            case "RS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RS;
                                break;
                            case "LFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFC;
                                break;
                            case "RFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RFC;
                                break;
                            case "SD":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SD;
                                break;
                            case "SL":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SL;
                                break;
                            case "SR ":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR_;
                                break;
                            case "T":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.T;
                                break;
                            case "B":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.B;
                                break;
                            case "Vendor defined":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_Channel", (string)Args[i].DataValue);
                               Args[i].DataValue = Enum_A_ARG_TYPE_Channel._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            if(OnResult_SetVolumeDB != null)
            {
               OnResult_SetVolumeDB((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Int16 )Args[2].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetVolumeDB.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetVolumeDB)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Int16 )Args[2].DataValue, null, Handle);
                }
                else
                {
                    WeakList_SetVolumeDB.Remove(wr);
                }
            }
        }
        private void Error_Sink_SetVolumeDB(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "Channel":
                        switch((string)Args[i].DataValue)
                        {
                            case "Master":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.MASTER;
                                break;
                            case "LF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LF;
                                break;
                            case "RF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RF;
                                break;
                            case "CF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.CF;
                                break;
                            case "LFE":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFE;
                                break;
                            case "LS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LS;
                                break;
                            case "RS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RS;
                                break;
                            case "LFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFC;
                                break;
                            case "RFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RFC;
                                break;
                            case "SD":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SD;
                                break;
                            case "SL":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SL;
                                break;
                            case "SR ":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR_;
                                break;
                            case "T":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.T;
                                break;
                            case "B":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.B;
                                break;
                            case "Vendor defined":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED;
                                break;
                        }
                        break;
                }
            }
            if(OnResult_SetVolumeDB != null)
            {
                 OnResult_SetVolumeDB((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Int16 )Args[2].DataValue, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetVolumeDB.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetVolumeDB)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Int16 )Args[2].DataValue, e, Handle);
                }
                else
                {
                    WeakList_SetVolumeDB.Remove(wr);
                }
            }
        }
        public void Sync_SetRedVideoBlackLevel(System.UInt32 InstanceID, System.UInt16 DesiredRedVideoBlackLevel)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredRedVideoBlackLevel", DesiredRedVideoBlackLevel);
            _S.InvokeSync("SetRedVideoBlackLevel", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            DesiredRedVideoBlackLevel = (System.UInt16) args[1].DataValue;
            return;
        }
        public int SetRedVideoBlackLevel(System.UInt32 InstanceID, System.UInt16 DesiredRedVideoBlackLevel)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredRedVideoBlackLevel", DesiredRedVideoBlackLevel);
           return(_S.InvokeAsync("SetRedVideoBlackLevel", args, new UPnPService.UPnPServiceInvokeHandler(Sink_SetRedVideoBlackLevel), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetRedVideoBlackLevel)));
        }
        private void Sink_SetRedVideoBlackLevel(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_SetRedVideoBlackLevel != null)
            {
               OnResult_SetRedVideoBlackLevel((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetRedVideoBlackLevel.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetRedVideoBlackLevel)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_SetRedVideoBlackLevel.Remove(wr);
                }
            }
        }
        private void Error_Sink_SetRedVideoBlackLevel(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_SetRedVideoBlackLevel != null)
            {
                 OnResult_SetRedVideoBlackLevel((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetRedVideoBlackLevel.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetRedVideoBlackLevel)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, Handle);
                }
                else
                {
                    WeakList_SetRedVideoBlackLevel.Remove(wr);
                }
            }
        }
        public void Sync_SetContrast(System.UInt32 InstanceID, System.UInt16 DesiredContrast)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredContrast", DesiredContrast);
            _S.InvokeSync("SetContrast", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            DesiredContrast = (System.UInt16) args[1].DataValue;
            return;
        }
        public int SetContrast(System.UInt32 InstanceID, System.UInt16 DesiredContrast)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredContrast", DesiredContrast);
           return(_S.InvokeAsync("SetContrast", args, new UPnPService.UPnPServiceInvokeHandler(Sink_SetContrast), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetContrast)));
        }
        private void Sink_SetContrast(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_SetContrast != null)
            {
               OnResult_SetContrast((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetContrast.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetContrast)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_SetContrast.Remove(wr);
                }
            }
        }
        private void Error_Sink_SetContrast(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_SetContrast != null)
            {
                 OnResult_SetContrast((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetContrast.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetContrast)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, Handle);
                }
                else
                {
                    WeakList_SetContrast.Remove(wr);
                }
            }
        }
        public void Sync_SetLoudness(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.Boolean DesiredLoudness)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           switch(Channel)
           {
               case Enum_A_ARG_TYPE_Channel.MASTER:
                   args[1] = new UPnPArgument("Channel", "Master");
                   break;
               case Enum_A_ARG_TYPE_Channel.LF:
                   args[1] = new UPnPArgument("Channel", "LF");
                   break;
               case Enum_A_ARG_TYPE_Channel.RF:
                   args[1] = new UPnPArgument("Channel", "RF");
                   break;
               case Enum_A_ARG_TYPE_Channel.CF:
                   args[1] = new UPnPArgument("Channel", "CF");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFE:
                   args[1] = new UPnPArgument("Channel", "LFE");
                   break;
               case Enum_A_ARG_TYPE_Channel.LS:
                   args[1] = new UPnPArgument("Channel", "LS");
                   break;
               case Enum_A_ARG_TYPE_Channel.RS:
                   args[1] = new UPnPArgument("Channel", "RS");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFC:
                   args[1] = new UPnPArgument("Channel", "LFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.RFC:
                   args[1] = new UPnPArgument("Channel", "RFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.SD:
                   args[1] = new UPnPArgument("Channel", "SD");
                   break;
               case Enum_A_ARG_TYPE_Channel.SL:
                   args[1] = new UPnPArgument("Channel", "SL");
                   break;
               case Enum_A_ARG_TYPE_Channel.SR_:
                   args[1] = new UPnPArgument("Channel", "SR ");
                   break;
               case Enum_A_ARG_TYPE_Channel.T:
                   args[1] = new UPnPArgument("Channel", "T");
                   break;
               case Enum_A_ARG_TYPE_Channel.B:
                   args[1] = new UPnPArgument("Channel", "B");
                   break;
               case Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED:
                   args[1] = new UPnPArgument("Channel", "Vendor defined");
                   break;
               default:
                  args[1] = new UPnPArgument("Channel", GetUnspecifiedValue("Enum_A_ARG_TYPE_Channel"));
                  break;
           }
           args[2] = new UPnPArgument("DesiredLoudness", DesiredLoudness);
            _S.InvokeSync("SetLoudness", args);
            for(int i=0;i<args.Length;++i)
            {
                switch(args[i].Name)
                {
                    case "Channel":
                        switch((string)args[i].DataValue)
                        {
                            case "Master":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.MASTER;
                                break;
                            case "LF":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LF;
                                break;
                            case "RF":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.RF;
                                break;
                            case "CF":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.CF;
                                break;
                            case "LFE":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFE;
                                break;
                            case "LS":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LS;
                                break;
                            case "RS":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.RS;
                                break;
                            case "LFC":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFC;
                                break;
                            case "RFC":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.RFC;
                                break;
                            case "SD":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SD;
                                break;
                            case "SL":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SL;
                                break;
                            case "SR ":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR_;
                                break;
                            case "T":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.T;
                                break;
                            case "B":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.B;
                                break;
                            case "Vendor defined":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_Channel", (string)args[i].DataValue);
                               args[i].DataValue = Enum_A_ARG_TYPE_Channel._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            InstanceID = (System.UInt32) args[0].DataValue;
            Channel = (Enum_A_ARG_TYPE_Channel) args[1].DataValue;
            DesiredLoudness = (System.Boolean) args[2].DataValue;
            return;
        }
        public int SetLoudness(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.Boolean DesiredLoudness)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           switch(Channel)
           {
               case Enum_A_ARG_TYPE_Channel.MASTER:
                   args[1] = new UPnPArgument("Channel", "Master");
                   break;
               case Enum_A_ARG_TYPE_Channel.LF:
                   args[1] = new UPnPArgument("Channel", "LF");
                   break;
               case Enum_A_ARG_TYPE_Channel.RF:
                   args[1] = new UPnPArgument("Channel", "RF");
                   break;
               case Enum_A_ARG_TYPE_Channel.CF:
                   args[1] = new UPnPArgument("Channel", "CF");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFE:
                   args[1] = new UPnPArgument("Channel", "LFE");
                   break;
               case Enum_A_ARG_TYPE_Channel.LS:
                   args[1] = new UPnPArgument("Channel", "LS");
                   break;
               case Enum_A_ARG_TYPE_Channel.RS:
                   args[1] = new UPnPArgument("Channel", "RS");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFC:
                   args[1] = new UPnPArgument("Channel", "LFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.RFC:
                   args[1] = new UPnPArgument("Channel", "RFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.SD:
                   args[1] = new UPnPArgument("Channel", "SD");
                   break;
               case Enum_A_ARG_TYPE_Channel.SL:
                   args[1] = new UPnPArgument("Channel", "SL");
                   break;
               case Enum_A_ARG_TYPE_Channel.SR_:
                   args[1] = new UPnPArgument("Channel", "SR ");
                   break;
               case Enum_A_ARG_TYPE_Channel.T:
                   args[1] = new UPnPArgument("Channel", "T");
                   break;
               case Enum_A_ARG_TYPE_Channel.B:
                   args[1] = new UPnPArgument("Channel", "B");
                   break;
               case Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED:
                   args[1] = new UPnPArgument("Channel", "Vendor defined");
                   break;
               default:
                  args[1] = new UPnPArgument("Channel", GetUnspecifiedValue("Enum_A_ARG_TYPE_Channel"));
                  break;
           }
           args[2] = new UPnPArgument("DesiredLoudness", DesiredLoudness);
           return(_S.InvokeAsync("SetLoudness", args, new UPnPService.UPnPServiceInvokeHandler(Sink_SetLoudness), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetLoudness)));
        }
        private void Sink_SetLoudness(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "Channel":
                        switch((string)Args[i].DataValue)
                        {
                            case "Master":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.MASTER;
                                break;
                            case "LF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LF;
                                break;
                            case "RF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RF;
                                break;
                            case "CF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.CF;
                                break;
                            case "LFE":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFE;
                                break;
                            case "LS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LS;
                                break;
                            case "RS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RS;
                                break;
                            case "LFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFC;
                                break;
                            case "RFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RFC;
                                break;
                            case "SD":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SD;
                                break;
                            case "SL":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SL;
                                break;
                            case "SR ":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR_;
                                break;
                            case "T":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.T;
                                break;
                            case "B":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.B;
                                break;
                            case "Vendor defined":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_Channel", (string)Args[i].DataValue);
                               Args[i].DataValue = Enum_A_ARG_TYPE_Channel._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            if(OnResult_SetLoudness != null)
            {
               OnResult_SetLoudness((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean )Args[2].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetLoudness.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetLoudness)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean )Args[2].DataValue, null, Handle);
                }
                else
                {
                    WeakList_SetLoudness.Remove(wr);
                }
            }
        }
        private void Error_Sink_SetLoudness(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "Channel":
                        switch((string)Args[i].DataValue)
                        {
                            case "Master":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.MASTER;
                                break;
                            case "LF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LF;
                                break;
                            case "RF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RF;
                                break;
                            case "CF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.CF;
                                break;
                            case "LFE":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFE;
                                break;
                            case "LS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LS;
                                break;
                            case "RS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RS;
                                break;
                            case "LFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFC;
                                break;
                            case "RFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RFC;
                                break;
                            case "SD":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SD;
                                break;
                            case "SL":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SL;
                                break;
                            case "SR ":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR_;
                                break;
                            case "T":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.T;
                                break;
                            case "B":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.B;
                                break;
                            case "Vendor defined":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED;
                                break;
                        }
                        break;
                }
            }
            if(OnResult_SetLoudness != null)
            {
                 OnResult_SetLoudness((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean )Args[2].DataValue, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetLoudness.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetLoudness)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean )Args[2].DataValue, e, Handle);
                }
                else
                {
                    WeakList_SetLoudness.Remove(wr);
                }
            }
        }
        public void Sync_SetBrightness(System.UInt32 InstanceID, System.UInt16 DesiredBrightness)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredBrightness", DesiredBrightness);
            _S.InvokeSync("SetBrightness", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            DesiredBrightness = (System.UInt16) args[1].DataValue;
            return;
        }
        public int SetBrightness(System.UInt32 InstanceID, System.UInt16 DesiredBrightness)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredBrightness", DesiredBrightness);
           return(_S.InvokeAsync("SetBrightness", args, new UPnPService.UPnPServiceInvokeHandler(Sink_SetBrightness), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetBrightness)));
        }
        private void Sink_SetBrightness(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_SetBrightness != null)
            {
               OnResult_SetBrightness((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetBrightness.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetBrightness)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_SetBrightness.Remove(wr);
                }
            }
        }
        private void Error_Sink_SetBrightness(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_SetBrightness != null)
            {
                 OnResult_SetBrightness((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetBrightness.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetBrightness)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, Handle);
                }
                else
                {
                    WeakList_SetBrightness.Remove(wr);
                }
            }
        }
        public void Sync_GetLoudness(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, out System.Boolean CurrentLoudness)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           switch(Channel)
           {
               case Enum_A_ARG_TYPE_Channel.MASTER:
                   args[1] = new UPnPArgument("Channel", "Master");
                   break;
               case Enum_A_ARG_TYPE_Channel.LF:
                   args[1] = new UPnPArgument("Channel", "LF");
                   break;
               case Enum_A_ARG_TYPE_Channel.RF:
                   args[1] = new UPnPArgument("Channel", "RF");
                   break;
               case Enum_A_ARG_TYPE_Channel.CF:
                   args[1] = new UPnPArgument("Channel", "CF");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFE:
                   args[1] = new UPnPArgument("Channel", "LFE");
                   break;
               case Enum_A_ARG_TYPE_Channel.LS:
                   args[1] = new UPnPArgument("Channel", "LS");
                   break;
               case Enum_A_ARG_TYPE_Channel.RS:
                   args[1] = new UPnPArgument("Channel", "RS");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFC:
                   args[1] = new UPnPArgument("Channel", "LFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.RFC:
                   args[1] = new UPnPArgument("Channel", "RFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.SD:
                   args[1] = new UPnPArgument("Channel", "SD");
                   break;
               case Enum_A_ARG_TYPE_Channel.SL:
                   args[1] = new UPnPArgument("Channel", "SL");
                   break;
               case Enum_A_ARG_TYPE_Channel.SR_:
                   args[1] = new UPnPArgument("Channel", "SR ");
                   break;
               case Enum_A_ARG_TYPE_Channel.T:
                   args[1] = new UPnPArgument("Channel", "T");
                   break;
               case Enum_A_ARG_TYPE_Channel.B:
                   args[1] = new UPnPArgument("Channel", "B");
                   break;
               case Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED:
                   args[1] = new UPnPArgument("Channel", "Vendor defined");
                   break;
               default:
                  args[1] = new UPnPArgument("Channel", GetUnspecifiedValue("Enum_A_ARG_TYPE_Channel"));
                  break;
           }
           args[2] = new UPnPArgument("CurrentLoudness", "");
            _S.InvokeSync("GetLoudness", args);
            for(int i=0;i<args.Length;++i)
            {
                switch(args[i].Name)
                {
                    case "Channel":
                        switch((string)args[i].DataValue)
                        {
                            case "Master":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.MASTER;
                                break;
                            case "LF":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LF;
                                break;
                            case "RF":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.RF;
                                break;
                            case "CF":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.CF;
                                break;
                            case "LFE":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFE;
                                break;
                            case "LS":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LS;
                                break;
                            case "RS":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.RS;
                                break;
                            case "LFC":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFC;
                                break;
                            case "RFC":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.RFC;
                                break;
                            case "SD":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SD;
                                break;
                            case "SL":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SL;
                                break;
                            case "SR ":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR_;
                                break;
                            case "T":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.T;
                                break;
                            case "B":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.B;
                                break;
                            case "Vendor defined":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_Channel", (string)args[i].DataValue);
                               args[i].DataValue = Enum_A_ARG_TYPE_Channel._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            InstanceID = (System.UInt32) args[0].DataValue;
            Channel = (Enum_A_ARG_TYPE_Channel) args[1].DataValue;
            CurrentLoudness = (System.Boolean) args[2].DataValue;
            return;
        }
        public int GetLoudness(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           switch(Channel)
           {
               case Enum_A_ARG_TYPE_Channel.MASTER:
                   args[1] = new UPnPArgument("Channel", "Master");
                   break;
               case Enum_A_ARG_TYPE_Channel.LF:
                   args[1] = new UPnPArgument("Channel", "LF");
                   break;
               case Enum_A_ARG_TYPE_Channel.RF:
                   args[1] = new UPnPArgument("Channel", "RF");
                   break;
               case Enum_A_ARG_TYPE_Channel.CF:
                   args[1] = new UPnPArgument("Channel", "CF");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFE:
                   args[1] = new UPnPArgument("Channel", "LFE");
                   break;
               case Enum_A_ARG_TYPE_Channel.LS:
                   args[1] = new UPnPArgument("Channel", "LS");
                   break;
               case Enum_A_ARG_TYPE_Channel.RS:
                   args[1] = new UPnPArgument("Channel", "RS");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFC:
                   args[1] = new UPnPArgument("Channel", "LFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.RFC:
                   args[1] = new UPnPArgument("Channel", "RFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.SD:
                   args[1] = new UPnPArgument("Channel", "SD");
                   break;
               case Enum_A_ARG_TYPE_Channel.SL:
                   args[1] = new UPnPArgument("Channel", "SL");
                   break;
               case Enum_A_ARG_TYPE_Channel.SR_:
                   args[1] = new UPnPArgument("Channel", "SR ");
                   break;
               case Enum_A_ARG_TYPE_Channel.T:
                   args[1] = new UPnPArgument("Channel", "T");
                   break;
               case Enum_A_ARG_TYPE_Channel.B:
                   args[1] = new UPnPArgument("Channel", "B");
                   break;
               case Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED:
                   args[1] = new UPnPArgument("Channel", "Vendor defined");
                   break;
               default:
                  args[1] = new UPnPArgument("Channel", GetUnspecifiedValue("Enum_A_ARG_TYPE_Channel"));
                  break;
           }
           args[2] = new UPnPArgument("CurrentLoudness", "");
           return(_S.InvokeAsync("GetLoudness", args, new UPnPService.UPnPServiceInvokeHandler(Sink_GetLoudness), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetLoudness)));
        }
        private void Sink_GetLoudness(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "Channel":
                        switch((string)Args[i].DataValue)
                        {
                            case "Master":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.MASTER;
                                break;
                            case "LF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LF;
                                break;
                            case "RF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RF;
                                break;
                            case "CF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.CF;
                                break;
                            case "LFE":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFE;
                                break;
                            case "LS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LS;
                                break;
                            case "RS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RS;
                                break;
                            case "LFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFC;
                                break;
                            case "RFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RFC;
                                break;
                            case "SD":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SD;
                                break;
                            case "SL":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SL;
                                break;
                            case "SR ":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR_;
                                break;
                            case "T":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.T;
                                break;
                            case "B":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.B;
                                break;
                            case "Vendor defined":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_Channel", (string)Args[i].DataValue);
                               Args[i].DataValue = Enum_A_ARG_TYPE_Channel._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            if(OnResult_GetLoudness != null)
            {
               OnResult_GetLoudness((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean )Args[2].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetLoudness.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetLoudness)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean )Args[2].DataValue, null, Handle);
                }
                else
                {
                    WeakList_GetLoudness.Remove(wr);
                }
            }
        }
        private void Error_Sink_GetLoudness(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "Channel":
                        switch((string)Args[i].DataValue)
                        {
                            case "Master":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.MASTER;
                                break;
                            case "LF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LF;
                                break;
                            case "RF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RF;
                                break;
                            case "CF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.CF;
                                break;
                            case "LFE":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFE;
                                break;
                            case "LS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LS;
                                break;
                            case "RS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RS;
                                break;
                            case "LFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFC;
                                break;
                            case "RFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RFC;
                                break;
                            case "SD":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SD;
                                break;
                            case "SL":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SL;
                                break;
                            case "SR ":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR_;
                                break;
                            case "T":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.T;
                                break;
                            case "B":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.B;
                                break;
                            case "Vendor defined":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED;
                                break;
                        }
                        break;
                }
            }
            if(OnResult_GetLoudness != null)
            {
                 OnResult_GetLoudness((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean)UPnPService.CreateObjectInstance(typeof(System.Boolean),null), e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetLoudness.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetLoudness)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean)UPnPService.CreateObjectInstance(typeof(System.Boolean),null), e, Handle);
                }
                else
                {
                    WeakList_GetLoudness.Remove(wr);
                }
            }
        }
        public void Sync_GetColorTemperature(System.UInt32 InstanceID, out System.UInt16 CurrentColorTemperature)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentColorTemperature", "");
            _S.InvokeSync("GetColorTemperature", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            CurrentColorTemperature = (System.UInt16) args[1].DataValue;
            return;
        }
        public int GetColorTemperature(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentColorTemperature", "");
           return(_S.InvokeAsync("GetColorTemperature", args, new UPnPService.UPnPServiceInvokeHandler(Sink_GetColorTemperature), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetColorTemperature)));
        }
        private void Sink_GetColorTemperature(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_GetColorTemperature != null)
            {
               OnResult_GetColorTemperature((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetColorTemperature.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetColorTemperature)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_GetColorTemperature.Remove(wr);
                }
            }
        }
        private void Error_Sink_GetColorTemperature(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_GetColorTemperature != null)
            {
                 OnResult_GetColorTemperature((System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetColorTemperature.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetColorTemperature)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, Handle);
                }
                else
                {
                    WeakList_GetColorTemperature.Remove(wr);
                }
            }
        }
        public void Sync_GetSharpness(System.UInt32 InstanceID, out System.UInt16 CurrentSharpness)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentSharpness", "");
            _S.InvokeSync("GetSharpness", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            CurrentSharpness = (System.UInt16) args[1].DataValue;
            return;
        }
        public int GetSharpness(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentSharpness", "");
           return(_S.InvokeAsync("GetSharpness", args, new UPnPService.UPnPServiceInvokeHandler(Sink_GetSharpness), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetSharpness)));
        }
        private void Sink_GetSharpness(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_GetSharpness != null)
            {
               OnResult_GetSharpness((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetSharpness.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetSharpness)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_GetSharpness.Remove(wr);
                }
            }
        }
        private void Error_Sink_GetSharpness(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_GetSharpness != null)
            {
                 OnResult_GetSharpness((System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetSharpness.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetSharpness)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, Handle);
                }
                else
                {
                    WeakList_GetSharpness.Remove(wr);
                }
            }
        }
        public void Sync_GetContrast(System.UInt32 InstanceID, out System.UInt16 CurrentContrast)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentContrast", "");
            _S.InvokeSync("GetContrast", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            CurrentContrast = (System.UInt16) args[1].DataValue;
            return;
        }
        public int GetContrast(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentContrast", "");
           return(_S.InvokeAsync("GetContrast", args, new UPnPService.UPnPServiceInvokeHandler(Sink_GetContrast), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetContrast)));
        }
        private void Sink_GetContrast(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_GetContrast != null)
            {
               OnResult_GetContrast((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetContrast.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetContrast)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_GetContrast.Remove(wr);
                }
            }
        }
        private void Error_Sink_GetContrast(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_GetContrast != null)
            {
                 OnResult_GetContrast((System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetContrast.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetContrast)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, Handle);
                }
                else
                {
                    WeakList_GetContrast.Remove(wr);
                }
            }
        }
        public void Sync_GetGreenVideoGain(System.UInt32 InstanceID, out System.UInt16 CurrentGreenVideoGain)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentGreenVideoGain", "");
            _S.InvokeSync("GetGreenVideoGain", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            CurrentGreenVideoGain = (System.UInt16) args[1].DataValue;
            return;
        }
        public int GetGreenVideoGain(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentGreenVideoGain", "");
           return(_S.InvokeAsync("GetGreenVideoGain", args, new UPnPService.UPnPServiceInvokeHandler(Sink_GetGreenVideoGain), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetGreenVideoGain)));
        }
        private void Sink_GetGreenVideoGain(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_GetGreenVideoGain != null)
            {
               OnResult_GetGreenVideoGain((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetGreenVideoGain.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetGreenVideoGain)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_GetGreenVideoGain.Remove(wr);
                }
            }
        }
        private void Error_Sink_GetGreenVideoGain(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_GetGreenVideoGain != null)
            {
                 OnResult_GetGreenVideoGain((System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetGreenVideoGain.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetGreenVideoGain)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, Handle);
                }
                else
                {
                    WeakList_GetGreenVideoGain.Remove(wr);
                }
            }
        }
        public void Sync_SetRedVideoGain(System.UInt32 InstanceID, System.UInt16 DesiredRedVideoGain)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredRedVideoGain", DesiredRedVideoGain);
            _S.InvokeSync("SetRedVideoGain", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            DesiredRedVideoGain = (System.UInt16) args[1].DataValue;
            return;
        }
        public int SetRedVideoGain(System.UInt32 InstanceID, System.UInt16 DesiredRedVideoGain)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredRedVideoGain", DesiredRedVideoGain);
           return(_S.InvokeAsync("SetRedVideoGain", args, new UPnPService.UPnPServiceInvokeHandler(Sink_SetRedVideoGain), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetRedVideoGain)));
        }
        private void Sink_SetRedVideoGain(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_SetRedVideoGain != null)
            {
               OnResult_SetRedVideoGain((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetRedVideoGain.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetRedVideoGain)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_SetRedVideoGain.Remove(wr);
                }
            }
        }
        private void Error_Sink_SetRedVideoGain(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_SetRedVideoGain != null)
            {
                 OnResult_SetRedVideoGain((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetRedVideoGain.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetRedVideoGain)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, Handle);
                }
                else
                {
                    WeakList_SetRedVideoGain.Remove(wr);
                }
            }
        }
        public void Sync_SetGreenVideoBlackLevel(System.UInt32 InstanceID, System.UInt16 DesiredGreenVideoBlackLevel)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredGreenVideoBlackLevel", DesiredGreenVideoBlackLevel);
            _S.InvokeSync("SetGreenVideoBlackLevel", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            DesiredGreenVideoBlackLevel = (System.UInt16) args[1].DataValue;
            return;
        }
        public int SetGreenVideoBlackLevel(System.UInt32 InstanceID, System.UInt16 DesiredGreenVideoBlackLevel)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredGreenVideoBlackLevel", DesiredGreenVideoBlackLevel);
           return(_S.InvokeAsync("SetGreenVideoBlackLevel", args, new UPnPService.UPnPServiceInvokeHandler(Sink_SetGreenVideoBlackLevel), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetGreenVideoBlackLevel)));
        }
        private void Sink_SetGreenVideoBlackLevel(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_SetGreenVideoBlackLevel != null)
            {
               OnResult_SetGreenVideoBlackLevel((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetGreenVideoBlackLevel.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetGreenVideoBlackLevel)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_SetGreenVideoBlackLevel.Remove(wr);
                }
            }
        }
        private void Error_Sink_SetGreenVideoBlackLevel(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_SetGreenVideoBlackLevel != null)
            {
                 OnResult_SetGreenVideoBlackLevel((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetGreenVideoBlackLevel.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetGreenVideoBlackLevel)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, Handle);
                }
                else
                {
                    WeakList_SetGreenVideoBlackLevel.Remove(wr);
                }
            }
        }
        public void Sync_GetRedVideoBlackLevel(System.UInt32 InstanceID, out System.UInt16 CurrentRedVideoBlackLevel)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentRedVideoBlackLevel", "");
            _S.InvokeSync("GetRedVideoBlackLevel", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            CurrentRedVideoBlackLevel = (System.UInt16) args[1].DataValue;
            return;
        }
        public int GetRedVideoBlackLevel(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentRedVideoBlackLevel", "");
           return(_S.InvokeAsync("GetRedVideoBlackLevel", args, new UPnPService.UPnPServiceInvokeHandler(Sink_GetRedVideoBlackLevel), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetRedVideoBlackLevel)));
        }
        private void Sink_GetRedVideoBlackLevel(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_GetRedVideoBlackLevel != null)
            {
               OnResult_GetRedVideoBlackLevel((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetRedVideoBlackLevel.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetRedVideoBlackLevel)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_GetRedVideoBlackLevel.Remove(wr);
                }
            }
        }
        private void Error_Sink_GetRedVideoBlackLevel(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_GetRedVideoBlackLevel != null)
            {
                 OnResult_GetRedVideoBlackLevel((System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetRedVideoBlackLevel.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetRedVideoBlackLevel)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, Handle);
                }
                else
                {
                    WeakList_GetRedVideoBlackLevel.Remove(wr);
                }
            }
        }
        public void Sync_GetBlueVideoBlackLevel(System.UInt32 InstanceID, out System.UInt16 CurrentBlueVideoBlackLevel)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentBlueVideoBlackLevel", "");
            _S.InvokeSync("GetBlueVideoBlackLevel", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            CurrentBlueVideoBlackLevel = (System.UInt16) args[1].DataValue;
            return;
        }
        public int GetBlueVideoBlackLevel(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentBlueVideoBlackLevel", "");
           return(_S.InvokeAsync("GetBlueVideoBlackLevel", args, new UPnPService.UPnPServiceInvokeHandler(Sink_GetBlueVideoBlackLevel), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetBlueVideoBlackLevel)));
        }
        private void Sink_GetBlueVideoBlackLevel(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_GetBlueVideoBlackLevel != null)
            {
               OnResult_GetBlueVideoBlackLevel((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetBlueVideoBlackLevel.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetBlueVideoBlackLevel)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_GetBlueVideoBlackLevel.Remove(wr);
                }
            }
        }
        private void Error_Sink_GetBlueVideoBlackLevel(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_GetBlueVideoBlackLevel != null)
            {
                 OnResult_GetBlueVideoBlackLevel((System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetBlueVideoBlackLevel.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetBlueVideoBlackLevel)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, Handle);
                }
                else
                {
                    WeakList_GetBlueVideoBlackLevel.Remove(wr);
                }
            }
        }
        public void Sync_GetBlueVideoGain(System.UInt32 InstanceID, out System.UInt16 CurrentBlueVideoGain)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentBlueVideoGain", "");
            _S.InvokeSync("GetBlueVideoGain", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            CurrentBlueVideoGain = (System.UInt16) args[1].DataValue;
            return;
        }
        public int GetBlueVideoGain(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentBlueVideoGain", "");
           return(_S.InvokeAsync("GetBlueVideoGain", args, new UPnPService.UPnPServiceInvokeHandler(Sink_GetBlueVideoGain), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetBlueVideoGain)));
        }
        private void Sink_GetBlueVideoGain(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_GetBlueVideoGain != null)
            {
               OnResult_GetBlueVideoGain((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetBlueVideoGain.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetBlueVideoGain)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_GetBlueVideoGain.Remove(wr);
                }
            }
        }
        private void Error_Sink_GetBlueVideoGain(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_GetBlueVideoGain != null)
            {
                 OnResult_GetBlueVideoGain((System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetBlueVideoGain.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetBlueVideoGain)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, Handle);
                }
                else
                {
                    WeakList_GetBlueVideoGain.Remove(wr);
                }
            }
        }
        public void Sync_SetBlueVideoBlackLevel(System.UInt32 InstanceID, System.UInt16 DesiredBlueVideoBlackLevel)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredBlueVideoBlackLevel", DesiredBlueVideoBlackLevel);
            _S.InvokeSync("SetBlueVideoBlackLevel", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            DesiredBlueVideoBlackLevel = (System.UInt16) args[1].DataValue;
            return;
        }
        public int SetBlueVideoBlackLevel(System.UInt32 InstanceID, System.UInt16 DesiredBlueVideoBlackLevel)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredBlueVideoBlackLevel", DesiredBlueVideoBlackLevel);
           return(_S.InvokeAsync("SetBlueVideoBlackLevel", args, new UPnPService.UPnPServiceInvokeHandler(Sink_SetBlueVideoBlackLevel), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetBlueVideoBlackLevel)));
        }
        private void Sink_SetBlueVideoBlackLevel(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_SetBlueVideoBlackLevel != null)
            {
               OnResult_SetBlueVideoBlackLevel((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetBlueVideoBlackLevel.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetBlueVideoBlackLevel)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_SetBlueVideoBlackLevel.Remove(wr);
                }
            }
        }
        private void Error_Sink_SetBlueVideoBlackLevel(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_SetBlueVideoBlackLevel != null)
            {
                 OnResult_SetBlueVideoBlackLevel((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetBlueVideoBlackLevel.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetBlueVideoBlackLevel)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, Handle);
                }
                else
                {
                    WeakList_SetBlueVideoBlackLevel.Remove(wr);
                }
            }
        }
        public void Sync_GetMute(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, out System.Boolean CurrentMute)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           switch(Channel)
           {
               case Enum_A_ARG_TYPE_Channel.MASTER:
                   args[1] = new UPnPArgument("Channel", "Master");
                   break;
               case Enum_A_ARG_TYPE_Channel.LF:
                   args[1] = new UPnPArgument("Channel", "LF");
                   break;
               case Enum_A_ARG_TYPE_Channel.RF:
                   args[1] = new UPnPArgument("Channel", "RF");
                   break;
               case Enum_A_ARG_TYPE_Channel.CF:
                   args[1] = new UPnPArgument("Channel", "CF");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFE:
                   args[1] = new UPnPArgument("Channel", "LFE");
                   break;
               case Enum_A_ARG_TYPE_Channel.LS:
                   args[1] = new UPnPArgument("Channel", "LS");
                   break;
               case Enum_A_ARG_TYPE_Channel.RS:
                   args[1] = new UPnPArgument("Channel", "RS");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFC:
                   args[1] = new UPnPArgument("Channel", "LFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.RFC:
                   args[1] = new UPnPArgument("Channel", "RFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.SD:
                   args[1] = new UPnPArgument("Channel", "SD");
                   break;
               case Enum_A_ARG_TYPE_Channel.SL:
                   args[1] = new UPnPArgument("Channel", "SL");
                   break;
               case Enum_A_ARG_TYPE_Channel.SR_:
                   args[1] = new UPnPArgument("Channel", "SR ");
                   break;
               case Enum_A_ARG_TYPE_Channel.T:
                   args[1] = new UPnPArgument("Channel", "T");
                   break;
               case Enum_A_ARG_TYPE_Channel.B:
                   args[1] = new UPnPArgument("Channel", "B");
                   break;
               case Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED:
                   args[1] = new UPnPArgument("Channel", "Vendor defined");
                   break;
               default:
                  args[1] = new UPnPArgument("Channel", GetUnspecifiedValue("Enum_A_ARG_TYPE_Channel"));
                  break;
           }
           args[2] = new UPnPArgument("CurrentMute", "");
            _S.InvokeSync("GetMute", args);
            for(int i=0;i<args.Length;++i)
            {
                switch(args[i].Name)
                {
                    case "Channel":
                        switch((string)args[i].DataValue)
                        {
                            case "Master":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.MASTER;
                                break;
                            case "LF":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LF;
                                break;
                            case "RF":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.RF;
                                break;
                            case "CF":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.CF;
                                break;
                            case "LFE":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFE;
                                break;
                            case "LS":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LS;
                                break;
                            case "RS":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.RS;
                                break;
                            case "LFC":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFC;
                                break;
                            case "RFC":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.RFC;
                                break;
                            case "SD":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SD;
                                break;
                            case "SL":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SL;
                                break;
                            case "SR ":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR_;
                                break;
                            case "T":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.T;
                                break;
                            case "B":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.B;
                                break;
                            case "Vendor defined":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_Channel", (string)args[i].DataValue);
                               args[i].DataValue = Enum_A_ARG_TYPE_Channel._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            InstanceID = (System.UInt32) args[0].DataValue;
            Channel = (Enum_A_ARG_TYPE_Channel) args[1].DataValue;
            CurrentMute = (System.Boolean) args[2].DataValue;
            return;
        }
        public int GetMute(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           switch(Channel)
           {
               case Enum_A_ARG_TYPE_Channel.MASTER:
                   args[1] = new UPnPArgument("Channel", "Master");
                   break;
               case Enum_A_ARG_TYPE_Channel.LF:
                   args[1] = new UPnPArgument("Channel", "LF");
                   break;
               case Enum_A_ARG_TYPE_Channel.RF:
                   args[1] = new UPnPArgument("Channel", "RF");
                   break;
               case Enum_A_ARG_TYPE_Channel.CF:
                   args[1] = new UPnPArgument("Channel", "CF");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFE:
                   args[1] = new UPnPArgument("Channel", "LFE");
                   break;
               case Enum_A_ARG_TYPE_Channel.LS:
                   args[1] = new UPnPArgument("Channel", "LS");
                   break;
               case Enum_A_ARG_TYPE_Channel.RS:
                   args[1] = new UPnPArgument("Channel", "RS");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFC:
                   args[1] = new UPnPArgument("Channel", "LFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.RFC:
                   args[1] = new UPnPArgument("Channel", "RFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.SD:
                   args[1] = new UPnPArgument("Channel", "SD");
                   break;
               case Enum_A_ARG_TYPE_Channel.SL:
                   args[1] = new UPnPArgument("Channel", "SL");
                   break;
               case Enum_A_ARG_TYPE_Channel.SR_:
                   args[1] = new UPnPArgument("Channel", "SR ");
                   break;
               case Enum_A_ARG_TYPE_Channel.T:
                   args[1] = new UPnPArgument("Channel", "T");
                   break;
               case Enum_A_ARG_TYPE_Channel.B:
                   args[1] = new UPnPArgument("Channel", "B");
                   break;
               case Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED:
                   args[1] = new UPnPArgument("Channel", "Vendor defined");
                   break;
               default:
                  args[1] = new UPnPArgument("Channel", GetUnspecifiedValue("Enum_A_ARG_TYPE_Channel"));
                  break;
           }
           args[2] = new UPnPArgument("CurrentMute", "");
           return(_S.InvokeAsync("GetMute", args, new UPnPService.UPnPServiceInvokeHandler(Sink_GetMute), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetMute)));
        }
        private void Sink_GetMute(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "Channel":
                        switch((string)Args[i].DataValue)
                        {
                            case "Master":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.MASTER;
                                break;
                            case "LF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LF;
                                break;
                            case "RF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RF;
                                break;
                            case "CF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.CF;
                                break;
                            case "LFE":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFE;
                                break;
                            case "LS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LS;
                                break;
                            case "RS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RS;
                                break;
                            case "LFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFC;
                                break;
                            case "RFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RFC;
                                break;
                            case "SD":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SD;
                                break;
                            case "SL":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SL;
                                break;
                            case "SR ":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR_;
                                break;
                            case "T":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.T;
                                break;
                            case "B":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.B;
                                break;
                            case "Vendor defined":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_Channel", (string)Args[i].DataValue);
                               Args[i].DataValue = Enum_A_ARG_TYPE_Channel._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            if(OnResult_GetMute != null)
            {
               OnResult_GetMute((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean )Args[2].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetMute.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetMute)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean )Args[2].DataValue, null, Handle);
                }
                else
                {
                    WeakList_GetMute.Remove(wr);
                }
            }
        }
        private void Error_Sink_GetMute(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "Channel":
                        switch((string)Args[i].DataValue)
                        {
                            case "Master":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.MASTER;
                                break;
                            case "LF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LF;
                                break;
                            case "RF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RF;
                                break;
                            case "CF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.CF;
                                break;
                            case "LFE":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFE;
                                break;
                            case "LS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LS;
                                break;
                            case "RS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RS;
                                break;
                            case "LFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFC;
                                break;
                            case "RFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RFC;
                                break;
                            case "SD":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SD;
                                break;
                            case "SL":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SL;
                                break;
                            case "SR ":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR_;
                                break;
                            case "T":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.T;
                                break;
                            case "B":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.B;
                                break;
                            case "Vendor defined":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED;
                                break;
                        }
                        break;
                }
            }
            if(OnResult_GetMute != null)
            {
                 OnResult_GetMute((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean)UPnPService.CreateObjectInstance(typeof(System.Boolean),null), e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetMute.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetMute)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean)UPnPService.CreateObjectInstance(typeof(System.Boolean),null), e, Handle);
                }
                else
                {
                    WeakList_GetMute.Remove(wr);
                }
            }
        }
        public void Sync_SetBlueVideoGain(System.UInt32 InstanceID, System.UInt16 DesiredBlueVideoGain)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredBlueVideoGain", DesiredBlueVideoGain);
            _S.InvokeSync("SetBlueVideoGain", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            DesiredBlueVideoGain = (System.UInt16) args[1].DataValue;
            return;
        }
        public int SetBlueVideoGain(System.UInt32 InstanceID, System.UInt16 DesiredBlueVideoGain)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredBlueVideoGain", DesiredBlueVideoGain);
           return(_S.InvokeAsync("SetBlueVideoGain", args, new UPnPService.UPnPServiceInvokeHandler(Sink_SetBlueVideoGain), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetBlueVideoGain)));
        }
        private void Sink_SetBlueVideoGain(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_SetBlueVideoGain != null)
            {
               OnResult_SetBlueVideoGain((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetBlueVideoGain.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetBlueVideoGain)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_SetBlueVideoGain.Remove(wr);
                }
            }
        }
        private void Error_Sink_SetBlueVideoGain(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_SetBlueVideoGain != null)
            {
                 OnResult_SetBlueVideoGain((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetBlueVideoGain.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetBlueVideoGain)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, Handle);
                }
                else
                {
                    WeakList_SetBlueVideoGain.Remove(wr);
                }
            }
        }
        public void Sync_GetVerticalKeystone(System.UInt32 InstanceID, out System.Int16 CurrentVerticalKeystone)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentVerticalKeystone", "");
            _S.InvokeSync("GetVerticalKeystone", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            CurrentVerticalKeystone = (System.Int16) args[1].DataValue;
            return;
        }
        public int GetVerticalKeystone(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentVerticalKeystone", "");
           return(_S.InvokeAsync("GetVerticalKeystone", args, new UPnPService.UPnPServiceInvokeHandler(Sink_GetVerticalKeystone), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetVerticalKeystone)));
        }
        private void Sink_GetVerticalKeystone(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_GetVerticalKeystone != null)
            {
               OnResult_GetVerticalKeystone((System.UInt32 )Args[0].DataValue, (System.Int16 )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetVerticalKeystone.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetVerticalKeystone)wr.Target)((System.UInt32 )Args[0].DataValue, (System.Int16 )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_GetVerticalKeystone.Remove(wr);
                }
            }
        }
        private void Error_Sink_GetVerticalKeystone(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_GetVerticalKeystone != null)
            {
                 OnResult_GetVerticalKeystone((System.UInt32 )Args[0].DataValue, (System.Int16)UPnPService.CreateObjectInstance(typeof(System.Int16),null), e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetVerticalKeystone.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetVerticalKeystone)wr.Target)((System.UInt32 )Args[0].DataValue, (System.Int16)UPnPService.CreateObjectInstance(typeof(System.Int16),null), e, Handle);
                }
                else
                {
                    WeakList_GetVerticalKeystone.Remove(wr);
                }
            }
        }
        public void Sync_SetVerticalKeystone(System.UInt32 InstanceID, System.Int16 DesiredVerticalKeystone)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredVerticalKeystone", DesiredVerticalKeystone);
            _S.InvokeSync("SetVerticalKeystone", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            DesiredVerticalKeystone = (System.Int16) args[1].DataValue;
            return;
        }
        public int SetVerticalKeystone(System.UInt32 InstanceID, System.Int16 DesiredVerticalKeystone)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredVerticalKeystone", DesiredVerticalKeystone);
           return(_S.InvokeAsync("SetVerticalKeystone", args, new UPnPService.UPnPServiceInvokeHandler(Sink_SetVerticalKeystone), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetVerticalKeystone)));
        }
        private void Sink_SetVerticalKeystone(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_SetVerticalKeystone != null)
            {
               OnResult_SetVerticalKeystone((System.UInt32 )Args[0].DataValue, (System.Int16 )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetVerticalKeystone.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetVerticalKeystone)wr.Target)((System.UInt32 )Args[0].DataValue, (System.Int16 )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_SetVerticalKeystone.Remove(wr);
                }
            }
        }
        private void Error_Sink_SetVerticalKeystone(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_SetVerticalKeystone != null)
            {
                 OnResult_SetVerticalKeystone((System.UInt32 )Args[0].DataValue, (System.Int16 )Args[1].DataValue, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetVerticalKeystone.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetVerticalKeystone)wr.Target)((System.UInt32 )Args[0].DataValue, (System.Int16 )Args[1].DataValue, e, Handle);
                }
                else
                {
                    WeakList_SetVerticalKeystone.Remove(wr);
                }
            }
        }
        public void Sync_GetBrightness(System.UInt32 InstanceID, out System.UInt16 CurrentBrightness)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentBrightness", "");
            _S.InvokeSync("GetBrightness", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            CurrentBrightness = (System.UInt16) args[1].DataValue;
            return;
        }
        public int GetBrightness(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentBrightness", "");
           return(_S.InvokeAsync("GetBrightness", args, new UPnPService.UPnPServiceInvokeHandler(Sink_GetBrightness), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetBrightness)));
        }
        private void Sink_GetBrightness(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_GetBrightness != null)
            {
               OnResult_GetBrightness((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetBrightness.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetBrightness)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_GetBrightness.Remove(wr);
                }
            }
        }
        private void Error_Sink_GetBrightness(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_GetBrightness != null)
            {
                 OnResult_GetBrightness((System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetBrightness.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetBrightness)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, Handle);
                }
                else
                {
                    WeakList_GetBrightness.Remove(wr);
                }
            }
        }
        public void Sync_GetVolumeDB(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, out System.Int16 CurrentVolume)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           switch(Channel)
           {
               case Enum_A_ARG_TYPE_Channel.MASTER:
                   args[1] = new UPnPArgument("Channel", "Master");
                   break;
               case Enum_A_ARG_TYPE_Channel.LF:
                   args[1] = new UPnPArgument("Channel", "LF");
                   break;
               case Enum_A_ARG_TYPE_Channel.RF:
                   args[1] = new UPnPArgument("Channel", "RF");
                   break;
               case Enum_A_ARG_TYPE_Channel.CF:
                   args[1] = new UPnPArgument("Channel", "CF");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFE:
                   args[1] = new UPnPArgument("Channel", "LFE");
                   break;
               case Enum_A_ARG_TYPE_Channel.LS:
                   args[1] = new UPnPArgument("Channel", "LS");
                   break;
               case Enum_A_ARG_TYPE_Channel.RS:
                   args[1] = new UPnPArgument("Channel", "RS");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFC:
                   args[1] = new UPnPArgument("Channel", "LFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.RFC:
                   args[1] = new UPnPArgument("Channel", "RFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.SD:
                   args[1] = new UPnPArgument("Channel", "SD");
                   break;
               case Enum_A_ARG_TYPE_Channel.SL:
                   args[1] = new UPnPArgument("Channel", "SL");
                   break;
               case Enum_A_ARG_TYPE_Channel.SR_:
                   args[1] = new UPnPArgument("Channel", "SR ");
                   break;
               case Enum_A_ARG_TYPE_Channel.T:
                   args[1] = new UPnPArgument("Channel", "T");
                   break;
               case Enum_A_ARG_TYPE_Channel.B:
                   args[1] = new UPnPArgument("Channel", "B");
                   break;
               case Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED:
                   args[1] = new UPnPArgument("Channel", "Vendor defined");
                   break;
               default:
                  args[1] = new UPnPArgument("Channel", GetUnspecifiedValue("Enum_A_ARG_TYPE_Channel"));
                  break;
           }
           args[2] = new UPnPArgument("CurrentVolume", "");
            _S.InvokeSync("GetVolumeDB", args);
            for(int i=0;i<args.Length;++i)
            {
                switch(args[i].Name)
                {
                    case "Channel":
                        switch((string)args[i].DataValue)
                        {
                            case "Master":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.MASTER;
                                break;
                            case "LF":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LF;
                                break;
                            case "RF":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.RF;
                                break;
                            case "CF":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.CF;
                                break;
                            case "LFE":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFE;
                                break;
                            case "LS":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LS;
                                break;
                            case "RS":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.RS;
                                break;
                            case "LFC":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFC;
                                break;
                            case "RFC":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.RFC;
                                break;
                            case "SD":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SD;
                                break;
                            case "SL":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SL;
                                break;
                            case "SR ":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR_;
                                break;
                            case "T":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.T;
                                break;
                            case "B":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.B;
                                break;
                            case "Vendor defined":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_Channel", (string)args[i].DataValue);
                               args[i].DataValue = Enum_A_ARG_TYPE_Channel._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            InstanceID = (System.UInt32) args[0].DataValue;
            Channel = (Enum_A_ARG_TYPE_Channel) args[1].DataValue;
            CurrentVolume = (System.Int16) args[2].DataValue;
            return;
        }
        public int GetVolumeDB(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           switch(Channel)
           {
               case Enum_A_ARG_TYPE_Channel.MASTER:
                   args[1] = new UPnPArgument("Channel", "Master");
                   break;
               case Enum_A_ARG_TYPE_Channel.LF:
                   args[1] = new UPnPArgument("Channel", "LF");
                   break;
               case Enum_A_ARG_TYPE_Channel.RF:
                   args[1] = new UPnPArgument("Channel", "RF");
                   break;
               case Enum_A_ARG_TYPE_Channel.CF:
                   args[1] = new UPnPArgument("Channel", "CF");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFE:
                   args[1] = new UPnPArgument("Channel", "LFE");
                   break;
               case Enum_A_ARG_TYPE_Channel.LS:
                   args[1] = new UPnPArgument("Channel", "LS");
                   break;
               case Enum_A_ARG_TYPE_Channel.RS:
                   args[1] = new UPnPArgument("Channel", "RS");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFC:
                   args[1] = new UPnPArgument("Channel", "LFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.RFC:
                   args[1] = new UPnPArgument("Channel", "RFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.SD:
                   args[1] = new UPnPArgument("Channel", "SD");
                   break;
               case Enum_A_ARG_TYPE_Channel.SL:
                   args[1] = new UPnPArgument("Channel", "SL");
                   break;
               case Enum_A_ARG_TYPE_Channel.SR_:
                   args[1] = new UPnPArgument("Channel", "SR ");
                   break;
               case Enum_A_ARG_TYPE_Channel.T:
                   args[1] = new UPnPArgument("Channel", "T");
                   break;
               case Enum_A_ARG_TYPE_Channel.B:
                   args[1] = new UPnPArgument("Channel", "B");
                   break;
               case Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED:
                   args[1] = new UPnPArgument("Channel", "Vendor defined");
                   break;
               default:
                  args[1] = new UPnPArgument("Channel", GetUnspecifiedValue("Enum_A_ARG_TYPE_Channel"));
                  break;
           }
           args[2] = new UPnPArgument("CurrentVolume", "");
           return(_S.InvokeAsync("GetVolumeDB", args, new UPnPService.UPnPServiceInvokeHandler(Sink_GetVolumeDB), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetVolumeDB)));
        }
        private void Sink_GetVolumeDB(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "Channel":
                        switch((string)Args[i].DataValue)
                        {
                            case "Master":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.MASTER;
                                break;
                            case "LF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LF;
                                break;
                            case "RF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RF;
                                break;
                            case "CF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.CF;
                                break;
                            case "LFE":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFE;
                                break;
                            case "LS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LS;
                                break;
                            case "RS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RS;
                                break;
                            case "LFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFC;
                                break;
                            case "RFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RFC;
                                break;
                            case "SD":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SD;
                                break;
                            case "SL":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SL;
                                break;
                            case "SR ":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR_;
                                break;
                            case "T":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.T;
                                break;
                            case "B":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.B;
                                break;
                            case "Vendor defined":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_Channel", (string)Args[i].DataValue);
                               Args[i].DataValue = Enum_A_ARG_TYPE_Channel._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            if(OnResult_GetVolumeDB != null)
            {
               OnResult_GetVolumeDB((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Int16 )Args[2].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetVolumeDB.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetVolumeDB)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Int16 )Args[2].DataValue, null, Handle);
                }
                else
                {
                    WeakList_GetVolumeDB.Remove(wr);
                }
            }
        }
        private void Error_Sink_GetVolumeDB(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "Channel":
                        switch((string)Args[i].DataValue)
                        {
                            case "Master":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.MASTER;
                                break;
                            case "LF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LF;
                                break;
                            case "RF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RF;
                                break;
                            case "CF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.CF;
                                break;
                            case "LFE":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFE;
                                break;
                            case "LS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LS;
                                break;
                            case "RS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RS;
                                break;
                            case "LFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFC;
                                break;
                            case "RFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RFC;
                                break;
                            case "SD":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SD;
                                break;
                            case "SL":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SL;
                                break;
                            case "SR ":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR_;
                                break;
                            case "T":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.T;
                                break;
                            case "B":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.B;
                                break;
                            case "Vendor defined":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED;
                                break;
                        }
                        break;
                }
            }
            if(OnResult_GetVolumeDB != null)
            {
                 OnResult_GetVolumeDB((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Int16)UPnPService.CreateObjectInstance(typeof(System.Int16),null), e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetVolumeDB.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetVolumeDB)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Int16)UPnPService.CreateObjectInstance(typeof(System.Int16),null), e, Handle);
                }
                else
                {
                    WeakList_GetVolumeDB.Remove(wr);
                }
            }
        }
        public void Sync_GetGreenVideoBlackLevel(System.UInt32 InstanceID, out System.UInt16 CurrentGreenVideoBlackLevel)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentGreenVideoBlackLevel", "");
            _S.InvokeSync("GetGreenVideoBlackLevel", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            CurrentGreenVideoBlackLevel = (System.UInt16) args[1].DataValue;
            return;
        }
        public int GetGreenVideoBlackLevel(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentGreenVideoBlackLevel", "");
           return(_S.InvokeAsync("GetGreenVideoBlackLevel", args, new UPnPService.UPnPServiceInvokeHandler(Sink_GetGreenVideoBlackLevel), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetGreenVideoBlackLevel)));
        }
        private void Sink_GetGreenVideoBlackLevel(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_GetGreenVideoBlackLevel != null)
            {
               OnResult_GetGreenVideoBlackLevel((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetGreenVideoBlackLevel.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetGreenVideoBlackLevel)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_GetGreenVideoBlackLevel.Remove(wr);
                }
            }
        }
        private void Error_Sink_GetGreenVideoBlackLevel(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_GetGreenVideoBlackLevel != null)
            {
                 OnResult_GetGreenVideoBlackLevel((System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetGreenVideoBlackLevel.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetGreenVideoBlackLevel)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, Handle);
                }
                else
                {
                    WeakList_GetGreenVideoBlackLevel.Remove(wr);
                }
            }
        }
        public void Sync_GetRedVideoGain(System.UInt32 InstanceID, out System.UInt16 CurrentRedVideoGain)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentRedVideoGain", "");
            _S.InvokeSync("GetRedVideoGain", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            CurrentRedVideoGain = (System.UInt16) args[1].DataValue;
            return;
        }
        public int GetRedVideoGain(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentRedVideoGain", "");
           return(_S.InvokeAsync("GetRedVideoGain", args, new UPnPService.UPnPServiceInvokeHandler(Sink_GetRedVideoGain), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetRedVideoGain)));
        }
        private void Sink_GetRedVideoGain(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_GetRedVideoGain != null)
            {
               OnResult_GetRedVideoGain((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetRedVideoGain.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetRedVideoGain)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_GetRedVideoGain.Remove(wr);
                }
            }
        }
        private void Error_Sink_GetRedVideoGain(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_GetRedVideoGain != null)
            {
                 OnResult_GetRedVideoGain((System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_GetRedVideoGain.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_GetRedVideoGain)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, Handle);
                }
                else
                {
                    WeakList_GetRedVideoGain.Remove(wr);
                }
            }
        }
        public void Sync_SetMute(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.Boolean DesiredMute)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           switch(Channel)
           {
               case Enum_A_ARG_TYPE_Channel.MASTER:
                   args[1] = new UPnPArgument("Channel", "Master");
                   break;
               case Enum_A_ARG_TYPE_Channel.LF:
                   args[1] = new UPnPArgument("Channel", "LF");
                   break;
               case Enum_A_ARG_TYPE_Channel.RF:
                   args[1] = new UPnPArgument("Channel", "RF");
                   break;
               case Enum_A_ARG_TYPE_Channel.CF:
                   args[1] = new UPnPArgument("Channel", "CF");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFE:
                   args[1] = new UPnPArgument("Channel", "LFE");
                   break;
               case Enum_A_ARG_TYPE_Channel.LS:
                   args[1] = new UPnPArgument("Channel", "LS");
                   break;
               case Enum_A_ARG_TYPE_Channel.RS:
                   args[1] = new UPnPArgument("Channel", "RS");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFC:
                   args[1] = new UPnPArgument("Channel", "LFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.RFC:
                   args[1] = new UPnPArgument("Channel", "RFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.SD:
                   args[1] = new UPnPArgument("Channel", "SD");
                   break;
               case Enum_A_ARG_TYPE_Channel.SL:
                   args[1] = new UPnPArgument("Channel", "SL");
                   break;
               case Enum_A_ARG_TYPE_Channel.SR_:
                   args[1] = new UPnPArgument("Channel", "SR ");
                   break;
               case Enum_A_ARG_TYPE_Channel.T:
                   args[1] = new UPnPArgument("Channel", "T");
                   break;
               case Enum_A_ARG_TYPE_Channel.B:
                   args[1] = new UPnPArgument("Channel", "B");
                   break;
               case Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED:
                   args[1] = new UPnPArgument("Channel", "Vendor defined");
                   break;
               default:
                  args[1] = new UPnPArgument("Channel", GetUnspecifiedValue("Enum_A_ARG_TYPE_Channel"));
                  break;
           }
           args[2] = new UPnPArgument("DesiredMute", DesiredMute);
            _S.InvokeSync("SetMute", args);
            for(int i=0;i<args.Length;++i)
            {
                switch(args[i].Name)
                {
                    case "Channel":
                        switch((string)args[i].DataValue)
                        {
                            case "Master":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.MASTER;
                                break;
                            case "LF":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LF;
                                break;
                            case "RF":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.RF;
                                break;
                            case "CF":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.CF;
                                break;
                            case "LFE":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFE;
                                break;
                            case "LS":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LS;
                                break;
                            case "RS":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.RS;
                                break;
                            case "LFC":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFC;
                                break;
                            case "RFC":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.RFC;
                                break;
                            case "SD":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SD;
                                break;
                            case "SL":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SL;
                                break;
                            case "SR ":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR_;
                                break;
                            case "T":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.T;
                                break;
                            case "B":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.B;
                                break;
                            case "Vendor defined":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_Channel", (string)args[i].DataValue);
                               args[i].DataValue = Enum_A_ARG_TYPE_Channel._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            InstanceID = (System.UInt32) args[0].DataValue;
            Channel = (Enum_A_ARG_TYPE_Channel) args[1].DataValue;
            DesiredMute = (System.Boolean) args[2].DataValue;
            return;
        }
        public int SetMute(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.Boolean DesiredMute)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           switch(Channel)
           {
               case Enum_A_ARG_TYPE_Channel.MASTER:
                   args[1] = new UPnPArgument("Channel", "Master");
                   break;
               case Enum_A_ARG_TYPE_Channel.LF:
                   args[1] = new UPnPArgument("Channel", "LF");
                   break;
               case Enum_A_ARG_TYPE_Channel.RF:
                   args[1] = new UPnPArgument("Channel", "RF");
                   break;
               case Enum_A_ARG_TYPE_Channel.CF:
                   args[1] = new UPnPArgument("Channel", "CF");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFE:
                   args[1] = new UPnPArgument("Channel", "LFE");
                   break;
               case Enum_A_ARG_TYPE_Channel.LS:
                   args[1] = new UPnPArgument("Channel", "LS");
                   break;
               case Enum_A_ARG_TYPE_Channel.RS:
                   args[1] = new UPnPArgument("Channel", "RS");
                   break;
               case Enum_A_ARG_TYPE_Channel.LFC:
                   args[1] = new UPnPArgument("Channel", "LFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.RFC:
                   args[1] = new UPnPArgument("Channel", "RFC");
                   break;
               case Enum_A_ARG_TYPE_Channel.SD:
                   args[1] = new UPnPArgument("Channel", "SD");
                   break;
               case Enum_A_ARG_TYPE_Channel.SL:
                   args[1] = new UPnPArgument("Channel", "SL");
                   break;
               case Enum_A_ARG_TYPE_Channel.SR_:
                   args[1] = new UPnPArgument("Channel", "SR ");
                   break;
               case Enum_A_ARG_TYPE_Channel.T:
                   args[1] = new UPnPArgument("Channel", "T");
                   break;
               case Enum_A_ARG_TYPE_Channel.B:
                   args[1] = new UPnPArgument("Channel", "B");
                   break;
               case Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED:
                   args[1] = new UPnPArgument("Channel", "Vendor defined");
                   break;
               default:
                  args[1] = new UPnPArgument("Channel", GetUnspecifiedValue("Enum_A_ARG_TYPE_Channel"));
                  break;
           }
           args[2] = new UPnPArgument("DesiredMute", DesiredMute);
           return(_S.InvokeAsync("SetMute", args, new UPnPService.UPnPServiceInvokeHandler(Sink_SetMute), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetMute)));
        }
        private void Sink_SetMute(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "Channel":
                        switch((string)Args[i].DataValue)
                        {
                            case "Master":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.MASTER;
                                break;
                            case "LF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LF;
                                break;
                            case "RF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RF;
                                break;
                            case "CF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.CF;
                                break;
                            case "LFE":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFE;
                                break;
                            case "LS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LS;
                                break;
                            case "RS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RS;
                                break;
                            case "LFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFC;
                                break;
                            case "RFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RFC;
                                break;
                            case "SD":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SD;
                                break;
                            case "SL":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SL;
                                break;
                            case "SR ":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR_;
                                break;
                            case "T":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.T;
                                break;
                            case "B":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.B;
                                break;
                            case "Vendor defined":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_Channel", (string)Args[i].DataValue);
                               Args[i].DataValue = Enum_A_ARG_TYPE_Channel._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            if(OnResult_SetMute != null)
            {
               OnResult_SetMute((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean )Args[2].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetMute.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetMute)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean )Args[2].DataValue, null, Handle);
                }
                else
                {
                    WeakList_SetMute.Remove(wr);
                }
            }
        }
        private void Error_Sink_SetMute(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "Channel":
                        switch((string)Args[i].DataValue)
                        {
                            case "Master":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.MASTER;
                                break;
                            case "LF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LF;
                                break;
                            case "RF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RF;
                                break;
                            case "CF":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.CF;
                                break;
                            case "LFE":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFE;
                                break;
                            case "LS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LS;
                                break;
                            case "RS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RS;
                                break;
                            case "LFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.LFC;
                                break;
                            case "RFC":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.RFC;
                                break;
                            case "SD":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SD;
                                break;
                            case "SL":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SL;
                                break;
                            case "SR ":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR_;
                                break;
                            case "T":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.T;
                                break;
                            case "B":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.B;
                                break;
                            case "Vendor defined":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.VENDOR_DEFINED;
                                break;
                        }
                        break;
                }
            }
            if(OnResult_SetMute != null)
            {
                 OnResult_SetMute((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean )Args[2].DataValue, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetMute.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetMute)wr.Target)((System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean )Args[2].DataValue, e, Handle);
                }
                else
                {
                    WeakList_SetMute.Remove(wr);
                }
            }
        }
        public void Sync_SetGreenVideoGain(System.UInt32 InstanceID, System.UInt16 DesiredGreenVideoGain)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredGreenVideoGain", DesiredGreenVideoGain);
            _S.InvokeSync("SetGreenVideoGain", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            DesiredGreenVideoGain = (System.UInt16) args[1].DataValue;
            return;
        }
        public int SetGreenVideoGain(System.UInt32 InstanceID, System.UInt16 DesiredGreenVideoGain)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredGreenVideoGain", DesiredGreenVideoGain);
           return(_S.InvokeAsync("SetGreenVideoGain", args, new UPnPService.UPnPServiceInvokeHandler(Sink_SetGreenVideoGain), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetGreenVideoGain)));
        }
        private void Sink_SetGreenVideoGain(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_SetGreenVideoGain != null)
            {
               OnResult_SetGreenVideoGain((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetGreenVideoGain.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetGreenVideoGain)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_SetGreenVideoGain.Remove(wr);
                }
            }
        }
        private void Error_Sink_SetGreenVideoGain(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_SetGreenVideoGain != null)
            {
                 OnResult_SetGreenVideoGain((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetGreenVideoGain.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetGreenVideoGain)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, Handle);
                }
                else
                {
                    WeakList_SetGreenVideoGain.Remove(wr);
                }
            }
        }
        public void Sync_SetSharpness(System.UInt32 InstanceID, System.UInt16 DesiredSharpness)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredSharpness", DesiredSharpness);
            _S.InvokeSync("SetSharpness", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            DesiredSharpness = (System.UInt16) args[1].DataValue;
            return;
        }
        public int SetSharpness(System.UInt32 InstanceID, System.UInt16 DesiredSharpness)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredSharpness", DesiredSharpness);
           return(_S.InvokeAsync("SetSharpness", args, new UPnPService.UPnPServiceInvokeHandler(Sink_SetSharpness), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetSharpness)));
        }
        private void Sink_SetSharpness(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_SetSharpness != null)
            {
               OnResult_SetSharpness((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetSharpness.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetSharpness)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_SetSharpness.Remove(wr);
                }
            }
        }
        private void Error_Sink_SetSharpness(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_SetSharpness != null)
            {
                 OnResult_SetSharpness((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetSharpness.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetSharpness)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, Handle);
                }
                else
                {
                    WeakList_SetSharpness.Remove(wr);
                }
            }
        }
        public void Sync_SetHorizontalKeystone(System.UInt32 InstanceID, System.Int16 DesiredHorizontalKeystone)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredHorizontalKeystone", DesiredHorizontalKeystone);
            _S.InvokeSync("SetHorizontalKeystone", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            DesiredHorizontalKeystone = (System.Int16) args[1].DataValue;
            return;
        }
        public int SetHorizontalKeystone(System.UInt32 InstanceID, System.Int16 DesiredHorizontalKeystone)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredHorizontalKeystone", DesiredHorizontalKeystone);
           return(_S.InvokeAsync("SetHorizontalKeystone", args, new UPnPService.UPnPServiceInvokeHandler(Sink_SetHorizontalKeystone), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetHorizontalKeystone)));
        }
        private void Sink_SetHorizontalKeystone(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_SetHorizontalKeystone != null)
            {
               OnResult_SetHorizontalKeystone((System.UInt32 )Args[0].DataValue, (System.Int16 )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetHorizontalKeystone.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetHorizontalKeystone)wr.Target)((System.UInt32 )Args[0].DataValue, (System.Int16 )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_SetHorizontalKeystone.Remove(wr);
                }
            }
        }
        private void Error_Sink_SetHorizontalKeystone(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_SetHorizontalKeystone != null)
            {
                 OnResult_SetHorizontalKeystone((System.UInt32 )Args[0].DataValue, (System.Int16 )Args[1].DataValue, e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetHorizontalKeystone.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetHorizontalKeystone)wr.Target)((System.UInt32 )Args[0].DataValue, (System.Int16 )Args[1].DataValue, e, Handle);
                }
                else
                {
                    WeakList_SetHorizontalKeystone.Remove(wr);
                }
            }
        }
        public void Sync_SetColorTemperature(System.UInt32 InstanceID, out System.UInt16 DesiredColorTemperature)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredColorTemperature", "");
            _S.InvokeSync("SetColorTemperature", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            DesiredColorTemperature = (System.UInt16) args[1].DataValue;
            return;
        }
        public int SetColorTemperature(System.UInt32 InstanceID)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredColorTemperature", "");
           return(_S.InvokeAsync("SetColorTemperature", args, new UPnPService.UPnPServiceInvokeHandler(Sink_SetColorTemperature), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetColorTemperature)));
        }
        private void Sink_SetColorTemperature(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, int Handle)
        {
            if(OnResult_SetColorTemperature != null)
            {
               OnResult_SetColorTemperature((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetColorTemperature.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetColorTemperature)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, Handle);
                }
                else
                {
                    WeakList_SetColorTemperature.Remove(wr);
                }
            }
        }
        private void Error_Sink_SetColorTemperature(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, int Handle)
        {
            if(OnResult_SetColorTemperature != null)
            {
                 OnResult_SetColorTemperature((System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, Handle);
            }
            WeakReference[] w = (WeakReference[])WeakList_SetColorTemperature.ToArray(typeof(WeakReference));
            foreach(WeakReference wr in w)
            {
                if(wr.IsAlive==true)
                {
                   ((Delegate_OnResult_SetColorTemperature)wr.Target)((System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, Handle);
                }
                else
                {
                    WeakList_SetColorTemperature.Remove(wr);
                }
            }
        }
    }
}