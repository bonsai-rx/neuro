using Bonsai.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.PulsePal
{
    [Description("Triggers the specified output channels to begin playing their pulse sequences.")]
    public class TriggerOutput : Sink
    {
        [Description("The name of the serial port.")]
        [Editor("Bonsai.PulsePal.Design.PulsePalConfigurationEditor, Bonsai.PulsePal.Design", typeof(UITypeEditor))]
        public string PortName { get; set; }

        [Description("A value representing the bitmask of channels to trigger.")]
        public byte Channels { get; set; }

        public override IObservable<TSource> Process<TSource>(IObservable<TSource> source)
        {
            return Observable.Using(
                () => PulsePalManager.ReserveConnection(PortName),
                pulsePal => source.Do(input =>
                {
                    lock (pulsePal.PulsePal)
                    {
                        pulsePal.PulsePal.TriggerOutputChannels(Channels);
                    }
                }));
        }
    }
}
