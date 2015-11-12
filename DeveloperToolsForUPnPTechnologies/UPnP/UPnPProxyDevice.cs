using System;
using Intel.UPNP;
using System.Collections;

namespace Intel.UPNP
{
	/// <summary>
	/// Summary description for UPnPProxyDevice.
	/// </summary>
	public class UPnPProxyDevice
	{
		private UPnPDevice ProxyDevice;
		private Hashtable ServiceTable = new Hashtable();
		private Hashtable InvokeTable = new Hashtable();
		private Hashtable RealServiceTable = new Hashtable();

		private Queue CallQueue = new Queue();
		private Hashtable ResponseTable = new Hashtable();
		private long InvokeID = 0;

		public string UDN
		{
			get
			{
				return(ProxyDevice.UniqueDeviceName);
			}
		}
		
		public UPnPProxyDevice(UPnPDevice device)
		{
			ProxyDevice = UPnPDevice.CreateRootDevice(2000,1,"");
			ProxyDevice.UniqueDeviceName = Guid.NewGuid().ToString();
			int id = ProxyDevice.UniqueDeviceName.GetHashCode();

			ProxyDevice.HasPresentation = false;
			ProxyDevice.FriendlyName = "Intel UPnP Proxy Device (" + id.ToString() + ")";
			ProxyDevice.Manufacturer = "Intel's Connected and Extended PC Lab";
			ProxyDevice.ManufacturerURL = "http://www.intel.com/labs";
			ProxyDevice.ModelName = "UPnP Proxy";
			ProxyDevice.StandardDeviceType = "UPnP_ProxyDevice";

			UPnPDevice d;

			d = UPnPDevice.CreateEmbeddedDevice(double.Parse(device.Version),device.UniqueDeviceName);
			d.FriendlyName = device.FriendlyName;
			d.Manufacturer = device.Manufacturer;
			d.ManufacturerURL = device.ManufacturerURL;
			d.ModelDescription = device.ModelDescription;
			d.ModelURL = device.ModelURL;
			d.DeviceURN = device.DeviceURN;

			foreach(UPnPService S in device.Services)
			{
				UPnPService S2 = (UPnPService)S.Clone();
				foreach(UPnPAction A in S2.Actions)
				{
					A.ParentService = S2;
					A.SpecialCase += new UPnPAction.SpecialInvokeCase(InvokeSink);
				}

				S2.SCPDURL = "_" + S2.ServiceID + "_scpd.xml";
				S2.ControlURL = "_" + S2.ServiceID + "_control";
				S2.EventURL = "_" + S2.ServiceID + "_event";

				ServiceTable[S2] = S;
				RealServiceTable[S] = S2;
				S.Subscribe(1000,new UPnPService.UPnPEventHandler(EventSink));
				d.AddService(S2);
			}
			ProxyDevice.AddDevice(d);
			ProxyDevice.StartDevice();
		}
		public void Stop()
		{
			ProxyDevice.StopDevice();
		}

		protected void EventSink(UPnPService sender, long seq)
		{
			UPnPService S = (UPnPService)RealServiceTable[sender];
			foreach(UPnPStateVariable v in sender.GetStateVariables())
			{
				UPnPStateVariable sv = S.GetStateVariableObject(v.Name);
				sv.Value = v.Value;
			}
		}
		protected void InvokeSink(UPnPAction sender, UPnPArgument[] InArgs, out object RetVal, out UPnPArgument[] OutArgs)
		{
			UPnPService S = (UPnPService)ServiceTable[sender.ParentService];
			UPnPAction A = S.GetAction(sender.Name);

			ArrayList TempList = new ArrayList();
			foreach(UPnPArgument arg in A.Arguments)
			{
				if((arg.IsReturnValue==false)&&(arg.Direction=="out")) TempList.Add(arg.Clone());
			}

			foreach(UPnPArgument arg in InArgs)
			{
				TempList.Add(arg);
			}

			lock(CallQueue)
			{
				++InvokeID;
				CallQueue.Enqueue(InvokeID);
				S.InvokeAsync(sender.Name,(UPnPArgument[])TempList.ToArray(typeof(UPnPArgument)),
					InvokeID,
					new UPnPService.UPnPServiceInvokeHandler(RealInvokeSink),
					null);
				InvokeTable[InvokeID] = sender.ParentService;
			}

			UPnPArgument[] OtherArgs;
			sender.ParentService.DelayInvokeRespose(InvokeID, out OtherArgs);
			throw(new DelayedResponseException());
		}
		private void RealInvokeSink(UPnPService sender, String MethodName, UPnPArgument[] Args, Object ReturnValue, object Tag)
		{
			UPnPService S = (UPnPService)InvokeTable[(long)Tag];
			ArrayList TempList = new ArrayList();
			foreach(UPnPArgument arg in Args)
			{
				if(arg.Direction=="out") TempList.Add(arg);
			}

			lock(CallQueue)
			{
				if((long)CallQueue.Peek()==(long)Tag)
				{
					S.DelayedInvokeResponse((long)Tag,ReturnValue,(UPnPArgument[])TempList.ToArray(typeof(UPnPArgument)),null);
					CallQueue.Dequeue();
					bool OK = CallQueue.Count>0;
					while(OK)
					{
						if(ResponseTable.ContainsKey((long)CallQueue.Peek()))
						{
							long key = (long)CallQueue.Dequeue();
							object[] G = (object[])ResponseTable[key];
							S.DelayedInvokeResponse(key,G[0],(UPnPArgument[])G[1],null);
							OK = CallQueue.Count>0;
						}
						else
						{
							OK = false;
						}
					}
				}
				else
				{
					// Don't Return this Yet
					ResponseTable[(long)Tag] = new object[2]{ReturnValue,(UPnPArgument[])TempList.ToArray(typeof(UPnPArgument))};
				}
			}

		}
	}
}
