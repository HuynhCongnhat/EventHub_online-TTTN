using EventHub.EventService.Application.DTOs;

namespace EventHub.EventService.Application.Commands.Schedules;

public class UpdateScheduleCommand
{
    public Guid Id { get; }

    public UpdateScheduleRequest Request { get; }

    public UpdateScheduleCommand(
        Guid id,
        UpdateScheduleRequest request)
    {
        Id = id;
        Request = request;
    }
}