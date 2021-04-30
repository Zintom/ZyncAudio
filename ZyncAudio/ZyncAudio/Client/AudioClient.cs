using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZyncAudio.Extensions;
using static ZyncAudio.WorkPump;

namespace ZyncAudio
{
    public class AudioClient
    {

        private class GenericWork : IWorkItem
        {

            private readonly Action<byte[]> _method;
            private readonly byte[] _argumentData;

            public GenericWork(Action<byte[]> method, byte[] argumentData)
            {
                _method = method;
                _argumentData = argumentData;
            }

            public void Invoke()
            {
                _method.Invoke(_argumentData);
            }
        }

        public ISocketClient SocketClient { get; private set; }

        private readonly Dictionary<MessageIdentifier, Action<byte[]>> _handlers = new();

        private readonly WorkPump _lowPriorityWorkPump;
        private readonly WorkPump _highPriorityWorkPump;

        private WaveFormat? _lastWaveFormatReceived;
        private BufferedWaveProvider? _bufferedWaveProvider;
        private WaveOutEvent? _waveOut;
        private float _targetVolume = 1.0f;

        private readonly ILogger? _logger;

        public event Action<string>? TrackInformationChanged;

        public AudioClient(ISocketClient socketClient, ILogger? logger)
        {
            _lowPriorityWorkPump = new();
            _lowPriorityWorkPump.Run(ThreadPriority.Lowest, threadName: "Low Priority Work Pump", isBackground: true);

            _highPriorityWorkPump = new();
            _highPriorityWorkPump.Run(ThreadPriority.AboveNormal, threadName: "High Priority Work Pump", isBackground: true);

            SocketClient = socketClient;
            SocketClient.DataReceived = DataReceived;
            SocketClient.SocketError = SocketError;

            _handlers.Add(MessageIdentifier.WaveFormatInformation |
                          MessageIdentifier.AudioProcessing, HandleWaveFormatInformation);

            _handlers.Add(MessageIdentifier.AudioSamples |
                          MessageIdentifier.AudioProcessing, HandleAudioSamples);

            _handlers.Add(MessageIdentifier.PlayAudio |
                          MessageIdentifier.AudioProcessing |
                          MessageIdentifier.ProcessImmediately, HandlePlayAudio);

            _handlers.Add(MessageIdentifier.StopAudio |
                          MessageIdentifier.AudioProcessing, HandleStopAudio);

            _handlers.Add(MessageIdentifier.Volume |
                          MessageIdentifier.AudioProcessing, HandleVolumeChangeRequest);

            _handlers.Add(MessageIdentifier.TrackInformation |
                          MessageIdentifier.NotUrgent, HandleTrackInformationChanged);

            _handlers.Add(MessageIdentifier.Request | MessageIdentifier.Ping, HandlePingRequest);
            _logger = logger;
        }

        public void Disconnect()
        {
            SocketClient.Disconnect();
            HandleStopAudio(null);
        }

        private void DataReceived(byte[] data, Socket client)
        {
            var messageIdentifier = (MessageIdentifier)BitConverter.ToInt32(data);

            if (_handlers.TryGetValue(messageIdentifier, out Action<byte[]>? handler))
            {
                if (messageIdentifier.HasFlag(MessageIdentifier.AudioProcessing) &&
                    !messageIdentifier.HasFlag(MessageIdentifier.ProcessImmediately)
                    || messageIdentifier.HasFlag(MessageIdentifier.NotUrgent))
                {
                    // Audio Processing requests go into a separate low priority queue(apart from those flagged as ProcessImmediately)
                    // as Audio Processing can clog the message pump as it may take from 10-200 ms to complete.
                    // We need as little latency as possible when responding to Ping and Play messages so naturally audio work
                    // is delegated to a lower priority thread.
                    _lowPriorityWorkPump.Add(new GenericWork(handler, data));
                }
                else
                {
                    _highPriorityWorkPump.Add(new GenericWork(handler, data));
                }
            }
            else
            {
                throw new NotImplementedException($"There is no handler for the message {messageIdentifier}.");
            }
        }

        private void HandlePingRequest(byte[] data)
        {
            SocketClient.Send(BitConverter.GetBytes((int)(MessageIdentifier.Response | MessageIdentifier.Ping)));
        }

        private void HandleWaveFormatInformation(byte[] data)
        {
            // Ignore the first 4 bytes as these are the message identifier
            byte[] waveFormatBytes = data.AsSpan(sizeof(int)).ToArray();
            _lastWaveFormatReceived = WaveFormatHelper.FromBytes(waveFormatBytes);

            _logger?.Log("Received Wave Format Information: " + _lastWaveFormatReceived.ToString());
        }

        private void HandleAudioSamples(byte[] data)
        {
            if (_waveOut == null)
            {
                _logger?.Log("Wave Out device was null so creating.");
                _waveOut = new WaveOutEvent
                {
                    Volume = _targetVolume
                };

                // We must have received the WaveFormat info before being
                // able to create our BufferedWaveProvider.
                if (_lastWaveFormatReceived == null) { throw new Exception("Wave Format information was not received prior to samples being received."); }

                _bufferedWaveProvider = new BufferedWaveProvider(_lastWaveFormatReceived)
                {
                    BufferDuration = TimeSpan.FromSeconds(30),
                    ReadFully = false
                };
                //int sampleBlockSize = _lastWaveFormatReceived.GetBitrate() / 8;

                // 30 second audio buffer.
                //_bufferedWaveProvider.BufferLength = sampleBlockSize * 30;

                _waveOut.Init(_bufferedWaveProvider);
            }

            if (_bufferedWaveProvider == null)
            {
                throw new Exception("The bufferred wave provider was empty.");
            }

            // Ignore first 4 bytes as this is the message identifier
            byte[] samples = data.AsSpan(sizeof(int)).ToArray();

            _bufferedWaveProvider.AddSamples(samples, 0, samples.Length);
            _logger?.Log($"New samples added, {Math.Round(samples.Length / 1000f / 1000f, 3)} megabytes. Bufferred: {_bufferedWaveProvider.BufferedBytes} bytes ({_bufferedWaveProvider.BufferedDuration} seconds).");
        }

        private void HandlePlayAudio(byte[] _)
        {
            if (_waveOut == null)
            {
                throw new InvalidOperationException("Play Audio request received however the WaveOutEvent device has not been initialized.");
            }

            _waveOut.Play();
        }

        private void HandleStopAudio(byte[]? _)
        {
            _waveOut?.Stop();
            _waveOut?.Dispose();
            _waveOut = null;
            _bufferedWaveProvider?.ClearBuffer();
            _bufferedWaveProvider = null;
            _lastWaveFormatReceived = null;
        }

        private void HandleVolumeChangeRequest(byte[] data)
        {
            // Ignore the first 4 bytes as that is the message identifier header.
            Span<byte> newVolume = data.AsSpan(sizeof(int));

            _targetVolume = BitConverter.ToSingle(newVolume);

            // Ensure that Volume does not exceed/preceed valid values.
            _targetVolume = Math.Clamp(_targetVolume, 0.0f, 1.0f);

            if (_waveOut != null)
            {
                _waveOut.Volume = _targetVolume;
            }
        }

        private void HandleTrackInformationChanged(byte[] data)
        {
            // Ignore the first 4 bytes as that is the message identifier header.
            Span<byte> nowPlayingText = data.AsSpan(sizeof(int));

            TrackInformationChanged?.Invoke(Encoding.UTF8.GetString(nowPlayingText));
        }

        private void SocketError(SocketException exception, Socket client)
        {

        }
    }
}
