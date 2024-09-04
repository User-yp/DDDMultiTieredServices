using Domain;
using MTS.Domain.Entity;
using MTS.Domain.IService;

namespace MTS.Domain;

public class DomainService
{
    private readonly IOrderRepository orderRepository;
    private readonly IModelRepository modelRepository;

    public DomainService(IOrderRepository orderRepository, IModelRepository modelRepository)
    {
        this.orderRepository = orderRepository;
        this.modelRepository = modelRepository;
    }

    public async Task<(OperateResult ,Order?)> GetOrderByIdAsync(Guid Id)
    {
        var res = await orderRepository.GetByIdAsync(Id);
        if (res == null)
            return (OperateResult.Failed(new OperateError { Code="400",Description="NoOrder"}), null );
        return (OperateResult.Success, res);
    }
    public async Task<(OperateResult,List<Order>?)> GetAllOrderAsync()
    {
        var res = await orderRepository.GetAllAsync();
        if (res.Count==0)
            return (OperateResult.Failed(new OperateError { Code = "400", Description = "NoOrders" }), null);
        return (OperateResult.Success, res);
    }
    public async Task<(OperateResult, Order?)> GetOrderByNameAsync(string name)
    {
        var res = await orderRepository.GetByNameAsync(name);
        if (res == null)
            return (OperateResult.Failed(new OperateError { Code = "400", Description = "NoOrder" }), null);
        return (OperateResult.Success, res);
    }
    public async Task<(OperateResult, bool)> UpdateNameByIdAsync(Guid id,string name)
    {
        var res = await orderRepository.UpdateNameByIdAsync(id,name);
        if (res == false)
            return (OperateResult.Failed(new OperateError { Code = "400", Description = "NoOrder" }), false);
        return (OperateResult.Success, res);
    }
}
