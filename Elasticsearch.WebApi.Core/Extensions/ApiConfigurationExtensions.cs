using Elasticsearch.Domain.IMiddleResp;
using Elasticsearch.WebApi.Core.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Elasticsearch.Infrastructure.MiddleResp;
using Elasticsearch.Domain.IRepository;
using Elasticsearch.Infrastructure.Repository;
using Serilog;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Elasticsearch.WebApi.Core.Extensions;

public static class ApiConfigurationExtensions
{
    public static void AddApiConfiguration(this IServiceCollection services)
    {
        services.AddRouting(options => options.LowercaseUrls = false);

        services.AddTransient<IActorMiddleResp, ActorMiddleResp>();
        services.AddTransient<IActorRepository, ActorRepository>();
        services.AddTransient<IOrderMiddleResp, OrderMiddleResp>();
        services.AddTransient<IOrderRepository, OrderRepository>();

        services.AddControllers();
    }

    public static void UseApiConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSerilogRequestLogging(opts => opts.EnrichDiagnosticContext = SerilogExtensions.EnrichFromRequest);

        app.UseMiddleware<RequestSerilLogMiddleware>();
        app.UseMiddleware<ErrorHandlingMiddleware>();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();
    }
}
