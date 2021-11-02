using SharpPcap;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Traffix.Providers.PcapFile
{
    public static class ObservableRawFrameSource
    {
        public static IObservable<RawCapture> CreateObservable(this ICaptureFileReader captureReader)
        {
            return Observable.Create<RawCapture>((observer, cancellation) => Task.Factory.StartNew(
                () =>
                {
                    while (!cancellation.IsCancellationRequested && captureReader.GetNextFrame(out var rawFrame))
                    {
                        observer.OnNext(rawFrame);
                    }
                    observer.OnCompleted();
                    captureReader.Dispose();
                }));
        }
    }
}
