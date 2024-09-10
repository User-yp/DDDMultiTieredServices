using Microsoft.Extensions.DependencyInjection;

namespace Commons;

public interface IModuleInitializer
{
    public void Initialize(IServiceCollection services);
}