using System.Collections.Generic;

namespace Traffix.Hosting.Console
{
    public interface ICmdletExecutor
    {
        IRuntimeProgressReporter ProgressReport { get; set; }
        IAsyncEnumerable<T> InvokeAsync<T>(AsyncCmdlet cmdlet);
    }
}
