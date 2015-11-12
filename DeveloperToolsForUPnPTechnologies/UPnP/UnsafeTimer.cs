using System;
using System.Threading;
using System.Collections;

namespace Intel.UPNP
{
	/// <summary>
	/// Summary description for UnsafeTimer.
	/// </summary>
	public class UnsafeTimer
	{
		public delegate void OnTimerHandler(UnsafeTimer sender);

		private Thread TimerThread;
		private ManualResetEvent AbortEvent;

		private SortedList TheList = new SortedList();
		private DateTime StartTime;

		public UnsafeTimer()
		{
			TimerThread = new Thread(new ThreadStart(Start));
			AbortEvent = new ManualResetEvent(false);

			TimerThread.Start();
		}
		public void RegisterCallback(int MillisecondTimeout, OnTimerHandler CB)
		{
			lock(TheList)
			{
				DateTime WaitTime;
				if(TheList.Count==0)
				{
					StartTime = DateTime.Now;
				}
				WaitTime = DateTime.Now.AddMilliseconds(MillisecondTimeout);

				if(TheList.ContainsKey(WaitTime.Ticks)==false)
				{
					TheList[WaitTime.Ticks] = new ArrayList();
				}
				((ArrayList)TheList[WaitTime.Ticks]).Add(CB);
				TimerThread.Interrupt();
			}
		}
		public void Dispose()
		{
			AbortEvent.Set();
			TimerThread.Interrupt();
		}
		private void Start()
		{
			ArrayList TriggerList = new ArrayList();
			int i = System.Threading.Timeout.Infinite;
			while(AbortEvent.WaitOne(0,false)==false)
			{
				try
				{
					lock(TheList)
					{
						i = System.Threading.Timeout.Infinite;
						bool OK = true;
						while(OK)
						{
							if(TheList.Count>0)
							{
								DateTime NOW = DateTime.Now;
								if((long)TheList.GetKey(0)<=NOW.Ticks)
								{
									ArrayList a = (ArrayList)TheList.GetByIndex(0);
									foreach(OnTimerHandler CB in a)
									{
										TriggerList.Add(CB);
									}
									TheList.RemoveAt(0);
								}
								else
								{
									OK = false;
								}
							}
							else
							{
								OK = false;
							}
						}
						if(TheList.Count>0)
						{
							StartTime = DateTime.Now;
							i = (int)TimeSpan.FromTicks((long)TheList.GetKey(0)-StartTime.Ticks).TotalMilliseconds;
						}
					}
					foreach(OnTimerHandler CB in TriggerList)
					{
						CB(this);
					}
					TriggerList.Clear();
					Thread.Sleep(i);
				}
				catch(System.Threading.ThreadInterruptedException)
				{
				}
			}
		}
	}
}
