namespace EventHub.Web.Models.Event;

public class EventDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public string? VenueMapImageUrl { get; set; }

    public Guid CategoryId { get; set; }

    public int LocationCapacity { get; set; }

    public string? CategoryName { get; set; }

    public Guid LocationId { get; set; }

    public string? LocationName { get; set; }

    public string? LocationAddress { get; set; }

    public string? City { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}