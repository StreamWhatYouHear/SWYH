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
using System.Reflection;

namespace OpenSource.UPnP
{
	/// <summary>
	/// Summary description for UPnPDebugObject.
	/// </summary>
	public sealed class UPnPDebugObject
	{
		private object _Object = null;
		private Type _Type = null;

		public UPnPDebugObject(object Obj)
		{
			_Object = Obj;
		}
		public UPnPDebugObject(Type tp)
		{
			_Type = tp;
		}
		public object GetStaticField(string FieldName)
		{
			FieldInfo fi = _Type.GetField(FieldName,BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic);
			return(fi.GetValue(null));
		}
		public object GetField(string FieldName)
		{
			FieldInfo fi = _Object.GetType().GetField(FieldName,BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic);
			return(fi.GetValue(_Object));
		}
		public void SetField(string FieldName, object Arg)
		{
			FieldInfo fi = _Object.GetType().GetField(FieldName,BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic);
			fi.SetValue(_Object,Arg);
		}
		public void SetProperty(string PropertyName, object Val)
		{
			PropertyInfo pi = _Object.GetType().GetProperty(PropertyName,BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic);
			pi.SetValue(_Object,Val,null);
		}
		public object GetProperty(string PropertyName, object[] indexes)
		{
			PropertyInfo pi = _Object.GetType().GetProperty(PropertyName,BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic);
			return(pi.GetValue(_Object,indexes));
		}

		public object InvokeNonStaticMethod(string MethodName, object[] Arg)
		{
			MethodInfo mi = _Object.GetType().GetMethod(MethodName,BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic);
			return(mi.Invoke(_Object,Arg));
		}
		public object InvokeStaticMethod(string MethodName, object[] Arg)
		{
			if (_Object!=null)
			{
				MethodInfo mi = _Object.GetType().GetMethod(MethodName,BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic);
				return(mi.Invoke(null,Arg));
			}
			else
			{
				MethodInfo mi = _Type.GetMethod(MethodName,BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic);
				return(mi.Invoke(null,Arg));
			}
		}
	}
}
