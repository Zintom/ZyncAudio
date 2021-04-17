using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Zintom.Parcelize.Helpers;
using ZyncAudio.Extensions;

namespace ZyncAudio
{
    public class AudioServer
    {

        public bool IsOpen { get; private set; } = true;

        public ISocketServer SocketServer { get; private set; }

        private Dictionary<MessageIdentifier, Action<byte[], Socket>> _handlers = new();

        private Dictionary<Socket, Stopwatch> _pingWatches = new();
        private Dictionary<Socket, long> _pingStats = new();

        public AudioServer(ISocketServer socketServer)
        {
            SocketServer = socketServer;
            SocketServer.DataReceived = DataReceived;
            SocketServer.SocketError = SocketError;

            _handlers.Add(MessageIdentifier.Response | MessageIdentifier.Ping, HandlePingResponse);

            PingClients();
        }

        /// <summary>
        /// Pings clients periodically to get their latency times.
        /// </summary>
        private async void PingClients()
        {
            while (IsOpen)
            {
                foreach (var client in SocketServer.Clients)
                {
                    // Initialize a stopwatch for this client if it does not exist.
                    if (!_pingWatches.TryGetValue(client, out Stopwatch? watch))
                    {
                        watch = new Stopwatch();
                        _pingWatches.Add(client, watch);
                    }

                    // If we are still waiting for a response from the client then pospone till the next check.
                    if (watch.ElapsedMilliseconds > 0)
                    {
                        continue;
                    }

                    watch.Start();

                    SocketServer.Send(BitConverter.GetBytes((int)(MessageIdentifier.Request | MessageIdentifier.Ping)), client);
                }

                await Task.Delay(1000);
            }
        }

        private void HandlePingResponse(byte[] data, Socket socket)
        {
            var watch = _pingWatches[socket];
            watch.Stop();

            _pingStats[socket] = watch.ElapsedMilliseconds;
            watch.Reset();
        }

        public void PlaySong(string fileName)
        {
            new Thread(() =>
            {
                AudioFileReader reader = new AudioFileReader(fileName);

                // Send the Wave Format information
                byte[] waveFormatBytes = WaveFormatHelper.ToBytes(reader.WaveFormat);
                SocketServer.SendAll(ArrayHelpers.CombineArrays(BitConverter.GetBytes((int)MessageIdentifier.WaveFormatInformation),
                                                                waveFormatBytes));

                int sampleBlockSize = reader.WaveFormat.GetBitrate() / 8; // The bitrate is the number of bits per second, so divide it by 8 to gets the bytes per second.

                // Send ~4 seconds of audio to get started.
                byte[] samples = new byte[sampleBlockSize * 4];
                reader.Read(samples, 0, samples.Length);

                SocketServer.SendAll(ArrayHelpers.CombineArrays(BitConverter.GetBytes((int)MessageIdentifier.AudioSamples),
                                                                samples));

                byte[] sampleBuffer = new byte[sampleBlockSize];
                while (reader.Read(sampleBuffer, 0, sampleBlockSize) > 0)
                {
                    SocketServer.SendAll(ArrayHelpers.CombineArrays(BitConverter.GetBytes((int)MessageIdentifier.AudioSamples),
                                                                    sampleBuffer));
                    Thread.Sleep(990);
                }
            }).Start();
        }

        private void DataReceived(byte[] data, Socket client)
        {
            var messageIdentifier = (MessageIdentifier)BitConverter.ToInt32(data);

            if (_handlers.TryGetValue(messageIdentifier, out Action<byte[], Socket>? handler))
            {
                handler.Invoke(data, client);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void SocketError(SocketException exception, Socket client)
        {

        }

    }
}
