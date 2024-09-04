using Microsoft.EntityFrameworkCore;
using MTS.Domain.Entity;
using MTS.Domain.IService;
using MTS.IRepository;

namespace MTS.Infrastructure.Service;

public class ModelRepository:IModelRepository
{
    private readonly IModelMiddleResp modelMiddleResp;
    private readonly BaseDbContext dbContext;

    public ModelRepository(IModelMiddleResp modelMiddleResp, BaseDbContext dbContext)
    {
        this.modelMiddleResp = modelMiddleResp;
        this.dbContext = dbContext;
    }

    public async Task<List<Model>?> GetAllAsync()
    {
        return await dbContext.Models.ToListAsync();
    }

    public async Task<Model?> GetByDescriptionAsync(string Description)
    {
        return await dbContext.Models.FirstOrDefaultAsync(m => m.ModelDescription == Description);
    }

    public async Task<Model?> GetByNameAsync(string Name)
    {
        return await dbContext.Models.FirstOrDefaultAsync(m => m.ModelName == Name);
    }

    public async Task<Model?> GetByVersionAsync(string Version)
    {
        return await dbContext.Models.FirstOrDefaultAsync(m => m.ModelVersion == Version);
    }
}
