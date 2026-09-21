namespace SBEnemyRandomizer.src;

using static SBEnemyRandomizer.src.GlobalSettings;

public static class Logger
{
    private static readonly object _lock = new();

    private static readonly string LogFile =
        Path.Combine(logPath, $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log");

    public static void Log(string message)
    {
        Console.WriteLine(message);
        if(basePath == AppContext.BaseDirectory) {
            lock (_lock)
            {
                Directory.CreateDirectory(logPath);
                File.AppendAllText(LogFile, $"{message}{Environment.NewLine}");
            }
        }
    }
}