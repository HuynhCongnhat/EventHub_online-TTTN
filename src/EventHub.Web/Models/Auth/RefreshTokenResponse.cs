namespace EventHub.Web.Models.Auth
{
    public class RefreshTokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;

        public Guid UserId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
