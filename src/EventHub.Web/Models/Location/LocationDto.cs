namespace EventHub.Web.Models.Location;

public class LocationDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string? City { get; set; }

    public int Capacity { get; set; }

    public DateTime CreatedAt { get; set; }
}