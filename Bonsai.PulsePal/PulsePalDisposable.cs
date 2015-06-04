using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reactive.Disposables;

namespace Bonsai.PulsePal
{
    sealed class PulsePalDisposable : ICancelable, IDisposable
    {
        IDisposable resource;

        public PulsePalDisposable(PulsePal pulsePal, IDisposable disposable)
        {
            if (pulsePal == null)
            {
                throw new ArgumentNullException("pulsePal");
            }

            if (disposable == null)
            {
                throw new ArgumentNullException("disposable");
            }

            PulsePal = pulsePal;
            resource = disposable;
        }

        public PulsePal PulsePal { get; private set; }

        public bool IsDisposed
        {
            get { return resource == null; }
        }

        public void Dispose()
        {
            var disposable = Interlocked.Exchange<IDisposable>(ref resource, null);
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
