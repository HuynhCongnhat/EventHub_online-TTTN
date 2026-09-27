namespace EventHub.EventService.Application.Queries.Schedules
{
    public class GetScheduleByIdQuery
    {
        public Guid Id { get; set; }

        public GetScheduleByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
