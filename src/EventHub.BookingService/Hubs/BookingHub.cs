using Microsoft.AspNetCore.SignalR;

namespace EventHub.BookingService.Hubs
{
    public class BookingHub : Hub
    {
        public async Task JoinEventGroup(Guid eventId)
        {
            if (eventId == Guid.Empty)
            {
                throw new HubException("EventId không hợp lệ.");
            }

            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                GetEventGroupName(eventId));
        }

        public async Task LeaveEventGroup(Guid eventId)
        {
            if (eventId == Guid.Empty)
            {
                throw new HubException("EventId không hợp lệ.");
            }

            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                GetEventGroupName(eventId));
        }

        public static string GetEventGroupName(Guid eventId)
        {
            return $"event-{eventId}";
        }
    }
}
