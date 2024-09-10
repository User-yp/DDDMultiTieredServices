namespace MTS.Domain.IMiddleResp;

public interface IBaseRepository<T>
{
    Task CreateAsync(T entity);
    Task<T?> GetAsync(Guid id);
    Task<List<T>?> GetAllAsync();
    Task<bool> DeleteAsync(Guid id);
}
