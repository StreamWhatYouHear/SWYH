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
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.Net.NetworkInformation;
using OpenSource.Utilities;

namespace OpenSource.UPnP
{
    /// <summary>
    /// This class monitors local IP addresses and sends events when a address
    /// is added or removed. Generally, this class should be created and
    /// GetLocalAddresses() should be used to query all currently available
    /// interfaces. Subsequent changes to the list will trigger events;
    /// </summary>
    public sealed class NetworkInfo
    {

        public static int NetworkPollSeconds = 15;
        private WeakEvent OnNewInterfaceEvent = new WeakEvent();
        private WeakEvent OnInterfaceDisabledEvent = new WeakEvent();
        public delegate void InterfaceHandler(NetworkInfo sender, IPAddress address);
        /// <summary>
        /// Triggered when a new Interface appears on the network
        /// </summary>
        /// 
        public event InterfaceHandler OnNewInterface
        {
            add
            {
                OnNewInterfaceEvent.Register(value);
            }
            remove
            {
                OnNewInterfaceEvent.UnRegister(value);
            }
        }
        /// <summary>
        /// Triggered when an interface goes down
        /// </summary>
        /// 
        public event InterfaceHandler OnInterfaceDisabled
        {
            add
            {
                OnInterfaceDisabledEvent.Register(value);
            }
            remove
            {
                OnInterfaceDisabledEvent.UnRegister(value);
            }
        }

        private LifeTimeMonitor InterfacePoller = new LifeTimeMonitor();
        //private String HostName;
        private ArrayList AddressTable = new ArrayList();


        /// <summary>
        /// Instantiates a new Monitor
        /// </summary>
        public NetworkInfo()
            : this(null)
        {
        }

        /// <summary>
        /// Instantiates a new Monitor
        /// </summary>
        /// <param name="onNewInterfaceSink">Interface Callback</param>
        public NetworkInfo(InterfaceHandler onNewInterfaceSink)
        {
            OpenSource.Utilities.InstanceTracker.Add(this);

            InterfacePoller.OnExpired += new LifeTimeMonitor.LifeTimeHandler(PollInterface);

            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();  
            foreach(NetworkInterface i in interfaces)
            {
                if (i.IsReceiveOnly == false && i.OperationalStatus == OperationalStatus.Up && i.SupportsMulticast == true)
                {
                    IPInterfaceProperties i2 = i.GetIPProperties();
                    foreach (UnicastIPAddressInformation i3 in i2.UnicastAddresses)
                    {
                        if (!AddressTable.Contains(i3.Address) && !i3.Address.Equals(IPAddress.IPv6Loopback)) { AddressTable.Add(i3.Address); }
                    }
                }
            }

            /*
            HostName = Dns.GetHostName();
            IPHostEntry HostInfo = Dns.GetHostEntry(HostName);
            AddressTable = new ArrayList(HostInfo.AddressList);
            */

            if (!AddressTable.Contains(IPAddress.Loopback))
            {
                AddressTable.Add(IPAddress.Loopback);
            }

            if (onNewInterfaceSink != null)
            {
                OnNewInterface += onNewInterfaceSink;
                foreach (IPAddress address in AddressTable)
                {
                    OnNewInterfaceEvent.Fire(this, address);
                }
            }

            InterfacePoller.Add(this, NetworkInfo.NetworkPollSeconds);
        }

        /// <summary>
        /// Retreve all IP addresses present on the local host.
        /// </summary>
        /// <returns>List of IP addresses present on the local host.</returns>
        public IPAddress[] GetLocalAddresses()
        {
            return ((IPAddress[])AddressTable.ToArray(typeof(IPAddress)));
        }

        /// <summary>
        /// Calledn every second to check for interface changes.
        /// </summary>
        private void PollInterface(LifeTimeMonitor sender, Object obj)
        {
            try
            {
                ArrayList CurrentAddressTable = new ArrayList();
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface i in interfaces)
                {
                    if (i.IsReceiveOnly == false && i.OperationalStatus == OperationalStatus.Up && i.SupportsMulticast == true)
                    {
                        IPInterfaceProperties i2 = i.GetIPProperties();
                        foreach (UnicastIPAddressInformation i3 in i2.UnicastAddresses)
                        {
                            if (!CurrentAddressTable.Contains(i3.Address) && !i3.Address.Equals(IPAddress.IPv6Loopback)) { CurrentAddressTable.Add(i3.Address); }
                        }
                    }
                }

                /*
                IPHostEntry HostInfo = Dns.GetHostEntry(HostName);
                ArrayList CurrentAddressTable = new ArrayList(HostInfo.AddressList);
                */

                ArrayList OldAddressTable = AddressTable;
                AddressTable = CurrentAddressTable;
                if (!AddressTable.Contains(IPAddress.Loopback))
                {
                    AddressTable.Add(IPAddress.Loopback);
                }

                foreach (IPAddress addr in CurrentAddressTable)
                {
                    if (OldAddressTable.Contains(addr) == false) OnNewInterfaceEvent.Fire(this, addr);
                }

                foreach (IPAddress addr in OldAddressTable)
                {
                    if (CurrentAddressTable.Contains(addr) == false) OnInterfaceDisabledEvent.Fire(this, addr);
                }

            }
            catch (Exception ex)
            {
                OpenSource.Utilities.EventLogger.Log(ex);
                //System.Windows.Forms.MessageBox.Show(ex.ToString(),"NetworkInfo");
            }
            InterfacePoller.Add(this, NetworkInfo.NetworkPollSeconds);
        }


        /// *** - This method should be eliminated if possible.
        /// <summary>
        /// Returns a random free port on a specific interface
        /// </summary>
        /// <param name="LowRange">The lower bound</param>
        /// <param name="UpperRange">The upper bound</param>
        /// <param name="OnThisIP">The interface</param>
        /// <returns></returns>
        static public int GetFreePort(int LowRange, int UpperRange, IPAddress OnThisIP)
        {
            Random NumGenerator = new Random();
            int TestPort;
            IPEndPoint TestEP;
            Socket TestSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //TestSocket.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.ExclusiveAddressUse,1);

            do
            {
                TestPort = NumGenerator.Next(LowRange, UpperRange);
                TestEP = new IPEndPoint(OnThisIP, TestPort);
                try
                {
                    TestSocket.Bind(TestEP);
                    break;
                }
                catch (Exception ex) 
                {
                    OpenSource.Utilities.EventLogger.Log(ex);
                }
            } while (true);

            TestSocket.Close();
            return TestPort;
        }

    }
}
