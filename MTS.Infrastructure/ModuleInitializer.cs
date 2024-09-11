using Commons.Initializer;
using Microsoft.Extensions.DependencyInjection;
using MTS.Domain.IMiddleResp;
using MTS.Domain;
using MTS.Domain.IRepository;
using MTS.Infrastructure.MiddleResp;
using MTS.Infrastructure.Repository;

namespace MTS.Infrastructure;

public class ModuleInitializer : IModuleInitializer
{
    public void Initialize(IServiceCollection services)
    {
        services.AddScoped<DomainService>();

        services.AddScoped<IModelMiddleResp, ModelMiddleResp>();
        services.AddScoped<IOrderMiddleResp, OrderMiddleResp>();

        services.AddScoped<IModelRepository, ModelRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
    }
}
