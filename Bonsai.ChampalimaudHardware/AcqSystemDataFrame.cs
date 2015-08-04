using OpenCV.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.ChampalimaudHardware
{
    public class AcqSystemDataFrame
    {
        public AcqSystemDataFrame(byte[] timestamp, ushort[,] amplifierData)
        {
            Timestamp = Mat.FromArray(timestamp);
            AmplifierData = Mat.FromArray(amplifierData);
        }

        public Mat Timestamp { get; private set; }

        public Mat AmplifierData { get; private set; }
    }
}
