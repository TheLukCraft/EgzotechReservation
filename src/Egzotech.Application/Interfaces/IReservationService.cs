using Egzotech.Application.DTOs.Reservations;
using Egzotech.Application.DTOs.Robots;

namespace Egzotech.Application.Interfaces;
public interface IReservationService
{
    Task<IEnumerable<RobotDto>> GetAllRobotsAsync(CancellationToken cancellationToken);
    Task<IEnumerable<RobotScheduleDto>> GetAllRobotsScheduleAsync(DateOnly date, CancellationToken cancellationToken);
    Task<ReservationResponseDto> LockSlotAsync(CreateReservationDto dto, CancellationToken cancellationToken);
    Task ConfirmReservationAsync(Guid reservationId, CancellationToken cancellationToken);
}