using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCV.Net;
using System.Reactive.Linq;
using System.Threading;
using System.ComponentModel;
using FTD2XX_NET;

namespace Bonsai.FlyPad
{
    public class FlyPadSource : Source<Mat>
    {
        const string StartCommand = "G";
        const string StopCommand = "S";
        const int ChannelCount = 64;
        const int FrameSize = 640;
        FTDI source = new FTDI();

        [TypeConverter(typeof(PortNameConverter))]
        public string PortName { get; set; }

        public int BaudRate { get; set; }

        private static void Write(FTDI source, string command)
        {
            var bytesWritten = 0u;
            var result = source.Write(command, command.Length, ref bytesWritten);
            if (result != FTDI.FT_STATUS.FT_OK)
            {
                throw new InvalidOperationException("Unable to write command to the FTDI device.");
            }
        }

        private static void Read(FTDI source, byte[] bytes)
        {
            var numBytesAvailable = 0u;
            var result = source.Read(bytes, (uint)bytes.Length, ref numBytesAvailable);
            if (result != FTDI.FT_STATUS.FT_OK)
            {
                throw new InvalidOperationException("Failed to read data frame from the FTDI device.");
            }
        }

        public override IObservable<Mat> Generate()
        {
            return Observable.Create<Mat>(observer =>
            {
                var running = true;
                var status = source.OpenBySerialNumber(PortName);
                if (status != FTDI.FT_STATUS.FT_OK)
                {
                    throw new InvalidOperationException("Unable to open the FTDI device at the specified serial port.");
                }

                status = source.SetTimeouts(5000, 1000);
                if (status != FTDI.FT_STATUS.FT_OK)
                {
                    throw new InvalidOperationException("Unable to set timeouts on the FTDI device.");
                }

                Write(source, StartCommand);
                var thread = new Thread(() =>
                {
                    var dataFrame = new short[ChannelCount, 1];
                    var packet = new byte[FrameSize];
                    while (running)
                    {
                        Read(source, packet);
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
                    Write(source, StopCommand);
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
                string[] portNames;
                var numberOfDevices = 0u;
                var source = ((FlyPadSource)context.Instance).source;
                source.GetNumberOfDevices(ref numberOfDevices);
                var deviceList = new FTDI.FT_DEVICE_INFO_NODE[numberOfDevices];
                var status = source.GetDeviceList(deviceList);
                if (status == FTDI.FT_STATUS.FT_OK)
                {
                    portNames = Array.ConvertAll(deviceList, device => device.SerialNumber);
                }
                else portNames = new string[0];
                return new StandardValuesCollection(portNames);
            }
        }
    }
}
