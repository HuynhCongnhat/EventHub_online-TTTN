namespace EventHub.EventService.Application.DTOs;

public class EventDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public Guid CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public Guid LocationId { get; set; }

    public string LocationName { get; set; } = string.Empty;

    public string LocationAddress { get; set; } = string.Empty;

    public string? City { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}