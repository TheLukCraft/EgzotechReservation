using Egzotech.Application.DTOs.Reservations;
using FluentValidation;
using Egzotech.Application.Helpers;

namespace Egzotech.Application.Validators;

public class CreateReservationDtoValidator : AbstractValidator<CreateReservationDto>
{
    public CreateReservationDtoValidator(TimeProvider timeProvider)
    {
        RuleFor(x => x.RobotId)
            .Must(id => Guid.TryParse(id, out _))
            .WithMessage("Invalid Robot ID format.");

        RuleFor(x => x.PatientEmail)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.StartTime)
            .Must(start => start >= timeProvider.GetUtcNow())
            .WithMessage("Reservation start time cannot be in the past.")
            .Must(TimeSlotHelper.HasValidMinutes)
            .WithMessage("Reservation must start exactly at :00 or :30 minutes.")
            .Must(TimeSlotHelper.HasZeroSeconds)
            .WithMessage("Reservation start time must have 00 seconds and milliseconds.");

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0)
            .Must(d => d % 30 == 0) 
            .WithMessage("Duration must be a multiple of 30 minutes (30, 60, 90, 120).")
            .LessThanOrEqualTo(120);
    }
}