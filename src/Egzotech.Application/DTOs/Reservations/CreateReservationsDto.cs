namespace Egzotech.Application.DTOs.Reservations;

public record CreateReservationDto(
    string RobotId,
    string PatientEmail,
    DateTimeOffset StartTime,
    int DurationMinutes
);

public record ReservationResponseDto(
    Guid Id,
    string RobotName,
    string Status,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    DateTimeOffset ExpiresAt
);