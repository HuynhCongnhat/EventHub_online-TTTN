namespace EventHub.AuthService.Models
{
    public class EmailVerificationToken
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; } = null!;

        //mã otp 6 chữ số
        public string Code { get; set; } = string.Empty;

        //otp hết hạn
        public DateTime ExpiresAt { get; set; }

        //otp đc su dung chua
        public bool IsUsed { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
