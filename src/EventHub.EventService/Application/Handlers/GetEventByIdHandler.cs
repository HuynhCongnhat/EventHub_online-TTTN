using EventHub.EventService.Application.DTOs;
using EventHub.EventService.Application.Queries;
using EventHub.EventService.Domain.Interfaces;

namespace EventHub.EventService.Application.Handlers;

public class GetEventByIdHandler
{
    private readonly IEventRepository _eventRepository;

    public GetEventByIdHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<EventDto?> HandleAsync(
        GetEventByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(
            query.Id,
            cancellationToken);

        if (eventEntity == null)
        {
            return null;
        }

        return new EventDto
        {
            Id = eventEntity.Id,
            Name = eventEntity.Name,
            Description = eventEntity.Description,
            ImageUrl = eventEntity.ImageUrl,

            CategoryId = eventEntity.CategoryId,
            LocationCapacity = eventEntity.Location?.Capacity ?? 0,
            CategoryName = eventEntity.Category?.Name ?? string.Empty,

            LocationId = eventEntity.LocationId,
            LocationName = eventEntity.Location?.Name ?? string.Empty,
            LocationAddress = eventEntity.Location?.Address ?? string.Empty,
            City = eventEntity.Location?.City,

            StartDate = eventEntity.StartDate,
            EndDate = eventEntity.EndDate,
            Status = eventEntity.Status,
            CreatedAt = eventEntity.CreatedAt,
            UpdatedAt = eventEntity.UpdatedAt
        };
    }
}