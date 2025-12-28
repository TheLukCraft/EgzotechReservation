using Egzotech.Application.DTOs.Reservations;
using Egzotech.Domain.Entities;

namespace Egzotech.Application.Interfaces;

public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> IsSlotOccupiedAsync(Guid robotId, DateTimeOffset start, DateTimeOffset end, CancellationToken cancellationToken);
    Task<ReservationResponseDto> LockSlotAsync(CreateReservationDto dto, CancellationToken cancellationToken);
    Task AddAsync(Reservation reservation, CancellationToken cancellationToken);
}