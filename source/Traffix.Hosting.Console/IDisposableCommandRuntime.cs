using System;
using System.Management.Automation;

namespace Traffix.Hosting.Console
{
    internal interface IDisposableCommandRuntime : ICommandRuntime, IDisposable
    {
    }
}
