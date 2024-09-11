namespace EventBus;

public interface IEventBus : IDisposable
{
    void Publish(string eventName, object? eventData);
    void Subscribe(string eventName, Type handlerType);
    void Unsubscribe(string eventName, Type handlerType);
}