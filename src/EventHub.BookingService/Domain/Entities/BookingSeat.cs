using EventHub.BookingService.Domain.Enums;

namespace EventHub.BookingService.Domain.Entities
{
    public class BookingSeat
    {
        public Guid Id { get; set; }

        public Guid EventId { get; set; }

        public Guid TicketInventoryId { get; set; }

        public TicketInventory TicketInventory { get; set; } = null!;

        public string SeatCode { get; set; } = string.Empty;

        public BookingSeatStatus Status { get; set; } = BookingSeatStatus.Available;

        public Guid? BookingOrderId { get; set; }

        public BookingOrder? BookingOrder { get; set; }

        public DateTime? HeldUntil { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
