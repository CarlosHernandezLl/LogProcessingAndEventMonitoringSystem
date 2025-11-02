using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogProcessingAndEventMonitoringSystem
{
    public class LogFile
    {
        public string FileName { get; set; }
        public DateTime CreatedAt { get; set; }
        public long SizeInBytes { get; set; }
        public LogFile(string fileName, DateTime createdAt, long sizeInBytes)
        {
            FileName = fileName;
            CreatedAt = createdAt;
            SizeInBytes = sizeInBytes;
        }

    }
}
