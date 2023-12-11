using StoreTransactionManager.Core.Interfaces;

namespace StoreTransactionManager.Infrastructure.Loggers;
public class ConsoleLogger : ILogger
{
    public void LogError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(Environment.NewLine + message);
    }

    public void LogMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(Environment.NewLine + message);
    }
}
