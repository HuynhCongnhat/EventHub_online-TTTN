using EventHub.EventService.Application.DTOs;

namespace EventHub.EventService.Application.Commands.Locations;

public class CreateLocationCommand
{
    public CreateLocationRequest Request { get; }

    public CreateLocationCommand(CreateLocationRequest request)
    {
        Request = request;
    }
}