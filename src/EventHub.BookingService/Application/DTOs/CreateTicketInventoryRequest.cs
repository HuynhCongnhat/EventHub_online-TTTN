namespace EventHub.BookingService.Application.DTOs
{
    public class CreateTicketInventoryRequest
    {
        public Guid EventId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int TotalQuantity { get; set; }
    }
}
