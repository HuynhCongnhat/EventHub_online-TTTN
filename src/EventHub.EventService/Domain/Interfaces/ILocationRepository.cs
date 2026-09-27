using EventHub.EventService.Domain.Entities;

namespace EventHub.EventService.Domain.Interfaces;

public interface ILocationRepository
{
    Task<List<Location>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Location?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Location> AddAsync(
        Location entity,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Location entity,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Location entity,
        CancellationToken cancellationToken = default);
}