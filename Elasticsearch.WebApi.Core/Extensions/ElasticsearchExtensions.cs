using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elasticsearch.WebApi.Core.Extensions;

public static class ElasticsearchExtensions
{
    public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
    {
        var defaultIndex = configuration["ElasticsearchSettings:defaultIndex"];
        var basicAuthUser = configuration["ElasticsearchSettings:username"];
        var basicAuthPassword = configuration["ElasticsearchSettings:password"];

        var settings = new ConnectionSettings(new Uri(configuration["ElasticsearchSettings:uri"]));

        if (!string.IsNullOrEmpty(defaultIndex))
            settings = settings.DefaultIndex(defaultIndex);

        if (!string.IsNullOrEmpty(basicAuthUser) && !string.IsNullOrEmpty(basicAuthPassword))
            settings = settings.BasicAuthentication(basicAuthUser, basicAuthPassword);

        settings.EnableApiVersioningHeader();

        services.AddSingleton<IElasticClient>(new ElasticClient(settings));
    }
    public static void AddElasticsearch(this IServiceCollection services, string EsStr)
    {
        /*var conn=  new ConnectionSettings(new Uri(EsStr));
        var setting=new ElasticClient(conn);*/

        services.AddSingleton<IElasticClient>(sp =>
        {
            return new ElasticClient(new ConnectionSettings(new Uri(EsStr)));
        });
    }
}
