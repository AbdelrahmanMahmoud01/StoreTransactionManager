using StoreTransactionManager.Core.Dtos;
using StoreTransactionManager.Core.Entites;
using StoreTransactionManager.Core.Interfaces;
using StoreTransactionManager.Infrastructure.Loggers;

namespace StoreTransactionManager.Infrastructure.Services;
public class GoodsService : IGoodsService
{
    private readonly ILogger _consoleLogger;
    private ILogger _serillogLogger;
    private string _directory = string.Empty;
    public GoodsService()
    {
        _consoleLogger = new ConsoleLogger();
    }

    #region Methods
    public async Task<GoodTransactionsDto> GetGoodTransactionsAsync(int goodID, DateTime fromDate, DateTime toDate)
    {
        try
        {
            bool startReadTransactions = false;
            int firstBalance = 0;
            List<GoodTransaction> goodTransactionsList = new List<GoodTransaction>();
            using (var reader = new StreamReader(@"\Data\StoreData.csv"))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();

                    if (line == null)
                        continue;


                    if (IsValidGoodTransactionsHeader(line))
                    {
                        startReadTransactions = true;
                        continue;
                    }

                    if (!startReadTransactions)
                    {
                        if(line.Contains(goodID.ToString()))
                        {
                            int.TryParse(line.Split(';')[0], out firstBalance);
                        }
                        continue;
                    }

                    if (!line.Contains(goodID.ToString()))
                    {
                        continue;
                    }
                     
                    var values = line.Split(';');

                    var goodTransaction = GoodTransaction.CreateValidObject(values);

                    if (goodTransaction is null)
                    {
                        continue;
                    }

                    goodTransactionsList.Add(goodTransaction);
                }
            }
            goodTransactionsList = goodTransactionsList
                .Where(x => x.TransactionDate >= fromDate && x.TransactionDate <= toDate)
                .ToList();
            var result = GoodTransactionsDto.CreateValidObject(goodTransactionsList, firstBalance);
            return result;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task SegregateGoodsDataAsync(string csvFilePath)
    {
        bool isValidPath = IsFileValidPath(ref csvFilePath);
        if (!isValidPath)
        {
            Console.Clear();
            _consoleLogger.LogError("Invalid File Path");
            return;
        }

        var isValidExtension = IsValidFileExtension(csvFilePath);
        if (!isValidExtension)
        {
            Console.Clear();
            _consoleLogger.LogError("Invalid File Extension");
            return;
        }

        _directory = Path.GetDirectoryName(csvFilePath);
        _serillogLogger = new SerilogLogger(_directory);

        _consoleLogger.LogMessage($"proccessing please wait ...");

        var goodTransactionsDictionary = await Task.Run(async () => await GetGoodsFromCsvFileAsync(csvFilePath));

        if (goodTransactionsDictionary != null && goodTransactionsDictionary.Any())
        {
            await Task.Run(async () => await CreateGoodsCsvFilesAsync(goodTransactionsDictionary));
        }
    }

    private async Task<Dictionary<int, List<GoodTransaction>>> GetGoodsFromCsvFileAsync(string filePath)
    {
        var goodTransactionsDictionary = new Dictionary<int, List<GoodTransaction>>();

        try
        {
            bool startReadTransactions = false;
            int rowNumber = -1;
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    rowNumber++;

                    var line = await reader.ReadLineAsync();

                    if (line == null)
                        continue;

                    if (IsValidGoodTransactionsHeader(line))
                    {
                        startReadTransactions = true;
                        continue;
                    }

                    if (!startReadTransactions)
                        continue;


                    var values = line.Split(';');

                    if (values.Length != 6)
                    {
                        _serillogLogger.LogError($"invalid row format with number {rowNumber} : {line}");
                        continue;
                    }

                    var goodTransaction = GoodTransaction.CreateValidObject(values);

                    if (goodTransaction is null)
                    {
                        _serillogLogger.LogError($"Invalid data for row number {rowNumber}: {line}");
                        continue;
                    }


                    if (!goodTransactionsDictionary.ContainsKey(goodTransaction.GoodID))
                    {
                        goodTransactionsDictionary[goodTransaction.GoodID] = new List<GoodTransaction>();
                    }

                    goodTransactionsDictionary[goodTransaction.GoodID].Add(goodTransaction);
                }
            }

            return goodTransactionsDictionary;
        }
        catch (Exception ex)
        {
            _consoleLogger.LogError($"something went wrong please try again");
            _serillogLogger.LogError($"error occuerd while trying to ReadCsvData " +
                $"reason {ex.Message} , stacktrace : {ex.StackTrace} , InnerException : {ex.InnerException}");
            return goodTransactionsDictionary;
        }
    }

    private async Task CreateGoodsCsvFilesAsync(Dictionary<int, List<GoodTransaction>> goodTransactionsDictionary)
    {
        try
        {
            var tasks = goodTransactionsDictionary.Select(async kvp =>
            {
                var goodId = kvp.Key;
                var transactions = kvp.Value;

                var filePath = Path.Combine(_directory, $"{goodId}.csv");

                using (var writer = new StreamWriter(filePath.ToString()))
                {
                    await writer.WriteLineAsync("Good ID;Transaction ID;Transaction Date;Amount;Direction;Comments");

                    foreach (var transaction in transactions)
                    {
                        await writer.WriteLineAsync($"{transaction.GoodID};{transaction.TransactionID};{transaction.TransactionDate:dd/MM/yyyy};{transaction.Amount};{transaction.Direction};{transaction.Comments}");
                    }
                }
                _consoleLogger.LogMessage($"{goodId}_transactions.csv done");
            });

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            _consoleLogger.LogError($"Something went wrong. Please try again.");
            _serillogLogger.LogError($"Error occurred while trying to write CSV data to file. " +
                $"Reason: {ex.Message}, StackTrace: {ex.StackTrace}, InnerException: {ex.InnerException}");
        }
    }

    #region Validations
    public bool IsFileValidPath(ref string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return false;

        if (filePath.StartsWith('"'))
            filePath = filePath.Remove(0, 1);

        if (filePath.EndsWith('"'))
            filePath = filePath.Remove(filePath.Length - 1, 1);

        return File.Exists(filePath);
    }

    //Assumptions : that the allowedExtensions will always be csv 
    private bool IsValidFileExtension(string filePath)
    {
        string extension = Path.GetExtension(filePath);
        return string.Equals(extension, ".csv", StringComparison.OrdinalIgnoreCase);
    }

    private bool IsValidGoodTransactionsHeader(string line)
    {
        var transactionsHeader = "Good ID;Transaction ID;Transaction Date;Amount;Direction;Comments";

        return transactionsHeader.Equals(line, StringComparison.OrdinalIgnoreCase);
    }


    #endregion

    #endregion

}
