using Egzotech.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Egzotech.Infrastructure.Persistence;

public class EgzotechDbContext(DbContextOptions<EgzotechDbContext> options) : DbContext(options)
{
    public DbSet<Robot> Robots { get; set; }
    public DbSet<Reservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EgzotechDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}