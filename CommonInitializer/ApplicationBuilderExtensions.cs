using EventBus.Extensions;
using Microsoft.AspNetCore.Builder;

namespace CommonInitializer;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseDefault(this IApplicationBuilder app)
    {
        //启用事件通讯
        app.UseEventBus();

        //启用跨域
        app.UseCors();//启用Cors

        app.UseForwardedHeaders();

        //app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        return app;
    }
}
