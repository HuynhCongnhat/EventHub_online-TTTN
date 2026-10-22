namespace EventHub.Web.Models.EventArea
{
    public class CreateEventAreaRequest
    {
        public string Name { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public string? Description { get; set; }
    }
}
