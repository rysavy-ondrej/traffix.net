# Loiret.Tool.Console

This library provides classes for simplifying the creation of Loiret console applications.
The use of this library is simple. To create a new console application, the main class should be derived 
from `LoiretConsoleApp`. The main method only calls `RunApplicationAsync` method that performs all 
necessary initialization and executes the required command.

Commands are implemented as function that calls command lets. The commad let is an implementation 
of `AsyncCmdlet` abstract class. 

```csharp
using ConsoleAppFramework;
using Loiret.Tools.Console;
using System.Management.Automation;
using System.Threading.Tasks;


namespace Loiret.Example
{
    public class Program : LoiretConsoleApp
    {
        public static async Task Main(string[] args)
        {
            await RunApplicationAsync(args).ConfigureAwait(false);    
        }

        [Command("Print-Hello")]
        public async Task PrintHello(string message)
        {
            using var cmd = new PrintHelloCommand
            {
                Message = message
            };
            await foreach(var msg in ExecuteCommandAsync(cmd).ConfigureAwait(false))
            {
                System.Console.WriteLine(msg);
            }
        }
    }

    [Cmdlet("Print", "Hello")]
    public class PrintHelloCommand : AsyncCmdlet
    {
        public string Message { get; set; }

        public override void Dispose()
        {
                
        }

        protected override Task BeginProcessingAsync()
        {
            return Task.CompletedTask;    
        }

        protected override Task EndProcessingAsync()
        {
            return Task.CompletedTask;
        }

        protected override Task ProcessRecordAsync()
        {
            this.WriteObject($"Hello:{Message}");
            return Task.CompletedTask;
        }

        protected override Task StopProcessingAsync()
        {
            return Task.CompletedTask;
        }
    }
}
```

The console application is then used as follows:

```
> consoleApp.exe Print-Hello -message "some message to print"
Hello:some message to print
```


The example demonstrates the simple way to define a command and process it using command let. The command let usually 
returns object that are read and printed in foreach block. If we are not interested in the resulting object and just want to 
execute the command the following snippet can be executed:

```csharp
ExecuteCommandAsync(cmd).ConsumeAll().ConfigureAwait(false);
```