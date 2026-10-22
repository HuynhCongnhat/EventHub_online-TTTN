using Microsoft.AspNetCore.SignalR.Client;

namespace EventHub.Web.Services;

public class SeatSignalRTestClient : IAsyncDisposable
{
    private readonly HubConnection _connection;


    public SeatSignalRTestClient()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5003/hubs/bookingHub")
                .WithAutomaticReconnect()
                .Build();

            _connection.On<object>(
                "SeatStatusChanged",
                seat =>
                {
                    Console.WriteLine(
                        $"Nhận cập nhật trạng thái ghế: {seat}");
                });
        }

    public async Task ConnectAsync()
    {
        if (_connection.State == HubConnectionState.Disconnected)
        {
            await _connection.StartAsync();
        }
    }

    public async Task JoinEventGroupAsync(Guid eventId)
    {
        await ConnectAsync();

        await _connection.InvokeAsync("JoinEventGroup", eventId);
        Console.WriteLine($"Đã tham gia nhóm sự kiện: {eventId}");
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
    }
}
