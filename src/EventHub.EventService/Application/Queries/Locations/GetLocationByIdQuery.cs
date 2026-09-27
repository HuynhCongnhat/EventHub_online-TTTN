namespace EventHub.EventService.Application.Queries.Locations
{
    public class GetLocationByIdQuery
    {
        public Guid Id { get; set; }

        public GetLocationByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
