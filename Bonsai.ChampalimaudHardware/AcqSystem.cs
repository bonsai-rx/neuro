using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCV.Net;
using System.Reactive.Linq;
using System.Threading;
using System.ComponentModel;
using FTD2XX_NET;

namespace Bonsai.ChampalimaudHardware
{
    public class AcqSystem : Source<Mat>
    {
        const string StartCommand = "S";
        const string StopCommand = "R";
        const byte SyncByte = 0x40;
        const int ChannelOffset = 2;
        const int ChannelCount = 6;
        const int FrameSize = 15;

        [TypeConverter(typeof(PortNameConverter))]
        public int LocationId { get; set; }

        public AcqSystemSamplingRate SamplingRate { get; set; }

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
                var source = new FTDI();
                var status = source.OpenByLocation((uint)LocationId);
                if (status != FTDI.FT_STATUS.FT_OK)
                {
                    throw new InvalidOperationException("Unable to open the FTDI device at the specified serial port.");
                }

                status = source.SetTimeouts(5000, 1000);
                if (status != FTDI.FT_STATUS.FT_OK)
                {
                    throw new InvalidOperationException("Unable to set timeouts on the FTDI device.");
                }

                Write(source, string.Format("{0}{1}", StartCommand, (int)SamplingRate));
                var thread = new Thread(() =>
                {
                    var dataFrame = new ushort[ChannelCount, 1];
                    var packet = new byte[FrameSize];
                    while (running)
                    {
                        Read(source, packet);

                        // Check if device data is valid
                        if (packet[0] != SyncByte)
                        {
                            observer.OnError(new InvalidOperationException(string.Format(
                                "Received misaligned data packet: unexpected sync byte {0}.",
                                packet[0])));
                        }

                        for (int i = 0; i < ChannelCount; i++)
                        {
                            dataFrame[i, 0] = (ushort)(((packet[ChannelOffset + i * 2]) | packet[ChannelOffset + i * 2 + 1]) << 8);
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

        class PortNameConverter : Int32Converter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                int[] portNames;
                var numberOfDevices = 0u;
                var source = new FTDI();
                source.GetNumberOfDevices(ref numberOfDevices);
                var deviceList = new FTDI.FT_DEVICE_INFO_NODE[numberOfDevices];
                var status = source.GetDeviceList(deviceList);
                if (status == FTDI.FT_STATUS.FT_OK)
                {
                    portNames = Array.ConvertAll(deviceList, device => (int)device.LocId);
                }
                else portNames = new int[0];
                return new StandardValuesCollection(portNames);
            }
        }
    }
}
