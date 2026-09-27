using EventHub.EventService.Domain.Entities;
using EventHub.EventService.Domain.Interfaces;
using EventHub.EventService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventHub.EventService.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly EventDbContext _context;

    public CategoryRepository(EventDbContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Category?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Category> AddAsync(
        Category entity,
        CancellationToken cancellationToken = default)
    {
        await _context.Categories.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task UpdateAsync(
        Category entity,
        CancellationToken cancellationToken = default)
    {
        _context.Categories.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Category entity,
        CancellationToken cancellationToken = default)
    {
        _context.Categories.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}