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
using System.Threading;
using System.Collections;
using OpenSource.Utilities;

namespace OpenSource.UPnP
{
	/// <summary>
	/// Safe timer, that doesn't spawn new threads, or resources that you own
	/// </summary>
	public sealed class SafeTimer
	{
		private bool WaitFlag;
		private bool StartFlag;
		private int timeout;
		/// <summary>
		/// These group of members are used to notify the modules above that an event has fired
		/// without holding any strong references
		/// </summary>
		private WeakEvent ElapsedWeakEvent = new WeakEvent();
		public delegate void TimeElapsedHandler();
		public event TimeElapsedHandler OnElapsed
		{
			add {ElapsedWeakEvent.Register(value);}
			remove {ElapsedWeakEvent.UnRegister(value);}
		}

		/// <summary>
		/// Time Period
		/// </summary>
		public int Interval = 0;
	public bool AutoReset = false;
		// SWC 20130227 Fix high cpu problem
		// SWC Declare constant for minimum timer interval
		private const int MINIMUM_INTERVAL = 1000;

		private RegisteredWaitHandle handle;
		private ManualResetEvent mre = new ManualResetEvent(false);
		private object RegLock = new object();
		private WaitOrTimerCallback WOTcb;

		public SafeTimer()
		{
			WaitFlag = false;
			timeout = 0;
			WOTcb = new WaitOrTimerCallback(HandleTimer);
			OpenSource.Utilities.InstanceTracker.Add(this);
		}
		public SafeTimer(int Milliseconds, bool Auto):this()
		{
			// SWC 20130227 Fix high cpu problem
			// SWC Validate Interval setting
			//if (Milliseconds <= 0 || Milliseconds >= Int32.MaxValue) Milliseconds = 1000; // This should never happen
			//Interval = Milliseconds;
			Interval = Math.Max(MINIMUM_INTERVAL, Milliseconds);
			AutoReset = Auto;
			OpenSource.Utilities.InstanceTracker.Add(this);
		}
		public void Start()
		{
			lock(RegLock)
			{
				if (WaitFlag == false)
				{
					mre.Reset();
					if (handle != null) handle.Unregister(null);
					// SWC 20130227 Fix high cpu problem
					// SWC Validate Interval setting, assign to timeout for use in callback timer
					//handle = ThreadPool.RegisterWaitForSingleObject(mre, WOTcb, null, Interval, true);
					timeout = Math.Max(MINIMUM_INTERVAL, Interval);
					handle = ThreadPool.RegisterWaitForSingleObject(mre, WOTcb, null, timeout, true);
					StartFlag = false;	
				}
				else
				{
					StartFlag = true;
					// SWC 20130227 Fix high cpu problem
					// SWC Move conditional assignment closer to actual usage
					//if (Interval < timeout)	timeout = Interval;
					if (Interval < timeout) timeout = Math.Max(MINIMUM_INTERVAL, Interval);
				}
			}
		}
		public void dispose()
		{
			// SWC Testing
			System.Reflection.MethodBase method
				= System.Reflection.MethodInfo.GetCurrentMethod();
			OpenSource.Utilities.EventLogger.Log(
				this,
				System.Diagnostics.EventLogEntryType.Information,
				String.Format(
					"Debug {0}" +
					"{1} disposed",
					System.Environment.NewLine,
					method.DeclaringType.Name));

            // SWC 20130122 Set lock because event handler is still callable
			lock (RegLock)
			{
				if (handle != null)
				{
					handle.Unregister(null);
					handle = null;
				}
				// SWC 20130227 Close event
				if (mre != null)
				{
					mre.Close();
					mre = null;
				}
			}
		}
#if FINALIZER_CHECK
		~SafeTimer()
		{
			// SWC Testing
			System.Reflection.MethodBase method
				= System.Reflection.MethodInfo.GetCurrentMethod();
			OpenSource.Utilities.EventLogger.Log(
				this,
				System.Diagnostics.EventLogEntryType.Error,
				String.Format(
				"{0} Finalizer called. Disposed= {2}",
				method.DeclaringType.Name,
				method.Name,
				(disposed ? "True" : "False")));
		}
#endif

		public void Stop()
		{
			bool IsOK;
			lock(RegLock)
			{
				if (handle!=null)
				{
					IsOK = handle.Unregister(null);
				}
				handle = null;
			}
		}
		private void HandleTimer(Object State, bool TimedOut)
		{
			// SWC 20130122 Fix ObjectDisposedException
			if (mre == null)
			{
                System.Reflection.MethodBase method =
                    System.Reflection.MethodInfo.GetCurrentMethod();
				OpenSource.Utilities.EventLogger.Log(
					this,
					System.Diagnostics.EventLogEntryType.Error,
					String.Format(
						"{0}.{1}() EH called on disposed object",
						method.DeclaringType.Name,
						method.Name));
					return;
			}

			if (TimedOut == false)
			{
				return;
			}
			
			lock(RegLock)
			{
                // SWC 20130122 Fix ObjectDisposedException
				if (mre == null)			// Object disposed
				{
                    System.Reflection.MethodBase method =
                        System.Reflection.MethodInfo.GetCurrentMethod();
					OpenSource.Utilities.EventLogger.Log(
						this,
						System.Diagnostics.EventLogEntryType.Error,
						String.Format(
							"{0}.{1}() msg01 Object disposed while executing",
							method.DeclaringType.Name,
							method.Name));
					return;
				}
				if (handle != null)
				{
					handle.Unregister(null);
					handle = null;
				}
				WaitFlag = true;
				StartFlag = false;
				// SWC 20130227 Fix high cpu problem
				// SWC Move assignment closer to actual usage
				//timeout = Interval;				
			}

			this.ElapsedWeakEvent.Fire();

            // SWC 20130122 Lock down the entire code block
			lock(RegLock)
			{
                // SWC 20130122 Fix ObjectDisposedException
				if (mre == null)			// Object disposed
				{
                    System.Reflection.MethodBase method =
                        System.Reflection.MethodInfo.GetCurrentMethod();
					OpenSource.Utilities.EventLogger.Log(
						this,
						System.Diagnostics.EventLogEntryType.Error,
						String.Format(
							"{0}.{1}() msg02 Object disposed while executing",
							method.DeclaringType.Name,
							method.Name));
					return;
				}
				if (AutoReset == true)
				{
					mre.Reset();
					// SWC 20130227 Fix high cpu problem
					// SWC Validate Interval setting, assign to timeout for use in callback timer
					//handle = ThreadPool.RegisterWaitForSingleObject(mre, WOTcb, null, Interval, true);
					timeout = Math.Max(MINIMUM_INTERVAL, Interval);
					handle = ThreadPool.RegisterWaitForSingleObject(mre, WOTcb, null, timeout, true);
				}
				else
				{
					if (WaitFlag == true && StartFlag == true)
					{
						// SWC 20130227 Fix high cpu problem
						// SWC Move assignment closer to actual usage
						//Interval = timeout;
						mre.Reset();
						if (handle != null) handle.Unregister(null);
						// SWC 20130227 Fix high cpu problem
						// SWC Conditionally assign validated Interval setting to timeout for use in callback timer
						//handle = ThreadPool.RegisterWaitForSingleObject(mre, WOTcb, null, Interval, true);
						handle = ThreadPool.RegisterWaitForSingleObject(mre, WOTcb, null, timeout, true);
						// SWC Reset timeout to max value for use in next execution
						timeout = Int32.MaxValue;
					}
					WaitFlag = false;
					StartFlag = false;
				}
			}
		}
	}
}
