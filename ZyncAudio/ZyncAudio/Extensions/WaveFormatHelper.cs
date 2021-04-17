using NAudio.Wave;
using System.IO;

namespace ZyncAudio.Extensions
{
    public static class WaveFormatHelper
    {

        /// <summary>
        /// Gets the bitrate for the given <see cref="WaveFormat"/>.
        /// </summary>
        public static int GetBitrate(this WaveFormat waveFormat)
        {
            return waveFormat.SampleRate * waveFormat.BitsPerSample * waveFormat.Channels;
        }

        public static byte[] ToBytes(WaveFormat waveFormat)
        {
            using (MemoryStream formatStream = new MemoryStream())
            using (var writer = new BinaryWriter(formatStream))
            {
                waveFormat.Serialize(writer);
                return formatStream.ToArray();
            }
        }

        public static WaveFormat FromBytes(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            using (var binaryReader = new BinaryReader(stream))
            {
                return new WaveFormat(binaryReader);
            }
        }

    }
}
