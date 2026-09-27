using EventHub.EventService.Domain.Entities;

namespace EventHub.EventService.Domain.Interfaces;

public interface IScheduleRepository
{
    Task<List<Schedule>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Schedule?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Schedule> AddAsync(
        Schedule entity,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Schedule entity,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Schedule entity,
        CancellationToken cancellationToken = default);
}