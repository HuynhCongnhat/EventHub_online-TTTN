using EventHub.EventService.Application.DTOs;
using EventHub.EventService.Domain.Entities;
using EventHub.EventService.Domain.Interfaces;

namespace EventHub.EventService.Application.Commands.Locations;

public class CreateLocationHandler
{
    private readonly ILocationRepository _repository;

    public CreateLocationHandler(ILocationRepository repository)
    {
        _repository = repository;
    }

    public async Task<LocationDto> HandleAsync(
        CreateLocationCommand command,
        CancellationToken cancellationToken = default)
    {
        var location = new Location
        {
            Id = Guid.NewGuid(),
            Name = command.Request.Name,
            Address = command.Request.Address,
            City = command.Request.City,
            Capacity = command.Request.Capacity,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(location, cancellationToken);

        return new LocationDto
        {
            Id = location.Id,
            Name = location.Name,
            Address = location.Address,
            City = location.City,
            Capacity = location.Capacity,
            CreatedAt = location.CreatedAt
        };
    }
}