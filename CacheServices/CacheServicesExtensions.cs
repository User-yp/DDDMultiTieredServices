using CacheServices.LocalCache;
using CacheServices.RedisService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CacheServices;

public static class CacheServicesExtensions
{
    public static IServiceCollection AddRedisService(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);

        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<RedisOptions>()
                .Configure(configuration.Bind)
                .ValidateDataAnnotations();

        services.AddSingleton<IRedisService, RedisService.RedisService>();
        return services;
    }

    public static IServiceCollection AddRedisService(this IServiceCollection services, Action<RedisOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);

        ArgumentNullException.ThrowIfNull(configureOptions);

        services.Configure(configureOptions);
        services.AddSingleton<IRedisService, RedisService.RedisService>();
        return services;
    }

    public static IServiceCollection AddMemoryCacheService(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddDistributedMemoryCache();
        services.AddScoped<IMemoryCacheService, MemoryCacheService>();
        services.AddScoped<IDistributedCacheService, DistributedCacheService>();
        return services;
    }
}
