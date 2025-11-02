using LogProcessingAndEventMonitoringSystem.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogProcessingAndEventMonitoringSystem.Plugins
{
    public class CriticalNightRule : ILogProcessingRule
    {
        public CriticalNightRule() { }

        public bool Apply(LogEntry logEntry, out string? failureMessage)
        {
            // Regla: Si el nivel de log es Error y la hora es entre las 00:00 y las 06:00, marcar como fallo.
            if (logEntry.Level == LogLevel.Error &&
                logEntry.Time.TimeOfDay >= TimeSpan.FromHours(0) &&
                logEntry.Time.TimeOfDay < TimeSpan.FromHours(24))
            {
                failureMessage = $"[CriticalNightRule] Critical error logged at night: {logEntry.Message} (Time: {logEntry.Time})";
                return false;
            }
            failureMessage = null;
            return true;
        }

    }
}
