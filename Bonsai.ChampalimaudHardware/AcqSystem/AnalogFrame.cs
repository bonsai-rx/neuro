using OpenCV.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.ChampalimaudHardware.AcqSystem
{
    public class AnalogFrame : DataFrame
    {
        public ushort Counter;
        public readonly ushort[] Data = new ushort[6];
    }
}
