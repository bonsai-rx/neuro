using Bonsai.IO;
using OpenCV.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.PulsePal
{
    public class UpdatePulseTrain : Sink<Mat>
    {
        public UpdatePulseTrain()
        {
            PulseId = 1;
        }

        [Description("The name of the serial port.")]
        [TypeConverter(typeof(SerialPortNameConverter))]
        public string PortName { get; set; }

        public int PulseId { get; set; }

        public int Frequency { get; set; }

        public override IObservable<Mat> Process(IObservable<Mat> source)
        {
            return Observable.Using(
                () => PulsePalManager.ReserveConnection(PortName),
                pulsePal => source.Do(input =>
                {
                    var pulseInterval = 1000000 / Frequency;
                    var pulseTimes = new int[input.Cols];
                    var pulseVoltages = new byte[input.Cols];
                    for (int i = 0; i < pulseTimes.Length; i++)
                    {
                        pulseTimes[i] = pulseInterval * i;
                    }

                    using (var voltageHeader = Mat.CreateMatHeader(pulseVoltages))
                    {
                        CV.Convert(input, voltageHeader);
                    }

                    lock (pulsePal.PulsePal)
                    {
                        pulsePal.PulsePal.SendCustomPulseTrain(PulseId, pulseTimes, pulseVoltages);
                    }
                }));
        }
    }
}
