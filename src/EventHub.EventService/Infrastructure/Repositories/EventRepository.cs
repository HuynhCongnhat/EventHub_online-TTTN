using EventHub.EventService.Domain.Entities;
using EventHub.EventService.Domain.Interfaces;
using EventHub.EventService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventHub.EventService.Infrastructure.Repositories;

public class EventRepository : IEventRepository
{
    private readonly EventDbContext _context;

    public EventRepository(EventDbContext context)
    {
        _context = context;
    }

    public async Task<List<Event>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .Include(x => x.Category)
            .Include(x => x.Location)
            .Include(x => x.Schedules)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Event?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .Include(x => x.Category)
            .Include(x => x.Location)
            .Include(x => x.Schedules)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Event> AddAsync(
        Event entity,
        CancellationToken cancellationToken = default)
    {
        await _context.Events.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task UpdateAsync(
        Event entity,
        CancellationToken cancellationToken = default)
    {
        _context.Events.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Event entity,
        CancellationToken cancellationToken = default)
    {
        _context.Events.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}