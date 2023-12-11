using Microsoft.Extensions.Caching.Memory;
using StoreTransactionManager.Core.Dtos;
using StoreTransactionManager.Core.Interfaces;

namespace StoreTransactionManager.Infrastructure.Services;
public class CachedGoodsService : IGoodsService
{
    private readonly IMemoryCache _memoryCache;
    private readonly GoodsService _decoratedGoodsService;

    public CachedGoodsService(IMemoryCache memoryCache, GoodsService decoratedGoodsService)
    {
        _memoryCache = memoryCache;
        _decoratedGoodsService = decoratedGoodsService;
    }

    public async Task<GoodTransactionsDto> GetGoodTransactionsAsync(int goodID, DateTime fromDate, DateTime toDate)
    {
        string cacheKey = $"GoodTransactions_{goodID}_{fromDate}_{toDate}";

        if (_memoryCache.TryGetValue(cacheKey, out GoodTransactionsDto cachedResult))
        {
            return cachedResult;
        }

        try
        {
            var result = await _memoryCache.GetOrCreateAsync(
                cacheKey,
                async entry =>
                {
                    entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                    return await _decoratedGoodsService.GetGoodTransactionsAsync(goodID, fromDate, toDate);
                });

            return result;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task SegregateGoodsDataAsync(string csvFilePath)
    {
        await _decoratedGoodsService.SegregateGoodsDataAsync(csvFilePath);
    }
}
