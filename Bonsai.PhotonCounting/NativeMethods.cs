using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.PhotonCounting
{
    public static class NativeMethods
    {
        const string LibName = "C8855-01api";
        public const byte TransferError = 0xFF;

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr C8855Open();

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall)]
        public static extern bool C8855MOpen(byte board,
            out IntPtr hC8855_1, out IntPtr hC8855_2,
            out IntPtr hC8855_3, out IntPtr hC8855_4,
            out IntPtr hC8855_5, out IntPtr hC8855_6,
            out IntPtr hC8855_7, out IntPtr hC8855_8,
            out IntPtr hC8855_9, out IntPtr hC8855_10,
            out IntPtr hC8855_11, out IntPtr hC8855_12,
            out IntPtr hC8855_13, out IntPtr hC8855_14,
            out IntPtr hC8855_15, out IntPtr hC8855_16);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall)]
        public static extern bool C8855Close(IntPtr hC8855);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall)]
        public static extern bool C8855Reset(IntPtr hC8855);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall)]
        public static extern bool C8855CountStart(IntPtr hC8855, TriggerMode triggerMode);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall)]
        public static extern bool C8855CountStop(IntPtr hC8855);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall)]
        public static extern bool C8855Setup(IntPtr hC8855, GateTime gateTime, TransferMode transferMode, ushort numberOfGates);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall)]
        public static extern bool C8855ReadData(IntPtr hC8855, [Out]int[] dataBuffer, out byte resultReturned);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall)]
        public static extern bool C8855SetPmtPower(IntPtr hC8855, PowerStatus powerStatus);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall)]
        public static extern bool C8855WritePort(IntPtr hC8855, byte data);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall)]
        public static extern bool C8855ReadID(IntPtr hC8855, out byte data);
    }
}
