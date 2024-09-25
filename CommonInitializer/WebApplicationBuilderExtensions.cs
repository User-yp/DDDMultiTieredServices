using Commons;
using Commons.Initializer;
using EventBus;
using Jwt;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using Infrastructure;
using EventBus.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using ASPNETCore;
using Serilog;
using CacheServices;
using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo;
using Commons.Extensions;
using Jwt.Extensions;
using Commons.Options;
using CacheServices.RedisService;
using Commons.OperateHelper;
using Validation.Extensions;
using Microsoft.Data.SqlClient;

namespace CommonInitializer;

public static class WebApplicationBuilderExtensions
{
    public static void ConfigureDbConfiguration(this WebApplicationBuilder builder)
    {
        builder.Host.ConfigureAppConfiguration((hostCtx, configBuilder) =>
        {
            //从Apollo获取Configuration
            /*configBuilder.AddApollo(hostCtx.Configuration.GetSection("apollo"))
            .AddNamespace("User.CommonConfiguration", ConfigFileFormat.Json).AddDefault();*/

            //从数据库配置服务器获取Configuration
            string connStr = builder.Configuration.GetValue<string>("ConnStr");
            configBuilder.AddDbConfiguration(() => new SqlConnection(connStr), reloadOnChange: true, reloadInterval: TimeSpan.FromSeconds(5));
        });
    }

    public static void ConfigureExtraServices(this WebApplicationBuilder builder, InitializerOptions initOptions)
    {
        IServiceCollection services = builder.Services;
        IConfiguration configuration = builder.Configuration;

        //获取所有程序集
        var assemblies = AssemblyHelper.GetAllReferencedAssemblies();

        //注册每个项目ModuleInitializer中自定义服务
        services.RunModuleInitializers(assemblies);

        //注册所有DbContex
        services.AddAllDbContexts(ctx =>
        {
            //连接字符串如果放到appsettings.json中，会有泄密的风险
            //如果放到UserSecrets中，每个项目都要配置，很麻烦
            //因此这里推荐放到环境变量中。
            string connStr = configuration.GetValue<string>(nameof(ConnStr));
            ctx.UseSqlServer(connStr);
        }, assemblies);

        //开始:Authentication,Authorization
        //只要需要校验Authentication报文头的地方（非IdentityService.WebAPI项目）也需要启用这些
        //IdentityService项目还需要启用AddIdentityCore
        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication();
        //JWTOptions jwtOpt = configuration.GetSection("JWT").Get<JWTOptions>();
        var jwtOpt =configuration.GetSection(nameof(JWTOptions)).GetOptions<JWTOptions>();
        builder.Services.AddJWTAuthentication(jwtOpt);
        //启用Swagger中的【Authorize】按钮。这样就不用每个项目的AddSwaggerGen中单独配置了
        builder.Services.Configure<SwaggerGenOptions>(c =>
        {
            c.AddAuthenticationHeader();
        });//结束:Authentication,Authorization
        

        //注册MediatR领域通讯
        services.AddMediatR(assemblies);

        //注册自定义过滤器事务单元
        services.Configure<MvcOptions>(options =>
        {
            options.Filters.Add<UnitOfWorkFilter>();
        });

        //设置时间格式。而非“2008-08-08T08:08:08”这样的格式
        services.Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter("yyyy-MM-dd HH:mm:ss"));
        });

        //注册配置跨域
        services.AddCors(options =>
        {
            //更好的在Program.cs中用绑定方式读取配置的方法：https://github.com/dotnet/aspnetcore/issues/21491
            //不过比较麻烦。
            //var corsOpt = configuration.GetSection("Cors").Get<CorsSettings>();
            var corsOpt = configuration.GetSection(nameof(CorsOptions)).GetOptions<CorsOptions>();
            string[] urls = corsOpt.Origins;
            options.AddDefaultPolicy(builder => builder.WithOrigins(urls)
                    .AllowAnyMethod().AllowAnyHeader().AllowCredentials());
        });

        //注册logging
        services.AddLogging(builder =>
        {
            Log.Logger = new LoggerConfiguration()
               // .MinimumLevel.Information().Enrich.FromLogContext()
               .WriteTo.Console()
               .WriteTo.File(initOptions.LogFilePath)
               .CreateLogger();
            builder.AddSerilog();
        });

        //注册FluentValidation手动校验模式
        services.AddFluentValidation(assemblies);
        //自动校验builder.Services.AddFluentValidationAutoValidation();
        
        //注册EventBus
        //services.Configure<RabbitMQOptions>(configuration.GetSection("RabbitMQ"));
        var rabbitMQOptions = configuration.GetSection(nameof(RabbitMQOptions)).GetOptions<RabbitMQOptions>();
        services.AddEventBus(initOptions.EventBusQueueName, rabbitMQOptions, assemblies);

        //注册Redis
        services.AddRedisService(redisOption =>
        {
            var res= builder.Configuration.GetSection(nameof(RedisOptions)).GetOptions<RedisOptions>();
            redisOption.ConnectionString = res.ConnectionString;
            redisOption.DbNumber=res.DbNumber;

        });

        //启用内存缓存
        services.AddMemoryCacheService();
    }
}
