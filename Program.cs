using LogProcessingAndEventMonitoringSystem;

class Program
{
    static async Task Main(string[] args)
    {
        var logFilePath = "test_log.txt";
        CreateTestLogFile(logFilePath);

        Console.WriteLine("El archivo fue creado en la ruta: " + Path.GetFullPath(logFilePath));

        Console.WriteLine("Reading log entries from file:");

        await using var logReader = new LogReader(logFilePath);
        {
            logReader.Filter = entry => entry.Level == LogLevel.Error || entry.Level == LogLevel.Warning;
            
            await foreach (var logEntry in logReader.ReadLogEntriesAsync())
            {
                await Task.Delay(50);
                Console.WriteLine($"[{logEntry.Time}] {logEntry.Level}: {logEntry.Message}");
            }
        }

        Console.WriteLine("\n--- Fin del Nivel 2 ---");
        Console.WriteLine("Presiona cualquier tecla para finalizar...");
        Console.ReadKey();

    }

    static void CreateTestLogFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        File.WriteAllLines(filePath, new string[]
        {
                $"[{DateTime.Now.AddSeconds(-5):yyyy-MM-dd HH:mm:ss} INFO] Application started.",
                $"[{DateTime.Now.AddSeconds(-4):yyyy-MM-dd HH:mm:ss} DEBUG] User 'Alice' logged in.",
                $"[{DateTime.Now.AddSeconds(-3):yyyy-MM-dd HH:mm:ss} ERROR] Failed to connect to database.",
                $"[{DateTime.Now.AddSeconds(-2):yyyy-MM-dd HH:mm:ss} INFO] Processing data batch 1.",
                $"[{DateTime.Now.AddSeconds(-1):yyyy-MM-dd HH:mm:ss} CRITICAL] Server shutdown detected.",
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss} WARNING] Low disk space on /dev/sda1."
        });
        Console.WriteLine($"Archivo de log de prueba '{filePath}' creado.");
    }

}
