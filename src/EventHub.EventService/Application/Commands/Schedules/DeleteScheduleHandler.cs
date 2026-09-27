using EventHub.EventService.Domain.Interfaces;

namespace EventHub.EventService.Application.Commands.Schedules;

public class DeleteScheduleHandler
{
    private readonly IScheduleRepository _repository;

    public DeleteScheduleHandler(IScheduleRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> HandleAsync(
        DeleteScheduleCommand command,
        CancellationToken cancellationToken = default)
    {
        var schedule = await _repository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (schedule == null)
        {
            return false;
        }

        await _repository.DeleteAsync(
            schedule,
            cancellationToken);

        return true;
    }
}