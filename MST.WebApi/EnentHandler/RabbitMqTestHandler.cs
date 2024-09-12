using EventBus.Attributes;
using EventBus.EventHandler;

namespace MTS.WebApi.EnentHandler;

[EventName("RabbitMqController")]
public class RabbitMqTestHandler : IIntegrationEventHandler
{
    public Task Handle(string eventName, string eventData)
    {
        Console.WriteLine($"{eventName}-{eventData}");
        return Task.CompletedTask;
    }
}
