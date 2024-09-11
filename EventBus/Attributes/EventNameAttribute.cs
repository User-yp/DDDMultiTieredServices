namespace EventBus.Attributes;


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class EventNameAttribute(string name) : Attribute
{
    public string Name { get; init; } = name;
}
