namespace EventHub.Web.Models.EventArea
{
    public class UpdateEventAreasRequest
    {
        public List<CreateEventAreaRequest> Areas { get; set; } = new();
    }
}
