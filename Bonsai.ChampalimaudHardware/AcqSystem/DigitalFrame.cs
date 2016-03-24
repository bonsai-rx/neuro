using OpenCV.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.ChampalimaudHardware.AcqSystem
{
    public class DigitalFrame : DataFrame
    {
        public byte Counter;
        public byte Channel;
        public byte SensorType;
        public readonly byte[] Data = new byte[20];
    }
}
