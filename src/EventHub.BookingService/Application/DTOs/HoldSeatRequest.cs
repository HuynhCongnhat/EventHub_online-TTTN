namespace EventHub.BookingService.Application.DTOs
{
    public class HoldSeatRequest
    {
        public Guid EventId { get; set; }

        public Guid SeatId { get; set; }

        public Guid UserId { get; set; }
    }
}
