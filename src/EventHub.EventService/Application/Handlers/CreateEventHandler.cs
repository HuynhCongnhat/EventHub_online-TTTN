using EventHub.EventService.Application.Commands;
using EventHub.EventService.Application.DTOs;
using EventHub.EventService.Domain.Entities;
using EventHub.EventService.Domain.Interfaces;

namespace EventHub.EventService.Application.Handlers;

public class CreateEventHandler
{
    private readonly IEventRepository _eventRepository;

    public CreateEventHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<EventDto> HandleAsync(
        CreateEventCommand command,
        CancellationToken cancellationToken = default)
    {
        var request = command.Request;

        var eventEntity = new Event
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            CategoryId = request.CategoryId,
            LocationId = request.LocationId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = request.Status,
            CreatedAt = DateTime.UtcNow
        };

        var createdEvent = await _eventRepository.AddAsync(
            eventEntity,
            cancellationToken);

        return new EventDto
        {
            Id = createdEvent.Id,
            Name = createdEvent.Name,
            Description = createdEvent.Description,
            ImageUrl = createdEvent.ImageUrl,
            CategoryId = createdEvent.CategoryId,
            LocationId = createdEvent.LocationId,
            StartDate = createdEvent.StartDate,
            EndDate = createdEvent.EndDate,
            Status = createdEvent.Status,
            CreatedAt = createdEvent.CreatedAt,
            UpdatedAt = createdEvent.UpdatedAt
        };
    }
}