namespace EventHub.EventService.Domain.Entities
{
    public class Schedule
    {
        public Guid Id { get; set; }

        public Guid EventId { get; set; }

        public Event Event { get; set; } = null!;

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
