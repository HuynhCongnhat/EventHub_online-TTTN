using EventHub.EventService.Application.DTOs;
using EventHub.EventService.Domain.Interfaces;

namespace EventHub.EventService.Application.Queries.Schedules;

public class GetAllSchedulesHandler
{
    private readonly IScheduleRepository _repository;

    public GetAllSchedulesHandler(IScheduleRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ScheduleDto>> HandleAsync(
        GetAllSchedulesQuery query,
        CancellationToken cancellationToken = default)
    {
        var schedules = await _repository.GetAllAsync(cancellationToken);

        return schedules.Select(schedule => new ScheduleDto
        {
            Id = schedule.Id,
            EventId = schedule.EventId,
            StartTime = schedule.StartTime,
            EndTime = schedule.EndTime,
            Note = schedule.Note,
            CreatedAt = schedule.CreatedAt
        }).ToList();
    }
}