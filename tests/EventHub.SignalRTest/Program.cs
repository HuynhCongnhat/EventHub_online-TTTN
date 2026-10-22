using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;
using System.Text.Json.Serialization;

var eventId = Guid.Parse("9465738a-570c-4337-96cd-a42be77fa653");
var hubUrl = "http://localhost:5003/hubs/bookingHub";

var connection = new HubConnectionBuilder()
.WithUrl(hubUrl)
.WithAutomaticReconnect()
.Build();

connection.On<JsonElement>("SeatStatusChanged", seat =>
{
    Console.WriteLine();
    Console.WriteLine("=== NHẬN CẬP NHẬT TRẠNG THÁI GHẾ ===");
    Console.WriteLine(JsonSerializer.Serialize(
    seat,
    new JsonSerializerOptions { WriteIndented = true }));
    Console.WriteLine("===================================");
});

connection.Reconnecting += error =>
{
    Console.WriteLine($"Đang kết nối lại: {error?.Message}");
    return Task.CompletedTask;
};

connection.Reconnected += async connectionId =>
{
    Console.WriteLine($"Đã kết nối lại: {connectionId}");
    await connection.InvokeAsync("JoinEventGroup", eventId);
    Console.WriteLine($"Đã tham gia lại nhóm sự kiện: {eventId}");
};

try
{
    await connection.StartAsync();
    Console.WriteLine("Kết nối SignalR thành công.");


    await connection.InvokeAsync("JoinEventGroup", eventId);
        Console.WriteLine($"Đã tham gia nhóm sự kiện: {eventId}");

        Console.WriteLine();
        Console.WriteLine("Đang chờ sự kiện SeatStatusChanged...");
        Console.WriteLine("Nhấn Ctrl+C để kết thúc.");

        await Task.Delay(Timeout.Infinite);


}
catch (Exception ex)
{
    Console.WriteLine($"Lỗi SignalR: {ex.Message}");
}
finally
{
    await connection.DisposeAsync();
}
