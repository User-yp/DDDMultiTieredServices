using Microsoft.Extensions.DependencyInjection;

namespace Commons.Initializer;

public interface IModuleInitializer
{
    public void Initialize(IServiceCollection services);
}