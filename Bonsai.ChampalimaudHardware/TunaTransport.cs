using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bonsai.ChampalimaudHardware
{
    class TunaTransport : IDisposable
    {
        const byte DataAndConf = 0x81;
        static readonly byte[] KeepAlive = new byte[] { 0x08, 0x08 };

        UdpClient client;
        IObservable<TunaDataFrame> messageReceived;

        public TunaTransport(int port, int tunaPort)
        {
            client = new UdpClient(port);
            messageReceived = Observable.Using(
                () => new EventLoopScheduler(),
                scheduler => Observable.Create<TunaDataFrame>(observer =>
                {
                    return scheduler.Schedule(recurse =>
                    {
                        try
                        {
                            var endPoint = new IPEndPoint(IPAddress.Any, 0);
                            var message = client.Receive(ref endPoint);
                            endPoint.Port = tunaPort;
                            var data = new TunaDataFrame(message);
                            if (message[0] == DataAndConf)
                            {
                                client.Send(KeepAlive, KeepAlive.Length, endPoint);
                            }
                            observer.OnNext(data);
                            recurse();
                        }
                        catch (Exception e)
                        {
                            observer.OnError(e);
                        }
                    });
                }))
                .PublishReconnectable()
                .RefCount();
        }

        public IObservable<TunaDataFrame> MessageReceived
        {
            get { return messageReceived; }
        }

        ~TunaTransport()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            var disposable = Interlocked.Exchange(ref client, null);
            if (disposable != null && disposing)
            {
                disposable.Close();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
