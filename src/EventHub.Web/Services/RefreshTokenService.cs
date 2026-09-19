using EventHub.Web.Models.Auth;

namespace EventHub.Web.Services;

public class RefreshTokenService
{
    private readonly AuthApiService _authApi;
    private readonly AuthSessionService _authSession;

    public RefreshTokenService(
        AuthApiService authApi,
        AuthSessionService authSession)
    {
        _authApi = authApi;
        _authSession = authSession;
    }

    public async Task<bool> TryRefreshAsync()
    {
        var refreshToken = await _authSession.GetRefreshTokenAsync();

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return false;
        }

        var result = await _authApi.RefreshTokenAsync(refreshToken);

        if (!result.Success || result.Data == null)
        {
            return false;
        }

        await _authSession.UpdateAccessTokenAsync(
            result.Data.AccessToken);

        return true;
    }
}