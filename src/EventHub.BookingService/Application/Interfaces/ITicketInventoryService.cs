using EventHub.BookingService.Application.DTOs;

namespace EventHub.BookingService.Application.Interfaces
{
    public interface ITicketInventoryService
    {
        Task<List<TicketInventoryDto>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);

        Task<TicketInventoryDto> CreateAsync(CreateTicketInventoryRequest request, CancellationToken cancellationToken = default);

        Task<HoldTicketResponse> HoldAsync(HoldTicketRequest request, CancellationToken cancellationToken = default);
    }
}
