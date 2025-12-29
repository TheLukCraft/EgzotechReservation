using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Egzotech.Infrastructure.Persistence;

public class EgzotechDbContextFactory : IDesignTimeDbContextFactory<EgzotechDbContext>
{
    public EgzotechDbContext CreateDbContext(string[] args)
    {
        var connectionString = ConnectionStringHelper.Build();

        var optionsBuilder = new DbContextOptionsBuilder<EgzotechDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new EgzotechDbContext(optionsBuilder.Options);
    }
}