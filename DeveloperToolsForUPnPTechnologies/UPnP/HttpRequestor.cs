/*
Copyright 2009-2010 Intel Corporation

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
using System.Net;
using System.Xml;
using System.Text;
using System.Collections;
using System.Net.Sockets;
using System.IO.Compression;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;

namespace OpenSource.UPnP
{
    public class HttpRequestor
    {
        private class RequestState
        {
            public string url;
            public int retrycount;
            public X509CertificateCollection certificates;
            public HttpWebRequest request;
            public HttpWebResponse response;
            public object tag;
            public byte[] postdata;
        }

        public delegate void RequestCompletedHandler(HttpRequestor sender, bool success, object tag, string url, byte[] data);
        public event RequestCompletedHandler OnRequestCompleted;

        public int Retrys = 3;
        public int Timeout = 300000;
        public int PendingRequests = 0;


        public void LaunchProxyRequest(string url, byte[] postData, object tag)
        {
            // Lets create our HTTP request state object
            RequestState r = new RequestState();
            r.url = url;
            r.certificates = null;
            r.retrycount = Retrys;
            r.tag = tag;
            r.request = (HttpWebRequest)HttpWebRequest.Create(url);
            r.request.ConnectionGroupName = url;
            r.request.Timeout = Timeout;
            r.request.Credentials = null;
            r.postdata = postData;

            /*
            // Setup the cache policy
            if (MeshUtils.IsMono() == false)
            {
                r.request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            }
            */

            PendingRequests++;
            // Setup post data if present
            if (postData != null)
            {
                byte[] data = postData;
                r.request.Method = "POST";
                r.request.ContentType = "application/octet";
                r.request.ContentLength = data.Length;
                r.request.Referer = "";
                r.request.BeginGetRequestStream(new AsyncCallback(GetRequestAsyncCallback), r);
            }
            else
            {
                r.request.BeginGetResponse(new AsyncCallback(GetResponseAsyncCallback), r);
            }
        }

        public void LaunchRequest(string url, byte[] postData, X509CertificateCollection certificates, NetworkCredential credentials, object tag)
        {
            // Lets create our HTTP request state object
            RequestState r = new RequestState();
            r.url = url;
            r.certificates = certificates;
            r.retrycount = Retrys;
            r.tag = tag;
            r.request = (HttpWebRequest)HttpWebRequest.Create(url);
            r.request.Proxy = null;
            r.request.ConnectionGroupName = url;
            r.request.Timeout = Timeout;
            r.postdata = postData;

            // Setup the credentials for HTTP digest
            if (credentials != null)
            {
                CredentialCache myCache = new CredentialCache();
                myCache.Add(new System.Uri(url), "Digest", credentials);
                r.request.Credentials = myCache;
                r.request.PreAuthenticate = true;
            }
            else
            {
                r.request.Credentials = null;
            }

            /*
            // Setup the cache policy
            if (MeshUtils.IsMono() == false)
            {
                r.request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            }
            */

            PendingRequests++;
            // Setup post data if present
            if (postData != null)
            {
                byte[] data = postData;
                r.request.Method = "POST";
                r.request.ContentType = "application/octet";
                r.request.ContentLength = data.Length;
                r.request.Referer = "";
                r.request.BeginGetRequestStream(new AsyncCallback(GetRequestAsyncCallback), r);
            }
            else
            {
                r.request.BeginGetResponse(new AsyncCallback(GetResponseAsyncCallback), r);
            }
        }

        private void GetRequestAsyncCallback(IAsyncResult ar)
        {
            // Get the request
            RequestState r = (RequestState)ar.AsyncState;
            Stream s = null;
            try
            {
                s = r.request.EndGetRequestStream(ar);
                s.Write(r.postdata, 0, r.postdata.Length);
                s.Close();
            }
            catch (Exception ex) 
            {
                OpenSource.Utilities.EventLogger.Log(ex);
                s = null; 
            }
            if (s == null)
            {
                PendingRequests--;
                if (OnRequestCompleted != null) OnRequestCompleted(this, false, r.tag, r.url, null);
                return;
            }
            r.request.BeginGetResponse(new AsyncCallback(GetResponseAsyncCallback), r);
        }

        private void GetResponseAsyncCallback(IAsyncResult ar)
        {
            // Get the response
            RequestState r = (RequestState)ar.AsyncState;
            try
            {
                r.response = (HttpWebResponse)r.request.EndGetResponse(ar);
            }
            catch (Exception /* ex */ ) 
            {
                // OpenSource.Utilities.EventLogger.Log(ex);
            }
            if (r.response == null)
            {
                PendingRequests--;
                if (OnRequestCompleted != null) OnRequestCompleted(this, false, r.tag, r.url, null);
                return;
            }

            byte[] buf;
            try
            {
                // Now pull everything out of the HTTP response stream
                Stream sss = r.response.GetResponseStream();
                if (r.response.ContentLength > 0)
                {
                    buf = new byte[(int)(r.response.ContentLength)];
                    int left = (int)(r.response.ContentLength);
                    int pos = 0;
                    int len = sss.Read(buf, pos, left);
                    while (len != 0)
                    {
                        pos += len;
                        left -= len;
                        len = sss.Read(buf, pos, left);
                    }
                }
                else
                {
                    MemoryStream mem = new MemoryStream();
                    buf = new byte[10000];
                    int len = sss.Read(buf, 0, 10000);
                    while (len != 0)
                    {
                        mem.Write(buf, 0, len);
                        len = sss.Read(buf, 0, 10000);
                    }
                    buf = mem.ToArray();
                }
            }
            catch (Exception ex)
            {
                OpenSource.Utilities.EventLogger.Log(ex);
                PendingRequests--;
                if (OnRequestCompleted != null) OnRequestCompleted(this, false, r.tag, r.url, null);
                return;
            }

            // Notify the caller of the response
            PendingRequests--;
            if (OnRequestCompleted != null) OnRequestCompleted(this, true, r.tag, r.url, buf);
        }


    }
}
