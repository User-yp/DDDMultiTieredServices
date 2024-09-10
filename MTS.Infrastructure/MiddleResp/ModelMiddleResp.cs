using MTS.Domain.Entity;
using MTS.Domain.IMiddleResp;

namespace MTS.Infrastructure.MiddleResp;

public class ModelMiddleResp : BaseRepository<Model>, IModelMiddleResp
{
    public ModelMiddleResp(BaseDbContext dbContext) : base(dbContext.Models)
    {
    }
}
