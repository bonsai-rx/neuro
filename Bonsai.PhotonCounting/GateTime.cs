using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.PhotonCounting
{
    public enum GateTime : byte
    {
        GateTime50US = 0x02,
        GateTime100US = 0x03,
        GateTime200US = 0x04,
        GateTime500US = 0x05,
        GateTime1MS = 0x06,
        GateTime2MS = 0x07,
        GateTime5MS = 0x08,
        GateTime10MS = 0x09,
        GateTime20MS = 0x0a,
        GateTime50MS = 0x0b,
        GateTime100MS = 0x0c,
        GateTime200MS = 0x0d,
        GateTime500MS = 0x0e,
        GateTime1S = 0x0f,
        GateTime2S = 0x10,
        GateTime5S = 0x11,
        GateTime10S = 0x12
    }
}
