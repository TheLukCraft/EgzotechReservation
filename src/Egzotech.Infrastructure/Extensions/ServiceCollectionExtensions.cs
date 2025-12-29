using Egzotech.Application.Interfaces;
using Egzotech.Infrastructure.Persistence;
using Egzotech.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Egzotech.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Egzotech.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        services.AddDbContext<EgzotechDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddSingleton(TimeProvider.System);

        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IRobotRepository, RobotRepository>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddHostedService<BackgroundJobs.ReservationCleanupWorker>();
        services.AddScoped<Seed.DataSeeder>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}