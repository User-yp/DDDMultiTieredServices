using Commons.Initializer;
using Microsoft.Extensions.DependencyInjection;

namespace CacheServices;

public class ModuleInitializer : IModuleInitializer
{
    public void Initialize(IServiceCollection services)
    {
        services.AddMemoryCacheService();
    }
}
