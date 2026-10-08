using EventHub.BookingService.Application.DTOs;
using EventHub.BookingService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.BookingService.Controllers;

[ApiController]
[Route("api/tickets")]
public class TicketInventoryController : ControllerBase
{
    private readonly ITicketInventoryService _ticketInventoryService;

    public TicketInventoryController(
        ITicketInventoryService ticketInventoryService)
    {
        _ticketInventoryService = ticketInventoryService;
    }

    [HttpGet("event/{eventId:guid}")]
    [ProducesResponseType(typeof(List<TicketInventoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TicketInventoryDto>>> GetByEventId(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        var tickets = await _ticketInventoryService
            .GetByEventIdAsync(eventId, cancellationToken);

        return Ok(tickets);
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(TicketInventoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TicketInventoryDto>> Create(
    [FromBody] CreateTicketInventoryRequest request,
    CancellationToken cancellationToken)
    {
        try
        {
            var ticket = await _ticketInventoryService
                .CreateAsync(request, cancellationToken);

            return CreatedAtAction(
                nameof(GetByEventId),
                new { eventId = ticket.EventId },
                ticket);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost("hold")]
    [ProducesResponseType(typeof(HoldTicketResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<HoldTicketResponse>> Hold(
    [FromBody] HoldTicketRequest request,
    CancellationToken cancellationToken)
    {
        try
        {
            var result = await _ticketInventoryService
                .HoldAsync(request, cancellationToken);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
}