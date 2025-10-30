using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogProcessingAndEventMonitoringSystem
{

    public class LogReader : IAsyncDisposable
    {
        public Func<LogEntry, bool>? Filter { get; set; }
        private readonly string _filePath;
        private StreamReader? _reader;
        private bool _disposed = false;

        public LogReader(string FilePath)
        {
            _filePath = FilePath;
            try
            {

                _reader = new StreamReader(_filePath);
                Console.WriteLine($"[LogReader] Archivo '{_filePath}' abierto.");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File Not Found: {ex.Message}");
                _reader = null;
                _disposed = true;
                throw;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error opening log file: {ex.Message}");
                _reader = null;
                _disposed = true;
                throw;
            }
        }

        public async IAsyncEnumerable<LogEntry> ReadLogEntriesAsync()
        {
            if (_disposed || _reader == null)
            {
                throw new ObjectDisposedException("LogReader has been disposed.");
            }

            string? line;

            while ((line = await _reader.ReadLineAsync()) != null)
            {
                var logEntry = ParseLogEntry(line);

                if (Filter == null || Filter(logEntry))
                {
                    yield return logEntry;
                }

            }

        }

        public LogEntry ParseLogEntry(string? line)
        {

            if (string.IsNullOrWhiteSpace(line))
            {
                return new LogEntry
                {
                    Id = Guid.NewGuid(),
                    Time = DateTime.UtcNow,
                    Level = LogLevel.Warning,
                    Message = "Empty log entry."
                };
            }

            try
            {
                var parts = line?.Split(']');

                if(parts == null || parts.Length < 2)
                {
                    throw new FormatException("Log entry format is invalid.");
                }

                var timestampAndLevel = parts?[0].TrimStart('[');
                var message = parts?[1].Trim();

                var tokens = timestampAndLevel?.Split(' ');
                var datetime = $"{tokens?[0]} {tokens?[1]}";
                var level = tokens?[2].ToLower();
                level = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(level ?? String.Empty);

                DateTime time;
                if (!DateTime.TryParse(datetime, out time))
                {
                    throw new FormatException("Timestamp format is invalid.");

                }

                LogLevel logLevel;
                if (!Enum.TryParse<LogLevel>(level, out logLevel))
                {
                    logLevel = LogLevel.Info;
                }

                return new LogEntry
                {
                    Id = Guid.NewGuid(),
                    Time = time,
                    Level = logLevel,
                    Message = message ?? string.Empty
                };

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing log entry: {ex.Message}");
                return new LogEntry
                {
                    Id = Guid.NewGuid(),
                    Time = DateTime.MinValue,
                    Level = LogLevel.Info,
                    Message = "Invalid log entry format."
                };
            }

        }


        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                if (_reader != null)
                {
                    // await _reader.DisposeAsync();
                    _reader = null;
                 

                    Console.WriteLine("[LogReader] Recurso de StreamReader liberado de forma asíncrona.");
                }
                _disposed = true;
                GC.SuppressFinalize(this);
            }

            await ValueTask.CompletedTask;
        }




    }
}
