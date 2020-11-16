using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

namespace Traffix.Hosting.Console
{

    public class CmdletExecutor : ICmdletExecutor
    {
        ILogger _logger;

        public CmdletExecutor(ILogger<CmdletExecutor> logger = null)
        {
            _logger = logger;
        }

        public IRuntimeProgressReporter ProgressReport { get; set; }

        public async IAsyncEnumerable<T> InvokeAsync<T>(AsyncCmdlet cmdlet)
        {
            if (cmdlet is null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            var results = new List<object>();
            using (var commandRuntime = new CliCommandRuntime(_logger, results, new ProgressReporterProxy(this)))
            {
                cmdlet.CommandRuntime = commandRuntime;
                await cmdlet.ExecuteAsync(commandRuntime).ConfigureAwait(false);
                for (int i = 0; i < results.Count; i++)
                    yield return (T)results[i];
            }
        }

        private class ProgressReporterProxy : IRuntimeProgressReporter
        {
            private CmdletExecutor _cmdletExecutor;

            public ProgressReporterProxy(CmdletExecutor cmdletExecutor)
            {
                this._cmdletExecutor = cmdletExecutor;
            }

            public void Dispose()
            {    
            }

            public void WriteProgress(ProgressRecord progressRecord)
            {
                _cmdletExecutor.ProgressReport?.WriteProgress(progressRecord);
            }
        }
    }

    public abstract class AsyncCmdlet : Cmdlet, IDisposable
    {
        private bool _disposedValue;

        /// <summary>
        /// The source of cancellation token used in the operations.
        /// </summary>
        protected CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
        

        /// <summary>
        /// Request for cancellation. It sets the cancellation token. If and when the processing
        /// stops depends on the implementation of the command let.
        /// </summary>
        public void Cancel()
        {
            StopProcessing();
        }

        internal async Task ExecuteAsync(ICommandRuntime commandRuntime)
        {
            await this.BeginProcessingAsync().ConfigureAwait(false);
            await this.ProcessRecordAsync().ConfigureAwait(false);
            await this.EndProcessingAsync().ConfigureAwait(false);
        }

        protected override void BeginProcessing()
        {
            AsyncHelper.RunSync(BeginProcessingAsync);
        }

        protected override void EndProcessing() 
        {
            AsyncHelper.RunSync(EndProcessingAsync);
        }
        protected override void ProcessRecord()
        {
            AsyncHelper.RunSync(ProcessRecordAsync);
        }
        protected override void StopProcessing()
        {
            AsyncHelper.RunSync(StopProcessingAsync);
        }

        protected abstract Task BeginProcessingAsync();
        protected abstract Task EndProcessingAsync();
        protected abstract Task ProcessRecordAsync();
        protected virtual Task StopProcessingAsync()
        {
            CancellationTokenSource.Cancel();
            return Task.CompletedTask;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {   // dispose managed state (managed objects)
                    CancellationTokenSource.Dispose();
                    
                }
                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~AsyncCmdlet()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    internal static class AsyncHelper
    {
        private static readonly TaskFactory _myTaskFactory = new
          TaskFactory(CancellationToken.None,
                      TaskCreationOptions.None,
                      TaskContinuationOptions.None,
                      TaskScheduler.Default);

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return AsyncHelper._myTaskFactory
              .StartNew(func)
              .Unwrap<TResult>()
              .GetAwaiter()
              .GetResult();
        }

        public static void RunSync(Func<Task> func)
        {
            AsyncHelper._myTaskFactory
              .StartNew(func)
              .Unwrap()
              .GetAwaiter()
              .GetResult();
        }
    }
}
