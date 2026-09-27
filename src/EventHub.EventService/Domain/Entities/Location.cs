namespace EventHub.EventService.Domain.Entities
{
    public class Location
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string? City { get; set; }

        public int Capacity { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
