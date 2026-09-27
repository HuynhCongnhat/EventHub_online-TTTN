using EventHub.EventService.Domain.Entities;
using EventHub.EventService.Domain.Interfaces;
using EventHub.EventService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventHub.EventService.Infrastructure.Repositories;

public class ScheduleRepository : IScheduleRepository
{
    private readonly EventDbContext _context;

    public ScheduleRepository(EventDbContext context)
    {
        _context = context;
    }

    public async Task<List<Schedule>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Schedules
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Schedule?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Schedules
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Schedule> AddAsync(
        Schedule entity,
        CancellationToken cancellationToken = default)
    {
        await _context.Schedules.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task UpdateAsync(
        Schedule entity,
        CancellationToken cancellationToken = default)
    {
        _context.Schedules.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Schedule entity,
        CancellationToken cancellationToken = default)
    {
        _context.Schedules.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}