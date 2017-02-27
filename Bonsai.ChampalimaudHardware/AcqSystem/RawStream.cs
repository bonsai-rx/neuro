using Bonsai.IO;
using OpenCV.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.ChampalimaudHardware.AcqSystem
{
    public class RawStream : Source<Mat>
    {
        const string StartCommand = "S";
        const string StopCommand = "R";
        const int BaudRate = 115200;

        public RawStream()
        {
            BufferLength = 10;
            SamplingRate = SamplingRate.SampleRate1000Hz;
        }

        [TypeConverter(typeof(SerialPortNameConverter))]
        public string PortName { get; set; }

        public SamplingRate SamplingRate { get; set; }

        public int BufferLength { get; set; }

        public override IObservable<Mat> Generate()
        {
            return Observable.Create<Mat>(observer =>
            {
                var bufferLength = BufferLength;
                var source = new SerialPort(PortName, BaudRate, Parity.None, 8, StopBits.One);
                source.Handshake = Handshake.RequestToSend;
                var readBuffer = new byte[source.ReadBufferSize];
                source.DataReceived += (sender, e) =>
                {
                    switch (e.EventType)
                    {
                        case SerialData.Chars:
                            try
                            {
                                var bytesRead = source.Read(readBuffer, 0, readBuffer.Length);
                                var outputArray = new byte[bytesRead];
                                Array.Copy(readBuffer, outputArray, bytesRead);
                                observer.OnNext(Mat.FromArray(outputArray));
                            }
                            catch (Exception ex)
                            {
                                observer.OnError(ex);
                            }

                            break;
                    }
                };

                source.Open();
                source.Write(string.Format("{0}{1}", StartCommand, (int)SamplingRate));
                return () =>
                {
                    source.Write(StopCommand);
                    source.Close();
                };
            });
        }
    }
}
