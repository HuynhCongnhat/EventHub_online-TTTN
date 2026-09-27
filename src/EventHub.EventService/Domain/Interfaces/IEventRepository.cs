using EventHub.EventService.Domain.Entities;

namespace EventHub.EventService.Domain.Interfaces;

public interface IEventRepository
{
    Task<List<Event>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Event?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Event> AddAsync(
        Event entity,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Event entity,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Event entity,
        CancellationToken cancellationToken = default);
}