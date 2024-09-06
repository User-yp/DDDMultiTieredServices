using MTS.Domain.Entity;
using MTS.IRepository;

namespace MTS.Infrastructure.Repository;

public class ModelMiddleResp : BaseRepository<Model>, IModelMiddleResp
{
    public ModelMiddleResp(BaseDbContext dbContext) : base(dbContext.Models)
    {
    }
}
