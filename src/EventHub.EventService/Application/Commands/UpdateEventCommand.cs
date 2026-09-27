using EventHub.EventService.Application.DTOs;

namespace EventHub.EventService.Application.Commands
{
    public class UpdateEventCommand
    {
        public Guid Id { get; set; }

        public UpdateEventRequest Request { get; set; } = new();
    }
}
