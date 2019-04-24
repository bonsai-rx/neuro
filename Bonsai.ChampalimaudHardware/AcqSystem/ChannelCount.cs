using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.ChampalimaudHardware.AcqSystem
{
    public enum ChannelCount : byte
    {
        One = 0x4F, // 'O'
        Two = 0x57, // 'W'
        Three = 0x48, // 'H'
        Six = 0x53 // 'S'
    }
}
