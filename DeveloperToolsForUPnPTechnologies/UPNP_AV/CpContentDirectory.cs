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
    public class CpContentDirectory
    {
       private Hashtable UnspecifiedTable = Hashtable.Synchronized(new Hashtable());
       internal UPnPService _S;

       public UPnPService GetUPnPService()
       {
            return(_S);
       }
       public static string SERVICE_NAME = "urn:schemas-upnp-org:service:ContentDirectory:";
       public double VERSION
       {
           get
           {
               return(double.Parse(_S.Version));
           }
       }

       public delegate void StateVariableModifiedHandler_TransferIDs(CpContentDirectory sender, System.String NewValue);
       private WeakEvent OnStateVariable_TransferIDs_Event = new WeakEvent();
       public event StateVariableModifiedHandler_TransferIDs OnStateVariable_TransferIDs
       {
			add{OnStateVariable_TransferIDs_Event.Register(value);}
			remove{OnStateVariable_TransferIDs_Event.UnRegister(value);}
       }
       protected void TransferIDs_ModifiedSink(UPnPStateVariable Var, object NewValue)
       {
            OnStateVariable_TransferIDs_Event.Fire(this, (System.String)NewValue);
       }
       public delegate void StateVariableModifiedHandler_ContainerUpdateIDs(CpContentDirectory sender, System.String NewValue);
       private WeakEvent OnStateVariable_ContainerUpdateIDs_Event = new WeakEvent();
       public event StateVariableModifiedHandler_ContainerUpdateIDs OnStateVariable_ContainerUpdateIDs
       {
			add{OnStateVariable_ContainerUpdateIDs_Event.Register(value);}
			remove{OnStateVariable_ContainerUpdateIDs_Event.UnRegister(value);}
       }
       protected void ContainerUpdateIDs_ModifiedSink(UPnPStateVariable Var, object NewValue)
       {
            OnStateVariable_ContainerUpdateIDs_Event.Fire(this, (System.String)NewValue);
       }
       public delegate void StateVariableModifiedHandler_SystemUpdateID(CpContentDirectory sender, System.UInt32 NewValue);
       private WeakEvent OnStateVariable_SystemUpdateID_Event = new WeakEvent();
       public event StateVariableModifiedHandler_SystemUpdateID OnStateVariable_SystemUpdateID
       {
			add{OnStateVariable_SystemUpdateID_Event.Register(value);}
			remove{OnStateVariable_SystemUpdateID_Event.UnRegister(value);}
       }
       protected void SystemUpdateID_ModifiedSink(UPnPStateVariable Var, object NewValue)
       {
            OnStateVariable_SystemUpdateID_Event.Fire(this, (System.UInt32)NewValue);
       }
       public delegate void SubscribeHandler(CpContentDirectory sender, bool Success);
       public event SubscribeHandler OnSubscribe;
       public delegate void Delegate_OnResult_ExportResource(CpContentDirectory sender, System.Uri SourceURI, System.Uri DestinationURI, System.UInt32 TransferID, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_ExportResource_Event = new WeakEvent();
       public event Delegate_OnResult_ExportResource OnResult_ExportResource
       {
			add{OnResult_ExportResource_Event.Register(value);}
			remove{OnResult_ExportResource_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_StopTransferResource(CpContentDirectory sender, System.UInt32 TransferID, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_StopTransferResource_Event = new WeakEvent();
       public event Delegate_OnResult_StopTransferResource OnResult_StopTransferResource
       {
			add{OnResult_StopTransferResource_Event.Register(value);}
			remove{OnResult_StopTransferResource_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_DestroyObject(CpContentDirectory sender, System.String ObjectID, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_DestroyObject_Event = new WeakEvent();
       public event Delegate_OnResult_DestroyObject OnResult_DestroyObject
       {
			add{OnResult_DestroyObject_Event.Register(value);}
			remove{OnResult_DestroyObject_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_UpdateObject(CpContentDirectory sender, System.String ObjectID, System.String CurrentTagValue, System.String NewTagValue, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_UpdateObject_Event = new WeakEvent();
       public event Delegate_OnResult_UpdateObject OnResult_UpdateObject
       {
			add{OnResult_UpdateObject_Event.Register(value);}
			remove{OnResult_UpdateObject_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_GetSystemUpdateID(CpContentDirectory sender, System.UInt32 Id, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_GetSystemUpdateID_Event = new WeakEvent();
       public event Delegate_OnResult_GetSystemUpdateID OnResult_GetSystemUpdateID
       {
			add{OnResult_GetSystemUpdateID_Event.Register(value);}
			remove{OnResult_GetSystemUpdateID_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_GetTransferProgress(CpContentDirectory sender, System.UInt32 TransferID, Enum_A_ARG_TYPE_TransferStatus TransferStatus, System.String TransferLength, System.String TransferTotal, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_GetTransferProgress_Event = new WeakEvent();
       public event Delegate_OnResult_GetTransferProgress OnResult_GetTransferProgress
       {
			add{OnResult_GetTransferProgress_Event.Register(value);}
			remove{OnResult_GetTransferProgress_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_GetSearchCapabilities(CpContentDirectory sender, System.String SearchCaps, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_GetSearchCapabilities_Event = new WeakEvent();
       public event Delegate_OnResult_GetSearchCapabilities OnResult_GetSearchCapabilities
       {
			add{OnResult_GetSearchCapabilities_Event.Register(value);}
			remove{OnResult_GetSearchCapabilities_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_CreateObject(CpContentDirectory sender, System.String ContainerID, System.String Elements, System.String ObjectID, System.String Result, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_CreateObject_Event = new WeakEvent();
       public event Delegate_OnResult_CreateObject OnResult_CreateObject
       {
			add{OnResult_CreateObject_Event.Register(value);}
			remove{OnResult_CreateObject_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_ImportResource(CpContentDirectory sender, System.Uri SourceURI, System.Uri DestinationURI, System.UInt32 TransferID, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_ImportResource_Event = new WeakEvent();
       public event Delegate_OnResult_ImportResource OnResult_ImportResource
       {
			add{OnResult_ImportResource_Event.Register(value);}
			remove{OnResult_ImportResource_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_Search(CpContentDirectory sender, System.String ContainerID, System.String SearchCriteria, System.String Filter, System.UInt32 StartingIndex, System.UInt32 RequestedCount, System.String SortCriteria, System.String Result, System.UInt32 NumberReturned, System.UInt32 TotalMatches, System.UInt32 UpdateID, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_Search_Event = new WeakEvent();
       public event Delegate_OnResult_Search OnResult_Search
       {
			add{OnResult_Search_Event.Register(value);}
			remove{OnResult_Search_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_GetSortCapabilities(CpContentDirectory sender, System.String SortCaps, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_GetSortCapabilities_Event = new WeakEvent();
       public event Delegate_OnResult_GetSortCapabilities OnResult_GetSortCapabilities
       {
			add{OnResult_GetSortCapabilities_Event.Register(value);}
			remove{OnResult_GetSortCapabilities_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_Browse(CpContentDirectory sender, System.String ObjectID, Enum_A_ARG_TYPE_BrowseFlag BrowseFlag, System.String Filter, System.UInt32 StartingIndex, System.UInt32 RequestedCount, System.String SortCriteria, System.String Result, System.UInt32 NumberReturned, System.UInt32 TotalMatches, System.UInt32 UpdateID, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_Browse_Event = new WeakEvent();
       public event Delegate_OnResult_Browse OnResult_Browse
       {
			add{OnResult_Browse_Event.Register(value);}
			remove{OnResult_Browse_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_CreateReference(CpContentDirectory sender, System.String ContainerID, System.String ObjectID, System.String NewID, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_CreateReference_Event = new WeakEvent();
       public event Delegate_OnResult_CreateReference OnResult_CreateReference
       {
			add{OnResult_CreateReference_Event.Register(value);}
			remove{OnResult_CreateReference_Event.UnRegister(value);}
       }
       public delegate void Delegate_OnResult_DeleteResource(CpContentDirectory sender, System.Uri ResourceURI, UPnPInvokeException e, object _Tag);
       private WeakEvent OnResult_DeleteResource_Event = new WeakEvent();
       public event Delegate_OnResult_DeleteResource OnResult_DeleteResource
       {
			add{OnResult_DeleteResource_Event.Register(value);}
			remove{OnResult_DeleteResource_Event.UnRegister(value);}
       }

        public CpContentDirectory(UPnPService s)
        {
            _S = s;
            _S.OnSubscribe += new UPnPService.UPnPEventSubscribeHandler(_subscribe_sink);
            if(HasStateVariable_TransferIDs) _S.GetStateVariableObject("TransferIDs").OnModified += new UPnPStateVariable.ModifiedHandler(TransferIDs_ModifiedSink);
            if(HasStateVariable_ContainerUpdateIDs) _S.GetStateVariableObject("ContainerUpdateIDs").OnModified += new UPnPStateVariable.ModifiedHandler(ContainerUpdateIDs_ModifiedSink);
            if(HasStateVariable_SystemUpdateID) _S.GetStateVariableObject("SystemUpdateID").OnModified += new UPnPStateVariable.ModifiedHandler(SystemUpdateID_ModifiedSink);
        }
        public void Dispose()
        {
            _S.OnSubscribe -= new UPnPService.UPnPEventSubscribeHandler(_subscribe_sink);
            OnSubscribe = null;
            if(HasStateVariable_TransferIDs) _S.GetStateVariableObject("TransferIDs").OnModified -= new UPnPStateVariable.ModifiedHandler(TransferIDs_ModifiedSink);
            if(HasStateVariable_ContainerUpdateIDs) _S.GetStateVariableObject("ContainerUpdateIDs").OnModified -= new UPnPStateVariable.ModifiedHandler(ContainerUpdateIDs_ModifiedSink);
            if(HasStateVariable_SystemUpdateID) _S.GetStateVariableObject("SystemUpdateID").OnModified -= new UPnPStateVariable.ModifiedHandler(SystemUpdateID_ModifiedSink);
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
        public string[] Values_A_ARG_TYPE_TransferStatus
        {
            get
            {
                UPnPStateVariable sv = _S.GetStateVariableObject("A_ARG_TYPE_TransferStatus");
                return(sv.AllowedStringValues);
            }
        }
        public string Enum_A_ARG_TYPE_TransferStatus_ToString(Enum_A_ARG_TYPE_TransferStatus en)
        {
            string RetVal = "";
            switch(en)
            {
                case Enum_A_ARG_TYPE_TransferStatus.COMPLETED:
                    RetVal = "COMPLETED";
                    break;
                case Enum_A_ARG_TYPE_TransferStatus.ERROR:
                    RetVal = "ERROR";
                    break;
                case Enum_A_ARG_TYPE_TransferStatus.IN_PROGRESS:
                    RetVal = "IN_PROGRESS";
                    break;
                case Enum_A_ARG_TYPE_TransferStatus.STOPPED:
                    RetVal = "STOPPED";
                    break;
                case Enum_A_ARG_TYPE_TransferStatus._UNSPECIFIED_:
                    RetVal = GetUnspecifiedValue("Enum_A_ARG_TYPE_TransferStatus");
                    break;
            }
            return(RetVal);
        }
        public enum Enum_A_ARG_TYPE_TransferStatus
        {
            _UNSPECIFIED_,
            COMPLETED,
            ERROR,
            IN_PROGRESS,
            STOPPED,
        }
        public Enum_A_ARG_TYPE_TransferStatus A_ARG_TYPE_TransferStatus
        {
            get
            {
               Enum_A_ARG_TYPE_TransferStatus RetVal = 0;
               string v = (string)_S.GetStateVariable("A_ARG_TYPE_TransferStatus");
               switch(v)
               {
                  case "COMPLETED":
                     RetVal = Enum_A_ARG_TYPE_TransferStatus.COMPLETED;
                     break;
                  case "ERROR":
                     RetVal = Enum_A_ARG_TYPE_TransferStatus.ERROR;
                     break;
                  case "IN_PROGRESS":
                     RetVal = Enum_A_ARG_TYPE_TransferStatus.IN_PROGRESS;
                     break;
                  case "STOPPED":
                     RetVal = Enum_A_ARG_TYPE_TransferStatus.STOPPED;
                     break;
                  default:
                     RetVal = Enum_A_ARG_TYPE_TransferStatus._UNSPECIFIED_;
                     SetUnspecifiedValue("Enum_A_ARG_TYPE_TransferStatus", v);
                     break;
               }
               return(RetVal);
           }
        }
        public string[] Values_A_ARG_TYPE_BrowseFlag
        {
            get
            {
                UPnPStateVariable sv = _S.GetStateVariableObject("A_ARG_TYPE_BrowseFlag");
                return(sv.AllowedStringValues);
            }
        }
        public string Enum_A_ARG_TYPE_BrowseFlag_ToString(Enum_A_ARG_TYPE_BrowseFlag en)
        {
            string RetVal = "";
            switch(en)
            {
                case Enum_A_ARG_TYPE_BrowseFlag.BROWSEMETADATA:
                    RetVal = "BrowseMetadata";
                    break;
                case Enum_A_ARG_TYPE_BrowseFlag.BROWSEDIRECTCHILDREN:
                    RetVal = "BrowseDirectChildren";
                    break;
                case Enum_A_ARG_TYPE_BrowseFlag._UNSPECIFIED_:
                    RetVal = GetUnspecifiedValue("Enum_A_ARG_TYPE_BrowseFlag");
                    break;
            }
            return(RetVal);
        }
        public enum Enum_A_ARG_TYPE_BrowseFlag
        {
            _UNSPECIFIED_,
            BROWSEMETADATA,
            BROWSEDIRECTCHILDREN,
        }
        public Enum_A_ARG_TYPE_BrowseFlag A_ARG_TYPE_BrowseFlag
        {
            get
            {
               Enum_A_ARG_TYPE_BrowseFlag RetVal = 0;
               string v = (string)_S.GetStateVariable("A_ARG_TYPE_BrowseFlag");
               switch(v)
               {
                  case "BrowseMetadata":
                     RetVal = Enum_A_ARG_TYPE_BrowseFlag.BROWSEMETADATA;
                     break;
                  case "BrowseDirectChildren":
                     RetVal = Enum_A_ARG_TYPE_BrowseFlag.BROWSEDIRECTCHILDREN;
                     break;
                  default:
                     RetVal = Enum_A_ARG_TYPE_BrowseFlag._UNSPECIFIED_;
                     SetUnspecifiedValue("Enum_A_ARG_TYPE_BrowseFlag", v);
                     break;
               }
               return(RetVal);
           }
        }
        public System.String A_ARG_TYPE_SortCriteria
        {
            get
            {
               return((System.String)_S.GetStateVariable("A_ARG_TYPE_SortCriteria"));
            }
        }
        public System.String A_ARG_TYPE_TransferLength
        {
            get
            {
               return((System.String)_S.GetStateVariable("A_ARG_TYPE_TransferLength"));
            }
        }
        public System.String TransferIDs
        {
            get
            {
               return((System.String)_S.GetStateVariable("TransferIDs"));
            }
        }
        public System.UInt32 A_ARG_TYPE_UpdateID
        {
            get
            {
               return((System.UInt32)_S.GetStateVariable("A_ARG_TYPE_UpdateID"));
            }
        }
        public System.String A_ARG_TYPE_SearchCriteria
        {
            get
            {
               return((System.String)_S.GetStateVariable("A_ARG_TYPE_SearchCriteria"));
            }
        }
        public System.String A_ARG_TYPE_Filter
        {
            get
            {
               return((System.String)_S.GetStateVariable("A_ARG_TYPE_Filter"));
            }
        }
        public System.String ContainerUpdateIDs
        {
            get
            {
               return((System.String)_S.GetStateVariable("ContainerUpdateIDs"));
            }
        }
        public System.String A_ARG_TYPE_Result
        {
            get
            {
               return((System.String)_S.GetStateVariable("A_ARG_TYPE_Result"));
            }
        }
        public System.UInt32 A_ARG_TYPE_Index
        {
            get
            {
               return((System.UInt32)_S.GetStateVariable("A_ARG_TYPE_Index"));
            }
        }
        public System.UInt32 A_ARG_TYPE_TransferID
        {
            get
            {
               return((System.UInt32)_S.GetStateVariable("A_ARG_TYPE_TransferID"));
            }
        }
        public System.String A_ARG_TYPE_TagValueList
        {
            get
            {
               return((System.String)_S.GetStateVariable("A_ARG_TYPE_TagValueList"));
            }
        }
        public System.Uri A_ARG_TYPE_URI
        {
            get
            {
               return((System.Uri)_S.GetStateVariable("A_ARG_TYPE_URI"));
            }
        }
        public System.String A_ARG_TYPE_ObjectID
        {
            get
            {
               return((System.String)_S.GetStateVariable("A_ARG_TYPE_ObjectID"));
            }
        }
        public System.String SortCapabilities
        {
            get
            {
               return((System.String)_S.GetStateVariable("SortCapabilities"));
            }
        }
        public System.UInt32 A_ARG_TYPE_Count
        {
            get
            {
               return((System.UInt32)_S.GetStateVariable("A_ARG_TYPE_Count"));
            }
        }
        public System.String SearchCapabilities
        {
            get
            {
               return((System.String)_S.GetStateVariable("SearchCapabilities"));
            }
        }
        public System.UInt32 SystemUpdateID
        {
            get
            {
               return((System.UInt32)_S.GetStateVariable("SystemUpdateID"));
            }
        }
        public System.String A_ARG_TYPE_TransferTotal
        {
            get
            {
               return((System.String)_S.GetStateVariable("A_ARG_TYPE_TransferTotal"));
            }
        }
        public bool HasStateVariable_A_ARG_TYPE_SortCriteria
        {
            get
            {
               if(_S.GetStateVariableObject("A_ARG_TYPE_SortCriteria")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_A_ARG_TYPE_TransferLength
        {
            get
            {
               if(_S.GetStateVariableObject("A_ARG_TYPE_TransferLength")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_TransferIDs
        {
            get
            {
               if(_S.GetStateVariableObject("TransferIDs")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_A_ARG_TYPE_UpdateID
        {
            get
            {
               if(_S.GetStateVariableObject("A_ARG_TYPE_UpdateID")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_A_ARG_TYPE_SearchCriteria
        {
            get
            {
               if(_S.GetStateVariableObject("A_ARG_TYPE_SearchCriteria")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_A_ARG_TYPE_Filter
        {
            get
            {
               if(_S.GetStateVariableObject("A_ARG_TYPE_Filter")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_ContainerUpdateIDs
        {
            get
            {
               if(_S.GetStateVariableObject("ContainerUpdateIDs")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_A_ARG_TYPE_Result
        {
            get
            {
               if(_S.GetStateVariableObject("A_ARG_TYPE_Result")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_A_ARG_TYPE_Index
        {
            get
            {
               if(_S.GetStateVariableObject("A_ARG_TYPE_Index")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_A_ARG_TYPE_TransferID
        {
            get
            {
               if(_S.GetStateVariableObject("A_ARG_TYPE_TransferID")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_A_ARG_TYPE_TagValueList
        {
            get
            {
               if(_S.GetStateVariableObject("A_ARG_TYPE_TagValueList")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_A_ARG_TYPE_URI
        {
            get
            {
               if(_S.GetStateVariableObject("A_ARG_TYPE_URI")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_A_ARG_TYPE_BrowseFlag
        {
            get
            {
               if(_S.GetStateVariableObject("A_ARG_TYPE_BrowseFlag")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_A_ARG_TYPE_ObjectID
        {
            get
            {
               if(_S.GetStateVariableObject("A_ARG_TYPE_ObjectID")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_SortCapabilities
        {
            get
            {
               if(_S.GetStateVariableObject("SortCapabilities")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_A_ARG_TYPE_Count
        {
            get
            {
               if(_S.GetStateVariableObject("A_ARG_TYPE_Count")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_SearchCapabilities
        {
            get
            {
               if(_S.GetStateVariableObject("SearchCapabilities")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_SystemUpdateID
        {
            get
            {
               if(_S.GetStateVariableObject("SystemUpdateID")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_A_ARG_TYPE_TransferStatus
        {
            get
            {
               if(_S.GetStateVariableObject("A_ARG_TYPE_TransferStatus")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasStateVariable_A_ARG_TYPE_TransferTotal
        {
            get
            {
               if(_S.GetStateVariableObject("A_ARG_TYPE_TransferTotal")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_ExportResource
        {
            get
            {
               if(_S.GetAction("ExportResource")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_StopTransferResource
        {
            get
            {
               if(_S.GetAction("StopTransferResource")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_DestroyObject
        {
            get
            {
               if(_S.GetAction("DestroyObject")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_UpdateObject
        {
            get
            {
               if(_S.GetAction("UpdateObject")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_GetSystemUpdateID
        {
            get
            {
               if(_S.GetAction("GetSystemUpdateID")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_GetTransferProgress
        {
            get
            {
               if(_S.GetAction("GetTransferProgress")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_GetSearchCapabilities
        {
            get
            {
               if(_S.GetAction("GetSearchCapabilities")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_CreateObject
        {
            get
            {
               if(_S.GetAction("CreateObject")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_ImportResource
        {
            get
            {
               if(_S.GetAction("ImportResource")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_Search
        {
            get
            {
               if(_S.GetAction("Search")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_GetSortCapabilities
        {
            get
            {
               if(_S.GetAction("GetSortCapabilities")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_Browse
        {
            get
            {
               if(_S.GetAction("Browse")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_CreateReference
        {
            get
            {
               if(_S.GetAction("CreateReference")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public bool HasAction_DeleteResource
        {
            get
            {
               if(_S.GetAction("DeleteResource")==null)
               {
                   return(false);
               }
               else
               {
                   return(true);
               }
            }
        }
        public void Sync_ExportResource(System.Uri SourceURI, System.Uri DestinationURI, out System.UInt32 TransferID)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("SourceURI", SourceURI);
           args[1] = new UPnPArgument("DestinationURI", DestinationURI);
           args[2] = new UPnPArgument("TransferID", "");
            _S.InvokeSync("ExportResource", args);
            SourceURI = (System.Uri) args[0].DataValue;
            DestinationURI = (System.Uri) args[1].DataValue;
            TransferID = (System.UInt32) args[2].DataValue;
            return;
        }
        public void ExportResource(System.Uri SourceURI, System.Uri DestinationURI)
        {
            ExportResource(SourceURI, DestinationURI, null, null);
        }
        public void ExportResource(System.Uri SourceURI, System.Uri DestinationURI, object _Tag, Delegate_OnResult_ExportResource _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("SourceURI", SourceURI);
           args[1] = new UPnPArgument("DestinationURI", DestinationURI);
           args[2] = new UPnPArgument("TransferID", "");
           _S.InvokeAsync("ExportResource", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_ExportResource), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_ExportResource));
        }
        private void Sink_ExportResource(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_ExportResource)StateInfo[1])(this, (System.Uri )Args[0].DataValue, (System.Uri )Args[1].DataValue, (System.UInt32 )Args[2].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_ExportResource_Event.Fire(this, (System.Uri )Args[0].DataValue, (System.Uri )Args[1].DataValue, (System.UInt32 )Args[2].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_ExportResource(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_ExportResource)StateInfo[1])(this, (System.Uri )Args[0].DataValue, (System.Uri )Args[1].DataValue, (System.UInt32)UPnPService.CreateObjectInstance(typeof(System.UInt32),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_ExportResource_Event.Fire(this, (System.Uri )Args[0].DataValue, (System.Uri )Args[1].DataValue, (System.UInt32)UPnPService.CreateObjectInstance(typeof(System.UInt32),null), e, StateInfo[0]);
            }
        }
        public void Sync_StopTransferResource(System.UInt32 TransferID)
        {
           UPnPArgument[] args = new UPnPArgument[1];
           args[0] = new UPnPArgument("TransferID", TransferID);
            _S.InvokeSync("StopTransferResource", args);
            TransferID = (System.UInt32) args[0].DataValue;
            return;
        }
        public void StopTransferResource(System.UInt32 TransferID)
        {
            StopTransferResource(TransferID, null, null);
        }
        public void StopTransferResource(System.UInt32 TransferID, object _Tag, Delegate_OnResult_StopTransferResource _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[1];
           args[0] = new UPnPArgument("TransferID", TransferID);
           _S.InvokeAsync("StopTransferResource", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_StopTransferResource), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_StopTransferResource));
        }
        private void Sink_StopTransferResource(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_StopTransferResource)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_StopTransferResource_Event.Fire(this, (System.UInt32 )Args[0].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_StopTransferResource(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_StopTransferResource)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, e, StateInfo[0]);
            }
            else
            {
                OnResult_StopTransferResource_Event.Fire(this, (System.UInt32 )Args[0].DataValue, e, StateInfo[0]);
            }
        }
        public void Sync_DestroyObject(System.String ObjectID)
        {
           UPnPArgument[] args = new UPnPArgument[1];
           args[0] = new UPnPArgument("ObjectID", ObjectID);
            _S.InvokeSync("DestroyObject", args);
            ObjectID = (System.String) args[0].DataValue;
            return;
        }
        public void DestroyObject(System.String ObjectID)
        {
            DestroyObject(ObjectID, null, null);
        }
        public void DestroyObject(System.String ObjectID, object _Tag, Delegate_OnResult_DestroyObject _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[1];
           args[0] = new UPnPArgument("ObjectID", ObjectID);
           _S.InvokeAsync("DestroyObject", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_DestroyObject), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_DestroyObject));
        }
        private void Sink_DestroyObject(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_DestroyObject)StateInfo[1])(this, (System.String )Args[0].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_DestroyObject_Event.Fire(this, (System.String )Args[0].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_DestroyObject(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_DestroyObject)StateInfo[1])(this, (System.String )Args[0].DataValue, e, StateInfo[0]);
            }
            else
            {
                OnResult_DestroyObject_Event.Fire(this, (System.String )Args[0].DataValue, e, StateInfo[0]);
            }
        }
        public void Sync_UpdateObject(System.String ObjectID, System.String CurrentTagValue, System.String NewTagValue)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("ObjectID", ObjectID);
           args[1] = new UPnPArgument("CurrentTagValue", CurrentTagValue);
           args[2] = new UPnPArgument("NewTagValue", NewTagValue);
            _S.InvokeSync("UpdateObject", args);
            ObjectID = (System.String) args[0].DataValue;
            CurrentTagValue = (System.String) args[1].DataValue;
            NewTagValue = (System.String) args[2].DataValue;
            return;
        }
        public void UpdateObject(System.String ObjectID, System.String CurrentTagValue, System.String NewTagValue)
        {
            UpdateObject(ObjectID, CurrentTagValue, NewTagValue, null, null);
        }
        public void UpdateObject(System.String ObjectID, System.String CurrentTagValue, System.String NewTagValue, object _Tag, Delegate_OnResult_UpdateObject _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("ObjectID", ObjectID);
           args[1] = new UPnPArgument("CurrentTagValue", CurrentTagValue);
           args[2] = new UPnPArgument("NewTagValue", NewTagValue);
           _S.InvokeAsync("UpdateObject", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_UpdateObject), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_UpdateObject));
        }
        private void Sink_UpdateObject(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_UpdateObject)StateInfo[1])(this, (System.String )Args[0].DataValue, (System.String )Args[1].DataValue, (System.String )Args[2].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_UpdateObject_Event.Fire(this, (System.String )Args[0].DataValue, (System.String )Args[1].DataValue, (System.String )Args[2].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_UpdateObject(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_UpdateObject)StateInfo[1])(this, (System.String )Args[0].DataValue, (System.String )Args[1].DataValue, (System.String )Args[2].DataValue, e, StateInfo[0]);
            }
            else
            {
                OnResult_UpdateObject_Event.Fire(this, (System.String )Args[0].DataValue, (System.String )Args[1].DataValue, (System.String )Args[2].DataValue, e, StateInfo[0]);
            }
        }
        public void Sync_GetSystemUpdateID(out System.UInt32 Id)
        {
           UPnPArgument[] args = new UPnPArgument[1];
           args[0] = new UPnPArgument("Id", "");
            _S.InvokeSync("GetSystemUpdateID", args);
            Id = (System.UInt32) args[0].DataValue;
            return;
        }
        public void GetSystemUpdateID()
        {
            GetSystemUpdateID(null, null);
        }
        public void GetSystemUpdateID(object _Tag, Delegate_OnResult_GetSystemUpdateID _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[1];
           args[0] = new UPnPArgument("Id", "");
           _S.InvokeAsync("GetSystemUpdateID", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_GetSystemUpdateID), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetSystemUpdateID));
        }
        private void Sink_GetSystemUpdateID(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetSystemUpdateID)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_GetSystemUpdateID_Event.Fire(this, (System.UInt32 )Args[0].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_GetSystemUpdateID(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetSystemUpdateID)StateInfo[1])(this, (System.UInt32)UPnPService.CreateObjectInstance(typeof(System.UInt32),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_GetSystemUpdateID_Event.Fire(this, (System.UInt32)UPnPService.CreateObjectInstance(typeof(System.UInt32),null), e, StateInfo[0]);
            }
        }
        public void Sync_GetTransferProgress(System.UInt32 TransferID, out Enum_A_ARG_TYPE_TransferStatus TransferStatus, out System.String TransferLength, out System.String TransferTotal)
        {
           UPnPArgument[] args = new UPnPArgument[4];
           args[0] = new UPnPArgument("TransferID", TransferID);
           args[1] = new UPnPArgument("TransferStatus", "");
           args[2] = new UPnPArgument("TransferLength", "");
           args[3] = new UPnPArgument("TransferTotal", "");
            _S.InvokeSync("GetTransferProgress", args);
            for(int i=0;i<args.Length;++i)
            {
                switch(args[i].Name)
                {
                    case "TransferStatus":
                        switch((string)args[i].DataValue)
                        {
                            case "COMPLETED":
                                args[i].DataValue = Enum_A_ARG_TYPE_TransferStatus.COMPLETED;
                                break;
                            case "ERROR":
                                args[i].DataValue = Enum_A_ARG_TYPE_TransferStatus.ERROR;
                                break;
                            case "IN_PROGRESS":
                                args[i].DataValue = Enum_A_ARG_TYPE_TransferStatus.IN_PROGRESS;
                                break;
                            case "STOPPED":
                                args[i].DataValue = Enum_A_ARG_TYPE_TransferStatus.STOPPED;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_TransferStatus", (string)args[i].DataValue);
                               args[i].DataValue = Enum_A_ARG_TYPE_TransferStatus._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            TransferID = (System.UInt32) args[0].DataValue;
            TransferStatus = (Enum_A_ARG_TYPE_TransferStatus) args[1].DataValue;
            TransferLength = (System.String) args[2].DataValue;
            TransferTotal = (System.String) args[3].DataValue;
            return;
        }
        public void GetTransferProgress(System.UInt32 TransferID)
        {
            GetTransferProgress(TransferID, null, null);
        }
        public void GetTransferProgress(System.UInt32 TransferID, object _Tag, Delegate_OnResult_GetTransferProgress _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[4];
           args[0] = new UPnPArgument("TransferID", TransferID);
           args[1] = new UPnPArgument("TransferStatus", "");
           args[2] = new UPnPArgument("TransferLength", "");
           args[3] = new UPnPArgument("TransferTotal", "");
           _S.InvokeAsync("GetTransferProgress", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_GetTransferProgress), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetTransferProgress));
        }
        private void Sink_GetTransferProgress(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "TransferStatus":
                        switch((string)Args[i].DataValue)
                        {
                            case "COMPLETED":
                                Args[i].DataValue = Enum_A_ARG_TYPE_TransferStatus.COMPLETED;
                                break;
                            case "ERROR":
                                Args[i].DataValue = Enum_A_ARG_TYPE_TransferStatus.ERROR;
                                break;
                            case "IN_PROGRESS":
                                Args[i].DataValue = Enum_A_ARG_TYPE_TransferStatus.IN_PROGRESS;
                                break;
                            case "STOPPED":
                                Args[i].DataValue = Enum_A_ARG_TYPE_TransferStatus.STOPPED;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_TransferStatus", (string)Args[i].DataValue);
                               Args[i].DataValue = Enum_A_ARG_TYPE_TransferStatus._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetTransferProgress)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_TransferStatus )Args[1].DataValue, (System.String )Args[2].DataValue, (System.String )Args[3].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_GetTransferProgress_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_TransferStatus )Args[1].DataValue, (System.String )Args[2].DataValue, (System.String )Args[3].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_GetTransferProgress(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetTransferProgress)StateInfo[1])(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_TransferStatus)0, (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_GetTransferProgress_Event.Fire(this, (System.UInt32 )Args[0].DataValue, (Enum_A_ARG_TYPE_TransferStatus)0, (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), e, StateInfo[0]);
            }
        }
        public void Sync_GetSearchCapabilities(out System.String SearchCaps)
        {
           UPnPArgument[] args = new UPnPArgument[1];
           args[0] = new UPnPArgument("SearchCaps", "");
            _S.InvokeSync("GetSearchCapabilities", args);
            SearchCaps = (System.String) args[0].DataValue;
            return;
        }
        public void GetSearchCapabilities()
        {
            GetSearchCapabilities(null, null);
        }
        public void GetSearchCapabilities(object _Tag, Delegate_OnResult_GetSearchCapabilities _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[1];
           args[0] = new UPnPArgument("SearchCaps", "");
           _S.InvokeAsync("GetSearchCapabilities", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_GetSearchCapabilities), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetSearchCapabilities));
        }
        private void Sink_GetSearchCapabilities(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetSearchCapabilities)StateInfo[1])(this, (System.String )Args[0].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_GetSearchCapabilities_Event.Fire(this, (System.String )Args[0].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_GetSearchCapabilities(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetSearchCapabilities)StateInfo[1])(this, (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_GetSearchCapabilities_Event.Fire(this, (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), e, StateInfo[0]);
            }
        }
        public void Sync_CreateObject(System.String ContainerID, System.String Elements, out System.String ObjectID, out System.String Result)
        {
           UPnPArgument[] args = new UPnPArgument[4];
           args[0] = new UPnPArgument("ContainerID", ContainerID);
           args[1] = new UPnPArgument("Elements", Elements);
           args[2] = new UPnPArgument("ObjectID", "");
           args[3] = new UPnPArgument("Result", "");
            _S.InvokeSync("CreateObject", args);
            ContainerID = (System.String) args[0].DataValue;
            Elements = (System.String) args[1].DataValue;
            ObjectID = (System.String) args[2].DataValue;
            Result = (System.String) args[3].DataValue;
            return;
        }
        public void CreateObject(System.String ContainerID, System.String Elements)
        {
            CreateObject(ContainerID, Elements, null, null);
        }
        public void CreateObject(System.String ContainerID, System.String Elements, object _Tag, Delegate_OnResult_CreateObject _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[4];
           args[0] = new UPnPArgument("ContainerID", ContainerID);
           args[1] = new UPnPArgument("Elements", Elements);
           args[2] = new UPnPArgument("ObjectID", "");
           args[3] = new UPnPArgument("Result", "");
           _S.InvokeAsync("CreateObject", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_CreateObject), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_CreateObject));
        }
        private void Sink_CreateObject(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_CreateObject)StateInfo[1])(this, (System.String )Args[0].DataValue, (System.String )Args[1].DataValue, (System.String )Args[2].DataValue, (System.String )Args[3].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_CreateObject_Event.Fire(this, (System.String )Args[0].DataValue, (System.String )Args[1].DataValue, (System.String )Args[2].DataValue, (System.String )Args[3].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_CreateObject(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_CreateObject)StateInfo[1])(this, (System.String )Args[0].DataValue, (System.String )Args[1].DataValue, (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_CreateObject_Event.Fire(this, (System.String )Args[0].DataValue, (System.String )Args[1].DataValue, (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), e, StateInfo[0]);
            }
        }
        public void Sync_ImportResource(System.Uri SourceURI, System.Uri DestinationURI, out System.UInt32 TransferID)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("SourceURI", SourceURI);
           args[1] = new UPnPArgument("DestinationURI", DestinationURI);
           args[2] = new UPnPArgument("TransferID", "");
            _S.InvokeSync("ImportResource", args);
            SourceURI = (System.Uri) args[0].DataValue;
            DestinationURI = (System.Uri) args[1].DataValue;
            TransferID = (System.UInt32) args[2].DataValue;
            return;
        }
        public void ImportResource(System.Uri SourceURI, System.Uri DestinationURI)
        {
            ImportResource(SourceURI, DestinationURI, null, null);
        }
        public void ImportResource(System.Uri SourceURI, System.Uri DestinationURI, object _Tag, Delegate_OnResult_ImportResource _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("SourceURI", SourceURI);
           args[1] = new UPnPArgument("DestinationURI", DestinationURI);
           args[2] = new UPnPArgument("TransferID", "");
           _S.InvokeAsync("ImportResource", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_ImportResource), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_ImportResource));
        }
        private void Sink_ImportResource(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_ImportResource)StateInfo[1])(this, (System.Uri )Args[0].DataValue, (System.Uri )Args[1].DataValue, (System.UInt32 )Args[2].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_ImportResource_Event.Fire(this, (System.Uri )Args[0].DataValue, (System.Uri )Args[1].DataValue, (System.UInt32 )Args[2].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_ImportResource(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_ImportResource)StateInfo[1])(this, (System.Uri )Args[0].DataValue, (System.Uri )Args[1].DataValue, (System.UInt32)UPnPService.CreateObjectInstance(typeof(System.UInt32),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_ImportResource_Event.Fire(this, (System.Uri )Args[0].DataValue, (System.Uri )Args[1].DataValue, (System.UInt32)UPnPService.CreateObjectInstance(typeof(System.UInt32),null), e, StateInfo[0]);
            }
        }
        public void Sync_Search(System.String ContainerID, System.String SearchCriteria, System.String Filter, System.UInt32 StartingIndex, System.UInt32 RequestedCount, System.String SortCriteria, out System.String Result, out System.UInt32 NumberReturned, out System.UInt32 TotalMatches, out System.UInt32 UpdateID)
        {
           UPnPArgument[] args = new UPnPArgument[10];
           args[0] = new UPnPArgument("ContainerID", ContainerID);
           args[1] = new UPnPArgument("SearchCriteria", SearchCriteria);
           args[2] = new UPnPArgument("Filter", Filter);
           args[3] = new UPnPArgument("StartingIndex", StartingIndex);
           args[4] = new UPnPArgument("RequestedCount", RequestedCount);
           args[5] = new UPnPArgument("SortCriteria", SortCriteria);
           args[6] = new UPnPArgument("Result", "");
           args[7] = new UPnPArgument("NumberReturned", "");
           args[8] = new UPnPArgument("TotalMatches", "");
           args[9] = new UPnPArgument("UpdateID", "");
            _S.InvokeSync("Search", args);
            ContainerID = (System.String) args[0].DataValue;
            SearchCriteria = (System.String) args[1].DataValue;
            Filter = (System.String) args[2].DataValue;
            StartingIndex = (System.UInt32) args[3].DataValue;
            RequestedCount = (System.UInt32) args[4].DataValue;
            SortCriteria = (System.String) args[5].DataValue;
            Result = (System.String) args[6].DataValue;
            NumberReturned = (System.UInt32) args[7].DataValue;
            TotalMatches = (System.UInt32) args[8].DataValue;
            UpdateID = (System.UInt32) args[9].DataValue;
            return;
        }
        public void Search(System.String ContainerID, System.String SearchCriteria, System.String Filter, System.UInt32 StartingIndex, System.UInt32 RequestedCount, System.String SortCriteria)
        {
            Search(ContainerID, SearchCriteria, Filter, StartingIndex, RequestedCount, SortCriteria, null, null);
        }
        public void Search(System.String ContainerID, System.String SearchCriteria, System.String Filter, System.UInt32 StartingIndex, System.UInt32 RequestedCount, System.String SortCriteria, object _Tag, Delegate_OnResult_Search _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[10];
           args[0] = new UPnPArgument("ContainerID", ContainerID);
           args[1] = new UPnPArgument("SearchCriteria", SearchCriteria);
           args[2] = new UPnPArgument("Filter", Filter);
           args[3] = new UPnPArgument("StartingIndex", StartingIndex);
           args[4] = new UPnPArgument("RequestedCount", RequestedCount);
           args[5] = new UPnPArgument("SortCriteria", SortCriteria);
           args[6] = new UPnPArgument("Result", "");
           args[7] = new UPnPArgument("NumberReturned", "");
           args[8] = new UPnPArgument("TotalMatches", "");
           args[9] = new UPnPArgument("UpdateID", "");
           _S.InvokeAsync("Search", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_Search), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_Search));
        }
        private void Sink_Search(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_Search)StateInfo[1])(this, (System.String )Args[0].DataValue, (System.String )Args[1].DataValue, (System.String )Args[2].DataValue, (System.UInt32 )Args[3].DataValue, (System.UInt32 )Args[4].DataValue, (System.String )Args[5].DataValue, (System.String )Args[6].DataValue, (System.UInt32 )Args[7].DataValue, (System.UInt32 )Args[8].DataValue, (System.UInt32 )Args[9].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_Search_Event.Fire(this, (System.String )Args[0].DataValue, (System.String )Args[1].DataValue, (System.String )Args[2].DataValue, (System.UInt32 )Args[3].DataValue, (System.UInt32 )Args[4].DataValue, (System.String )Args[5].DataValue, (System.String )Args[6].DataValue, (System.UInt32 )Args[7].DataValue, (System.UInt32 )Args[8].DataValue, (System.UInt32 )Args[9].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_Search(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_Search)StateInfo[1])(this, (System.String )Args[0].DataValue, (System.String )Args[1].DataValue, (System.String )Args[2].DataValue, (System.UInt32 )Args[3].DataValue, (System.UInt32 )Args[4].DataValue, (System.String )Args[5].DataValue, (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.UInt32)UPnPService.CreateObjectInstance(typeof(System.UInt32),null), (System.UInt32)UPnPService.CreateObjectInstance(typeof(System.UInt32),null), (System.UInt32)UPnPService.CreateObjectInstance(typeof(System.UInt32),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_Search_Event.Fire(this, (System.String )Args[0].DataValue, (System.String )Args[1].DataValue, (System.String )Args[2].DataValue, (System.UInt32 )Args[3].DataValue, (System.UInt32 )Args[4].DataValue, (System.String )Args[5].DataValue, (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.UInt32)UPnPService.CreateObjectInstance(typeof(System.UInt32),null), (System.UInt32)UPnPService.CreateObjectInstance(typeof(System.UInt32),null), (System.UInt32)UPnPService.CreateObjectInstance(typeof(System.UInt32),null), e, StateInfo[0]);
            }
        }
        public void Sync_GetSortCapabilities(out System.String SortCaps)
        {
           UPnPArgument[] args = new UPnPArgument[1];
           args[0] = new UPnPArgument("SortCaps", "");
            _S.InvokeSync("GetSortCapabilities", args);
            SortCaps = (System.String) args[0].DataValue;
            return;
        }
        public void GetSortCapabilities()
        {
            GetSortCapabilities(null, null);
        }
        public void GetSortCapabilities(object _Tag, Delegate_OnResult_GetSortCapabilities _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[1];
           args[0] = new UPnPArgument("SortCaps", "");
           _S.InvokeAsync("GetSortCapabilities", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_GetSortCapabilities), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_GetSortCapabilities));
        }
        private void Sink_GetSortCapabilities(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetSortCapabilities)StateInfo[1])(this, (System.String )Args[0].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_GetSortCapabilities_Event.Fire(this, (System.String )Args[0].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_GetSortCapabilities(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_GetSortCapabilities)StateInfo[1])(this, (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_GetSortCapabilities_Event.Fire(this, (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), e, StateInfo[0]);
            }
        }
        public void Sync_Browse(System.String ObjectID, Enum_A_ARG_TYPE_BrowseFlag BrowseFlag, System.String Filter, System.UInt32 StartingIndex, System.UInt32 RequestedCount, System.String SortCriteria, out System.String Result, out System.UInt32 NumberReturned, out System.UInt32 TotalMatches, out System.UInt32 UpdateID)
        {
           UPnPArgument[] args = new UPnPArgument[10];
           args[0] = new UPnPArgument("ObjectID", ObjectID);
           switch(BrowseFlag)
           {
               case Enum_A_ARG_TYPE_BrowseFlag.BROWSEMETADATA:
                   args[1] = new UPnPArgument("BrowseFlag", "BrowseMetadata");
                   break;
               case Enum_A_ARG_TYPE_BrowseFlag.BROWSEDIRECTCHILDREN:
                   args[1] = new UPnPArgument("BrowseFlag", "BrowseDirectChildren");
                   break;
               default:
                  args[1] = new UPnPArgument("BrowseFlag", GetUnspecifiedValue("Enum_A_ARG_TYPE_BrowseFlag"));
                  break;
           }
           args[2] = new UPnPArgument("Filter", Filter);
           args[3] = new UPnPArgument("StartingIndex", StartingIndex);
           args[4] = new UPnPArgument("RequestedCount", RequestedCount);
           args[5] = new UPnPArgument("SortCriteria", SortCriteria);
           args[6] = new UPnPArgument("Result", "");
           args[7] = new UPnPArgument("NumberReturned", "");
           args[8] = new UPnPArgument("TotalMatches", "");
           args[9] = new UPnPArgument("UpdateID", "");
            _S.InvokeSync("Browse", args);
            for(int i=0;i<args.Length;++i)
            {
                switch(args[i].Name)
                {
                    case "BrowseFlag":
                        switch((string)args[i].DataValue)
                        {
                            case "BrowseMetadata":
                                args[i].DataValue = Enum_A_ARG_TYPE_BrowseFlag.BROWSEMETADATA;
                                break;
                            case "BrowseDirectChildren":
                                args[i].DataValue = Enum_A_ARG_TYPE_BrowseFlag.BROWSEDIRECTCHILDREN;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_BrowseFlag", (string)args[i].DataValue);
                               args[i].DataValue = Enum_A_ARG_TYPE_BrowseFlag._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            ObjectID = (System.String) args[0].DataValue;
            BrowseFlag = (Enum_A_ARG_TYPE_BrowseFlag) args[1].DataValue;
            Filter = (System.String) args[2].DataValue;
            StartingIndex = (System.UInt32) args[3].DataValue;
            RequestedCount = (System.UInt32) args[4].DataValue;
            SortCriteria = (System.String) args[5].DataValue;
            Result = (System.String) args[6].DataValue;
            NumberReturned = (System.UInt32) args[7].DataValue;
            TotalMatches = (System.UInt32) args[8].DataValue;
            UpdateID = (System.UInt32) args[9].DataValue;
            return;
        }
        public void Browse(System.String ObjectID, Enum_A_ARG_TYPE_BrowseFlag BrowseFlag, System.String Filter, System.UInt32 StartingIndex, System.UInt32 RequestedCount, System.String SortCriteria)
        {
            Browse(ObjectID, BrowseFlag, Filter, StartingIndex, RequestedCount, SortCriteria, null, null);
        }
        public void Browse(System.String ObjectID, Enum_A_ARG_TYPE_BrowseFlag BrowseFlag, System.String Filter, System.UInt32 StartingIndex, System.UInt32 RequestedCount, System.String SortCriteria, object _Tag, Delegate_OnResult_Browse _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[10];
           args[0] = new UPnPArgument("ObjectID", ObjectID);
           switch(BrowseFlag)
           {
               case Enum_A_ARG_TYPE_BrowseFlag.BROWSEMETADATA:
                   args[1] = new UPnPArgument("BrowseFlag", "BrowseMetadata");
                   break;
               case Enum_A_ARG_TYPE_BrowseFlag.BROWSEDIRECTCHILDREN:
                   args[1] = new UPnPArgument("BrowseFlag", "BrowseDirectChildren");
                   break;
               default:
                  args[1] = new UPnPArgument("BrowseFlag", GetUnspecifiedValue("Enum_A_ARG_TYPE_BrowseFlag"));
                  break;
           }
           args[2] = new UPnPArgument("Filter", Filter);
           args[3] = new UPnPArgument("StartingIndex", StartingIndex);
           args[4] = new UPnPArgument("RequestedCount", RequestedCount);
           args[5] = new UPnPArgument("SortCriteria", SortCriteria);
           args[6] = new UPnPArgument("Result", "");
           args[7] = new UPnPArgument("NumberReturned", "");
           args[8] = new UPnPArgument("TotalMatches", "");
           args[9] = new UPnPArgument("UpdateID", "");
           _S.InvokeAsync("Browse", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_Browse), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_Browse));
        }
        private void Sink_Browse(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "BrowseFlag":
                        switch((string)Args[i].DataValue)
                        {
                            case "BrowseMetadata":
                                Args[i].DataValue = Enum_A_ARG_TYPE_BrowseFlag.BROWSEMETADATA;
                                break;
                            case "BrowseDirectChildren":
                                Args[i].DataValue = Enum_A_ARG_TYPE_BrowseFlag.BROWSEDIRECTCHILDREN;
                                break;
                            default:
                               SetUnspecifiedValue("Enum_A_ARG_TYPE_BrowseFlag", (string)Args[i].DataValue);
                               Args[i].DataValue = Enum_A_ARG_TYPE_BrowseFlag._UNSPECIFIED_;
                               break;
                        }
                        break;
                }
            }
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_Browse)StateInfo[1])(this, (System.String )Args[0].DataValue, (Enum_A_ARG_TYPE_BrowseFlag )Args[1].DataValue, (System.String )Args[2].DataValue, (System.UInt32 )Args[3].DataValue, (System.UInt32 )Args[4].DataValue, (System.String )Args[5].DataValue, (System.String )Args[6].DataValue, (System.UInt32 )Args[7].DataValue, (System.UInt32 )Args[8].DataValue, (System.UInt32 )Args[9].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_Browse_Event.Fire(this, (System.String )Args[0].DataValue, (Enum_A_ARG_TYPE_BrowseFlag )Args[1].DataValue, (System.String )Args[2].DataValue, (System.UInt32 )Args[3].DataValue, (System.UInt32 )Args[4].DataValue, (System.String )Args[5].DataValue, (System.String )Args[6].DataValue, (System.UInt32 )Args[7].DataValue, (System.UInt32 )Args[8].DataValue, (System.UInt32 )Args[9].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_Browse(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            for(int i=0;i<Args.Length;++i)
            {
                switch(Args[i].Name)
                {
                    case "BrowseFlag":
                        switch((string)Args[i].DataValue)
                        {
                            case "BrowseMetadata":
                                Args[i].DataValue = Enum_A_ARG_TYPE_BrowseFlag.BROWSEMETADATA;
                                break;
                            case "BrowseDirectChildren":
                                Args[i].DataValue = Enum_A_ARG_TYPE_BrowseFlag.BROWSEDIRECTCHILDREN;
                                break;
                        }
                        break;
                }
            }
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_Browse)StateInfo[1])(this, (System.String )Args[0].DataValue, (Enum_A_ARG_TYPE_BrowseFlag )Args[1].DataValue, (System.String )Args[2].DataValue, (System.UInt32 )Args[3].DataValue, (System.UInt32 )Args[4].DataValue, (System.String )Args[5].DataValue, (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.UInt32)UPnPService.CreateObjectInstance(typeof(System.UInt32),null), (System.UInt32)UPnPService.CreateObjectInstance(typeof(System.UInt32),null), (System.UInt32)UPnPService.CreateObjectInstance(typeof(System.UInt32),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_Browse_Event.Fire(this, (System.String )Args[0].DataValue, (Enum_A_ARG_TYPE_BrowseFlag )Args[1].DataValue, (System.String )Args[2].DataValue, (System.UInt32 )Args[3].DataValue, (System.UInt32 )Args[4].DataValue, (System.String )Args[5].DataValue, (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), (System.UInt32)UPnPService.CreateObjectInstance(typeof(System.UInt32),null), (System.UInt32)UPnPService.CreateObjectInstance(typeof(System.UInt32),null), (System.UInt32)UPnPService.CreateObjectInstance(typeof(System.UInt32),null), e, StateInfo[0]);
            }
        }
        public void Sync_CreateReference(System.String ContainerID, System.String ObjectID, out System.String NewID)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("ContainerID", ContainerID);
           args[1] = new UPnPArgument("ObjectID", ObjectID);
           args[2] = new UPnPArgument("NewID", "");
            _S.InvokeSync("CreateReference", args);
            ContainerID = (System.String) args[0].DataValue;
            ObjectID = (System.String) args[1].DataValue;
            NewID = (System.String) args[2].DataValue;
            return;
        }
        public void CreateReference(System.String ContainerID, System.String ObjectID)
        {
            CreateReference(ContainerID, ObjectID, null, null);
        }
        public void CreateReference(System.String ContainerID, System.String ObjectID, object _Tag, Delegate_OnResult_CreateReference _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[3];
           args[0] = new UPnPArgument("ContainerID", ContainerID);
           args[1] = new UPnPArgument("ObjectID", ObjectID);
           args[2] = new UPnPArgument("NewID", "");
           _S.InvokeAsync("CreateReference", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_CreateReference), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_CreateReference));
        }
        private void Sink_CreateReference(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_CreateReference)StateInfo[1])(this, (System.String )Args[0].DataValue, (System.String )Args[1].DataValue, (System.String )Args[2].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_CreateReference_Event.Fire(this, (System.String )Args[0].DataValue, (System.String )Args[1].DataValue, (System.String )Args[2].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_CreateReference(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_CreateReference)StateInfo[1])(this, (System.String )Args[0].DataValue, (System.String )Args[1].DataValue, (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), e, StateInfo[0]);
            }
            else
            {
                OnResult_CreateReference_Event.Fire(this, (System.String )Args[0].DataValue, (System.String )Args[1].DataValue, (System.String)UPnPService.CreateObjectInstance(typeof(System.String),null), e, StateInfo[0]);
            }
        }
        public void Sync_DeleteResource(System.Uri ResourceURI)
        {
           UPnPArgument[] args = new UPnPArgument[1];
           args[0] = new UPnPArgument("ResourceURI", ResourceURI);
            _S.InvokeSync("DeleteResource", args);
            ResourceURI = (System.Uri) args[0].DataValue;
            return;
        }
        public void DeleteResource(System.Uri ResourceURI)
        {
            DeleteResource(ResourceURI, null, null);
        }
        public void DeleteResource(System.Uri ResourceURI, object _Tag, Delegate_OnResult_DeleteResource _Callback)
        {
           UPnPArgument[] args = new UPnPArgument[1];
           args[0] = new UPnPArgument("ResourceURI", ResourceURI);
           _S.InvokeAsync("DeleteResource", args, new object[2]{_Tag,_Callback},new UPnPService.UPnPServiceInvokeHandler(Sink_DeleteResource), new UPnPService.UPnPServiceInvokeErrorHandler(Error_Sink_DeleteResource));
        }
        private void Sink_DeleteResource(UPnPService sender, string MethodName, UPnPArgument[] Args, object RetVal, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_DeleteResource)StateInfo[1])(this, (System.Uri )Args[0].DataValue, null, StateInfo[0]);
            }
            else
            {
                OnResult_DeleteResource_Event.Fire(this, (System.Uri )Args[0].DataValue, null, StateInfo[0]);
            }
        }
        private void Error_Sink_DeleteResource(UPnPService sender, string MethodName, UPnPArgument[] Args, UPnPInvokeException e, object _Tag)
        {
            object[] StateInfo = (object[])_Tag;
            if(StateInfo[1]!=null)
            {
                ((Delegate_OnResult_DeleteResource)StateInfo[1])(this, (System.Uri )Args[0].DataValue, e, StateInfo[0]);
            }
            else
            {
                OnResult_DeleteResource_Event.Fire(this, (System.Uri )Args[0].DataValue, e, StateInfo[0]);
            }
        }
    }
}