using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.ChampalimaudHardware.Mesh
{
    public class TunaConfiguration : Combinator<TunaDataFrame, TunaConfigurationFrame>
    {
        public override IObservable<TunaConfigurationFrame> Process(IObservable<TunaDataFrame> source)
        {
            return source.Select(frame => frame as TunaConfigurationFrame)
                         .Where(frame => frame != null);
        }
    }
}
