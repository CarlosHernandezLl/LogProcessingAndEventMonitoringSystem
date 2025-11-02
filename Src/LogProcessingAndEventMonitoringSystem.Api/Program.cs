using LogProcessingAndEventMonitoringSystem;
using LogProcessingAndEventMonitoringSystem.Core;

class Program
{
    static async Task Main(string[] args)
    {
        var logFilePath = "test_log.txt";
        CreateTestLogFile(logFilePath);

        Console.WriteLine("El archivo de log fue creado en: " + Path.GetFullPath(logFilePath));

        var logProcessor = new LogProcessor();

        await using var logReader = new LogReader(logFilePath);
        {
            logReader.Filter = entry => entry.Level == LogLevel.Error || entry.Level == LogLevel.Warning;

            Console.WriteLine($"\nIniciando lectura, filtrado y validación de reglas de '{logFilePath}'...");
            var processingTasks = new List<Task<List<string?>>>();
            int entryCount = 0;

            await foreach (var logEntry in logReader.ReadLogEntriesAsync())
            {
                entryCount++;

                Console.WriteLine($"[Main] Recibida entrada {entryCount} (Nivel: {logEntry.Level}) para validación: {logEntry.Message.Substring(0, Math.Min(logEntry.Message.Length, 40))}..."); Console.WriteLine($"[{logEntry.Time}] {logEntry.Level}: {logEntry.Message}");

                processingTasks.Add(Task.Run(() => logProcessor.ProccessLogEntry(logEntry)));

                await Task.Delay(50); // Simula procesamiento asíncrono

            }

            Console.WriteLine($"\n[Main] Todas las entradas de log han sido leídas. Esperando que finalicen {processingTasks.Count} tareas de validación...");

            var allValidationResults = await Task.WhenAll(processingTasks);

            Console.WriteLine("\n--- Resumen de Resultados de Validación Dinámica de Reglas: ---");
            int totalFailedEntries = 0;

            foreach (var entryValidationMessages in allValidationResults)
            {
                if (entryValidationMessages.Count > 0)
                {
                    totalFailedEntries++;
                    Console.WriteLine($"\n[ENTRADA CON FALLOS] Falló {entryValidationMessages.Count} regla(s):");
                    foreach (var message in entryValidationMessages)
                    {
                        Console.WriteLine($"  -> {message}");
                    }
                }
            }

            Console.WriteLine($"\n[RESUMEN FINAL] Total de entradas de log con fallos de reglas: {totalFailedEntries}");
        }

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

