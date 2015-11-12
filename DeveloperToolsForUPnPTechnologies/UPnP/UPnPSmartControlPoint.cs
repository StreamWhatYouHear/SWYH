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
using System.Collections;
using System.Globalization;

namespace OpenSource.UPnP
{
    /// <summary>
    /// Enables the user to quickly & efficiently find and keep track of UPnP devices on the
    /// network. This class is internaly optimized to keep network traffic to a minimum while
    /// serving many users at once. This class will keep track of devices coming and going and
    /// devices that change IP address or change subnet. The class will also gather all the
    /// relevent UPnP device information prior to informing the user about the device.
    /// </summary>
    public sealed class UPnPSmartControlPoint
    {
        private bool MultiFilter = true;
        internal static UPnPInternalSmartControlPoint iSCP = new UPnPInternalSmartControlPoint();
        private string[] PartialMatchFilters = new string[1] { "upnp:rootdevice" };
        private double[] MinimumVersion = new double[1] { 1.0 };

        public delegate void DeviceHandler(UPnPSmartControlPoint sender, UPnPDevice device);
        public delegate void ServiceHandler(UPnPSmartControlPoint sender, UPnPService service);

        /// <summary>
        /// Triggered when a Device that passes the filter appears on the network
        /// <para>
        /// Also triggered when a device that contains objects that pass all the filters appears on the network. This only applies if more than one filter is passed.
        /// </para>
        /// </summary>
        public event DeviceHandler OnAddedDevice;
        /// <summary>
        /// Triggered when a Device that passes the filter disappears from the network
        /// </summary>
        public event DeviceHandler OnRemovedDevice;

        /// <summary>
        /// Triggered when a Service that passes the filter appears on the network
        /// </summary>
        public event ServiceHandler OnAddedService;
        /// <summary>
        /// Triggered when a Service that passes the filter disappears from the network
        /// </summary>
        public event ServiceHandler OnRemovedService;


        public void ForceDisposeDevice(UPnPDevice root)
        {
            while (root.ParentDevice != null)
            {
                root = root.ParentDevice;
            }
            iSCP.SSDPNotifySink(null, null, null, false, root.UniqueDeviceName, "upnp:rootdevice", 0, null);
        }


        /// <summary>
        /// Keep track of all UPnP devices on the network
        /// </summary>
        public UPnPSmartControlPoint()
            : this(null)
        {
        }

        /// <summary>
        /// Keep track of all UPnP devices on the network. The user can expect the OnAddedDeviceSink
        /// delegate to immidiatly be called for each device that is already known.
        /// </summary>
        /// <param name="OnAddedDeviceSink">Delegate called when a UPnP device is detected</param>
        public UPnPSmartControlPoint(DeviceHandler OnAddedDeviceSink)
            : this(OnAddedDeviceSink, "upnp:rootdevice")
        {
        }

        /// <summary>
        /// Keep track of all UPnP devices on the network. The user can expect the OnAddedDeviceSink
        /// delegate to immidiatly be called for each device that is already known.
        /// </summary>
        /// <param name="OnAddedDeviceSink"></param>
        /// <param name="DevicePartialMatchFilter"></param>
        public UPnPSmartControlPoint(DeviceHandler OnAddedDeviceSink, string DevicePartialMatchFilter)
            : this(OnAddedDeviceSink, null, DevicePartialMatchFilter)
        {
        }

        /// <summary>
        /// Keep track of all UPnP devices on the network. The user can expect the OnAddedDeviceSink or OnAddedServiceSink
        /// delegate to immidiatly be called for each device that is already known.
        /// <para>
        /// if multiple filters are supplied, the results will be that of the parent device which satisfies all the search criteria. 
        /// </para>
        /// </summary>
        /// <param name="OnAddedDeviceSink"></param>
        /// <param name="OnAddedServiceSink"></param>
        /// <param name="Filters">Array of strings, which represent the search criteria</param>
        public UPnPSmartControlPoint(DeviceHandler OnAddedDeviceSink, ServiceHandler OnAddedServiceSink, string[] Filters)
        {
            //MultiFilter = true;
            PartialMatchFilters = new String[Filters.Length];
            MinimumVersion = new double[Filters.Length];
            for (int i = 0; i < PartialMatchFilters.Length; ++i)
            {
                if (Filters[i].Length > 15 && Filters[i].Length > UPnPStringFormatter.GetURNPrefix(Filters[i]).Length)
                {
                    PartialMatchFilters[i] = UPnPStringFormatter.GetURNPrefix(Filters[i]);
                    try
                    {
                        MinimumVersion[i] = double.Parse(Filters[i].Substring(PartialMatchFilters[i].Length), new CultureInfo("en-US").NumberFormat);
                    }
                    catch
                    {
                        MinimumVersion[i] = 1.0;
                    }
                }
                else
                {
                    PartialMatchFilters[i] = Filters[i];
                    MinimumVersion[i] = 1.0;
                }

            }

            if (OnAddedDeviceSink != null) { this.OnAddedDevice += OnAddedDeviceSink; }
            if (OnAddedServiceSink != null) { this.OnAddedService += OnAddedServiceSink; }

            iSCP.OnAddedDevice += new UPnPInternalSmartControlPoint.DeviceHandler(HandleAddedDevice);
            iSCP.OnDeviceExpired += new UPnPInternalSmartControlPoint.DeviceHandler(HandleExpiredDevice);
            iSCP.OnRemovedDevice += new UPnPInternalSmartControlPoint.DeviceHandler(HandleRemovedDevice);
            iSCP.OnUpdatedDevice += new UPnPInternalSmartControlPoint.DeviceHandler(HandleUpdatedDevice);

            IEnumerator cdEN = iSCP.GetCurrentDevices().GetEnumerator();
            if ((OnAddedDeviceSink != null || OnAddedServiceSink != null) && cdEN != null)
            {
                while (cdEN.MoveNext()) { HandleAddedDevice(null, (UPnPDevice)cdEN.Current); }
            }
        }

        /// <summary>
        /// Keep track of all UPnP devices on the network. The user can expect the OnAddedDeviceSink
        /// delegate to immidiatly be called for each device that is already known that matches the
        /// filter.
        /// </summary>
        /// <param name="OnAddedDeviceSink">Delegate called when a UPnP device is detected that match the filter</param>
        /// <param name="DevicePartialMatchFilter">Sets the filter to UPnP devices that start with this string</param>
        public UPnPSmartControlPoint(DeviceHandler OnAddedDeviceSink, ServiceHandler OnAddedServiceSink, string DevicePartialMatchFilter)
            : this(OnAddedDeviceSink, OnAddedServiceSink, new string[1] { DevicePartialMatchFilter })
        {
            MultiFilter = false;
        }

        /// <summary>
        /// Rescans the network
        /// </summary>
        public void Rescan()
        {
            iSCP.Rescan();
        }

        public void UnicastSearch(IPAddress RemoteAddress)
        {
            iSCP.UnicastSearch(RemoteAddress);
        }

        /// <summary>
        /// An arraylist of Devices
        /// </summary>
        public ArrayList Devices
        {
            get
            {
                ArrayList dList = new ArrayList();
                ArrayList sList = new ArrayList();
                Hashtable h = new Hashtable();
                int filterIndex;

                object[] r;
                bool MatchAll = false;
                UPnPDevice[] d = iSCP.GetCurrentDevices();
                for (int i = 0; i < d.Length; ++i)
                {
                    MatchAll = true;
                    for (filterIndex = 0; filterIndex < PartialMatchFilters.Length; ++filterIndex)
                    {
                        string filter = PartialMatchFilters[filterIndex];
                        double Version = MinimumVersion[filterIndex];

                        if (CheckDeviceAgainstFilter(filter, Version, d[i], out r) == false)
                        {
                            MatchAll = false;
                            break;
                        }
                        else
                        {
                            foreach (object x in r)
                            {
                                if (x.GetType().FullName == "OpenSource.UPnP.UPnPDevice")
                                {
                                    dList.Add((UPnPDevice)x);
                                }
                                else
                                {
                                    sList.Add((UPnPService)x);
                                }
                            }
                        }
                    }

                    if (MatchAll == true)
                    {

                        foreach (UPnPDevice dev in dList)
                        {
                            bool _OK_ = true;
                            foreach (string filter in PartialMatchFilters)
                            {
                                if (dev.GetDevices(filter).Length == 0)
                                {
                                    if (dev.GetServices(filter).Length == 0)
                                    {
                                        _OK_ = false;
                                        break;
                                    }
                                }
                            }
                            if (_OK_ == true)
                            {
                                h[dev] = dev;
                            }
                        }

                    }

                }

                ArrayList a = new ArrayList();
                IDictionaryEnumerator ide = h.GetEnumerator();
                while (ide.MoveNext())
                {
                    a.Add(ide.Value);
                }
                return (a);
            }
        }

        /// <summary>
        /// Forward the OnAddedDevice event to the user.
        /// </summary>
        /// <param name="sender">UPnPInternalSmartControlPoint that sent the event</param>
        /// <param name="device">The UPnPDevice object that was added</param>
        private void HandleAddedDevice(UPnPInternalSmartControlPoint sender, UPnPDevice device)
        {
            if ((OnAddedDevice != null) || (OnAddedService != null))
            {
                object[] r;
                ArrayList dList = new ArrayList();
                ArrayList sList = new ArrayList();
                Hashtable h = new Hashtable();

                bool MatchAll = true;
                for (int filterIndex = 0; filterIndex < PartialMatchFilters.Length; ++filterIndex)
                {
                    string filter = PartialMatchFilters[filterIndex];
                    double Version = MinimumVersion[filterIndex];

                    if (CheckDeviceAgainstFilter(filter, Version, device, out r) == false)
                    {
                        MatchAll = false;
                        break;
                    }
                    else
                    {
                        foreach (object x in r)
                        {
                            if (x.GetType().FullName == "OpenSource.UPnP.UPnPDevice")
                            {
                                dList.Add((UPnPDevice)x);
                                if (PartialMatchFilters.Length == 1)
                                {
                                    if (OnAddedDevice != null)
                                    {
                                        OnAddedDevice(this, (UPnPDevice)x);
                                    }
                                }
                            }
                            else
                            {
                                sList.Add((UPnPService)x);
                                if (PartialMatchFilters.Length == 1)
                                {
                                    if (MultiFilter == false)
                                    {
                                        if (OnAddedService != null)
                                        {
                                            OnAddedService(this, (UPnPService)x);
                                        }
                                    }
                                    else
                                    {
                                        if (OnAddedDevice != null)
                                        {
                                            OnAddedDevice(this, ((UPnPService)x).ParentDevice);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (MatchAll == true)
                {
                    if (PartialMatchFilters.Length == 1)
                    {
                        return;
                    }
                    else
                    {
                        foreach (UPnPDevice dev in dList)
                        {
                            bool _OK_ = true;
                            foreach (string filter in PartialMatchFilters)
                            {
                                if (dev.GetDevices(filter).Length == 0)
                                {
                                    if (dev.GetServices(filter).Length == 0)
                                    {
                                        _OK_ = false;
                                        break;
                                    }
                                }
                            }
                            if (_OK_ == true)
                            {
                                h[dev] = dev;
                            }
                        }
                        foreach (UPnPService serv in sList)
                        {
                            bool _OK_ = true;
                            foreach (string filter in PartialMatchFilters)
                            {
                                if (serv.ParentDevice.GetServices(filter).Length == 0)
                                {
                                    _OK_ = false;
                                    break;
                                }
                            }
                            if (_OK_ == true)
                            {
                                if (h.ContainsKey(serv.ParentDevice) == false)
                                {
                                    h[serv.ParentDevice] = serv.ParentDevice;
                                }
                            }
                        }
                    }
                }

                IDictionaryEnumerator ide = h.GetEnumerator();
                while (ide.MoveNext())
                {
                    if (OnAddedDevice != null)
                    {
                        OnAddedDevice(this, (UPnPDevice)ide.Value);
                    }
                }
            }

        }

        /// <summary>
        /// Forward the OnUpdatedDevice event to the user.
        /// </summary>
        /// <param name="sender">UPnPInternalSmartControlPoint that sent the event</param>
        /// <param name="device">The UPnPDevice object that was updated</param>
        private void HandleUpdatedDevice(UPnPInternalSmartControlPoint sender, UPnPDevice device)
        {

        }

        /// <summary>
        /// Forward the OnRemovedDevice event to the user.
        /// </summary>
        /// <param name="sender">UPnPInternalSmartControlPoint that sent the event</param>
        /// <param name="device">The UPnPDevice object that was removed from the network</param>
        private void HandleRemovedDevice(UPnPInternalSmartControlPoint sender, UPnPDevice device)
        {
            if ((OnRemovedDevice != null) || (OnRemovedService != null))
            {
                object[] r;
                ArrayList dList = new ArrayList();
                ArrayList sList = new ArrayList();
                Hashtable h = new Hashtable();

                bool MatchAll = true;
                for (int filterIndex = 0; filterIndex < PartialMatchFilters.Length; ++filterIndex)
                {
                    string filter = PartialMatchFilters[filterIndex];
                    double Version = MinimumVersion[filterIndex];

                    if (CheckDeviceAgainstFilter(filter, Version, device, out r) == false)
                    {
                        MatchAll = false;
                        break;
                    }
                    else
                    {
                        foreach (object x in r)
                        {
                            if (x.GetType().FullName == "OpenSource.UPnP.UPnPDevice")
                            {
                                dList.Add((UPnPDevice)x);
                                if (PartialMatchFilters.Length == 1)
                                {
                                    if (OnRemovedDevice != null)
                                    {
                                        OnRemovedDevice(this, (UPnPDevice)x);
                                    }
                                }
                            }
                            else
                            {
                                sList.Add((UPnPService)x);
                                if (PartialMatchFilters.Length == 1)
                                {
                                    if (OnRemovedDevice != null)
                                    {
                                        OnRemovedDevice(this, (UPnPDevice)x);
                                    }
                                }
                            }
                        }
                    }
                }
                if (MatchAll == true)
                {
                    if (PartialMatchFilters.Length == 1)
                    {
                        if (OnRemovedService != null)
                        {
                            foreach (UPnPService S in sList)
                            {
                                OnRemovedService(this, S);
                            }
                        }
                        return;
                    }
                    else
                    {
                        foreach (UPnPDevice dev in dList)
                        {
                            bool _OK_ = true;
                            foreach (string filter in PartialMatchFilters)
                            {
                                if (dev.GetDevices(filter).Length == 0)
                                {
                                    if (dev.GetServices(filter).Length == 0)
                                    {
                                        _OK_ = false;
                                        break;
                                    }
                                }
                            }
                            if (_OK_ == true)
                            {
                                h[dev] = dev;
                            }
                        }
                        foreach (UPnPService serv in sList)
                        {
                            bool _OK_ = true;
                            foreach (string filter in PartialMatchFilters)
                            {
                                if (serv.ParentDevice.GetServices(filter).Length == 0)
                                {
                                    _OK_ = false;
                                    break;
                                }
                            }
                            if (_OK_ == true)
                            {
                                if (h.ContainsKey(serv.ParentDevice) == false)
                                {
                                    h[serv.ParentDevice] = serv.ParentDevice;
                                }
                            }
                        }
                    }
                }

                IDictionaryEnumerator ide = h.GetEnumerator();
                while (ide.MoveNext())
                {
                    if (OnRemovedDevice != null)
                    {
                        OnRemovedDevice(this, (UPnPDevice)ide.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Forward the HandleExpiredDevice event to the user as an OnRemovedDevice event.
        /// </summary>
        /// <param name="sender">UPnPInternalSmartControlPoint that sent the event</param>
        /// <param name="device">The UPnPDevice object that was removed from the network</param>
        private void HandleExpiredDevice(UPnPInternalSmartControlPoint sender, UPnPDevice device)
        {
            HandleRemovedDevice(sender, device);
        }

        private bool CheckDeviceAgainstFilter(string filter, double Version, UPnPDevice device, out object[] MatchingObject)
        {
            ArrayList TempList = new ArrayList();
            // No devices to filter.
            if (device == null)
            {
                MatchingObject = new Object[0];
                return false;
            }

            // Filter is null, all devices will show up.
            if ((filter == "upnp:rootdevice") &&
                (device.Root == true))
            {
                MatchingObject = new Object[1] { device };
                return true;
            }

            if (device.Root == false)
            {
                bool TempBool;
                object[] TempObj;

                foreach (UPnPDevice edevice in device.EmbeddedDevices)
                {
                    TempBool = CheckDeviceAgainstFilter(filter, Version, edevice, out TempObj);
                    if (TempBool == true)
                    {
                        foreach (Object t in TempObj)
                        {
                            TempList.Add(t);
                        }
                    }
                }
            }
            else
            {
                foreach (UPnPDevice dv in device.EmbeddedDevices)
                {
                    object[] m;
                    CheckDeviceAgainstFilter(filter, Version, dv, out m);
                    foreach (object mm in m)
                    {
                        TempList.Add(mm);
                    }
                }
            }

            if ((device.UniqueDeviceName == filter) ||
                (device.DeviceURN_Prefix == filter && double.Parse(device.Version) >= Version))
            {
                TempList.Add(device);
            }
            else
            {
                // Check Services
                for (int x = 0; x < device.Services.Length; ++x)
                {
                    if ((device.Services[x].ServiceID == filter) ||
                       (device.Services[x].ServiceURN_Prefix == filter && double.Parse(device.Services[x].Version) >= Version))
                    {
                        TempList.Add(device.Services[x]);
                    }
                }
            }

            if (TempList.Count == 0)
            {
                MatchingObject = new Object[0];
                return (false);
            }
            else
            {
                MatchingObject = (object[])TempList.ToArray(typeof(object));
                return (true);
            }
        }

        public void ForceDeviceAddition(Uri url)
        {
            iSCP.ForceDeviceAddition(url);
        }

    }
}
