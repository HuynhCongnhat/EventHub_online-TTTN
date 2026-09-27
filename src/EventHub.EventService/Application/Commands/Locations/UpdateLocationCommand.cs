using EventHub.EventService.Application.DTOs;

namespace EventHub.EventService.Application.Commands.Locations;

public class UpdateLocationCommand
{
    public Guid Id { get; }

    public UpdateLocationRequest Request { get; }

    public UpdateLocationCommand(
        Guid id,
        UpdateLocationRequest request)
    {
        Id = id;
        Request = request;
    }
}