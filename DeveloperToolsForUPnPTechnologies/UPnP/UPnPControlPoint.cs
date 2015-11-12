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
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Collections;

namespace OpenSource.UPnP
{
    /// <summary>
    /// The object by which you can leverage UPnPDevices on your network.
    /// </summary>
    public sealed class UPnPControlPoint
    {
        private static int _mx = 5;
        public static int MX
        {
            get { return (_mx); }
            set { if (value > 0) _mx = value; }
        }
        public delegate void SearchHandler(IPEndPoint ResponseFromEndPoint, IPEndPoint ResponseReceivedOnEndPoint, Uri DescriptionLocation, String USN, String SearchTarget, int MaxAge);
        /// <summary>
        /// Triggered when an AsyncSearch result returns
        /// </summary>
        public event SearchHandler OnSearch;

        public delegate void CreateDeviceHandler(UPnPDevice Device, Uri DescriptionURL);
        /// <summary>
        /// Triggered when an Async CreateDevice returns...
        /// <para>
        /// Depracated. Use UPnPDeviceFactory</para>
        /// </summary>
        public event CreateDeviceHandler OnCreateDevice;

        private NetworkInfo NetInfo;
        private ArrayList SyncData;
        private DeviceNode SyncDevice;
        private Hashtable CreateTable;
        private SSDP SSDPServer;
        private Hashtable SSDPSessions;
        private LifeTimeMonitor Lifetime;

        private struct DeviceNode
        {
            public UPnPDevice TheDevice;
            public Uri URL;
        }

        /// <summary>
        /// This event is triggered when other UPnPDevices on your network are alive.
        /// </summary>
        public event SSDP.NotifyHandler OnNotify;

        /// <summary>
        /// Constructs a new Control Point, and waits for your commands and receives events
        /// </summary>
        public UPnPControlPoint()
        {
            CreateTable = Hashtable.Synchronized(new Hashtable());
            NetInfo = new NetworkInfo(new NetworkInfo.InterfaceHandler(NewInterface));
            SyncData = ArrayList.Synchronized(new ArrayList());
            SSDPSessions = Hashtable.Synchronized(new Hashtable());
            Lifetime = new LifeTimeMonitor();
            Lifetime.OnExpired += new LifeTimeMonitor.LifeTimeHandler(HandleExpired);

            SSDPServer = new SSDP(65535);
            SSDPServer.OnNotify += new SSDP.NotifyHandler(HandleNotify);
        }

        public UPnPControlPoint(NetworkInfo ni)
        {
            CreateTable = Hashtable.Synchronized(new Hashtable());
            NetInfo = ni;
            SyncData = ArrayList.Synchronized(new ArrayList());
            SSDPSessions = Hashtable.Synchronized(new Hashtable());
            Lifetime = new LifeTimeMonitor();
            Lifetime.OnExpired += new LifeTimeMonitor.LifeTimeHandler(HandleExpired);

            SSDPServer = new SSDP(65535);
            SSDPServer.OnNotify += new SSDP.NotifyHandler(HandleNotify);
        }

        ~UPnPControlPoint()
        {
        }

        private void PreProcessNotify(IPEndPoint source, String LocationURL, bool IsAlive, String USN, String ST, int MaxAge)
        {

        }

        private void HandleAlert(Object sender, Object RObj)
        {

        }

        private void NewInterface(NetworkInfo sender, IPAddress Intfce)
        {
        }

        private void HandleNotify(IPEndPoint source, IPEndPoint local, Uri LocationURL, bool IsAlive, String USN, String ST, int MaxAge, HTTPMessage Packet)
        {
            if (IsAlive && LocationURL != null) OpenSource.Utilities.EventLogger.Log(this, System.Diagnostics.EventLogEntryType.SuccessAudit, LocationURL.ToString());
            if (OnNotify != null) OnNotify(source, local, LocationURL, IsAlive, USN, ST, MaxAge, Packet);
        }

        /// <summary>
        /// Creates a device from a URL. [Depracated] use UPnPDeviceFactory
        /// </summary>
        /// <param name="DescriptionURL"></param>
        /// <param name="LifeTime"></param>
        public void CreateDeviceAsync(Uri DescriptionURL, int LifeTime)
        {
            //ToDo: Replace the failed callback
            UPnPDeviceFactory fac = new UPnPDeviceFactory(DescriptionURL, LifeTime, new UPnPDeviceFactory.UPnPDeviceHandler(HandleDeviceCreation), null, null, null);
            CreateTable[fac] = fac;
            Lifetime.Add(fac, 30);
        }

        private void HandleDeviceCreation(UPnPDeviceFactory Factory, UPnPDevice device, Uri URL)
        {
            Factory.Shutdown();
            if (OnCreateDevice != null)
            {
                OnCreateDevice(device, URL);
            }
        }

        private void CreateSyncCallback(UPnPDevice Device, Uri URL)
        {
            SyncDevice.TheDevice = Device;
            SyncDevice.URL = URL;
        }

        private void HandleExpired(LifeTimeMonitor sender, object Obj)
        {
            if (Obj.GetType().FullName == "OpenSource.UPnP.UPnPDeviceFactory")
            {
                ((UPnPDeviceFactory)Obj).Shutdown();
                CreateTable.Remove(Obj);
            }
        }

        public void FindDeviceAsync(String SearchTarget)
        {
            FindDeviceAsync(SearchTarget, Utils.UpnpMulticastV4EndPoint);
            FindDeviceAsync(SearchTarget, Utils.UpnpMulticastV6EndPoint1);
            FindDeviceAsync(SearchTarget, Utils.UpnpMulticastV6EndPoint2);
        }

        public void FindDeviceAsync(String SearchTarget, IPAddress RemoteAddress)
        {
            FindDeviceAsync(SearchTarget, Utils.UpnpMulticastV4EndPoint);
        }

        /// <summary>
        /// Searches for a SearchTarget Asynchronously
        /// </summary>
        /// <param name="SearchTarget">The Target</param>
        public void FindDeviceAsync(String SearchTarget, IPEndPoint RemoteEP)
        {
            HTTPMessage request = new HTTPMessage();
            request.Directive = "M-SEARCH";
            request.DirectiveObj = "*";
            request.AddTag("ST", SearchTarget);
            request.AddTag("MX", MX.ToString());
            request.AddTag("MAN", "\"ssdp:discover\"");
            if (RemoteEP.AddressFamily == AddressFamily.InterNetwork) request.AddTag("HOST", RemoteEP.ToString()); // "239.255.255.250:1900"
            if (RemoteEP.AddressFamily == AddressFamily.InterNetworkV6) request.AddTag("HOST", string.Format("[{0}]:{1}", RemoteEP.Address.ToString(), RemoteEP.Port)); // "[FF05::C]:1900"
            byte[] buffer = UTF8Encoding.UTF8.GetBytes(request.StringPacket);

            IPAddress[] LocalAddresses = NetInfo.GetLocalAddresses();

            foreach (IPAddress localaddr in LocalAddresses)
            {
                try
                {
                    UdpClient session = (UdpClient)SSDPSessions[localaddr];
                    if (session == null)
                    {
                        session = new UdpClient(new IPEndPoint(localaddr, 0));
                        session.EnableBroadcast = true;
                        session.BeginReceive(new AsyncCallback(OnReceiveSink), session);
                        SSDPSessions[localaddr] = session;
                    }
                    if (RemoteEP.AddressFamily != session.Client.AddressFamily) continue;
                    if ((RemoteEP.AddressFamily == AddressFamily.InterNetworkV6) && ((IPEndPoint)session.Client.LocalEndPoint).Address.IsIPv6LinkLocal == true && RemoteEP != Utils.UpnpMulticastV6EndPoint2) continue;
                    if ((RemoteEP.AddressFamily == AddressFamily.InterNetworkV6) && ((IPEndPoint)session.Client.LocalEndPoint).Address.IsIPv6LinkLocal == false && RemoteEP != Utils.UpnpMulticastV6EndPoint1) continue;

                    IPEndPoint lep = (IPEndPoint)session.Client.LocalEndPoint;
                    if (session.Client.AddressFamily == AddressFamily.InterNetwork)
                    {
                        session.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, localaddr.GetAddressBytes());
                    }
                    else if (session.Client.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        session.Client.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.MulticastInterface, BitConverter.GetBytes((int)localaddr.ScopeId));
                    }

                    session.Send(buffer, buffer.Length, RemoteEP);
                    session.Send(buffer, buffer.Length, RemoteEP);
                }
                catch (Exception ex)
                {
                    OpenSource.Utilities.EventLogger.Log(this, System.Diagnostics.EventLogEntryType.Error, "CP Failure: " + localaddr.ToString());
                    OpenSource.Utilities.EventLogger.Log(ex);
                }
            }
        }

        public void OnReceiveSink(IAsyncResult ar)
        {
            IPEndPoint ep = null;
            UdpClient client = (UdpClient)ar.AsyncState;
            try
            {
                byte[] buf = client.EndReceive(ar, ref ep);
                if (buf != null)
                {
                    OnReceiveSink2(buf, ep, (IPEndPoint)client.Client.LocalEndPoint);
                    client.BeginReceive(new AsyncCallback(OnReceiveSink), client);
                    return;
                }
            }
            catch (Exception ex) 
            {
                OpenSource.Utilities.EventLogger.Log(ex);
            }
            IPEndPoint local = (IPEndPoint)client.Client.LocalEndPoint;
            SSDPSessions.Remove(local.Address);
        }

        private void OnReceiveSink2(byte[] buffer, IPEndPoint remote, IPEndPoint local)
        {
            HTTPMessage msg;

            try
            {
                msg = HTTPMessage.ParseByteArray(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                OpenSource.Utilities.EventLogger.Log(ex);
                msg = new HTTPMessage();
                msg.Directive = "---";
                msg.DirectiveObj = "---";
                msg.BodyBuffer = buffer;
            }
            msg.LocalEndPoint = local;
            msg.RemoteEndPoint = remote;

            DText parser = new DText();

            String Location = msg.GetTag("Location");
            int MaxAge = 0;
            String ma = msg.GetTag("Cache-Control").Trim();
            if (ma != "")
            {
                parser.ATTRMARK = ",";
                parser.MULTMARK = "=";
                parser[0] = ma;
                for (int i = 1; i <= parser.DCOUNT(); ++i)
                {
                    if (parser[i, 1].Trim().ToUpper() == "MAX-AGE")
                    {
                        MaxAge = int.Parse(parser[i, 2].Trim());
                        break;
                    }
                }
            }
            ma = msg.GetTag("USN");
            String USN = ma.Substring(ma.IndexOf(":") + 1);
            String ST = msg.GetTag("ST");
            if (USN.IndexOf("::") != -1) USN = USN.Substring(0, USN.IndexOf("::"));
            OpenSource.Utilities.EventLogger.Log(this, System.Diagnostics.EventLogEntryType.SuccessAudit, msg.RemoteEndPoint.ToString());
            if (OnSearch != null) OnSearch(msg.RemoteEndPoint, msg.LocalEndPoint, new Uri(Location), USN, ST, MaxAge);
        }


    }
}
