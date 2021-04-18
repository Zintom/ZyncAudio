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
            _handlers.Add(MessageIdentifier.PlayAudio, HandlePlayAudio);
            _handlers.Add(MessageIdentifier.StopAudio, HandleStopAudio);
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
                handler.Invoke(data);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void HandlePingRequest(byte[] data)
        {
            // Artifical lag
            //Thread.Sleep(1500);

            SocketClient.Send(BitConverter.GetBytes((int)(MessageIdentifier.Response | MessageIdentifier.Ping)));
        }

        private void HandleWaveFormatInformation(byte[] data)
        {
            // Ignore the first 4 bytes as these are the message identifier
            byte[] waveFormatBytes = data.AsSpan(sizeof(int)).ToArray();
            _lastWaveFormatReceived = WaveFormatHelper.FromBytes(waveFormatBytes);

            _logger?.Log("Received Wave Format Information: " + _lastWaveFormatReceived.ToString());
        }

        private void HandlePlayAudio(byte[] _)
        {
            // Artifical lag
            //Thread.Sleep(1500);

            if (_waveOut == null)
            {
                throw new InvalidOperationException("Play Audio request received however the WaveOutEvent device has not been initialized.");
            }

            _waveOut.Play();
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

                // 30 second audio buffer.
                _bufferedWaveProvider.BufferLength = sampleBlockSize * 30;

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

        private void HandleStopAudio(byte[]? _)
        {
            _waveOut?.Stop();
            _waveOut?.Dispose();
            _waveOut = null;
            _bufferedWaveProvider?.ClearBuffer();
            _bufferedWaveProvider = null;
            _lastWaveFormatReceived = null;
        }

        private void SocketError(SocketException exception, Socket client)
        {

        }
    }
}
