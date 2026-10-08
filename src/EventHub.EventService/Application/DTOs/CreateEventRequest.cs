namespace EventHub.EventService.Application.DTOs;

public class CreateEventRequest
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public string? VenueMapImageUrl { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public string LocationName { get; set; } = string.Empty;

    public string LocationAddress { get; set; } = string.Empty;

    public string? LocationCity { get; set; }

    public int LocationCapacity { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Status { get; set; } = "Draft";
}