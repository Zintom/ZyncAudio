using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Zintom.Parcelize.Helpers;

namespace ZyncAudio.Extensions
{
    public static class LengthPrefixedSocket
    {

        public static void SendLengthPrefixed(this Socket socket, byte[] data, Action<SocketException, Socket>? exceptionThrown)
        {
            SendLengthPrefixed<object>(socket, data, exceptionThrown, null);
        }

        public static void SendLengthPrefixed<T>(this Socket socket, byte[] data, Action<SocketException, Socket>? exceptionThrown, TaskCompletionSource<T>? taskCompletionSource)
        {
            byte[] lengthPrefixed = ArrayHelpers.CombineArrays(BitConverter.GetBytes(data.Length), data);

            try
            {
                socket.BeginSend(lengthPrefixed, 0, lengthPrefixed.Length, SocketFlags.None,
                    (ar) =>
                    {
                        try
                        {
                            socket.EndSend(ar);
                        }
                        catch (SocketException e)
                        {
                            exceptionThrown?.Invoke(e, socket);
                            taskCompletionSource?.TrySetException(e);
                        }
                    }, socket);
            }
            catch (SocketException e)
            {
                exceptionThrown?.Invoke(e, socket);
                taskCompletionSource?.TrySetException(e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="continuousReceive">Whether we should begin receiving after a sucessful receive (continuously).</param>
        /// <param name="dataReceiveComplete"></param>
        /// <param name="socketExceptionThrown"></param>
        public static void BeginReceiveLengthPrefixed(this Socket socket, bool continuousReceive, Action<byte[], Socket>? dataReceiveComplete, Action<SocketException, Socket>? socketExceptionThrown)
        {
            var state = new StateObject(socket, new byte[4], dataReceiveComplete, socketExceptionThrown)
            {
                ContinuousReceive = continuousReceive
            };
            socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, EndReceivePrefix, state);
        }

        private static void EndReceivePrefix(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState!;
            try
            {
                state.BytesRead += state.WorkerSocket.EndReceive(ar);


                if (state.BytesRead < state.Buffer.Length)
                {
                    state.WorkerSocket.BeginReceive(state.Buffer, state.BytesRead, state.Buffer.Length - state.BytesRead, SocketFlags.None, EndReceivePrefix, state);
                    return;
                }

                // The prefix has been received so convert it
                // to an integer, this represents the length of the data to be received.
                int LengthToReceive = BitConverter.ToInt32(state.Buffer);

                // Resize the buffer to accomodate the data to be received.
                state.Buffer = new byte[LengthToReceive];
                state.BytesRead = 0;

                state.WorkerSocket.BeginReceive(state.Buffer, 0, LengthToReceive, SocketFlags.None, EndReceiveData, state);
            }
            catch (SocketException e)
            {
                state.SocketExceptionThrown?.Invoke(e, state.WorkerSocket);
            }
            catch (ObjectDisposedException e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private static void EndReceiveData(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState!;
            try
            {
                state.BytesRead += state.WorkerSocket.EndReceive(ar);

                if (state.BytesRead < state.Buffer.Length)
                {
                    state.WorkerSocket.BeginReceive(state.Buffer, state.BytesRead, state.Buffer.Length - state.BytesRead, SocketFlags.None, EndReceiveData, state);
                    return;
                }

                state.DataReceiveComplete?.Invoke(state.Buffer, state.WorkerSocket);

                if (state.ContinuousReceive)
                {
                    // Reset the state object.
                    state.Buffer = new byte[4];
                    state.BytesRead = 0;

                    // Begin receiving the prefix for the next piece of data.
                    state.WorkerSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, EndReceivePrefix, state);
                }
            }
            catch (SocketException e)
            {
                // Send exception back to caller.
                state.SocketExceptionThrown?.Invoke(e, state.WorkerSocket);
            }
            catch (ObjectDisposedException e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private class StateObject
        {
#pragma warning disable IDE1006 // Naming Styles
            /// <summary>
            /// Whether we should begin receiving after a sucessful receive (continuously).
            /// </summary>
            internal bool ContinuousReceive { get; init; } = false;

            internal readonly Socket WorkerSocket;
            internal byte[] Buffer;
            internal int BytesRead;

            internal Action<SocketException, Socket>? SocketExceptionThrown;
            internal Action<byte[], Socket>? DataReceiveComplete;

            internal StateObject(Socket socket, byte[] buffer, Action<byte[], Socket>? dataReceiveCompleteDelegate, Action<SocketException, Socket>? socketExceptionThrown)
            {
                WorkerSocket = socket;
                Buffer = buffer;
                SocketExceptionThrown = socketExceptionThrown;
                DataReceiveComplete = dataReceiveCompleteDelegate;
            }
#pragma warning restore IDE1006 // Naming Styles
        }
    }
}
