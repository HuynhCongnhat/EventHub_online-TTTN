namespace EventHub.EventService.Application.Commands
{
    public class DeleteEventCommand
    {
        public Guid Id { get; set; }

        public DeleteEventCommand(Guid id)
        {
            Id = id;
        }
    }
}
