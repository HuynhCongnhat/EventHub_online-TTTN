namespace EventHub.EventService.Application.DTOs
{
    public class UpdateScheduleRequest
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string? Note { get; set; }
    }
}
