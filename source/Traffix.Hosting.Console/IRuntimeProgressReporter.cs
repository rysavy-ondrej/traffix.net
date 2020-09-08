using System;
using System.Management.Automation;

namespace Traffix.Hosting.Console
{
    public interface IRuntimeProgressReporter : IDisposable
    {
        void WriteProgress(ProgressRecord progressRecord);
    }
}
