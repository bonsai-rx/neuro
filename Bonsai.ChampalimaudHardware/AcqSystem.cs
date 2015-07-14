using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCV.Net;
using System.Reactive.Linq;
using System.Threading;
using System.ComponentModel;
using FTD2XX_NET;
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

        [TypeConverter(typeof(SerialPortNameConverter))]
        public string PortName { get; set; }

        public AcqSystemSamplingRate SamplingRate { get; set; }

        public int BufferLength { get; set; }

        public override IObservable<Mat> Generate()
        {
            return Observable.Create<Mat>(observer =>
            {
                var running = true;
                var source = new SerialPort(PortName, BaudRate);
                source.RtsEnable = true;
                source.Open();

                source.Write(string.Format("{0}{1}", StartCommand, (int)SamplingRate));
                var thread = new Thread(() =>
                {
                    var packetOffset = 0;
                    var initialized = false;
                    var dataFrame = new ushort[ChannelCount, BufferLength];
                    var packet = new byte[FrameSize * BufferLength];
                    while (!initialized && running)
                    {
                        var value = (byte)source.ReadByte();
                        initialized = value == SyncByte;
                        packet[0] = value;
                        packetOffset = 1;
                    }

                    while (running)
                    {
                        source.Read(packet, packetOffset, packet.Length - packetOffset);
                        packetOffset = 0;

                        // Check if device data is valid
                        for (int i = 0; i < BufferLength; i++)
                        {
                            if (packet[i * FrameSize] != SyncByte)
                            {
                                observer.OnError(new InvalidOperationException(string.Format(
                                    "Received misaligned data packet: unexpected sync byte {0}.",
                                    packet[i * FrameSize])));
                            }
                        }

                        for (int i = 0; i < ChannelCount; i++)
                        {
                            for (int j = 0; j < BufferLength; j++)
                            {
                                dataFrame[i, j] = (ushort)((
                                    (packet[ChannelOffset + i * 2 + j * FrameSize])
                                    | packet[ChannelOffset + i * 2 + j * FrameSize + 1]) << 8);
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
                    source.Write(StopCommand);
                    source.Close();
                };
            });
        }
    }
}
