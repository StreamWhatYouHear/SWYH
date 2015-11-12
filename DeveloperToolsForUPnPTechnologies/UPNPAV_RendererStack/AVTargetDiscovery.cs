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
using OpenSource.UPnP;
using OpenSource.UPnP.AV;

namespace OpenSource.UPnP.AV.RENDERER.CP
{
	/// <summary>
	/// This is the DiscoveryClass that is used to discover AVRenderers on the network.
	/// This object is very very simple, and serves only one meaningful purpose, and that is to event
	/// you when a renderer appears on the network. This can be obtained through the <see cref="OpenSource.UPnP.AVRENDERERSTACK.AVTargetDiscovery.OnRenderer"/>
	/// event.
	/// </summary>
	internal class AVTargetDiscovery
	{
		public delegate void ManagerHandler(AVRenderer renderer);
		/// <summary>
		/// This is fired when a Renderer is discovered
		/// </summary>
		public event ManagerHandler OnRenderer;
		/// <summary>
		/// This is fired when a Renderer disappears
		/// </summary>
		public event ManagerHandler RemovedRenderer;

		private ArrayList Weak_OnRendererList = new ArrayList();
		private ArrayList Weak_RemovedRendererList = new ArrayList();

		public void AddWeakEvent_OnRenderer(ManagerHandler mh)
		{
			Weak_OnRendererList.Add(new WeakReference(mh));
		}
		public void AddWeakEvent_RemovedRenderer(ManagerHandler mh)
		{
			Weak_RemovedRendererList.Add(new WeakReference(mh));
		}

		protected UPnPSmartControlPoint scp;
		protected ArrayList ManagerList = new ArrayList();

		public void ReScan()
		{
			scp.Rescan();
		}

		public void ForceDisposeRenderer(AVRenderer r)
		{
			scp.ForceDisposeDevice(r.MainDevice);
		}

		public AVTargetDiscovery():this(null)
		{
		}
		public AVTargetDiscovery(ManagerHandler NewCallback)
		{
			OnRenderer += new ManagerHandler(Weak_OnRendererSink);
			RemovedRenderer += new ManagerHandler(Weak_RemovedRendererSink);

			if(NewCallback!=null) OnRenderer += NewCallback;
			scp = new UPnPSmartControlPoint(new UPnPSmartControlPoint.DeviceHandler(AddSink),
				null,
				new string[3]{CpAVTransport.SERVICE_NAME,CpRenderingControl.SERVICE_NAME,CpConnectionManager.SERVICE_NAME});
			scp.OnRemovedDevice += new UPnPSmartControlPoint.DeviceHandler(RemoveSink);
		}

		public AVRenderer GetRenderer(string DeviceUDN)
		{
			lock(ManagerList)
			{
				foreach(AVRenderer r in ManagerList)
				{
					if(r.UniqueDeviceName==DeviceUDN) return(r);
				}
			}
			return(null);
		}
		public AVRenderer[] GetRenderers()
		{
			return((AVRenderer[])ManagerList.ToArray(typeof(AVRenderer)));
		}

		private void Weak_OnRendererSink(AVRenderer r)
		{
			WeakReference[] wr = (WeakReference[])Weak_OnRendererList.ToArray(typeof(WeakReference));
			foreach(WeakReference W in wr)
			{
				if(W.IsAlive)
				{
					((ManagerHandler)W.Target)(r);
				}
				else
				{
					Weak_OnRendererList.Remove(W);
				}
			}
		}
		private void Weak_RemovedRendererSink(AVRenderer r)
		{
			WeakReference[] wr = (WeakReference[])Weak_RemovedRendererList.ToArray(typeof(WeakReference));
			foreach(WeakReference W in wr)
			{
				if(W.IsAlive)
				{
					((ManagerHandler)W.Target)(r);
				}
				else
				{
					Weak_RemovedRendererList.Remove(W);
				}
			}
		}

		protected void RemoveSink(UPnPSmartControlPoint sender, UPnPDevice device)
		{	
			AVRenderer RemoveThis = null;
			lock(ManagerList)
			{
				for(int i=0;i<ManagerList.Count;++i)
				{
					AVRenderer avm = (AVRenderer)ManagerList[i];
					if(avm.MainDevice.UniqueDeviceName == device.UniqueDeviceName)
					{
						RemoveThis = avm;
						ManagerList.RemoveAt(i);
						break;
					}
				}
			}

			if(RemoveThis!=null)
			{
				RemoveThis.Removed();
				if(RemovedRenderer!=null) RemovedRenderer(RemoveThis);
			}
		}
		protected void AddSink(UPnPSmartControlPoint sender, UPnPDevice device)
		{
			AVRenderer avm = new AVRenderer(device);
			lock(ManagerList)
			{
				ManagerList.Add(avm);
			}

			if(OnRenderer!=null) OnRenderer(avm);
		}

        public void ForceDeviceAddition(Uri url)
        {
            scp.ForceDeviceAddition(url);
        }
    }
}
