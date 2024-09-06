using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public static class DbContextOptionsBuilderFactory
{
    public static DbContextOptionsBuilder<TDbContext> Create<TDbContext>() where TDbContext : DbContext
    {
        var connStr = Environment.GetEnvironmentVariable("ConnStr");
        var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();
        optionsBuilder.UseSqlServer(connStr);
        return optionsBuilder;
    }
}
