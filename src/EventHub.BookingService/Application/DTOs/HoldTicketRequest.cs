namespace EventHub.BookingService.Application.DTOs
{
    public class HoldTicketRequest
    {
        public Guid UserId { get; set; }

        public Guid TicketInventoryId { get; set; }

        public int Quantity { get; set; }
    }
}
