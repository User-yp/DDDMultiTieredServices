using MediatR;
using MTS.Domain.EnentHandler;
using Newtonsoft.Json;

namespace MTS.WebApi.EnentHandler;

public class OrderDeletedHandler : INotificationHandler<OrderDeletedEvent>
{
    public Task Handle(OrderDeletedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"监听事件响应{JsonConvert.SerializeObject(notification.Order)}");
        return Task.CompletedTask;
    }
}
