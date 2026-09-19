using System.IdentityModel.Tokens.Jwt;

namespace EventHub.Web.Services;

public class AuthTokenRefreshService : IAsyncDisposable
{
    private readonly AuthSessionService _authSession;
    private readonly CustomAuthenticationStateProvider _authStateProvider;

    private PeriodicTimer? _timer;
    private CancellationTokenSource? _cts;
    private Task? _refreshTask;

    public AuthTokenRefreshService(
        AuthSessionService authSession,
        CustomAuthenticationStateProvider authStateProvider)
    {
        _authSession = authSession;
        _authStateProvider = authStateProvider;
    }

    public void Start()
    {
        if (_refreshTask != null)
        {
            return;
        }

        _cts = new CancellationTokenSource();

        _timer = new PeriodicTimer(
            TimeSpan.FromMinutes(1));

        _refreshTask = RefreshLoopAsync(_cts.Token);
    }

    private async Task RefreshLoopAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            while (_timer != null &&
                   await _timer.WaitForNextTickAsync(cancellationToken))
            {
                var accessToken =
                    await _authSession.GetTokenAsync();

                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    continue;
                }

                if (IsTokenExpiringSoon(accessToken))
                {
                    await _authStateProvider.TryRefreshTokenAsync();
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Service bị dừng bình thường.
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"AuthTokenRefreshService error: {ex.Message}");
        }
    }

    private static bool IsTokenExpiringSoon(
        string accessToken)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();

            var token = handler.ReadJwtToken(accessToken);

            var expiration = token.ValidTo;

            var remaining =
                expiration - DateTime.UtcNow;

            return remaining <= TimeSpan.FromMinutes(5);
        }
        catch
        {
            return false;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_cts != null)
        {
            await _cts.CancelAsync();
            _cts.Dispose();
        }

        _timer?.Dispose();

        if (_refreshTask != null)
        {
            try
            {
                await _refreshTask;
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}