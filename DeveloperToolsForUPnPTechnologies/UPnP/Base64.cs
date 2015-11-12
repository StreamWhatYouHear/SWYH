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
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace OpenSource.UPnP
{
	/// <summary>
	/// Convenience class for Base64 operations
	/// </summary>
	public class Base64
	{
		/// <summary>
		/// Encodes a String as Base64
		/// </summary>
		/// <param name="TheString">The String to encode</param>
		/// <returns></returns>
		public static String StringToBase64(String TheString)
		{
			UTF8Encoding UTF8 = new UTF8Encoding();
			Byte[] buffer = UTF8.GetBytes(TheString);
			return(Encode(buffer));
		}

		/// <summary>
		/// Decodes a Base64 String, and returns it as a String
		/// </summary>
		/// <param name="b64">The Encoding</param>
		/// <returns></returns>
		public static String Base64ToString(String b64)
		{
			Byte[] buffer = Decode(b64);
			UTF8Encoding UTF8 = new UTF8Encoding();
			return(UTF8.GetString(buffer));
		}
		/// <summary>
		/// Encodes a Byte Array as Base64
		/// </summary>
		/// <param name="buffer">The Byte Array to Encode</param>
		/// <returns></returns>
		public static String Encode(byte[] buffer)
		{
			return(Encode(buffer,0,buffer.Length));
		}

		/// <summary>
		/// Encodes a Specific part of a Byte Array as Base64
		/// </summary>
		/// <param name="buffer">The Byte Array to Encode</param>
		/// <param name="offset">The offset to begin encoding</param>
		/// <param name="length">The number of bytes to encode</param>
		/// <returns></returns>
		public static String Encode(byte[] buffer, int offset, int length)
		{
			length += offset;
			ToBase64Transform x = new ToBase64Transform();
			byte[] OutBuf;
			MemoryStream ms = new MemoryStream();
			int pos=offset;
			int size = 3;
			if (length<3)
			{
				size = length;
			}
			do
			{
				OutBuf = x.TransformFinalBlock(buffer,pos,size);
				pos += size;
				if (length-pos<size)
				{
					size = length-pos;
				}
				ms.Write(OutBuf,0,OutBuf.Length);
			}while(pos<length);
			
			OutBuf = ms.ToArray();
			ms.Close();

			UTF8Encoding y = new UTF8Encoding();
			return(y.GetString(OutBuf));
		}
		/// <summary>
		/// Decodes a Base64 Encoding, and returns the Byte Array
		/// </summary>
		/// <param name="Text">The Encoding</param>
		/// <returns></returns>
		public static byte[] Decode(String Text)
		{
			FromBase64Transform x = new FromBase64Transform();
			UTF8Encoding y = new UTF8Encoding();
			byte[] OutBuf;
			Byte[] buffer = y.GetBytes(Text);

			OutBuf = new byte[buffer.Length*3];
			int BytesWritten = x.TransformBlock(buffer,0,buffer.Length,OutBuf,0);
			
			Byte[] NewBuf = new Byte[BytesWritten];
			Array.Copy(OutBuf,0,NewBuf,0,NewBuf.Length);

			return(NewBuf);
		}
	}
}
