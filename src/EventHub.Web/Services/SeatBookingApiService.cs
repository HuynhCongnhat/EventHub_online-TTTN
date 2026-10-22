using System.Net.Http.Json;
using EventHub.Web.Models.Booking;

namespace EventHub.Web.Services;

public class SeatBookingApiService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public SeatBookingApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient CreateClient()
    {
        return _httpClientFactory.CreateClient("AuthService");
    }

    public async Task<List<SeatStatusChangedDto>> GetSeatsAsync(
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var client = CreateClient();

        var seats = await client.GetFromJsonAsync<List<SeatStatusChangedDto>>(
            $"api/seats/event/{eventId}",
            cancellationToken);

        return seats ?? new List<SeatStatusChangedDto>();
    }

    public async Task<SeatStatusChangedDto> HoldSeatAsync(
        Guid eventId,
        Guid seatId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var client = CreateClient();

        var response = await client.PostAsJsonAsync(
            "api/seats/hold",
            new
            {
                EventId = eventId,
                SeatId = seatId,
                UserId = userId
            },
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var message = await response.Content.ReadAsStringAsync(
                cancellationToken);

            throw new HttpRequestException(
                $"Không thể giữ ghế ({(int)response.StatusCode}): {message}",
                null,
                response.StatusCode);
        }

        return await response.Content.ReadFromJsonAsync<SeatStatusChangedDto>(
            cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException(
                "BookingService không trả về thông tin ghế.");
    }

    public async Task<SeatStatusChangedDto> ReleaseSeatAsync(
        Guid seatId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var client = CreateClient();

        var response = await client.PostAsync(
            $"api/seats/{seatId}/release?userId={userId}",
            content: null,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var message = await response.Content.ReadAsStringAsync(
                cancellationToken);

            throw new HttpRequestException(
                $"Không thể giải phóng ghế ({(int)response.StatusCode}): {message}",
                null,
                response.StatusCode);
        }

        return await response.Content.ReadFromJsonAsync<SeatStatusChangedDto>(
            cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException(
                "BookingService không trả về trạng thái ghế.");
    }
}
