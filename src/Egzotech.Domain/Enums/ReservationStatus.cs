namespace Egzotech.Domain.Enums;

public enum ReservationStatus
{
    Locked = 1,    // Temporarily reserved
    Confirmed = 2, // Confirmed reservation
    Expired = 3    // Reservation expired without confirmation
}