using Microsoft.EntityFrameworkCore;
using MTS.Domain.Entity;
using MTS.IRepository;

namespace MTS.Infrastructure.Repository;

public abstract class BaseRepository<T> : IBaseRepository<T> where T : BaseIndex
{
    private readonly DbSet<T> dbSet;

    protected BaseRepository(DbSet<T> dbSet)
    {
        this.dbSet = dbSet;
    }
    public async Task<bool> DeleteAsync(Guid id)
    {
        var res =await GetAsync(id);
        if (res == null)
            return false;
        res.Deleted();

        return true;
    }

    public async Task<T?> GetAsync(Guid id)
    {
         return await dbSet.FirstOrDefaultAsync(t => t.Id == id);
    }
}
