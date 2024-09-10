using MTS.Domain.Entity;
using MTS.Domain.IMiddleResp;

namespace MTS.Infrastructure.MiddleResp;

public class OrderMiddleResp : BaseRepository<Order>, IOrderMiddleResp
{
    public OrderMiddleResp(BaseDbContext dbContext) : base(dbContext.Orders)
    {
    }
}
