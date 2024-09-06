using MTS.Domain.Entity;
using MTS.IRepository;

namespace MTS.Infrastructure.Repository;

public class OrderMiddleResp : BaseRepository<Order>, IOrderMiddleResp
{
    public OrderMiddleResp(BaseDbContext dbContext) : base(dbContext.Orders)
    {
    }
}
