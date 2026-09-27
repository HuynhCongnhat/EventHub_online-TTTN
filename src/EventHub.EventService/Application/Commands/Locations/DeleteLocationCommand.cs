namespace EventHub.EventService.Application.Commands.Locations
{
    public class DeleteLocationCommand
    {
        public Guid Id { get; set; }

        public DeleteLocationCommand(Guid id)
        {
            Id = id;
        }
    }
}
