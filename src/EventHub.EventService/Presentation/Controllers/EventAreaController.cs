
using EventHub.EventService.Application.DTOs;
using EventHub.EventService.Domain.Entities;
using EventHub.EventService.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.EventService.Presentation.Controllers;

[ApiController]
[Route("api/events/{eventId:guid}/areas")]
public class EventAreaController : ControllerBase
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventAreaRepository _eventAreaRepository;

    public EventAreaController(
        IEventRepository eventRepository,
        IEventAreaRepository eventAreaRepository)
    {
        _eventRepository = eventRepository;
        _eventAreaRepository = eventAreaRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<EventAreaDto>>> GetByEventId(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(
            eventId, cancellationToken);

        if (eventEntity == null)
            return NotFound(new { message = "Không tìm thấy sự kiện." });

        var areas = await _eventAreaRepository.GetByEventIdAsync(
            eventId, cancellationToken);

        return Ok(areas.Select(x => new EventAreaDto
        {
            Id = x.Id,
            EventId = x.EventId,
            Name = x.Name,
            Capacity = x.Capacity,
            Description = x.Description
        }).ToList());
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAreas(
        Guid eventId,
        [FromBody] UpdateEventAreasRequest request,
        CancellationToken cancellationToken)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(
            eventId, cancellationToken);

        if (eventEntity == null)
            return NotFound(new { message = "Không tìm thấy sự kiện." });

        if (request.Areas == null || request.Areas.Count == 0)
            return BadRequest(new { message = "Phải có ít nhất một khu vực." });

        if (request.Areas.Any(x =>
            string.IsNullOrWhiteSpace(x.Name) || x.Capacity <= 0))
        {
            return BadRequest(new
            {
                message = "Tên khu vực không được để trống và sức chứa phải lớn hơn 0."
            });
        }

        var names = request.Areas
            .Select(x => x.Name.Trim())
            .ToList();

        if (names.Distinct(StringComparer.OrdinalIgnoreCase).Count() != names.Count)
        {
            return BadRequest(new
            {
                message = "Tên khu vực không được trùng nhau."
            });
        }

        var totalCapacity = request.Areas.Sum(x => (long)x.Capacity);

        if (totalCapacity > int.MaxValue)
        {
            return BadRequest(new
            {
                message = "Tổng sức chứa vượt giới hạn cho phép."
            });
        }

        var areas = request.Areas.Select(x => new EventArea
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            Name = x.Name.Trim(),
            Capacity = x.Capacity,
            Description = x.Description?.Trim()
        }).ToList();

        await _eventAreaRepository.DeleteByEventIdAsync(
            eventId, cancellationToken);

        await _eventAreaRepository.AddRangeAsync(
            areas, cancellationToken);

        return Ok(new
        {
            eventId,
            totalCapacity,
            areas = areas.Select(x => new EventAreaDto
            {
                Id = x.Id,
                EventId = x.EventId,
                Name = x.Name,
                Capacity = x.Capacity,
                Description = x.Description
            }).ToList()
        });
    }
}