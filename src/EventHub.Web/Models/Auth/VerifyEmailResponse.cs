namespace EventHub.Web.Models.Auth
{
    public class VerifyEmailResponse
    {
        public string Message { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool EmailVerified { get; set; }
    }
}
