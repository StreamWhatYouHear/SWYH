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
using System.Xml;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.Globalization;
using System.Security.Cryptography;
using OpenSource.Utilities;

namespace OpenSource.UPnP
{

    public class MiniWebServerException : Exception
    {
        public MiniWebServerException(String x) : base(x) { }
    }

    /// <summary>
    /// A generic Web Server
    /// </summary>
    public sealed class MiniWebServer
    {
        public bool IdleTimeout = true;
        private LifeTimeMonitor KeepAliveTimer = new LifeTimeMonitor();
        private LifeTimeMonitor SessionTimer = new LifeTimeMonitor();

        private Hashtable SessionTable = new Hashtable();
        private IPEndPoint endpoint_local;

        public delegate void NewSessionHandler(MiniWebServer sender, HTTPSession session);
        private WeakEvent OnSessionEvent = new WeakEvent();
        
        /// <summary>
        /// Triggered when a new session is created
        /// </summary>
        public event NewSessionHandler OnSession
        {
            add { OnSessionEvent.Register(value); }
            remove { OnSessionEvent.UnRegister(value); }
        }

        public delegate void HTTPReceiveHandler(HTTPMessage msg, HTTPSession WebSession);
        private WeakEvent OnReceiveEvent = new WeakEvent();
        
        /// <summary>
        /// This is triggered when an HTTP packet is received
        /// </summary>
        public event HTTPReceiveHandler OnReceive
        {
            add { OnReceiveEvent.Register(value); }
            remove { OnReceiveEvent.UnRegister(value); }
        }
        private WeakEvent OnHeaderEvent = new WeakEvent();
        
        /// <summary>
        /// Triggered when an HTTP Header is received
        /// </summary>
        public event HTTPReceiveHandler OnHeader
        {
            add { OnHeaderEvent.Register(value); }
            remove { OnHeaderEvent.UnRegister(value); }
        }

        /// <summary>
        /// Instantiates a new MiniWebServer listenting on the given EndPoint
        /// </summary>
        /// <param name="local">IPEndPoint to listen on</param>
        public MiniWebServer(IPEndPoint local)
        {
            OpenSource.Utilities.InstanceTracker.Add(this);

            SessionTimer.OnExpired += new LifeTimeMonitor.LifeTimeHandler(SessionTimerSink);
            KeepAliveTimer.OnExpired += new LifeTimeMonitor.LifeTimeHandler(KeepAliveSink);
            endpoint_local = local;

            MainSocket = new Socket(local.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            MainSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            MainSocket.Bind(local);

            if (MainSocket.LocalEndPoint != null) endpoint_local = (IPEndPoint)MainSocket.LocalEndPoint;
            MainSocket.Listen(25);
            MainSocket.BeginAccept(new AsyncCallback(Accept), null);
            KeepAliveTimer.Add(false, 7);
        }

        ~MiniWebServer()
        {
            Dispose();
        }

        private void SessionTimerSink(LifeTimeMonitor sender, object obj)
        {
            HTTPSession s = (HTTPSession)obj;

            s.Close();
        }

        private void KeepAliveSink(LifeTimeMonitor sender, object obj)
        {
            if (IdleTimeout == false) return;
            ArrayList RemoveList = new ArrayList();
            lock (SessionTable)
            {

                IDictionaryEnumerator en = SessionTable.GetEnumerator();
                while (en.MoveNext())
                {
                    if (((HTTPSession)en.Value).Monitor.IsTimeout())
                    {
                        RemoveList.Add(en.Value);
                    }
                }
            }
            foreach (HTTPSession HS in RemoveList)
            {
                HS.Close();
            }
            KeepAliveTimer.Add(false, 7);
        }

        public void Dispose()
        {
            MainSocket.Close();
        }

        private void HandleHeader(HTTPSession sender, HTTPMessage Header, Stream StreamObject)
        {
            SessionTimer.Remove(sender);
            OnHeaderEvent.Fire(Header, sender);
        }
        private void HandleRequest(HTTPSession WebSession, HTTPMessage request)
        {
            OnReceiveEvent.Fire(request, WebSession);
        }
        private void CloseSink(HTTPSession s)
        {
            lock (SessionTable)
            {
                SessionTable.Remove(s);
            }
        }

        private void Accept(IAsyncResult result)
        {
            HTTPSession WebSession = null;

            try
            {
                Socket AcceptedSocket = MainSocket.EndAccept(result);
                lock (SessionTable)
                {
                    WebSession = new HTTPSession(this.LocalIPEndPoint, AcceptedSocket);
                    WebSession.OnClosed += new HTTPSession.SessionHandler(CloseSink);
                    WebSession.OnHeader += new HTTPSession.ReceiveHeaderHandler(HandleHeader);
                    WebSession.OnReceive += new HTTPSession.ReceiveHandler(HandleRequest);
                    SessionTable[WebSession] = WebSession;
                }
                SessionTimer.Add(WebSession, 3);
                OnSessionEvent.Fire(this, WebSession);
                WebSession.StartReading();
            }
            catch (Exception err)
            {
                if (err.GetType() != typeof(System.ObjectDisposedException))
                {
                    // Error
                    OpenSource.Utilities.EventLogger.Log(err);
                }
            }

            try
            {
                MainSocket.BeginAccept(new AsyncCallback(Accept), null);
            }
            catch (Exception ex)
            {
                OpenSource.Utilities.EventLogger.Log(ex);
                // Socket was closed
            }
        }

        public IPEndPoint LocalIPEndPoint
        {
            get
            {
                return (endpoint_local);
            }
        }

        private Socket MainSocket;
    }
}
