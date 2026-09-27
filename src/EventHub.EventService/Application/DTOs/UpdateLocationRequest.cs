namespace EventHub.EventService.Application.DTOs;

public class UpdateLocationRequest
{
    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string? City { get; set; }

    public int Capacity { get; set; }
}