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
using System.Threading;
using OpenSource.Utilities;
using OpenSource.UPnP;

namespace OpenSource.UPnP.AV
{
    /// <summary>
    /// Transparent ClientSide UPnP Service
    /// </summary>
    public class CpRenderingControl
    {
       private Hashtable UnspecifiedTable = Hashtable.Synchronized(new Hashtable());
       internal UPnPService _S;

       public UPnPService GetUPnPService()
       {
            return(_S);
       }
       public static string SERVICE_NAME = "urn:schemas-upnp-org:service:RenderingControl:";
       public double VERSION
       {
           get
           {
               return(double.Parse(_S.Version));
           }
       }

       public delegate void StateVariableModifiedHandler_LastChange(CpRenderingControl sender, System.String NewValue);
       private WeakEvent OnStateVariable_LastChange_Event = new WeakEvent();
       public event StateVariableModifiedHandler_LastChange OnStateVariable_LastChange
       {
			add{OnStateVariable_LastChange_Event.Register(value);}
			remove{OnStateVariable_LastChange_Event.UnRegister(value);}
       }
       protected void LastChange_ModifiedSink(UPnPStateVariable Var, object NewValue)
       {
            OnStateVariable_LastChange_Event.Fire(this, (System.String)NewValue);
       }
       public delegate void SubscribeHandler(CpRenderingControl sender, bool Success);
       public event SubscribeHandler OnSubscribe;
       public delegate void Delegate_OnResult_GetHorizontalKeystone(CpRenderingControl sender, System.UInt32 InstanceID, System.Int16 CurrentHorizontalKeystone, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_GetHorizontalKeystone_Event = new WeakEvent();
       public event Delegate_OnResult_GetHorizontalKeystone OnResult_GetHorizontalKeystone
       {
			add{OnResult_GetHorizontalKeystone_Event.Register(value);}
			remove{OnResult_GetHorizontalKeystone_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_GetVolume(CpRenderingControl sender, System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.UInt16 CurrentVolume, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_GetVolume_Event = new WeakEvent();
       public event Delegate_OnResult_GetVolume OnResult_GetVolume
       {
			add{OnResult_GetVolume_Event.Register(value);}
			remove{OnResult_GetVolume_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_SelectPreset(CpRenderingControl sender, System.UInt32 InstanceID, Enum_A_ARG_TYPE_PresetName PresetName, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_SelectPreset_Event = new WeakEvent();
       public event Delegate_OnResult_SelectPreset OnResult_SelectPreset
       {
			add{OnResult_SelectPreset_Event.Register(value);}
			remove{OnResult_SelectPreset_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_SetVolume(CpRenderingControl sender, System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.UInt16 DesiredVolume, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_SetVolume_Event = new WeakEvent();
       public event Delegate_OnResult_SetVolume OnResult_SetVolume
       {
			add{OnResult_SetVolume_Event.Register(value);}
			remove{OnResult_SetVolume_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_ListPresets(CpRenderingControl sender, System.UInt32 InstanceID, System.String CurrentPresetNameList, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_ListPresets_Event = new WeakEvent();
       public event Delegate_OnResult_ListPresets OnResult_ListPresets
       {
			add{OnResult_ListPresets_Event.Register(value);}
			remove{OnResult_ListPresets_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_SetVolumeDB(CpRenderingControl sender, System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.Int16 DesiredVolume, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_SetVolumeDB_Event = new WeakEvent();
       public event Delegate_OnResult_SetVolumeDB OnResult_SetVolumeDB
       {
			add{OnResult_SetVolumeDB_Event.Register(value);}
			remove{OnResult_SetVolumeDB_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_SetRedVideoBlackLevel(CpRenderingControl sender, System.UInt32 InstanceID, System.UInt16 DesiredRedVideoBlackLevel, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_SetRedVideoBlackLevel_Event = new WeakEvent();
       public event Delegate_OnResult_SetRedVideoBlackLevel OnResult_SetRedVideoBlackLevel
       {
			add{OnResult_SetRedVideoBlackLevel_Event.Register(value);}
			remove{OnResult_SetRedVideoBlackLevel_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_SetContrast(CpRenderingControl sender, System.UInt32 InstanceID, System.UInt16 DesiredContrast, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_SetContrast_Event = new WeakEvent();
       public event Delegate_OnResult_SetContrast OnResult_SetContrast
       {
			add{OnResult_SetContrast_Event.Register(value);}
			remove{OnResult_SetContrast_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_SetLoudness(CpRenderingControl sender, System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.Boolean DesiredLoudness, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_SetLoudness_Event = new WeakEvent();
       public event Delegate_OnResult_SetLoudness OnResult_SetLoudness
       {
			add{OnResult_SetLoudness_Event.Register(value);}
			remove{OnResult_SetLoudness_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_SetBrightness(CpRenderingControl sender, System.UInt32 InstanceID, System.UInt16 DesiredBrightness, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_SetBrightness_Event = new WeakEvent();
       public event Delegate_OnResult_SetBrightness OnResult_SetBrightness
       {
			add{OnResult_SetBrightness_Event.Register(value);}
			remove{OnResult_SetBrightness_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_GetLoudness(CpRenderingControl sender, System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.Boolean CurrentLoudness, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_GetLoudness_Event = new WeakEvent();
       public event Delegate_OnResult_GetLoudness OnResult_GetLoudness
       {
			add{OnResult_GetLoudness_Event.Register(value);}
			remove{OnResult_GetLoudness_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_GetColorTemperature(CpRenderingControl sender, System.UInt32 InstanceID, System.UInt16 CurrentColorTemperature, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_GetColorTemperature_Event = new WeakEvent();
       public event Delegate_OnResult_GetColorTemperature OnResult_GetColorTemperature
       {
			add{OnResult_GetColorTemperature_Event.Register(value);}
			remove{OnResult_GetColorTemperature_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_GetSharpness(CpRenderingControl sender, System.UInt32 InstanceID, System.UInt16 CurrentSharpness, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_GetSharpness_Event = new WeakEvent();
       public event Delegate_OnResult_GetSharpness OnResult_GetSharpness
       {
			add{OnResult_GetSharpness_Event.Register(value);}
			remove{OnResult_GetSharpness_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_GetContrast(CpRenderingControl sender, System.UInt32 InstanceID, System.UInt16 CurrentContrast, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_GetContrast_Event = new WeakEvent();
       public event Delegate_OnResult_GetContrast OnResult_GetContrast
       {
			add{OnResult_GetContrast_Event.Register(value);}
			remove{OnResult_GetContrast_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_GetGreenVideoGain(CpRenderingControl sender, System.UInt32 InstanceID, System.UInt16 CurrentGreenVideoGain, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_GetGreenVideoGain_Event = new WeakEvent();
       public event Delegate_OnResult_GetGreenVideoGain OnResult_GetGreenVideoGain
       {
			add{OnResult_GetGreenVideoGain_Event.Register(value);}
			remove{OnResult_GetGreenVideoGain_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_SetRedVideoGain(CpRenderingControl sender, System.UInt32 InstanceID, System.UInt16 DesiredRedVideoGain, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_SetRedVideoGain_Event = new WeakEvent();
       public event Delegate_OnResult_SetRedVideoGain OnResult_SetRedVideoGain
       {
			add{OnResult_SetRedVideoGain_Event.Register(value);}
			remove{OnResult_SetRedVideoGain_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_SetGreenVideoBlackLevel(CpRenderingControl sender, System.UInt32 InstanceID, System.UInt16 DesiredGreenVideoBlackLevel, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_SetGreenVideoBlackLevel_Event = new WeakEvent();
       public event Delegate_OnResult_SetGreenVideoBlackLevel OnResult_SetGreenVideoBlackLevel
       {
			add{OnResult_SetGreenVideoBlackLevel_Event.Register(value);}
			remove{OnResult_SetGreenVideoBlackLevel_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_GetVolumeDBRange(CpRenderingControl sender, System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.Int16 MinValue, System.Int16 MaxValue, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_GetVolumeDBRange_Event = new WeakEvent();
       public event Delegate_OnResult_GetVolumeDBRange OnResult_GetVolumeDBRange
       {
			add{OnResult_GetVolumeDBRange_Event.Register(value);}
			remove{OnResult_GetVolumeDBRange_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_GetRedVideoBlackLevel(CpRenderingControl sender, System.UInt32 InstanceID, System.UInt16 CurrentRedVideoBlackLevel, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_GetRedVideoBlackLevel_Event = new WeakEvent();
       public event Delegate_OnResult_GetRedVideoBlackLevel OnResult_GetRedVideoBlackLevel
       {
			add{OnResult_GetRedVideoBlackLevel_Event.Register(value);}
			remove{OnResult_GetRedVideoBlackLevel_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_GetBlueVideoBlackLevel(CpRenderingControl sender, System.UInt32 InstanceID, System.UInt16 CurrentBlueVideoBlackLevel, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_GetBlueVideoBlackLevel_Event = new WeakEvent();
       public event Delegate_OnResult_GetBlueVideoBlackLevel OnResult_GetBlueVideoBlackLevel
       {
			add{OnResult_GetBlueVideoBlackLevel_Event.Register(value);}
			remove{OnResult_GetBlueVideoBlackLevel_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_GetBlueVideoGain(CpRenderingControl sender, System.UInt32 InstanceID, System.UInt16 CurrentBlueVideoGain, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_GetBlueVideoGain_Event = new WeakEvent();
       public event Delegate_OnResult_GetBlueVideoGain OnResult_GetBlueVideoGain
       {
			add{OnResult_GetBlueVideoGain_Event.Register(value);}
			remove{OnResult_GetBlueVideoGain_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_SetBlueVideoBlackLevel(CpRenderingControl sender, System.UInt32 InstanceID, System.UInt16 DesiredBlueVideoBlackLevel, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_SetBlueVideoBlackLevel_Event = new WeakEvent();
       public event Delegate_OnResult_SetBlueVideoBlackLevel OnResult_SetBlueVideoBlackLevel
       {
			add{OnResult_SetBlueVideoBlackLevel_Event.Register(value);}
			remove{OnResult_SetBlueVideoBlackLevel_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_GetMute(CpRenderingControl sender, System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.Boolean CurrentMute, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_GetMute_Event = new WeakEvent();
       public event Delegate_OnResult_GetMute OnResult_GetMute
       {
			add{OnResult_GetMute_Event.Register(value);}
			remove{OnResult_GetMute_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_SetBlueVideoGain(CpRenderingControl sender, System.UInt32 InstanceID, System.UInt16 DesiredBlueVideoGain, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_SetBlueVideoGain_Event = new WeakEvent();
       public event Delegate_OnResult_SetBlueVideoGain OnResult_SetBlueVideoGain
       {
			add{OnResult_SetBlueVideoGain_Event.Register(value);}
			remove{OnResult_SetBlueVideoGain_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_GetVerticalKeystone(CpRenderingControl sender, System.UInt32 InstanceID, System.Int16 CurrentVerticalKeystone, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_GetVerticalKeystone_Event = new WeakEvent();
       public event Delegate_OnResult_GetVerticalKeystone OnResult_GetVerticalKeystone
       {
			add{OnResult_GetVerticalKeystone_Event.Register(value);}
			remove{OnResult_GetVerticalKeystone_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_SetVerticalKeystone(CpRenderingControl sender, System.UInt32 InstanceID, System.Int16 DesiredVerticalKeystone, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_SetVerticalKeystone_Event = new WeakEvent();
       public event Delegate_OnResult_SetVerticalKeystone OnResult_SetVerticalKeystone
       {
			add{OnResult_SetVerticalKeystone_Event.Register(value);}
			remove{OnResult_SetVerticalKeystone_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_GetBrightness(CpRenderingControl sender, System.UInt32 InstanceID, System.UInt16 CurrentBrightness, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_GetBrightness_Event = new WeakEvent();
       public event Delegate_OnResult_GetBrightness OnResult_GetBrightness
       {
			add{OnResult_GetBrightness_Event.Register(value);}
			remove{OnResult_GetBrightness_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_GetVolumeDB(CpRenderingControl sender, System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.Int16 CurrentVolume, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_GetVolumeDB_Event = new WeakEvent();
       public event Delegate_OnResult_GetVolumeDB OnResult_GetVolumeDB
       {
			add{OnResult_GetVolumeDB_Event.Register(value);}
			remove{OnResult_GetVolumeDB_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_GetGreenVideoBlackLevel(CpRenderingControl sender, System.UInt32 InstanceID, System.UInt16 CurrentGreenVideoBlackLevel, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_GetGreenVideoBlackLevel_Event = new WeakEvent();
       public event Delegate_OnResult_GetGreenVideoBlackLevel OnResult_GetGreenVideoBlackLevel
       {
			add{OnResult_GetGreenVideoBlackLevel_Event.Register(value);}
			remove{OnResult_GetGreenVideoBlackLevel_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_GetRedVideoGain(CpRenderingControl sender, System.UInt32 InstanceID, System.UInt16 CurrentRedVideoGain, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_GetRedVideoGain_Event = new WeakEvent();
       public event Delegate_OnResult_GetRedVideoGain OnResult_GetRedVideoGain
       {
			add{OnResult_GetRedVideoGain_Event.Register(value);}
			remove{OnResult_GetRedVideoGain_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_SetMute(CpRenderingControl sender, System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.Boolean DesiredMute, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_SetMute_Event = new WeakEvent();
       public event Delegate_OnResult_SetMute OnResult_SetMute
       {
			add{OnResult_SetMute_Event.Register(value);}
			remove{OnResult_SetMute_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_SetGreenVideoGain(CpRenderingControl sender, System.UInt32 InstanceID, System.UInt16 DesiredGreenVideoGain, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_SetGreenVideoGain_Event = new WeakEvent();
       public event Delegate_OnResult_SetGreenVideoGain OnResult_SetGreenVideoGain
       {
			add{OnResult_SetGreenVideoGain_Event.Register(value);}
			remove{OnResult_SetGreenVideoGain_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_SetSharpness(CpRenderingControl sender, System.UInt32 InstanceID, System.UInt16 DesiredSharpness, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_SetSharpness_Event = new WeakEvent();
       public event Delegate_OnResult_SetSharpness OnResult_SetSharpness
       {
			add{OnResult_SetSharpness_Event.Register(value);}
			remove{OnResult_SetSharpness_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_SetHorizontalKeystone(CpRenderingControl sender, System.UInt32 InstanceID, System.Int16 DesiredHorizontalKeystone, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_SetHorizontalKeystone_Event = new WeakEvent();
       public event Delegate_OnResult_SetHorizontalKeystone OnResult_SetHorizontalKeystone
       {
			add{OnResult_SetHorizontalKeystone_Event.Register(value);}
			remove{OnResult_SetHorizontalKeystone_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_SetColorTemperature(CpRenderingControl sender, System.UInt32 InstanceID, System.UInt16 DesiredColorTemperature, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_SetColorTemperature_Event = new WeakEvent();
       public event Delegate_OnResult_SetColorTemperature OnResult_SetColorTemperature
       {
			add{OnResult_SetColorTemperature_Event.Register(value);}
			remove{OnResult_SetColorTemperature_Event.UnRegister(value);}
       }

        public CpRenderingControl(UPnPService s)
        {
            _S = s;
            _S.OnSubscribe += new UPnPService.UPnPEventSubscribeHandler(_subscribe_sink);
            if(HasStateVariable_LastChange) _S.GetStateVariableObject("LastChange").OnModified += new UPnPStateVariable.ModifiedHandler(LastChange_ModifiedSink);
        }
        public void Dispose()
        {
            _S.OnSubscribe -= new UPnPService.UPnPEventSubscribeHandler(_subscribe_sink);
            OnSubscribe = null;
            if(HasStateVariable_LastChange) _S.GetStateVariableObject("LastChange").OnModified -= new UPnPStateVariable.ModifiedHandler(LastChange_ModifiedSink);
        }
        public void _subscribe(int Timeout)
        {
            _S.Subscribe(Timeout, null);
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
                case Enum_A_ARG_TYPE_Channel.SR:
                    RetVal = "SR";
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
            SR,
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
                  case "SR":
                     RetVal = Enum_A_ARG_TYPE_Channel.SR;
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
                 return(_S.GetStateVariableObject("Volume").Maximum!=null);
             }
        }
        public bool HasMinimum_Volume
        {
             get
             {
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
        public bool HasAction_GetVolumeDBRange
        {
            get
            {
               if(_S.GetAction("GetVolumeDBRange")==null)
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
        public void GetHorizontalKeystone(System.UInt32 InstanceID)
        {
            GetHorizontalKeystone(InstanceID, null, null);
        }
        public void GetHorizontalKeystone(System.UInt32 InstanceID, object _Tag, Delegate_OnResult_GetHorizontalKeystone _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentHorizontalKeystone", "");
           _S.InvokeAsync("GetHorizontalKeystone", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_GetHorizontalKeystone), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetHorizontalKeystone));
        }
        private void Sink_GetHorizontalKeystone(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetHorizontalKeystone)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.Int16 )Args[1].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_GetHorizontalKeystone_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.Int16 )Args[1].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_GetHorizontalKeystone(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetHorizontalKeystone)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.Int16)UPnPService.CreateObjectInstance(typeof(System.Int16),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_GetHorizontalKeystone_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.Int16)UPnPService.CreateObjectInstance(typeof(System.Int16),null), e, StateInfo[0]);
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
               case Enum_A_ARG_TYPE_Channel.SR:
                   args[1] = new UPnPArgument("Channel", "SR");
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
                            case "SR":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR;
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
        public void GetVolume(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel)
        {
            GetVolume(InstanceID, Channel, null, null);
        }
        public void GetVolume(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, object _Tag, Delegate_OnResult_GetVolume _Callback)
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
               case Enum_A_ARG_TYPE_Channel.SR:
                   args[1] = new UPnPArgument("Channel", "SR");
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
           _S.InvokeAsync("GetVolume", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_GetVolume), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetVolume));
        }
        private void Sink_GetVolume(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
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
                            case "SR":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR;
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
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetVolume)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.UInt16 )Args[2].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_GetVolume_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.UInt16 )Args[2].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_GetVolume(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
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
                            case "SR":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR;
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
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetVolume)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_GetVolume_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, StateInfo[0]);
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
        public void SelectPreset(System.UInt32 InstanceID, Enum_A_ARG_TYPE_PresetName PresetName)
        {
            SelectPreset(InstanceID, PresetName, null, null);
        }
        public void SelectPreset(System.UInt32 InstanceID, Enum_A_ARG_TYPE_PresetName PresetName, object _Tag, Delegate_OnResult_SelectPreset _Callback)
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
           _S.InvokeAsync("SelectPreset", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_SelectPreset), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SelectPreset));
        }
        private void Sink_SelectPreset(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
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
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SelectPreset)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_PresetName )Args[1].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_SelectPreset_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_PresetName )Args[1].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_SelectPreset(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
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
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SelectPreset)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_PresetName )Args[1].DataValue, e, StateInfo[0]);
            }
            else
            {
                OnResult_SelectPreset_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_PresetName )Args[1].DataValue, e, StateInfo[0]);
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
               case Enum_A_ARG_TYPE_Channel.SR:
                   args[1] = new UPnPArgument("Channel", "SR");
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
                            case "SR":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR;
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
        public void SetVolume(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.UInt16 DesiredVolume)
        {
            SetVolume(InstanceID, Channel, DesiredVolume, null, null);
        }
        public void SetVolume(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.UInt16 DesiredVolume, object _Tag, Delegate_OnResult_SetVolume _Callback)
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
               case Enum_A_ARG_TYPE_Channel.SR:
                   args[1] = new UPnPArgument("Channel", "SR");
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
           _S.InvokeAsync("SetVolume", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_SetVolume), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetVolume));
        }
        private void Sink_SetVolume(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
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
                            case "SR":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR;
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
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetVolume)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.UInt16 )Args[2].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_SetVolume_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.UInt16 )Args[2].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_SetVolume(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
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
                            case "SR":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR;
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
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetVolume)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.UInt16 )Args[2].DataValue, e, StateInfo[0]);
            }
            else
            {
                OnResult_SetVolume_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.UInt16 )Args[2].DataValue, e, StateInfo[0]);
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
        public void ListPresets(System.UInt32 InstanceID)
        {
            ListPresets(InstanceID, null, null);
        }
        public void ListPresets(System.UInt32 InstanceID, object _Tag, Delegate_OnResult_ListPresets _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentPresetNameList", "");
           _S.InvokeAsync("ListPresets", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_ListPresets), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_ListPresets));
        }
        private void Sink_ListPresets(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_ListPresets)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.String )Args[1].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_ListPresets_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.String )Args[1].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_ListPresets(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_ListPresets)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_ListPresets_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), e, StateInfo[0]);
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
               case Enum_A_ARG_TYPE_Channel.SR:
                   args[1] = new UPnPArgument("Channel", "SR");
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
                            case "SR":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR;
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
        public void SetVolumeDB(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.Int16 DesiredVolume)
        {
            SetVolumeDB(InstanceID, Channel, DesiredVolume, null, null);
        }
        public void SetVolumeDB(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.Int16 DesiredVolume, object _Tag, Delegate_OnResult_SetVolumeDB _Callback)
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
               case Enum_A_ARG_TYPE_Channel.SR:
                   args[1] = new UPnPArgument("Channel", "SR");
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
           _S.InvokeAsync("SetVolumeDB", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_SetVolumeDB), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetVolumeDB));
        }
        private void Sink_SetVolumeDB(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
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
                            case "SR":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR;
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
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetVolumeDB)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Int16 )Args[2].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_SetVolumeDB_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Int16 )Args[2].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_SetVolumeDB(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
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
                            case "SR":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR;
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
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetVolumeDB)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Int16 )Args[2].DataValue, e, StateInfo[0]);
            }
            else
            {
                OnResult_SetVolumeDB_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Int16 )Args[2].DataValue, e, StateInfo[0]);
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
        public void SetRedVideoBlackLevel(System.UInt32 InstanceID, System.UInt16 DesiredRedVideoBlackLevel)
        {
            SetRedVideoBlackLevel(InstanceID, DesiredRedVideoBlackLevel, null, null);
        }
        public void SetRedVideoBlackLevel(System.UInt32 InstanceID, System.UInt16 DesiredRedVideoBlackLevel, object _Tag, Delegate_OnResult_SetRedVideoBlackLevel _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredRedVideoBlackLevel", DesiredRedVideoBlackLevel);
           _S.InvokeAsync("SetRedVideoBlackLevel", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_SetRedVideoBlackLevel), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetRedVideoBlackLevel));
        }
        private void Sink_SetRedVideoBlackLevel(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetRedVideoBlackLevel)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_SetRedVideoBlackLevel_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_SetRedVideoBlackLevel(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetRedVideoBlackLevel)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, StateInfo[0]);
            }
            else
            {
                OnResult_SetRedVideoBlackLevel_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, StateInfo[0]);
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
        public void SetContrast(System.UInt32 InstanceID, System.UInt16 DesiredContrast)
        {
            SetContrast(InstanceID, DesiredContrast, null, null);
        }
        public void SetContrast(System.UInt32 InstanceID, System.UInt16 DesiredContrast, object _Tag, Delegate_OnResult_SetContrast _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredContrast", DesiredContrast);
           _S.InvokeAsync("SetContrast", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_SetContrast), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetContrast));
        }
        private void Sink_SetContrast(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetContrast)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_SetContrast_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_SetContrast(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetContrast)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, StateInfo[0]);
            }
            else
            {
                OnResult_SetContrast_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, StateInfo[0]);
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
               case Enum_A_ARG_TYPE_Channel.SR:
                   args[1] = new UPnPArgument("Channel", "SR");
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
                            case "SR":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR;
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
        public void SetLoudness(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.Boolean DesiredLoudness)
        {
            SetLoudness(InstanceID, Channel, DesiredLoudness, null, null);
        }
        public void SetLoudness(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.Boolean DesiredLoudness, object _Tag, Delegate_OnResult_SetLoudness _Callback)
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
               case Enum_A_ARG_TYPE_Channel.SR:
                   args[1] = new UPnPArgument("Channel", "SR");
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
           _S.InvokeAsync("SetLoudness", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_SetLoudness), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetLoudness));
        }
        private void Sink_SetLoudness(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
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
                            case "SR":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR;
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
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetLoudness)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean )Args[2].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_SetLoudness_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean )Args[2].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_SetLoudness(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
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
                            case "SR":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR;
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
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetLoudness)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean )Args[2].DataValue, e, StateInfo[0]);
            }
            else
            {
                OnResult_SetLoudness_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean )Args[2].DataValue, e, StateInfo[0]);
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
        public void SetBrightness(System.UInt32 InstanceID, System.UInt16 DesiredBrightness)
        {
            SetBrightness(InstanceID, DesiredBrightness, null, null);
        }
        public void SetBrightness(System.UInt32 InstanceID, System.UInt16 DesiredBrightness, object _Tag, Delegate_OnResult_SetBrightness _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredBrightness", DesiredBrightness);
           _S.InvokeAsync("SetBrightness", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_SetBrightness), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetBrightness));
        }
        private void Sink_SetBrightness(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetBrightness)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_SetBrightness_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_SetBrightness(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetBrightness)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, StateInfo[0]);
            }
            else
            {
                OnResult_SetBrightness_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, StateInfo[0]);
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
               case Enum_A_ARG_TYPE_Channel.SR:
                   args[1] = new UPnPArgument("Channel", "SR");
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
                            case "SR":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR;
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
        public void GetLoudness(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel)
        {
            GetLoudness(InstanceID, Channel, null, null);
        }
        public void GetLoudness(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, object _Tag, Delegate_OnResult_GetLoudness _Callback)
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
               case Enum_A_ARG_TYPE_Channel.SR:
                   args[1] = new UPnPArgument("Channel", "SR");
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
           _S.InvokeAsync("GetLoudness", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_GetLoudness), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetLoudness));
        }
        private void Sink_GetLoudness(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
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
                            case "SR":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR;
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
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetLoudness)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean )Args[2].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_GetLoudness_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean )Args[2].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_GetLoudness(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
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
                            case "SR":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR;
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
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetLoudness)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean)UPnPService.CreateObjectInstance(typeof(System.Boolean),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_GetLoudness_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean)UPnPService.CreateObjectInstance(typeof(System.Boolean),null), e, StateInfo[0]);
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
        public void GetColorTemperature(System.UInt32 InstanceID)
        {
            GetColorTemperature(InstanceID, null, null);
        }
        public void GetColorTemperature(System.UInt32 InstanceID, object _Tag, Delegate_OnResult_GetColorTemperature _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentColorTemperature", "");
           _S.InvokeAsync("GetColorTemperature", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_GetColorTemperature), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetColorTemperature));
        }
        private void Sink_GetColorTemperature(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetColorTemperature)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_GetColorTemperature_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_GetColorTemperature(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetColorTemperature)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_GetColorTemperature_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, StateInfo[0]);
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
        public void GetSharpness(System.UInt32 InstanceID)
        {
            GetSharpness(InstanceID, null, null);
        }
        public void GetSharpness(System.UInt32 InstanceID, object _Tag, Delegate_OnResult_GetSharpness _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentSharpness", "");
           _S.InvokeAsync("GetSharpness", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_GetSharpness), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetSharpness));
        }
        private void Sink_GetSharpness(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetSharpness)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_GetSharpness_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_GetSharpness(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetSharpness)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_GetSharpness_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, StateInfo[0]);
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
        public void GetContrast(System.UInt32 InstanceID)
        {
            GetContrast(InstanceID, null, null);
        }
        public void GetContrast(System.UInt32 InstanceID, object _Tag, Delegate_OnResult_GetContrast _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentContrast", "");
           _S.InvokeAsync("GetContrast", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_GetContrast), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetContrast));
        }
        private void Sink_GetContrast(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetContrast)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_GetContrast_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_GetContrast(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetContrast)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_GetContrast_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, StateInfo[0]);
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
        public void GetGreenVideoGain(System.UInt32 InstanceID)
        {
            GetGreenVideoGain(InstanceID, null, null);
        }
        public void GetGreenVideoGain(System.UInt32 InstanceID, object _Tag, Delegate_OnResult_GetGreenVideoGain _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentGreenVideoGain", "");
           _S.InvokeAsync("GetGreenVideoGain", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_GetGreenVideoGain), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetGreenVideoGain));
        }
        private void Sink_GetGreenVideoGain(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetGreenVideoGain)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_GetGreenVideoGain_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_GetGreenVideoGain(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetGreenVideoGain)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_GetGreenVideoGain_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, StateInfo[0]);
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
        public void SetRedVideoGain(System.UInt32 InstanceID, System.UInt16 DesiredRedVideoGain)
        {
            SetRedVideoGain(InstanceID, DesiredRedVideoGain, null, null);
        }
        public void SetRedVideoGain(System.UInt32 InstanceID, System.UInt16 DesiredRedVideoGain, object _Tag, Delegate_OnResult_SetRedVideoGain _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredRedVideoGain", DesiredRedVideoGain);
           _S.InvokeAsync("SetRedVideoGain", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_SetRedVideoGain), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetRedVideoGain));
        }
        private void Sink_SetRedVideoGain(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetRedVideoGain)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_SetRedVideoGain_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_SetRedVideoGain(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetRedVideoGain)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, StateInfo[0]);
            }
            else
            {
                OnResult_SetRedVideoGain_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, StateInfo[0]);
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
        public void SetGreenVideoBlackLevel(System.UInt32 InstanceID, System.UInt16 DesiredGreenVideoBlackLevel)
        {
            SetGreenVideoBlackLevel(InstanceID, DesiredGreenVideoBlackLevel, null, null);
        }
        public void SetGreenVideoBlackLevel(System.UInt32 InstanceID, System.UInt16 DesiredGreenVideoBlackLevel, object _Tag, Delegate_OnResult_SetGreenVideoBlackLevel _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredGreenVideoBlackLevel", DesiredGreenVideoBlackLevel);
           _S.InvokeAsync("SetGreenVideoBlackLevel", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_SetGreenVideoBlackLevel), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetGreenVideoBlackLevel));
        }
        private void Sink_SetGreenVideoBlackLevel(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetGreenVideoBlackLevel)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_SetGreenVideoBlackLevel_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_SetGreenVideoBlackLevel(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetGreenVideoBlackLevel)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, StateInfo[0]);
            }
            else
            {
                OnResult_SetGreenVideoBlackLevel_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, StateInfo[0]);
            }
        }
        public void Sync_GetVolumeDBRange(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, out System.Int16 MinValue, out System.Int16 MaxValue)
        {
           UPnPArgument[] args = new UPnPArgument[4];
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
               case Enum_A_ARG_TYPE_Channel.SR:
                   args[1] = new UPnPArgument("Channel", "SR");
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
           args[2] = new UPnPArgument("MinValue", "");
           args[3] = new UPnPArgument("MaxValue", "");
            _S.InvokeSync("GetVolumeDBRange", args);
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
                            case "SR":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR;
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
            MinValue = (System.Int16) args[2].DataValue;
            MaxValue = (System.Int16) args[3].DataValue;
            return;
        }
        public void GetVolumeDBRange(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel)
        {
            GetVolumeDBRange(InstanceID, Channel, null, null);
        }
        public void GetVolumeDBRange(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, object _Tag, Delegate_OnResult_GetVolumeDBRange _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[4];
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
               case Enum_A_ARG_TYPE_Channel.SR:
                   args[1] = new UPnPArgument("Channel", "SR");
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
           args[2] = new UPnPArgument("MinValue", "");
           args[3] = new UPnPArgument("MaxValue", "");
           _S.InvokeAsync("GetVolumeDBRange", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_GetVolumeDBRange), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetVolumeDBRange));
        }
        private void Sink_GetVolumeDBRange(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
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
                            case "SR":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR;
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
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetVolumeDBRange)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Int16 )Args[2].DataValue, (System.Int16 )Args[3].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_GetVolumeDBRange_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Int16 )Args[2].DataValue, (System.Int16 )Args[3].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_GetVolumeDBRange(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
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
                            case "SR":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR;
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
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetVolumeDBRange)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Int16)UPnPService.CreateObjectInstance(typeof(System.Int16),null), (System.Int16)UPnPService.CreateObjectInstance(typeof(System.Int16),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_GetVolumeDBRange_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Int16)UPnPService.CreateObjectInstance(typeof(System.Int16),null), (System.Int16)UPnPService.CreateObjectInstance(typeof(System.Int16),null), e, StateInfo[0]);
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
        public void GetRedVideoBlackLevel(System.UInt32 InstanceID)
        {
            GetRedVideoBlackLevel(InstanceID, null, null);
        }
        public void GetRedVideoBlackLevel(System.UInt32 InstanceID, object _Tag, Delegate_OnResult_GetRedVideoBlackLevel _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentRedVideoBlackLevel", "");
           _S.InvokeAsync("GetRedVideoBlackLevel", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_GetRedVideoBlackLevel), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetRedVideoBlackLevel));
        }
        private void Sink_GetRedVideoBlackLevel(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetRedVideoBlackLevel)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_GetRedVideoBlackLevel_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_GetRedVideoBlackLevel(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetRedVideoBlackLevel)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_GetRedVideoBlackLevel_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, StateInfo[0]);
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
        public void GetBlueVideoBlackLevel(System.UInt32 InstanceID)
        {
            GetBlueVideoBlackLevel(InstanceID, null, null);
        }
        public void GetBlueVideoBlackLevel(System.UInt32 InstanceID, object _Tag, Delegate_OnResult_GetBlueVideoBlackLevel _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentBlueVideoBlackLevel", "");
           _S.InvokeAsync("GetBlueVideoBlackLevel", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_GetBlueVideoBlackLevel), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetBlueVideoBlackLevel));
        }
        private void Sink_GetBlueVideoBlackLevel(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetBlueVideoBlackLevel)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_GetBlueVideoBlackLevel_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_GetBlueVideoBlackLevel(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetBlueVideoBlackLevel)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_GetBlueVideoBlackLevel_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, StateInfo[0]);
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
        public void GetBlueVideoGain(System.UInt32 InstanceID)
        {
            GetBlueVideoGain(InstanceID, null, null);
        }
        public void GetBlueVideoGain(System.UInt32 InstanceID, object _Tag, Delegate_OnResult_GetBlueVideoGain _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentBlueVideoGain", "");
           _S.InvokeAsync("GetBlueVideoGain", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_GetBlueVideoGain), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetBlueVideoGain));
        }
        private void Sink_GetBlueVideoGain(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetBlueVideoGain)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_GetBlueVideoGain_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_GetBlueVideoGain(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetBlueVideoGain)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_GetBlueVideoGain_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, StateInfo[0]);
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
        public void SetBlueVideoBlackLevel(System.UInt32 InstanceID, System.UInt16 DesiredBlueVideoBlackLevel)
        {
            SetBlueVideoBlackLevel(InstanceID, DesiredBlueVideoBlackLevel, null, null);
        }
        public void SetBlueVideoBlackLevel(System.UInt32 InstanceID, System.UInt16 DesiredBlueVideoBlackLevel, object _Tag, Delegate_OnResult_SetBlueVideoBlackLevel _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredBlueVideoBlackLevel", DesiredBlueVideoBlackLevel);
           _S.InvokeAsync("SetBlueVideoBlackLevel", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_SetBlueVideoBlackLevel), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetBlueVideoBlackLevel));
        }
        private void Sink_SetBlueVideoBlackLevel(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetBlueVideoBlackLevel)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_SetBlueVideoBlackLevel_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_SetBlueVideoBlackLevel(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetBlueVideoBlackLevel)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, StateInfo[0]);
            }
            else
            {
                OnResult_SetBlueVideoBlackLevel_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, StateInfo[0]);
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
               case Enum_A_ARG_TYPE_Channel.SR:
                   args[1] = new UPnPArgument("Channel", "SR");
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
                            case "SR":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR;
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
        public void GetMute(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel)
        {
            GetMute(InstanceID, Channel, null, null);
        }
        public void GetMute(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, object _Tag, Delegate_OnResult_GetMute _Callback)
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
               case Enum_A_ARG_TYPE_Channel.SR:
                   args[1] = new UPnPArgument("Channel", "SR");
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
           _S.InvokeAsync("GetMute", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_GetMute), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetMute));
        }
        private void Sink_GetMute(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
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
                            case "SR":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR;
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
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetMute)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean )Args[2].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_GetMute_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean )Args[2].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_GetMute(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
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
                            case "SR":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR;
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
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetMute)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean)UPnPService.CreateObjectInstance(typeof(System.Boolean),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_GetMute_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean)UPnPService.CreateObjectInstance(typeof(System.Boolean),null), e, StateInfo[0]);
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
        public void SetBlueVideoGain(System.UInt32 InstanceID, System.UInt16 DesiredBlueVideoGain)
        {
            SetBlueVideoGain(InstanceID, DesiredBlueVideoGain, null, null);
        }
        public void SetBlueVideoGain(System.UInt32 InstanceID, System.UInt16 DesiredBlueVideoGain, object _Tag, Delegate_OnResult_SetBlueVideoGain _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredBlueVideoGain", DesiredBlueVideoGain);
           _S.InvokeAsync("SetBlueVideoGain", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_SetBlueVideoGain), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetBlueVideoGain));
        }
        private void Sink_SetBlueVideoGain(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetBlueVideoGain)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_SetBlueVideoGain_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_SetBlueVideoGain(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetBlueVideoGain)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, StateInfo[0]);
            }
            else
            {
                OnResult_SetBlueVideoGain_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, StateInfo[0]);
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
        public void GetVerticalKeystone(System.UInt32 InstanceID)
        {
            GetVerticalKeystone(InstanceID, null, null);
        }
        public void GetVerticalKeystone(System.UInt32 InstanceID, object _Tag, Delegate_OnResult_GetVerticalKeystone _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentVerticalKeystone", "");
           _S.InvokeAsync("GetVerticalKeystone", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_GetVerticalKeystone), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetVerticalKeystone));
        }
        private void Sink_GetVerticalKeystone(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetVerticalKeystone)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.Int16 )Args[1].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_GetVerticalKeystone_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.Int16 )Args[1].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_GetVerticalKeystone(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetVerticalKeystone)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.Int16)UPnPService.CreateObjectInstance(typeof(System.Int16),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_GetVerticalKeystone_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.Int16)UPnPService.CreateObjectInstance(typeof(System.Int16),null), e, StateInfo[0]);
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
        public void SetVerticalKeystone(System.UInt32 InstanceID, System.Int16 DesiredVerticalKeystone)
        {
            SetVerticalKeystone(InstanceID, DesiredVerticalKeystone, null, null);
        }
        public void SetVerticalKeystone(System.UInt32 InstanceID, System.Int16 DesiredVerticalKeystone, object _Tag, Delegate_OnResult_SetVerticalKeystone _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredVerticalKeystone", DesiredVerticalKeystone);
           _S.InvokeAsync("SetVerticalKeystone", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_SetVerticalKeystone), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetVerticalKeystone));
        }
        private void Sink_SetVerticalKeystone(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetVerticalKeystone)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.Int16 )Args[1].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_SetVerticalKeystone_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.Int16 )Args[1].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_SetVerticalKeystone(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetVerticalKeystone)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.Int16 )Args[1].DataValue, e, StateInfo[0]);
            }
            else
            {
                OnResult_SetVerticalKeystone_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.Int16 )Args[1].DataValue, e, StateInfo[0]);
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
        public void GetBrightness(System.UInt32 InstanceID)
        {
            GetBrightness(InstanceID, null, null);
        }
        public void GetBrightness(System.UInt32 InstanceID, object _Tag, Delegate_OnResult_GetBrightness _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentBrightness", "");
           _S.InvokeAsync("GetBrightness", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_GetBrightness), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetBrightness));
        }
        private void Sink_GetBrightness(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetBrightness)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_GetBrightness_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_GetBrightness(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetBrightness)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_GetBrightness_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, StateInfo[0]);
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
               case Enum_A_ARG_TYPE_Channel.SR:
                   args[1] = new UPnPArgument("Channel", "SR");
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
                            case "SR":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR;
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
        public void GetVolumeDB(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel)
        {
            GetVolumeDB(InstanceID, Channel, null, null);
        }
        public void GetVolumeDB(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, object _Tag, Delegate_OnResult_GetVolumeDB _Callback)
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
               case Enum_A_ARG_TYPE_Channel.SR:
                   args[1] = new UPnPArgument("Channel", "SR");
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
           _S.InvokeAsync("GetVolumeDB", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_GetVolumeDB), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetVolumeDB));
        }
        private void Sink_GetVolumeDB(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
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
                            case "SR":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR;
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
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetVolumeDB)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Int16 )Args[2].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_GetVolumeDB_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Int16 )Args[2].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_GetVolumeDB(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
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
                            case "SR":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR;
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
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetVolumeDB)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Int16)UPnPService.CreateObjectInstance(typeof(System.Int16),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_GetVolumeDB_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Int16)UPnPService.CreateObjectInstance(typeof(System.Int16),null), e, StateInfo[0]);
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
        public void GetGreenVideoBlackLevel(System.UInt32 InstanceID)
        {
            GetGreenVideoBlackLevel(InstanceID, null, null);
        }
        public void GetGreenVideoBlackLevel(System.UInt32 InstanceID, object _Tag, Delegate_OnResult_GetGreenVideoBlackLevel _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentGreenVideoBlackLevel", "");
           _S.InvokeAsync("GetGreenVideoBlackLevel", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_GetGreenVideoBlackLevel), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetGreenVideoBlackLevel));
        }
        private void Sink_GetGreenVideoBlackLevel(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetGreenVideoBlackLevel)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_GetGreenVideoBlackLevel_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_GetGreenVideoBlackLevel(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetGreenVideoBlackLevel)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_GetGreenVideoBlackLevel_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, StateInfo[0]);
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
        public void GetRedVideoGain(System.UInt32 InstanceID)
        {
            GetRedVideoGain(InstanceID, null, null);
        }
        public void GetRedVideoGain(System.UInt32 InstanceID, object _Tag, Delegate_OnResult_GetRedVideoGain _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("CurrentRedVideoGain", "");
           _S.InvokeAsync("GetRedVideoGain", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_GetRedVideoGain), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetRedVideoGain));
        }
        private void Sink_GetRedVideoGain(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetRedVideoGain)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_GetRedVideoGain_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_GetRedVideoGain(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetRedVideoGain)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_GetRedVideoGain_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16)UPnPService.CreateObjectInstance(typeof(System.UInt16),null), e, StateInfo[0]);
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
               case Enum_A_ARG_TYPE_Channel.SR:
                   args[1] = new UPnPArgument("Channel", "SR");
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
                            case "SR":
                                args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR;
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
        public void SetMute(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.Boolean DesiredMute)
        {
            SetMute(InstanceID, Channel, DesiredMute, null, null);
        }
        public void SetMute(System.UInt32 InstanceID, Enum_A_ARG_TYPE_Channel Channel, System.Boolean DesiredMute, object _Tag, Delegate_OnResult_SetMute _Callback)
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
               case Enum_A_ARG_TYPE_Channel.SR:
                   args[1] = new UPnPArgument("Channel", "SR");
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
           _S.InvokeAsync("SetMute", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_SetMute), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetMute));
        }
        private void Sink_SetMute(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
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
                            case "SR":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR;
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
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetMute)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean )Args[2].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_SetMute_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean )Args[2].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_SetMute(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
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
                            case "SR":
                                Args[i].DataValue = Enum_A_ARG_TYPE_Channel.SR;
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
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetMute)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean )Args[2].DataValue, e, StateInfo[0]);
            }
            else
            {
                OnResult_SetMute_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_Channel )Args[1].DataValue, (System.Boolean )Args[2].DataValue, e, StateInfo[0]);
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
        public void SetGreenVideoGain(System.UInt32 InstanceID, System.UInt16 DesiredGreenVideoGain)
        {
            SetGreenVideoGain(InstanceID, DesiredGreenVideoGain, null, null);
        }
        public void SetGreenVideoGain(System.UInt32 InstanceID, System.UInt16 DesiredGreenVideoGain, object _Tag, Delegate_OnResult_SetGreenVideoGain _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredGreenVideoGain", DesiredGreenVideoGain);
           _S.InvokeAsync("SetGreenVideoGain", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_SetGreenVideoGain), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetGreenVideoGain));
        }
        private void Sink_SetGreenVideoGain(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetGreenVideoGain)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_SetGreenVideoGain_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_SetGreenVideoGain(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetGreenVideoGain)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, StateInfo[0]);
            }
            else
            {
                OnResult_SetGreenVideoGain_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, StateInfo[0]);
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
        public void SetSharpness(System.UInt32 InstanceID, System.UInt16 DesiredSharpness)
        {
            SetSharpness(InstanceID, DesiredSharpness, null, null);
        }
        public void SetSharpness(System.UInt32 InstanceID, System.UInt16 DesiredSharpness, object _Tag, Delegate_OnResult_SetSharpness _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredSharpness", DesiredSharpness);
           _S.InvokeAsync("SetSharpness", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_SetSharpness), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetSharpness));
        }
        private void Sink_SetSharpness(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetSharpness)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_SetSharpness_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_SetSharpness(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetSharpness)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, StateInfo[0]);
            }
            else
            {
                OnResult_SetSharpness_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, StateInfo[0]);
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
        public void SetHorizontalKeystone(System.UInt32 InstanceID, System.Int16 DesiredHorizontalKeystone)
        {
            SetHorizontalKeystone(InstanceID, DesiredHorizontalKeystone, null, null);
        }
        public void SetHorizontalKeystone(System.UInt32 InstanceID, System.Int16 DesiredHorizontalKeystone, object _Tag, Delegate_OnResult_SetHorizontalKeystone _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredHorizontalKeystone", DesiredHorizontalKeystone);
           _S.InvokeAsync("SetHorizontalKeystone", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_SetHorizontalKeystone), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetHorizontalKeystone));
        }
        private void Sink_SetHorizontalKeystone(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetHorizontalKeystone)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.Int16 )Args[1].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_SetHorizontalKeystone_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.Int16 )Args[1].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_SetHorizontalKeystone(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetHorizontalKeystone)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.Int16 )Args[1].DataValue, e, StateInfo[0]);
            }
            else
            {
                OnResult_SetHorizontalKeystone_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.Int16 )Args[1].DataValue, e, StateInfo[0]);
            }
        }
        public void Sync_SetColorTemperature(System.UInt32 InstanceID, System.UInt16 DesiredColorTemperature)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredColorTemperature", DesiredColorTemperature);
            _S.InvokeSync("SetColorTemperature", args);
            InstanceID = (System.UInt32) args[0].DataValue;
            DesiredColorTemperature = (System.UInt16) args[1].DataValue;
            return;
        }
        public void SetColorTemperature(System.UInt32 InstanceID, System.UInt16 DesiredColorTemperature)
        {
            SetColorTemperature(InstanceID, DesiredColorTemperature, null, null);
        }
        public void SetColorTemperature(System.UInt32 InstanceID, System.UInt16 DesiredColorTemperature, object _Tag, Delegate_OnResult_SetColorTemperature _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[2];
           args[0] = new UPnPArgument("InstanceID", InstanceID);
           args[1] = new UPnPArgument("DesiredColorTemperature", DesiredColorTemperature);
           _S.InvokeAsync("SetColorTemperature", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_SetColorTemperature), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_SetColorTemperature));
        }
        private void Sink_SetColorTemperature(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetColorTemperature)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_SetColorTemperature_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_SetColorTemperature(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_SetColorTemperature)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, StateInfo[0]);
            }
            else
            {
                OnResult_SetColorTemperature_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (System.UInt16 )Args[1].DataValue, e, StateInfo[0]);
            }
        }
    }
}