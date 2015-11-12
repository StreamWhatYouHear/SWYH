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
using System.Text;
using System.Runtime.Serialization;
using OpenSource.UPnP;

namespace OpenSource.UPnP.AV.CdsMetadata
{
	/// <summary>
	/// This struct wraps the standard Timespan value type 
	/// with an <see cref="IValueType"/> interface.
	/// </summary>
	[Serializable()]
	public struct _TimeSpan : IValueType
	{
		/// <summary>
		/// Returns the unsigned int value.
		/// Not reliable if IsValid is false.
		/// </summary>
		public object Value { get { return m_Value; } }
		/// <summary>
		/// Indicates if the value returned from Value
		/// is reliable.
		/// </summary>
		public bool IsValid { get { return m_IsValid; } }
		/// <summary>
		/// The actual value.
		/// </summary>
		public readonly TimeSpan m_Value;
		/// <summary>
		/// Indicates if the value is reliable.
		/// </summary>
		private bool m_IsValid;
		
		/// <summary>
		/// Creates a valid TimeSpan wrapped in the interface.
		/// </summary>
		/// <param name="val"></param>
		public _TimeSpan(TimeSpan val)
		{
			m_Value = val;
			m_IsValid = true;
		}
		/// <summary>
		/// Creates an invalid/unassigned uint in an object.
		/// </summary>
		/// <param name="invalid">param is ignored</param>
		public _TimeSpan (bool invalid)
		{
			m_IsValid = false;
			m_Value = new TimeSpan(0);
		}

		/// <summary>
		/// Creates a timespan from a ContentDirectory-formatted string.
		/// <para>
		/// The time duration of the media, in the format
		/// "h*:mm:ss:f*" or "h*:mm:ss.f0/f1",
		/// where
		/// <list type="bullet">
		/// <item>
		/// <term>h*</term>
		/// <description>any number of digits (including no digits) to indicated elapsed time in hours</description>
		/// </item>
		/// <item>
		/// <term>mm</term>
		/// <description>0 to 59, to indicate minutes</description>
		/// </item>
		/// <item>
		/// <term>ss</term>
		/// <description>0 to 59, to indicate seconds</description>
		/// </item>
		/// <item>
		/// <term>f*</term>
		/// <description>any number of digits (including no digits) to indicated fractions of seconds</description>
		/// </item>
		/// <item>
		/// <term>f0/f1</term>
		/// <description>a fraction, with f0 and f1 at least one digit long and f0 is less than f1. This field may be preceded
		/// by an optional + or - sign, and the preceding decimal point may be omitted if no
		/// fractional seconds are listed</description>
		/// </item>
		/// </list>
		/// </para>
		/// <para>
		/// TODO: Need to properly support fractional seconds. Currently only support .NET formatted
		/// fractional seconds... the "x/y" format for fractional seconds is not supported.
		/// </para>
		/// </summary>
		/// <param name="CdsFormattedDuration"></param>
		public _TimeSpan (string CdsFormattedDuration)
		{
			string duration = CdsFormattedDuration.Trim();
			StringBuilder sb = new StringBuilder();

			//handle positive/negative character

			bool negative = duration.StartsWith("-");

			if ((duration.StartsWith("-") || (duration.StartsWith("+"))))
			{
				duration.Remove(0,1);
			}

			if (negative)
			{
				sb.Append("-");
			}

			// determine whether the duration includes hours/days
			int firstColonPos = duration.IndexOf(":");
			if ((firstColonPos != duration.LastIndexOf(":")) && firstColonPos != -1)
			{
				// we have hours (and possibly days) to populte
				string h = duration.Substring(0, firstColonPos);

				int days_hours = int.Parse(h);

				int days = days_hours / 24;
				int hours = days_hours % 24;

				if (days > 0)
				{
					sb.AppendFormat("{0}:", days);
				}

				sb.AppendFormat("{0}:", hours);

				duration = duration.Remove(0, h.Length+1);
			}
			else
			{
				sb.Append("00:");
			}

			//append minutes, seconds, and fractional seconds
			sb.Append(duration);

			this.m_Value = TimeSpan.Parse(sb.ToString());
			this.m_IsValid = true;
		}

		/// <summary>
		/// Casts the value into its ContentDirectory-valid string form. 
		/// <para>
		/// The time duration of the media, in the format
		/// "h*:mm:ss:f*" or "h*:mm:ss.f0/f1",
		/// where
		/// <list type="bullet">
		/// <item>
		/// <term>h*</term>
		/// <description>any number of digits (including no digits) to indicated elapsed time in hours</description>
		/// </item>
		/// <item>
		/// <term>mm</term>
		/// <description>0 to 59, to indicate minutes</description>
		/// </item>
		/// <item>
		/// <term>ss</term>
		/// <description>0 to 59, to indicate seconds</description>
		/// </item>
		/// <item>
		/// <term>f*</term>
		/// <description>any number of digits (including no digits) to indicated fractions of seconds</description>
		/// </item>
		/// <item>
		/// <term>f0/f1</term>
		/// <description>a fraction, with f0 and f1 at least one digit long and f0 is less than f1. This field may be preceded
		/// by an optional + or - sign, and the preceding decimal point may be omitted if no
		/// fractional seconds are listed</description>
		/// </item>
		/// </list>
		/// </para>
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if (this.m_IsValid)
			{
				StringBuilder sb = new StringBuilder();
				if ((this.m_Value.Hours > 0) || (this.m_Value.Days > 0))
				{
					int hours = 0;
					if (this.m_Value.Days > 0)
					{
						hours += (this.m_Value.Days * 24);
					}
					hours += this.m_Value.Hours;
					sb.AppendFormat("{0}:", hours);
				}

				if (this.m_Value.Minutes < 10)
				{
					sb.AppendFormat("0{0}:", this.m_Value.Minutes);
				}
				else
				{
					sb.AppendFormat("{0}:", this.m_Value.Minutes);
				}

				if (this.m_Value.Seconds < 10)
				{
					sb.AppendFormat("0{0}", this.m_Value.Seconds);
				}
				else
				{
					sb.AppendFormat("{0}", this.m_Value.Seconds);
				}

				// TODO: handle fractional digits in terms of milliseconds

				return sb.ToString();
			}

			return "00:00";
		}

		/// <summary>
		/// Allows comparisons with standard numerical types
		/// using the underlying uint.CompareTo() method.
		/// If the object is another _TimeSpan value, it
		/// will properly extract the value for comparison.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			System.Type type = obj.GetType();
			object compareToThis = obj;

			if (type == typeof(_TimeSpan))
			{
				_DateTime ui = (_DateTime) obj;
				compareToThis = ui.m_Value;
			}
			return this.m_Value.CompareTo(compareToThis);
		}

		private static DText DT = new DText();
	}
}
