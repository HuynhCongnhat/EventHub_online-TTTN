using EventHub.EventService.Domain.Interfaces;

namespace EventHub.EventService.Application.Commands.Locations;

public class DeleteLocationHandler
{
    private readonly ILocationRepository _repository;

    public DeleteLocationHandler(ILocationRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> HandleAsync(
        DeleteLocationCommand command,
        CancellationToken cancellationToken = default)
    {
        var location = await _repository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (location == null)
        {
            return false;
        }

        await _repository.DeleteAsync(location, cancellationToken);

        return true;
    }
}