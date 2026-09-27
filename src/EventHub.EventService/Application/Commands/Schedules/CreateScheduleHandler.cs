using EventHub.EventService.Application.DTOs;
using EventHub.EventService.Domain.Entities;
using EventHub.EventService.Domain.Interfaces;

namespace EventHub.EventService.Application.Commands.Schedules;

public class CreateScheduleHandler
{
    private readonly IScheduleRepository _repository;

    public CreateScheduleHandler(IScheduleRepository repository)
    {
        _repository = repository;
    }

    public async Task<ScheduleDto> HandleAsync(
        CreateScheduleCommand command,
        CancellationToken cancellationToken = default)
    {
        var schedule = new Schedule
        {
            Id = Guid.NewGuid(),
            EventId = command.Request.EventId,
            StartTime = command.Request.StartTime,
            EndTime = command.Request.EndTime,
            Note = command.Request.Note,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(schedule, cancellationToken);

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