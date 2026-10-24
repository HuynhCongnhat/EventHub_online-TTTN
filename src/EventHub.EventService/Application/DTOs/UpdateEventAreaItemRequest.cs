namespace EventHub.EventService.Application.DTOs
{
    public class UpdateEventAreaItemRequest
    {
        public Guid? Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public string? Description { get; set; }
    }
}
