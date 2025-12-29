using Egzotech.Domain.Entities;

namespace Egzotech.Application.Interfaces;

public interface IReservationRepository
{
    Task<IEnumerable<Reservation>> GetForDateRangeAsync(DateTimeOffset start, DateTimeOffset end, CancellationToken cancellationToken);
    Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> IsSlotOccupiedAsync(Guid robotId, DateTimeOffset start, DateTimeOffset end, CancellationToken cancellationToken);
    Task AddAsync(Reservation reservation, CancellationToken cancellationToken);
}