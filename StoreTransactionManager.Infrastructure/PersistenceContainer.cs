using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StoreTransactionManager.Core.Interfaces;
using StoreTransactionManager.Infrastructure.Services;

namespace StoreTransactionManager.Infrastructure;
public static class PersistenceContainer
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<GoodsService>();
        services.AddScoped<IGoodsService, CachedGoodsService>();
        return services;
    }

}
