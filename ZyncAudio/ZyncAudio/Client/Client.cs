using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ZyncAudio.Extensions;

namespace ZyncAudio
{
    public interface ISocketClient
    {
        Action<byte[], Socket>? DataReceived { get; set; }

        Action<SocketException, Socket>? SocketError { get; set; }

        void Connect(IPAddress address, int port);

        void Send(byte[] data);

        void Send<T>(byte[] data, TaskCompletionSource<T>? taskCompletionSource);
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
        private readonly Socket _workerSocket;

        public Action<byte[], Socket>? DataReceived { get; set; }

        public Action<SocketException, Socket>? SocketError { get; set; }

        public Client()
        {
            _workerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _workerSocket.ReceiveTimeout = 5000;
        }

        /// <summary>
        /// Connects to the remote <paramref name="address"/> on the given <paramref name="port"/>
        /// and begins a receive loop once connection is established.
        /// </summary>
        public void Connect(IPAddress address, int port)
        {
            _workerSocket.BeginConnect(new IPEndPoint(address, port), EndConnect, null);
        }

        private void EndConnect(IAsyncResult ar)
        {
            _workerSocket.EndConnect(ar);

            // Begin receive loop
            _workerSocket.BeginReceiveLengthPrefixed(true, DataReceived, SocketError);
        }

        public void Send(byte[] data)
        {
            Send<object>(data, null);
        }

        public void Send<T>(byte[] data, TaskCompletionSource<T>? taskCompletionSource)
        {
            _workerSocket.SendLengthPrefixed(data, SocketError, taskCompletionSource);
        }
    }
}