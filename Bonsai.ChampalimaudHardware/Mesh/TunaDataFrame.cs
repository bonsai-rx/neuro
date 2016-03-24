using OpenCV.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.ChampalimaudHardware.Mesh
{
    public class TunaDataFrame
    {
        const int NumSamples = 4;
        const int DataLength = 72 / 2;
        const int NumChannels = DataLength / NumSamples;
        const int MessageLength = DataLength * 2 + 8;
        const int SyncFlag = 0x8000;
        const int ButtonFlag = 0x4000;
        const int AlignedFlag = 0x2000;
        const int ErrorFlag = 0x1000;
        const int IdMask = 0x0FFF;

        internal TunaDataFrame(byte[] message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (message.Length < MessageLength)
            {
                throw new ArgumentException("Invalid message length.", "message");
            }

            var messageId = message[1] | message[2] << 8;
            Sync = (messageId & SyncFlag) != 0;
            Button = (messageId & ButtonFlag) != 0;
            Aligned = (messageId & AlignedFlag) != 0;
            Error = (messageId & ErrorFlag) != 0;
            Id = messageId & IdMask;

            Message = message;
            Second = message[3] | message[4] << 8 | message[5] << 16 | message[6] << 24;
            using (var dataHeader = Mat.CreateMatHeader(message, 1, message.Length / 2, Depth.S16, 1))
            {
                var tunaData = new Mat(NumChannels, NumSamples, Depth.S16, 1);
                var sampleData = dataHeader.GetSubRect(new Rect(4, 0, DataLength, 1)).Reshape(0, 4);
                CV.Transpose(sampleData, tunaData);
                Data = tunaData;
            }
        }

        public int Id { get; private set; }

        public bool Sync { get; set; }

        public bool Button { get; set; }

        public bool Aligned { get; set; }

        public bool Error { get; set; }

        public long Second { get; private set; }

        public Mat Data { get; private set; }

        public byte[] Message { get; private set; }
    }
}
