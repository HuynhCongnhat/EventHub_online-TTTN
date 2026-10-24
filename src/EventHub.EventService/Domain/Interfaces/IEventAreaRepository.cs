using EventHub.EventService.Domain.Entities;

namespace EventHub.EventService.Domain.Interfaces;

public interface IEventAreaRepository
{
    Task<List<EventArea>> GetByEventIdAsync(
        Guid eventId,
        CancellationToken cancellationToken = default);

    Task<EventArea> AddAsync(
        EventArea entity,
        CancellationToken cancellationToken = default);

    Task AddRangeAsync(
        IEnumerable<EventArea> entities,
        CancellationToken cancellationToken = default);

    Task DeleteByEventIdAsync(
        Guid eventId,
        CancellationToken cancellationToken = default);

    Task UpdateRangeAsync(
        IEnumerable<EventArea> entities,
        CancellationToken cancellationToken = default);

    Task DeleteByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default);
}