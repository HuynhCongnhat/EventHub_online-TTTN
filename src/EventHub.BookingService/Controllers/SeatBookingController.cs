using EventHub.BookingService.Application.DTOs;
using EventHub.BookingService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.BookingService.Controllers;

[ApiController]
[Route("api/seats")]
public class SeatBookingController : ControllerBase
{
    private readonly ISeatBookingService _seatBookingService;


    public SeatBookingController(ISeatBookingService seatBookingService)
        {
            _seatBookingService = seatBookingService;
        }

    [HttpGet("event/{eventId:guid}")]
    public async Task<IActionResult> GetSeatsByEventId(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        try
        {
            var seats = await _seatBookingService.GetSeatsByEventIdAsync(
                eventId,
                cancellationToken);

            return Ok(seats);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("hold")]
    public async Task<IActionResult> HoldSeat(
        [FromBody] HoldSeatRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _seatBookingService.HoldSeatAsync(
                request,
                cancellationToken);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{seatId:guid}/release")]
    public async Task<IActionResult> ReleaseSeat(
        Guid seatId,
        [FromQuery] Guid userId,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _seatBookingService.ReleaseSeatAsync(
                seatId,
                userId,
                cancellationToken);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
