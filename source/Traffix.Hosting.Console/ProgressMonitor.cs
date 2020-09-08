using System;
using System.Management.Automation;
using System.Threading;

namespace Traffix.Hosting.Console
{
    public class ProgressMonitor : IDisposable
    {
        readonly private Timer _timer;
        readonly private Func<ProgressRecord> _getProgressRecord;
        readonly private ICommandRuntime _runtime;

        public ProgressMonitor(ICommandRuntime runtime, Func<ProgressRecord> getProgressRecord)
        {
            _runtime = runtime;
            _timer = new Timer(OnProgressTls, null, 500, 500);
            _getProgressRecord = getProgressRecord;
        }


        public void Complete(ProgressRecord progressRecord)
        {
            _runtime.WriteProgress(progressRecord);
            _timer.Dispose();
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

        void OnProgressTls(object state)
        {
            _runtime.WriteProgress(_getProgressRecord());
        }
    }
}
