namespace EventHub.Web.Models.Auth
{
    public class ResendVerificationCodeResponse
    {
        public string Message { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public DateTime VerificationCodeExpiresAt { get; set; }
    }
}
