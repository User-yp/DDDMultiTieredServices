using MTS.Domain;
using MTS.Domain.IService;
using MTS.Infrastructure.Repository;
using MTS.Infrastructure.Service;
using MTS.IRepository;

namespace MTS.WebApi;

public static class ServiceExtensions
{
    public static void AddBaseServies(this IServiceCollection services)
    {
        services.AddScoped<DomainService>();

        services.AddScoped<IModelMiddleResp, ModelMiddleResp>();
        services.AddScoped<IOrderMiddleResp, OrderMiddleResp>();

        services.AddScoped<IModelRepository, ModelRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
    }
}
