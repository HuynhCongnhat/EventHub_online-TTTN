using EventHub.EventService.Application.DTOs;
using EventHub.EventService.Domain.Interfaces;

namespace EventHub.EventService.Application.Commands.Locations;

public class UpdateLocationHandler
{
    private readonly ILocationRepository _repository;

    public UpdateLocationHandler(ILocationRepository repository)
    {
        _repository = repository;
    }

    public async Task<LocationDto?> HandleAsync(
        UpdateLocationCommand command,
        CancellationToken cancellationToken = default)
    {
        var location = await _repository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (location == null)
        {
            return null;
        }

        location.Name = command.Request.Name;
        location.Address = command.Request.Address;
        location.City = command.Request.City;
        location.Capacity = command.Request.Capacity;

        await _repository.UpdateAsync(location, cancellationToken);

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