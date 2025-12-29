namespace Egzotech.Application.DTOs.Reservations;

public record RobotScheduleDto(
    Guid RobotId,
    string RobotName,
    string RobotModel,
    DateOnly Date,
    IEnumerable<TimeSlotDto> Slots
);

public record TimeSlotDto(
    DateTimeOffset Start,
    DateTimeOffset End,
    string Status
);