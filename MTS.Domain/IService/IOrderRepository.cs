using MTS.Domain.Entity;

namespace MTS.Domain.IService;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id);
    Task<List<Order>?> GetAllAsync();
    Task<Order?> GetByNameAsync(string Name);
    Task<Order?> GetByProductNameAsync(string ProductName);
    Task<Order?> GetByProductDescriptionAsync(string Description);
    Task<bool> UpdateNameByIdAsync(Guid guid, string Name);
}
