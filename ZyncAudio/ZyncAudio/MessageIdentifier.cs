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
        Ping = 16
    }
}
