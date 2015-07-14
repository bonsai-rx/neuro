using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCV.Net;
using System.Reactive.Linq;
using System.Threading;
using System.ComponentModel;
using System.IO.Ports;
using Bonsai.IO;

namespace Bonsai.ChampalimaudHardware
{
    public class AcqSystem : Source<Mat>
    {
        const string StartCommand = "S";
        const string StopCommand = "R";
        const int BaudRate = 115200;
        const byte SyncByte = 0x40;
        const int ChannelOffset = 2;
        const int ChannelCount = 6;
        const int FrameSize = 15;

        public AcqSystem()
        {
            BufferLength = 10;
            SamplingRate = AcqSystemSamplingRate.SampleRate1000Hz;
        }

        [TypeConverter(typeof(SerialPortNameConverter))]
        public string PortName { get; set; }

        public AcqSystemSamplingRate SamplingRate { get; set; }

        public int BufferLength { get; set; }

        public override IObservable<Mat> Generate()
        {
            return Observable.Create<Mat>(observer =>
            {
                var bufferLength = BufferLength;
                var source = new SerialPort(PortName, BaudRate, Parity.None, 8, StopBits.One);
                source.RtsEnable = true;

                var packetCount = 0;
                var packetOffset = 0;
                var channelOffset = 0;
                var dataFrame = new ushort[ChannelCount, bufferLength];
                var readBuffer = new byte[source.ReadBufferSize];
                source.DataReceived += (sender, e) =>
                {
                    switch (e.EventType)
                    {
                        case SerialData.Chars:
                            var bytesToRead = source.BytesToRead;
                            source.Read(readBuffer, 0, bytesToRead);
                            for (int i = 0; i < bytesToRead; i++)
                            {
                                if (packetOffset == 0 && readBuffer[i] != SyncByte)
                                {
                                    observer.OnError(new InvalidOperationException(string.Format(
                                        "Received misaligned data packet: unexpected sync byte {0}.",
                                        readBuffer[0])));
                                }
                                else if (packetOffset >= ChannelOffset)
                                {
                                    if (packetOffset % 2 == 0)
                                    {
                                        dataFrame[channelOffset, packetCount] = readBuffer[i];
                                    }
                                    else
                                    {
                                        dataFrame[channelOffset, packetCount] |= (ushort)(readBuffer[i] << 8);
                                        channelOffset = (channelOffset + 1) % ChannelCount;
                                    }
                                }

                                packetOffset = (packetOffset + 1) % FrameSize;
                                if (packetOffset == 0)
                                {
                                    packetCount = (packetCount + 1) % bufferLength;
                                    if (packetCount == 0)
                                    {
                                        var dataOutput = Mat.FromArray(dataFrame);
                                        observer.OnNext(dataOutput);
                                    }
                                }
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
