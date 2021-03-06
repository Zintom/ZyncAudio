using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ZyncAudio.Extensions;

namespace ZyncAudio
{
    public interface ISocketClient
    {
        Action<byte[], Socket>? DataReceived { get; set; }

        Action<Exception, Socket>? ConnectionProblem { get; set; }

        /// <summary>
        /// Occurs when the client disconnects from the server.
        /// </summary>
        Action? Disconnected { get; set; }

        /// <summary>
        /// Occurs when the client connected to the server.
        /// </summary>
        Action? Connected { get; set; }

        /// <summary>
        /// Attempts to connect to the target machine so long as the client is not already connecting or connected.
        /// </summary>
        /// <returns><see langword="true"/> if we will be connecting to the machine, <see langword="false"/> if not.</returns>
        bool Connect(IPAddress address, int port);

        /// <summary>
        /// Attempts to disconnect from the currently connected machine so long as the client is not already disconnected.
        /// </summary>
        /// <returns><see langword="true"/> if we disconnected, <see langword="false"/> if not.</returns>
        bool Disconnect();

        void Send(byte[] data);

        void Send<T>(byte[] data, TaskCompletionSource<T>? taskCompletionSource);
    }

    enum ConnectionState
    {
        Disconnected,
        Connecting,
        Connected,
    }

    /// <summary>
    /// A basic client which connects asynchronously.
    /// </summary>
    /// <remarks>Send will use the length-prefixed option.
    /// <para/>
    /// Receive will begin looping once connected.
    /// </remarks>
    public sealed class Client : ISocketClient
    {
        private Socket? _workerSocket;

        public Action<byte[], Socket>? DataReceived { get; set; }

        public Action<Exception, Socket>? ConnectionProblem { get; set; }

        public Action? Disconnected { get; set; }

        public Action? Connected { get; set; }

        private readonly object _stateChangeLockObject = new();

        private ConnectionState _connectionState = ConnectionState.Disconnected;

        public Client()
        {
        }

        public bool Connect(IPAddress address, int port)
        {
            lock (_stateChangeLockObject)
            {
                if (_connectionState == ConnectionState.Connected
                    || _connectionState == ConnectionState.Connecting) { return false; }
                _connectionState = ConnectionState.Connecting;

                _workerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _workerSocket.ReceiveTimeout = 5000;
                _workerSocket.BeginConnect(new IPEndPoint(address, port), EndConnect, null);

                return true;
            }
        }

        private void EndConnect(IAsyncResult ar)
        {
            lock (_stateChangeLockObject)
            {
                if (_connectionState != ConnectionState.Connecting) { return; }
                _connectionState = ConnectionState.Connected;

                try
                {
                    _workerSocket!.EndConnect(ar);
                }
                catch (SocketException e)
                {
                    ConnectionProblem?.Invoke(e, _workerSocket!);
                    Disconnect();
                    return;
                }

                Connected?.Invoke();

                // Nagle Algorithm could cause the sync to be slightly off.
                _workerSocket.NoDelay = true;

                // Begin receive loop
                _workerSocket.BeginReceiveLengthPrefixed(true, DataReceived, HandleSocketError);
            }
        }

        public bool Disconnect()
        {
            lock (_stateChangeLockObject)
            {
                if (_connectionState == ConnectionState.Disconnected) { return false; }
                _connectionState = ConnectionState.Disconnected;

                if (_workerSocket?.Connected ?? false)
                {
                    _workerSocket?.Shutdown(SocketShutdown.Both);
                    _workerSocket?.Disconnect(false);
                }
                _workerSocket?.Dispose();
                _workerSocket = null;

                Disconnected?.Invoke();

                return true;
            }
        }

        private void HandleSocketError(Exception e, Socket client)
        {
            Disconnect();
            Disconnected?.Invoke();
            ConnectionProblem?.Invoke(e, client);
        }

        public void Send(byte[] data)
        {
            Send<object>(data, null);
        }

        public void Send<T>(byte[] data, TaskCompletionSource<T>? taskCompletionSource)
        {
            _workerSocket?.SendLengthPrefixed(data, ConnectionProblem, taskCompletionSource);
        }
    }
}