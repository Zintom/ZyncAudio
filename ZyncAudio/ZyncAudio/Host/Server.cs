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

        Action? ClientConnected { get; set; }
       
        List<Socket> Clients { get; }

        void Open(IPAddress address, int port);

        void Send(byte[] data, Socket client);

        void SendAll(byte[] data);
    }

    /// <summary>
    /// A basic multi-client socket server.
    /// </summary>
    /// <remarks>A receive loop is started for each client that connects.</remarks>
    public sealed class Server : ISocketServer
    {
        private readonly Socket _listener;

        private readonly ILogger? _logger;

        public List<Socket> Clients { get; private set; } = new();

        public Action<byte[], Socket>? DataReceived { get; set; }

        public Action<SocketException, Socket>? SocketError { get; set; }

        public Action? ClientConnected { get; set; }

        public Server(ILogger? logger)
        {
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Clients = new List<Socket>();
            _logger = logger;
        }

        /// <summary>
        /// Opens the server on the given <paramref name="address"/> on the given <paramref name="port"/>.
        /// </summary>
        public void Open(IPAddress address, int port)
        {
            _listener.Bind(new IPEndPoint(address, port));
            _listener.Listen();

            _logger?.Log($"Server open at {address} on port {port}");

            _listener.BeginAccept(EndAccept, null);
        }

        private void EndAccept(IAsyncResult ar)
        {
            Socket client = _listener.EndAccept(ar);

            _logger?.Log($"Client connected ({client.RemoteEndPoint})");
            ClientConnected?.Invoke();

            Clients.Add(client);

            client.BeginReceiveLengthPrefixed(
                true,
                (d, s) => { DataReceived?.Invoke(d, s); },
                HandleSocketException);

            // Begin accepting other clients
            _listener.BeginAccept(EndAccept, null);
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
            _logger?.Log($"Client forcefully disconnected ({client.RemoteEndPoint})");

            SocketError?.Invoke(e, client);
        }
    }
}
