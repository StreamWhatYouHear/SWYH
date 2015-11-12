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
	/// This object derives from UPnPStateVariable. This implementation adds the ability
	/// to moderate the event, as defined in UPnP A/V.
	/// <para>
	/// Simply set the Accumulator and Moderation Period.
	/// </para>
	/// </summary>
	public class UPnPModeratedStateVariable : UPnPStateVariable
	{
		private LifeTimeMonitor.LifeTimeHandler LifeTimeHandler;
		/// <summary>
		/// This is the interface which describes an accumulator, which is what
		/// the ModeratedUPnPStateVariable uses to merge values together.
		/// </summary>
		public interface IAccumulator
		{
			/// <summary>
			/// This is called by the UPnPModeratedStateVariable, to merge two values together
			/// </summary>
			/// <param name="current">The Current Value</param>
			/// <param name="newobject">The Value to Merge</param>
			/// <returns>Merged Value</returns>
			object Merge(object current, object newobject);
			/// <summary>
			/// This is called by the UPnPModeratedStateVariable at the end of the moderation period, after
			/// the UPnPEvent has been sent.
			/// </summary>
			/// <returns>The value to (re)set the UPnPStateVariable to</returns>
			object Reset();
		}

		/// <summary>
		/// This is the default implementation that is used by the UPnPModeratedStateVariable
		/// by default. This implementation simply writes the most recent value.
		/// </summary>
		public class DefaultAccumulator : IAccumulator
		{
			public object Merge(object current, object newobject)
			{
				return(newobject);
			}
			public object Reset()
			{
				return(null);
			}
		}
		
		/// <summary>
		/// The Accumulator to use. DefaultAccumulator is used by default.
		/// </summary>
		public IAccumulator Accumulator = new DefaultAccumulator();
		protected object PendingObject = null;

		protected double Seconds = 0;
		protected int PendingEvents = 0;
		protected LifeTimeMonitor t = new LifeTimeMonitor();

		public UPnPModeratedStateVariable(string VarName, object VarValue):base(VarName,VarValue)
		{
			InitMonitor();
		}
		public UPnPModeratedStateVariable(string VarName, Type VarType, bool SendEvents):base(VarName,VarType,SendEvents)
		{
			InitMonitor();
		}
		public UPnPModeratedStateVariable(string VarName, object VarValue, string[] AllowedValues):base(VarName,VarValue,AllowedValues)
		{
			InitMonitor();
		}

		protected void InitMonitor()
		{
			LifeTimeHandler = new LifeTimeMonitor.LifeTimeHandler(LifeTimeSink);
			t.OnExpired += LifeTimeHandler;
		}

		protected void LifeTimeSink(LifeTimeMonitor sender, object Obj)
		{
			lock(this)
			{
				if (PendingEvents>1)
				{
					base.Value = PendingObject;
				}
				PendingObject = Accumulator.Reset();
				PendingEvents = 0;
			}
			/*

				if (PendingObject != null)
				{
					base.Value = PendingObject;
					PendingObject = Accumulator.Reset();
					PendingEvents = 1;
					t.Add(this,Seconds);
				}
				else
				{
					StateObject = Accumulator.Reset();
					PendingEvents = 0;
				}
			}*/
		}

		/// <summary>
		/// The Moderation period to use. Zero if none, positive number otherwise.
		/// </summary>
		public double ModerationPeriod
		{
			get
			{
				return(Seconds);
			}
			set
			{
				lock(this)
				{
					Seconds = value;	
				}
			}
		}

		public override Object Value
		{
			get
			{
				return(base.Value);
			}
			set
			{
				if (Seconds==0)
				{
					base.Value = value;
					return;
				}
				lock(this)
				{
					if (PendingEvents==0)
					{
						++PendingEvents;
						base.Value = value;
						//PendingObject = Accumulator.Merge(PendingObject,value);
						PendingObject = Accumulator.Reset();
						t.Add(this,Seconds);
					}
					else
					{
						++PendingEvents;
						PendingObject = Accumulator.Merge(PendingObject,value);
					}
				}
			}
		}
	}
}
