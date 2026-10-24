
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

        var existingAreas = await _eventAreaRepository.GetByEventIdAsync(
    eventId, cancellationToken);

        var existingById = existingAreas.ToDictionary(x => x.Id);
        var requestedIds = request.Areas
            .Where(x => x.Id.HasValue)
            .Select(x => x.Id!.Value)
            .ToList();

        // Từ chối ID bị trùng trong request.
        if (requestedIds.Distinct().Count() != requestedIds.Count)
        {
            return BadRequest(new
            {
                message = "ID khu vực không được trùng nhau."
            });
        }

        // Từ chối ID không thuộc sự kiện hiện tại hoặc không tồn tại.
        if (requestedIds.Any(id => !existingById.ContainsKey(id)))
        {
            return BadRequest(new
            {
                message = "Có khu vực không tồn tại hoặc không thuộc sự kiện này."
            });
        }

        var areasToUpdate = new List<EventArea>();
        var areasToAdd = new List<EventArea>();

        foreach (var item in request.Areas)
        {
            if (item.Id.HasValue)
            {
                var area = existingById[item.Id.Value];

                area.Name = item.Name.Trim();
                area.Capacity = item.Capacity;
                area.Description = item.Description?.Trim();

                areasToUpdate.Add(area);
            }
            else
            {
                areasToAdd.Add(new EventArea
                {
                    Id = Guid.NewGuid(),
                    EventId = eventId,
                    Name = item.Name.Trim(),
                    Capacity = item.Capacity,
                    Description = item.Description?.Trim()
                });
            }
        }

        // Chỉ xóa những khu vực cũ không còn được gửi lên.
        var requestedIdSet = requestedIds.ToHashSet();
        var idsToDelete = existingAreas
            .Where(x => !requestedIdSet.Contains(x.Id))
            .Select(x => x.Id)
            .ToList();

        await _eventAreaRepository.UpdateRangeAsync(
            areasToUpdate, cancellationToken);

        await _eventAreaRepository.AddRangeAsync(
            areasToAdd, cancellationToken);

        await _eventAreaRepository.DeleteByIdsAsync(
            idsToDelete, cancellationToken);

        var savedAreas = await _eventAreaRepository.GetByEventIdAsync(
            eventId, cancellationToken);

        return Ok(new
        {
            eventId,
            totalCapacity = savedAreas.Sum(x => (long)x.Capacity),
            areas = savedAreas.Select(x => new EventAreaDto
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