using System;
using System.Management.Automation;
using SProgressBar = ShellProgressBar.ProgressBar;

namespace Traffix.Hosting.Console
{
    public sealed class ShellRuntimeProgressBar : IRuntimeProgressReporter
    {
        private SProgressBar _progressBar;

        public void Dispose()
        {
            _progressBar?.Dispose();
        }

        public void WriteProgress(ProgressRecord progressRecord)
        {
            if (progressRecord is null)
            {
                throw new ArgumentNullException(nameof(progressRecord));
            }

            if (progressRecord.RecordType == ProgressRecordType.Processing)
            {
                if (_progressBar is null)
                {
                    const int totalTicks = 100;
                    var options = new ShellProgressBar.ProgressBarOptions
                    {
                        DisplayTimeInRealTime = false
                    };

                    System.Console.WriteLine();
                    System.Console.WriteLine();
                    _progressBar = new SProgressBar(totalTicks, progressRecord.Activity, options);
                }

                _progressBar.Tick(progressRecord.PercentComplete, progressRecord.Activity + ": " +  progressRecord.StatusDescription);
            }
            else if (_progressBar != null)
            {
                _progressBar.Tick(100, progressRecord.Activity + ": " + progressRecord.StatusDescription);
                _progressBar.Dispose();
                _progressBar = null;
            }
        }
    }
}
