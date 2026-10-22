namespace EventHub.Web.Models.EventArea
{
    public class EventAreaDto
    {
        public Guid Id { get; set; }

        public Guid EventId { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public string? Description { get; set; }
    }
}
