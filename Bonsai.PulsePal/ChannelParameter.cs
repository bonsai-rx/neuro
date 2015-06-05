using System;
using System.Collections.Generic;
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

        public int Channel { get; set; }

        public ParameterCode ParameterCode { get; set; }

        public int Value { get; set; }
    }
}
