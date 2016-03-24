using OpenCV.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.ChampalimaudHardware.AcqSystem
{
    public class MotionSensorData : Transform<DataFrame, Mat>
    {
        const byte MotionSensorType = 0x0;

        public int Channel { get; set; }

        public override IObservable<Mat> Process(IObservable<DataFrame> source)
        {
            return Process(source.Select(frame => frame as DigitalFrame).Where(frame => frame != null));
        }

        public IObservable<Mat> Process(IObservable<DigitalFrame> source)
        {
            return source.Where(frame => frame.SensorType == MotionSensorType && frame.Channel == Channel)
                         .Select(frame => Mat.FromArray(frame.Data, 9, 1, Depth.S16, 1));
        }
    }
}
