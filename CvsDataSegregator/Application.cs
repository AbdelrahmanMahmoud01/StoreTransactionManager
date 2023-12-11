using StoreTransactionManager.Core.Interfaces;
using StoreTransactionManager.Infrastructure.Loggers;
using StoreTransactionManager.Infrastructure.Services;

namespace GoodsDataSegregator;
public class Application
{
    private readonly ILogger _consoleLogger;
    private readonly IGoodsService _goodsService;

    public Application()
    {
        _consoleLogger = new ConsoleLogger();
        _goodsService = new GoodsService();
    }

    public async Task Run()
    {
        bool tryAgain = true;
        do
        {
            _consoleLogger.LogMessage("Please drop the csv file");
            var filePath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(filePath))
            {
                tryAgain = true;
                _consoleLogger.LogError("file path cannot be empty");
                continue;
            }

            await _goodsService.SegregateGoodsDataAsync(filePath);

            _consoleLogger.LogMessage("want to do another process ? y/n");
            string result = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(result) && result?.ToLower() == "y")
            {
                tryAgain = true;
                Console.Clear();
                continue;
            }

            break;

        } while (tryAgain);

    }
}
