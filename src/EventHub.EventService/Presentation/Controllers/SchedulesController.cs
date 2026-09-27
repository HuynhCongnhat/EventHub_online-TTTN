using EventHub.EventService.Application.Commands.Schedules;
using EventHub.EventService.Application.DTOs;
using EventHub.EventService.Application.Queries.Schedules;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.EventService.Presentation.Controllers;

[ApiController]
[Route("api/schedules")]
public class SchedulesController : ControllerBase
{
    private readonly GetAllSchedulesHandler _getAllHandler;
    private readonly GetScheduleByIdHandler _getByIdHandler;
    private readonly CreateScheduleHandler _createHandler;
    private readonly UpdateScheduleHandler _updateHandler;
    private readonly DeleteScheduleHandler _deleteHandler;

    public SchedulesController(
        GetAllSchedulesHandler getAllHandler,
        GetScheduleByIdHandler getByIdHandler,
        CreateScheduleHandler createHandler,
        UpdateScheduleHandler updateHandler,
        DeleteScheduleHandler deleteHandler)
    {
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
    }

    // GET: api/schedules
    [HttpGet]
    public async Task<ActionResult<List<ScheduleDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _getAllHandler.HandleAsync(
            new GetAllSchedulesQuery(),
            cancellationToken);

        return Ok(result);
    }

    // GET: api/schedules/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ScheduleDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _getByIdHandler.HandleAsync(
            new GetScheduleByIdQuery(id),
            cancellationToken);

        if (result == null)
        {
            return NotFound(new
            {
                message = "Không tìm thấy lịch sự kiện."
            });
        }

        return Ok(result);
    }

    // POST: api/schedules
    [HttpPost]
    public async Task<ActionResult<ScheduleDto>> Create(
        [FromBody] CreateScheduleRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _createHandler.HandleAsync(
            new CreateScheduleCommand(request),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    // PUT: api/schedules/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ScheduleDto>> Update(
        Guid id,
        [FromBody] UpdateScheduleRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _updateHandler.HandleAsync(
            new UpdateScheduleCommand(id, request),
            cancellationToken);

        if (result == null)
        {
            return NotFound(new
            {
                message = "Không tìm thấy lịch sự kiện."
            });
        }

        return Ok(result);
    }

    // DELETE: api/schedules/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _deleteHandler.HandleAsync(
            new DeleteScheduleCommand(id),
            cancellationToken);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Không tìm thấy lịch sự kiện."
            });
        }

        return NoContent();
    }
}