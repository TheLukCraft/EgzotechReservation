using Egzotech.Application.Interfaces;
using Egzotech.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Egzotech.Infrastructure.BackgroundJobs;

public class ReservationCleanupWorker(
    IServiceScopeFactory scopeFactory,
    TimeProvider timeProvider,
    ILogger<ReservationCleanupWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Reservation Cleanup Worker started.");

        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await ProcessExpiredReservations(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while cleaning up reservations.");
            }
        }
    }

    private async Task ProcessExpiredReservations(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var dbContext = scope.ServiceProvider.GetRequiredService<Persistence.EgzotechDbContext>();

        var now = timeProvider.GetUtcNow();

        // Find expired reservations and mark them as Expired
        var rowsAffected = await dbContext.Reservations
            .Where(r => r.Status == ReservationStatus.Locked && r.ExpiresAt < now)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.Status, ReservationStatus.Expired), ct);

        if (rowsAffected > 0)
        {
            logger.LogInformation("Expired {Count} reservations.", rowsAffected);
        }
    }
}