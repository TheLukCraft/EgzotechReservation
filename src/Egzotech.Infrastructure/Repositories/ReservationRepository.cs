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
}