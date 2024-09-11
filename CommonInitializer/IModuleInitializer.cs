using Microsoft.Extensions.DependencyInjection;

namespace CommonInitializer;

public interface IModuleInitializer
{
    public void Initialize(IServiceCollection services);
}
