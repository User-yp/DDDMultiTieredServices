using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore.Design;

namespace MTS.Infrastructure;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BaseDbContext>
{
    private readonly IMediator mediator;

    public DesignTimeDbContextFactory(IMediator mediator)
    {
        this.mediator = mediator;
    }
    public BaseDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = DbContextOptionsBuilderFactory.Create<BaseDbContext>();
        return new BaseDbContext(optionsBuilder.Options, mediator);
    }
}
