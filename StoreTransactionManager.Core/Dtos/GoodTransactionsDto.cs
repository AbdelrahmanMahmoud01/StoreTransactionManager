using StoreTransactionManager.Core.Entites;

namespace StoreTransactionManager.Core.Dtos;
public class GoodTransactionsDto
{
    private GoodTransactionsDto()
    {
        
    }

    public List<GoodTransaction> Transactions { get; private set; } = new List<GoodTransaction>();
    public int TransactionsCount { get => Transactions.Count(); }
    public int FirstBalance { get; private set; }

    public decimal TransactionsRemainingAmount
	{
		get
		{
            return Transactions
            .Where(transaction => transaction.Direction == Enums.TransactionDirection.In)
            .Sum(transaction => transaction.Amount) -
            Transactions
            .Where(transaction => transaction.Direction == Enums.TransactionDirection.Out)
            .Sum(transaction => transaction.Amount) 
            + FirstBalance;
        }
	}

    public decimal TransactionsTotalAmount
    {
        get
        {
            return Transactions
            .Sum(transaction => transaction.Amount) 
            + FirstBalance;
        }
    }

    public static GoodTransactionsDto New(List<GoodTransaction> transactions , int firstBalance)
    {
        return new GoodTransactionsDto
        {
            Transactions = transactions,
            FirstBalance = firstBalance
        };
    }
}
