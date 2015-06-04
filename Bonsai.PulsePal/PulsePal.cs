using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace Bonsai.PulsePal
{
    sealed class PulsePal : IDisposable
    {
        public const int BaudRate = 115200;
        const int MaxDataBytes = 35;

        const byte OpMenuCommand      = 0xD5;
        const byte HandshakeCommand   = 0x48;
        const byte AcknowledgeCommand = 0x4B;
        const byte TriggerCommand     = 0x4D;
        const byte SetDisplayCommand  = 0x4E;
        const byte SetVoltageCommand  = 0x4F;
        const byte AbortCommand       = 0x50;
        const byte DisconnectCommand  = 0x51;
        const byte LoopCommand        = 0x52;
        const byte ClientIdCommand    = 0x59;
        const byte LineBreak          = 0xFE;

        bool disposed;
        bool initialized;
        readonly SerialPort serialPort;
        readonly byte[] responseBuffer;
        readonly byte[] commandBuffer;
        readonly byte[] readBuffer;

        public PulsePal(string portName)
        {
            serialPort = new SerialPort(portName);
            serialPort.BaudRate = BaudRate;
            serialPort.DataBits = 8;
            serialPort.StopBits = StopBits.One;
            serialPort.Parity = Parity.None;
            serialPort.DtrEnable = false;
            serialPort.RtsEnable = true;

            responseBuffer = new byte[2];
            commandBuffer = new byte[MaxDataBytes];
            readBuffer = new byte[serialPort.ReadBufferSize];
            serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
        }

        public int MajorVersion { get; private set; }

        public int MinorVersion { get; private set; }

        public bool IsOpen
        {
            get { return serialPort.IsOpen; }
        }

        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var bytesToRead = serialPort.BytesToRead;
            if (serialPort.IsOpen && bytesToRead > 0)
            {
                bytesToRead = serialPort.Read(readBuffer, 0, bytesToRead);
                for (int i = 0; i < bytesToRead; i++)
                {
                    ProcessInput(readBuffer[i]);
                }
            }
        }

        public void Open()
        {
            serialPort.Open();
            serialPort.ReadExisting();
            commandBuffer[0] = OpMenuCommand;
            commandBuffer[1] = HandshakeCommand;
            serialPort.Write(commandBuffer, 0, 2);
        }

        public void TriggerOutputChannels(byte channels)
        {
            commandBuffer[0] = OpMenuCommand;
            commandBuffer[1] = TriggerCommand;
            commandBuffer[2] = channels;
            serialPort.Write(commandBuffer, 0, 3);
        }

        int WriteText(string text, int index)
        {
            var i = 0;
            for (; i < text.Length && i < 16; i++)
            {
                commandBuffer[i + index] = (byte)text[i];
            }

            return index + i;
        }

        public void SetDisplay(string text)
        {
            SetDisplay(text, string.Empty);
        }

        public void SetDisplay(string row1, string row2)
        {
            var index = 0;
            commandBuffer[index++] = OpMenuCommand;
            commandBuffer[index++] = SetDisplayCommand;
            index = WriteText(row1, index);
            if (!string.IsNullOrEmpty(row2))
            {
                commandBuffer[index++] = LineBreak;
                WriteText(row2, index);
            }
        }

        public void SetFixedVoltage(byte channel, byte voltage)
        {
            commandBuffer[0] = OpMenuCommand;
            commandBuffer[1] = SetVoltageCommand;
            commandBuffer[2] = channel;
            commandBuffer[3] = voltage;
            serialPort.Write(commandBuffer, 0, 4);
        }

        public void AbortPulseTrains()
        {
            commandBuffer[0] = OpMenuCommand;
            commandBuffer[1] = AbortCommand;
            serialPort.Write(commandBuffer, 0, 2);
        }

        public void SetContinuousLoop(byte channel, bool loop)
        {
            commandBuffer[0] = OpMenuCommand;
            commandBuffer[1] = LoopCommand;
            commandBuffer[2] = channel;
            commandBuffer[3] = (byte)(loop ? 1 : 0);
            serialPort.Write(commandBuffer, 0, 4);
        }

        public void SetClientId(string id)
        {
            commandBuffer[0] = OpMenuCommand;
            commandBuffer[1] = ClientIdCommand;
            for (int i = 0; i < 6; i++)
            {
                commandBuffer[i + 2] = i < id.Length ? (byte)id[i] : (byte)' ';
            }
            serialPort.Write(commandBuffer, 0, 8);
        }

        void SetVersion(int majorVersion, int minorVersion)
        {
            MajorVersion = majorVersion;
            MinorVersion = minorVersion;
        }

        void ProcessInput(byte inputData)
        {
            if (!initialized && inputData != AcknowledgeCommand)
            {
                throw new InvalidOperationException("Unexpected return value from PulsePal.");
            }

            switch (inputData)
            {
                case AcknowledgeCommand:
                    initialized = true;
                    break;
                default:
                    break;
            }
        }

        public void Close()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~PulsePal()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    commandBuffer[0] = OpMenuCommand;
                    commandBuffer[1] = DisconnectCommand;
                    serialPort.Write(commandBuffer, 0, 2);
                    serialPort.Close();
                    disposed = true;
                }
            }
        }

        void IDisposable.Dispose()
        {
            Close();
        }
    }
}
