using Infrastructure;
using Microsoft.EntityFrameworkCore;
using MTS.Domain.Entity;

namespace MTS.Infrastructure;

public class BaseDbContext: IBaseDbContext
{
    

    public DbSet<Order> Orders { get; set; }
    public DbSet<Model> Models { get; set; }
    public BaseDbContext(DbContextOptions options) : base(options)
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("ConnStr"));
        optionsBuilder.LogTo(str =>
        {
            if (!str.Contains("Executing"))
                return;
            Console.WriteLine(str);
        });
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        modelBuilder.EnableSoftDeletionGlobalFilter();
    }
}
