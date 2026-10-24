namespace EventHub.EventService.Application.DTOs;

public class UpdateEventAreasRequest
{
    public List<UpdateEventAreaItemRequest> Areas { get; set; } = new();
}