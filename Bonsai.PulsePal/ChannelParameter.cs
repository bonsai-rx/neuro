using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.PulsePal
{
    public class ChannelParameter
    {
        public ChannelParameter()
        {
            Channel = 1;
            ParameterCode = ParameterCode.Biphasic;
        }

        [Description("The channel to configure.")]
        public int Channel { get; set; }

        [Description("The parameter to configure.")]
        public ParameterCode ParameterCode { get; set; }

        [Description("The value of the specified parameter.")]
        public int Value { get; set; }
    }
}
