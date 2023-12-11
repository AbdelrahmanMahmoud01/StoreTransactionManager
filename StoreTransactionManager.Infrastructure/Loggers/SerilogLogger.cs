using Serilog;

namespace StoreTransactionManager.Infrastructure.Loggers;
public class SerilogLogger : Core.Interfaces.ILogger
{
    public SerilogLogger(string logFilePath)
    {
        logFilePath = Path.Combine(logFilePath, $"errors.txt");
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(logFilePath)
            .CreateLogger();
    }

    public void LogError(string message)
    {
        try
        {
            Log.Error(message);
        }
        catch (Exception)
        {
            
        }
    }

    public void LogMessage(string message)
    {
        Log.Information(message);
    }
}
