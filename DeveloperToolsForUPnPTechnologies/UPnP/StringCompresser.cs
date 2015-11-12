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

namespace OpenSource.Utilities
{
	/// <summary>
	/// Summary description for StringCompresser.
	/// </summary>
	public class StringCompressor
	{
		static private bool FindMatch(byte[] srcBuffer, int srcOffset, int srcLength, byte[] TargetBuffer, int TargetOffset, int TargetLength, out int Offset) 
		{
			bool Match,Done;
			int i = -1;
			

			Offset = -1;

			Match = false;
			Done = false;
			if (TargetOffset+TargetLength>TargetBuffer.Length)
			{
				return(false);
			}
			while(Done==false)
			{
				do
				{
					++i;
				}while(i < srcLength && srcBuffer[srcOffset+i]!=TargetBuffer[TargetOffset]);
			
				if (i<srcLength && srcLength-i>=TargetLength)
				{
					//FirstMatch
					Match = true;
					for(int x=0;x<TargetLength;++x)
					{
						if (srcBuffer[srcOffset+i+x]!=TargetBuffer[TargetOffset+x])
						{
							Match=false;
							break;
						}
					}
					if (Match==true)
					{
						//Found Substring Match
						Done = true;
						Offset = i;
					}
				}
				else
				{
					Done = true;
				}
			}
			return(Match);
		}
		static public int BestMatch(byte[] buffer, int Offset, int bufferLength,int WindowSize, int MinMatchSize, int MaxMatchSize, out int MatchOffset, out int MatchLength)
		{
			bool Match = false;
			int UseOffset = Offset-WindowSize<0?0:Offset-WindowSize;
			int UseSize = Offset-UseOffset;
			int _offset,_offset2;
			int currentLength;

			int BestOffset=0,BestLength=MinMatchSize;
			MatchOffset=0;
			int Forward = 0;

			for(int x=0;x<MinMatchSize;++x)
			{
				currentLength = MinMatchSize;
				Match = FindMatch(buffer,UseOffset+Forward,UseSize,buffer,Offset+Forward,currentLength,out _offset);
				if (Match)
				{
					while(Offset+MinMatchSize<=bufferLength && FindMatch(buffer,UseOffset+Forward,UseSize,buffer,Offset+Forward,currentLength,out _offset2))
					{
						_offset = _offset2;
						++currentLength;
					}
					--currentLength;
					if (currentLength>MaxMatchSize) {currentLength = MaxMatchSize;}
					if (Forward==0) {MatchOffset = _offset;}
				}
				
				if (currentLength==MaxMatchSize)
				{
					// Can't Beat this
					MatchLength = currentLength;
					MatchOffset = _offset;
//					UTF8Encoding U = new UTF8Encoding();
//					string z = U.GetString(buffer,UseOffset+Forward+_offset,currentLength);
//					if (UseOffset==3116)
//					{
//						int zz=4;
//					}
					return(Forward);
				}
				else
				{
					if (currentLength>BestLength)
					{
//						if (Forward!=0)
//						{
//							int hmm=5;
//							UTF8Encoding U = new UTF8Encoding();
//							string z = U.GetString(buffer,UseOffset+Forward+_offset,currentLength);
//							int zz=4;
//						}
						BestOffset = Forward;
						BestLength = currentLength;
						MatchOffset = _offset;
					}
				}
				++Forward;
			}
			
			MatchLength = BestLength;
			return(BestOffset);
		}


		static public byte[] CompressString(string str)
		{
			UTF8Encoding U = new UTF8Encoding();
			byte[] inbuf = U.GetBytes(str);

			return(CompressString(inbuf,0,inbuf.Length));
		}
		static private byte[] CompressString(byte[] inbuf, int inbufOffset, int inbufLength)
		{
			return(CompressString(inbuf,inbufOffset,inbufLength,0));
		}
		static private byte[] CompressString(byte[] inbuf, int inbufOffset, int inbufLength,int loop)
		{
			MemoryStream buf = new MemoryStream();
			MemoryStream tmp = new MemoryStream();
			byte nblock;
			uint cblock;
			int written=0;

			int Offset;
			int i=0,ln=0;
			int currentOffset,currentLength;

			currentOffset = inbufOffset;
			currentLength = 4;

			bool UnCompressBlock = false;

			while(currentOffset+currentLength<=inbuf.Length)
			{
				if (written-1023<0)
				{
					i=0;
				}
				else
				{
					i=written-1023;
				}
				ln = written-i;
				if (FindMatch(inbuf,i,ln,inbuf,currentOffset,currentLength,out Offset))
				{
					int OptimumOffset = BestMatch(inbuf,currentOffset,inbuf.Length,1023,4,63,out Offset,out currentLength);
					for(int ctr=0;ctr<OptimumOffset;++ctr)
					{
						tmp.Write(inbuf,currentOffset,1);
						++written;
						if (tmp.Length==255)
						{
							if (UnCompressBlock==true)
							{
								buf.Write(new byte[2]{0,0},0,2);
							}
							buf.Write(new byte[1]{(byte)255},0,1);
							buf.Write(tmp.ToArray(),0,(int)tmp.Length);
							tmp = new MemoryStream();
							UnCompressBlock = true;
						}
						++i;
						++currentOffset;
					}

					if (tmp.Length>0)
					{
						if (UnCompressBlock==true)
						{
							buf.Write(new byte[2]{0,0},0,2);
						}
						nblock = (byte)tmp.Length;
						buf.Write(BitConverter.GetBytes(nblock),0,1);
						buf.Write(tmp.ToArray(),0,(int)tmp.Length);
						tmp = new MemoryStream();
						UnCompressBlock = true;
					}

					Offset = i+Offset;
					Offset = written-Offset;

					cblock = (uint)Offset;
					cblock = cblock<<6;
					cblock = cblock|(uint)currentLength;

					if (UnCompressBlock==false)
					{
						buf.Write(new Byte[1]{(byte)0},0,1);
					}
					UTF8Encoding U = new UTF8Encoding();
					string zzz = U.GetString(inbuf,written-Offset,currentLength);
					string wz = U.GetString(inbuf,0,written);

	
					written += currentLength;
					buf.Write(BitConverter.GetBytes(cblock),0,1);
					buf.Write(BitConverter.GetBytes(cblock),1,1);

					currentOffset+=currentLength;
					currentLength = 4;
					UnCompressBlock = false;
				}
				else
				{
					//No Match, just copy
					tmp.Write(inbuf,currentOffset,1);
					++written;
					if (tmp.Length==255)
					{
						if (UnCompressBlock==true)
						{
							buf.Write(new byte[2]{0,0},0,2);
						}
						buf.Write(new byte[1]{(byte)255},0,1);
						buf.Write(tmp.ToArray(),0,(int)tmp.Length);
						tmp = new MemoryStream();
						UnCompressBlock = true;
					}
					++currentOffset;
				}
			}
			if (tmp.Length>0)
			{
				if (UnCompressBlock==true)
				{
					buf.Write(new byte[2]{0,0},0,2);
				}
				nblock = (byte)tmp.Length;
				buf.Write(BitConverter.GetBytes(nblock),0,1);
				buf.Write(tmp.ToArray(),0,(int)tmp.Length);
				tmp = new MemoryStream();
				UnCompressBlock = true;
			}
			if (inbuf.Length-currentOffset!=0)
			{
				if (UnCompressBlock==true)
				{
					buf.Write(new byte[2]{0,0},0,2);
				}
				nblock = (byte)(inbuf.Length-currentOffset);
				buf.Write(BitConverter.GetBytes(nblock),0,1);
				buf.Write(inbuf,currentOffset,inbuf.Length-currentOffset);
				UnCompressBlock = true;
			}
			if (UnCompressBlock==true)
			{
				buf.Write(new byte[2]{0,0},0,2);
			}
			return(buf.ToArray());
		}
		static public string DecompressString(byte[] buffer,int offset, int length)
		{
			UTF8Encoding U = new UTF8Encoding();
			MemoryStream OutBuf = new MemoryStream();
			int pos=0;
			byte nblock;
			uint cblock;
			uint lmask = 63;

			int coffset,clength;

			while(pos<length-offset)
			{
				nblock = buffer[pos];
				if (nblock!=0)
				{
					OutBuf.Write(buffer,pos+1,(int)nblock);
					pos += (1+(int)nblock);
				}
				else
				{
					++pos;
				}
				if (pos<length-offset)
				{
					cblock = BitConverter.ToUInt16(buffer,pos);
					if (cblock==0)
					{
						//No Compression Block, SKIP
						pos+=2;
					}
					else
					{
						clength = (int)(cblock&lmask);
						coffset = (int)(cblock>>6);
//						if (OutBuf.Length<1023)
//						{
//							cpos = coffset;
//						}
//						else
//						{
//							cpos = (int)OutBuf.Length-1023+coffset;
//						}
//						z1 = U.GetString(OutBuf.ToArray(),0,(int)OutBuf.Length);
//						z2 = U.GetString(OutBuf.ToArray(),cpos,clength);
						OutBuf.Write(OutBuf.ToArray(),(int)OutBuf.Length-(int)coffset,clength);
						pos+=2;
					}
				}
			}
			return(U.GetString(OutBuf.ToArray()));
		}
	}
}
