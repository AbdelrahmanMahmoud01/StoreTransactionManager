using StoreTransactionManager.Core.Enums;
using System.Globalization;

namespace StoreTransactionManager.Core.Entites;
public class GoodTransaction
{
    private GoodTransaction() { }

    public int GoodID { get; private set; }
    public int TransactionID { get; private set; }
    public DateTime TransactionDate { get; private set; }
    public decimal Amount { get; private set; }
    public TransactionDirection Direction { get; private set; }
    public string? Comments { get; private set; }

    public static GoodTransaction CreateValidObject(string?[] lineData)
    {
        if (!TryParseLineData(lineData, out var goodTransaction))
        {
            return null;
        }

        return goodTransaction;
    }

    private static bool TryParseLineData(string?[] lineData, out GoodTransaction goodTransaction)
    {
        goodTransaction = null;

        if (!int.TryParse(lineData[0], out int goodId)
            || !int.TryParse(lineData[1], out int transactionId)
            || !DateTime.TryParseExact(lineData[2], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime transactionDate)
            || !decimal.TryParse(lineData[3], out decimal amount)
            || !Enum.TryParse(lineData[4], out TransactionDirection direction))
        {
            return false;
        }

        goodTransaction = new GoodTransaction
        {
            Amount = amount,
            Direction = direction,
            GoodID = goodId,
            TransactionDate = transactionDate,
            TransactionID = transactionId,
            Comments = lineData[5]
        };

        return true;
    }
}


