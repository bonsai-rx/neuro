using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.ChampalimaudHardware.Mesh
{
    public class TunaConfigurationFrame : TunaDataFrame
    {
        const int DataLength = 72 + 8;
        const int ConfigurationLength = 11;
        const int MessageLength = DataLength + ConfigurationLength;

        public TunaConfigurationFrame(byte[] message)
            : base(message)
        {
            if (message.Length < MessageLength)
            {
                throw new ArgumentException("Invalid message length.", "message");
            }

            Frequency = message[DataLength + 0];
            AccelerometerRange = message[DataLength + 1];
            AccelerometerNoise = message[DataLength + 2];
            AccelerometerHalfBandwidth = message[DataLength + 3];
            GyroscopeRange = message[DataLength + 4];
            GyroscopeBandwidth = message[DataLength + 5];
            BatteryLevel = message[DataLength + 10];
        }

        public int Frequency { get; set; }

        public int AccelerometerRange { get; set; }

        public int AccelerometerNoise { get; set; }

        public int AccelerometerHalfBandwidth { get; set; }

        public int GyroscopeRange { get; set; }

        public int GyroscopeBandwidth { get; set; }

        public int BatteryLevel { get; set; }
    }
}
