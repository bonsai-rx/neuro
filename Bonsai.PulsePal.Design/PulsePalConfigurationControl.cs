using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Collections.ObjectModel;
using System.Windows.Forms.Design;
using System.IO.Ports;
using Bonsai.IO.Design;
using Bonsai.Design;

namespace Bonsai.PulsePal.Design
{
    public partial class PulsePalConfigurationControl : SerialPortConfigurationControl
    {
        protected override object LoadConfiguration()
        {
            return PulsePalManager.LoadConfiguration();
        }

        protected override void SaveConfiguration(object configuration)
        {
            var pulsePalConfiguration = configuration as PulsePalConfigurationCollection;
            if (pulsePalConfiguration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            PulsePalManager.SaveConfiguration(pulsePalConfiguration);
        }

        protected override CollectionEditor CreateConfigurationEditor(Type type)
        {
            return new PulsePalConfigurationCollectionEditor(type);
        }

        class PulsePalConfigurationCollectionEditor : DescriptiveCollectionEditor
        {
            public PulsePalConfigurationCollectionEditor(Type type)
                : base(type)
            {
            }

            protected override string GetDisplayText(object value)
            {
                var configuration = value as PulsePalConfiguration;
                if (configuration != null)
                {
                    if (!string.IsNullOrEmpty(configuration.PortName))
                    {
                        return configuration.PortName;
                    }

                    return typeof(PulsePalConfiguration).Name;
                }

                return base.GetDisplayText(value);
            }
        }
    }
}
