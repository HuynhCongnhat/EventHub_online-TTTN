namespace EventHub.Web.Models.Auth
{
    public class ForgotPasswordResponse
    {
        public string Message { get; set; } = string.Empty;

        public string? ResetToken { get; set; } 
    }
}
