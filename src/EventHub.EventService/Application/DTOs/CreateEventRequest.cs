namespace EventHub.EventService.Application.DTOs;

public class CreateEventRequest
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public Guid CategoryId { get; set; }

    public Guid LocationId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Status { get; set; } = "Draft";
}