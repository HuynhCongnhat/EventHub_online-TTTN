namespace EventHub.EventService.Application.Commands
{
    public class DeleteCategoryCommand
    {
        public Guid Id { get; set; }

        public DeleteCategoryCommand(Guid id)
        {
            Id = id;
        }
    }
}
