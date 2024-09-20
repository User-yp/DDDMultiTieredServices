using Elasticsearch.Domain.Entity;
using Elasticsearch.Domain.IMiddleResp;
using Elasticsearch.Domain.IRepository;

namespace Elasticsearch.Infrastructure.Repository;

public class OrderRepository(IOrderMiddleResp orderMiddleResp):IOrderRepository
{
    private readonly IOrderMiddleResp orderMiddleResp;

    public async Task<ICollection<Order>> GetAllAsync()
    {
        var result = await orderMiddleResp.GetAllAsync();

        return result.ToList();
    }

    public async Task<Order> GetOrderByIdAsync(Guid guid)
    {
        var result = await orderMiddleResp.GetAsync(guid);

        if (result != null)
            return result;
        return null;
    }
}
