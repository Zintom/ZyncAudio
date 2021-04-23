using System;

namespace ZyncAudio
{
    [Flags]
    public enum MessageIdentifier
    {
        None = 0,
        Request = 1,
        Response = 2,
        WaveFormatInformation = 4,
        AudioSamples = 8,
        PlayAudio = 16,
        StopAudio = 32,
        Volume = 64,
        Ping = 128,

        /// <summary>
        /// This message relates to audio processing.
        /// </summary>
        AudioProcessing = 256,

        /// <summary>
        /// This message should be processed immediately (or as soon as possible).
        /// </summary>
        ProcessImmediately = 512,
    }
}
