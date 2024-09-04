using Microsoft.EntityFrameworkCore;
using MTS.Domain.Entity;
using MTS.Domain.IService;
using MTS.IRepository;

namespace MTS.Infrastructure.Service;

public class OrderRepository:IOrderRepository
{
    private readonly IOrderMiddleResp orderMiddleResp;
    private readonly BaseDbContext dbContext;

    public OrderRepository(IOrderMiddleResp orderMiddleResp, BaseDbContext dbContext)
    {
        this.orderMiddleResp = orderMiddleResp;
        this.dbContext = dbContext;
    }
    public async Task<Order?> GetByIdAsync(Guid id)
    {
        return await orderMiddleResp.GetAsync(id);
    }
    public async Task<List<Order>?> GetAllAsync()
    {
        return await dbContext.Orders.ToListAsync();
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

}
