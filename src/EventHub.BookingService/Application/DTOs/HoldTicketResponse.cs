namespace EventHub.BookingService.Application.DTOs
{
    public class HoldTicketResponse
    {
        public Guid BookingOrderId { get; set; }

        public string OrderCode { get; set; } = string.Empty;

        public Guid TicketInventoryId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}
