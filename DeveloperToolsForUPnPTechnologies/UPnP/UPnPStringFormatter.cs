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
	/// Helper class that escapes/unescapes XML reserved characters
	/// </summary>
	public class UPnPStringFormatter
	{
		public static string GetURNPrefix(string urn)
		{
			int len;
			DText p = new DText();
			p.ATTRMARK = ":";
			p[0] = urn;
			len = p[p.DCOUNT()].Length;
			return(urn.Substring(0,urn.Length-len));
		}
		/// <summary>
		/// Escape reserved characters
		/// </summary>
		/// <param name="InString">Input</param>
		/// <returns>Result</returns>
		public static string EscapeString(string InString)
		{
			InString = InString.Replace("&","&amp;");
			InString = InString.Replace("<","&lt;");
			InString = InString.Replace(">","&gt;");
			InString = InString.Replace("\"","&quot;");
			InString = InString.Replace("'","&apos;");
			return(InString);
		}
		public static string PartialEscapeString(string InString)
		{
			InString = InString.Replace("\"","&quot;");
			InString = InString.Replace("'","&apos;");
			return(InString);
		}
		/// <summary>
		/// Unescapes encoded reserved characters
		/// </summary>
		/// <param name="InString">Input</param>
		/// <returns>Result</returns>
		public static string UnEscapeString(string InString)
		{
			InString = InString.Replace("&lt;","<");
			InString = InString.Replace("&gt;",">");
			InString = InString.Replace("&quot;","\"");
			InString = InString.Replace("&apos;", "'");
			InString = InString.Replace("&amp;","&");
			return(InString);
		}	
	}
}
