namespace EventHub.EventService.Application.DTOs;

public class UpdateEventAreasRequest
{
    public List<CreateEventAreaRequest> Areas { get; set; } = new();
}