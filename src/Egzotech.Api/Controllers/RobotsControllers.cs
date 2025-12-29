using Egzotech.Application.DTOs.Reservations;
using Egzotech.Application.DTOs.Robots;
using Egzotech.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Egzotech.Api.Controllers;

[ApiController]
[Route("api/robots")]
[Produces("application/json")]
public class RobotsController(IReservationService reservationService) : ControllerBase
{
    /// <summary>
    /// Returns the schedule (time slots) for all rehabilitation robots.
    /// </summary>
    /// <param name="date">Date to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of robots with their schedules.</returns>
    [HttpGet("slots")]
    [ProducesResponseType(typeof(IEnumerable<RobotScheduleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSlots([FromQuery] DateOnly date, CancellationToken cancellationToken)
    {
        var result = await reservationService.GetAllRobotsScheduleAsync(date, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Returns a simple list of all available robots.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of robots (ID, Name, Model).</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RobotDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await reservationService.GetAllRobotsAsync(cancellationToken);
        return Ok(result);
    }
}