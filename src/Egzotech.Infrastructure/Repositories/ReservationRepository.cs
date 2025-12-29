using Egzotech.Application.Interfaces;
using Egzotech.Domain.Entities;
using Egzotech.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Egzotech.Infrastructure.Persistence.Repositories;

internal class ReservationRepository(EgzotechDbContext dbContext) : IReservationRepository
{
    public async Task AddAsync(Reservation reservation, CancellationToken cancellationToken)
    {
        await dbContext.Reservations.AddAsync(reservation, cancellationToken);
    }

    public async Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Reservations
            .Include(reservation => reservation.Robot)
            .FirstOrDefaultAsync(reservation => reservation.Id == id, cancellationToken);
    }

    public async Task<bool> IsSlotOccupiedAsync(Guid robotId, DateTimeOffset startTime, DateTimeOffset endTime, CancellationToken cancellationToken)
    {
        return await dbContext.Reservations
            .AnyAsync(reservation =>
                reservation.RobotId == robotId &&
                reservation.Status != ReservationStatus.Expired &&
                reservation.StartTime < endTime &&
                reservation.EndTime > startTime,
                cancellationToken);
    }

    public async Task<IEnumerable<Reservation>> GetForRobotAndDateAsync(Guid robotId, DateOnly date, CancellationToken cancellationToken)
    {
        // Convert DateOnly to DateTimeOffset range for the entire day
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

    public async Task<IEnumerable<Reservation>> GetForDateRangeAsync(DateTimeOffset start, DateTimeOffset end, CancellationToken cancellationToken)
    {
        return await dbContext.Reservations
            .Where(r => r.Status != ReservationStatus.Expired &&
                        r.StartTime < end &&
                        r.EndTime > start) // Pobieramy wszystko co zahacza o ten dzień
            .ToListAsync(cancellationToken);
    }
}