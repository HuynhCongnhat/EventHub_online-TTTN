namespace EventHub.BookingService.Application.DTOs
{
    public class SeatStatusChangedDto
    {
        public Guid EventId { get; set; }

        public Guid SeatId { get; set; }

        public string SeatCode { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public Guid? BookingOrderId { get; set; }

        public string? OrderCode { get; set; }

        public DateTime? HeldUntil { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
