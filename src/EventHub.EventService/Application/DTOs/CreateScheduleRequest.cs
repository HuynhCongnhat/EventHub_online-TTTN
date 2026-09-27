namespace EventHub.EventService.Application.DTOs
{
    public class CreateScheduleRequest
    {
        public Guid EventId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
        
        public string? Note { get; set; }
    }
}
