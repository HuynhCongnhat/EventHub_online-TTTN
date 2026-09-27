using EventHub.EventService.Domain.Entities;
using EventHub.EventService.Domain.Interfaces;
using EventHub.EventService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventHub.EventService.Infrastructure.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly EventDbContext _context;

    public LocationRepository(EventDbContext context)
    {
        _context = context;
    }

    public async Task<List<Location>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Locations
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Location?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Locations
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Location> AddAsync(
        Location entity,
        CancellationToken cancellationToken = default)
    {
        await _context.Locations.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task UpdateAsync(
        Location entity,
        CancellationToken cancellationToken = default)
    {
        _context.Locations.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Location entity,
        CancellationToken cancellationToken = default)
    {
        _context.Locations.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}