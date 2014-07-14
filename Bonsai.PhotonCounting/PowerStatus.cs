using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.PhotonCounting
{
    public enum PowerStatus : byte
    {
        PmtPowerOff = 0,
        PmtPowerOn = 1,
        PmtPowerCheck = 2
    }
}
