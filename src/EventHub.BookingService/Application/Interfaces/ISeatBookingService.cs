using EventHub.BookingService.Application.DTOs;

namespace EventHub.BookingService.Application.Interfaces
{
    public interface ISeatBookingService
    {
        Task<SeatStatusChangedDto> HoldSeatAsync(HoldSeatRequest request, CancellationToken cancellationToken = default);

        Task<SeatStatusChangedDto> ReleaseSeatAsync(Guid seatId, Guid userId, CancellationToken cancellationToken = default);

        Task<List<SeatStatusChangedDto>> GetSeatsByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);

    }
}
