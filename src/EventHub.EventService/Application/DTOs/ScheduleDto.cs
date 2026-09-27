namespace EventHub.EventService.Application.DTOs;

public class ScheduleDto
{
    public Guid Id { get; set; }

    public Guid EventId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }
}