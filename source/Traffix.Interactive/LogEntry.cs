using System;

namespace Traffix.Interactive
{
    public enum LogLevel { Trace, Debug, Information, Warning, Error, Critical, None }
    public struct LogEntry
    {
        public LogLevel Level { get; }
        public string Message { get; }

        public Exception Exception { get; } 
    }
}
