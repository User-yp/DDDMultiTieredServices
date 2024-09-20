using Domain;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Domain.Entity;

public class User : IdentityUser<Guid>, IHasCreationTime, IHasDeletionTime, ISoftDelete
{
    public DateTime CreationTime { get; init; }

    public DateTime? DeletionTime { get; private set; }

    public bool IsDeleted { get; private set; }
    public User() : base()
    {
        Id = Guid.NewGuid();
        CreationTime = DateTime.Now;
    }
    public User(string userName) : base(userName)
    {
        Id = Guid.NewGuid();
        CreationTime = DateTime.Now;
    }
    public void SoftDelete()
    {
        this.IsDeleted = true;
        this.DeletionTime = DateTime.Now;
    }
}
