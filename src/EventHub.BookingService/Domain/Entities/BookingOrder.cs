using EventHub.BookingService.Domain.Enums;

namespace EventHub.BookingService.Domain.Entities
{
    public class BookingOrder
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid EventId { get; set; }

        public string OrderCode { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public BookingOrderStatus Status { get; set; } = BookingOrderStatus.Pending;

        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<BookingItem> Items { get; set; } = new List<BookingItem>();
    }
}
