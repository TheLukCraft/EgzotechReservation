using Egzotech.Application.Interfaces;
using Egzotech.Infrastructure.Persistence;
using Egzotech.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Egzotech.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<EgzotechDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IRobotRepository, RobotRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }
}