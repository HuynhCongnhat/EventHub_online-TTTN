using EventHub.EventService.Application.Commands;
using EventHub.EventService.Application.DTOs;
using EventHub.EventService.Application.Handlers;
using EventHub.EventService.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.EventService.Presentation.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly GetAllEventsHandler _getAllEventsHandler;
    private readonly GetEventByIdHandler _getEventByIdHandler;
    private readonly CreateEventHandler _createEventHandler;
    private readonly UpdateEventHandler _updateEventHandler;
    private readonly DeleteEventHandler _deleteEventHandler;

    public EventsController(
        GetAllEventsHandler getAllEventsHandler,
        GetEventByIdHandler getEventByIdHandler,
        CreateEventHandler createEventHandler,
        UpdateEventHandler updateEventHandler,
        DeleteEventHandler deleteEventHandler)
    {
        _getAllEventsHandler = getAllEventsHandler;
        _getEventByIdHandler = getEventByIdHandler;
        _createEventHandler = createEventHandler;
        _updateEventHandler = updateEventHandler;
        _deleteEventHandler = deleteEventHandler;
    }

    // GET: api/events
    [HttpGet]
    public async Task<ActionResult<List<EventDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var query = new GetAllEventsQuery();

        var result = await _getAllEventsHandler.HandleAsync(
            query,
            cancellationToken);

        return Ok(result);
    }

    // GET: api/events/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EventDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetEventByIdQuery(id);

        var result = await _getEventByIdHandler.HandleAsync(
            query,
            cancellationToken);

        if (result == null)
        {
            return NotFound(new
            {
                message = "Không tìm thấy sự kiện."
            });
        }

        return Ok(result);
    }

    // POST: api/events
    [HttpPost]
    public async Task<ActionResult<EventDto>> Create(
        [FromBody] CreateEventRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateEventCommand
        {
            Request = request
        };

        var result = await _createEventHandler.HandleAsync(
            command,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    // PUT: api/events/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EventDto>> Update(
        Guid id,
        [FromBody] UpdateEventRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateEventCommand
        {
            Id = id,
            Request = request
        };

        var result = await _updateEventHandler.HandleAsync(
            command,
            cancellationToken);

        if (result == null)
        {
            return NotFound(new
            {
                message = "Không tìm thấy sự kiện."
            });
        }

        return Ok(result);
    }

    // DELETE: api/events/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteEventCommand(id);

        var deleted = await _deleteEventHandler.HandleAsync(
            command,
            cancellationToken);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Không tìm thấy sự kiện."
            });
        }

        return NoContent();
    }
}