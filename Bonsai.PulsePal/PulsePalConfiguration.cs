using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Bonsai.IO;
using System.Collections.ObjectModel;

namespace Bonsai.PulsePal
{
    public class PulsePalConfiguration
    {
        readonly ChannelParameterCollection channelParameters = new ChannelParameterCollection();

        [Description("The name of the serial port.")]
        [TypeConverter(typeof(SerialPortNameConverter))]
        public string PortName { get; set; }

        [Description("The collection of parameters used to specify operation of individual channels.")]
        public ChannelParameterCollection ChannelParameters
        {
            get { return channelParameters; }
        }
    }
}
