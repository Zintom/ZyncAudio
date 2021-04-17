using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZyncAudio
{
    [Flags]
    public enum MessageIdentifier
    {
        None = 0,
        Request = 1,
        Response = 2,
        WaveFormatInformation = 4,
        AudioSamples = 8
    }
}
