using EventHub.Web.Models.Booking;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace EventHub.Web.Services;

public sealed class SeatRealtimeService : IAsyncDisposable
{
    private readonly NavigationManager _navigationManager;
    private HubConnection? _connection;
    private Guid _eventId;
    private bool _disposed;

    public event Action<SeatStatusChangedDto>? SeatStatusChanged;
    public event Action? Reconnected;
    public event Action<Exception?>? ConnectionClosed;

    public HubConnectionState State =>
        _connection?.State ?? HubConnectionState.Disconnected;

    public SeatRealtimeService(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }

    public async Task ConnectAsync(Guid eventId)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (eventId == Guid.Empty)
            throw new ArgumentException("EventId không hợp lệ.", nameof(eventId));

        if (_connection is not null && _eventId == eventId &&
            _connection.State == HubConnectionState.Connected)
        {
            return;
        }

        await DisposeConnectionAsync();
        _eventId = eventId;

        var hubUrl = GetHubUrl();

        var connection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        connection.On<SeatStatusChangedDto>(
            "SeatStatusChanged",
            dto =>
            {
                if (dto.EventId == _eventId)
                    SeatStatusChanged?.Invoke(dto);
            });

        connection.Reconnected += async _ =>
        {
            try
            {
                await connection.InvokeAsync("JoinEventGroup", _eventId);
                Reconnected?.Invoke();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"Không thể tham gia lại nhóm ghế: {ex.Message}");
            }
        };

        connection.Closed += ex =>
        {
            ConnectionClosed?.Invoke(ex);
            return Task.CompletedTask;
        };

        _connection = connection;

        try
        {
            await connection.StartAsync();
            await connection.InvokeAsync("JoinEventGroup", _eventId);
        }
        catch
        {
            await DisposeConnectionAsync();
            throw;
        }
    }

    public async Task DisconnectAsync()
    {
        await DisposeConnectionAsync();
        _eventId = Guid.Empty;
    }

    private string GetHubUrl()
    {
        // Web chạy trong Docker: gọi Gateway qua mạng nội bộ Docker.
        var runningInContainer =
            string.Equals(
                Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"),
                "true",
                StringComparison.OrdinalIgnoreCase);

        if (runningInContainer)
        {
            return "http://apigateway:8080/hubs/bookingHub";
        }

        // Web chạy trực tiếp trên máy Windows.
        return "http://localhost:5000/hubs/bookingHub";
    }

    private async Task DisposeConnectionAsync()
    {
        var connection = _connection;
        _connection = null;

        if (connection is not null)
        {
            await connection.DisposeAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;
        await DisposeConnectionAsync();
    }
}