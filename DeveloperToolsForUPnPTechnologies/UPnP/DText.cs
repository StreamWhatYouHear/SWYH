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

namespace OpenSource.UPnP
{
	/// <summary>
	/// Summary description for DText.
	/// </summary>
	public class DText
	{
		/*
		public string ATTRMARK = "^";
		public string MULTMARK = "]";
		public string SUBVMARK = "/";
		*/
		public string ATTRMARK = "\x80";
		public string MULTMARK = "\x81";
		public string SUBVMARK = "\x82";

		private ArrayList ATTRLIST = new ArrayList();

		public DText()
		{
		}
		public DText(string STR)
		{
			ParseString(STR);
		}

		public int DCOUNT()
		{
			return(ATTRLIST.Count);
		}
		public int DCOUNT(int A)
		{
			if (A==0) return(DCOUNT());
			if (ATTRLIST.Count<A) return(0);
			return(((ArrayList)ATTRLIST[A-1]).Count);
		}
		public int DCOUNT(int A, int M)
		{
			if (M==0) return(DCOUNT(A));
			if (ATTRLIST.Count<A) return(0);
			if (((ArrayList)ATTRLIST[A-1]).Count<M) return(0);
			return(((ArrayList)((ArrayList)ATTRLIST[A-1])[M-1]).Count);
		}

		public string this[int A]
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				if (A>0)
				{
					int i = DCOUNT(A);
					for(int x=1;x<=i;++x)
					{
						if (x!=1) sb.Append(MULTMARK);
						sb.Append(this[A,x]);
					}
					return(sb.ToString());
				}
				else
				{
					int i = DCOUNT();
					for(int x=1;x<=i;++x)
					{
						if (x!=1) sb.Append(ATTRMARK);
						sb.Append(this[x]);
					}
					return(sb.ToString());
				}
			}
			set
			{
				if (A==0)
				{
					ATTRLIST = this.ParseString(value);
				}
				else
				{
					while(ATTRLIST.Count<A)
						ATTRLIST.Add(new ArrayList());

					ArrayList t = ParseString(value);
					if (t.Count>1)
					{
						// ATTRMARKS in this
						ATTRLIST.Insert(A-1,t);
					}
					else
					{
						// No ATTRMARKS
						if (ATTRLIST.Count<A-1)
						{
							ATTRLIST.Insert(A-1,t[0]);
						}
						else
						{
							ATTRLIST[A-1] = t[0];
						}
					}
				}
			}
		}
		public string this[int A, int M]
		{
			get
			{
				if (M==0) return(this[A]);

				StringBuilder sb = new StringBuilder();
				int i = DCOUNT(A,M);
				for(int x=1;x<=i;++x)
				{
					if (x!=1) sb.Append(SUBVMARK);
					sb.Append(this[A,M,x]);
				}
				return(sb.ToString());
			}
			set
			{
				if (M==0)
				{
					this[A] = value;
					return;
				}

				while(ATTRLIST.Count<A)
					ATTRLIST.Add(new ArrayList());
				while(((ArrayList)ATTRLIST[A-1]).Count<M)
					((ArrayList)ATTRLIST[A-1]).Add(new ArrayList());

				ArrayList t = ParseString(value);
				if (t.Count>1)
				{
					// There are ATTRMARKs in this
					ATTRLIST.Insert(A-1,t);
				}
				else
				{
					if (((ArrayList)t[0]).Count>1)
					{
						// There are MULTMARKS in this
						((ArrayList)ATTRLIST[A-1]).Insert(M-1,t[0]);
					}
					else
					{
						// Only SUBVMARK stuff						
						((ArrayList)ATTRLIST[A-1])[M-1] = ((ArrayList)((ArrayList)t[0])[0]);
					}
				}
			}
		}
		public string this[int A, int M, int V]
		{
			get
			{
				if (V==0) return(this[A,M]);

				try
				{
					return((string)((ArrayList)((ArrayList)ATTRLIST[A-1])[M-1])[V-1]);
				}
				catch(Exception ex)
				{
                    OpenSource.Utilities.EventLogger.Log(ex);
                    return ("");
				}
			}
			set
			{
				if (V==0)
				{
					this[A,M] = value;
					return;
				}
				while(ATTRLIST.Count<A)
					ATTRLIST.Add(new ArrayList());
				while(((ArrayList)ATTRLIST[A-1]).Count<M)
					((ArrayList)ATTRLIST[A-1]).Add(new ArrayList());
				while(((ArrayList)((ArrayList)ATTRLIST[A-1])[M-1]).Count<V)
					((ArrayList)((ArrayList)ATTRLIST[A-1])[M-1]).Add(new ArrayList());

				((ArrayList)((ArrayList)ATTRLIST[A-1])[M-1])[V-1] = value;
			}
		}
		
		private ArrayList ParseString(string STR)
		{
			if (STR.Length==0)
			{
				ArrayList Temp = new ArrayList();
				Temp.Add(new ArrayList());
				((ArrayList)Temp[0]).Add(new ArrayList());
				return(Temp);
			}

			int ATTRCTR = 1;
			int MULTCTR = 1;
			int SUBVCTR = 1;

			StringBuilder sb = new StringBuilder();
			ArrayList _ATTRLIST = new ArrayList();

			int i=0;
			while(i<STR.Length)
			{
				while(_ATTRLIST.Count<ATTRCTR)
					_ATTRLIST.Add(new ArrayList());
				while(((ArrayList)_ATTRLIST[ATTRCTR-1]).Count<MULTCTR)
					((ArrayList)_ATTRLIST[ATTRCTR-1]).Add(new ArrayList());
				while(((ArrayList)((ArrayList)_ATTRLIST[ATTRCTR-1])[MULTCTR-1]).Count<SUBVCTR)
					((ArrayList)((ArrayList)_ATTRLIST[ATTRCTR-1])[MULTCTR-1]).Add(new ArrayList());


				string t = STR.Substring(i,1);
				if (t==ATTRMARK.Substring(0,1) || t==MULTMARK.Substring(0,1) || t==SUBVMARK.Substring(0,1))
				{
					bool isATTRMARK = false;
					bool isMULTMARK = false;
					bool isSUBVMARK = false;

					if (i+ATTRMARK.Length<=STR.Length)
					{
						if (STR.Substring(i,ATTRMARK.Length)==ATTRMARK)
						{
							isATTRMARK = true;
							i+=(ATTRMARK.Length-1);
						}
					}
					if (i+MULTMARK.Length<=STR.Length)
					{
						if (STR.Substring(i,MULTMARK.Length)==MULTMARK)
						{
							isMULTMARK = true;
							i+=(MULTMARK.Length-1);
						}
					}
					if (i+SUBVMARK.Length<=STR.Length)
					{
						if (STR.Substring(i,SUBVMARK.Length)==SUBVMARK)
						{
							isSUBVMARK = true;
							i+=(SUBVMARK.Length-1);
						}
					}

					if (isATTRMARK||isMULTMARK||isSUBVMARK)
					{
						((ArrayList)((ArrayList)_ATTRLIST[ATTRCTR-1])[MULTCTR-1])[SUBVCTR-1] = sb.ToString();
						sb = new StringBuilder();

						if (isATTRMARK) 
						{
							++ATTRCTR;
							MULTCTR = 1;
							SUBVCTR = 1;
						}
						if (isMULTMARK) 
						{
							++MULTCTR;
							SUBVCTR = 1;
						}
						if (isSUBVMARK) ++SUBVCTR;
					}
					else
					{
						sb.Append(t);
					}
				}
				else
				{
					sb.Append(t);
				}
				++i;
			}
			if (sb.Length>0)
			{
				((ArrayList)((ArrayList)_ATTRLIST[ATTRCTR-1])[MULTCTR-1])[SUBVCTR-1] = sb.ToString();
			}
			else
			{
				while(_ATTRLIST.Count<ATTRCTR)
					_ATTRLIST.Add(new ArrayList());
				while(((ArrayList)_ATTRLIST[ATTRCTR-1]).Count<MULTCTR)
					((ArrayList)_ATTRLIST[ATTRCTR-1]).Add(new ArrayList());
				while(((ArrayList)((ArrayList)_ATTRLIST[ATTRCTR-1])[MULTCTR-1]).Count<SUBVCTR)
					((ArrayList)((ArrayList)_ATTRLIST[ATTRCTR-1])[MULTCTR-1]).Add(new ArrayList());

				((ArrayList)((ArrayList)_ATTRLIST[ATTRCTR-1])[MULTCTR-1])[SUBVCTR-1] = "";
			}
			return(_ATTRLIST);
		}
	}
}
