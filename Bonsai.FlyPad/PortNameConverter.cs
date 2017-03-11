using FTD2XX_NET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.FlyPad
{
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
