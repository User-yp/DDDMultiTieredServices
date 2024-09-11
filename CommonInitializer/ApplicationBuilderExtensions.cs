using EventBus.Extensions;
using Microsoft.AspNetCore.Builder;

namespace CommonInitializer;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseDefault(this IApplicationBuilder app)
    {
        app.UseEventBus();
        app.UseCors();//启用Cors
        app.UseForwardedHeaders();
        //app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }
}
