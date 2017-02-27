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

namespace Bonsai.ChampalimaudHardware.AcqSystem
{
    public class AcquisitionBoard : Source<DataFrame>
    {
        const string StartCommand = "S";
        const string StopCommand = "R";
        const int DefaultBaudRate = 115200;
        const int ChannelCount = 6;
        const int AuxiliaryDataSize = 20;
        const int AmplifierFrameSize = 2 * ChannelCount + 4;
        const int AuxiliaryFrameSize = AuxiliaryDataSize + 5;

        public AcquisitionBoard()
        {
            SamplingRate = SamplingRate.SampleRate1000Hz;
            BaudRate = DefaultBaudRate;
        }

        [TypeConverter(typeof(SerialPortNameConverter))]
        public string PortName { get; set; }

        public SamplingRate SamplingRate { get; set; }

        [TypeConverter(typeof(BaudRateConverter))]
        public int BaudRate { get; set; }

        static DataFrame CreateDataFrame(byte syncByte, out AnalogFrame analogFrame, out DigitalFrame digitalFrame)
        {
            switch (syncByte)
            {
                case 0x40:
                    digitalFrame = null;
                    return analogFrame = new AnalogFrame { Type = DataFrameType.Analog };
                case 0x2A:
                    digitalFrame = null;
                    return analogFrame = new AnalogFrame { Type = DataFrameType.Trigger };
                case 0x3F:
                    analogFrame = null;
                    return digitalFrame = new DigitalFrame { Type = DataFrameType.Digital };
                default:
                    analogFrame = null;
                    return digitalFrame = null;
            }
        }

        static int GetFrameSize(DataFrame dataFrame)
        {
            if (dataFrame == null) return 1;
            else
            {
                switch (dataFrame.Type)
                {
                    case DataFrameType.Analog:
                    case DataFrameType.Trigger:
                        return AmplifierFrameSize;
                    case DataFrameType.Digital:
                        return AuxiliaryFrameSize;
                    default: return 1;
                }
            }
        }

        public override IObservable<DataFrame> Generate()
        {
            return Observable.Create<DataFrame>(observer =>
            {
                var source = new SerialPort(PortName, BaudRate, Parity.None, 8, StopBits.One);
                source.Handshake = Handshake.RequestToSend;

                var checksum = 0;
                var packetSum = 0;
                var packetOffset = 0;
                var payloadOffset = 0;
                var dataFrame = default(DataFrame);
                var analogFrame = default(AnalogFrame);
                var digitalFrame = default(DigitalFrame);
                var frameSize = GetFrameSize(dataFrame);
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
                                    if (dataFrame == null)
                                    {
                                        dataFrame = CreateDataFrame(readBuffer[i], out analogFrame, out digitalFrame);
                                        frameSize = GetFrameSize(dataFrame);
                                        packetOffset = 0;
                                    }
                                    else if (packetOffset == frameSize - 1)
                                    {
                                        checksum = readBuffer[i];
                                        packetSum -= checksum;
                                        if (checksum == (packetSum & 255))
                                        {
                                            packetSum = 0;
                                            observer.OnNext(dataFrame);
                                        }

                                        analogFrame = null;
                                        digitalFrame = null;
                                        dataFrame = null;
                                    }
                                    else if (analogFrame != null)
                                    {
                                        // Parse analog frame payload
                                        if (packetOffset == 1) analogFrame.Counter = readBuffer[i];
                                        else if (packetOffset == 2) analogFrame.Counter |= (ushort)(readBuffer[i] << 8);
                                        else
                                        {
                                            if (packetOffset % 2 != 0)
                                            {
                                                analogFrame.Data[payloadOffset] = readBuffer[i];
                                            }
                                            else
                                            {
                                                analogFrame.Data[payloadOffset] |= (ushort)(readBuffer[i] << 8);
                                                payloadOffset = (payloadOffset + 1) % ChannelCount;
                                            }
                                        }
                                    }
                                    else // digitalFrame != null
                                    {
                                        // Parse digital frame payload
                                        if (packetOffset == 1) digitalFrame.Counter = readBuffer[i];
                                        else if (packetOffset == 2) digitalFrame.Channel = readBuffer[i];
                                        else if (packetOffset == 3) digitalFrame.SensorType = readBuffer[i];
                                        else
                                        {
                                            digitalFrame.Data[payloadOffset] = readBuffer[i];
                                            payloadOffset = (payloadOffset + 1) % AuxiliaryDataSize;
                                        }
                                    }

                                    packetOffset = (packetOffset + 1) % frameSize;
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
