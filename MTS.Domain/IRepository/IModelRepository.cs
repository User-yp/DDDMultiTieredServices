using MTS.Domain.Entity;

namespace MTS.Domain.IService;

public interface IModelRepository
{
    Task<List<Model>?> GetAllAsync();
    Task<Model?> GetByNameAsync(string Name);
    Task<Model?> GetByVersionAsync(string Version);
    Task<Model?> GetByDescriptionAsync(string Description);
}
