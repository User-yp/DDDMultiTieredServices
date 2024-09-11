using Microsoft.AspNetCore.Builder;

namespace EventBus.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseEventBus(this IApplicationBuilder appBuilder)
    {
        //获得IEventBus一次，就会立即加载IEventBus，这样扫描所有的EventHandler，保证消息及时消费
        object? eventBus = appBuilder.ApplicationServices.GetService(typeof(IEventBus));
        return eventBus == null ? throw new ApplicationException("找不到IEventBus实例") : appBuilder;
    }
}
