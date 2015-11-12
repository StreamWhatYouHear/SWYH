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
using OpenSource.UPnP;

namespace OpenSource.UPnP.AV.RENDERER.CP
{
	/// <summary>
	/// Summary description for AVRendererDiscovery.
	/// </summary>
	public sealed class AVRendererDiscovery
	{
		public static bool PollPositionEnabled = true;
		private SafeTimer PollTimer = new SafeTimer();
		public delegate void DiscoveryHandler(AVRendererDiscovery sender, AVRenderer renderer);
		public event DiscoveryHandler OnRenderer;
		public event DiscoveryHandler OnRendererRemoved;

		private AVTargetDiscovery.ManagerHandler _New;
		private AVTargetDiscovery.ManagerHandler _Removed;

		private static AVTargetDiscovery disco = null;

		public void ReScan()
		{
			disco.ReScan();
		}

		public void ForceDisposeRenderer(AVRenderer r)
		{
			disco.ForceDisposeRenderer(r);
		}

		public AVRendererDiscovery(DiscoveryHandler OnRendererCallback)
		{
			PollTimer.Interval = 500;
			PollTimer.OnElapsed += new SafeTimer.TimeElapsedHandler(PollSink);

			_New = new AVTargetDiscovery.ManagerHandler(AddSink);
			_Removed = new AVTargetDiscovery.ManagerHandler(RemovedSink);

			OnRenderer += OnRendererCallback;
			lock(this)
			{
				if(disco==null)
				{
					disco = new AVTargetDiscovery();
				}
				if(OnRenderer!=null)
				{
					foreach(AVRenderer r in disco.GetRenderers())
					{
						OnRenderer(this,r);
					}
				}
				disco.AddWeakEvent_OnRenderer(_New);
				disco.AddWeakEvent_RemovedRenderer(_Removed);
			}
			if(AVRendererDiscovery.PollPositionEnabled)
			{
				PollTimer.Start();
			}
		}
		private void PollSink()
		{
			foreach(AVRenderer R in Renderers)
			{
				R._Poll();
			}
			PollTimer.Start();
		}
		public AVRenderer[] Renderers
		{
			get
			{
				return(disco.GetRenderers());
			}
		}
		public AVRenderer GetRenderer(string UDN)
		{
			return(disco.GetRenderer(UDN));
		}
		private void AddSink(AVRenderer r)
		{
			OpenSource.Utilities.EventLogger.Log(this,System.Diagnostics.EventLogEntryType.Error,r.FriendlyName+" [ADDED]");
			if(OnRenderer!=null) OnRenderer(this,r);
		}
		private void RemovedSink(AVRenderer r)
		{
			OpenSource.Utilities.EventLogger.Log(this,System.Diagnostics.EventLogEntryType.Error,r.FriendlyName+" [REMOVED]");
			if(this.OnRendererRemoved!=null) OnRendererRemoved(this,r);
		}
        public void ForceDeviceAddition(Uri url)
        {
            disco.ForceDeviceAddition(url);
        }
	}
}
