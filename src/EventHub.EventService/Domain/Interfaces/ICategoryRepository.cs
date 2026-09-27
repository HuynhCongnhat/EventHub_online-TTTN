using EventHub.EventService.Domain.Entities;

namespace EventHub.EventService.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Category?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Category> AddAsync(
        Category entity,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Category entity,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Category entity,
        CancellationToken cancellationToken = default);
}