using System.Net.Http.Json;
using EventHub.Web.Models.Category;
using EventHub.Web.Models.Event;
using EventHub.Web.Models.Location;

namespace EventHub.Web.Services;

public class EventApiService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public EventApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient CreateClient()
    {
        return _httpClientFactory.CreateClient("AuthService");
    }


    // EVENTS

    public async Task<List<EventDto>> GetEventsAsync()
    {
        var client = CreateClient();

        var events = await client.GetFromJsonAsync<List<EventDto>>(
            "api/events");

        return events ?? new List<EventDto>();
    }

    public async Task<EventDto?> GetEventByIdAsync(Guid id)
    {
        var client = CreateClient();

        var response = await client.GetAsync($"api/events/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<EventDto>();
    }

    public async Task<EventDto?> CreateEventAsync(
        CreateEventRequest request)
    {
        var client = CreateClient();

        var response = await client.PostAsJsonAsync(
            "api/events",
            request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<EventDto>();
    }

    public async Task<bool> UpdateEventAsync(
        Guid id,
        UpdateEventRequest request)
    {
        var client = CreateClient();

        var response = await client.PutAsJsonAsync(
            $"api/events/{id}",
            request);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteEventAsync(Guid id)
    {
        var client = CreateClient();

        var response = await client.DeleteAsync(
            $"api/events/{id}");

        return response.IsSuccessStatusCode;
    }

    
    // CATEGORIES

    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        var client = CreateClient();

        var categories =
            await client.GetFromJsonAsync<List<CategoryDto>>(
                "api/categories");

        return categories ?? new List<CategoryDto>();
    }


    // LOCATIONS

    public async Task<List<LocationDto>> GetLocationsAsync()
    {
        var client = CreateClient();

        var locations =
            await client.GetFromJsonAsync<List<LocationDto>>(
                "api/locations");

        return locations ?? new List<LocationDto>();
    }
}