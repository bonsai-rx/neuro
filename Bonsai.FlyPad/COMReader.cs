using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace Bonsai.FlyPad
{
    class COMReader
    {
        public event EventHandler<DataStreamEventArgs> onReceiving;

        public COMReader() { }

        public virtual bool isOpen { get; set; }

        public virtual EventHandler<DataStreamEventArgs> OnReceiving { get { return this.onReceiving; } set { this.onReceiving = value; } }

        public virtual string[] PortsAvailable
        {
            get
            {
                int num = 0;
                return SerialPort.GetPortNames().OrderBy(a => a.Length > 3 && int.TryParse(a.Substring(3), out num) ? num : 0).ToArray();
            }
        }

        public virtual bool Open(string port, int baudRate) { return false; }

        public virtual void Close() { }

        public virtual void Send(byte[] packet) { }

        public virtual void Send(string packet) { }

        public virtual int Receive(byte[] bytes, int offset, int count) { return 0; }

        public virtual void Dispose() { this.Close(); }
    }

    class DataStreamEventArgs : EventArgs
    {
        private byte[] _bytes;

        public DataStreamEventArgs(byte[] bytes) { this._bytes = bytes; }

        public byte[] Buffer {get { return this._bytes; }}

        public string StringBuffer { get {
            return ASCIIEncoding.ASCII.GetString(this._bytes); 
        } }

        public string BinaryBuffer { get { return this.formatToBinary(this._bytes); } }

        private string formatToBinary(byte[] data)
        {
            string result = string.Empty;
            foreach (byte value in data)
            {
                string binarybyte = Convert.ToString(value, 2);
                while (binarybyte.Length < 8)
                {
                    binarybyte = "0" + binarybyte;
                }
                result += binarybyte;
            }
            return result;
        }
    }
}
