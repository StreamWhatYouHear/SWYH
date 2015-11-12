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
using System.Net;
using System.Threading;
using System.Collections;

namespace OpenSource.UPnP
{
    /// <summary>
    /// This is a convenience class to facilitate HTTP communication. This class
    /// handles the creation and sending of requests with an <seealso cref="OpenSource.UPnP.HTTPSession"/>
    /// object. HTTPRequest also allows you to pipeline requests, whether or not the server supports it, 
    /// because that functionality is abstracted away, so even if pipelining is not supported,
    /// this object will serialize the requests, and open seperate sessions.
    /// </summary>
    public sealed class HTTPRequest
    {
        private bool ReceivedFirstResponse = false;

        public bool IdleTimeout = true;
        private static LifeTimeMonitor KeepAliveTimer = new LifeTimeMonitor();
        LifeTimeMonitor.LifeTimeHandler KeepAliveHandler;

        public static bool PIPELINE = true;
        private bool _PIPELINE = true;

        private Hashtable NotPipelinedTable = Hashtable.Synchronized(new Hashtable());

        private HTTPMessage LastMessage;
        public delegate void HeaderHandler(HTTPRequest sender, HTTPSession WebSession, HTTPMessage header, Stream StreamObj, object Tag);
        private IPEndPoint _Source;
        public IPEndPoint Source
        {
            get
            {
                return (_Source);
            }
        }
        private class StateData
        {
            public object Tag = null;
            public HeaderHandler HeaderCB = null;
            public HTTPMessage Request = null;
            public IPEndPoint Dest = null;

            public StateData(HTTPMessage req, IPEndPoint d, object Tag, HeaderHandler HeaderCB)
            {
                this.Dest = d;
                this.Request = req;
                this.Tag = Tag;
                this.HeaderCB = HeaderCB;
            }
        }

        public delegate void InactiveClosedHandler(HTTPRequest sender);
        public event InactiveClosedHandler OnInactiveClosed;

        public delegate void RequestHandler(HTTPRequest sender, HTTPMessage Response, object Tag);
        /// <summary>
        /// Fired when a the response is received
        /// </summary>
        public event RequestHandler OnResponse;
        /// <summary>
        /// Fired when anything is sent/received on the socket
        /// </summary>
        public event RequestHandler OnSniffPacket;

        public delegate void SniffHandler(HTTPRequest sender, byte[] buffer, int offset, int count);
        /// <summary>
        /// Fired when anything is sent/received on the socket
        /// </summary>
        public event SniffHandler OnSniff;

        private HTTPSession s = null;
        private Queue TagQueue = new Queue();


        /// <summary>
        /// Instantiates a new Request Object
        /// </summary>
        public HTTPRequest()
        {
            KeepAliveHandler = new LifeTimeMonitor.LifeTimeHandler(KeepAliveSink);
            KeepAliveTimer.OnExpired += KeepAliveHandler;
            OpenSource.Utilities.InstanceTracker.Add(this);
            _PIPELINE = PIPELINE;
        }


        private void KeepAliveSink(LifeTimeMonitor sender, object obj)
        {
            if (IdleTimeout == false || (int)obj != this.GetHashCode()) return;
            this.ForceCloseSession();
            if (this.OnInactiveClosed != null) this.OnInactiveClosed(this);
        }
        public IPEndPoint ProxySetting = null;

        /// <summary>
        /// Terminates and disposes this object
        /// </summary>
        public void Dispose()
        {
            lock (this.TagQueue)
            {
                HTTPSession x = this.s;
                if (x != null)
                    x.Close();
                s = null;
                TagQueue.Clear();
            }
        }

        public void SetSniffHandlers()
        {
            if (s != null)
            {
                s.OnSniff += new HTTPSession.SniffHandler(SniffSink);
                s.OnSniffPacket += new HTTPSession.ReceiveHandler(SniffPacketSink);
            }
        }
        public void ReleaseSniffHandlers()
        {
            if (s != null)
            {
                s.OnSniff -= new HTTPSession.SniffHandler(SniffSink);
                s.OnSniffPacket -= new HTTPSession.ReceiveHandler(SniffPacketSink);
            }
        }

        internal void ForceCloseSession()
        {
            try
            {
                this.s.Close();
            }
            catch (Exception ex)
            {
                OpenSource.Utilities.EventLogger.Log(ex);
            }
        }

        /// <summary>
        /// Pipelines a request packet
        /// </summary>
        /// <param name="dest">Destination IPEndPoint</param>
        /// <param name="MSG">HTTPMessage Packet</param>
        /// <param name="Tag">State Data</param>
        public void PipelineRequest(IPEndPoint dest, HTTPMessage MSG, object Tag)
        {
            ContinueRequest(dest, "", Tag, MSG);
        }

        /// <summary>
        /// Pipelines a Uri request
        /// </summary>
        /// <param name="Resource">Uri to GET</param>
        /// <param name="Tag">State Data</param>
        public void PipelineRequest(Uri Resource, object Tag)
        {
            object[] Args = new Object[2] { Resource, Tag };

            string IP = Resource.Host;
            if (Resource.HostNameType == UriHostNameType.Dns)
            {
                Dns.BeginGetHostEntry(IP, new AsyncCallback(GetHostByNameSink), (object)Args);
            }
            else
            {
                ContinueRequest(
                    new IPEndPoint(IPAddress.Parse(Resource.Host), Resource.Port),
                    HTTPMessage.UnEscapeString(Resource.PathAndQuery),
                    Tag,
                    null);

            }
        }
        private void GetHostByNameSink(IAsyncResult result)
        {
            IPHostEntry e = null;
            try
            {
                e = Dns.EndGetHostEntry(result);
            }
            catch (Exception ex)
            {
                // Could not resolve?
                OpenSource.Utilities.EventLogger.Log(ex);
                return;
            }

            object[] Args = (object[])result.AsyncState;
            Uri Resource = (Uri)Args[0];
            object Tag = (object)Args[1];

            ContinueRequest(
                new IPEndPoint(e.AddressList[0], Resource.Port),
                HTTPMessage.UnEscapeString(Resource.PathAndQuery),
                Tag,
                null);
        }

        private string RemoveIPv6Scope(string addr)
        {
            int i = addr.IndexOf('%');
            if (i >= 0) addr = addr.Substring(0, i);
            return addr;
        }

        private void ContinueRequest(IPEndPoint dest, string PQ, object Tag, HTTPMessage MSG)
        {
            HTTPMessage r = null;
            if (MSG == null)
            {
                r = new HTTPMessage();
                r.Directive = "GET";
                r.DirectiveObj = PQ;
                if (dest.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) r.AddTag("Host", dest.ToString());
                if (dest.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6) r.AddTag("Host", "[" + RemoveIPv6Scope(dest.ToString()) + "]");
            }
            else
            {
                r = MSG;
            }

            lock (TagQueue)
            {
                this.IdleTimeout = false;
                KeepAliveTimer.Remove(this.GetHashCode());

                LastMessage = r;
                if ((PIPELINE == false && _PIPELINE == false) || (_PIPELINE == false))
                {
                    HTTPRequest TR = new HTTPRequest();
                    TR.ProxySetting = ProxySetting;
                    TR._PIPELINE = true;
                    if (this.OnSniff != null) TR.OnSniff += new HTTPRequest.SniffHandler(NonPipelinedSniffSink);
                    if (this.OnSniffPacket != null) TR.OnSniffPacket += new HTTPRequest.RequestHandler(NonPipelinedSniffPacketSink);
                    TR.OnResponse += new HTTPRequest.RequestHandler(NonPipelinedResponseSink);
                    this.NotPipelinedTable[TR] = TR;
                    TR.PipelineRequest(dest, r, Tag);
                    return;
                }

                bool NeedSend = (TagQueue.Count == 0);
                TagQueue.Enqueue(new StateData(r, dest, Tag, null));

                IPAddress localif = IPAddress.Any;
                if (dest.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6) localif = IPAddress.IPv6Any;

                if (s == null)
                {
                    ReceivedFirstResponse = false;
                    if (ProxySetting != null)
                    {
                        s = new HTTPSession(new IPEndPoint(localif, 0),
                            ProxySetting,
                            new HTTPSession.SessionHandler(CreateSink),
                            new HTTPSession.SessionHandler(CreateFailedSink),
                            null);
                    }
                    else
                    {
                        s = new HTTPSession(new IPEndPoint(localif, 0),
                            dest,
                            new HTTPSession.SessionHandler(CreateSink),
                            new HTTPSession.SessionHandler(CreateFailedSink),
                            null);
                    }
                }
                else
                {
                    if (s.IsConnected && this.ReceivedFirstResponse)
                    {
                        try
                        {
                            if (ProxySetting == null)
                            {
                                s.Send(r);
                            }
                            else
                            {
                                HTTPMessage pr = (HTTPMessage)r.Clone();
                                pr.DirectiveObj = "http://" + dest.ToString() + pr.DirectiveObj;
                                pr.Version = "1.0";
                                s.Send(pr);
                            }
                        }
                        catch (Exception ex)
                        {
                            OpenSource.Utilities.EventLogger.Log(ex);
                        }
                    }
                }
            }
        }

        private void NonPipelinedSniffSink(HTTPRequest sender, byte[] buffer, int offset, int count)
        {
            if (OnSniff != null)
                OnSniff(this, buffer, offset, count);
        }
        private void NonPipelinedSniffPacketSink(HTTPRequest sender, HTTPMessage Response, object Tag)
        {
            if (OnSniffPacket != null)
                OnSniffPacket(this, Response, Tag);
        }

        private void NonPipelinedResponseSink(HTTPRequest sender, HTTPMessage Response, object Tag)
        {
            //			OpenSource.Utilities.EventLogger.Log(sender.s,System.Diagnostics.EventLogEntryType.Information,"TryingToDispose");
            _Source = sender.Source;
            this.NotPipelinedTable.Remove(sender);
            sender.Dispose();
            if (this.OnResponse != null)
                OnResponse(this, Response, Tag);
        }
        private void HeaderSink(HTTPSession sender, HTTPMessage header, Stream TheStream)
        {
            _Source = sender.Source;
            StateData sd = null;
            if (TheStream != null)
            {
                // This is the result of post-headers in a chunked document
                sd = (StateData)sender.StateObject;
                object Tag = sd.Tag;
                if (sd.HeaderCB != null) sd.HeaderCB(this, sender, header, TheStream, Tag);
                sender.StateObject = null;
                KeepAliveTimer.Add(this.GetHashCode(), 10);
            }
            else
            {
                lock (TagQueue)
                {
                    sd = (StateData)TagQueue.Dequeue();
                }
                sender.StateObject = sd;
                object Tag = sd.Tag;
                if (sd.HeaderCB != null)
                {
                    sd.HeaderCB(this, sender, header, TheStream, Tag);
                    if (sender.UserStream != null && !sender.IsChunked)
                    {
                        // If I don't set this to null, this holds a strong reference, resulting in
                        // possible memory leaks
                        sender.StateObject = null;
                    }
                }
            }
        }
        private void ReceiveSink(HTTPSession sender, HTTPMessage msg)
        {
            StateData sd = (StateData)sender.StateObject;
            object Tag = sd.Tag;

            if (msg.Version == "1.0" || msg.Version == "0.9")
            {
                sender.Close();
            }
            else
            {
                if (msg.GetTag("Connection").ToUpper() == "CLOSE")
                {
                    sender.Close();
                }
            }


            if (OnResponse != null) OnResponse(this, msg, Tag);
            // If I don't set this to null, this holds a strong reference, resulting in
            // possible memory leaks
            sender.StateObject = null;
            lock (TagQueue)
            {
                if (TagQueue.Count == 0)
                {
                    this.IdleTimeout = true;
                    KeepAliveTimer.Add(this.GetHashCode(), 10);
                }
            }
        }

        private void SniffSink(byte[] buffer, int offset, int count)
        {
            if (OnSniff != null)
            {
                OnSniff(this, buffer, offset, count);
            }
        }
        private void SniffPacketSink(HTTPSession sender, HTTPMessage MSG)
        {
            if (this.OnSniffPacket != null)
            {
                if (sender.StateObject == null)
                {
                    OnSniffPacket(this, MSG, null);
                    return;
                }
                StateData sd = (StateData)sender.StateObject;
                object Tag = sd.Tag;

                OnSniffPacket(this, MSG, Tag);
            }
        }

        private void StreamDoneSink(HTTPSession sender, Stream StreamObject)
        {
            //ToDo: Place callback from StateData here, to notify Stream is Done
        }

        private void RequestAnsweredSink(HTTPSession ss)
        {
            lock (TagQueue)
            {
                if (!ReceivedFirstResponse)
                {
                    ReceivedFirstResponse = true;
                    IEnumerator en = TagQueue.GetEnumerator();
                    while (en.MoveNext())
                    {
                        StateData sd = (StateData)en.Current;
                        try
                        {
                            if (ProxySetting == null)
                            {
                                ss.Send(sd.Request);
                            }
                            else
                            {
                                HTTPMessage pr = (HTTPMessage)sd.Request.Clone();
                                pr.DirectiveObj = "http://" + sd.Dest.ToString() + pr.DirectiveObj;
                                pr.Version = "1.0";
                                ss.Send(pr);
                            }
                        }
                        catch (Exception ex)
                        {
                            OpenSource.Utilities.EventLogger.Log(ex);
                        }
                    }
                }
            }
        }

        private void CreateSink(HTTPSession ss)
        {
            lock (TagQueue)
            {
                ss.OnHeader += new HTTPSession.ReceiveHeaderHandler(HeaderSink);
                ss.OnReceive += new HTTPSession.ReceiveHandler(ReceiveSink);
                ss.OnClosed += new HTTPSession.SessionHandler(CloseSink);
                ss.OnStreamDone += new HTTPSession.StreamDoneHandler(StreamDoneSink);
                ss.OnRequestAnswered += new HTTPSession.SessionHandler(RequestAnsweredSink);

                if (this.OnSniff != null) ss.OnSniff += new HTTPSession.SniffHandler(SniffSink);
                if (this.OnSniffPacket != null) ss.OnSniffPacket += new HTTPSession.ReceiveHandler(SniffPacketSink);

                StateData sd = (StateData)TagQueue.Peek();

                try
                {
                    if (ProxySetting == null)
                    {
                        ss.Send(sd.Request);
                    }
                    else
                    {
                        HTTPMessage pr = (HTTPMessage)sd.Request.Clone();
                        pr.DirectiveObj = "http://" + sd.Dest.ToString() + pr.DirectiveObj;
                        pr.Version = "1.0";
                        ss.Send(pr);
                    }
                }
                catch (Exception exc)
                {
                    OpenSource.Utilities.EventLogger.Log(exc);
                }
            }
        }

        private void CloseSink(HTTPSession ss)
        {
            bool err = false;
            string erraddr = "";

            ss.CancelAllEvents();
            lock (TagQueue)
            {
                KeepAliveTimer.Remove(this.GetHashCode());

                if (TagQueue.Count > 0)
                {
                    OpenSource.Utilities.EventLogger.Log(this, System.Diagnostics.EventLogEntryType.Information, "Switching Pipeline Modes [" + ss.GetHashCode().ToString() + "]");
                    _PIPELINE = false;
                    if (!ReceivedFirstResponse)
                    {
                        erraddr = ((StateData)TagQueue.Peek()).Dest.ToString();
                    }
                }

                if (!ReceivedFirstResponse)
                {
                    OpenSource.Utilities.EventLogger.Log(this, System.Diagnostics.EventLogEntryType.Error, "Server[" + erraddr + "] closed socket without answering");
                    err = true;
                }

                while (TagQueue.Count > 0)
                {
                    StateData sd = (StateData)TagQueue.Dequeue();
                    if (!err)
                    {
                        HTTPRequest TR = new HTTPRequest();
                        TR.ProxySetting = ProxySetting;
                        TR._PIPELINE = true;
                        if (this.OnSniff != null) TR.OnSniff += new HTTPRequest.SniffHandler(NonPipelinedSniffSink);
                        if (this.OnSniffPacket != null) TR.OnSniffPacket += new HTTPRequest.RequestHandler(NonPipelinedSniffPacketSink);
                        TR.OnResponse += new HTTPRequest.RequestHandler(NonPipelinedResponseSink);
                        this.NotPipelinedTable[TR] = TR;
                        TR.PipelineRequest(sd.Dest, sd.Request, sd.Tag);
                    }
                    else
                    {
                        if (OnResponse != null) OnResponse(this, null, sd.Tag);

                    }
                }
                s = null;
            }
        }

        private void CreateFailedSink(HTTPSession ss)
        {
            lock (TagQueue)
            {
                while (TagQueue.Count > 0)
                {
                    StateData sd = (StateData)TagQueue.Dequeue();
                    OpenSource.Utilities.EventLogger.Log(this, System.Diagnostics.EventLogEntryType.Error, "Connection Attempt to [" + sd.Dest.ToString() + "] Refused/Failed");

                    object Tag = sd.Tag;
                    if (sd.HeaderCB != null)
                    {
                        sd.HeaderCB(this, ss, null, null, Tag);
                    }
                    else
                    {
                        if (this.OnResponse != null) OnResponse(this, null, Tag);
                    }
                }
                s = null;
            }
        }
    }
}
