using EventHub.EventService.Application.DTOs;

namespace EventHub.EventService.Application.Commands.Schedules
{
    public class CreateScheduleCommand
    {
        public CreateScheduleRequest Request { get; set; } = new();

        public CreateScheduleCommand(CreateScheduleRequest request)
        {
            Request = request;
        }
    }
}
