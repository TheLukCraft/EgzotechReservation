using Egzotech.Application.DTOs.Reservations;

namespace Egzotech.Application.Interfaces;
public interface IReservationService
{
    Task<ReservationResponseDto> LockSlotAsync(CreateReservationDto dto, CancellationToken cancellationToken);
    Task ConfirmReservationAsync(Guid reservationId, CancellationToken cancellationToken);
}