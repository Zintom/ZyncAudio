using NAudio.Wave;
using System;
using System.Net.Sockets;
using System.Threading;
using Zintom.Parcelize.Helpers;
using ZyncAudio.Extensions;

namespace ZyncAudio
{
    public class AudioServer
    {

        public ISocketServer SocketServer { get; private set; }

        public AudioServer(ISocketServer socketServer)
        {
            SocketServer = socketServer;
            SocketServer.DataReceived = DataReceived;
            SocketServer.SocketError = SocketError;
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

                //int bitrate = reader.WaveFormat.SampleRate * reader.WaveFormat.BitsPerSample * reader.WaveFormat.Channels;
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
                    Thread.Sleep(1000);
                }
            }).Start();
        }

        private void DataReceived(byte[] data, Socket client)
        {

        }

        private void SocketError(SocketException exception, Socket client)
        {

        }

    }
}
