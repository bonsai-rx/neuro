﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.ChampalimaudHardware.Mesh
{
    public class Tuna : Source<TunaDataFrame>
    {
        public Tuna()
        {
            Port = 51002;
            TunaPort = 51003;
        }

        public int Port { get; set; }

        public int TunaPort { get; set; }

        public override IObservable<TunaDataFrame> Generate()
        {
            return Observable.Using(
                () => new TunaTransport(Port, TunaPort),
                transport => transport.MessageReceived)
                .SubscribeOn(TaskPoolScheduler.Default);
        }
    }
}
