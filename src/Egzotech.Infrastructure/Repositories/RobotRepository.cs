using Egzotech.Application.Interfaces;
using Egzotech.Domain.Entities;
using Egzotech.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Egzotech.Infrastructure.Persistence.Repositories;

internal class RobotRepository(EgzotechDbContext dbContext) : IRobotRepository
{
    public async Task<IEnumerable<Reservation>> GetForRobotAndDateAsync(Guid robotId, DateOnly date, CancellationToken cancellationToken)
    {
        var startOfDay = new DateTimeOffset(date.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        var endOfDay = new DateTimeOffset(date.ToDateTime(TimeOnly.MaxValue), TimeSpan.Zero);

        return await dbContext.Reservations
            .Where(reservation => reservation.RobotId == robotId &&
                        reservation.Status != ReservationStatus.Expired &&
                        reservation.StartTime >= startOfDay &&
                        reservation.StartTime <= endOfDay)
            .OrderBy(reservation => reservation.StartTime)
            .ToListAsync(cancellationToken);
    }
    public async Task<Robot?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Robots
            .FirstOrDefaultAsync(robot => robot.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Robot>> GetAllActiveAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Robots
            .Where(r => r.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Robots
            .AnyAsync(robot => robot.Id == id, cancellationToken);
    }

}