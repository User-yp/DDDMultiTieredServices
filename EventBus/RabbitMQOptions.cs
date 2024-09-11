namespace EventBus;

public class RabbitMQOptions
{
    public string HostName { get; set; }
    public string ExchangeName { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
}
