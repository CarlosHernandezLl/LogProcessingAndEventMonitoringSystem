using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogProcessingAndEventMonitoringSystem.Core
{
    public interface ILogProcessingRule
    {
        bool Apply(LogEntry logEntry, out string? failureMessage);
    }
}
