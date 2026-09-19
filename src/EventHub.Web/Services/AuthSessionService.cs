using System.Text.Json;
using EventHub.Web.Models.Auth;
using Microsoft.JSInterop;

using LoginResponseModel = EventHub.Web.Models.Auth.LoginResponse;

namespace EventHub.Web.Services;

public class AuthSessionService
{
    private readonly IJSRuntime _js;

    private const string AccessTokenKey = "eventhub_access_token";
    private const string RefreshTokenKey = "eventhub_refresh_token";
    private const string UserKey = "eventhub_user";

    public AuthSessionService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task SetSessionAsync(LoginResponseModel data)
    {
        // Lưu Access Token
        await _js.InvokeVoidAsync(
            "sessionStorage.setItem",
            AccessTokenKey,
            data.AccessToken);

        // Lưu Refresh Token
        await _js.InvokeVoidAsync(
            "sessionStorage.setItem",
            RefreshTokenKey,
            data.RefreshToken);

        // Lưu thông tin User
        var userJson = JsonSerializer.Serialize(data);

        await _js.InvokeVoidAsync(
            "sessionStorage.setItem",
            UserKey,
            userJson);
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _js.InvokeAsync<string?>(
            "sessionStorage.getItem",
            AccessTokenKey);
    }

    public async Task<string?> GetRefreshTokenAsync()
    {
        return await _js.InvokeAsync<string?>(
            "sessionStorage.getItem",
            RefreshTokenKey);
    }

    public async Task UpdateAccessTokenAsync(string accessToken)
    {
        await _js.InvokeVoidAsync(
            "sessionStorage.setItem",
            AccessTokenKey,
            accessToken);
    }

    public async Task<LoginResponseModel?> GetUserAsync()
    {
        var userJson = await _js.InvokeAsync<string?>(
            "sessionStorage.getItem",
            UserKey);

        if (string.IsNullOrWhiteSpace(userJson))
        {
            return null;
        }

        return JsonSerializer.Deserialize<LoginResponseModel>(userJson);
    }

    public async Task<bool> IsLoggedInAsync()
    {
        var token = await GetTokenAsync();

        return !string.IsNullOrWhiteSpace(token);
    }

    public async Task ClearSessionAsync()
    {
        // Xóa Access Token
        await _js.InvokeVoidAsync(
            "sessionStorage.removeItem",
            AccessTokenKey);

        // Xóa Refresh Token
        await _js.InvokeVoidAsync(
            "sessionStorage.removeItem",
            RefreshTokenKey);

        // Xóa thông tin User
        await _js.InvokeVoidAsync(
            "sessionStorage.removeItem",
            UserKey);
    }
}