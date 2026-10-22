using Microsoft.AspNetCore.SignalR;
using EventHub.BookingService.Application.DTOs;
using EventHub.BookingService.Hubs;

namespace EventHub.BookingService.Application.Services
{
    public class SeatRealtimeNotifier
    {
        private readonly IHubContext<BookingHub> _hubContext;

        public SeatRealtimeNotifier(IHubContext<BookingHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task NotifiSeatStatusChangedAsync(SeatStatusChangedDto seat, CancellationToken cancellationToken = default)
        {
            var groupName = BookingHub.GetEventGroupName(seat.EventId);

            return _hubContext.Clients.Group(groupName).SendAsync("SeatStatusChanged", seat, cancellationToken);
        }

    }
}
