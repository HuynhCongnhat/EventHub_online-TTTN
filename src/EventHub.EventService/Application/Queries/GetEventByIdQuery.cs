namespace EventHub.EventService.Application.Queries
{
    public class GetEventByIdQuery
    {
        public Guid Id { get; set; }

        public GetEventByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
