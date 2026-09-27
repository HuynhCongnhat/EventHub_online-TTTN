namespace EventHub.EventService.Application.Commands.Schedules
{
    public class DeleteScheduleCommand
    {
        public Guid Id { get; set; }

        public DeleteScheduleCommand(Guid id)
        {
            Id = id;
        }
    }
}
