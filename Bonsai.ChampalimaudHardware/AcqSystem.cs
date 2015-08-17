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
    public class AcqSystem : Source<AcqSystemDataFrame>
    {
        const string StartCommand = "S";
        const string StopCommand = "R";
        const int BaudRate = 115200;
        const byte SyncByte = 0x40;
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

        public override IObservable<AcqSystemDataFrame> Generate()
        {
            return Observable.Create<AcqSystemDataFrame>(observer =>
            {
                var bufferLength = BufferLength;
                var source = new SerialPort(PortName, BaudRate, Parity.None, 8, StopBits.One);
                source.RtsEnable = true;

                var ready = false;
                var checksum = 0;
                var packetSum = 0;
                var packetCount = 0;
                var packetOffset = 0;
                var channelOffset = 0;
                var timestamp = new byte[bufferLength];
                var dataFrame = new ushort[ChannelCount, bufferLength];
                var readBuffer = new byte[source.ReadBufferSize];
                source.DataReceived += (sender, e) =>
                {
                    switch (e.EventType)
                    {
                        case SerialData.Chars:
                            try
                            {
                                var bytesRead = source.Read(readBuffer, 0, readBuffer.Length);
                                for (int i = 0; i < bytesRead; i++)
                                {
                                    packetSum += readBuffer[i];
                                    if (!ready)
                                    {
                                        ready = readBuffer[i] == SyncByte;
                                        packetOffset = 0;
                                        packetSum = 0;
                                    }
                                    else if (packetOffset == 0)
                                    {
                                        if (ready = readBuffer[i] == SyncByte && checksum == (packetSum & 255))
                                        {
                                            packetSum = 0;
                                            packetCount = (packetCount + 1) % bufferLength;
                                            if (packetCount == 0)
                                            {
                                                var dataOutput = new AcqSystemDataFrame(timestamp, dataFrame);
                                                observer.OnNext(dataOutput);
                                            }
                                        }
                                    }
                                    else if (packetOffset == 1)
                                    {
                                        timestamp[packetCount] = readBuffer[i];
                                    }
                                    else if (packetOffset == 14)
                                    {
                                        checksum = readBuffer[i];
                                        packetSum -= checksum;
                                    }
                                    else
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
                                }
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
