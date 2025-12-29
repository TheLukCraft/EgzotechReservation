using System.Data;
using Egzotech.Application.DTOs.Reservations;
using Egzotech.Application.DTOs.Robots;
using Egzotech.Application.Interfaces;
using Egzotech.Domain.Entities;

namespace Egzotech.Application.Services;

public class ReservationService(
    IReservationRepository reservationRepository,
    IRobotRepository robotRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider
    ) : IReservationService
{
    public async Task<IEnumerable<RobotDto>> GetAllRobotsAsync(CancellationToken cancellationToken)
    {
        var robots = await robotRepository.GetAllActiveAsync(cancellationToken);

        return robots.Select(r => new RobotDto(
            r.Id, 
            r.Name, 
            r.Model
            ));
    }
    public async Task<IEnumerable<RobotScheduleDto>> GetAllRobotsScheduleAsync(DateOnly date, CancellationToken cancellationToken)
    {
        var robots = await robotRepository.GetAllActiveAsync(cancellationToken);

        var startOfDay = new DateTimeOffset(date.ToDateTime(new TimeOnly(8, 0)), TimeSpan.Zero);
        var endOfDay = new DateTimeOffset(date.ToDateTime(new TimeOnly(18, 0)), TimeSpan.Zero);

        var allReservations = await reservationRepository.GetForDateRangeAsync(startOfDay, endOfDay, cancellationToken);

        var result = new List<RobotScheduleDto>();

        foreach (var robot in robots)
        {
            var robotReservations = allReservations.Where(r => r.RobotId == robot.Id).ToList();
            var slots = GenerateSlotsForRobot(robotReservations, startOfDay, endOfDay);

            result.Add(new RobotScheduleDto(
                robot.Id,
                robot.Name,
                robot.Model,
                date,
                slots
            ));
        }

        return result;
    }

    public async Task ConfirmReservationAsync(Guid reservationId, CancellationToken cancellationToken)
    {
        var reservation = await reservationRepository.GetByIdAsync(reservationId, cancellationToken);

        if (reservation is null)
        {
            throw new KeyNotFoundException($"Reservation {reservationId} not found.");
        }

        reservation.Confirm();

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<ReservationResponseDto> LockSlotAsync(CreateReservationDto dto, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(dto.RobotId, out var robotGuid))
            throw new ArgumentException($"Invalid Robot ID format: {dto.RobotId}");

        var robotExists = await robotRepository.ExistsAsync(robotGuid, cancellationToken);
        if (!robotExists)
            throw new KeyNotFoundException($"Robot with ID {robotGuid} not found.");

        var endTime = dto.StartTime.AddMinutes(dto.DurationMinutes);

        using var transaction = await unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable);

        try
        {
            var isSlotOccupied = await reservationRepository.IsSlotOccupiedAsync(robotGuid, dto.StartTime, endTime, cancellationToken);

            if (isSlotOccupied)
            {
                throw new InvalidOperationException("This slot is already reserved.");
            }

            var reservation = new Reservation(
                robotGuid,
                dto.PatientEmail,
                dto.StartTime,
                endTime,
                timeProvider
            );

            await reservationRepository.AddAsync(reservation, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            transaction.Commit();

            return new ReservationResponseDto(
                reservation.Id,
                dto.RobotId,
                reservation.Status.ToString(),
                reservation.StartTime,
                reservation.EndTime,
                reservation.ExpiresAt
            );
        }
        catch
        {
            throw;
        }
    }

    private IEnumerable<TimeSlotDto> GenerateSlotsForRobot(List<Reservation> reservations, DateTimeOffset start, DateTimeOffset end)
    {
        var slotDuration = TimeSpan.FromMinutes(30);
        var slots = new List<TimeSlotDto>();
        var current = start;

        while (current < end)
        {
            var next = current.Add(slotDuration);

            // Check if this slot overlaps with any reservation
            var overlappingReservation = reservations.FirstOrDefault(r =>
                r.StartTime < next && r.EndTime > current && r.Status != Domain.Enums.ReservationStatus.Expired);

            string status = overlappingReservation != null
                ? overlappingReservation.Status.ToString()
                : "Available";

            slots.Add(new TimeSlotDto(current, next, status));
            current = next;
        }
        return slots;
    }
}