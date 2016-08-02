using OpenCV.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.ChampalimaudHardware.Mesh
{
    public class TunaFromArray : Transform<byte[], TunaDataFrame>
    {
        public override IObservable<TunaDataFrame> Process(IObservable<byte[]> source)
        {
            return source.Select(input => new TunaDataFrame(input));
        }
    }
}
