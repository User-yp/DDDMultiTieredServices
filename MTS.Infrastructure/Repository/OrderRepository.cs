using MediatR;
using Microsoft.EntityFrameworkCore;
using MTS.Domain.EnentHandler;
using MTS.Domain.Entity;
using MTS.Domain.IService;
using MTS.IRepository;

namespace MTS.Infrastructure.Service;

public class OrderRepository:IOrderRepository
{
    private readonly IOrderMiddleResp orderMiddleResp;
    private readonly BaseDbContext dbContext;
    private readonly IMediator mediator;

    public OrderRepository(IOrderMiddleResp orderMiddleResp, BaseDbContext dbContext,IMediator mediator)
    {
        this.orderMiddleResp = orderMiddleResp;
        this.dbContext = dbContext;
        this.mediator = mediator;
    }
    public async Task<bool> CreateOrderAsync(Order order)
    {
        await orderMiddleResp.CreateAsync(order);
        var res= await dbContext.SaveChangesAsync();
        if(res<=0) return false;
        return true;
    }
    public async Task<Order?> GetByIdAsync(Guid id)
    {
        return await orderMiddleResp.GetAsync(id);
    }
    public async Task<List<Order>?> GetAllAsync()
    {
        return await orderMiddleResp.GetAllAsync();
    }

    public async Task<Order?> GetByNameAsync(string Name)
    {
        return await dbContext.Orders.FirstOrDefaultAsync(o => o.OrderName == Name);
    }

    public async Task<Order?> GetByProductDescriptionAsync(string Description)
    {
        return await dbContext.Orders.FirstOrDefaultAsync(O => O.ProductDescription == Description);
    }

    public async Task<Order?> GetByProductNameAsync(string ProductName)
    {
        return await dbContext.Orders.FirstOrDefaultAsync(o => o.ProductName == ProductName);
    }
    public async Task<bool> UpdateNameByIdAsync(Guid guid, string Name) 
    {
        var res = await orderMiddleResp.GetAsync(guid);
        if (res==null)
            return false;
        res.ChangeName(Name);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool>DeletedByIdAsync(Guid id)
    {
        var res =await GetByIdAsync(id);
        if (res==null) return false;
        res.SoftDelete();
        await mediator.Publish(new OrderDeletedEvent(res));
        return true;
    }
}
