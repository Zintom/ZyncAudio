using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

        private readonly Dictionary<MessageIdentifier, Action<byte[], Socket>> _handlers = new();

        private PlaybackState _playbackState = PlaybackState.Stopped;

        public Pinger Pinger { get; private init; }

        public event Action? PlaybackStoppedNaturally;

        public long CurrentTrackPositionBytes { get; set; } = 0L;

        public AudioServer(ISocketServer socketServer)
        {
            SocketServer = socketServer;
            SocketServer.DataReceived = DataReceived;
            SocketServer.SocketError = SocketError;

            Pinger = new Pinger(SocketServer);

            _handlers.Add(MessageIdentifier.Response | MessageIdentifier.Ping, Pinger.PingResponseReceived);
            _handlers.Add(MessageIdentifier.Request | MessageIdentifier.AudioSamples, HandleSampleRequest);

            Pinger.Start();
        }

        /// <summary>
        /// Used to ensure Play and Stop are not able to be executed at the same time.
        /// </summary>
        private readonly object _playOrStopLockObject = new();

        public bool PlayAsync(string? fileName, long startOffsetBytePosition = 0)
        {
            if (fileName == null) return false;

            lock (_playOrStopLockObject)
            {
                if (_playbackState != PlaybackState.Stopped)
                {
                    return false;
                }

                _playbackState = PlaybackState.Playing;

                new Thread(() => PlaySong(fileName, startOffsetBytePosition))
                {
                    Priority = ThreadPriority.AboveNormal
                }.Start();
                return true;
            }
        }

        public bool PlayLiveAudioAsync(IWaveProvider waveProvider, int upFrontSampleSecondsToSend)
        {
            if (waveProvider == null) return false;

            lock (_playOrStopLockObject)
            {
                if (_playbackState != PlaybackState.Stopped)
                {
                    return false;
                }

                _playbackState = PlaybackState.Playing;

                new Thread(() => PlaySong(null, waveProvider, 0, upFrontSampleSecondsToSend))
                {
                    Priority = ThreadPriority.AboveNormal
                }.Start();
                return true;
            }
        }

        private void PlaySong(string fileName, long startOffsetBytePosition)
        {
            try
            {
                var reader = new AudioFileReader(fileName);
                PlaySong(reader, reader, startOffsetBytePosition);
            }
            catch (FormatException)
            {

            }
        }

        private void PlaySong(WaveStream? waveStream, IWaveProvider waveProvider, long startOffsetBytePosition, int upFrontSampleSecondsToSend = 2)
        {
            // Inform all clients to stop any audio they might be playing.
            SocketServer.SendAll(BitConverter.GetBytes((int)(MessageIdentifier.StopAudio | MessageIdentifier.AudioProcessing)));

            // Send the Wave Format information
            byte[] waveFormatBytes = WaveFormatHelper.ToBytes(waveProvider.WaveFormat);
            SocketServer.SendAll(ArrayHelpers.CombineArrays(BitConverter.GetBytes((int)(MessageIdentifier.WaveFormatInformation | MessageIdentifier.AudioProcessing)),
                                                            waveFormatBytes));

            int byteRate = waveProvider.WaveFormat.GetBitrate() / 8; // The bitrate is the number of bits per second, so divide it by 8 to gets the bytes per second.

            // Send intial samples of audio
            bool allSamplesUsed = SendUpFrontSamples(SocketServer, waveProvider, byteRate, upFrontSampleSecondsToSend, startOffsetBytePosition);

            if (allSamplesUsed || _stopPlayback)
            {
                goto stopAudio;
            }

            // Instruct clients to play in sync.

            // Wait until we have the latency values for every client.
            Pinger.HasResponseFromAll.WaitOne();

            // Give all clients 1 seconds to initialize their playback.
            Thread.Sleep((int)Pinger.HighestLatency + 2000);
            // Send sync'd command.
            SendSynchronisedPlayCommand();

            Stopwatch playingTimeWatch = new();
            playingTimeWatch.Start();

            // Give clients a second to compose themselves as they have just started playing audio.
            Thread.Sleep(250);
            if (_stopPlayback) { goto stopAudio; }

            SendSamplesUntilEnd(waveStream, waveProvider, byteRate);

            if (!_stopPlayback && waveStream != null)
            {
                // Wait for clients to finish playing.
                while (playingTimeWatch.Elapsed < waveStream.TotalTime)
                {
                    Thread.Sleep(250);
                }
            }

            waveStream?.Dispose();

        stopAudio:

            SocketServer.SendAll(BitConverter.GetBytes((int)(MessageIdentifier.StopAudio | MessageIdentifier.AudioProcessing)));

            // Reset state to stopped.
            _playbackState = PlaybackState.Stopped;

            // Inform waiting threads that playback has stopped.
            _stoppedPlayback.Set();

            if (!_stopPlayback)
            {
                // If there was no signal to stop the playback then it
                // has stopped "naturally".
                PlaybackStoppedNaturally?.Invoke();
            }

            _stopPlayback = false;
        }

        /// <returns><see langword="true"/> if all available samples have been used, or <see langword="false"/> if there are more available..</returns>
        private static bool SendUpFrontSamples(ISocketServer server, IWaveProvider waveProvider, int byteRate, int secondsToSendUpFront, long startOffsetBytePosition)
        {
            // Align the start position with the block size.
            startOffsetBytePosition -= startOffsetBytePosition % waveProvider.WaveFormat.BlockAlign;

            // Read out the bytes which we wish to skip past (the start offset)
            byte[] startOffsetBuffer = new byte[startOffsetBytePosition];
            int startOffsetBytesRead = waveProvider.Read(startOffsetBuffer, 0, startOffsetBuffer.Length);

            // If we read less than what we requested then we have come
            // to the end of the stream.
            if (startOffsetBytesRead < startOffsetBuffer.Length)
            {
                return true;
            }

            //
            // Send secondsToSendUpFront seconds of audio to get started.
            //
            byte[] initialSamples = new byte[byteRate * secondsToSendUpFront];
            int initialBytesRead = waveProvider.Read(initialSamples, 0, initialSamples.Length);
            // Some tracks may be less than 4 seconds long so trim down the initial sample length down
            // to what was actually read from the reader.
            initialSamples = initialSamples.AsSpan(0, initialBytesRead).ToArray();

            server.SendAll(ArrayHelpers.CombineArrays(BitConverter.GetBytes((int)(MessageIdentifier.AudioSamples | MessageIdentifier.AudioProcessing)),
                                                            initialSamples));

            return false;
        }

        private void SendSynchronisedPlayCommand()
        {
            long highestLatency = Pinger.HighestLatency;

            List<KeyValuePair<Socket, long>> remainingClientsToSendTo = new();
            foreach (var pair in Pinger.PingStatistics)
            {
                remainingClientsToSendTo.Add(pair);
            }

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
        }

        private void SendSamplesUntilEnd(WaveStream? waveStream, IWaveProvider waveProvider, int byteRate)
        {
            Stopwatch elapsed = new Stopwatch();
            elapsed.Start();

            double bytesPerMillisecond = byteRate / 1000d;

            // Stores an estimate of how many samples the clients have available to them.
            long numberOfSamplesClientsHave = 0;
            while (!_stopPlayback)
            {
                // Estimate the amount of bytes that have been consumed by the clients, Elapsed Milliseconds * Bytes Per Millisecond.
                numberOfSamplesClientsHave -= (long)(bytesPerMillisecond * elapsed.ElapsedMilliseconds);

                // Ensure the estimation never goes below zero.
                numberOfSamplesClientsHave = Math.Clamp(numberOfSamplesClientsHave, 0, long.MaxValue);
                elapsed.Restart();

                // If there are less than 2 seconds of samples available then send out some more.
                if (numberOfSamplesClientsHave < byteRate * 2)
                {
                    // Send 4 seconds of samples (or the maximum number available).
                    byte[] sampleBuffer = new byte[byteRate * 4 - (byteRate % waveProvider.WaveFormat.BlockAlign)];

                    int bytesRead = waveProvider.Read(sampleBuffer, 0, sampleBuffer.Length);

                    // If there are no samples left to be read then quit.
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    // Trim the buffer to be exactly the length of the amount of bytes read.
                    sampleBuffer = sampleBuffer.AsSpan(0, bytesRead).ToArray();
                    CurrentTrackPositionBytes = waveStream?.Position ?? 0;

                    SocketServer.SendAll(ArrayHelpers.CombineArrays(BitConverter.GetBytes((int)(MessageIdentifier.AudioSamples | MessageIdentifier.AudioProcessing)),
                                                                    sampleBuffer));

                    // Increment our estimation of how many bytes the clients have.
                    numberOfSamplesClientsHave += sampleBuffer.Length;
                }

                Thread.Sleep(750);
            }

            elapsed.Stop();
        }

        private volatile bool _stopPlayback = false;
        private readonly ManualResetEvent _stoppedPlayback = new ManualResetEvent(false);

        /// <summary>
        /// Stops playback of the current track if something is playing.
        /// </summary>
        public bool Stop()
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

        public void ChangeVolume(float volume = 1.0f)
        {
            // Ensure that Volume does not exceed/preceed valid values.
            volume = Math.Clamp(volume, 0.0f, 1.0f);

            SocketServer.SendAll(ArrayHelpers.CombineArrays(BitConverter.GetBytes((int)(MessageIdentifier.Volume | MessageIdentifier.AudioProcessing)),
                                                            BitConverter.GetBytes(volume)));
        }

        public void ChangeNowPlayingInfo(string nowPlayingText)
        {
            SocketServer.SendAll(ArrayHelpers.CombineArrays(BitConverter.GetBytes((int)(MessageIdentifier.TrackInformation | MessageIdentifier.NotUrgent)),
                                                            Encoding.UTF8.GetBytes(nowPlayingText)));
        }

        private void HandleSampleRequest(byte[] _, Socket client)
        {

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
