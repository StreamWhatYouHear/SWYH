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
using System.Net.Sockets;
using OpenSource.Utilities;

namespace OpenSource.UPnP
{
    /// <summary>
    /// An event based AsyncSocket Object
    /// </summary>
    public sealed class AsyncSocket
    {
        public class StopReadException : Exception
        {
            public StopReadException()
                : base("User initiated StopRead")
            {
            }
        }

        private Thread StopThread = null;
        private bool SentDisconnect = false;


        public void StopReading()
        {
            StopThread = Thread.CurrentThread;
        }

        /// <summary>
        /// The number of bytes read
        /// </summary>
        public int BufferReadLength = 0;
        /// <summary>
        /// The index to begin reading
        /// <para>
        /// setting the BeginPointer equal to the BufferSize has
        /// the same result as setting it to zero
        /// </para>
        /// </summary>
        public int BufferBeginPointer = 0;
        internal int BufferEndPointer = 0;
        /// <summary>
        /// The size of the data chunk
        /// </summary>
        public int BufferSize = 0;

        private Socket MainSocket;
        private IPEndPoint endpoint_local;

        private object SendLock;

        private AsyncCallback ReceiveCB;
        private AsyncCallback SendCB;
        private AsyncCallback ConnectCB;

        private int PendingBytesSent;
        private object CountLock;
        private long TotalBytesSent;

        private EndPoint rEP;
        private Byte[] MainBuffer;
        private Queue SendQueue;

        public delegate void OnReceiveHandler(AsyncSocket sender, Byte[] buffer, int HeadPointer, int BufferSize, int BytesRead, IPEndPoint source, IPEndPoint remote);
        private WeakEvent OnReceiveEvent = new WeakEvent();
        /// <summary>
        /// This is triggered when there is data to be processed
        /// </summary>
        public event OnReceiveHandler OnReceive
        {
            add
            {
                OnReceiveEvent.Register(value);
            }
            remove
            {
                OnReceiveEvent.UnRegister(value);
            }
        }

        public delegate void OnSendReadyHandler(object Tag);
        private WeakEvent OnSendReadyEvent = new WeakEvent();
        /// <summary>
        /// This is triggered when the SendQueue is ready for more
        /// </summary>
        public event OnSendReadyHandler OnSendReady
        {
            add
            {
                OnSendReadyEvent.Register(value);
            }
            remove
            {
                OnSendReadyEvent.UnRegister(value);
            }
        }

        public delegate void ConnectHandler(AsyncSocket sender);
        private WeakEvent OnConnectEvent = new WeakEvent();
        private WeakEvent OnConnectFailedEvent = new WeakEvent();
        private WeakEvent OnDisconnectEvent = new WeakEvent();
        /// <summary>
        /// This is triggered when a Connection attempt was successful
        /// </summary>
        public event ConnectHandler OnConnect
        {
            add
            {
                OnConnectEvent.Register(value);
            }
            remove
            {
                OnConnectEvent.UnRegister(value);
            }
        }
        /// <summary>
        /// This is triggered when a Connection attempt failed
        /// </summary>
        public event ConnectHandler OnConnectFailed
        {
            add
            {
                OnConnectFailedEvent.Register(value);
            }
            remove
            {
                OnConnectFailedEvent.UnRegister(value);
            }
        }
        /// <summary>
        /// This is triggered when the underlying socket closed
        /// </summary>
        public event ConnectHandler OnDisconnect
        {
            add
            {
                OnDisconnectEvent.Register(value);
            }
            remove
            {
                OnDisconnectEvent.UnRegister(value);
            }
        }

        private EndPoint LocalEP;
        private EndPoint RemoteEP;

        private Stream _WriteStream = null;


        private struct SendInfo
        {
            public Byte[] buffer;
            public int offset;
            public int count;
            public object Tag;
            public IPEndPoint dest;
        }


        /// <summary>
        /// Creates a new AsyncSocket, with a stream object to write to
        /// </summary>
        /// <param name="WriteStream">The Stream to use</param>
        public AsyncSocket(Stream WriteStream)
        {
            _WriteStream = WriteStream;
            MainBuffer = new byte[4096];
        }
        /// <summary>
        /// Creates a new AsyncSocket, with a fixed size buffer to write to
        /// </summary>
        /// <param name="BufferSize">Size of buffer</param>
        public AsyncSocket(int BufferSize)
        {
            MainBuffer = new byte[BufferSize];
        }
        /// <summary>
        /// Attaches this AsyncSocket to a new Socket instance, using the given info.
        /// </summary>
        /// <param name="local">Local interface to use</param>
        /// <param name="PType">Protocol Type</param>
        public void Attach(IPEndPoint local, ProtocolType PType)
        {
            endpoint_local = local;
            TotalBytesSent = 0;
            LocalEP = (EndPoint)local;
            Init();

            MainSocket = null;

            if (PType == ProtocolType.Tcp)
            {
                MainSocket = new Socket(local.AddressFamily, SocketType.Stream, PType);
            }

            if (PType == ProtocolType.Udp)
            {
                MainSocket = new Socket(local.AddressFamily, SocketType.Dgram, PType);
                MainSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            }

            if (MainSocket != null)
            {
                MainSocket.Bind(local);
                System.Reflection.PropertyInfo pi = MainSocket.GetType().GetProperty("UseOnlyOverlappedIO");
                if (pi != null)
                {
                    pi.SetValue(MainSocket, true, null);
                }
            }
            else
            {
                throw (new Exception(PType.ToString() + " not supported"));
            }
        }
        /// <summary>
        /// Attach this AsyncSocket to an existing Socket
        /// </summary>
        /// <param name="UseThisSocket">The Socket</param>
        public void Attach(Socket UseThisSocket)
        {
            endpoint_local = (IPEndPoint)UseThisSocket.LocalEndPoint;
            TotalBytesSent = 0;
            LocalEP = UseThisSocket.LocalEndPoint;
            if (UseThisSocket.SocketType == SocketType.Stream)
            {
                RemoteEP = UseThisSocket.RemoteEndPoint;
                endpoint_local = (IPEndPoint)UseThisSocket.LocalEndPoint;
            }
            else
            {
                RemoteEP = null;
            }
            MainSocket = UseThisSocket;
            System.Reflection.PropertyInfo pi = MainSocket.GetType().GetProperty("UseOnlyOverlappedIO");
            if (pi != null)
            {
                pi.SetValue(MainSocket, true, null);
            }
            Init();
        }


        public void SetTTL(int TTL)
        {
            MainSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, TTL);
        }
        /// <summary>
        /// Join a multicast group
        /// </summary>
        /// <param name="local">Interface to use</param>
        /// <param name="MulticastAddress">MulticastAddress to join</param>
        public void AddMembership(IPEndPoint local, IPAddress MulticastAddress)
        {
            try
            {
                MainSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, 1);
            }
            catch (Exception ex)
            {
                OpenSource.Utilities.EventLogger.Log(ex);
                // This will only fail if the network stack does not support this
                // Which means you are probably running Win9x
            }

            try
            {
                MainSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(MulticastAddress));
            }
            catch (Exception ex)
            {
                OpenSource.Utilities.EventLogger.Log(ex);
                OpenSource.Utilities.EventLogger.Log(this, System.Diagnostics.EventLogEntryType.Error, "Cannot AddMembership to IPAddress: " + MulticastAddress.ToString());
            }
            try
            {
                MainSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, local.Address.GetAddressBytes());
            }
            catch (Exception ex)
            {
                OpenSource.Utilities.EventLogger.Log(ex);
                OpenSource.Utilities.EventLogger.Log(this, System.Diagnostics.EventLogEntryType.Error, "Cannot Set Multicast Interface to IPAddress: " + local.Address.ToString());
            }
        }
        /// <summary>
        /// Leave a multicast group
        /// </summary>
        /// <param name="MulticastAddress">Multicast Address to leave</param>
        public void DropMembership(IPAddress MulticastAddress)
        {
            MainSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DropMembership, new MulticastOption(MulticastAddress));
        }

        /// <summary>
        /// Number of bytes in the send queue pending
        /// </summary>
        public int Pending
        {
            get { return (PendingBytesSent); }
        }

        /// <summary>
        /// Total bytes send
        /// </summary>
        public long Total
        {
            get { return (TotalBytesSent); }
        }

        /// <summary>
        /// The Local EndPoint
        /// </summary>
        public EndPoint LocalEndPoint
        {
            get
            {
                if (MainSocket.LocalEndPoint != null)
                {
                    return ((IPEndPoint)MainSocket.LocalEndPoint);
                }
                else
                {
                    return (endpoint_local);
                }
            }
        }

        /// <summary>
        /// The Remote EndPoint
        /// </summary>
        public EndPoint RemoteEndPoint
        {
            get { return (MainSocket.RemoteEndPoint); }
        }

        /// <summary>
        /// Connect to a remote socket
        /// </summary>
        /// <param name="Remote">IPEndPoint to connect to</param>
        public void Connect(IPEndPoint Remote)
        {
            if (MainSocket.SocketType != SocketType.Stream)
            {
                throw (new Exception("Cannot connect a non StreamSocket"));
            }
            System.Reflection.PropertyInfo pi = MainSocket.GetType().GetProperty("UseOnlyOverlappedIO");
            if (pi != null)
            {
                pi.SetValue(MainSocket, true, null);
            }
            MainSocket.BeginConnect(Remote, ConnectCB, null);
        }

        private void HandleConnect(IAsyncResult result)
        {
            bool IsOK = false;
            Exception xx;
            try
            {
                MainSocket.EndConnect(result);
                IsOK = true;
                RemoteEP = MainSocket.RemoteEndPoint;
            }
            catch (Exception x)
            {
                OpenSource.Utilities.EventLogger.Log(x);
                xx = x;
            }

            if ((IsOK == true) && (MainSocket.Connected == true))
            {
                OnConnectEvent.Fire(this);
            }
            else
            {
                OnConnectFailedEvent.Fire(this);
            }
        }

        /// <summary>
        /// Start AsyncReads
        /// </summary>
        /// <returns>Successfully started</returns>
        public void Begin()
        {
            bool Disconnect = false;
            IPEndPoint src, from;

            if (MainSocket.SocketType == SocketType.Stream)
            {
                from = (IPEndPoint)MainSocket.RemoteEndPoint;
            }
            else
            {
                from = (IPEndPoint)rEP;
            }

            try
            {
                src = (IPEndPoint)MainSocket.LocalEndPoint;
            }
            catch (Exception ex)
            {
                OpenSource.Utilities.EventLogger.Log(ex);
                src = new IPEndPoint(IPAddress.Any, 0);
            }

            while ((BufferBeginPointer != 0) &&
                (BufferBeginPointer != BufferEndPointer))
            {
                Array.Copy(MainBuffer, BufferBeginPointer, MainBuffer, 0, BufferEndPointer - BufferBeginPointer);
                BufferEndPointer = BufferEndPointer - BufferBeginPointer;
                BufferBeginPointer = 0;
                BufferSize = BufferEndPointer;
                try
                {
                    OnReceiveEvent.Fire(this, MainBuffer, BufferBeginPointer, BufferSize, 0, src, from);
                }
                catch (AsyncSocket.StopReadException ex)
                {
                    OpenSource.Utilities.EventLogger.Log(ex);
                    return;
                }
                if (StopThread != null)
                {
                    if (Thread.CurrentThread.GetHashCode() == StopThread.GetHashCode())
                    {
                        StopThread = null;
                        return;
                    }
                }
            }

            try
            {
                if (MainSocket.SocketType == SocketType.Stream)
                {
                    MainSocket.BeginReceive(MainBuffer, BufferEndPointer, BufferReadLength, SocketFlags.None, ReceiveCB, null);
                }
                else
                {
                    MainSocket.BeginReceiveFrom(MainBuffer, BufferEndPointer, BufferReadLength, SocketFlags.None, ref rEP, ReceiveCB, null);
                }
            }
            catch (Exception ex)
            {
                OpenSource.Utilities.EventLogger.Log(ex);
                Disconnect = true;
            }

            if (Disconnect == true)
            {
                bool OK = false;
                lock (this)
                {
                    if (SentDisconnect == false)
                    {
                        OK = true;
                        SentDisconnect = true;
                    }
                }
                if (OK == true)
                {
                    MainSocket = null;
                }
                if (Disconnect == true && OK == true) OnDisconnectEvent.Fire(this);
            }
        }

        private void Init()
        {
            BufferReadLength = MainBuffer.Length;
            CountLock = new object();
            PendingBytesSent = 0;
            SendLock = new object();
            SendQueue = new Queue();
            ReceiveCB = new AsyncCallback(HandleReceive);
            SendCB = new AsyncCallback(HandleSend);
            ConnectCB = new AsyncCallback(HandleConnect);
            rEP = (EndPoint)new IPEndPoint(0, 0);
        }

        /// <summary>
        /// Closes the socket
        /// </summary>
        public void Close()
        {
            //SendLock.WaitOne();
            //SendResult = null;
            //SendLock.ReleaseMutex();
            if (MainSocket != null)
            {
                try
                {
                    MainSocket.Shutdown(SocketShutdown.Both);
                    MainSocket.Close();
                }
                catch (Exception ex)
                {
                    OpenSource.Utilities.EventLogger.Log(ex);
                }
            }
        }
        /*
                public bool Sync_Send(byte[] buffer)
                {
                    return Sync_Send(buffer, 0, buffer.Length);
                }

                public bool Sync_Send(byte[] buffer, int offset, int count)
                {
                    if (MainSocket == null) return false;
                    try
                    {
                        MainSocket.Send(buffer, offset, count, SocketFlags.None);
                        return true;
                    }
                    catch (Exception e)
                    {
                        OpenSource.Utilities.EventLogger.Log(e);
                        return false;
                    }
                }
        */
        /// <summary>
        /// Asynchronously send bytes
        /// </summary>
        /// <param name="buffer"></param>
        public void Send(Byte[] buffer)
        {
            Send(buffer, null);
        }

        /// <summary>
        /// Asynchronously send bytes
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="Tag"></param>
        public void Send(Byte[] buffer, object Tag)
        {
            Send(buffer, 0, buffer.Length, Tag);
        }

        /// <summary>
        /// Asyncronously send bytes
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="Tag"></param>
        public void Send(Byte[] buffer, int offset, int length, object Tag)
        {
            Send(buffer, offset, length, null, Tag);
        }

        public void Send(Byte[] buffer, int offset, int length, IPEndPoint dest)
        {
            Send(buffer, offset, length, dest, null);
        }

        /// <summary>
        /// Asynchronously send a UDP payload
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="dest"></param>
        /// <param name="Tag"></param>
        public void Send(Byte[] buffer, int offset, int length, IPEndPoint dest, object Tag)
        {
            bool Disconnect = false;

            SendInfo SI;

            lock (SendLock)
            {
                lock (CountLock)
                {
                    if (PendingBytesSent > 0)
                    {
                        SI = new SendInfo();
                        SI.buffer = buffer;
                        SI.offset = offset;
                        SI.count = length;
                        SI.dest = dest;
                        SI.Tag = Tag;
                        SendQueue.Enqueue(SI);
                    }
                    else
                    {
                        PendingBytesSent += length;
                        try
                        {
                            if (MainSocket.SocketType == SocketType.Stream)
                            {
                                MainSocket.BeginSend(buffer, offset, length, SocketFlags.None, SendCB, Tag);
                            }
                            else
                            {
                                MainSocket.BeginSendTo(buffer, offset, length, SocketFlags.None, dest, SendCB, Tag);
                            }
                        }
                        catch (Exception ex)
                        {
                            OpenSource.Utilities.EventLogger.Log(ex);
                            OpenSource.Utilities.EventLogger.Log(this, System.Diagnostics.EventLogEntryType.Error, "Send Failure [Normal for non-pipelined connection]");
                            Disconnect = true;
                        }
                    }
                }
            }

            if (Disconnect == true)
            {
                bool OK = false;
                lock (this)
                {
                    if (SentDisconnect == false)
                    {
                        OK = true;
                        SentDisconnect = true;
                    }
                }
                if (OK == true)
                {
                    this.MainSocket = null;
                    OnDisconnectEvent.Fire(this);
                }
            }
        }

        private void HandleSend(IAsyncResult result)
        {
            int sent = 0;
            bool Ready = false;
            bool Disconnect = false;

            try
            {
                SendInfo SI;
                lock (SendLock)
                {
                    try
                    {
                        if (MainSocket.SocketType == SocketType.Stream)
                        {
                            sent = MainSocket.EndSend(result);
                        }
                        else
                        {
                            sent = MainSocket.EndSendTo(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        OpenSource.Utilities.EventLogger.Log(ex);
                        Disconnect = true;
                    }
                    lock (CountLock)
                    {
                        PendingBytesSent -= sent;
                        TotalBytesSent += sent;
                    }

                    if (SendQueue.Count > 0)
                    {
                        SI = (SendInfo)SendQueue.Dequeue();
                        try
                        {
                            if (MainSocket.SocketType == SocketType.Stream)
                            {
                                MainSocket.BeginSend(SI.buffer, SI.offset, SI.count, SocketFlags.None, SendCB, SI.Tag);
                            }
                            else
                            {
                                MainSocket.BeginSendTo(SI.buffer, SI.offset, SI.count, SocketFlags.None, SI.dest, SendCB, SI.Tag);
                            }
                        }
                        catch (Exception ex)
                        {
                            OpenSource.Utilities.EventLogger.Log(ex);
                            OpenSource.Utilities.EventLogger.Log(this, System.Diagnostics.EventLogEntryType.Error, "Send Failure [Normal for non-pipelined connection]");
                            Disconnect = true;
                        }
                    }
                    else
                    {
                        Ready = true;
                    }

                }
                if (Disconnect == true)
                {
                    bool OK = false;
                    lock (this)
                    {
                        if (SentDisconnect == false)
                        {
                            OK = true;
                            SentDisconnect = true;
                        }
                    }
                    if (OK == true)
                    {
                        MainSocket = null;
                    }
                    if (OK == true) OnDisconnectEvent.Fire(this);
                }
                else
                {
                    if (Ready == true)
                    {
                        OnSendReadyEvent.Fire(result.AsyncState);
                    }
                }
            }
            catch (Exception ex)
            {
                OpenSource.Utilities.EventLogger.Log(ex);
            }
        }

        private void HandleReceive(IAsyncResult result)
        {
            int BytesReceived = 0;
            IPEndPoint from;
            IPEndPoint src;
            bool Disconnect = false;

            try
            {
                if (MainSocket.SocketType == SocketType.Stream)
                {
                    from = (IPEndPoint)MainSocket.RemoteEndPoint;
                    BytesReceived = MainSocket.EndReceive(result);
                }
                else
                {
                    BytesReceived = MainSocket.EndReceiveFrom(result, ref rEP);
                    from = (IPEndPoint)rEP;
                }
            }
            catch (Exception ex)
            {
                // Socket Error
                bool _OK = false;
                OpenSource.Utilities.EventLogger.Log(ex);
                lock (this)
                {
                    if (SentDisconnect == false)
                    {
                        _OK = true;
                        SentDisconnect = true;
                    }
                }
                if (_OK == true)
                {
                    MainSocket = null;
                }
                if (_OK == true) OnDisconnectEvent.Fire(this);
                return;
            }

            //			OpenSource.Utilities.EventLogger.Log(this,System.Diagnostics.EventLogEntryType.Information,"BytesRead: " + BytesReceived.ToString() + " BytesRequested: "+this.BufferReadLength.ToString());

            if (BytesReceived <= 0)
            {
                Disconnect = true;
            }

            if (BytesReceived != 0)
            {
                try
                {
                    src = (IPEndPoint)MainSocket.LocalEndPoint;
                }
                catch (Exception ex)
                {
                    OpenSource.Utilities.EventLogger.Log(ex);
                    src = new IPEndPoint(IPAddress.Any, 0);
                }


                BufferEndPointer += BytesReceived;

                BufferSize = BufferEndPointer - BufferBeginPointer;
                BufferReadLength = MainBuffer.Length - BufferEndPointer;

                if (_WriteStream == null)
                {
                    try
                    {
                        OnReceiveEvent.Fire(this, MainBuffer, BufferBeginPointer, BufferSize, BytesReceived, src, from);
                    }
                    catch (AsyncSocket.StopReadException ex)
                    {
                        OpenSource.Utilities.EventLogger.Log(ex);
                        return;
                    }
                }
                else
                {
                    _WriteStream.Write(MainBuffer, 0, BytesReceived);
                    BufferBeginPointer = BufferEndPointer;
                    BufferReadLength = MainBuffer.Length;
                }

                while ((BufferBeginPointer != 0) &&
                    (BufferBeginPointer != BufferEndPointer))
                {
                    Array.Copy(MainBuffer, BufferBeginPointer, MainBuffer, 0, BufferEndPointer - BufferBeginPointer);
                    BufferEndPointer = BufferEndPointer - BufferBeginPointer;
                    BufferBeginPointer = 0;
                    BufferSize = BufferEndPointer;
                    try
                    {
                        OnReceiveEvent.Fire(this, MainBuffer, BufferBeginPointer, BufferSize, 0, src, from);
                    }
                    catch (AsyncSocket.StopReadException ex)
                    {
                        OpenSource.Utilities.EventLogger.Log(ex);
                        return;
                    }
                    if (StopThread != null)
                    {
                        if (Thread.CurrentThread.GetHashCode() == StopThread.GetHashCode())
                        {
                            StopThread = null;
                            return;
                        }
                    }
                }

                if (BufferBeginPointer == BufferEndPointer)
                {
                    // ResetBuffer then continue reading
                    BufferBeginPointer = 0;
                    BufferEndPointer = 0;
                }

                if (StopThread != null)
                {
                    if (Thread.CurrentThread.GetHashCode() == StopThread.GetHashCode())
                    {
                        StopThread = null;
                        return;
                    }
                }
                try
                {
                    if (MainSocket != null && MainSocket.Connected)
                    {
                        if (MainSocket.SocketType == SocketType.Stream)
                        {
                            MainSocket.BeginReceive(MainBuffer, BufferEndPointer, BufferReadLength, SocketFlags.None, ReceiveCB, MainSocket);
                        }
                        else
                        {
                            MainSocket.BeginReceiveFrom(MainBuffer, BufferEndPointer, BufferReadLength, SocketFlags.None, ref rEP, ReceiveCB, MainSocket);
                        }
                    }
                    else
                    {
                        Disconnect = true;
                    }
                }
                catch (Exception ex)
                {
                    OpenSource.Utilities.EventLogger.Log(ex);
                    Disconnect = true;
                }
            }

            if (Disconnect == true)
            {
                bool OK = false;
                lock (this)
                {
                    if (SentDisconnect == false)
                    {
                        OK = true;
                        SentDisconnect = true;
                    }
                }
                if (OK == true)
                {
                    MainSocket = null;
                }
                if (Disconnect == true && OK == true) OnDisconnectEvent.Fire(this);
            }
        }

    }
}
