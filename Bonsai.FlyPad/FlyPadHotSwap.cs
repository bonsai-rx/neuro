using OpenCV.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.FlyPad
{
    public class FlyPadHotSwap : Source<Mat>
    {
        static readonly Mat InvalidSamples = Mat.Zeros(FlyPadSource.ChannelCount, 1, Depth.S16, 1);
        readonly FlyPadSource flyPad = new FlyPadSource();

        [TypeConverter(typeof(PortNameConverter))]
        public int LocationId
        {
            get { return flyPad.LocationId; }
            set { flyPad.LocationId = value; }
        }

        public override IObservable<Mat> Generate()
        {
            return flyPad.Generate().OnErrorResumeNext(Observable.Return(InvalidSamples)).Repeat();
        }
    }
}
