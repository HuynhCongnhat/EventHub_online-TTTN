using EventHub.EventService.Application.Commands;
using EventHub.EventService.Application.DTOs;
using EventHub.EventService.Domain.Interfaces;

namespace EventHub.EventService.Application.Handlers;

public class UpdateEventHandler
{
    private readonly IEventRepository _eventRepository;

    public UpdateEventHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<EventDto?> HandleAsync(
        UpdateEventCommand command,
        CancellationToken cancellationToken = default)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (eventEntity == null)
        {
            return null;
        }

        var request = command.Request;

        eventEntity.Name = request.Name;
        eventEntity.Description = request.Description;
        eventEntity.ImageUrl = request.ImageUrl;
        eventEntity.CategoryId = request.CategoryId;
        eventEntity.LocationId = request.LocationId;
        eventEntity.StartDate = request.StartDate;
        eventEntity.EndDate = request.EndDate;
        eventEntity.Status = request.Status;
        eventEntity.UpdatedAt = DateTime.UtcNow;

        await _eventRepository.UpdateAsync(
            eventEntity,
            cancellationToken);

        return new EventDto
        {
            Id = eventEntity.Id,
            Name = eventEntity.Name,
            Description = eventEntity.Description,
            ImageUrl = eventEntity.ImageUrl,
            CategoryId = eventEntity.CategoryId,
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