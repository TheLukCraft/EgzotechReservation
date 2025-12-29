using Egzotech.Domain.Entities;
using Egzotech.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Egzotech.Infrastructure.Seed;

public class DataSeeder(EgzotechDbContext context)
{
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        await context.Database.MigrateAsync(cancellationToken);

        if (!await context.Robots.AnyAsync(cancellationToken))
        {
            var robots = new List<Robot>
            {
                new Robot("Luna EMG", "EMG-2025"),
                new Robot("Sidra Leg", "LEG-2025"),
                new Robot("Meissa OT", "OT-2025"),
                new Robot("Stella BIO", "BIO-2025"),
            };

            await context.Robots.AddRangeAsync(robots, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}