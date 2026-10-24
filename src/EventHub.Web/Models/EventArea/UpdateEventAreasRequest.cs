namespace EventHub.Web.Models.EventArea
{
    public class UpdateEventAreasRequest
    {
        public List<UpdateEventAreaItemRequest> Areas { get; set; } = new();
    }
}
