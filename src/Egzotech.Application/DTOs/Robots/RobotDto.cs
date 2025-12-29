namespace Egzotech.Application.DTOs.Robots;

public record RobotDto(
    Guid Id,
    string Name,
    string Model
);