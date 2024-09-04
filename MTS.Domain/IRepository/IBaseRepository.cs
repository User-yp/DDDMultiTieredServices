namespace MTS.IRepository;

public interface IBaseRepository<T>
{
    Task<T?> GetAsync(Guid id);
    Task<bool> DeleteAsync(Guid id);
}
