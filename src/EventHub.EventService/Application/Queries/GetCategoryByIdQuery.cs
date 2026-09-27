
namespace EventHub.EventService.Application.Queries
{
    public class GetCategoryByIdQuery
    {
        public Guid Id { get; set; }

        public GetCategoryByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
