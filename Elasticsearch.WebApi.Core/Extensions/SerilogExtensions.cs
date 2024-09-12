using Elastic.Apm.SerilogEnricher;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Filters;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Elasticsearch;

namespace Elasticsearch.WebApi.Core.Extensions;

public static class SerilogExtensions
{
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder, IConfiguration configuration, string applicationName)
    {
        var str = new Uri(configuration["ElasticsearchSettings:uri"]);
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.WithProperty("ApplicationName", $"{applicationName} - {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}")
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()
        .Enrich.WithElasticApmCorrelationInfo()
            .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.StaticFiles"))
            .Filter.ByExcluding(z => z.MessageTemplate.Text.Contains("specific error"))
            .WriteTo.Async(writeTo => writeTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration["ElasticsearchSettings:uri"]))
            {
                TypeName = null,
                AutoRegisterTemplate = true,
                IndexFormat = "eslogs",
                OverwriteTemplate = true,
                FailureCallback = e => Console.WriteLine("Unable to submit event " + e.MessageTemplate),
                BatchAction = ElasticOpType.Create,
                BatchPostingLimit = 5,
                ModifyConnectionSettings = x => x.BasicAuthentication(configuration["ElasticsearchSettings:username"], configuration["ElasticsearchSettings:password"])
            }))
            .WriteTo.Async(writeTo => writeTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"))
            .WriteTo.Debug()
            .WriteTo.File(Path.Combine(AppContext.BaseDirectory, "Logs", "log-.txt"), rollingInterval: RollingInterval.Day)
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Host.UseSerilog(Log.Logger, true);
        return builder;
    }

    public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
    {
        diagnosticContext.Set("UserName", httpContext?.User?.Identity?.Name);
        diagnosticContext.Set("ClientIP", httpContext?.Connection?.RemoteIpAddress?.ToString());
        diagnosticContext.Set("UserAgent", httpContext?.Request?.Headers["User-Agent"].FirstOrDefault());
        diagnosticContext.Set("Resource", httpContext.GetMetricsCurrentResourceName());
    }

    public static string GetMetricsCurrentResourceName(this HttpContext httpContext)
    {
        if (httpContext == null)
            throw new ArgumentNullException(nameof(httpContext));

        var endpoint = httpContext.Features.Get<IEndpointFeature>()?.Endpoint;

        return endpoint?.Metadata.GetMetadata<EndpointNameMetadata>()?.EndpointName;
    }
}
