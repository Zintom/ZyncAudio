using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using ZyncAudio.Extensions;

namespace ZyncAudio
{
    public class AudioClient
    {

        public ISocketClient SocketClient { get; private set; }

        private Dictionary<MessageIdentifier, Action<byte[]>> _handlers = new();

        private WaveFormat? _lastWaveFormatReceived;
        private BufferedWaveProvider? _bufferedWaveProvider;
        private WaveOutEvent? _waveOut;

        private ILogger? _logger;

        public AudioClient(ISocketClient socketClient, ILogger? logger)
        {
            SocketClient = socketClient;
            SocketClient.DataReceived = DataReceived;
            SocketClient.SocketError = SocketError;

            _handlers.Add(MessageIdentifier.WaveFormatInformation, HandleWaveFormatInformation);
            _handlers.Add(MessageIdentifier.AudioSamples, HandleAudioSamples);
            _logger = logger;
        }

        private void DataReceived(byte[] data, Socket client)
        {
            var messageIdentifier = (MessageIdentifier)BitConverter.ToInt32(data);

            if (_handlers.TryGetValue(messageIdentifier, out Action<byte[]>? handler))
            {
                handler.Invoke(data);
            }
            else
            {
                throw new NotImplementedException();
            }
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
                _waveOut = new WaveOutEvent();
                _waveOut.Volume = 1;

                // We must have received the WaveFormat info before being
                // able to create our BufferedWaveProvider.
                if (_lastWaveFormatReceived == null) { throw new Exception("Wave Format information was not received prior to samples being received."); }

                _bufferedWaveProvider = new BufferedWaveProvider(_lastWaveFormatReceived);

                int sampleBlockSize = _lastWaveFormatReceived.GetBitrate() / 8;
                // 5 second audio buffer.
                _bufferedWaveProvider.BufferLength = sampleBlockSize * 5;

                new Thread(() =>
                {
                    Thread.CurrentThread.Priority = ThreadPriority.Highest;
                    _waveOut.Init(_bufferedWaveProvider);
                    _waveOut.Play();
                }).Start();
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

        private void SocketError(SocketException exception, Socket client)
        {

        }
    }
}
