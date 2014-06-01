using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCV.Net;
using System.Reactive.Linq;
using System.Threading;
using System.ComponentModel;

namespace Bonsai.FlyPad
{
    public class FlyPadSource : Source<Mat>
    {
        const int ChannelCount = 64;
        const int FrameSize = 640;
        FTDIReader source = new FTDIReader();

        [TypeConverter(typeof(PortNameConverter))]
        public string PortName { get; set; }

        public int BaudRate { get; set; }

        public override IObservable<Mat> Generate()
        {
            return Observable.Create<Mat>(observer =>
            {
                var running = true;
                source.Open(PortName, BaudRate);
                source.Send("G");
                var thread = new Thread(() =>
                {
                    var dataFrame = new short[ChannelCount, 1];
                    var packet = new byte[FrameSize];
                    while (running)
                    {
                        source.Read(packet);
                        for (int i = 0; i < packet.Length; i += 20)
                        {
                            var channelOffset = (i / 20) * 2;

                            // Check if device data is valid
                            if (packet[i + 18] != 0x3F)
                            {
                                dataFrame[channelOffset + 0, 0] = (Int16)(((packet[i + 1] << 8) | packet[i + 2]) >> 4);
                                dataFrame[channelOffset + 1, 0] = (Int16)(((packet[i + 3] << 8) | packet[i + 4]) >> 4);
                            }
                            else
                            {
                                dataFrame[channelOffset + 0, 0] = -1;
                                dataFrame[channelOffset + 1, 0] = -1;
                            }
                        }

                        var dataOutput = Mat.FromArray(dataFrame);
                        observer.OnNext(dataOutput);
                    }
                });

                thread.Start();
                return () =>
                {
                    running = false;
                    if (thread != Thread.CurrentThread) thread.Join();
                    source.Send("S");
                    source.Close();
                };
            });
        }

        class PortNameConverter : StringConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                var flypad = (FlyPadSource)context.Instance;
                return new StandardValuesCollection(flypad.source.PortsAvailable);
            }
        }
    }
}
