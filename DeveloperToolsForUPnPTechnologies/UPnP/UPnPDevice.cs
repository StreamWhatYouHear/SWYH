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
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

namespace OpenSource.UPnP
{
    /// <summary>
    /// An interface that is used to encapsulate a device implementation
    /// </summary>
    public interface IUPnPDevice
    {
        /// <summary>
        /// Gets the underlying UPnPDevice
        /// </summary>
        UPnPDevice GetUPnPDevice();
    }
    public sealed class UPnPDeviceComparer_Type : IComparer
    {
        public int Compare(object x, object y)
        {
            // TODO:  Add UPnPDeviceComparer_Type.Compare implementation
            UPnPDevice x1 = (UPnPDevice)x;
            UPnPDevice x2 = (UPnPDevice)y;

            return (string.Compare(x1.DeviceURN, x2.DeviceURN));
        }
    }


    /// <summary>
    /// The container to hold UPnPDevices and UPnPServices.
    /// </summary>
    /// 
    public sealed class UPnPDevice
    {
        public object User = null;
        public object User2 = null;
        public object User3 = null;
        public object Reserved = null;

        private string _BootID = "";
        private int Arch_Major = 1;
        private int Arch_Minor = 0;

        private Hashtable CP_RegisteredInterfaces = new Hashtable();

        private Hashtable UpdateTable = new Hashtable();
        private Hashtable CustomField = new Hashtable();

        internal bool NoSSDP = false;

        private int UseThisPort = 0;
        private Hashtable InitialEventTable = Hashtable.Synchronized(new Hashtable());
        public delegate void OnRemovedHandler(UPnPDevice sender);
        public event OnRemovedHandler OnRemoved;

        public string BootID
        {
            set
            {
                _BootID = value;
            }
        }
        internal struct InvokerInfoStruct
        {
            public HTTPSession WebSession;
            public UPnPArgument[] OutArgs;
            public UPnPArgument RetArg;
            public string MethodName;
            public string SOAPAction;
        }
        public class FrindlyNameComparer : IComparer
        {
            public int Compare(object o1, object o2)
            {
                UPnPDevice d1 = (UPnPDevice)o1;
                UPnPDevice d2 = (UPnPDevice)o2;
                return String.Compare(d1.FriendlyName, d2.FriendlyName);
            }
        }

        private IPEndPoint ManualIPEndPoint = null;
        internal delegate void SniffHandler(byte[] Raw, int offset, int length);
        internal event SniffHandler OnSniff;
        internal delegate void SniffPacketHandler(HTTPMessage Packet);
        internal event SniffPacketHandler OnSniffPacket;

        public delegate void VirtualDirectoryHandler(UPnPDevice sender, HTTPMessage msg, HTTPSession WebSession, string VirtualDir);
        private Hashtable VirtualDir_Table;
        private Hashtable VirtualDir_Header_Table;

        public delegate void DeviceUpdateHandler(UPnPDevice device);

        internal Uri descXmlLocation = null;
        private NetworkInfo NetInfo;

        private SSDP SSDPServer;
        private Hashtable WebServerTable;
        internal Hashtable InvokerInfo = Hashtable.Synchronized(new Hashtable());
        private bool IsRoot = false;

        private System.Drawing.Image _icon = null;
        private System.Drawing.Image _icon2 = null;
        // Note: Cannot be created dynamically from the icon property; http://www.dotnet247.com/247reference/msgs/49/248277.aspx
        private System.Drawing.Icon _favicon = null;

        static private Hashtable CPWebServerTable;
        static private long DeviceCount = 0;

        static private NetworkInfo CPNetworkInfo;

        public String ProprietaryDeviceType;
        public IPAddress InterfaceToHost;
        private String RootPath;
        private bool ControlPointOnly;
        private string __DeviceURN;

        public String DeviceURN
        {
            get
            {
                return __DeviceURN;
            }
            set
            {
                __DeviceURN = value;
                DText p = new DText();
                p.ATTRMARK = ":";
                p[0] = value;

                if (int.Parse(Version) > 0 && Version != "1" && p[p.DCOUNT()] == "1")
                {
                    p[p.DCOUNT()] = Version;
                    __DeviceURN = p[0];
                }
                else
                {
                    SetVersion(p[p.DCOUNT()]);
                }
            }
        }
        public string DeviceURN_Prefix
        {
            get
            {
                int len;
                DText p = new DText();
                p.ATTRMARK = ":";
                p[0] = __DeviceURN;
                len = p[p.DCOUNT()].Length;
                return (__DeviceURN.Substring(0, __DeviceURN.Length - len));
            }
        }
        /// <summary>
        /// bool that indicates if a presentation page exists
        /// </summary>
        public bool HasPresentation;
        public Uri BaseURL;
        public String LocationURL;
        public String FriendlyName;
        public String Manufacturer;
        public String ManufacturerURL;
        public String ModelDescription;
        public String ModelName;
        public String ModelNumber;
        public Uri ModelURL;
        public String SerialNumber;
        public String ProductCode;
        public String UniqueDeviceName;

        public int Major;
        public int Minor;
        public int ExpirationTimeout;

        private UPnPDevice parent;

        private String _PresentationURL;
        /// <summary>
        /// Array of UPnPServices contained in this device
        /// </summary>
        public UPnPService[] Services;
        /// <summary>
        /// Array of UPnPDevices contained in this device
        /// </summary>
        public UPnPDevice[] EmbeddedDevices = new UPnPDevice[0];
        /// <summary>
        /// bool that indicates if this is a RootDevice
        /// </summary>
        public bool Root
        {
            get { return IsRoot; }
        }

        public void ClearCustomFieldsInDescription()
        {
            CustomField.Clear();
        }
        public void AddCustomFieldInDescription(string FieldName, string FieldValue, string Namespace)
        {
            if (CustomField.ContainsKey(Namespace) == false)
            {
                CustomField[Namespace] = new Hashtable();
            }
            ((Hashtable)CustomField[Namespace])[FieldName] = FieldValue;
        }
        public string GetCustomFieldFromDescription(string FieldName, string Namespace)
        {
            if (CustomField.ContainsKey(Namespace) && ((Hashtable)CustomField[Namespace]).ContainsKey(FieldName))
            {
                return ((string)((Hashtable)CustomField[Namespace])[FieldName]);
            }
            else
            {
                return null;
            }
        }
        public List<string> GetCustomFieldFromDescription_Namespaces()
        {
            List<string> retVal = new List<string>();
            foreach (object key in CustomField.Keys)
            {
                retVal.Add(key.ToString());
            }
            return (retVal);
        }
        public List<KeyValuePair<string, string>> GetCustomFieldsFromDescription(string Namespace)
        {
            List<KeyValuePair<string, string>> retVal = new List<KeyValuePair<string, string>>();

            if (CustomField.ContainsKey(Namespace))
            {
                IDictionaryEnumerator e = ((Hashtable)CustomField[Namespace]).GetEnumerator();
                while (e.MoveNext())
                {
                    retVal.Add(new KeyValuePair<string, string>(e.Key.ToString(), e.Value.ToString()));
                }
            }

            return (retVal);
        }
        /// <summary>
        /// Pointer to the parent device, if this is a non-root device
        /// </summary>
        public UPnPDevice ParentDevice
        {
            get
            {
                return parent;
            }
        }
        /// <summary>
        /// Reference to the icon for this device
        /// </summary>
        public System.Drawing.Image Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                if (this.ControlPointOnly == false) _icon = value;
            }
        }
        public System.Drawing.Image Icon2
        {
            get
            {
                return _icon2;
            }
            set
            {
                if (this.ControlPointOnly == false) _icon2 = value;
            }
        }

        // Favicon. Will not be read from remote device
        public System.Drawing.Icon favicon
        {
            get
            {
                return _favicon;
            }
            set
            {
                if (this.ControlPointOnly == false) _favicon = value;
            }
        }

        internal void Removed()
        {
            // Clean up internal references, to aid Garbage Collector
            IPAddress[] alist = CPNetworkInfo.GetLocalAddresses();
            for (int i = 0; i < alist.Length; ++i)
            {
                CP_RegisteredInterfaces[alist[i].ToString()] = true;
                ((MiniWebServer)CPWebServerTable[alist[i].ToString()]).OnReceive -= new MiniWebServer.HTTPReceiveHandler(HandleWebRequest);
                ((MiniWebServer)CPWebServerTable[alist[i].ToString()]).OnHeader -= new MiniWebServer.HTTPReceiveHandler(HandleHeaderRequest);
            }
            if (OnRemoved != null) OnRemoved(this);
        }

        /// <summary>
        /// Instantiate a "root" Device
        /// </summary>
        /// <param name="DeviceExpiration">Refresh Cycle</param>
        /// <param name="version">Major.Minor</param>
        /// <param name="RootDir">Root path for use by the WebServer to serve misc. docs</param>
        /// <returns>UPnPDevice Instance</returns>
        public static UPnPDevice CreateRootDevice(int DeviceExpiration, double version, String RootDir)
        {
            return new UPnPDevice(DeviceExpiration, version, RootDir);
        }
        /// <summary>
        /// Instantiate an "embedded" Device
        /// </summary>
        /// <param name="version">Major.Minor</param>
        /// <param name="UDN">Globally Unique Identifier</param>
        /// <returns>UPnPDevice Instance</returns>
        public static UPnPDevice CreateEmbeddedDevice(double version, String UDN)
        {
            return new UPnPDevice(version, UDN);
        }

        /// <summary>
        /// Constructs a UPnPDevice for use as a control point
        /// </summary>
        internal UPnPDevice()
        {
            OpenSource.Utilities.InstanceTracker.Add(this);

            // Control Point Only
            parent = null;
            ControlPointOnly = true;
            Services = new UPnPService[0];
            HasPresentation = false;

            VirtualDir_Table = new Hashtable();
            VirtualDir_Header_Table = new Hashtable();

            lock (typeof(UPnPDevice))
            {
                if (DeviceCount == 0)
                {
                    CPWebServerTable = new Hashtable();
                    CPNetworkInfo = new NetworkInfo(new NetworkInfo.InterfaceHandler(NewCPInterface));
                }
                else
                {
                    IPAddress[] alist = CPNetworkInfo.GetLocalAddresses();
                    for (int i = 0; i < alist.Length; ++i)
                    {
                        CP_RegisteredInterfaces[alist[i].ToString()] = true;
                        ((MiniWebServer)CPWebServerTable[alist[i].ToString()]).OnReceive += new MiniWebServer.HTTPReceiveHandler(HandleWebRequest);
                        ((MiniWebServer)CPWebServerTable[alist[i].ToString()]).OnHeader += new MiniWebServer.HTTPReceiveHandler(HandleHeaderRequest);
                    }
                    CPNetworkInfo.OnNewInterface += new NetworkInfo.InterfaceHandler(NewCPInterface);
                }
                ++DeviceCount;
            }

            SSDPServer = new SSDP(ExpirationTimeout);
            SSDPServer.OnRefresh += new SSDP.RefreshHandler(SendNotify);
            SSDPServer.OnSearch += new SSDP.SearchHandler(HandleSearch);
        }

        internal UPnPDevice(double version, String UDN)
        {
            OpenSource.Utilities.InstanceTracker.Add(this);
            // Full Device
            IsRoot = false;

            VirtualDir_Table = new Hashtable();
            VirtualDir_Header_Table = new Hashtable();

            parent = null;
            HasPresentation = true;
            ControlPointOnly = false;
            RootPath = "";

            if (version == 0)
            {
                Major = 1;
                Minor = 0;
            }
            else
            {
                DText TempNum = new DText();
                TempNum.ATTRMARK = ".";
                TempNum[0] = version.ToString();

                Major = int.Parse(TempNum[1]);
                if (TempNum.DCOUNT() == 2)
                {
                    Minor = int.Parse(TempNum[2]);
                }
                else
                {
                    Minor = 0;
                }
            }
            Services = new UPnPService[0];
            if (UDN == "")
            {
                UniqueDeviceName = Guid.NewGuid().ToString();
            }
            else
            {
                UniqueDeviceName = UDN;
            }

            SSDPServer = new SSDP(ExpirationTimeout);
            SSDPServer.OnRefresh += new SSDP.RefreshHandler(SendNotify);
            SSDPServer.OnSearch += new SSDP.SearchHandler(HandleSearch);
        }

        internal UPnPDevice(int DeviceExpiration, double version, String RootDir)
        {
            OpenSource.Utilities.InstanceTracker.Add(this);
            // Full Device
            IsRoot = true;

            parent = null;
            HasPresentation = true;
            ControlPointOnly = false;
            RootPath = RootDir;
            ExpirationTimeout = DeviceExpiration;
            WebServerTable = Hashtable.Synchronized(new Hashtable());

            VirtualDir_Table = new Hashtable();
            VirtualDir_Header_Table = new Hashtable();

            if (version == 0)
            {
                Major = 1;
                Minor = 0;
            }
            else
            {
                DText TempNum = new DText();
                TempNum.ATTRMARK = ".";
                TempNum[0] = version.ToString();

                Major = int.Parse(TempNum[1]);
                if (TempNum.DCOUNT() == 2)
                {
                    Minor = int.Parse(TempNum[2]);
                }
                else
                {
                    Minor = 0;
                }
            }
            Services = new UPnPService[0];
            UniqueDeviceName = Guid.NewGuid().ToString();

            SSDPServer = new SSDP(ExpirationTimeout);
            SSDPServer.OnRefresh += new SSDP.RefreshHandler(SendNotify);
            SSDPServer.OnSearch += new SSDP.SearchHandler(HandleSearch);
        }


        /// <summary>
        /// Recursively searches the device heirarchy and returns an array of UPnPDevices that match the urn
        /// </summary>
        /// <param name="urn">The URN</param>
        /// <returns>Array of UPnPDevices</returns>
        public UPnPDevice[] GetDevices(string urn)
        {
            ArrayList t = new ArrayList();
            if (this.DeviceURN.ToLower().StartsWith(urn.ToLower()) == true)
            {
                t.Add(this);
            }
            foreach (UPnPDevice d in EmbeddedDevices)
            {
                if (d.DeviceURN.ToLower().StartsWith(urn.ToLower()) == true)
                {
                    t.Add(d);
                }
            }
            return ((UPnPDevice[])t.ToArray(typeof(UPnPDevice)));
        }
        /// <summary>
        /// Recursively searches the device heirarchy and returns an array of UPnPServices that match the urn
        /// </summary>
        /// <param name="urn">The URN</param>
        /// <returns>Array of UPnPServices</returns>
        public UPnPService[] GetServices(string urn)
        {
            ArrayList t = new ArrayList();
            foreach (UPnPService s in Services)
            {
                if (s.ServiceURN.ToLower().StartsWith(urn.ToLower()) == true)
                {
                    t.Add(s);
                }
            }
            return ((UPnPService[])t.ToArray(typeof(UPnPService)));
        }
        public UPnPDevice GetDevice(string UDN)
        {
            if (this.UniqueDeviceName == UDN) return (this);

            UPnPDevice RetVal = null;
            foreach (UPnPDevice ed in this.EmbeddedDevices)
            {
                RetVal = ed.GetDevice(UDN);
                if (RetVal != null) return (RetVal);
            }
            return (null);
        }
        public UPnPService GetService(string ServiceID)
        {
            if (ServiceID.ToUpper().StartsWith("URN:") == false)
            {
                ServiceID = "urn:upnp-org:serviceId:" + ServiceID;
            }
            foreach (UPnPService s in this.Services)
            {
                if (s.ServiceID == ServiceID) return (s);
            }
            return (null);
        }

        public override int GetHashCode()
        {
            if (ControlPointOnly == true)
            {
                if (BaseURL.Host == "127.0.0.1")
                {
                    return (this.descXmlLocation.GetHashCode());
                }
                else
                {
                    return (BaseURL.GetHashCode());
                }

            }
            else
            {
                return (base.GetHashCode());

                //				string temp = this.UniqueDeviceName;
                //				for(int x = 0;x<Services.Length;++x)
                //				{
                //					temp += Services[x].GetSCPDXml();
                //				}
                //				return(temp.GetHashCode());
            }
        }

        public override string ToString()
        {
            return FriendlyName;
        }

        internal void AddSubVirtualDirectory(string vd)
        {
            for (int i = 0; i < Services.Length; ++i)
            {
                Services[i].AddVirtualDirectory(vd);
            }

            foreach (UPnPDevice ed in this.EmbeddedDevices)
            {
                ed.AddSubVirtualDirectory(vd);
            }
        }

        /// <summary>
        /// Add a UPnPDevice to this device
        /// </summary>
        /// <param name="device">Device</param>
        public void AddDevice(IUPnPDevice device)
        {
            AddDevice(device.GetUPnPDevice());
        }

        /// <summary>
        /// Add a UPnPDevice to this device
        /// </summary>
        /// <param name="device">Device</param>
        public void AddDevice(UPnPDevice device)
        {
            device.ExpirationTimeout = this.ExpirationTimeout;
            device.parent = this;
            device.IsRoot = false;
            SetInterfaceToHost(device);

            UPnPDevice[] temp = new UPnPDevice[EmbeddedDevices.Length + 1];
            Array.Copy(EmbeddedDevices, 0, temp, 0, EmbeddedDevices.Length);
            temp[EmbeddedDevices.Length] = device;

            Array.Sort(temp, new UPnPDeviceComparer_Type());

            EmbeddedDevices = temp;

            // Special Processing
            if (ControlPointOnly == false)
            {
                device.AddSubVirtualDirectory(device.UniqueDeviceName);
                this.AddVirtualDirectory(device.UniqueDeviceName, new UPnPDevice.VirtualDirectoryHandler(device.HandleParent_Header), new UPnPDevice.VirtualDirectoryHandler(device.HandleParent));
            }
            else
            {
                this.AddVirtualDirectory(device.UniqueDeviceName, null, new UPnPDevice.VirtualDirectoryHandler(device.EventProcesser));
                ProcessDevice_EVENTCALLBACK(this);
            }
        }

        private void SetInterfaceToHost(UPnPDevice d)
        {
            d.InterfaceToHost = this.InterfaceToHost;
            foreach (UPnPDevice ed in d.EmbeddedDevices)
            {
                SetInterfaceToHost(ed);
            }
        }
        private void ProcessDevice_EVENTCALLBACK(UPnPDevice d)
        {
            MiniWebServer mws;

            foreach (UPnPDevice ed in d.EmbeddedDevices)
            {
                ProcessDevice_EVENTCALLBACK(ed);
            }
            foreach (UPnPService es in d.Services)
            {
                if (InterfaceToHost != null)
                {
                    mws = (MiniWebServer)CPWebServerTable[InterfaceToHost.ToString()];
                    if (InterfaceToHost.AddressFamily == AddressFamily.InterNetwork)
                    {
                        es.EventCallbackURL = "http://" + InterfaceToHost.ToString() + ":" + mws.LocalIPEndPoint.Port.ToString() + "/" + d.UniqueDeviceName + "/" + es.ServiceID;
                    }
                    else if (InterfaceToHost.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        es.EventCallbackURL = "http://[" + RemoveIPv6Scope(InterfaceToHost.ToString()) + "]:" + mws.LocalIPEndPoint.Port.ToString() + "/" + d.UniqueDeviceName + "/" + es.ServiceID;
                    }
                }
            }
        }

        private void NewCPInterface(NetworkInfo sender, IPAddress ip)
        {
            lock (typeof(UPnPDevice))
            {
                if (CPWebServerTable.ContainsKey(ip.ToString()) == false)
                {
                    MiniWebServer ws = new MiniWebServer(new IPEndPoint(ip, 0));
                    ws.OnReceive += new MiniWebServer.HTTPReceiveHandler(HandleWebRequest);
                    ws.OnHeader += new MiniWebServer.HTTPReceiveHandler(HandleHeaderRequest);
                    CPWebServerTable[ip.ToString()] = ws;
                }
                else
                {
                    if (CP_RegisteredInterfaces.ContainsKey(ip.ToString()) == false)
                    {
                        CP_RegisteredInterfaces[ip.ToString()] = true;
                        ((MiniWebServer)CPWebServerTable[ip.ToString()]).OnReceive += new MiniWebServer.HTTPReceiveHandler(HandleWebRequest);
                        ((MiniWebServer)CPWebServerTable[ip.ToString()]).OnHeader += new MiniWebServer.HTTPReceiveHandler(HandleHeaderRequest);
                    }
                }

            }
        }

        private void UpdateDeviceSink(UPnPDeviceFactory sender, UPnPDevice d, Uri LocationUri)
        {
            lock (UpdateTable)
            {
                UpdateTable.Remove(sender);
            }
            this.BaseURL = d.BaseURL;
            this.InterfaceToHost = d.InterfaceToHost;

            foreach (UPnPDevice ed in this.EmbeddedDevices)
            {
                SetInterfaceToHost(ed);
            }
            UpdateDeviceSink2(d);
        }

        private void UpdateDeviceSink2(UPnPDevice d)
        {
            foreach (UPnPService s in d.Services)
            {
                UPnPService MatchingService = this.GetService(s.ServiceID);

                if (MatchingService != null)
                {
                    MatchingService._Update(s);
                }
                else
                {
                    // This SHOULD never happen... Device Vendor messed up if this section executes
                }
            }
            foreach (UPnPDevice ed in d.EmbeddedDevices)
            {
                UpdateDeviceSink2(ed);
            }
        }
        public void UpdateDevice(Uri LocationUri, IPAddress HostInterface)
        {
            lock (UpdateTable)
            {
                UPnPDeviceFactory df = new UPnPDeviceFactory(LocationUri, 250, new UPnPDeviceFactory.UPnPDeviceHandler(UpdateDeviceSink), null, null, null);
                UpdateTable[df] = df;
            }
        }

        private void DisabledInterface(NetworkInfo sender, IPAddress ip)
        {
            SendNotify(); // Advertise on other valid interfaces

            MiniWebServer y;
            try
            {
                y = (MiniWebServer)WebServerTable[ip.ToString()];
                if (y != null) y.Dispose();
                WebServerTable[ip.ToString()] = null;
            }
            catch (Exception ex)
            {
                OpenSource.Utilities.EventLogger.Log(ex);
            }
        }

        private void ManualNewDeviceInterface(IPAddress ip, int Port)
        {
            //String tmp = ip.ToString();
            MiniWebServer WebServer = new MiniWebServer(new IPEndPoint(ip, Port));
            if ((this.OnSniff != null) || (this.OnSniffPacket != null)) WebServer.OnSession += new MiniWebServer.NewSessionHandler(SniffSessionSink);

            WebServer.OnReceive += new MiniWebServer.HTTPReceiveHandler(HandleWebRequest);
            WebServer.OnHeader += new MiniWebServer.HTTPReceiveHandler(HandleHeaderRequest);
            WebServerTable[ip.ToString()] = WebServer;
            SendNotify(ip);
        }
        private void NewDeviceInterface(NetworkInfo sender, IPAddress ip)
        {
            try
            {
                //String tmp = ip.ToString();

                MiniWebServer WebServer;
                WebServer = new MiniWebServer(new IPEndPoint(ip, UseThisPort));
                if ((this.OnSniff != null) || (this.OnSniffPacket != null)) WebServer.OnSession += new MiniWebServer.NewSessionHandler(SniffSessionSink);

                WebServer.OnReceive += new MiniWebServer.HTTPReceiveHandler(HandleWebRequest);
                WebServer.OnHeader += new MiniWebServer.HTTPReceiveHandler(HandleHeaderRequest);
                WebServerTable[ip.ToString()] = WebServer;
                SendNotify(ip);
            }
            catch (SocketException ex)
            {
                // Cannot bind to this IPAddress, so just ignore
                OpenSource.Utilities.EventLogger.Log(ex);
                OpenSource.Utilities.EventLogger.Log(ex, "UPnPDevice: " + this.FriendlyName + " @" + ip.ToString());
                //				System.Windows.Forms.MessageBox.Show(ex.ToString(),"UPnPDevice: " + this.FriendlyName+" @"+ip.ToString());
            }

        }

        public IPEndPoint RemoteEndPoint
        {
            get
            {
                String sIP = BaseURL.Host;
                if (BaseURL.HostNameType == UriHostNameType.Dns)
                {
                    sIP = Dns.GetHostEntry(sIP).AddressList[0].ToString();
                }
                return (new IPEndPoint(IPAddress.Parse(sIP), BaseURL.Port));
            }
        }


        public void StartDevice(IPEndPoint Manual)
        {
            if (ControlPointOnly == false)
            {
                if (IsRoot == true)
                {
                    this.ManualIPEndPoint = Manual;
                    this.ManualNewDeviceInterface(Manual.Address, Manual.Port);
                    Advertise();
                }
                else
                {
                    throw (new Exception("Cannot Start/Stop a Non-Root Device directly"));
                }
            }
            else
            {
                throw (new Exception("Cannot Start/Stop a Device instantiated by a Control Point"));
            }
        }

        public void StartDevice(int PortNumber)
        {
            UseThisPort = PortNumber;
            StartDevice();
        }

        public IPEndPoint[] LocalIPEndPoints
        {
            get
            {
                ArrayList list = new ArrayList();
                foreach (IPAddress a in NetInfo.GetLocalAddresses())
                {
                    MiniWebServer m = (MiniWebServer)WebServerTable[a.ToString()];
                    if (m != null)
                    {
                        list.Add(m.LocalIPEndPoint);
                    }
                }
                return ((IPEndPoint[])list.ToArray(typeof(IPEndPoint)));
            }
        }



        /// <summary>
        /// Start the UPnPDevice
        /// </summary>
        public void StartDevice()
        {
            if (ControlPointOnly == false)
            {
                if (IsRoot == true)
                {
                    NetInfo = new NetworkInfo(new NetworkInfo.InterfaceHandler(NewDeviceInterface));
                    NetInfo.OnInterfaceDisabled += new NetworkInfo.InterfaceHandler(DisabledInterface);
                    Advertise();
                }
                else
                {
                    throw (new Exception("Cannot Start/Stop a Non-Root Device directly"));
                }
            }
            else
            {
                throw (new Exception("Cannot Start/Stop a Device instantiated by a Control Point"));
            }
        }

        /// <summary>
        /// Stop the container and all services, and BYEBYE
        /// </summary>
        public void StopDevice()
        {
            if (ControlPointOnly == false)
            {
                if (IsRoot == true)
                {
                    if (NetInfo != null)
                    {
                        IPAddress[] ips = NetInfo.GetLocalAddresses();
                        for (int x = 0; x < ips.Length; ++x) SendBye(ips[x]);
                    }
                    else
                    {
                        if (ManualIPEndPoint != null) SendBye(this.ManualIPEndPoint.Address);
                    }

                    for (int x = 0; x < Services.Length; ++x)
                    {
                        Services[x].Dispose();
                    }

                    DisposeAllServers();
                }
                else
                {
                    throw (new Exception("Cannot Start/Stop a Non-Root Device Directly"));
                }
            }
            else
            {
                throw (new Exception("Cannot Start/Stop a Device instantiated by a Control Point"));
            }
        }

        private void SniffSessionSink(MiniWebServer Sender, HTTPSession s)
        {
            if (OnSniff != null)
            {
                s.OnSniff += new HTTPSession.SniffHandler(SniffSessionSink2);
            }
            else
            {
                if (OnSniffPacket == null)
                {
                    Sender.OnSession -= new MiniWebServer.NewSessionHandler(SniffSessionSink);
                }
            }

            if (OnSniffPacket != null)
            {
                s.OnSniffPacket += new HTTPSession.ReceiveHandler(SniffSessionSink3);
            }

        }
        private void SniffSessionSink2(byte[] raw, int offset, int length)
        {
            if (OnSniff != null) OnSniff(raw, offset, length);
        }
        private void SniffSessionSink3(HTTPSession sender, HTTPMessage msg)
        {
            if (OnSniffPacket != null) OnSniffPacket((HTTPMessage)msg.Clone());
        }


        private void DisposeAllServers()
        {
            MiniWebServer WebServer;
            Object[] WebKeys = new Object[WebServerTable.Keys.Count];
            WebServerTable.Keys.CopyTo(WebKeys, 0);

            for (int x = 0; x < WebKeys.Length; ++x)
            {
                WebServer = (MiniWebServer)WebServerTable[WebKeys[x]];
                WebServerTable.Remove(WebKeys[x]);
                if (WebServer != null)
                {
                    WebServer.Dispose();
                }
            }

        }
        private void HandleParent_Header(UPnPDevice sender, HTTPMessage msg, HTTPSession WebSession, string VD)
        {
            HandleHeaderRequest(msg, WebSession);
        }
        private void HandleParent(UPnPDevice sender, HTTPMessage msg, HTTPSession WebSession, string VD)
        {
            HandleWebRequest(msg, WebSession);
        }

        private void HandleEventReceive(HTTPSession TheSession, HTTPMessage msg)
        {
            TheSession.Close();
        }
        private void HandleInitialEvent(HTTPRequest R, HTTPMessage M, object Tag)
        {
            R.Dispose();
            this.InitialEventTable.Remove(R);
        }
        private void HandleHeaderRequest(HTTPMessage msg, HTTPSession WebSession)
        {
            DText parser = new DText();
            HTTPMessage Response = new HTTPMessage();
            String Method = msg.Directive;
            String MethodData = msg.DirectiveObj;

            VirtualDirectoryHandler H_cb = null;
            VirtualDirectoryHandler P_cb = null;

            String vd = "";
            String vdobj = "";

            // Check VirtualDirTable
            int vdi;
            try
            {
                vdi = MethodData.IndexOf("/", 1);
                if (vdi != -1)
                {
                    vdobj = MethodData.Substring(vdi);
                    vd = MethodData.Substring(0, vdi);
                    if (VirtualDir_Header_Table.ContainsKey(vd))
                    {
                        if (VirtualDir_Header_Table[vd] != null) H_cb = (VirtualDirectoryHandler)VirtualDir_Header_Table[vd];
                    }
                    if (VirtualDir_Table.ContainsKey(vd))
                    {
                        if (VirtualDir_Table[vd] != null) P_cb = (VirtualDirectoryHandler)VirtualDir_Table[vd];
                    }
                }
            }
            catch (Exception ex)
            {
                OpenSource.Utilities.EventLogger.Log(ex);
            }

            if ((H_cb != null) || (P_cb != null))
            {
                HTTPMessage _msg = (HTTPMessage)msg.Clone();
                _msg.DirectiveObj = vdobj;
                WebSession.InternalStateObject = new Object[3] { vd, vdobj, P_cb };
                if (H_cb != null) H_cb(this, _msg, WebSession, vd);
                return;
            }
        }
        private void EventProcesser(UPnPDevice sender, HTTPMessage msg, HTTPSession WebSession, string VirtualDir)
        {
            if (ControlPointOnly == true)
            {
                String Method = msg.Directive;
                HTTPMessage Response = new HTTPMessage();

                if (Method != "NOTIFY")
                {
                    Response.StatusCode = 405;
                    Response.StatusData = Method + " not supported";
                    WebSession.Send(Response);
                    return; // Other methods are unknown to us
                }
                else if (Method == "NOTIFY")
                {
                    for (int id = 0; id < Services.Length; ++id)
                    {
                        //						SSDP.ParseURL(Services[id].__eventurl,out WebIP, out WebPort, out WebData);
                        //						if (WebData==MethodData)
                        //						{
                        if (Services[id].IsYourEvent(msg.GetTag("SID")) == true)
                        {
                            Response.StatusCode = 200;
                            Response.StatusData = "OK";
                            WebSession.Send(Response);

                            Services[id]._TriggerEvent(msg.GetTag("SID"), long.Parse(msg.GetTag("SEQ")), msg.StringBuffer, 0);
                            break;
                        }
                        //						}
                    }
                }
            }
        }
        private void HandleWebRequest(HTTPMessage msg, HTTPSession WebSession)
        {
            DText parser = new DText();
            HTTPMessage Response = new HTTPMessage();
            HTTPMessage Response2 = null;
            String Method = msg.Directive;
            String MethodData = msg.DirectiveObj;

            if (WebSession.InternalStateObject != null)
            {
                HTTPMessage _msg = (HTTPMessage)msg.Clone();
                object[] state = (object[])WebSession.InternalStateObject;
                _msg.DirectiveObj = (string)state[1];
                VirtualDirectoryHandler t = (VirtualDirectoryHandler)state[2];
                WebSession.InternalStateObject = null;
                t(this, _msg, WebSession, (string)state[0]);
                return;

            }

            if ((Method != "GET") && (Method != "HEAD") && (Method != "POST") &&
                (Method != "SUBSCRIBE") && (Method != "UNSUBSCRIBE") &&
                (Method != "NOTIFY"))
            {
                Response.StatusCode = 405;
                Response.StatusData = Method + " not supported";
                WebSession.Send(Response);
                return; // Other methods are unknown to us
            }


            // Process Headers
            if (Method == "GET" || Method == "HEAD")
            {
                try
                {
                    Response = Get(MethodData, WebSession.Source);
                }
                catch (UPnPCustomException ce)
                {
                    OpenSource.Utilities.EventLogger.Log(ce);
                    Response.StatusCode = ce.ErrorCode;
                    Response.StatusData = ce.ErrorDescription;
                    WebSession.Send(Response);
                    return;
                }
                catch (Exception e)
                {
                    OpenSource.Utilities.EventLogger.Log(e);
                    Response.StatusCode = 500;
                    Response.StatusData = "Internal";
                    Response.StringBuffer = e.ToString();
                }
                if (Method == "HEAD")
                {
                    Response.BodyBuffer = null;
                }
                WebSession.Send(Response);
            }

            if (Method == "POST")
            {
                //InvokerInfo[Thread.CurrentThread.GetHashCode()] = WebSession;
                try
                {
                    Response = Post(MethodData, msg.StringBuffer, msg.GetTag("SOAPACTION"), WebSession);
                }
                catch (DelayedResponseException ex)
                {
                    OpenSource.Utilities.EventLogger.Log(ex);
                    InvokerInfo.Remove(Thread.CurrentThread.GetHashCode());
                    WebSession.StopReading();
                    return;
                }
                catch (UPnPCustomException ce)
                {
                    OpenSource.Utilities.EventLogger.Log(this, System.Diagnostics.EventLogEntryType.Error,
                        "UPnP Error [" + ce.ErrorCode.ToString() + "] " + ce.ErrorDescription);
                    Response.StatusCode = 500;
                    Response.StatusData = "Internal";
                    Response.StringBuffer = BuildErrorBody(ce);
                    WebSession.Send(Response);
                    InvokerInfo.Remove(Thread.CurrentThread.GetHashCode());
                    return;
                }
                catch (UPnPInvokeException ie)
                {
                    Response.StatusCode = 500;
                    Response.StatusData = "Internal";
                    if (ie.UPNP != null)
                    {
                        OpenSource.Utilities.EventLogger.Log(this, System.Diagnostics.EventLogEntryType.Error,
                            "UPnP Error [" + ie.UPNP.ErrorCode.ToString() + "] " + ie.UPNP.ErrorDescription);
                        Response.StringBuffer = BuildErrorBody(ie.UPNP);
                    }
                    else
                    {
                        OpenSource.Utilities.EventLogger.Log(this, System.Diagnostics.EventLogEntryType.Error,
                            "UPnP Invocation Error [" + ie.MethodName + "] " + ie.Message);
                        Response.StringBuffer = BuildErrorBody(new UPnPCustomException(500, ie.Message));
                    }
                    WebSession.Send(Response);
                    InvokerInfo.Remove(Thread.CurrentThread.GetHashCode());
                    return;
                }
                catch (UPnPTypeMismatchException tme)
                {
                    OpenSource.Utilities.EventLogger.Log(tme);
                    Response.StatusCode = 500;
                    Response.StatusData = "Internal";
                    Response.StringBuffer = BuildErrorBody(new UPnPCustomException(402, tme.Message));
                    WebSession.Send(Response);
                    InvokerInfo.Remove(Thread.CurrentThread.GetHashCode());
                    return;
                }
                catch (UPnPStateVariable.OutOfRangeException oor)
                {
                    OpenSource.Utilities.EventLogger.Log(oor);
                    Response.StatusCode = 500;
                    Response.StatusData = "Internal";
                    Response.StringBuffer = BuildErrorBody(new UPnPCustomException(402, oor.Message));
                    WebSession.Send(Response);
                    InvokerInfo.Remove(Thread.CurrentThread.GetHashCode());
                    return;
                }
                catch (System.Reflection.TargetInvocationException tie)
                {
                    Exception inner = tie.InnerException;
                    OpenSource.Utilities.EventLogger.Log(tie);
                    while (inner.InnerException != null && (typeof(UPnPCustomException).IsInstanceOfType(inner) == false))
                    {
                        inner = inner.InnerException;
                    }
                    if (typeof(UPnPCustomException).IsInstanceOfType(inner))
                    {
                        UPnPCustomException ce = (UPnPCustomException)inner;
                        OpenSource.Utilities.EventLogger.Log(this, System.Diagnostics.EventLogEntryType.Error,
                            "UPnP Error [" + ce.ErrorCode.ToString() + "] " + ce.ErrorDescription);
                        Response.StatusCode = 500;
                        Response.StatusData = "Internal";
                        Response.StringBuffer = BuildErrorBody(ce);
                        WebSession.Send(Response);
                        InvokerInfo.Remove(Thread.CurrentThread.GetHashCode());
                        return;
                    }
                    else
                    {
                        Response.StatusCode = 500;
                        Response.StatusData = "Internal";
                        Response.StringBuffer = BuildErrorBody(new UPnPCustomException(500, inner.ToString()));
                        WebSession.Send(Response);
                        OpenSource.Utilities.EventLogger.Log(inner);
                        return;
                    }
                }
                catch (Exception e)
                {
                    Response.StatusCode = 500;
                    Response.StatusData = "Internal";
                    Response.StringBuffer = BuildErrorBody(new UPnPCustomException(500, e.ToString()));
                    WebSession.Send(Response);
                    OpenSource.Utilities.EventLogger.Log(e);
                    return;
                }

                WebSession.Send(Response);
                InvokerInfo.Remove(Thread.CurrentThread.GetHashCode());
                return;
            }

            if (Method == "SUBSCRIBE")
            {
                String SID = msg.GetTag("SID");
                String NT = msg.GetTag("NT");
                String Timeout = msg.GetTag("Timeout");
                String CallbackURL = msg.GetTag("Callback");
                if (Timeout == "")
                {
                    Timeout = "7200"; // Default  = 2 Hours
                }
                else
                {
                    Timeout = Timeout.Substring(Timeout.IndexOf("-") + 1).Trim().ToUpper();
                    if (Timeout == "INFINITE")
                    {
                        Timeout = "0";
                    }
                }
                if (SID != "")
                {
                    // Renew
                    RenewEvents(MethodData.Substring(1), SID, Timeout);
                }
                else
                {
                    // Subscribe
                    try
                    {
                        Response2 = SubscribeEvents(ref SID, MethodData.Substring(1), CallbackURL, Timeout);
                    }
                    catch (Exception s_exception)
                    {
                        OpenSource.Utilities.EventLogger.Log(s_exception);
                        HTTPMessage err = new HTTPMessage();
                        err.StatusCode = 500;
                        err.StatusData = s_exception.Message;
                        WebSession.Send(err);
                        return;
                    }
                }
                if (Timeout == "0")
                {
                    Timeout = "Second-infinite";
                }
                else
                {
                    Timeout = "Second-" + Timeout;
                }
                Response.StatusCode = 200;
                Response.StatusData = "OK";
                Response.AddTag("Server", "Windows NT/5.0, UPnP/1.0");
                Response.AddTag("SID", SID);
                Response.AddTag("Timeout", Timeout);
                WebSession.Send(Response);
                if (Response2 != null)
                {
                    Uri[] cbURL = ParseEventURL(CallbackURL);
                    for (int x = 0; x < cbURL.Length; ++x)
                    {
                        Response2.DirectiveObj = HTTPMessage.UnEscapeString(cbURL[x].PathAndQuery);
                        Response2.AddTag("Host", cbURL[x].Host + ":" + cbURL[x].Port.ToString());

                        IPEndPoint d = new IPEndPoint(IPAddress.Parse(cbURL[x].Host), cbURL[x].Port);
                        HTTPRequest R = new HTTPRequest();
                        R.OnResponse += new HTTPRequest.RequestHandler(HandleInitialEvent);
                        this.InitialEventTable[R] = R;
                        R.PipelineRequest(d, Response2, null);
                    }
                }
            }

            if (Method == "UNSUBSCRIBE")
            {
                CancelEvent(MethodData.Substring(1), msg.GetTag("SID"));
                Response.StatusCode = 200;
                Response.StatusData = "OK";
                WebSession.Send(Response);
            }
        }

        private HTTPMessage Post(String MethodData, String XML, String SOAPACTION, HTTPSession WebSession)
        {
            return (Invoke(MethodData.Substring(1), XML, SOAPACTION, WebSession));
        }


        private HTTPMessage Get(String GetWhat, IPEndPoint local)
        {
            HTTPMessage msg = new HTTPMessage();

            if (GetWhat == "/")
            {
                msg.StatusCode = 200;
                msg.StatusData = "OK";
                msg.AddTag("Content-Type", "text/xml");
                msg.BodyBuffer = GetRootDeviceXML(local);
                return (msg);
            }
            else
            {
                GetWhat = GetWhat.Substring(1);
            }

            if ((GetWhat == "icon.png") && (_icon != null))
            {
                lock (_icon)
                {
                    MemoryStream mstm = new MemoryStream();
                    _icon.Save(mstm, System.Drawing.Imaging.ImageFormat.Png);
                    msg.StatusCode = 200;
                    msg.StatusData = "OK";
                    msg.ContentType = "image/png";
                    msg.BodyBuffer = mstm.ToArray();
                    mstm.Close();
                }
                return (msg);
            }

            if ((GetWhat == "favicon.ico") && (_favicon != null))
            {
                lock (_favicon)
                {
                    MemoryStream mstm = new MemoryStream();
                    _favicon.Save(mstm);
                    msg.StatusCode = 200;
                    msg.StatusData = "OK";
                    msg.ContentType = "image/x-icon";
                    msg.BodyBuffer = mstm.ToArray();
                    mstm.Close();
                }
                return (msg);
            }

            bool SCPDok = false;
            for (int id = 0; id < Services.Length; ++id)
            {
                if (GetWhat == Services[id].SCPDFile)
                {
                    SCPDok = true;
                    msg.StatusCode = 200;
                    msg.StatusData = "OK";
                    msg.AddTag("Content-Type", "text/xml");
                    msg.BodyBuffer = Services[id].GetSCPDXml();
                    break;
                }
            }
            if (SCPDok == true)
            {
                return (msg);
            }

            try
            {
                FileStream fs = new FileStream(RootPath + GetWhat, FileMode.Open, FileAccess.Read, FileShare.Read);
                byte[] buffer = new byte[(int)fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
                fs.Close();
                msg.StatusCode = 200;
                msg.StatusData = "OK";
                String ct = "application/octet-stream";
                if ((GetWhat.EndsWith(".html")) || (GetWhat.EndsWith(".htm")) == true)
                {
                    ct = "text/html";
                }
                else
                {
                    if (GetWhat.EndsWith(".xml") == true)
                    {
                        ct = "text/xml";
                    }
                }
                msg.AddTag("Content-Type", ct);
                msg.BodyBuffer = buffer;
                return (msg);
            }
            catch (Exception ex)
            {
                OpenSource.Utilities.EventLogger.Log(ex);
                throw (new UPnPCustomException(404, "File Not Found"));
            }
        }
        private void HandleSearch(String ST, IPEndPoint src, IPEndPoint local)
        {
            if (WebServerTable == null) return;
            //if (local == null) local = new IPEndPoint(IPAddress.Loopback, 1234); // DEBUG **********************

            string x = src.Address.ToString();
            ArrayList ResponseList = new ArrayList();
            HTTPMessage msg;
            string Location = null;
            if (local.AddressFamily == AddressFamily.InterNetwork)
            {
                Location = "http://" + local.Address.ToString() + ":" + ((MiniWebServer)WebServerTable[local.Address.ToString()]).LocalIPEndPoint.Port.ToString() + "/";
            }
            if (local.AddressFamily == AddressFamily.InterNetworkV6)
            {
                string xx = local.Address.ToString();
                xx = xx.Substring(0, xx.IndexOf("%"));
                Location = "http://[" + xx + "]:" + ((MiniWebServer)WebServerTable[local.Address.ToString()]).LocalIPEndPoint.Port.ToString() + "/";
            }

            if ((ST == "upnp:rootdevice") || (ST == "ssdp:all"))
            {
                msg = new HTTPMessage();
                msg.StatusCode = 200;
                msg.StatusData = "OK";
                msg.AddTag("ST", "upnp:rootdevice");
                msg.AddTag("USN", "uuid:" + UniqueDeviceName + "::upnp:rootdevice");
                msg.AddTag("Location", Location);
                msg.AddTag("Server", "Windows NT/5.0, UPnP/1.0");
                msg.AddTag("EXT", "");
                msg.AddTag("Cache-Control", "max-age=" + ExpirationTimeout.ToString());
                ResponseList.Add(msg);
            }

            ContainsSearchTarget(ST, Location, ResponseList);

            foreach (HTTPMessage SR in ResponseList)
            {
                try
                {
                    SR.LocalEndPoint = local;
                    SR.RemoteEndPoint = src;
                    SSDPServer.UnicastData(SR);
                }
                catch (SocketException ex)
                {
                    OpenSource.Utilities.EventLogger.Log(ex);
                }
            }
        }

        /// <summary>
        /// Force the device to (re)Advertise
        /// </summary>
        public void Advertise()
        {
            SendNotify();
        }

        private Uri[] ParseEventURL(String URLList)
        {
            DText parser = new DText();
            parser.ATTRMARK = ">";
            String temp;
            ArrayList TList = new ArrayList();

            parser[0] = URLList;

            int cnt = parser.DCOUNT();
            for (int x = 1; x <= cnt; ++x)
            {
                temp = parser[x];
                try
                {
                    temp = temp.Substring(temp.IndexOf("<") + 1);
                    TList.Add(new Uri(temp));
                }
                catch (Exception ex)
                {
                    OpenSource.Utilities.EventLogger.Log(ex);
                }
            }
            Uri[] RetVal = new Uri[TList.Count];
            for (int x = 0; x < RetVal.Length; ++x)
            {
                RetVal[x] = (Uri)TList[x];
            }
            return (RetVal);
        }

        private void SendNotify()
        {
            // Send a notify on all local interfaces
            if (NetInfo == null)
            {
                if (this.ManualIPEndPoint != null)
                {
                    SendNotify(this.ManualIPEndPoint.Address);
                }
            }
            else
            {
                foreach (IPAddress address in NetInfo.GetLocalAddresses())
                {
                    SendNotify(address);
                }
            }
        }

        private void SendNotify(IPAddress local)
        {
            if (!NoSSDP)
            {
                HTTPMessage[] packet = BuildNotifyPacket(local);
                for (int x = 0; x < packet.Length; ++x)
                {
                    SSDPServer.BroadcastData(packet[x], local);
                }
            }
        }

        private void SendBye(IPAddress local)
        {
            HTTPMessage[] Response = BuildByePacket(local);
            for (int x = 0; x < Response.Length; ++x) SSDPServer.BroadcastData(Response[x], local);
        }
        private bool RenewEvents(String MethodData, String SID, String Timeout)
        {
            bool IsOK = false;

            for (int id = 0; id < Services.Length; ++id)
            {
                if (Services[id].EventURL == MethodData)
                {
                    if (Services[id]._RenewEvent(SID, Timeout) == false)
                    {
                        throw (new Exception(SID + " is not a valid SID"));
                    }
                    IsOK = true;
                    break;
                }
            }
            if (IsOK == false)
            {
                throw (new Exception(MethodData + " is not a valid Event location"));
            }
            else
            {
                return (true);
            }
        }
        private void CancelEvent(String MethodData, String SID)
        {
            for (int id = 0; id < Services.Length; ++id)
            {
                if (Services[id].EventURL == MethodData)
                {
                    Services[id]._CancelEvent(SID);
                    break;
                }
            }
        }
        private HTTPMessage SubscribeEvents(ref String SID, String MethodData, String CallbackURL, String Timeout)
        {
            bool IsOK = false;

            HTTPMessage response2 = new HTTPMessage();

            for (int id = 0; id < Services.Length; ++id)
            {
                if (Services[id].EventURL == MethodData)
                {
                    response2 = Services[id]._SubscribeEvent(out SID, CallbackURL, Timeout);
                    IsOK = true;
                    break;
                }
            }
            if (IsOK == false)
            {
                throw (new Exception(MethodData + " is not a valid Event location"));
            }
            else
            {
                return (response2);
            }
        }

        private HTTPMessage Invoke(String Control, String XML, String SOAPACTION, HTTPSession WebSession)
        {
            String MethodTag = "";

            ArrayList VarList = new ArrayList();
            UPnPArgument VarArg;

            StringReader MyString = new StringReader(XML);
            XmlTextReader XMLDoc = new XmlTextReader(MyString);

            XMLDoc.Read();
            XMLDoc.MoveToContent();
            if (XMLDoc.LocalName == "Envelope")
            {
                XMLDoc.Read();
                XMLDoc.MoveToContent();

                if (XMLDoc.LocalName == "Body")
                {
                    XMLDoc.Read();
                    XMLDoc.MoveToContent();

                    MethodTag = XMLDoc.LocalName;

                    XMLDoc.Read();
                    XMLDoc.MoveToContent();
                    while ((XMLDoc.LocalName != MethodTag) && (XMLDoc.LocalName != "Envelope")
                        && (XMLDoc.LocalName != "Body"))
                    {
                        //						VarArg = new UPnPArgument(XMLDoc.LocalName,UPnPStringFormatter.UnEscapeString(XMLDoc.ReadInnerXml()));
                        VarArg = new UPnPArgument(XMLDoc.LocalName, XMLDoc.ReadString());

                        VarList.Add(VarArg);
                        if (XMLDoc.LocalName == "" || XMLDoc.IsStartElement() == false || XMLDoc.IsEmptyElement)
                        {
                            XMLDoc.Read();
                            XMLDoc.MoveToContent();
                        }
                    }
                }
            }

            Object RetVal = "";
            bool found = false;
            int id = 0;

            for (id = 0; id < Services.Length; ++id)
            {
                if (Services[id].ControlURL == Control)
                {
                    if (MethodTag != "QueryStateVariable")
                    {
                        UPnPAction A = Services[id].GetAction(MethodTag);
                        if (A == null) { break; }
                        ArrayList tlist = new ArrayList();
                        InvokerInfoStruct iis = new InvokerInfoStruct();
                        iis.WebSession = WebSession;
                        iis.MethodName = MethodTag;
                        iis.SOAPAction = SOAPACTION;
                        foreach (UPnPArgument arg in A.Arguments)
                        {
                            if (arg.IsReturnValue == true) iis.RetArg = (UPnPArgument)arg.Clone();
                            if (arg.Direction == "out") tlist.Add(arg.Clone());
                        }
                        iis.OutArgs = (UPnPArgument[])tlist.ToArray(typeof(UPnPArgument));
                        InvokerInfo[Thread.CurrentThread.GetHashCode()] = iis;
                    }

                    RetVal = Services[id].InvokeLocal(MethodTag, ref VarList);
                    found = true;
                    break;
                }
            }
            if (found == false)
            {
                throw (new UPnPCustomException(401, "Invalid Action: " + MethodTag));
            }


            HTTPMessage response = ParseInvokeResponse(MethodTag, SOAPACTION, Services[id].ServiceURN, RetVal, (UPnPArgument[])VarList.ToArray(typeof(UPnPArgument)));
            return (response);
        }

        internal HTTPMessage ParseInvokeResponse(string MethodTag, string SOAPACTION, string urn, object RetVal, UPnPArgument[] OutArgs)
        {
            HTTPMessage response = new HTTPMessage();
            MemoryStream mstream = new MemoryStream(4096);
            XmlTextWriter W = new XmlTextWriter(mstream, System.Text.Encoding.UTF8);
            W.Formatting = Formatting.Indented;
            W.Indentation = 3;

            W.WriteStartDocument();
            String S = "http://schemas.xmlsoap.org/soap/envelope/";

            W.WriteStartElement("s", "Envelope", S);
            W.WriteAttributeString("s", "encodingStyle", S, "http://schemas.xmlsoap.org/soap/encoding/");

            W.WriteStartElement("s", "Body", S);

            if (SOAPACTION.EndsWith("#QueryStateVariable\"") == false)
            {
                W.WriteStartElement("u", MethodTag + "Response", urn);
                if (RetVal != null)
                {
                    W.WriteElementString(((UPnPArgument)RetVal).Name, UPnPService.SerializeObjectInstance(((UPnPArgument)RetVal).DataValue));
                }
                foreach (UPnPArgument arg in OutArgs)
                {
                    W.WriteElementString(arg.Name, UPnPService.SerializeObjectInstance(arg.DataValue));
                }
            }
            else
            {
                //QueryStateVariableResponse
                String QSV = "urn:schemas-upnp-org:control-1-0";
                W.WriteStartElement("u", MethodTag + "Response", QSV);
                W.WriteElementString("return", UPnPStringFormatter.EscapeString(UPnPService.SerializeObjectInstance(RetVal)));
            }

            W.WriteEndElement();
            W.WriteEndElement();
            W.WriteEndElement();
            W.WriteEndDocument();
            W.Flush();

            byte[] wbuf = new Byte[mstream.Length - 3];
            mstream.Seek(3, SeekOrigin.Begin);
            mstream.Read(wbuf, 0, wbuf.Length);
            W.Close();

            response.StatusCode = 200;
            response.StatusData = "OK";
            response.AddTag("Content-Type", "text/xml; charset=\"utf-8\"");
            response.AddTag("EXT", "");
            response.AddTag("Server", "Windows NT/5.0, UPnP/1.0");
            response.BodyBuffer = wbuf;
            return (response);
        }

        public bool RemoveService(IUPnPService service)
        {
            return (RemoveService(service.GetUPnPService()));
        }
        public bool RemoveService(UPnPService service)
        {
            int j = 0;
            int i = 0;
            UPnPService[] temp = new UPnPService[Services.Length - 1];
            for (i = 0; i < Services.Length; i++)
            {
                if (Services[i] != service)
                {
                    temp[j] = Services[i];
                    j++;
                }
            }
            if (i == j + 1)
            {
                Services = temp;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Add a UPnPService to this device
        /// </summary>
        /// <param name="service">Service</param>
        public void AddService(IUPnPService service)
        {
            AddService(service.GetUPnPService());
        }

        /// <summary>
        /// Add a UPnPService to this device
        /// </summary>
        /// <param name="service">Service</param>
        public void AddService(UPnPService service)
        {
            if (ControlPointOnly == true)
            {
                // This is a Remote Service
                String bURL;
                if (BaseURL == null)
                {
                    bURL = "";
                }
                else
                {
                    bURL = BaseURL.AbsoluteUri;
                }

                if (bURL.EndsWith("/") == false)
                {
                    bURL += "/";
                }

                if (service.__controlurl.StartsWith("http://") == false)
                {
                    if (service.__controlurl.StartsWith("/") == true)
                    {
                        service.ControlURL = "http://" + BaseURL.Host + ":" + BaseURL.Port.ToString() + service.__controlurl;
                    }
                    else
                    {
                        service.ControlURL = bURL + service.__controlurl;
                    }
                }
                if (service.SCPDURL.StartsWith("http://") == false)
                {
                    if (service.SCPDURL.StartsWith("/") == true)
                    {
                        service.SCPDURL = "http://" + BaseURL.Host + ":" + BaseURL.Port.ToString() + service.SCPDURL;
                    }
                    else
                    {
                        service.SCPDURL = bURL + service.SCPDURL;
                    }
                }

                if (service.__eventurl.StartsWith("http://") == false)
                {
                    if (service.__eventurl.StartsWith("/") == true)
                    {
                        service.EventURL = "http://" + BaseURL.Host + ":" + BaseURL.Port.ToString() + service.__eventurl;
                    }
                    else
                    {
                        service.EventURL = bURL + service.__eventurl;
                    }
                }
                String WebIP;
                int WebPort;
                String WebData;
                MiniWebServer mws;
                String ITH;
                SSDP.ParseURL(service.__eventurl, out WebIP, out WebPort, out WebData);
                if (InterfaceToHost != null)
                {
                    try
                    {
                        ITH = InterfaceToHost.ToString();
                        mws = (MiniWebServer)CPWebServerTable[ITH];

                        UPnPDevice td = this;
                        while (td.parent != null) td = td.parent;
                        td.AddVirtualDirectory(this.UniqueDeviceName, null, new UPnPDevice.VirtualDirectoryHandler(EventProcesser));

                        if (InterfaceToHost.AddressFamily == AddressFamily.InterNetwork)
                        {
                            service.EventCallbackURL = "http://" + InterfaceToHost.ToString() + ":" + mws.LocalIPEndPoint.Port.ToString() + "/" + this.UniqueDeviceName + "/" + service.ServiceID;
                        }
                        else if (InterfaceToHost.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            service.EventCallbackURL = "http://[" + RemoveIPv6Scope(InterfaceToHost.ToString()) + "]:" + mws.LocalIPEndPoint.Port.ToString() + "/" + this.UniqueDeviceName + "/" + service.ServiceID;
                        }
                    }
                    catch (Exception e)
                    {
                        OpenSource.Utilities.EventLogger.Log(e);
                    }
                }
            }
            else
            {
                // This is a local Service
                if (service.SCPDURL == "" || service.SCPDURL == null) service.SCPDURL = "_" + service.ServiceID + "_scpd.xml";
                if (service.ControlURL == "") service.ControlURL = "_" + service.ServiceID + "_control";
                if (service.EventURL == "") service.EventURL = "_" + service.ServiceID + "_event";
            }

            service.ParentDevice = this;

            SortedList SL = new SortedList();
            foreach (UPnPService _s in Services)
            {
                SL.Add(_s.ServiceURN + "[[" + _s.ServiceID, _s);
            }
            SL.Add(service.ServiceURN + "[[" + service.ServiceID, service);

            UPnPService[] temp = new UPnPService[Services.Length + 1];
            SL.Values.CopyTo(temp, 0);
            Services = temp;
        }
        /// <summary>
        /// Enter the Presentation file, and the PresentationURL will be built
        /// </summary>
        public String PresentationURL
        {
            get
            {
                return (_PresentationURL);
            }
            set
            {
                _PresentationURL = value;
            }
        }

        /// <summary>
        /// Add a virutal directory to the MiniWebServer
        /// </summary>
        /// <param name="dir">The virtual directory</param>
        /// <param name="callback">The callback to receive packets</param>
        public void AddVirtualDirectory(String dir, VirtualDirectoryHandler HeaderCallback, VirtualDirectoryHandler PacketCallback)
        {
            if (dir.StartsWith("/") == false) { dir = "/" + dir; }
            VirtualDir_Table[dir] = PacketCallback;
            VirtualDir_Header_Table[dir] = HeaderCallback;
        }

        /// <summary>
        /// Enter the type, and the URN will be automatically generated for a Standard UPnP Device Type
        /// </summary>
        /// <remarks>Returns an empty string if the DeviceURN does not contain a standard UPnP Device Type</remarks>
        public String StandardDeviceType
        {
            get
            {
                string result = "";
                if (DeviceURN.Length > 29 && DeviceURN.Substring(0, 28) == "urn:schemas-upnp-org:device:") result = DeviceURN.Split(':')[3];
                return result;
            }
            set
            {
                DeviceURN = string.Format("urn:schemas-upnp-org:device:{0}:{1}", value, Version);
            }
        }

        /// <summary>
        /// Get the version of this device
        /// </summary>
        public String Version
        {
            get
            {
                if (Minor == 0)
                {
                    return (Major.ToString());
                }
                else
                {
                    return (Major.ToString() + "-" + Minor.ToString());
                }
            }
        }
        public string ArchitectureVersion
        {
            get
            {
                return (Arch_Major.ToString() + "." + Arch_Minor.ToString());
            }
            set
            {
                DText p = new DText();
                p.ATTRMARK = ".";
                p[0] = value;
                Arch_Major = int.Parse(p[1]);
                Arch_Minor = int.Parse(p[2]);

                foreach (UPnPDevice d in this.EmbeddedDevices)
                {
                    d.ArchitectureVersion = value;
                }
            }
        }
        private void SetVersion(string v)
        {
            DText p = new DText();
            if (v.IndexOf("-") == -1)
            {
                p.ATTRMARK = ".";
            }
            else
            {
                p.ATTRMARK = "-";
            }
            p[0] = v;

            string mj = p[1];
            string mn = p[2];

            this.Major = 0;
            int.TryParse(mj, out this.Major);
            this.Minor = 0;
            int.TryParse(mn, out this.Minor);

            /*
            if (mj == "")
            {
                this.Major = 0;
            }
            else
            {
                this.Major = int.Parse(mj);
            }

            if (mn == "")
            {
                this.Minor = 0;
            }
            else
            {
                this.Minor = int.Parse(mn);
            }
            */
        }

        private HTTPMessage[] BuildNotifyPacket(IPAddress local)
        {
            ArrayList NotifyList = new ArrayList();
            IPEndPoint localep = null;
            HTTPMessage msg;

            try
            {
                localep = ((MiniWebServer)WebServerTable[local.ToString()]).LocalIPEndPoint;
            }
            catch (Exception ex)
            {
                OpenSource.Utilities.EventLogger.Log(ex);
                return (new HTTPMessage[0]);
            }
            String BaseURL;
            if (localep.AddressFamily == AddressFamily.InterNetworkV6)
            {
                string ipaddr = localep.Address.ToString();
                int i = ipaddr.IndexOf("%");
                if (i > 0) ipaddr = ipaddr.Substring(0, i);
                BaseURL = "http://[" + ipaddr + "]:" + localep.Port.ToString() + "/";
            }
            else
            {
                BaseURL = "http://" + localep.Address.ToString() + ":" + localep.Port.ToString() + "/";
            }

            msg = new HTTPMessage();
            msg.Directive = "NOTIFY";
            msg.DirectiveObj = "*";
            msg.AddTag("Host", Utils.GetMulticastAddrBraketPort(local));
            msg.AddTag("NT", "upnp:rootdevice");
            msg.AddTag("NTS", "ssdp:alive");
            msg.AddTag("Location", BaseURL);
            msg.AddTag("USN", "uuid:" + UniqueDeviceName + "::upnp:rootdevice");
            msg.AddTag("Server", "Windows NT/5.0, UPnP/1.0");
            msg.AddTag("Cache-Control", "max-age=" + ExpirationTimeout.ToString());
            NotifyList.Add(msg);

            BuildNotifyPacket2(BaseURL, NotifyList, local);

            foreach (UPnPDevice d in this.EmbeddedDevices)
            {
                d.BuildNotifyPacket2(BaseURL, NotifyList, local);
            }

            return ((HTTPMessage[])NotifyList.ToArray(typeof(HTTPMessage)));
        }

        private void BuildNotifyPacket2(string BaseURL, ArrayList NotifyList, IPAddress local)
        {
            HTTPMessage msg;

            for (int id = 0; id < Services.Length; ++id)
            {
                msg = new HTTPMessage();
                msg.Directive = "NOTIFY";
                msg.DirectiveObj = "*";
                msg.AddTag("Host", Utils.GetMulticastAddrBraketPort(local));
                msg.AddTag("NT", Services[id].ServiceURN);
                msg.AddTag("NTS", "ssdp:alive");
                msg.AddTag("Location", BaseURL);
                msg.AddTag("USN", "uuid:" + UniqueDeviceName + "::" + Services[id].ServiceURN);
                msg.AddTag("Server", "Windows NT/5.0, UPnP/1.0");
                msg.AddTag("Cache-Control", "max-age=" + ExpirationTimeout.ToString());
                NotifyList.Add(msg);
            }
            msg = new HTTPMessage();
            msg.Directive = "NOTIFY";
            msg.DirectiveObj = "*";
            msg.AddTag("Host", Utils.GetMulticastAddrBraketPort(local));
            msg.AddTag("NT", DeviceURN);
            msg.AddTag("NTS", "ssdp:alive");
            msg.AddTag("Location", BaseURL);
            msg.AddTag("USN", "uuid:" + UniqueDeviceName + "::" + DeviceURN);
            msg.AddTag("Server", "Windows NT/5.0, UPnP/1.0");
            msg.AddTag("Cache-Control", "max-age=" + ExpirationTimeout.ToString());
            NotifyList.Add(msg);

            msg = new HTTPMessage();
            msg.Directive = "NOTIFY";
            msg.DirectiveObj = "*";
            msg.AddTag("Host", Utils.GetMulticastAddrBraketPort(local));
            msg.AddTag("NT", "uuid:" + UniqueDeviceName);
            msg.AddTag("NTS", "ssdp:alive");
            msg.AddTag("Location", BaseURL);
            msg.AddTag("USN", "uuid:" + UniqueDeviceName);
            msg.AddTag("Server", "Windows NT/5.0, UPnP/1.0");
            msg.AddTag("Cache-Control", "max-age=" + ExpirationTimeout.ToString());
            NotifyList.Add(msg);
        }

        private HTTPMessage[] BuildByePacket(IPAddress local)
        {
            HTTPMessage msg;
            ArrayList ByeList = new ArrayList();

            msg = new HTTPMessage();
            msg.Directive = "NOTIFY";
            msg.DirectiveObj = "*";
            msg.AddTag("Host", Utils.GetMulticastAddrBraketPort(local));
            msg.AddTag("NT", "upnp:rootdevice");
            msg.AddTag("NTS", "ssdp:byebye");
            msg.AddTag("USN", "uuid:" + UniqueDeviceName + "::upnp:rootdevice");
            ByeList.Add(msg);

            BuildByePacket2(ByeList, local);
            foreach (UPnPDevice d in this.EmbeddedDevices)
            {
                d.BuildByePacket2(ByeList, local);
            }

            return ((HTTPMessage[])ByeList.ToArray(typeof(HTTPMessage)));
        }
        private void BuildByePacket2(ArrayList ByeList, IPAddress local)
        {
            HTTPMessage msg;

            for (int id = 0; id < Services.Length; ++id)
            {
                msg = new HTTPMessage();
                msg.Directive = "NOTIFY";
                msg.DirectiveObj = "*";
                msg.AddTag("Host", Utils.GetMulticastAddrBraketPort(local));
                msg.AddTag("NT", Services[id].ServiceURN);
                msg.AddTag("NTS", "ssdp:byebye");
                msg.AddTag("USN", "uuid:" + UniqueDeviceName + "::" + Services[id].ServiceURN);
                ByeList.Add(msg);
            }
            msg = new HTTPMessage();
            msg.Directive = "NOTIFY";
            msg.DirectiveObj = "*";
            msg.AddTag("Host", Utils.GetMulticastAddrBraketPort(local));
            msg.AddTag("NT", DeviceURN);
            msg.AddTag("NTS", "ssdp:byebye");
            msg.AddTag("USN", "uuid:" + UniqueDeviceName + "::" + DeviceURN);
            ByeList.Add(msg);

            msg = new HTTPMessage();
            msg.Directive = "NOTIFY";
            msg.DirectiveObj = "*";
            msg.AddTag("Host", Utils.GetMulticastAddrBraketPort(local));
            msg.AddTag("NT", "uuid:" + UniqueDeviceName);
            msg.AddTag("NTS", "ssdp:byebye");
            msg.AddTag("USN", "uuid:" + UniqueDeviceName);
            ByeList.Add(msg);
        }

        internal string BuildErrorBody(UPnPCustomException e)
        {
            System.Text.StringBuilder s = new System.Text.StringBuilder();

            s.Append("<s:Envelope\r\n");
            s.Append("   xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n");
            s.Append("   s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">\r\n");
            s.Append("   <s:Body>\r\n");
            s.Append("      <s:Fault>\r\n");
            s.Append("         <faultcode>s:Client</faultcode>\r\n");
            s.Append("         <faultstring>UPnPError</faultstring>\r\n");
            s.Append("            <detail>\r\n");
            s.Append("               <UPnPError xmlns=\"urn:schemas-upnp-org:control-1-0\">\r\n");
            s.Append("                  <errorCode>" + e.ErrorCode.ToString() + "</errorCode>\r\n");
            s.Append("                  <errorDescription>" + UPnPStringFormatter.EscapeString(e.ErrorDescription) + "</errorDescription>\r\n");
            s.Append("               </UPnPError>\r\n");
            s.Append("            </detail>\r\n");
            s.Append("      </s:Fault>\r\n");
            s.Append("   </s:Body>\r\n");
            s.Append("</s:Envelope>");
            return (s.ToString());
        }

        /*
        static internal UPnPDevice Parse(String XML)
        {
            return(Parse(XML,null));
        }*/
        static internal String highlightError(String original, int line, int charPosition)
        {
            int i,j,k;
            original = original.Replace("\r\n","\r");  // Clean up any CRLF nonesense
            original = original.Replace("\n","\r");
            String[] sarr = original.Split(new char[] {'\r'});
            string[] sarr2 = new String[sarr.Length + 1];
            for (i=0, j=0; i < sarr.Length; ++i, ++j)
            {
                if (i + 1 == line)
                {
                    sarr2[j] = "--->" + sarr[i];
                    j++;
                    sarr2[j] = "    ";
                    if (charPosition > sarr[i].Length)
                    {
                        charPosition = sarr[i].Length + 1;    // Strange case. Point the carret one spot beyond the last character
                    }
                    for (k = 1; k < charPosition; ++k)
                    {
                        sarr2[j] += " ";
                    }
                    sarr2[j] += "^";
                }
                else
                {
                    sarr2[j] = "    " + sarr[i];
                }
            }
            if (i == j)
            {
                sarr2[j] = "--->?";
            }
            String result = String.Join("\r",sarr2) + "\r";
            result = result.Replace("\r","\r\n");
            return(result);
        }

        static internal UPnPDevice Parse(String XML, Uri source, IPAddress Intfce)
        {
            bool Skipping;
            int embeddedLine = 0;
            //PlugFest HeaderChange
            string AssumedBaseURL = source.AbsoluteUri;
            AssumedBaseURL = AssumedBaseURL.Substring(0, AssumedBaseURL.LastIndexOf("/"));

            StringReader MyString = new StringReader(XML);
            XmlTextReader XMLDoc = new XmlTextReader(MyString);
            UPnPDevice RetVal = new UPnPDevice();
            RetVal.InterfaceToHost = Intfce;
            RetVal.IsRoot = true;

            bool NeedParseDevice = false;
            string NeedParseDeviceString = "";

            try
            {
                XMLDoc.Read();
                XMLDoc.MoveToContent();

                if (XMLDoc.LocalName == "root")
                {
                    XMLDoc.Read();
                    XMLDoc.MoveToContent();

                    while ((XMLDoc.LocalName != "root") && (XMLDoc.EOF == false))
                    {
                        Skipping = false;
                        switch (XMLDoc.LocalName)
                        {
                            case "specVersion":
                                XMLDoc.Read();
                                XMLDoc.MoveToContent();

                                //ToDo: This is supposed to be Arch_Major and Arch_Minor
                                RetVal.Arch_Major = int.Parse(XMLDoc.ReadString());
                                XMLDoc.Read();
                                XMLDoc.MoveToContent();
                                RetVal.Arch_Minor = int.Parse(XMLDoc.ReadString());

                                XMLDoc.Read();
                                XMLDoc.MoveToContent();
                                break;
                            case "URLBase":
                                RetVal.BaseURL = new Uri(XMLDoc.ReadString());
                                break;
                            case "device":
                                embeddedLine = XMLDoc.LineNumber;
                                if (RetVal.BaseURL == null)
                                {
                                    NeedParseDevice = true;
                                    NeedParseDeviceString = XMLDoc.ReadOuterXml();
                                }
                                else
                                {
                                    //ParseDevice("<device>\r\n" + XMLDoc.ReadInnerXml() + "</device>", embeddedLine, ref RetVal);
                                    ParseDevice(XMLDoc.ReadOuterXml(), embeddedLine-1, ref RetVal);
                                }
                                break;
                            default:
                                XMLDoc.Skip();
                                Skipping = true;
                                break;
                        }
                        if (Skipping == false)
                        {
                            XMLDoc.Read();
                            // XMLDoc.MoveToContent();	
                        }
                    }
                    if (NeedParseDevice == true)
                    {
                        if (RetVal.BaseURL == null)
                        {
                            RetVal.BaseURL = new Uri(AssumedBaseURL);
                            // RetVal.BaseURL = new Uri("http://127.0.0.1/");
                        }
                        ParseDevice(NeedParseDeviceString, embeddedLine-1, ref RetVal);
                    }
                    return (RetVal);
                }
                else
                {
                    return (null);
                }
            }
            catch (XMLParsingException ex)
            {
                OpenSource.Utilities.EventLogger.Log(ex,"\r\nLine: " + ex.line.ToString() + 
                                    ", Position: " + ex.position.ToString() +
                                    "\r\nURL: " + source + "\r\nXML:\r\n" + highlightError(XML,ex.line,ex.position));
            }
            catch (Exception ex)
            {
                OpenSource.Utilities.EventLogger.Log(ex, "Invalid UPnP Device Description\r\nLine " + (XMLDoc.LineNumber).ToString() +
                                    ", Position " + XMLDoc.LinePosition.ToString() +
                                    "\r\nURL: " + source + "\r\nXML:\r\n" + highlightError(XML, XMLDoc.LineNumber, XMLDoc.LinePosition));
            }
            return (null);
        }

        static private void ParseDeviceList(String XML, int startLine, ref UPnPDevice RetVal)
        {
            StringReader MyString = new StringReader(XML);
            XmlTextReader XMLDoc = new XmlTextReader(MyString);
            UPnPDevice EmbeddedDevice = null;
            int embededLine;

            try
            {
                XMLDoc.Read();
                XMLDoc.MoveToContent();

                if (XMLDoc.LocalName == "deviceList")
                {
                    XMLDoc.Read();
                    XMLDoc.MoveToContent();

                    while (XMLDoc.LocalName != "deviceList" && !XMLDoc.EOF)
                    {
                        if (XMLDoc.LocalName == "device")
                        {
                            EmbeddedDevice = new UPnPDevice();
                            EmbeddedDevice.IsRoot = false;
                            EmbeddedDevice.BaseURL = RetVal.BaseURL;
                            embededLine = XMLDoc.LineNumber;
                            //ParseDevice("<device>\r\n" + XMLDoc.ReadInnerXml() + "</device>", startLine + embededLine, ref EmbeddedDevice);
                            ParseDevice(XMLDoc.ReadOuterXml(), startLine + embededLine, ref EmbeddedDevice);
                            RetVal.AddDevice(EmbeddedDevice);
                        }
                        if (!XMLDoc.IsStartElement() && XMLDoc.LocalName != "deviceList")
                        {
                            XMLDoc.Read();
                            XMLDoc.MoveToContent();
                        }
                    }
                }
            }
            catch (XMLParsingException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new XMLParsingException("Invalid DeviceList XML near line ", (startLine + XMLDoc.LineNumber),XMLDoc.LinePosition, ex);
            }
        }
        static private void ParseDevice(String XML, int startLine, ref UPnPDevice RetVal)
        {
            string TempString;
            UPnPService service;
            int embeddedLine;
            DText p = new DText();
            TextReader MyString = new StringReader(XML);
            XmlTextReader XMLDoc = new XmlTextReader(MyString);

            try
            {
                XMLDoc.Read();
                XMLDoc.MoveToContent();

                if (XMLDoc.LocalName == "device")
                {
                    if (XMLDoc.AttributeCount > 0)
                    {
                        for (int ax = 0; ax < XMLDoc.AttributeCount; ++ax)
                        {
                            XMLDoc.MoveToAttribute(ax);
                            if (XMLDoc.LocalName == "MaxVersion")
                            {
                                RetVal.SetVersion(XMLDoc.Value);
                            }
                        }
                        XMLDoc.MoveToContent();
                        XMLDoc.Read();
                    }
                    else
                    {
                        XMLDoc.Read();
                        XMLDoc.MoveToContent();
                    }

                    while (XMLDoc.LocalName != "device" && !XMLDoc.EOF)
                    {
                        switch (XMLDoc.LocalName)
                        {
                            case "deviceList":
                                embeddedLine = XMLDoc.LineNumber;
                                ParseDeviceList(XMLDoc.ReadOuterXml(), startLine + embeddedLine-1, ref RetVal);
                                break;
                            case "URLBase":
                                // Here, tport is a hack to make Windows Home Server visible. WHS does no set the port in the BaseURL and so, you need to keep it from the SSDP message.
                                int tport = 80;
                                if (RetVal.BaseURL != null) tport = RetVal.BaseURL.Port;
                                RetVal.BaseURL = new Uri(XMLDoc.ReadString());
                                if (RetVal.BaseURL.Port == 80 && RetVal.BaseURL.Port != tport) RetVal.BaseURL = new Uri(RetVal.BaseURL.Scheme + "://" + RetVal.BaseURL.Host + ":" + tport + RetVal.BaseURL.AbsolutePath);
                                break;
                            case "deviceType":
                                RetVal.DeviceURN = XMLDoc.ReadString();
                                break;
                            case "friendlyName":
                                RetVal.FriendlyName = XMLDoc.ReadString();
                                break;
                            case "manufacturer":
                                RetVal.Manufacturer = XMLDoc.ReadString();
                                break;
                            case "manufacturerURL":
                                RetVal.ManufacturerURL = XMLDoc.ReadString();
                                break;
                            case "modelDescription":
                                RetVal.ModelDescription = XMLDoc.ReadString();
                                break;
                            case "modelName":
                                RetVal.ModelName = XMLDoc.ReadString();
                                break;
                            case "modelNumber":
                                RetVal.ModelNumber = XMLDoc.ReadString();
                                break;
                            case "modelURL":
                                try
                                {
                                    string u = XMLDoc.ReadString();
                                    if (Uri.TryCreate(u, UriKind.Absolute, out RetVal.ModelURL) == false) { Uri.TryCreate("http://" + u, UriKind.Absolute, out RetVal.ModelURL); }
                                }
                                catch (Exception ex)
                                {
                                    OpenSource.Utilities.EventLogger.Log(ex);
                                }
                                break;
                            case "serialNumber":
                                RetVal.SerialNumber = XMLDoc.ReadString();
                                break;
                            case "UDN":
                                TempString = XMLDoc.ReadString();
                                RetVal.UniqueDeviceName = TempString.Substring(5);
                                break;
                            case "UPC":
                                RetVal.ProductCode = XMLDoc.ReadString();
                                break;
                            case "presentationURL":
                                RetVal.HasPresentation = true;
                                RetVal.PresentationURL = XMLDoc.ReadString();
                                break;
                            case "serviceList":
                                if (XMLDoc.IsEmptyElement) break;

                                XMLDoc.Read();
                                XMLDoc.MoveToContent();
                                while (XMLDoc.LocalName != "serviceList")
                                {
                                    if (XMLDoc.LocalName == "service")
                                    {
                                        embeddedLine = XMLDoc.LineNumber;
                                        service = UPnPService.Parse(XMLDoc.ReadOuterXml(), embeddedLine-1+startLine);
                                        RetVal.AddService(service);
                                    }
                                    if (!XMLDoc.IsStartElement())
                                    {
                                        if (XMLDoc.LocalName != "serviceList")
                                        {
                                            XMLDoc.Read();
                                            XMLDoc.MoveToContent();
                                        }
                                    }
                                }
                                break;
                            case "iconList":
                                bool finishedIconList = false;
                                while (!finishedIconList && XMLDoc.Read())
                                {
                                    switch (XMLDoc.NodeType)
                                    {
                                        case XmlNodeType.Element:
                                            if (XMLDoc.LocalName == "icon")
                                            {
                                                embeddedLine = XMLDoc.LineNumber;
                                                ParseIconXML(RetVal, startLine + embeddedLine-1, XMLDoc.ReadOuterXml());
                                                if (XMLDoc.NodeType == XmlNodeType.EndElement && XMLDoc.LocalName == "iconList") { finishedIconList = true; }
                                            }
                                            break;
                                        case XmlNodeType.EndElement:
                                            if (XMLDoc.LocalName == "iconList") { finishedIconList = true; }
                                            break;
                                    }
                                }
                                break;
                            default:
                                if (XMLDoc.LocalName != "")
                                {
                                    string customPrefix = XMLDoc.Prefix;
                                    string customFieldName = XMLDoc.LocalName;
                                    string customFieldNamespace = XMLDoc.LookupNamespace(customPrefix);
                                    string customFieldVal = XMLDoc.ReadInnerXml();
                                    RetVal.AddCustomFieldInDescription(customFieldName, customFieldVal, customFieldNamespace);
                                }
                                else
                                {
                                    XMLDoc.Skip();
                                }
                                continue;
                        }

                        XMLDoc.Read();
                        //XMLDoc.MoveToContent();
                    }
                }
            }
            catch (XMLParsingException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new XMLParsingException("Invalid Device XML", startLine + XMLDoc.LineNumber, XMLDoc.LinePosition, ex);
            }
        }

        private static void ParseIconXML(UPnPDevice d, int startLine, String XML)
        {
            StringReader MyString = new StringReader(XML);
            XmlTextReader XMLDoc = new XmlTextReader(MyString);
            String iurl = null;

            try
            {
                XMLDoc.Read();
                XMLDoc.MoveToContent();

                if (XMLDoc.LocalName == "icon")
                {
                    XMLDoc.Read();
                    XMLDoc.MoveToContent();
                    while (XMLDoc.LocalName != "icon")
                    {
                        if (XMLDoc.LocalName == "url")
                        {
                            iurl = XMLDoc.ReadString();
                        }
                        else
                        {
                            XMLDoc.Skip();
                        }
                        XMLDoc.Read();
                        XMLDoc.MoveToContent();
                    }
                }

                if (iurl != null && d.BaseURL != null)
                {
                    if (iurl.ToUpper().StartsWith("HTTP://") == false)
                    {
                        if (iurl.StartsWith("/") == true)
                        {
                            iurl = "http://" + d.BaseURL.Host + ":" + d.BaseURL.Port.ToString() + iurl;
                        }
                        else
                        {
                            iurl = HTTPMessage.UnEscapeString(d.BaseURL.AbsoluteUri + iurl);
                        }
                    }
                    d.FetchIcon(new Uri(iurl));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid icon XML near line " + (startLine + XMLDoc.LineNumber).ToString() + ", Position " + XMLDoc.LinePosition.ToString(), ex);
            }
        }

        internal void FetchIcon(Uri IconUri)
        {
            HttpRequestor httprequestor = new HttpRequestor();
            httprequestor.OnRequestCompleted += new HttpRequestor.RequestCompletedHandler(HandleIcon);
            httprequestor.LaunchRequest(IconUri.ToString(), null, null, null, null);
            InitialEventTable[httprequestor] = httprequestor;
        }

        void HandleIcon(HttpRequestor sender, bool success, object tag, string url, byte[] data)
        {
            InitialEventTable.Remove(sender);
            if (success)
            {
                System.Drawing.Image i = System.Drawing.Image.FromStream(new MemoryStream(data));
                if (i != null) _icon = i;
            }
        }

        /// <summary>
        /// Adds response packets for matches in a search request
        /// </summary>
        /// <param name="ST">SearchTarget</param>
        /// <param name="Location">Location</param>
        /// <param name="ResponseList">ArrayList</param>
        public void ContainsSearchTarget(String ST, string Location, ArrayList ResponseList)
        {
            HTTPMessage msg;

            if (ST == "ssdp:all")
            {
                msg = new HTTPMessage();
                msg.StatusCode = 200;
                msg.StatusData = "OK";
                msg.AddTag("ST", "uuid:" + UniqueDeviceName);
                msg.AddTag("USN", "uuid:" + UniqueDeviceName);
                msg.AddTag("Location", Location);
                msg.AddTag("Server", "Windows NT/5.0, UPnP/1.0");
                msg.AddTag("EXT", "");
                msg.AddTag("Cache-Control", "max-age=" + ExpirationTimeout.ToString());
                ResponseList.Add(msg);

                msg = new HTTPMessage();
                msg.StatusCode = 200;
                msg.StatusData = "OK";
                msg.AddTag("ST", DeviceURN);
                msg.AddTag("USN", "uuid:" + UniqueDeviceName + "::" + DeviceURN);
                msg.AddTag("Location", Location);
                msg.AddTag("Server", "Windows NT/5.0, UPnP/1.0");
                msg.AddTag("EXT", "");
                msg.AddTag("Cache-Control", "max-age=" + ExpirationTimeout.ToString());
                ResponseList.Add(msg);
            }

            if (("uuid:" + UniqueDeviceName) == ST)
            {
                msg = new HTTPMessage();
                msg.StatusCode = 200;
                msg.StatusData = "OK";
                msg.AddTag("ST", ST);
                msg.AddTag("USN", "uuid:" + UniqueDeviceName);
                msg.AddTag("Location", Location);
                msg.AddTag("Server", "Windows NT/5.0, UPnP/1.0");
                msg.AddTag("EXT", "");
                msg.AddTag("Cache-Control", "max-age=" + ExpirationTimeout.ToString());
                ResponseList.Add(msg);
            }

            if (DeviceURN == ST)
            {
                msg = new HTTPMessage();
                msg.StatusCode = 200;
                msg.StatusData = "OK";
                msg.AddTag("ST", DeviceURN);
                msg.AddTag("USN", "uuid:" + UniqueDeviceName + "::" + DeviceURN);
                msg.AddTag("Location", Location);
                msg.AddTag("Server", "Windows NT/5.0, UPnP/1.0");
                msg.AddTag("EXT", "");
                msg.AddTag("Cache-Control", "max-age=" + ExpirationTimeout.ToString());
                ResponseList.Add(msg);
            }

            foreach (UPnPService s in Services)
            {
                if (ST == "ssdp:all")
                {
                    msg = new HTTPMessage();
                    msg.StatusCode = 200;
                    msg.StatusData = "OK";
                    msg.AddTag("ST", s.ServiceURN);
                    msg.AddTag("USN", "uuid:" + UniqueDeviceName + "::" + s.ServiceURN);
                    msg.AddTag("Location", Location);
                    msg.AddTag("Server", "Windows NT/5.0, UPnP/1.0");
                    msg.AddTag("EXT", "");
                    msg.AddTag("Cache-Control", "max-age=" + ExpirationTimeout.ToString());
                    ResponseList.Add(msg);
                }
                if (s.ServiceURN == ST)
                {
                    msg = new HTTPMessage();
                    msg.StatusCode = 200;
                    msg.StatusData = "OK";
                    msg.AddTag("ST", s.ServiceURN);
                    msg.AddTag("USN", "uuid:" + UniqueDeviceName + "::" + s.ServiceURN);
                    msg.AddTag("Location", Location);
                    msg.AddTag("Server", "Windows NT/5.0, UPnP/1.0");
                    msg.AddTag("EXT", "");
                    msg.AddTag("Cache-Control", "max-age=" + ExpirationTimeout.ToString());
                    ResponseList.Add(msg);
                }
            }

            foreach (UPnPDevice d in this.EmbeddedDevices)
            {
                d.ContainsSearchTarget(ST, Location, ResponseList);
            }
        }
        /// <summary>
        /// Retreives the XML for this UPnPDevice Container
        /// </summary>
        /// <returns></returns>
        public byte[] GetRootDeviceXML(IPEndPoint local)
        {
            MemoryStream mstream = new MemoryStream();
            XmlTextWriter XDoc = new XmlTextWriter(mstream, System.Text.Encoding.UTF8);
            XDoc.Formatting = System.Xml.Formatting.Indented;
            XDoc.Indentation = 3;

            XDoc.WriteStartDocument();
            XDoc.WriteStartElement("root", "urn:schemas-upnp-org:device-1-0");
            if (_BootID != "")
            {
                XDoc.WriteAttributeString("configId", this._BootID);
            }

            XDoc.WriteStartElement("specVersion");
            XDoc.WriteElementString("major", Arch_Major.ToString());
            XDoc.WriteElementString("minor", Arch_Minor.ToString());
            XDoc.WriteEndElement();

            // This element is not needed anymore
            //XDoc.WriteElementString("URLBase","http://" + local.Address.ToString() + ":" + local.Port.ToString() + "/");
            GetNonRootDeviceXML(local, XDoc);

            XDoc.WriteEndElement();
            XDoc.WriteEndDocument();
            XDoc.Flush();

            byte[] RetVal = new byte[mstream.Length - 3];
            mstream.Seek(3, SeekOrigin.Begin);
            mstream.Read(RetVal, 0, RetVal.Length);
            XDoc.Close();
            return (RetVal);
        }
        private void GetNonRootDeviceXML(IPEndPoint local, XmlTextWriter XDoc)
        {
            IDictionaryEnumerator de = CustomField.GetEnumerator();
            DText pp = new DText(); ;
            pp.ATTRMARK = ":";

            XDoc.WriteStartElement("device");

            //
            // Always advertise version :1
            //
            XDoc.WriteElementString("deviceType", DeviceURN);


            if (HasPresentation == true)
            {
                XDoc.WriteElementString("presentationURL", PresentationURL);
            }

            while (de.MoveNext())
            {
                IDictionaryEnumerator ede = ((Hashtable)de.Value).GetEnumerator();
                while (ede.MoveNext())
                {
                    string localName = (string)ede.Key;
                    string elementValue = (string)ede.Value;
                    string ns = (string)de.Key;

                    pp[0] = localName;
                    if (pp.DCOUNT() == 2)
                    {
                        XDoc.WriteStartElement(pp[1], pp[2], ns);
                        XDoc.WriteString(elementValue);
                        XDoc.WriteEndElement();
                    }
                    else
                    {
                        if (ns != "")
                        {
                            XDoc.WriteElementString(localName, ns, elementValue);
                        }
                        else
                        {
                            XDoc.WriteElementString(localName, elementValue);
                        }
                    }
                }
            }

            XDoc.WriteElementString("friendlyName", FriendlyName);
            if (Manufacturer != null) XDoc.WriteElementString("manufacturer", Manufacturer);
            if (ManufacturerURL != null) XDoc.WriteElementString("manufacturerURL", ManufacturerURL);
            if (ModelDescription != null) XDoc.WriteElementString("modelDescription", ModelDescription);
            if (ModelName != null) XDoc.WriteElementString("modelName", ModelName);
            if (ModelNumber != null) XDoc.WriteElementString("modelNumber", ModelNumber);
            if (ModelURL != null) XDoc.WriteElementString("modelURL", HTTPMessage.UnEscapeString(ModelURL.AbsoluteUri));
            if (SerialNumber != null) XDoc.WriteElementString("serialNumber", SerialNumber);
            XDoc.WriteElementString("UDN", "uuid:" + UniqueDeviceName);

            if (_icon != null)
            {
                lock (_icon)
                {
                    XDoc.WriteStartElement("iconList");
                    XDoc.WriteStartElement("icon");
                    XDoc.WriteElementString("mimetype", "image/png");
                    XDoc.WriteElementString("width", _icon.Width.ToString());
                    XDoc.WriteElementString("height", _icon.Height.ToString());
                    XDoc.WriteElementString("depth", System.Drawing.Image.GetPixelFormatSize(_icon.PixelFormat).ToString());
                    XDoc.WriteElementString("url", "/icon.png");
                    XDoc.WriteEndElement();

                    XDoc.WriteStartElement("icon");
                    XDoc.WriteElementString("mimetype", "image/jpg");
                    XDoc.WriteElementString("width", _icon.Width.ToString());
                    XDoc.WriteElementString("height", _icon.Height.ToString());
                    XDoc.WriteElementString("depth", System.Drawing.Image.GetPixelFormatSize(_icon.PixelFormat).ToString());
                    XDoc.WriteElementString("url", "/icon.jpg");
                    XDoc.WriteEndElement();

                    if (_icon2 != null)
                    {
                        XDoc.WriteStartElement("icon");
                        XDoc.WriteElementString("mimetype", "image/png");
                        XDoc.WriteElementString("width", _icon2.Width.ToString());
                        XDoc.WriteElementString("height", _icon2.Height.ToString());
                        XDoc.WriteElementString("depth", System.Drawing.Image.GetPixelFormatSize(_icon.PixelFormat).ToString());
                        XDoc.WriteElementString("url", "/icon2.png");
                        XDoc.WriteEndElement();

                        XDoc.WriteStartElement("icon");
                        XDoc.WriteElementString("mimetype", "image/jpg");
                        XDoc.WriteElementString("width", _icon2.Width.ToString());
                        XDoc.WriteElementString("height", _icon2.Height.ToString());
                        XDoc.WriteElementString("depth", System.Drawing.Image.GetPixelFormatSize(_icon2.PixelFormat).ToString());
                        XDoc.WriteElementString("url", "/icon2.jpg");
                        XDoc.WriteEndElement();
                    }

                    XDoc.WriteEndElement();
                }
            }
            if (Services.Length > 0)
            {
                XDoc.WriteStartElement("serviceList");
                for (int sid = 0; sid < Services.Length; ++sid)
                {
                    Services[sid].GetServiceXML(XDoc);
                }
                XDoc.WriteEndElement();
            }

            if (EmbeddedDevices.Length > 0)
            {
                XDoc.WriteStartElement("deviceList");
                for (int ei = 0; ei < EmbeddedDevices.Length; ++ei)
                {
                    EmbeddedDevices[ei].GetNonRootDeviceXML(local, XDoc);
                }
                XDoc.WriteEndElement();
            }

            XDoc.WriteEndElement();
        }

        private string RemoveIPv6Scope(string addr)
        {
            int i = addr.IndexOf('%');
            if (i >= 0) addr = addr.Substring(0, i);
            return addr;
        }

        public static bool TestDeviceParsing(string filename)
        {
            FileStream f = new FileStream(filename, FileMode.Open, FileAccess.Read);
            byte[] b = new byte[(int)f.Length];
            f.Read(b, 0, b.Length);
            string s = System.Text.UTF8Encoding.UTF8.GetString(b);
            UPnPDevice d = new UPnPDevice();
            d.BaseURL = new Uri("http://127.0.0.1/");
            UPnPDevice.ParseDevice(s, 0, ref d);
            return true;
        }
    }
}
