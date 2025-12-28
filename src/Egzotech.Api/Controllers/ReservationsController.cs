using Egzotech.Application.DTOs.Reservations;
using Egzotech.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Egzotech.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController(IReservationService reservationService) : ControllerBase
{
    // POST api/reservations/lock
    [HttpPost("lock")]
    public async Task<IActionResult> LockSlot([FromBody] CreateReservationDto dto, CancellationToken cancellationToken)
    {
        var result = await reservationService.LockSlotAsync(dto, cancellationToken);
        
        return CreatedAtAction(nameof(LockSlot), new { id = result.Id }, result);
    }

    // POST api/reservations/{id}/confirm
    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id, CancellationToken cancellationToken)
    {
        await reservationService.ConfirmReservationAsync(id, cancellationToken);
        return NoContent();
    }
}