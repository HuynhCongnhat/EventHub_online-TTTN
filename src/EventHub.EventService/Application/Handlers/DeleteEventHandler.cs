using EventHub.EventService.Application.Commands;
using EventHub.EventService.Domain.Interfaces;

namespace EventHub.EventService.Application.Handlers;

public class DeleteEventHandler
{
    private readonly IEventRepository _eventRepository;

    public DeleteEventHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<bool> HandleAsync(
        DeleteEventCommand command,
        CancellationToken cancellationToken = default)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (eventEntity == null)
        {
            return false;
        }

        await _eventRepository.DeleteAsync(
            eventEntity,
            cancellationToken);

        return true;
    }
}