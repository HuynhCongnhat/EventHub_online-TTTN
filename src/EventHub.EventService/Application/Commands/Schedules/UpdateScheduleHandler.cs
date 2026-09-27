using EventHub.EventService.Application.DTOs;
using EventHub.EventService.Domain.Interfaces;

namespace EventHub.EventService.Application.Commands.Schedules;

public class UpdateScheduleHandler
{
    private readonly IScheduleRepository _repository;

    public UpdateScheduleHandler(IScheduleRepository repository)
    {
        _repository = repository;
    }

    public async Task<ScheduleDto?> HandleAsync(
        UpdateScheduleCommand command,
        CancellationToken cancellationToken = default)
    {
        var schedule = await _repository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (schedule == null)
        {
            return null;
        }

        schedule.StartTime = command.Request.StartTime;
        schedule.EndTime = command.Request.EndTime;
        schedule.Note = command.Request.Note;

        await _repository.UpdateAsync(
            schedule,
            cancellationToken);

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