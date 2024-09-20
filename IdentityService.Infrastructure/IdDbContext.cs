using IdentityService.Domain.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Infrastructure;

namespace IdentityService.Infrastructure;

public class IdDbContext : IdentityDbContext<User, Role, Guid>
{
    public IdDbContext(DbContextOptions<IdDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        modelBuilder.EnableSoftDeletionGlobalFilter();
    }
}
