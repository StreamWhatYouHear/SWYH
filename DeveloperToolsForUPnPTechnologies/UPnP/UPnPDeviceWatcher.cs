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

namespace OpenSource.UPnP
{
	/// <summary>
	/// Summary description for UPnPDeviceWatcher.
	/// </summary>
	public class UPnPDeviceWatcher
	{
		WeakReference W;
		public delegate void SniffHandler(byte[] raw, int offset, int length);
		public event SniffHandler OnSniff;
		public delegate void SniffPacketHandler(HTTPMessage Packet);
		public event SniffPacketHandler OnSniffPacket;

		public UPnPDeviceWatcher(UPnPDevice d)
		{
			W = new WeakReference(d);
			d.OnSniff += new UPnPDevice.SniffHandler(SniffSink);
			d.OnSniffPacket += new UPnPDevice.SniffPacketHandler(SniffPacketSink);
		}
		private void SniffSink(byte[] raw, int offset, int length)
		{
			if (OnSniff!=null) OnSniff(raw,offset,length);
		}
		private void SniffPacketSink(HTTPMessage Packet)
		{
			if (OnSniffPacket!=null) OnSniffPacket(Packet);
		}
	}
}
