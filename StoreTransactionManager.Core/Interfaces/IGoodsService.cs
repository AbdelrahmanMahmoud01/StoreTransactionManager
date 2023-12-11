using StoreTransactionManager.Core.Dtos;

namespace StoreTransactionManager.Core.Interfaces;
public interface IGoodsService
{
    Task SegregateGoodsDataAsync(string csvFilePath);
    Task<GoodTransactionsDto> GetGoodTransactionsAsync(int goodID , DateTime fromDate, DateTime toDate);
}
