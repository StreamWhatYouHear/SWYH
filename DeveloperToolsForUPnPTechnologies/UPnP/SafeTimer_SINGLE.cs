using System;
using System.Threading;
using System.Collections;

namespace Intel.UPNP
{

	public sealed class ReallySafeTimer
	{
		private static SafeTimer_SINGLE timer = new SafeTimer_SINGLE();
		private SafeTimer_SINGLE.TimeElapsedHandler SINGLE_ElapsedHandler;

		public delegate void TimeElapsedHandler();
		/// <summary>
		/// Triggered when the time period elapses
		/// </summary>
		public event TimeElapsedHandler OnElapsed;

		/// <summary>
		/// Time Period
		/// </summary>
		public int Interval = 0;
		public bool AutoReset = false;

		public ReallySafeTimer()
		{
			SINGLE_ElapsedHandler = new SafeTimer_SINGLE.TimeElapsedHandler(HandleTimer);
		}

		public void Start()
		{
			timer.SetTimer(Interval,SINGLE_ElapsedHandler);
		}
		public void Stop()
		{
			// NOP
		}
		private void HandleTimer(SafeTimer_SINGLE sender)
		{
			if(OnElapsed!=null) OnElapsed();
		}
	}

	/// <summary>
	/// Summary description for SafeTimer_SINGLE.
	/// </summary>
	public sealed class SafeTimer_SINGLE
	{
		private SortedList TimeoutList = new SortedList();

		public delegate void TimeElapsedHandler(SafeTimer_SINGLE sender);
		private RegisteredWaitHandle handle;
		private ManualResetEvent mre = new ManualResetEvent(false);
		private WaitOrTimerCallback WOTcb;

		public SafeTimer_SINGLE()
		{
			WOTcb = new WaitOrTimerCallback(HandleTimer);
		}

		public void SetTimer(int MillisecondTimeout, TimeElapsedHandler CB)
		{
			lock(TimeoutList)
			{
				long Ticks = DateTime.Now.AddMilliseconds(MillisecondTimeout).Ticks;
				while(TimeoutList.ContainsKey(Ticks)==true)
				{
					++Ticks;
				}

				TimeoutList.Add(Ticks,CB);
				if(TimeoutList.Count==1)
				{
					// First Entry
					mre.Reset();
					handle = ThreadPool.RegisterWaitForSingleObject(
						mre,
						WOTcb,
						null,
						MillisecondTimeout,
						true);
				}
				else
				{
					mre.Set();
				}
			}
		}

		private void HandleTimer(Object State, bool TimedOut)
		{
			ArrayList TriggerList = new ArrayList();
			lock(TimeoutList)
			{
				while(TimeoutList.Count>0)
				{
					DateTime timeout = new DateTime((long)TimeoutList.GetKey(0));
					TimeElapsedHandler cb = (TimeElapsedHandler)TimeoutList.GetByIndex(0);

					if(DateTime.Now.CompareTo(timeout)>=0)
					{
						// Trigger
						TriggerList.Add(cb);
						TimeoutList.RemoveAt(0);
					}
					else
					{
						// Reset timer
						mre.Reset();
						int MillisecondTimeout = (int)new TimeSpan(timeout.Ticks-DateTime.Now.Ticks).TotalMilliseconds;
						if(MillisecondTimeout<=0) 
						{
							mre.Set();
							MillisecondTimeout = 0;
						}
						handle = ThreadPool.RegisterWaitForSingleObject(
							mre,
							WOTcb,
							null,
							MillisecondTimeout,
							true);
						break;
					}
				}
			}
			foreach(TimeElapsedHandler _cb in TriggerList)
			{
				_cb(this);
			}
		}
	}
}
