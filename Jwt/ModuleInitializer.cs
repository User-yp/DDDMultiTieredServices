using Commons.Initializer;
using Microsoft.Extensions.DependencyInjection;

namespace Jwt;

public class ModuleInitializer : IModuleInitializer
{
    public void Initialize(IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
    }
}
