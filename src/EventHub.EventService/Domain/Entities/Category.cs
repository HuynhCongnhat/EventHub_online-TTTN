namespace EventHub.EventService.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
