using Egzotech.Domain.Enums;

namespace Egzotech.Domain.Entities;
public class Reservation
{
    public Guid Id { get; private set; }
    public Guid RobotId { get; private set; }
    public Robot? Robot { get; private set; }
    public string PatientEmail { get; private set; } = string.Empty;

    public DateTimeOffset StartTime { get; private set; }
    public DateTimeOffset EndTime { get; private set; }
    public ReservationStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }

    public Reservation() { }

    public Reservation(Guid robotId, string PatientEmail, DateTimeOffset startTime,
        DateTimeOffset endTime, TimeProvider timeProvider)
    {
        if (endTime <= startTime)
            throw new ArgumentException("End time must be after start time.");
        
        Id = Guid.NewGuid();
        RobotId = robotId;
        this.PatientEmail = PatientEmail;
        StartTime = startTime;
        EndTime = endTime;
        Status = ReservationStatus.Locked;

        var actualDateTime = timeProvider.GetUtcNow();
        CreatedAt = actualDateTime;
        ExpiresAt = actualDateTime.AddMinutes(10);
    }

    public void Confirm()
    {
        if (Status != ReservationStatus.Locked)
            throw new InvalidOperationException($"Cannot confirm reservation in status {Status}");
            
        Status = ReservationStatus.Confirmed;
    }

    public void MarkAsExpired()
    {
        if (Status == ReservationStatus.Locked)
        {
            Status = ReservationStatus.Expired;
        }
    }

}