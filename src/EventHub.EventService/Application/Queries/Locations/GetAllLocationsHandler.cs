using EventHub.EventService.Application.DTOs;
using EventHub.EventService.Domain.Interfaces;

namespace EventHub.EventService.Application.Queries.Locations;

public class GetAllLocationsHandler
{
    private readonly ILocationRepository _repository;

    public GetAllLocationsHandler(ILocationRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<LocationDto>> HandleAsync(
        GetAllLocationsQuery query,
        CancellationToken cancellationToken = default)
    {
        var locations = await _repository.GetAllAsync(cancellationToken);

        return locations.Select(location => new LocationDto
        {
            Id = location.Id,
            Name = location.Name,
            Address = location.Address,
            City = location.City,
            Capacity = location.Capacity,
            CreatedAt = location.CreatedAt
        }).ToList();
    }
}