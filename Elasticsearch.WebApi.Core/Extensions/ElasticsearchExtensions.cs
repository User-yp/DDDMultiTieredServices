using Commons.Extensions;
using Commons.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace Elasticsearch.WebApi.Core.Extensions;

public static class ElasticsearchExtensions
{
    //get option from configuration
    public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration )
    {
        var elasticOptions= configuration.GetSection(nameof(ElasticOptions)).GetOptions<ElasticOptions>();

        var defaultIndex = elasticOptions.DefaultIndex;
        var basicAuthUser = elasticOptions.UserName;
        var basicAuthPassword = elasticOptions.Password;

        var settings = new ConnectionSettings(elasticOptions.Uri);

        if (!string.IsNullOrEmpty(defaultIndex))
            settings = settings.DefaultIndex(defaultIndex);

        if (!string.IsNullOrEmpty(basicAuthUser) && !string.IsNullOrEmpty(basicAuthPassword))
            settings = settings.BasicAuthentication(basicAuthUser, basicAuthPassword);

        settings.EnableApiVersioningHeader();

        services.AddSingleton<IElasticClient>(new ElasticClient(settings));
    }

    //get configuration from ESstr
    public static void AddElasticsearch(this IServiceCollection services, string EsStr)
    {
        services.AddSingleton<IElasticClient>(sp =>
        {
            return new ElasticClient(new ConnectionSettings(new Uri(EsStr)));
        });
    }
}
