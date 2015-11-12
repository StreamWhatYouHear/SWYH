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

namespace OpenSource.UPnP.AV.CdsMetadata
{
	/// <summary>
	/// This class provides static methods that cast an object 
	/// into another object of the appropriate type. If
	/// the object is null, or a value cannot be represented
	/// because of a type conflict, the returned value is 
	/// is a non-NULL default value.
	/// </summary>
	public class PreventNullCast
	{
		/// <summary>
		/// Used to cast a _UInt or a null value  into a _UInt object. If the object is null,
		/// a new _UInt is created with the IsValid property set to false.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static _UInt CastUInt(object obj)
		{
			if (obj is _UInt) { return (_UInt) obj; }
			return new _UInt(false);
		}

		/// <summary>
		/// Used to cast a _ULong or a null value  into a _ULong object. If the object is null,
		/// a new _ULong is created with the IsValid property set to false.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static _ULong CastULong(object obj)
		{
			if (obj is _ULong) { return (_ULong) obj; }
			return new _ULong(false);
		}

		/// <summary>
		/// Used to cast a string or a null value into a string object. If the object is null,
		/// empty string is returned.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string CastString(object obj)
		{
			string casted = obj as string;
			if (casted != null) { return casted; }
			return "";
		}

		/// <summary>
		/// Used to cast a _TimeSpan or a null value into a _TimeSpan object. If the object is
		/// null, an unassigned/invalid _TimeSpan is returned.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static _TimeSpan CastTimeSpan(object obj)
		{
			if (obj is _TimeSpan) { return (_TimeSpan) obj; }
			return new _TimeSpan(false);
		}

	}
}
