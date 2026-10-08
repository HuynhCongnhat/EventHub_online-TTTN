using EventHub.BookingService.Domain.Enums;

namespace EventHub.BookingService.Domain.Entities
{
    public class TicketInventory
    {
        public Guid Id { get; set; }

        //id sự kiện lấy từ EventSevice booking không sở hữu event entity
        public Guid EventId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int TotalQuantity { get; set; }

        public int SoldQuantity { get; set; }

        public int HeldQuantity { get; set; }

        public TicketHoldStatus Status { get; set; } = TicketHoldStatus.Active;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public int AvailableQuantity => TotalQuantity - SoldQuantity - HeldQuantity;

    }
}
