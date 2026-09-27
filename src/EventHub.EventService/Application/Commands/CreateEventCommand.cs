using EventHub.EventService.Application.DTOs;

namespace EventHub.EventService.Application.Commands
{
    public class CreateEventCommand
    {
        public CreateEventRequest Request { get; set; } = new();
    }
}
