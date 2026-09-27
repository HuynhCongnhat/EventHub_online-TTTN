using EventHub.EventService.Application.DTOs;
using EventHub.EventService.Domain.Interfaces;

namespace EventHub.EventService.Application.Queries.Locations;

public class GetLocationByIdHandler
{
    private readonly ILocationRepository _repository;

    public GetLocationByIdHandler(ILocationRepository repository)
    {
        _repository = repository;
    }

    public async Task<LocationDto?> HandleAsync(
        GetLocationByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var location = await _repository.GetByIdAsync(
            query.Id,
            cancellationToken);

        if (location == null)
        {
            return null;
        }

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