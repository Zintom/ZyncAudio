using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Zintom.Parcelize.Helpers;
using ZyncAudio.Extensions;

namespace ZyncAudio.Host
{
    enum PlaybackState
    {
        Stopped,
        Playing
    }

    public class AudioServer
    {

        public bool IsOpen { get; private set; } = true;

        public ISocketServer SocketServer { get; private set; }

        private Dictionary<MessageIdentifier, Action<byte[], Socket>> _handlers = new();

        public Pinger Pinger { get; private init; }

        private PlaybackState _playbackState = PlaybackState.Stopped;

        public AudioServer(ISocketServer socketServer)
        {
            SocketServer = socketServer;
            SocketServer.DataReceived = DataReceived;
            SocketServer.SocketError = SocketError;

            Pinger = new Pinger(SocketServer);

            _handlers.Add(MessageIdentifier.Response | MessageIdentifier.Ping, Pinger.PingResponse);

            Pinger.Start();
        }

        /// <summary>
        /// Used to ensure Play and Stop are not able to be executed at the same time.
        /// </summary>
        private readonly object _playOrStopLockObject = new();

        private Playlist? _currentPlayList;

        public async void PlayListAsync(Playlist playlist)
        {
            _currentPlayList = playlist;

            string? currentTrack;
            while ((currentTrack = _currentPlayList.Current) != null)
            {
                await PlayAsync(currentTrack).ConfigureAwait(false);
                _currentPlayList.MoveNext();
            }
        }

        private Task PlayAsync(string fileName)
        {
            lock (_playOrStopLockObject)
            {
                if (_playbackState == PlaybackState.Playing)
                {
                    return Task.FromResult(false);
                }

                _playbackState = PlaybackState.Playing;

                var tcs = new TaskCompletionSource();
                ThreadPool.QueueUserWorkItem((_) => { PlaySong(fileName, tcs); });
                //Task.Run(() => { PlaySong(fileName, tcs); }).ConfigureAwait(false);
                return tcs.Task;
            }
        }

        private void PlaySong(string fileName, TaskCompletionSource tcs)
        {
            // Inform all clients to stop any audio they might be playing.
            SocketServer.SendAll(BitConverter.GetBytes((int)(MessageIdentifier.StopAudio | MessageIdentifier.AudioProcessing)));

            AudioFileReader reader = new AudioFileReader(fileName);

            #region Send the Wave Format information
            byte[] waveFormatBytes = WaveFormatHelper.ToBytes(reader.WaveFormat);
            SocketServer.SendAll(ArrayHelpers.CombineArrays(BitConverter.GetBytes((int)(MessageIdentifier.WaveFormatInformation | MessageIdentifier.AudioProcessing)),
                                                            waveFormatBytes));
            #endregion

            #region Send Initial 4s of samples
            int sampleBlockSize = reader.WaveFormat.GetBitrate() / 8; // The bitrate is the number of bits per second, so divide it by 8 to gets the bytes per second.

            //
            // Send ~4 seconds of audio to get started.
            //
            byte[] initialSamples = new byte[sampleBlockSize * 4];
            reader.Read(initialSamples, 0, initialSamples.Length);

            SocketServer.SendAll(ArrayHelpers.CombineArrays(BitConverter.GetBytes((int)(MessageIdentifier.AudioSamples | MessageIdentifier.AudioProcessing)),
                                                            initialSamples));
            #endregion

            #region Instruct clients to play in sync.
            //
            // Wait until we have the latency values for every client.
            //
            Pinger.HasResponseFromAll.WaitOne();

            long highestLatency = Pinger.HighestPingClient / 2;

            List<KeyValuePair<Socket, long>> remainingClientsToSendTo = new();
            foreach (var pair in Pinger.PingStatistics)
            {
                remainingClientsToSendTo.Add(pair);
            }

            // Give all clients 1 second to initialize their playback.
            Thread.Sleep((int)highestLatency + 2000);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            while (remainingClientsToSendTo.Count > 0 && !_stopPlayback)
            {
                for (int i = 0; i < remainingClientsToSendTo.Count; i++)
                {
                    long latency = remainingClientsToSendTo[i].Value / 2;

                    //Debug.WriteLine($"Elapsed {watch.ElapsedMilliseconds} ms");

                    if (watch.ElapsedMilliseconds >= highestLatency - latency)
                    {
                        // Inform the client that it should begin playing immediately.
                        SocketServer.Send(BitConverter.GetBytes((int)(MessageIdentifier.PlayAudio |
                                                                      MessageIdentifier.AudioProcessing | 
                                                                      MessageIdentifier.ProcessImmediately)), remainingClientsToSendTo[i].Key);
                        remainingClientsToSendTo.RemoveAt(i);
                        i--;
                    }
                }
            }

            watch.Stop();

            #endregion

            Thread.Sleep(250);

            #region Send Remaining Audio at 1 sample block per second.

            byte[] sampleBuffer = new byte[sampleBlockSize];
            while (reader.Read(sampleBuffer, 0, sampleBlockSize) > 0 && !_stopPlayback)
            {
                SocketServer.SendAll(ArrayHelpers.CombineArrays(BitConverter.GetBytes((int)(MessageIdentifier.AudioSamples | MessageIdentifier.AudioProcessing)),
                                                                sampleBuffer));

                //Debug.WriteLine("Server: Sending samples" + DateTime.Now.ToLongTimeString());

                Thread.Sleep(990);
            }
            #endregion

            reader.Dispose();

            if (!_stopPlayback)
            {
                // Wait for clients to run out of samples
                Thread.Sleep(4000);
            }

            SocketServer.SendAll(BitConverter.GetBytes((int)(MessageIdentifier.StopAudio | MessageIdentifier.AudioProcessing)));

            // Inform waiting threads that playback has stopped.
            _stoppedPlayback.Set();
            // Reset state to stopped.
            _stopPlayback = false;
            _playbackState = PlaybackState.Stopped;

            tcs.TrySetResult();
        }

        private volatile bool _stopPlayback = false;
        private readonly ManualResetEvent _stoppedPlayback = new ManualResetEvent(false);

        /// <summary>
        /// Stops playback of the current track and depending on the <see cref="Playlist.PlayingMode"/>, another song may start playing.
        /// </summary>
        public bool Next()
        {
            lock (_playOrStopLockObject)
            {
                if (_playbackState != PlaybackState.Playing)
                {
                    return false;
                }

                _stoppedPlayback.Reset();

                // Inform the player that we wish to stop.
                _stopPlayback = true;

                // Wait for the sample distributor to respond.
                _stoppedPlayback.WaitOne();

                return true;
            }
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
