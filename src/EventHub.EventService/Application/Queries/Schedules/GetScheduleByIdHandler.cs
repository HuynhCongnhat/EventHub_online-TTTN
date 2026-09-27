using EventHub.EventService.Application.DTOs;
using EventHub.EventService.Domain.Interfaces;

namespace EventHub.EventService.Application.Queries.Schedules;

public class GetScheduleByIdHandler
{
    private readonly IScheduleRepository _repository;

    public GetScheduleByIdHandler(IScheduleRepository repository)
    {
        _repository = repository;
    }

    public async Task<ScheduleDto?> HandleAsync(
        GetScheduleByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var schedule = await _repository.GetByIdAsync(
            query.Id,
            cancellationToken);

        if (schedule == null)
        {
            return null;
        }

        return new ScheduleDto
        {
            Id = schedule.Id,
            EventId = schedule.EventId,
            StartTime = schedule.StartTime,
            EndTime = schedule.EndTime,
            Note = schedule.Note,
            CreatedAt = schedule.CreatedAt
        };
    }
}