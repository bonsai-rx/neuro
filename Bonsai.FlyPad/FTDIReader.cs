using System;
using System.IO.Ports;
using System.Threading;
using Diagnostics = System.Diagnostics;

using FTD2XX_NET;
using System.Collections.Generic;

namespace Bonsai.FlyPad
{
    class FTDIReader : COMReader
    {

        UInt32 ftdiDeviceCount = 0;
        FTDI myFtdiDevice = new FTDI();
        FTDI.FT_STATUS ftStatus; //Create new instance of the FTDI device class
        FTDI.FT_DEVICE_INFO_NODE[] ftdiDeviceList;
        uint numBytesAvailable;
        
        private Thread serialThread;
        private double _PacketsRate;
        private DateTime _lastReceive;

        //The Critical Frequency of Communication to Avoid Any Lag
        private const int frequency = 20;

        public FTDIReader()
        {
            this.ftStatus = myFtdiDevice.GetNumberOfDevices(ref ftdiDeviceCount);
            _lastReceive = DateTime.MinValue;

            serialThread = new Thread(new ThreadStart(SerialReceiving));
            serialThread.Priority = ThreadPriority.Highest;
            serialThread.Name = "SerialHandle" + serialThread.ManagedThreadId;
        }

        public override bool Open(string port, int baudRate)
        {
            try
            {
                ftStatus = myFtdiDevice.OpenByLocation((uint)Convert.ToInt32(port));
                if (ftStatus == FTDI.FT_STATUS.FT_OK)
                {
                    ftStatus = myFtdiDevice.SetTimeouts(5000, 1000);
                    //if (this.isOpen) this.serialThread.Start();
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public override void Close()
        {
            myFtdiDevice.Close();
            //serialThread.Abort();
        }

        public override void Send(byte[] packet)
        {
            uint written = 0;
            this.myFtdiDevice.Write(packet, packet.Length, ref written);
        }

        public override void Send(string packet)
        {
            uint written = 0;
            this.myFtdiDevice.Write(packet, packet.Length, ref written);
        }

        public override void Dispose()
        {
            this.Close();
        }

        private void SerialReceiving()
        {
            uint numBytesRead = 0;
            while (true)
            {
                
                ftStatus = myFtdiDevice.GetRxBytesAvailable(ref numBytesAvailable);

                TimeSpan tmpInterval = (DateTime.Now - this._lastReceive);
                if (numBytesAvailable > 0)
                {
                    byte[] buf = new byte[numBytesAvailable];
                    ftStatus = myFtdiDevice.Read( buf, numBytesAvailable, ref numBytesRead);
                    if (numBytesRead > 0) OnSerialReceiving(buf);
                }

                #region Frequency Control
                _PacketsRate = ((_PacketsRate + numBytesRead) / 2);
                _lastReceive = DateTime.Now;
                ftStatus = myFtdiDevice.GetRxBytesAvailable(ref numBytesAvailable);
                if ((double)(numBytesRead + numBytesAvailable) / 2 <= _PacketsRate)
                {
                    if (tmpInterval.Milliseconds > 0) Thread.Sleep(tmpInterval.Milliseconds > frequency ? frequency : tmpInterval.Milliseconds);
                }
                #endregion
            }
        }

        public void Read(byte[] bytes)
        {
            var numBytesAvailable = 0u;
            while (numBytesAvailable < bytes.Length)
            {
                ftStatus = myFtdiDevice.GetRxBytesAvailable(ref numBytesAvailable);
            }

            myFtdiDevice.Read(bytes, (uint)bytes.Length, ref numBytesAvailable);
        }

        private void OnSerialReceiving(byte[] res)
        {
            if (base.OnReceiving != null) base.OnReceiving(this, new DataStreamEventArgs(res));
        }

        public override bool isOpen
        {
            get
            {
                if (this.myFtdiDevice == null) return false;
                else return this.myFtdiDevice.IsOpen;
            }
        }

        public override string[] PortsAvailable
        {
            get
            {
                List<string> list = new List<string>();
                ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[ftdiDeviceCount];
                ftStatus = myFtdiDevice.GetDeviceList(ftdiDeviceList);

                if (this.ftStatus == FTDI.FT_STATUS.FT_OK)
                {
                    for (UInt32 i = 0; i < ftdiDeviceCount; i++)
                    {
                        list.Add( ftdiDeviceList[i].LocId.ToString());
                    }
                }

                return list.ToArray();
            }
        }




    }





    






}
