using System.Security.Claims;
using EventHub.Web.Models.Auth;
using Microsoft.AspNetCore.Components.Authorization;

namespace EventHub.Web.Services;

public class CustomAuthenticationStateProvider
    : AuthenticationStateProvider
{
    private readonly AuthSessionService _authSession;
    private readonly RefreshTokenService _refreshTokenService;

    private ClaimsPrincipal _currentUser =
        new ClaimsPrincipal(new ClaimsIdentity());

    private bool _initialized = false;

    public CustomAuthenticationStateProvider(
        AuthSessionService authSession, RefreshTokenService refreshTokenService)
    {
        _authSession = authSession;
        _refreshTokenService = refreshTokenService;
    }

    public override Task<AuthenticationState>
        GetAuthenticationStateAsync()
    {
        return Task.FromResult(
            new AuthenticationState(_currentUser));
    }

    public async Task InitializeAsync()
    {
        Console.WriteLine("=== InitializeAsync START ===");

        if (_initialized)
        {
            Console.WriteLine("InitializeAsync: already initialized.");
            return;
        }

        _initialized = true;

        var user = await _authSession.GetUserAsync();

        if (user == null)
        {
            Console.WriteLine("InitializeAsync: USER IS NULL");

            _currentUser =
                new ClaimsPrincipal(
                    new ClaimsIdentity());
        }
        else
        {
            Console.WriteLine("InitializeAsync: USER FOUND");
            Console.WriteLine($"UserId: {user.UserId}");
            Console.WriteLine($"Email: {user.Email}");
            Console.WriteLine($"FullName: {user.FullName}");
            Console.WriteLine($"Role: {user.Role}");

            _currentUser = CreateClaimsPrincipal(user);

            Console.WriteLine(
                $"Claims Name: {_currentUser.Identity?.Name}");

            Console.WriteLine(
                $"Claims Role: {_currentUser.FindFirst(ClaimTypes.Role)?.Value}");
        }

        NotifyAuthenticationStateChanged(
            Task.FromResult(
                new AuthenticationState(_currentUser)));

        Console.WriteLine("=== InitializeAsync END ===");
    }

    public void NotifyUserAuthentication(
        LoginResponse user)
    {
        _currentUser = CreateClaimsPrincipal(user);

        NotifyAuthenticationStateChanged(
            Task.FromResult(
                new AuthenticationState(_currentUser)));
    }
    public async Task<bool> TryRefreshTokenAsync()
    {
        var success = await _refreshTokenService.TryRefreshAsync();

        if (!success)
        {
            await NotifyUserLogout();
            return false;
        }

        var user = await _authSession.GetUserAsync();

        if (user == null)
        {
            await NotifyUserLogout();
            return false;
        }

        _currentUser = CreateClaimsPrincipal(user);

        NotifyAuthenticationStateChanged(
            Task.FromResult(
                new AuthenticationState(_currentUser)));

        return true;
    }

    public async Task NotifyUserLogout()
    {
        await _authSession.ClearSessionAsync();

        _currentUser =
            new ClaimsPrincipal(
                new ClaimsIdentity());

        _initialized = false;

        NotifyAuthenticationStateChanged(
            Task.FromResult(
                new AuthenticationState(_currentUser)));
    }

    private static ClaimsPrincipal CreateClaimsPrincipal(
        LoginResponse user)
    {
        var claims = new List<Claim>
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.UserId.ToString()),

            new Claim(
                ClaimTypes.Email,
                user.Email),

            new Claim(
                ClaimTypes.Name,
                user.FullName),

            new Claim(
                ClaimTypes.Role,
                user.Role)
        };

        var identity = new ClaimsIdentity(
            claims,
            "EventHubAuth");

        return new ClaimsPrincipal(identity);
    }
}