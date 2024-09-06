
using Domain;
using MTS.Domain.EnentHandler;

namespace MTS.Domain.Entity;

public record Order:  AggregateRootEntity, IAggregateRoot
{
    public string OrderName { get;private set; }
    public string ProductName { get;private set; }
    public string ProductDescription { get; private set; }

    public Order()
    {
        
    }
    public Order(string OrderName, string ProductName, string ProductDescription)
    {
        this.OrderName = OrderName;
        this.ProductName = ProductName;
        this.ProductDescription = ProductDescription;
    }
    public void ChangeName(string name)
    {
        this.OrderName = name;
    }
    public override void SoftDelete()
    {
        //base.SoftDelete();
        //AddDomainEvent(new OrderDeletedEvent(this));
    }
}