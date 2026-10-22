namespace EventHub.EventService.Domain.Entities
{
    public class EventArea
    {
        public Guid Id { get; set; }

        public Guid EventId { get; set; }

        public Event Event { get; set; } = null!;

        public string Name { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
