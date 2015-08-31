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
    public class PulseCounter : Source<Mat>
    {
        public PulseCounter()
        {
            NumberOfGates = 1;
            GateTime = GateTime.GateTime1MS;
            TransferMode = TransferMode.BlockTransfer;
        }

        [Description("Specifies the gate integration time.")]
        public GateTime GateTime { get; set; }

        [Description("Specifies the counter data transfer mode. Should be single transfer if the data is less than 1024.")]
        public TransferMode TransferMode { get; set; }

        [Description("Specifies the number of counter gates to acquire signals.")]
        public int NumberOfGates { get; set; }

        [Description("Specifies the trigger mode.")]
        public TriggerMode TriggerMode { get; set; }

        [Description("Specifies whether the PMT unit should be powered on.")]
        public bool PowerPmt { get; set; }

        public override IObservable<Mat> Generate()
        {
            return Observable.Create<Mat>(observer =>
            {
                var running = true;
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

                        var powerPmt = PowerPmt;
                        if (powerPmt)
                        {
                            result = NativeMethods.C8855SetPmtPower(handle, PowerStatus.PmtPowerOn);
                            if (!result)
                            {
                                throw new InvalidOperationException(Resources.SetPmtPowerException);
                            }
                        }

                        while (running)
                        {
                            var transferMode = TransferMode;
                            var numberOfGates = NumberOfGates;
                            var bufferSize = transferMode == TransferMode.SingleTransfer ? numberOfGates : numberOfGates * 1024;
                            result = NativeMethods.C8855Setup(handle, GateTime, transferMode, (ushort)numberOfGates);
                            if (!result)
                            {
                                throw new InvalidOperationException(Resources.SetupException);
                            }

                            byte datareturn;
                            int[] buffer = new int[bufferSize];
                            result = NativeMethods.C8855CountStart(handle, TriggerMode);
                            result = NativeMethods.C8855ReadData(handle, buffer, out datareturn);
                            result = NativeMethods.C8855CountStop(handle);
                            observer.OnNext(Mat.FromArray(buffer));
                        }

                        if (powerPmt)
                        {
                            result = NativeMethods.C8855SetPmtPower(handle, PowerStatus.PmtPowerOff);
                            if (!result)
                            {
                                throw new InvalidOperationException(Resources.SetPmtPowerException);
                            }
                        }
                    }
                    catch (Exception ex) { observer.OnError(ex); }
                    finally
                    {
                        if (handle.ToInt64() > 0)
                        {
                            NativeMethods.C8855Close(handle);
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
