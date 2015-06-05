using Bonsai.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.PulsePal.Design
{
    class ChannelParameterCollectionEditor : DescriptiveCollectionEditor
    {
        public ChannelParameterCollectionEditor(Type type)
            : base(type)
        {
        }

        protected override string GetDisplayText(object value)
        {
            var channelParameter = value as ChannelParameter;
            if (channelParameter != null)
            {
                return channelParameter.ParameterCode.ToString();
            }

            return base.GetDisplayText(value);
        }
    }
}
