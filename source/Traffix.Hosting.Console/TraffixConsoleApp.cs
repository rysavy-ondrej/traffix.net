using ConsoleAppFramework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Traffix.Hosting.Console
{
    public class TraffixConsoleApp : ConsoleAppBase
    {
        /// <summary>
        /// Reference to created host instance that manages the current console application.
        /// </summary>
        protected static IHost ApplicationHost;

        /// <summary>
        /// Runs the console application using the default settings.
        /// </summary>
        /// <param name="args">the command line arguments.</param>
        /// <returns>Completion task.</returns>
        protected async static Task RunApplicationAsync(string[] args)
        {
            var builder = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();
                    services.AddScoped(typeof(ICmdletExecutor), typeof(CmdletExecutor));
                })

                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddConsole();
                    configLogging.AddDebug();
                });

            ApplicationHost = builder.UseConsoleAppFramework(args).Build();
            await ApplicationHost.RunAsync().ConfigureAwait(false);
            ApplicationHost.Dispose();
        }

        /// <summary>
        /// Gets the service of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the service to retrieve.</typeparam>
        /// <returns>An instance of the service for the given type.</returns>
        protected static T GetService<T>()
        {
            if (ApplicationHost == null) throw new InvalidOperationException("Host instance cannot be null.");
            return ApplicationHost.Services.GetService<T>();
        }
        /// <summary>
        /// Executes the given command in the newly created executor. 
        /// </summary>
        /// <param name="cmd">The command to execute.</param>
        /// <param name="noProgress">Set to true if progress information should not be displayed.</param>
        /// <returns>The completion task.</returns>
        protected static async IAsyncEnumerable<object> ExecuteCommandAsync(AsyncCmdlet cmd, bool noProgress = false)
        {
            var executor = GetService<ICmdletExecutor>();
            if (noProgress == false)
            {
                executor.ProgressReport = new ShellRuntimeProgressBar();
            }
            var results = executor.InvokeAsync<object>(cmd);

            await foreach (var result in results)
            {
                yield return result;
            }
        }


    }

    public static class AsyncEnumerable
    {
        public static async Task ConsumeAll<T>(this IAsyncEnumerable<T> items)
        {
            await foreach (var _ in items)
            {
            }
        }
    }
}
