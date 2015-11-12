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
using System.Collections;
using System.Runtime.Serialization;
using OpenSource.UPnP;
using OpenSource.UPnP.AV;

namespace OpenSource.UPnP.AV.CdsMetadata
{
	/// <summary>
	/// 
	/// <para>
	/// Accumulator_SystemUpdateID is responsible for accumulating and resetting the values
	/// that get sent in the ContentDirectory's ContainerUpdateIDs state variable. The state variable
	/// is a moderated state variable that events a maximum of once every 2 seconds, 
	/// and has a dynamic string value derived from the containers that have recently changed.
	/// </para>
	/// 
	/// <para>
	/// The format of the string is a comma-separated-value (CSV) list in the form of 
	/// <example>
	/// "[containerID]Accumulator_ContainerUpdateIDs.Delimitor[container.UpdateID],[containerIDn]Accumulator_ContainerUpdateIDs.Delimitor[container.UpdatedID]...".
	/// </example>
	/// </para>
	/// 
	/// </summary>
	[Serializable()]
	public class Accumulator_ContainerUpdateIDs : UPnPModeratedStateVariable.IAccumulator
	{
		/// <summary>
		/// This method is called automatically by the UPNP stack when eventing the
		/// ContainerUpdateIDs. Programmers need only set the value of the ContainerUpdateIDs
		/// state variable with a single elem
		/// </summary>
		/// <param name="current">Comma-separated-value list in the string form of "[containerID] UpdateID= [container.UpdateID], [containerIDn] UpdateID= [container.UpdatedID]"</param>
		/// <param name="newObject">String that should overwrite an existing comma-separated-value item or append to the existing list, in form "[containerID] UpdateID= [container.UpdateID]"</param>
		/// <returns></returns>
		public object Merge(object current, object newObject)
		{
			DText Parser = new DText();
			Parser.ATTRMARK = Accumulator_ContainerUpdateIDs.CSV_Delimitor;

			DText Parser2 = new DText();
			Parser2.ATTRMARK = Accumulator_ContainerUpdateIDs.Delimitor;

			if (current == null) current = "";
			if (newObject == null) newObject = "";

			string curStr = current.ToString();
			string newStr = newObject.ToString();

			if (newStr == "") return "";

			int i;
			Hashtable hash = new Hashtable();
			if (curStr != "")
			{

				Parser[0] = curStr;
				int cnt = Parser.DCOUNT();
				for (i=1; i <= cnt; i++)
				{
					string id, update;
					if (Accumulator_ContainerUpdateIDs.Delimitor == Accumulator_ContainerUpdateIDs.CSV_Delimitor)
					{
						id = Parser[i];
						i++;
						update = Parser[i];
						hash[id] = update;
					}
					else
					{
						string pair = Parser[i];
						Parser2[0] = pair;
						id = Parser2[1];;
						update = Parser2[2];
						hash[id] = update;
					}

					if (id == "")
					{
						throw new ApplicationException("Bad evil. Container ID is empty string.");
					}
					if (update == "")
					{
						throw new ApplicationException("Bad evil. Update ID is empty string.");
					}
				}
			}

			/*
			 * add or overwrite a container update value
			 */ 

			Parser2[0] = newStr;
			string id2 = Parser2[1];
			string update2 = Parser2[2];
			hash[id2] = update2;

			StringBuilder sb = new StringBuilder(hash.Count * 20);
			i=0;
			foreach (string key in hash.Keys)
			{
				if (i > 0)
				{
					sb.Append(",");
				}
				i++;

				string val = hash[key].ToString();
				if (key == "")
				{
					throw new ApplicationException("Bad evil. Accumulator has empty string for key.");
				}
				if (val == "")
				{
					throw new ApplicationException("Bad evil. Accumulator has empty string for value.");
				}
				sb.AppendFormat("{0}{1}{2}", key, Delimitor, val);
			}

			return sb.ToString();
		}

		/// <summary>
		/// Called automatically by the UPNP stack. The method is called after
		/// the variable is evented, and the "current" value becomes an
		/// empty string.
		/// </summary>
		/// <returns></returns>
		public object Reset() { return ""; }

		/// <summary>
		/// Delimitor value used to separate the ContainerID from the container's updateID.
		/// </summary>
		public const string Delimitor = ",";

		/// <summary>
		/// Used to separate out pairs of containerID+updateID from each other
		/// in a comma separated value list.
		/// </summary>
		public const string CSV_Delimitor = ",";
		

		/// <summary>
		/// Allows parsing of delimited strings.
		/// </summary>
		private static DText DT = new DText();
	}
}
