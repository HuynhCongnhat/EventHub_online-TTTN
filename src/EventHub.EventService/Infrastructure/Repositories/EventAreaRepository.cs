using EventHub.EventService.Domain.Entities;
using EventHub.EventService.Domain.Interfaces;
using EventHub.EventService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventHub.EventService.Infrastructure.Repositories;

public class EventAreaRepository : IEventAreaRepository
{
    private readonly EventDbContext _context;

    public EventAreaRepository(EventDbContext context)
    {
        _context = context;
    }

    public async Task<List<EventArea>> GetByEventIdAsync(
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        return await _context.EventAreas
            .AsNoTracking()
            .Where(x => x.EventId == eventId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<EventArea> AddAsync(
        EventArea entity,
        CancellationToken cancellationToken = default)
    {
        await _context.EventAreas.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task AddRangeAsync(
        IEnumerable<EventArea> entities,
        CancellationToken cancellationToken = default)
    {
        await _context.EventAreas.AddRangeAsync(entities, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteByEventIdAsync(
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var areas = await _context.EventAreas
            .Where(x => x.EventId == eventId)
            .ToListAsync(cancellationToken);

        if (areas.Count == 0)
            return;

        _context.EventAreas.RemoveRange(areas);
        await _context.SaveChangesAsync(cancellationToken);
    }
}