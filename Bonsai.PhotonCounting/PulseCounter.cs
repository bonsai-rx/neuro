using Bonsai.PhotonCounting.Properties;
using OpenCV.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;

namespace Bonsai.PhotonCounting
{
    [Description("Produces a sequence of photon counts from the C8855 Hamamatsu counting unit.")]
    public class PulseCounter : Source<int>
    {
        public PulseCounter()
        {
            NumberOfGates = 1;
            GateTime = GateTime.GateTime1MS;
        }

        [Description("Specifies the gate integration time.")]
        public GateTime GateTime { get; set; }

        [Range(2, 1024)]
        [Description("Specifies the number of counter gates to acquire signals.")]
        [Editor(DesignTypes.NumericUpDownEditor, "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public int NumberOfGates { get; set; }

        [Description("Specifies the trigger mode.")]
        public TriggerMode TriggerMode { get; set; }

        [Description("Specifies whether the PMT unit should be powered on.")]
        public bool PowerPmt { get; set; }

        [Description("Specifies whether the specified number of measurements should be repeated indefinitely.")]
        public bool ContinuousAcquisition { get; set; }

        public override IObservable<int> Generate()
        {
            return Observable.Create<int>(observer =>
            {
                var running = true;
                var powerPmt = PowerPmt;
                var thread = new Thread(() =>
                {
                    var handle = new IntPtr(-1);
                    try
                    {
                        handle = NativeMethods.C8855Open();
                        if (handle.ToInt64() < 0)
                        {
                            throw new InvalidOperationException(Resources.InvalidHandleException);
                        }

                        var result = NativeMethods.C8855Reset(handle);
                        if (!result)
                        {
                            throw new InvalidOperationException(Resources.ResetException);
                        }

                        if (powerPmt)
                        {
                            result = NativeMethods.C8855SetPmtPower(handle, PowerStatus.PmtPowerOn);
                            if (!result)
                            {
                                throw new InvalidOperationException(Resources.SetPmtPowerException);
                            }
                        }

                        do
                        {
                            var numberOfGates = NumberOfGates;
                            result = NativeMethods.C8855Setup(handle, GateTime, TransferMode.SingleTransfer, (ushort)numberOfGates);
                            if (!result)
                            {
                                throw new InvalidOperationException(Resources.SetupException);
                            }

                            byte dataReturn;
                            result = NativeMethods.C8855CountStart(handle, TriggerMode);
                            if (!result)
                            {
                                throw new InvalidOperationException(Resources.CountStartException);
                            }

                            var buffer = new int[1];
                            var gateCount = NumberOfGates;
                            while (gateCount > 0 && running)
                            {
                                result = NativeMethods.C8855ReadData(handle, buffer, out dataReturn);
                                if (!result)
                                {
                                    throw new InvalidOperationException(Resources.ReadDataException);
                                }

                                observer.OnNext(buffer[0]);
                                gateCount--;
                            }

                            result = NativeMethods.C8855CountStop(handle);
                            if (!result)
                            {
                                throw new InvalidOperationException(Resources.CountStopException);
                            }
                        }
                        while (ContinuousAcquisition && running);

                        observer.OnCompleted();
                    }
                    catch (Exception ex) { observer.OnError(ex); }
                    finally
                    {
                        if (powerPmt)
                        {
                            var result = NativeMethods.C8855SetPmtPower(handle, PowerStatus.PmtPowerOff);
                            if (!result)
                            {
                                throw new InvalidOperationException(Resources.SetPmtPowerException);
                            }
                        }

                        if (handle.ToInt64() > 0)
                        {
                            var result = NativeMethods.C8855Close(handle);
                            if (!result)
                            {
                                throw new InvalidOperationException(Resources.CloseException);
                            }
                        }
                    }
                });

                thread.Start();
                return Disposable.Create(() =>
                {
                    running = false;
                });
            });
        }
    }
}
