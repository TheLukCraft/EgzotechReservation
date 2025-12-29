using Egzotech.Application.DTOs.Reservations;
using Egzotech.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace Egzotech.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ReservationsController(IReservationService reservationService, 
IValidator<CreateReservationDto> validator) : ControllerBase
{    
    /// <summary>
    /// Temporarily locks a time slot for 10 minutes.
    /// </summary>
    /// <remarks>
    /// Creates a reservation with Locked'' status. If the reservation is not confirmed 
    /// within 10 minutes, the system will automatically release the slot.
    /// </remarks>
    /// <param name="dto">Reservation details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created reservation details including ID.</returns>
    /// <response code="201">Slot successfully locked.</response>
    /// <response code="400">Invalid data provided.</response>
    /// <response code="404">Robot not found.</response>
    /// <response code="409">The slot is already taken.</response>
    [HttpPost("lock")]
    [ProducesResponseType(typeof(ReservationResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> LockSlot([FromBody] CreateReservationDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(dto, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => new { Field = e.PropertyName, Error = e.ErrorMessage }));
        }

        var result = await reservationService.LockSlotAsync(dto, cancellationToken);
        
        return CreatedAtAction(nameof(LockSlot), new { id = result.Id }, result);
    }

    /// <summary>
    /// Confirms (pays for) an existing reservation.
    /// </summary>
    /// <param name="id">The reservation ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Reservation confirmed successfully.</response>
    /// <response code="404">Reservation not found.</response>
    /// <response code="409">Reservation is expired or cannot be confirmed.</response>
    [HttpPost("{id:guid}/confirm")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)] 
    [ProducesResponseType(StatusCodes.Status409Conflict)] 
    public async Task<IActionResult> Confirm(Guid id, CancellationToken cancellationToken)
    {
        await reservationService.ConfirmReservationAsync(id, cancellationToken);
        return NoContent();
    }
}