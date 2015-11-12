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
using System.Threading;
using System.Collections;
using OpenSource.Utilities;

namespace OpenSource.UPnP
{
    /// <summary>
    /// This class implements the real functionality of the UPnP Smart Control Point
    /// System. For performance raisons, we want all the instances of the Smart
    /// Control Point classes to be connected and re-use information to avoid
    /// as much un-necessary UPnP network traffic as possible. This internal class
    /// is only instanciated once and does all the work on behalf of all of the
    /// Smart Control Point objects.
    /// </summary>
    internal sealed class UPnPInternalSmartControlPoint
    {
        private UPnPControlPoint genericControlPoint;
        private NetworkInfo hostNetworkInfo;
        private Hashtable deviceTable = new Hashtable();
        private object deviceTableLock = new object();
        private ArrayList activeDeviceList = ArrayList.Synchronized(new ArrayList());

        private LifeTimeMonitor deviceLifeTimeClock = new LifeTimeMonitor();
        private LifeTimeMonitor deviceUpdateClock = new LifeTimeMonitor();
        private UPnPDeviceFactory deviceFactory = new UPnPDeviceFactory();

        public delegate void DeviceHandler(UPnPInternalSmartControlPoint sender, UPnPDevice Device);
        private WeakEvent OnDeviceExpiredEvent = new WeakEvent();
        private WeakEvent OnAddedDeviceEvent = new WeakEvent();
        private WeakEvent OnRemovedDeviceEvent = new WeakEvent();
        private WeakEvent OnUpdatedDeviceEvent = new WeakEvent();

        public event DeviceHandler OnDeviceExpired
        {
            add { OnDeviceExpiredEvent.Register(value); }
            remove { OnDeviceExpiredEvent.UnRegister(value); }
        }
        public event DeviceHandler OnAddedDevice
        {
            add { OnAddedDeviceEvent.Register(value); }
            remove { OnAddedDeviceEvent.UnRegister(value); }
        }
        public event DeviceHandler OnRemovedDevice
        {
            add { OnRemovedDeviceEvent.Register(value); }
            remove { OnRemovedDeviceEvent.UnRegister(value); }
        }

        public event DeviceHandler OnUpdatedDevice
        {
            add { OnUpdatedDeviceEvent.Register(value); }
            remove { OnUpdatedDeviceEvent.UnRegister(value); }
        }

        private struct DeviceInfo
        {
            public UPnPDevice Device;
            public DateTime NotifyTime;
            public string UDN;

            public Uri BaseURL;
            public int MaxAge;
            public IPEndPoint LocalEP;
            public IPEndPoint SourceEP;

            public Uri PendingBaseURL;
            public int PendingMaxAge;
            public IPEndPoint PendingLocalEP;
            public IPEndPoint PendingSourceEP;
        }

        public UPnPInternalSmartControlPoint()
        {
            deviceFactory.OnDevice += new UPnPDeviceFactory.UPnPDeviceHandler(DeviceFactoryCreationSink);
            deviceFactory.OnFailed += new UPnPDeviceFactory.UPnPDeviceFailedHandler(DeviceFactoryFailedSink);
            deviceLifeTimeClock.OnExpired += new LifeTimeMonitor.LifeTimeHandler(DeviceLifeTimeClockSink);
            deviceUpdateClock.OnExpired += new LifeTimeMonitor.LifeTimeHandler(DeviceUpdateClockSink);

            hostNetworkInfo = new NetworkInfo(new NetworkInfo.InterfaceHandler(NetworkInfoNewInterfaceSink));
            hostNetworkInfo.OnInterfaceDisabled += new NetworkInfo.InterfaceHandler(NetworkInfoOldInterfaceSink);

            // Launch a search for all devices and start populating the
            // internal smart control point device list.
            genericControlPoint = new UPnPControlPoint();
            genericControlPoint.OnSearch += new UPnPControlPoint.SearchHandler(UPnPControlPointSearchSink);
            genericControlPoint.OnNotify += new SSDP.NotifyHandler(SSDPNotifySink);

            genericControlPoint.FindDeviceAsync("upnp:rootdevice");
        }

        public void Rescan()
        {
            ArrayList l;
            // We have to copy the list of keys here because we have reports of the hashtable being changed even when the lock is applied;
            lock (deviceTableLock)
            {
                l = new ArrayList(deviceTable.Keys);
            }
            foreach (string USN in l) deviceLifeTimeClock.Add(USN, 20);
            genericControlPoint.FindDeviceAsync("upnp:rootdevice");
        }

        public void UnicastSearch(IPAddress RemoteAddress)
        {
            genericControlPoint.FindDeviceAsync("upnp:rootdevice", RemoteAddress);
        }

        private void NetworkInfoNewInterfaceSink(NetworkInfo sender, IPAddress Intfce)
        {
            if (genericControlPoint != null) genericControlPoint.FindDeviceAsync("upnp:rootdevice");
        }
        private void NetworkInfoOldInterfaceSink(NetworkInfo sender, IPAddress Intfce)
        {
            UPnPDevice[] listing;
            ArrayList TempList = new ArrayList();

            lock (deviceTableLock)
            {
                listing = GetCurrentDevices();
                foreach (UPnPDevice d in listing)
                {
                    if (d.InterfaceToHost.Equals(Intfce))
                    {
                        TempList.Add(UnprotectedRemoveMe(d));
                    }
                }
            }

            foreach (UPnPDevice d in TempList)
            {
                d.Removed();
                OnRemovedDeviceEvent.Fire(this, d);
            }

            genericControlPoint.FindDeviceAsync("upnp:rootdevice");
        }

        public UPnPDevice[] GetCurrentDevices()
        {
            return (UPnPDevice[])activeDeviceList.ToArray(typeof(UPnPDevice));
        }

        /// <summary>
        /// Triggered when a SSDP search result is received
        /// </summary>
        private void UPnPControlPointSearchSink(IPEndPoint source, IPEndPoint local, Uri LocationURL, String USN, String SearchTarget, int MaxAge)
        {
            // A bit like getting a SSDP notification, but we don't do automatic
            // source change in this case. The only valid scenario of a search
            // result is device creation.
            lock (deviceTableLock)
            {
                if (deviceTable.ContainsKey(USN) == false)
                {
                    // Never saw this device before
                    DeviceInfo deviceInfo = new DeviceInfo();
                    deviceInfo.Device = null;
                    deviceInfo.UDN = USN;
                    deviceInfo.NotifyTime = DateTime.Now;
                    deviceInfo.BaseURL = LocationURL;
                    deviceInfo.MaxAge = MaxAge;
                    deviceInfo.LocalEP = local;
                    deviceInfo.SourceEP = source;
                    deviceTable[USN] = deviceInfo;
                    deviceFactory.CreateDevice(deviceInfo.BaseURL, deviceInfo.MaxAge, local.Address, USN);
                }
                else
                {
                    DeviceInfo deviceInfo = (DeviceInfo)deviceTable[USN];
                    if (deviceInfo.Device != null) // If the device is in creation mode, do nothing
                    {
                        if (deviceInfo.BaseURL.Equals(LocationURL))
                        {
                            // Cancel a possible source change
                            deviceUpdateClock.Remove(deviceInfo);
                            deviceInfo.PendingBaseURL = null;
                            deviceInfo.PendingMaxAge = 0;
                            deviceInfo.PendingLocalEP = null;
                            deviceInfo.PendingSourceEP = null;
                            // Then simply update the lifetime
                            deviceInfo.NotifyTime = DateTime.Now;
                            deviceTable[USN] = deviceInfo;
                            deviceLifeTimeClock.Add(deviceInfo.UDN, MaxAge);
                        }
                        else
                        {
                            // Wow, same device, different source - Check timing
                            if (deviceInfo.NotifyTime.AddSeconds(10).Ticks < DateTime.Now.Ticks)
                            {
                                // This is a possible source change. Wait for 3 seconds and make the switch.
                                deviceInfo.PendingBaseURL = LocationURL;
                                deviceInfo.PendingMaxAge = MaxAge;
                                deviceInfo.PendingLocalEP = local;
                                deviceInfo.PendingSourceEP = source;
                                deviceUpdateClock.Add(deviceInfo.UDN, 3);
                            }
                        }
                    }
                }
            }
        }

        internal UPnPDevice UnprotectedRemoveMe(string UDN)
        {
            DeviceInfo deviceInfo;
            UPnPDevice removedDevice = null;

            if (deviceTable.ContainsKey(UDN))
            {
                deviceInfo = (DeviceInfo)deviceTable[UDN];
                removedDevice = deviceInfo.Device;
                deviceTable.Remove(UDN);
                deviceLifeTimeClock.Remove(deviceInfo.UDN);
                deviceUpdateClock.Remove(deviceInfo);
                activeDeviceList.Remove(removedDevice);
            }

            return (removedDevice);
        }
        internal UPnPDevice UnprotectedRemoveMe(UPnPDevice _d)
        {
            UPnPDevice d = _d;
            while (d.ParentDevice != null)
            {
                d = d.ParentDevice;
            }

            return (UnprotectedRemoveMe(d.UniqueDeviceName));
        }
        internal void RemoveMe(UPnPDevice _d)
        {
            UPnPDevice d = _d;
            UPnPDevice removedDevice = null;
            while (d.ParentDevice != null)
            {
                d = d.ParentDevice;
            }
            lock (deviceTableLock)
            {
                if (deviceTable.ContainsKey(d.UniqueDeviceName) == false)
                {
                    return;
                }
                removedDevice = UnprotectedRemoveMe(d);
            }
            if (removedDevice != null)
            {
                removedDevice.Removed();
            }
            if (removedDevice != null)
            {
                OnRemovedDeviceEvent.Fire(this, removedDevice);
            }
        }
        /// <summary>
        /// Triggered when a SSDP notification is received
        /// </summary>
        internal void SSDPNotifySink(IPEndPoint source, IPEndPoint local, Uri LocationURL, bool IsAlive, String USN, String SearchTarget, int MaxAge, HTTPMessage Packet)
        {
            UPnPDevice removedDevice = null;
            // Simple ignore everything that is not root
            if (SearchTarget != "upnp:rootdevice") return;

            if (IsAlive == false)
            {
                // The easy part first... we got a SSDP BYE message
                // Remove the device completely no matter what state it is in
                // right now. Also clear all clocks.
                lock (deviceTableLock)
                {
                    removedDevice = UnprotectedRemoveMe(USN);
                }
                if (removedDevice != null)
                {
                    removedDevice.Removed();
                    OnRemovedDeviceEvent.Fire(this, removedDevice);
                }
            }
            else
            {
                lock (deviceTableLock)
                {
                    // Ok, This device is annoncing itself.
                    if (deviceTable.ContainsKey(USN) == false)
                    {
                        // Never saw this device before
                        DeviceInfo deviceInfo = new DeviceInfo();
                        deviceInfo.Device = null;
                        deviceInfo.UDN = USN;
                        deviceInfo.NotifyTime = DateTime.Now;
                        deviceInfo.BaseURL = LocationURL;
                        deviceInfo.MaxAge = MaxAge;
                        deviceInfo.LocalEP = local;
                        deviceInfo.SourceEP = source;
                        deviceTable[USN] = deviceInfo;
                        deviceFactory.CreateDevice(deviceInfo.BaseURL, deviceInfo.MaxAge, local.Address, USN); // TODO: Does URI construction work all this time??
                    }
                    else
                    {
                        // We already know about this device, lets check it out

                        DeviceInfo deviceInfo = (DeviceInfo)deviceTable[USN];
                        if (deviceInfo.Device != null) // If the device is in creation mode, do nothing
                        {
                            if (deviceInfo.BaseURL.Equals(LocationURL))
                            {
                                // Cancel a possible source change
                                deviceUpdateClock.Remove(deviceInfo);
                                deviceInfo.PendingBaseURL = null;
                                deviceInfo.PendingMaxAge = 0;
                                deviceInfo.PendingLocalEP = null;
                                deviceInfo.PendingSourceEP = null;
                                // Then simply update the lifetime
                                deviceInfo.NotifyTime = DateTime.Now;
                                deviceTable[USN] = deviceInfo;
                                deviceLifeTimeClock.Add(deviceInfo.UDN, MaxAge);
                            }
                            else
                            {
                                // Wow, same device, different source - Check timing
                                if (deviceInfo.NotifyTime.AddSeconds(10).Ticks < DateTime.Now.Ticks)
                                {
                                    // This is a possible source change. Wait for 3 seconds and make the switch.
                                    deviceInfo.PendingBaseURL = LocationURL;
                                    deviceInfo.PendingMaxAge = MaxAge;
                                    deviceInfo.PendingLocalEP = local;
                                    deviceInfo.PendingSourceEP = source;
                                    deviceTable[USN] = deviceInfo;
                                    deviceUpdateClock.Add(deviceInfo.UDN, 3);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Triggered when a device lifetime has expired
        /// </summary>
        /// <param name="Obj"></param>
        private void DeviceLifeTimeClockSink(LifeTimeMonitor sender, object obj)
        {
            DeviceInfo deviceInfo;
            lock (deviceTableLock)
            {
                if (deviceTable.ContainsKey(obj) == false)
                {
                    return;
                }
                deviceInfo = (DeviceInfo)deviceTable[obj];
                deviceTable.Remove(obj);
                deviceUpdateClock.Remove(obj);	// Cancel any source change
                if (activeDeviceList.Contains(deviceInfo.Device))
                {
                    activeDeviceList.Remove(deviceInfo.Device);
                }
                else
                {
                    deviceInfo.Device = null;	// Don't warn user about this, user does not know about device
                }
            }
            if (deviceInfo.Device != null)
            {
                deviceInfo.Device.Removed();
            }
            if (deviceInfo.Device != null)
            {
                deviceInfo.Device.Removed();
                OnDeviceExpiredEvent.Fire(this, deviceInfo.Device);
            }
        }

        /// <summary>
        /// Triggered when a device must be updated to a new source IP address
        /// </summary>
        /// <param name="Obj"></param>
        private void DeviceUpdateClockSink(LifeTimeMonitor sender, object obj)
        {
            // Make the source switch
            lock (deviceTableLock)
            {
                if (deviceTable.ContainsKey(obj) == false)
                {
                    return;
                }
                DeviceInfo deviceInfo = (DeviceInfo)deviceTable[obj];
                if (deviceInfo.PendingBaseURL == null)
                {
                    return;	// Cancel the switch
                }

                deviceInfo.BaseURL = deviceInfo.PendingBaseURL;
                deviceInfo.MaxAge = deviceInfo.PendingMaxAge;
                deviceInfo.SourceEP = deviceInfo.PendingSourceEP;
                deviceInfo.LocalEP = deviceInfo.PendingLocalEP;
                deviceInfo.NotifyTime = DateTime.Now;
                deviceInfo.Device.UpdateDevice(deviceInfo.BaseURL, deviceInfo.LocalEP.Address);
                deviceTable[obj] = deviceInfo;

                deviceLifeTimeClock.Add(deviceInfo.UDN, deviceInfo.MaxAge);
            }
            //if (OnUpdatedDevice != null) OnUpdatedDevice(this,deviceInfo.Device);
        }

        private void DeviceFactoryFailedSink(UPnPDeviceFactory sender, Uri URL, Exception e, string urn)
        {
            lock (deviceTableLock)
            {
                if (deviceTable.ContainsKey(urn)) deviceTable.Remove(urn);
            }
        }

        /// <summary>
        /// Triggered when a new UPnP device is created.
        /// </summary>
        private void DeviceFactoryCreationSink(UPnPDeviceFactory sender, UPnPDevice device, Uri locationURL)
        {
            // Hardening
            if (deviceTable.Contains(device.UniqueDeviceName) == false && deviceTable.Contains("FORCEDDEVICE") == false)
            {
                OpenSource.Utilities.EventLogger.Log(this, System.Diagnostics.EventLogEntryType.Error, "UPnPDevice[" + device.FriendlyName + "]@" + device.LocationURL + " advertised UDN[" + device.UniqueDeviceName + "] in xml but not in SSDP");
                return;
            }

            lock (deviceTableLock)
            {
                DeviceInfo deviceInfo;
                if (deviceTable.Contains(device.UniqueDeviceName) == false)
                {
                    // This must be the forced device
                    deviceInfo = (DeviceInfo)deviceTable["FORCEDDEVICE"];
                    deviceTable.Remove("FORCEDDEVICE");
                    deviceTable[device.UniqueDeviceName] = deviceInfo;
                }

                // Hardening - Creating a device we have should never happen.
                if (((DeviceInfo)deviceTable[device.UniqueDeviceName]).Device != null)
                {
                    OpenSource.Utilities.EventLogger.Log(this, System.Diagnostics.EventLogEntryType.Error, "Unexpected UPnP Device Creation: " + device.FriendlyName + "@" + device.LocationURL);
                    return;
                }

                // Lets update out state and notify the user.
                deviceInfo = (DeviceInfo)deviceTable[device.UniqueDeviceName];
                deviceInfo.Device = device;
                deviceTable[device.UniqueDeviceName] = deviceInfo;
                deviceLifeTimeClock.Add(device.UniqueDeviceName, device.ExpirationTimeout);
                activeDeviceList.Add(device);
            }
            OnAddedDeviceEvent.Fire(this, device);
        }

        public void ForceDeviceAddition(Uri url)
        {
            DeviceInfo deviceInfo = new DeviceInfo();
            deviceInfo.Device = null;
            deviceInfo.UDN = "FORCEDDEVICE";
            deviceInfo.NotifyTime = DateTime.Now;
            deviceInfo.BaseURL = url;
            deviceInfo.MaxAge = 1800;
            deviceInfo.LocalEP = null;
            deviceInfo.SourceEP = null;
            deviceTable["FORCEDDEVICE"] = deviceInfo;
            deviceFactory.CreateDevice(deviceInfo.BaseURL, deviceInfo.MaxAge, null, null);
        }

    }
}
