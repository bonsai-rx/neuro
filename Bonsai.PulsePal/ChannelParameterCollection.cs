using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.PulsePal
{
    [Editor("Bonsai.PulsePal.Design.ChannelParameterCollectionEditor, Bonsai.PulsePal.Design", typeof(UITypeEditor))]
    public class ChannelParameterCollection : Collection<ChannelParameter>
    {
    }
}
