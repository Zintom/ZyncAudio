using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        /// <summary>
        /// Used to communicate track information to the host.
        /// </summary>
        public class TrackInformation
        {
            public enum TrackType
            {
                /// <summary>
                /// The track has a fixed length (i.e a mp3 file).
                /// </summary>
                FixedLength,
                /// <summary>
                /// The track has no fixed length (i.e a live stream).
                /// </summary>
                LiveStream
            }

            public TrackType Type { get; private init; }

            /// <summary>
            /// The length of the track in <b>milliseconds</b>, undefined if <see cref="Type"/> is <see cref="TrackType.LiveStream"/>
            /// </summary>
            public long Length { get; private init; }

            public TrackInformation(long lengthInMilliseconds = -1)
            {
                Length = lengthInMilliseconds;
                Type = lengthInMilliseconds == -1 ? TrackType.LiveStream : TrackType.FixedLength;
            }
        }

        public bool IsOpen { get; private set; } = true;

        public ISocketServer SocketServer { get; private set; }

        private readonly Dictionary<MessageIdentifier, Action<byte[], Socket>> _handlers = new();

        private PlaybackState _playbackState = PlaybackState.Stopped;

        public Pinger Pinger { get; private init; }

        public event Action<TrackInformation>? PlaybackStarted;
        public event Action? PlaybackStoppedNaturally;

        private long _trackStartedUnixTimeMillis = -1;
        /// <summary>
        /// Gets the amount of time since the track was started.
        /// </summary>
        public TimeSpan CurrentTrackElapsedTime
        {
            get
            {
                if (_trackStartedUnixTimeMillis == -1) { return TimeSpan.Zero; }

                return TimeSpan.FromMilliseconds(DateTimeOffset.Now.ToUnixTimeMilliseconds() - _trackStartedUnixTimeMillis);
            }
        }

        public AudioServer(ISocketServer socketServer)
        {
            SocketServer = socketServer;
            SocketServer.DataReceived = DataReceived;
            SocketServer.ConnectionProblem = SocketError;

            Pinger = new Pinger(SocketServer);

            _handlers.Add(MessageIdentifier.Response | MessageIdentifier.Ping, Pinger.PingResponseReceived);
            _handlers.Add(MessageIdentifier.Request | MessageIdentifier.AudioSamples, HandleSampleRequest);

            Pinger.Start();
        }

        /// <summary>
        /// Used to ensure Play and Stop are not able to be executed at the same time.
        /// </summary>
        private readonly object _playOrStopLockObject = new();

        public bool PlayAsync(string? fileName, TimeSpan startOffset, TimeSpan preBuffer)
        {
            if (fileName == null) return false;
            if (SocketServer.Clients.Count == 0) return false;

            lock (_playOrStopLockObject)
            {
                if (_playbackState != PlaybackState.Stopped)
                {
                    return false;
                }

                _playbackState = PlaybackState.Playing;

                new Thread(() => PlaySong(fileName, startOffset, preBuffer))
                {
                    Priority = ThreadPriority.AboveNormal
                }.Start();
                return true;
            }
        }

        public bool PlayLiveAudioAsync(IWaveProvider waveProvider, TimeSpan preBuffer)
        {
            if (waveProvider == null) return false;
            if (SocketServer.Clients.Count == 0) return false;

            lock (_playOrStopLockObject)
            {
                if (_playbackState != PlaybackState.Stopped)
                {
                    return false;
                }

                _playbackState = PlaybackState.Playing;

                new Thread(() => PlaySong(null, waveProvider, TimeSpan.Zero, preBuffer))
                {
                    Priority = ThreadPriority.AboveNormal
                }.Start();
                return true;
            }
        }

        private AudioFileReader? _lastLoadedFile = null;
        /// <inheritdoc cref="PlaySong(WaveStream?, IWaveProvider, TimeSpan, TimeSpan)"/>
        private void PlaySong(string fileName, TimeSpan startOffset, TimeSpan preBuffer)
        {
            if (_lastLoadedFile != null)
            {
                if (fileName == _lastLoadedFile.FileName)
                {
                    Debug.WriteLine($"There is an existing stream for {new FileInfo(fileName).Name}, reusing.");
                    _lastLoadedFile.Position = 0;
                }
                else
                {
                    Debug.WriteLine($"There is an existing stream for {new FileInfo(_lastLoadedFile.FileName).Name}, disposing to make way for {new FileInfo(fileName).Name}.");
                    _lastLoadedFile?.Dispose();
                    _lastLoadedFile = null;
                }
            }

            if (_lastLoadedFile == null)
            {
                Debug.WriteLine($"There is no existing stream for {new FileInfo(fileName).Name}, acquiring.");

                try
                {
                    _lastLoadedFile = new AudioFileReader(fileName);
                }
                catch (FormatException)
                {
                    _lastLoadedFile?.Dispose();
                    _lastLoadedFile = null;
                    return;
                }
            }

            PlaySong(_lastLoadedFile, _lastLoadedFile, startOffset, preBuffer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="waveStream"></param>
        /// <param name="waveProvider"></param>
        /// <param name="startOffset">The time position where we should begin playback from.</param>
        /// <param name="preBuffer">How many samples (in time) we should send to the client up front.</param>
        private void PlaySong(WaveStream? waveStream, IWaveProvider waveProvider, TimeSpan startOffset, TimeSpan preBuffer)
        {
            TrackInformation trackInformation;
            if (waveStream == null)
            {
                trackInformation = new TrackInformation();
            }
            else
            {
                trackInformation = new TrackInformation((long)waveStream.TotalTime.TotalMilliseconds);
            }
            PlaybackStarted?.Invoke(trackInformation);

            // Inform all clients to stop any audio they might be playing.
            SocketServer.SendAll(BitConverter.GetBytes((int)(MessageIdentifier.StopAudio | MessageIdentifier.AudioProcessing)));

            // Send the Wave Format information
            byte[] waveFormatBytes = WaveFormatHelper.ToBytes(waveProvider.WaveFormat);
            SocketServer.SendAll(ArrayHelpers.CombineArrays(BitConverter.GetBytes((int)(MessageIdentifier.WaveFormatInformation | MessageIdentifier.AudioProcessing)),
                                                            waveFormatBytes));

            int byteRate = waveProvider.WaveFormat.GetBitrate() / 8; // The bitrate is the number of bits per second, so divide it by 8 to gets the bytes per second.

            long startOffsetMilliseconds = (long)startOffset.TotalMilliseconds;
            long startOffsetBytePosition = startOffsetMilliseconds == 0 ? 0 : (long)((startOffsetMilliseconds / 1000d) * byteRate);

            // Send intial samples of audio
            bool allSamplesUsed = SendUpFrontSamples(SocketServer, waveProvider, byteRate, (int)preBuffer.TotalSeconds, startOffsetBytePosition);

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

            if (waveStream != null)
            {
                //                                                                    // We need to offset the start time
                //                                                                    // by the amount of bytes we are offset by.
                //                                                                    // (Byte Offset / Bytes Per Second) * 1000 (to get milliseconds).
                _trackStartedUnixTimeMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds() - startOffsetMilliseconds;
            }

            // Give clients a second to compose themselves as they have just started playing audio.
            Thread.Sleep(250);
            if (_stopPlayback) { goto stopAudio; }

            SendSamplesUntilEnd(waveProvider, byteRate);

            if (!_stopPlayback && waveStream != null)
            {
                // Wait for clients to finish playing.
                while (CurrentTrackElapsedTime < waveStream.TotalTime)
                {
                    Thread.Sleep(250);
                }
            }

        stopAudio:

            // Do not dispose of the WaveStream as we may be able
            // to reuse it if the user is scrubbing through the current song.
            //waveStream?.Dispose();

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
            _trackStartedUnixTimeMillis = -1;
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

        private void SendSamplesUntilEnd(IWaveProvider waveProvider, int byteRate)
        {
            Stopwatch elapsed = new Stopwatch();
            elapsed.Start();

            double bytesPerMillisecond = byteRate / 1000d;

            // Stores an estimate of how many samples the clients have available to them.
            long numberOfSamplesClientsHave = 0;
            do
            {
                //Debug.WriteLine("numberOfSamplesClientsHave: " + numberOfSamplesClientsHave);

                // Estimate the amount of bytes that have been consumed by the clients, Elapsed Milliseconds * Bytes Per Millisecond.
                numberOfSamplesClientsHave -= (long)(bytesPerMillisecond * elapsed.ElapsedMilliseconds);

                // Ensure the estimation never goes below zero.
                numberOfSamplesClientsHave = Math.Max(0, numberOfSamplesClientsHave);
                elapsed.Restart();

                // If there are less than 2 seconds of samples available then send out some more.
                if (numberOfSamplesClientsHave < byteRate * 2)
                {
                    // Send 4 seconds of samples (or the maximum number available).
                    byte[] sampleBuffer = new byte[byteRate * 4 - (byteRate % waveProvider.WaveFormat.BlockAlign)];

                    int bytesRead = waveProvider.Read(sampleBuffer, 0, sampleBuffer.Length);

                    // If there are no samples left to be read go to exit condition check.
                    if (bytesRead == 0)
                    {
                        // Don't burn out the CPU cycles for no reason as this is
                        // a hot path when a song is coming to an end (no samples left but numberOfSamplesClientsHave > 0).
                        Thread.Sleep(500);
                        continue;
                    }

                    // Trim the buffer to be exactly the length of the amount of bytes read.
                    sampleBuffer = sampleBuffer.AsSpan(0, bytesRead).ToArray();

                    SocketServer.SendAll(ArrayHelpers.CombineArrays(BitConverter.GetBytes((int)(MessageIdentifier.AudioSamples | MessageIdentifier.AudioProcessing)),
                                                                    sampleBuffer));

                    // Increment our estimation of how many bytes the clients have.
                    numberOfSamplesClientsHave += sampleBuffer.Length;
                }

                Thread.Sleep(750);
            } while (!_stopPlayback && numberOfSamplesClientsHave > 0);

            elapsed.Stop();
        }

        private volatile bool _stopPlayback = false;
        private readonly ManualResetEvent _stoppedPlayback = new ManualResetEvent(false);

        /// <summary>
        /// Stops playback of the current track if something is playing.
        /// </summary>
        /// <param name="disposeOfFile">Set to <see langword="false"/> if you intend for the current file to be reused on the next call to <c>Play</c>.</param>
        public bool Stop(bool disposeOfFile = true)
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

                if (disposeOfFile)
                {
                    // The caller has indicated they do not wish to reuse the current file for the next
                    // call to play so dispose of it.
                    _lastLoadedFile?.Dispose();
                    _lastLoadedFile = null;
                }

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

        private void SocketError(Exception exception, Socket client)
        {

        }

    }
}
