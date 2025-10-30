using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogProcessingAndEventMonitoringSystem
{
    public struct LogEntry
    {
        public required Guid Id { get; init; } = Guid.NewGuid();
        public required DateTime Time { get; init; }
        public required LogLevel Level { get; init; }
        public required string Message { get; init; }
        public string? Source { get; init; }

        public LogEntry() { }


    }

    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Debug
    }

}
