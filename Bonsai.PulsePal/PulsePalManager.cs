using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Reactive.Disposables;

namespace Bonsai.PulsePal
{
    static class PulsePalManager
    {
        public const string DefaultConfigurationFile = "PulsePal.config";
        static readonly Dictionary<string, Tuple<PulsePal, RefCountDisposable>> openConnections = new Dictionary<string, Tuple<PulsePal, RefCountDisposable>>();
        static readonly object openConnectionsLock = new object();

        public static PulsePalDisposable ReserveConnection(string portName)
        {
            if (string.IsNullOrEmpty(portName))
            {
                throw new ArgumentException("A serial port name must be specified.", "portName");
            }

            Tuple<PulsePal, RefCountDisposable> connection;
            lock (openConnectionsLock)
            {
                if (!openConnections.TryGetValue(portName, out connection))
                {
                    var pulsePal = new PulsePal(portName);
                    pulsePal.Open();
                    pulsePal.SetClientId("Bonsai");
                    var configuration = LoadConfiguration();
                    if (configuration.Contains(portName))
                    {
                        var pulsePalConfiguration = configuration[portName];
                        foreach (var parameter in pulsePalConfiguration.ChannelParameters)
                        {
                            pulsePal.ProgramParameter(parameter.Channel, parameter.ParameterCode, parameter.Value);
                        }
                    }

                    var dispose = Disposable.Create(() =>
                    {
                        pulsePal.Close();
                        openConnections.Remove(portName);
                    });

                    var refCount = new RefCountDisposable(dispose);
                    connection = Tuple.Create(pulsePal, refCount);
                    openConnections.Add(portName, connection);
                    return new PulsePalDisposable(pulsePal, refCount);
                }
            }

            return new PulsePalDisposable(connection.Item1, connection.Item2.GetDisposable());
        }

        public static PulsePalConfigurationCollection LoadConfiguration()
        {
            if (!File.Exists(DefaultConfigurationFile))
            {
                return new PulsePalConfigurationCollection();
            }

            var serializer = new XmlSerializer(typeof(PulsePalConfigurationCollection));
            using (var reader = XmlReader.Create(DefaultConfigurationFile))
            {
                return (PulsePalConfigurationCollection)serializer.Deserialize(reader);
            }
        }

        public static void SaveConfiguration(PulsePalConfigurationCollection configuration)
        {
            var serializer = new XmlSerializer(typeof(PulsePalConfigurationCollection));
            using (var writer = XmlWriter.Create(DefaultConfigurationFile, new XmlWriterSettings { Indent = true }))
            {
                serializer.Serialize(writer, configuration);
            }
        }
    }
}
