using OpenCV.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.ChampalimaudHardware.AcqSystem
{
    public class AnalogData : Transform<DataFrame, Mat>
    {
        public AnalogData()
        {
            BufferLength = 10;
        }

        public int BufferLength { get; set; }

        public override IObservable<Mat> Process(IObservable<DataFrame> source)
        {
            return Process(source.Select(frame => frame as AnalogFrame).Where(frame => frame != null));
        }

        public IObservable<Mat> Process(IObservable<AnalogFrame> source)
        {
            return Observable.Defer(() =>
            {
                var packetCount = 0;
                var bufferSamples = 0;
                var bufferLength = BufferLength;
                if (bufferLength <= 0)
                {
                    throw new InvalidOperationException("The buffer length must be greater than zero.");
                }

                var buffer = default(ushort[]);
                return source.Select(frame =>
                {
                    var data = frame.Data;
                    var rows = data.GetLength(0) + 1;
                    var columns = data.GetLength(1);
                    if (buffer == null)
                    {
                        bufferSamples = columns * bufferLength;
                        buffer = new ushort[rows * bufferSamples];
                    }

                    for (int i = 1; i < rows; i++)
                    {
                        for (int j = 0; j < columns; j++)
                        {
                            buffer[(i * bufferSamples) + j + packetCount] = data[i - 1, j];
                        }
                    }

                    for (int j = 0; j < columns; j++)
                    {
                        buffer[j + packetCount] = (ushort)(frame.Counter + j);
                    }
                    return (packetCount = (packetCount + 1) % bufferLength) == 0 ? rows : 0;
                })
                .Where(rows => rows > 0)
                .Select(rows => Mat.FromArray(buffer, rows, bufferLength, Depth.U16, 1));
            });
        }
    }
}
