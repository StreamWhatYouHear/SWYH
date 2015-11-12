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
using System.Net;
using System.Text;
using System.Collections;

namespace OpenSource.UPnP
{
	/// <summary>
	/// A generic HTTP/UPnP Packet
	/// </summary>
	[Serializable()]
	public sealed class HTTPMessage : ICloneable
	{
		internal bool DontShowContentLength = false;
		public bool OverrideContentLength = false;

		/// <summary>
		/// Contructs an Empty Packet
		/// </summary>
		public HTTPMessage():this("1.1")
		{
		}
		public HTTPMessage(string version)
		{
			OpenSource.Utilities.InstanceTracker.Add(this);
			TheHeaders = new Hashtable();
			ResponseCode = -1;
			ResponseData = "";
			Method = "";
			MethodData = "";
			DataBuffer = new byte[0];	
			Version = version;
		}

		public object Clone()
		{
			object obj = this.MemberwiseClone();
			OpenSource.Utilities.InstanceTracker.Add(obj);
			return(obj);
		}
		/// <summary>
		/// Get/Set the body as a Byte Array
		/// </summary>
		public byte[] BodyBuffer
		{
			get
			{
				return(DataBuffer);
			}
			set
			{
				DataBuffer = value;
			}
		}
		/// <summary>
		/// Returns the Character encoding of the body, if it was defined
		/// </summary>
		public string CharSet
		{
			get
			{
				string ct = ContentType;
				
				DText p = new DText();
				p.ATTRMARK = ";";
				p.MULTMARK = "=";
				p[0] = ct;
				string V = "";

				if (p.DCOUNT()>1)
				{
					for(int i=1;i<=p.DCOUNT();++i)
					{
						if (p[i,1].Trim().ToUpper()=="CHARSET")
						{
							V = p[i,2].Trim().ToUpper();
							if (V.StartsWith("\"")==true)
							{
								V = V.Substring(1);
							}
							if (V.EndsWith("\"")==true)
							{
								V = V.Substring(0,V.Length-1);
							}
							break;
						}
					}
					return(V);
				}
				else
				{
					return("");
				}
			}
		}
		/// <summary>
		/// Gets/Sets the Content-type field
		/// </summary>
		public string ContentType
		{
			get
			{
				return(GetTag("Content-Type"));
			}
			set
			{
				AddTag("Content-Type",value);
			}
		}
		/// <summary>
		/// Gets the Body (encoded as CharSet) as a String, Sets the body as a UTF-8 encoded string
		/// </summary>
		public String StringBuffer
		{
			get
			{
				if (CharSet=="UTF-16")
				{
					UnicodeEncoding UTF16 = new UnicodeEncoding();
					return(UTF16.GetString(DataBuffer));
				}
				else
				{
					UTF8Encoding UTF8 = new UTF8Encoding();
					return(UTF8.GetString(DataBuffer));
				}

			}
			set
			{
				UTF8Encoding UTF8 = new UTF8Encoding();
				DataBuffer = UTF8.GetBytes(value);
			}
		}

		/// <summary>
		/// Removes a header
		/// </summary>
		/// <param name="TagName"></param>
		public void RemoveTag(String TagName)
		{
			try
			{
				TheHeaders.Remove(TagName.ToUpper());
			}
			catch(Exception ex)
			{
                OpenSource.Utilities.EventLogger.Log(ex);
            }
		}

		public IDictionaryEnumerator GetHeaderEnumerator()
		{
			return(TheHeaders.GetEnumerator());
		}
							 
		/// <summary>
		/// Add/Change a Header
		/// </summary>
		/// <param name="TagName">Header Tag</param>
		/// <param name="TagData">Header Value</param>
		public void AddTag(String TagName, String TagData)
		{
			TheHeaders[TagName.ToUpper()] = TagData;
			//TheHeaders[TagName] = TagData;
		}
		public void AppendTag(string TagName, string TagData)
		{
			if (TheHeaders.ContainsKey(TagName.ToUpper())==false)
			{
				TheHeaders[TagName.ToUpper()] = TagData;
			}
			else
			{
				if (TheHeaders[TagName.ToUpper()].GetType()==typeof(string))
				{
					ArrayList n = new ArrayList();
					n.Add(TheHeaders[TagName.ToUpper()]);
					TheHeaders[TagName.ToUpper()] = n;
				}
				((ArrayList)TheHeaders[TagName.ToUpper()]).Add(TagData);
			}
		}

		public bool HasTag(string TagName)
		{
			return(TheHeaders.ContainsKey(TagName.ToUpper()));
		}
		/// <summary>
		/// Returns the value for this tag
		/// </summary>
		/// <param name="TagName">Header Tag</param>
		/// <returns>Header Value</returns>
		public String GetTag(String TagName)
		{
			Object x = TheHeaders[TagName.ToUpper()];
			if (x==null)
			{
				return("");
			}
			else
			{
				if (x.GetType()==typeof(string))
				{
					return(((String)x).Trim());
				}
				else
				{
					string RetVal = "";
					foreach(string v in (ArrayList)x)
					{
						RetVal += v.Trim();
					}
					return(RetVal);
				}
			}
		}

		/// <summary>
		/// Returns the entire packet as a String, for debug purposes only. Note, the body is treated as a UTF-8 string
		/// </summary>
		public String StringPacket
		{
			get
			{
				UTF8Encoding UTF8 = new UTF8Encoding();
				return(UTF8.GetString(RawPacket));
			}
		}

		/// <summary>
		/// Returns the entire packet as a Byte Array
		/// </summary>
		public byte[] RawPacket
		{
			get
			{
				return(BuildPacket());
			}
		}

		/// <summary>
		/// Parses a Byte Array, and build a Packet.
		/// </summary>
		/// <param name="buffer">The Array of Bytes</param>
		/// <returns></returns>
		static public HTTPMessage ParseByteArray(byte[] buffer)
		{
			return(ParseByteArray(buffer,0,buffer.Length));
		}
		/// <summary>
		/// Parses a Byte Array at a specific location, and builds a Packet.
		/// </summary>
		/// <param name="buffer">The Array of Bytes</param>
		/// <param name="indx">The Start Index</param>
		/// <param name="count">The number of Bytes to process</param>
		/// <returns></returns>
		static public HTTPMessage ParseByteArray(byte[] buffer, int indx, int count)
		{	
			HTTPMessage TheMessage = new HTTPMessage();
			UTF8Encoding UTF8 = new UTF8Encoding();
			String TempData = UTF8.GetString(buffer,indx,count);
			DText parser = new DText();
			String TempString;

			int idx = TempData.IndexOf("\r\n\r\n");
            if (idx < 0) return null;
			TempData = TempData.Substring(0,idx);

			parser.ATTRMARK = "\r\n";
			parser.MULTMARK = ":";
			parser[0] = TempData;
			string CurrentLine = parser[1];

			DText HdrParser = new DText();
			HdrParser.ATTRMARK = " ";
			HdrParser.MULTMARK = "/";
			HdrParser[0] = CurrentLine;
			
			if (CurrentLine.ToUpper().StartsWith("HTTP/")==true)
			{
				TheMessage.ResponseCode = int.Parse(HdrParser[2]);
				int s1 = CurrentLine.IndexOf(" ");
				s1 = CurrentLine.IndexOf(" ",s1+1);
				TheMessage.ResponseData = HTTPMessage.UnEscapeString(CurrentLine.Substring(s1));
				try
				{
					TheMessage.Version = HdrParser[1,2];
				}
				catch(Exception ex)
				{
                    OpenSource.Utilities.EventLogger.Log(ex);
                    TheMessage.Version = "0.9";
				}
			}
			else
			{
				TheMessage.Directive = HdrParser[1];
				TempString = CurrentLine.Substring(CurrentLine.LastIndexOf(" ") + 1);
				if (TempString.ToUpper().StartsWith("HTTP/")==false)
				{
					TheMessage.Version = "0.9";
					TheMessage.DirectiveObj = HTTPMessage.UnEscapeString(TempString);
				}
				else
				{
					TheMessage.Version = TempString.Substring(TempString.IndexOf("/")+1);
					int fs = CurrentLine.IndexOf(" ") + 1;
					TheMessage.DirectiveObj = HTTPMessage.UnEscapeString(CurrentLine.Substring(
						fs,
						CurrentLine.Length-fs-TempString.Length-1));
				}
			}
			String Tag="";
			String TagData="";

			for(int line=2;line<=parser.DCOUNT();++line)
			{
				if (Tag!="" && parser[line,1].StartsWith(" "))
				{
					TagData = parser[line,1].Substring(1);
				}
				else
				{				
					Tag = parser[line,1];
					TagData = "";
					for(int i=2;i<=parser.DCOUNT(line);++i)
					{
						if (TagData=="")
						{
							TagData = parser[line,i];
						}
						else
						{
							TagData = TagData + parser.MULTMARK + parser[line,i];
						}
					}
				}
				TheMessage.AppendTag(Tag,TagData);
			}
			int cl=0;
			if (TheMessage.HasTag("Content-Length"))
			{
				try
				{
					cl = int.Parse(TheMessage.GetTag("Content-Length"));
				}
				catch(Exception ex)
				{
                    OpenSource.Utilities.EventLogger.Log(ex);
                    cl = -1;
				}
			}
			else
			{
				cl = -1;
			}

			byte[] tbuffer;
			if (cl>0)
			{
				tbuffer = new byte[cl];
				if ((idx+4+cl)>count)
				{
					// NOP
				}
				else
				{
					Array.Copy(buffer,idx+4,tbuffer,0,cl);
					TheMessage.DataBuffer = tbuffer;
				}
			}
			if (cl==-1)
			{
				tbuffer = new Byte[count-(idx+4)];
				Array.Copy(buffer,idx+4,tbuffer,0,tbuffer.Length);
				TheMessage.DataBuffer = tbuffer;
			}
			if (cl==0)
			{
				TheMessage.DataBuffer = new byte[0];
			}
			return(TheMessage);
		}

		private byte[] BuildPacket()
		{
			byte[] buffer;
			if (DataBuffer==null)
			{
				DataBuffer = new Byte[0];
			}
			
			if (Version=="1.0")
			{
				if ((Method=="")&&(ResponseCode==-1))
				{
					return(DataBuffer);
				}
			}


			UTF8Encoding UTF8 = new UTF8Encoding();
			String sbuf;
			IDictionaryEnumerator en = TheHeaders.GetEnumerator();
			en.Reset();

			if (Method!="")
			{
				if (Version!="")
				{
					sbuf = Method + " " + EscapeString(MethodData) + " HTTP/" + Version + "\r\n";
				}
				else
				{
					sbuf = Method + " " + EscapeString(MethodData) + "\r\n";
				}
			}
			else
			{
				sbuf = "HTTP/" + Version + " " + ResponseCode.ToString() + " " + ResponseData + "\r\n";
			}
			while(en.MoveNext())
			{
				if ((String)en.Key!="CONTENT-LENGTH" || OverrideContentLength==true)
				{
					if (en.Value.GetType()==typeof(string))
					{
						sbuf += (String)en.Key + ": " + (String)en.Value + "\r\n";
					}
					else
					{
						sbuf += (String)en.Key + ":";
						foreach(string v in (ArrayList)en.Value)
						{
							sbuf += (" " + v + "\r\n");
						}
					}
				}
			}
			if (StatusCode==-1 && DontShowContentLength==false)
			{
				sbuf += "Content-Length: " + DataBuffer.Length.ToString() + "\r\n";
			}
			else if (Version!="1.0" && Version!="0.9" && Version!="" && DontShowContentLength==false)
			{
				if (OverrideContentLength==false)
				{
					sbuf += "Content-Length: " + DataBuffer.Length.ToString() + "\r\n";
				}
			}

			sbuf += "\r\n";
			
			buffer = new byte[UTF8.GetByteCount(sbuf) + DataBuffer.Length];
			UTF8.GetBytes(sbuf,0,sbuf.Length,buffer,0);
			Array.Copy(DataBuffer,0,buffer,buffer.Length-DataBuffer.Length,DataBuffer.Length);
			return(buffer);
		}

		/// <summary>
		/// Get/Set the HTTP Command
		/// </summary>
		public string Directive
		{
			get
			{
				return(Method);
			}
			set
			{
				Method = value;
			}
		}
		/// <summary>
		/// Get/Set the Object the Command is operating on
		/// </summary>
		public String DirectiveObj
		{
			get
			{
				return(MethodData);
			}
			set
			{
				MethodData = value;
			}
		}

		/// <summary>
		/// Get/Set the HTTP Status Code
		/// </summary>
		public int StatusCode
		{
			get
			{
				return(ResponseCode);
			}
			set
			{
				ResponseCode = value;
			}
		}

		/// <summary>
		/// Get/Set the Extra data associated with the Status Code
		/// </summary>
		public String StatusData
		{
			get
			{
				return(ResponseData);
			}
			set
			{
				ResponseData = value;
			}
		}

		/// <summary>
		/// Escapes a string
		/// </summary>
		/// <param name="TheString"></param>
		/// <returns></returns>
		public static string EscapeString(String TheString)
		{
			UTF8Encoding UTF8 = new UTF8Encoding();
			byte[] buffer = UTF8.GetBytes(TheString);
			StringBuilder s = new StringBuilder();
			
			foreach(byte val in buffer)
			{
				if (((val>=63)&&(val<=90))||
					((val>=97)&&(val<=122))||
					(val>=47 && val<=57)||
					val==59 || val==47 || val==63 ||
					val==58 || val==64 || val==61 ||
					val==43 || val==36 || val==45 ||
					val==95 || val==46 || val==42
					)
				{
					s.Append((char)val);
				}
				else
				{
					s.Append("%" + val.ToString("X"));
				}
			}

			return(s.ToString());
		}
		/// <summary>
		/// Unescapes a string
		/// </summary>
		/// <param name="TheString"></param>
		/// <returns></returns>
		public static string UnEscapeString(string TheString)
		{
			IEnumerator en = TheString.GetEnumerator();
			string t;
			ArrayList Temp = new ArrayList();
			UTF8Encoding UTF8 = new UTF8Encoding();

			while(en.MoveNext())
			{
				if ((char)en.Current=='%')
				{
					en.MoveNext();
					t = new string((char)en.Current,1);
					en.MoveNext();
					t += new string((char)en.Current,1);
					int X = int.Parse(t.ToUpper(),System.Globalization.NumberStyles.HexNumber);
					Temp.Add((byte)X);
				}
				else
				{
					Temp.Add((byte)(int)(char)en.Current);
				}
			}
			return(UTF8.GetString((byte[])Temp.ToArray(typeof(byte))));
		}

		private String Method;
		private String MethodData;
		private int ResponseCode;
		private String ResponseData;

		private Hashtable TheHeaders;
		private byte[] DataBuffer;
		public string Version = "1.1";

		[NonSerialized()] public Object StateObject = null;
		public IPEndPoint LocalEndPoint;
		public IPEndPoint RemoteEndPoint;
	}
}
