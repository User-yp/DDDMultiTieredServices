using Microsoft.AspNetCore.Identity;

namespace IdentityService.Domain.Entity;

public class Role : IdentityRole<Guid>
{
    public Role()
    {
        this.Id = Guid.NewGuid();
    }
}
