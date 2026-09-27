using EventHub.EventService.Application.Commands.Locations;
using EventHub.EventService.Application.DTOs;
using EventHub.EventService.Application.Queries.Locations;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.EventService.Presentation.Controllers;

[ApiController]
[Route("api/locations")]
public class LocationsController : ControllerBase
{
    private readonly GetAllLocationsHandler _getAllHandler;
    private readonly GetLocationByIdHandler _getByIdHandler;
    private readonly CreateLocationHandler _createHandler;
    private readonly UpdateLocationHandler _updateHandler;
    private readonly DeleteLocationHandler _deleteHandler;

    public LocationsController(
        GetAllLocationsHandler getAllHandler,
        GetLocationByIdHandler getByIdHandler,
        CreateLocationHandler createHandler,
        UpdateLocationHandler updateHandler,
        DeleteLocationHandler deleteHandler)
    {
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
    }

    // GET: api/locations
    [HttpGet]
    public async Task<ActionResult<List<LocationDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _getAllHandler.HandleAsync(
            new GetAllLocationsQuery(),
            cancellationToken);

        return Ok(result);
    }

    // GET: api/locations/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LocationDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _getByIdHandler.HandleAsync(
            new GetLocationByIdQuery(id),
            cancellationToken);

        if (result == null)
        {
            return NotFound(new
            {
                message = "Không tìm thấy địa điểm."
            });
        }

        return Ok(result);
    }

    // POST: api/locations
    [HttpPost]
    public async Task<ActionResult<LocationDto>> Create(
        [FromBody] CreateLocationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _createHandler.HandleAsync(
            new CreateLocationCommand(request),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    // PUT: api/locations/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<LocationDto>> Update(
        Guid id,
        [FromBody] UpdateLocationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _updateHandler.HandleAsync(
            new UpdateLocationCommand(id, request),
            cancellationToken);

        if (result == null)
        {
            return NotFound(new
            {
                message = "Không tìm thấy địa điểm."
            });
        }

        return Ok(result);
    }

    // DELETE: api/locations/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _deleteHandler.HandleAsync(
            new DeleteLocationCommand(id),
            cancellationToken);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Không tìm thấy địa điểm."
            });
        }

        return NoContent();
    }
}