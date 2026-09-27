using EventHub.EventService.Application.DTOs;
using EventHub.EventService.Application.Queries;
using EventHub.EventService.Domain.Interfaces;

namespace EventHub.EventService.Application.Handlers;

public class GetAllEventsHandler
{
    private readonly IEventRepository _eventRepository;

    public GetAllEventsHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<List<EventDto>> HandleAsync(
        GetAllEventsQuery query,
        CancellationToken cancellationToken = default)
    {
        var events = await _eventRepository.GetAllAsync(cancellationToken);

        return events.Select(x => new EventDto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            ImageUrl = x.ImageUrl,
            CategoryId = x.CategoryId,
            CategoryName = x.Category?.Name ?? string.Empty,
            LocationId = x.LocationId,
            LocationName = x.Location?.Name ?? string.Empty,
            LocationAddress = x.Location?.Address ?? string.Empty,
            City = x.Location?.City,
            StartDate = x.StartDate,
            EndDate = x.EndDate,
            Status = x.Status,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }).ToList();
    }
}