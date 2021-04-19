using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using ZyncAudio.Extensions;

namespace ZyncAudio
{
    public interface ISocketServer
    {
        Action<byte[], Socket>? DataReceived { get; set; }

        Action<SocketException, Socket>? SocketError { get; set; }

        Action<Socket>? ClientConnected { get; set; }

        Action<Socket>? ClientDisconnected { get; set; }

        List<Socket> Clients { get; }

        bool Open(IPAddress address, int port);

        bool Close();

        void Send(byte[] data, Socket client);

        void SendAll(byte[] data);
    }

    /// <summary>
    /// A basic multi-client socket server.
    /// </summary>
    /// <remarks>A receive loop is started for each client that connects.</remarks>
    public sealed class Server : ISocketServer
    {
        enum ConnectionState
        {
            None,
            Open,
            Closed
        }

        private Socket? _listener;

        private ConnectionState _connectionState = ConnectionState.None;

        private object _stateChangeLockObject = new();

        private readonly ILogger? _logger;

        public List<Socket> Clients { get; private set; } = new();

        public Action<byte[], Socket>? DataReceived { get; set; }

        public Action<SocketException, Socket>? SocketError { get; set; }

        public Action<Socket>? ClientConnected { get; set; }

        public Action<Socket>? ClientDisconnected { get; set; }

        public Server(ILogger? logger)
        {
            Clients = new List<Socket>();
            _logger = logger;
        }

        /// <summary>
        /// Opens the server on the given <paramref name="address"/> on the given <paramref name="port"/>.
        /// </summary>
        public bool Open(IPAddress address, int port)
        {
            lock (_stateChangeLockObject)
            {
                if (_connectionState != ConnectionState.None) { return false; }
                _connectionState = ConnectionState.Open;

                _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _listener.Bind(new IPEndPoint(address, port));
                _listener.Listen();

                _logger?.Log($"Server open at {address} on port {port}");

                _listener.BeginAccept(EndAccept, null);

                return true;
            }
        }

        private void EndAccept(IAsyncResult ar)
        {
            lock (_stateChangeLockObject)
            {
                if (_connectionState != ConnectionState.Open) { return; }
                if (_listener == null) { return; }

                Socket client;
                try
                {
                    client = _listener.EndAccept(ar);
                }
                catch (ObjectDisposedException) { return; }

                _logger?.Log($"Client connected ({client.RemoteEndPoint})");
                ClientConnected?.Invoke(client);

                Clients.Add(client);

                // Nagle Algorithm could cause the sync to be slightly off.
                client.NoDelay = true;
                client.BeginReceiveLengthPrefixed(
                    true,
                    (d, s) => { DataReceived?.Invoke(d, s); },
                    HandleSocketException);

                // Begin accepting other clients
                _listener.BeginAccept(EndAccept, null);
            }
        }

        public bool Close()
        {
            lock (_stateChangeLockObject)
            {
                if(_connectionState != ConnectionState.Open) { return false; }
                _connectionState = ConnectionState.Closed;

                StopAccepting();
                for (int i = 0; i < Clients.Count; i++)
                {
                    Clients[i].Shutdown(SocketShutdown.Both);
                    Clients[i].Disconnect(false);
                    Clients[i].Dispose();
                    Clients.RemoveAt(i);
                    i--;
                }

                return true;
            }
        }

        public void StopAccepting()
        {
            _listener?.Dispose();
            _listener = null;
        }

        public void Send(byte[] data, Socket client)
        {
            client.SendLengthPrefixed(data, HandleSocketException);
        }

        public void SendAll(byte[] data)
        {
            for (int i = 0; i < Clients.Count; i++)
            {
                Clients[i].SendLengthPrefixed(data, HandleSocketException);
            }
        }

        private void HandleSocketException(SocketException e, Socket client)
        {
            Clients.Remove(client);
            ClientDisconnected?.Invoke(client);
            _logger?.Log($"Client forcefully disconnected ({client.RemoteEndPoint})");

            SocketError?.Invoke(e, client);
        }
    }
}
