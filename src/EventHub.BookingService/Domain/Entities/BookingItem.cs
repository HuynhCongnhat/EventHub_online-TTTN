namespace EventHub.BookingService.Domain.Entities
{
    public class BookingItem
    {
        public Guid Id { get; set; }

        public Guid BookingOrderId { get; set; }

        public BookingOrder BookingOrder { get; set; } = null!;

        public Guid TicketInventoryId { get; set; }

        public TicketInventory TicketInventory { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
